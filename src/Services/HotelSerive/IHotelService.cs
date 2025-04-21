using Konoha.Models.HotelService;

namespace Konoha.Services.HotelService
{
    public interface IHotelService
    {
        Task<string> GetDestinationIdAsync(string city);
        Task<List<CustomHotelModel>> SearchHotelsAsync(string destinationId, string checkInDate, string checkOutDate);
    }
}