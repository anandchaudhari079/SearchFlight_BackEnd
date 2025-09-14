// File: Services/AirportService.cs
using Microsoft.Extensions.Caching.Memory;

namespace search_flight_backend.Services
{
    public class AirportService : IAirportService
    {
        private readonly IMemoryCache _cache;
        private static readonly List<string> _origins = new() { "Chennai", "Banglore", "Delhi", "Kolkata", "Mumbai" };
        private static readonly Dictionary<string, List<string>> _routes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Mumbai", new List<string> { "Chennai", "Banglore", "Delhi", "Kolkata" } },
            { "Chennai", new List<string> { "Delhi", "Banglore", "Mumbai", "Kolkata" } },
            { "Banglore", new List<string> { "Delhi", "Chennai", "Mumbai", "Kolkata" } },
            { "Delhi", new List<string> { "Mumbai", "Banglore", "Chennai", "Kolkata" } },
            { "Kolkata", new List<string> { "Delhi", "Banglore", "Mumbai", "Chennai" } }
        };

        public AirportService(IMemoryCache cache)
        {
            _cache = cache;
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