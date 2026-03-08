using Microsoft.Data.Sqlite;
using NLog;
using EmbeddedUI.UI.Models;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace EmbeddedUI.UI.Services
{
    public class LocalDataStoreService
    {
        private readonly SqliteConnection _connection = new SqliteConnection("Data Source=DB/readings.db");
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public LocalDataStoreService()
        {
            SQLitePCL.Batteries.Init();
            _connection.Open();
        }

        public void InsertChartEntry(ChartEntry chart)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO charts (categories, interval, is_continuous, chart_index, date_from, date_to) " +
                "VALUES ($categories, $interval, $is_cont, $chart_index, $date_from, $date_to);";
            command.Parameters.AddWithValue("$categories", string.Join(",", chart.Categories));
            command.Parameters.AddWithValue("$interval", chart.Interval);
            command.Parameters.AddWithValue("$is_cont", chart.IsContinuous);
            command.Parameters.AddWithValue("$chart_index", chart.Index);
            command.Parameters.AddWithValue("$date_from", chart.From);
            command.Parameters.AddWithValue("$date_to", chart.To);

            command.ExecuteNonQuery();
        }

        public List<ChartEntry> GetCharts()
        {
            var result = new List<ChartEntry>();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT categories, interval, is_continuous, chart_index, date_from, date_to FROM charts;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new ChartEntry()
                {
                    Categories = reader.GetString(0).Split(",").ToList(),
                    Interval = (ChartIntervalEnum)reader.GetInt32(1),
                    IsContinuous = reader.GetBoolean(2),
                    Index = reader.GetInt32(3),
                    From = reader.GetDateTime(4),
                    To = reader.GetDateTime(5)
                };

                result.Add(row);
            }

            return result;
        }

        public void InsertTemperatureEntry(double temperatureValue)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO temperature (timestamp, temp) VALUES ($timestamp, $temp);";
            command.Parameters.AddWithValue("$timestamp", DateTime.Now);
            command.Parameters.AddWithValue("$temp", temperatureValue);
            command.ExecuteNonQuery();
        }

        public void InsertTemperatureEntry(ValueEntry temperatureEntry)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO temperature (timestamp, temp) VALUES ($timestamp, $temp);";
            command.Parameters.AddWithValue("$timestamp", temperatureEntry.TimeStamp);
            command.Parameters.AddWithValue("$temp", temperatureEntry.Value);
            command.ExecuteNonQuery();
        }

        public List<ValueEntry> GetTemperatures(DateTime from, DateTime to)
        {
            var result = new List<ValueEntry>();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT id, timestamp, temp FROM temperature WHERE timestamp >= $from AND timestamp <= $to;";
            command.Parameters.AddWithValue("$from", from);
            command.Parameters.AddWithValue("$to", to);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new ValueEntry()
                {
                    Id = reader.GetInt32(0),
                    TimeStamp = reader.GetDateTime(1),
                    Value = reader.GetDouble(2)
                };

                result.Add(row);
            }

            return result;
        }

        public void InsertPressureEntry(double pressureValue)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO pressure (timestamp, pressure) VALUES ($timestamp, $pressure);";
            command.Parameters.AddWithValue("$timestamp", DateTime.Now);
            command.Parameters.AddWithValue("$pressure", pressureValue);
            command.ExecuteNonQuery();
        }

        public void InsertPressureEntry(ValueEntry pressureEntry)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO pressure (timestamp, pressure) VALUES ($timestamp, $pressure);";
            command.Parameters.AddWithValue("$timestamp", pressureEntry.TimeStamp);
            command.Parameters.AddWithValue("$pressure", pressureEntry.Value);
            command.ExecuteNonQuery();
        }

        public List<ValueEntry> GetPressures(DateTime from, DateTime to)
        {
            var result = new List<ValueEntry>();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT id, timestamp, pressure FROM pressure WHERE timestamp >= $from AND timestamp <= $to;";
            command.Parameters.AddWithValue("$from", from);
            command.Parameters.AddWithValue("$to", to);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new ValueEntry()
                {
                    Id = reader.GetInt32(0),
                    TimeStamp = reader.GetDateTime(1),
                    Value = reader.GetDouble(2)
                };

                result.Add(row);
            }

            return result;
        }

        public void InsertHumidityEntry(double humidityValue)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO humidity (timestamp, humidity) VALUES ($timestamp, $humidity);";
            command.Parameters.AddWithValue("$timestamp", DateTime.Now);
            command.Parameters.AddWithValue("$humidity", humidityValue);
            command.ExecuteNonQuery();
        }

        public void InsertHumidityEntry(ValueEntry humidityEntry)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO humidity (timestamp, humidity) VALUES ($timestamp, $humidity);";
            command.Parameters.AddWithValue("$timestamp", humidityEntry.TimeStamp);
            command.Parameters.AddWithValue("$humidity", humidityEntry.Value);
            command.ExecuteNonQuery();
        }

        public List<ValueEntry> GetHumidities(DateTime from, DateTime to)
        {
            var result = new List<ValueEntry>();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT id, timestamp, temp FROM humidity WHERE timestamp >= $from AND timestamp <= $to;";
            command.Parameters.AddWithValue("$from", from);
            command.Parameters.AddWithValue("$to", to);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new ValueEntry()
                {
                    Id = reader.GetInt32(0),
                    TimeStamp = reader.GetDateTime(1),
                    Value = reader.GetDouble(2)
                };

                result.Add(row);
            }

            return result;
        }
    }
}
