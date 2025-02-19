﻿using System.Text.Json.Serialization;

namespace MauiClientApp.Models
{
    public class WeatherForecast
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("temperatureC")]
        public int TemperatureC { get; set; }

        [JsonPropertyName("temperatureF")]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [JsonPropertyName("summary")]
        public string Summary { get; set; }
    }
}
