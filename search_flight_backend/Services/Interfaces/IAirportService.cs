// File: Services/IAirportService.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace search_flight_backend.Services
{
    public interface IAirportService
    {
        Task<List<string>> GetOriginAirportsAsync();
        Task<List<string>?> GetDestinationAirportsAsync(string originAirport);
    }
}