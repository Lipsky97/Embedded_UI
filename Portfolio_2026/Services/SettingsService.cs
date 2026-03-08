using EmbeddedUI.UI.Models;
using System.Xml.Linq;

namespace EmbeddedUI.UI.Services
{
    public class SettingsService
    {
        public TemperatureUnits TemperatureUnit { get; set; } = TemperatureUnits.Celsius;
        public AltitudeUnits AltitudeUnit { get; set; } = AltitudeUnits.Meters;
        public PressureUnits PressureUnit { get; set; } = PressureUnits.Hpa;
        private readonly string _settingsFilePath;
        public SettingsService()
        {
            _settingsFilePath = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName, "config", "settings.xml");
            //SaveSettings();
            LoadSettings();
        }

        public void LoadSettings()
        {
            var doc = XDocument.Load(_settingsFilePath);
            var root = doc.Element("Settings");
            TemperatureUnit = Enum.TryParse(root.Element("TemperatureUnit")?.Value, out TemperatureUnits tempUnit) ? tempUnit : TemperatureUnits.Celsius;
            AltitudeUnit = Enum.TryParse(root.Element("HumidityUnit")?.Value, out AltitudeUnits altitudeUnit) ? altitudeUnit : AltitudeUnits.Meters;
            PressureUnit = Enum.TryParse(root.Element("PressureUnit")?.Value, out PressureUnits pressureUnit) ? pressureUnit : PressureUnits.Hpa;
        }

        public void SaveSettings()
        {
            var settingsXml = new XElement("Settings",
                new XElement("TemperatureUnit", TemperatureUnit),
                new XElement("HumidityUnit", AltitudeUnit),
                new XElement("PressureUnit", PressureUnit)
            );

            settingsXml.Save(_settingsFilePath);
        }
    }
}
