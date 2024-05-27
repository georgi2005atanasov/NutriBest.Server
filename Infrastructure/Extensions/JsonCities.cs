﻿namespace NutriBest.Server.Infrastructure.Extensions
{
    using Newtonsoft.Json;

    public class JsonCities
    {
        [JsonProperty("city")]
        public string City { get; set; } = null!;

        [JsonProperty("lat")]
        public string Latitude { get; set; } = null!;

        [JsonProperty("lng")]
        public string Longitude { get; set; } = null!;
    }
}
