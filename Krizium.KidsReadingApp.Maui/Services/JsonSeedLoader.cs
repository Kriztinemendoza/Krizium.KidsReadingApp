using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Krizium.KidsReadingApp.Data;

namespace Krizium.KidsReadingApp.Maui.Services
{
    public static class JsonSeedLoader
    {
        public static async Task<SeedDataJson?> LoadSeedDataAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("Assets/seed_data.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(false); 

            return JsonSerializer.Deserialize<SeedDataJson>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
