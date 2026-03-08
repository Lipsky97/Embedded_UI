using EmbeddedUI.UI.Models;
using System.Xml.Linq;

namespace EmbeddedUI.UI.Services
{
    public class DataClientService
    {
        public bool Connected { get; set; }
        public event EventHandler? DeviceStatusChanged;
        public DeviceStatus DeviceStatus 
        {
            get
            {
                return _currentDeviceStatus;
            }
            set
            {
                if (value.Temperature != _currentDeviceStatus.Temperature ||
                    value.Humidity != _currentDeviceStatus.Humidity ||
                    value.Pressure != _currentDeviceStatus.Pressure)
                {
                    _currentDeviceStatus = value;
                    DeviceStatusChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private DeviceStatus _currentDeviceStatus = new DeviceStatus();

        private readonly SemaphoreSlim _readXmlMutex = new SemaphoreSlim(1, 1);
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        //private readonly HttpClient _httpClient = new HttpClient();
        private readonly LocalDataStoreService _dataStore = new LocalDataStoreService();
        private readonly SettingsService _settingsService = new SettingsService();
#if DEBUG
        private readonly string _url = "http://192.168.1.154:8080/xml";
#else
        private readonly string _url = "http://127.0.0.1:8080/xml";
#endif

        public DataClientService()
        {
            ConnectToDevice();
        }

        public void ConnectToDevice()
        {
            if (Connected)
            {
                return;
            }
            else
            {
                _logger.Info("Connecting to device...");
                Connected = true;
                StartPolling(_url);
            }
        }

        private void StartPolling(string url)
        {
            Task.Run(async () =>
            {
                try
                {
                    var connectRetries = 0;
                    while (Connected)
                    {
                        var queueResult = await _readXmlMutex.WaitAsync(30000);

                        try
                        {
                            string data = await GetData(url);

                            if (!string.IsNullOrEmpty(data))
                            {
                                connectRetries = 0;
                                ProcessXmlUpdate(data);
                            }
                            else
                            {
                                if (connectRetries > 5)
                                {
                                    Connected = false;
                                }
                                _logger.Warn($"Received empty data from device. ({connectRetries}/5)");
                                connectRetries++;
                            }

                            LogStatus();
                            await Task.Delay(1000);
                        }
                        finally
                        {
                            _readXmlMutex.Release();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while polling device.");
                    Connected = false;
                }
            });
        }

        private async Task<string> GetData(string url)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                response.Dispose();

                return responseData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while getting data from device.");
                return string.Empty;
            }
        }

        private void ProcessXmlUpdate(string data)
        {
            try
            {
                var doc = XDocument.Parse(data);

                var root = doc.Element("SensorReadings");
                var temp = root.Element("Temperature")?.Value;
                var humidity = root.Element("Humidity")?.Value;
                var pressure = root.Element("Pressure")?.Value;

                var newStatus = new DeviceStatus();

                if (double.TryParse(temp, out var tempValue))
                {
                    newStatus.Temperature = tempValue;
                }

                if (double.TryParse(humidity, out var humidityValue))
                {
                    newStatus.Humidity = humidityValue;
                }

                if (double.TryParse(pressure, out var pressureValue))
                {
                    newStatus.Pressure = pressureValue;
                }

                DeviceStatus = newStatus;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while processing XML data.");
            }
        }

        private void LogStatus()
        {
            _dataStore.InsertTemperatureEntry(_currentDeviceStatus.Temperature);
            _dataStore.InsertHumidityEntry(_currentDeviceStatus.Humidity);
            _dataStore.InsertPressureEntry(_currentDeviceStatus.Pressure);
        }

        public string GetTemperature()
        {
            if (_settingsService.TemperatureUnit == TemperatureUnits.Celsius)
            {
                return _currentDeviceStatus.Temperature.ToString("F2") + " °C";
            }
            else
            {
                var tempF = _currentDeviceStatus.Temperature * 9 / 5 + 32;
                return tempF.ToString("F2") + " °F";
            }
        }

        public string GetHumidity()
        {
            return _currentDeviceStatus.Humidity.ToString("F2") + "%";
        }

        public string GetPressure()
        {
            if (_settingsService.PressureUnit == PressureUnits.Hpa)
            {
                return _currentDeviceStatus.Pressure.ToString("F2") + " hPa";
            }
            else
            {
                return _currentDeviceStatus.Pressure.ToString("F2") + " mbar";
            }
        }

        public string GetAltitude()
        {
            var alt = CalculateAltitude();
            if (_settingsService.AltitudeUnit == AltitudeUnits.Feet)
            {
                alt *= 3.28084;
                return alt.ToString("F2") + " ft";
            }
            else
            {
                return alt.ToString("F2") + " m";
            }
        }

        private double CalculateAltitude()
        {
            double R = 287.05; 
            double G = 9.80665; 
            double PO = 101325;

            double P = _currentDeviceStatus.Pressure * 100; 
            double T = _currentDeviceStatus.Temperature + 273.15;

            double altitude = (R * T / G) * Math.Log(PO / P);
            return altitude;
        }
    }
}
