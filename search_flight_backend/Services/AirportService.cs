using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace search_flight_backend.Services
{
    public class AirportService : IAirportService
    {
        private readonly IMemoryCache _cache;
        private readonly Dictionary<string, List<string>> _routes;
        private static readonly List<string> _origins = new() { "Chennai", "Banglore", "Delhi", "Kolkata", "Mumbai" };

        public AirportService(IMemoryCache cache)
        {
            _cache = cache;
            _routes = LoadRoutesFromJson();
        }

        private Dictionary<string, List<string>> LoadRoutesFromJson()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "routes.json");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Routes data file not found.", filePath);

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        }

        public Task<List<string>> GetOriginAirportsAsync()
        {
            return Task.FromResult(_cache.GetOrCreate("originAirports", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return _origins;
            })!);
        }

        public Task<List<string>?> GetDestinationAirportsAsync(string originAirport)
        {
            if (string.IsNullOrEmpty(originAirport)) return Task.FromResult<List<string>?>(null);
            var cacheKey = $"destinations_{originAirport.ToLower()}";
            return Task.FromResult(_cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return _routes.TryGetValue(originAirport, out var dest) ? dest : null;
            }));
        }
    }
}