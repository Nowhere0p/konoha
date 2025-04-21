using Konoha.Services.HotelService;
using RestSharp;
using System.Text.Json;
using System.Threading.Tasks;
using Konoha.Models.HotelService;

namespace Konoha.Services.HotelService;
    public class HotelService : IHotelService
    {
        private readonly string _apiKey = "3c2f1d4a9emsh107a34a4497f281p1bd7abjsn50e2fc2df716";
        private readonly string _host = "booking-com15.p.rapidapi.com";
        private readonly RestClient _client;

        public HotelService()
        {
            _client = new RestClient();
        }

        public async Task<string?> GetDestinationIdAsync(string city)
        {
            var request = new RestRequest($"https://{_host}/api/v1/hotels/searchDestination")
                .AddQueryParameter("query", city)
                .AddHeader("X-RapidAPI-Key", _apiKey)
                .AddHeader("X-RapidAPI-Host", _host);

            var response = await _client.ExecuteGetAsync(request);
            if (!response.IsSuccessful) return null;

            var result = JsonSerializer.Deserialize<DestinationSearchResponse>(response.Content);
            return result?.Data?.FirstOrDefault()?.DestId;
        }

        public async Task<List<CustomHotelModel>> SearchHotelsAsync(string destId, string checkInDate, string checkOutDate)
        {
            var request = new RestRequest($"https://{_host}/api/v1/hotels/searchHotels")
                .AddQueryParameter("dest_id", destId)
                .AddQueryParameter("search_type", "CITY")
                .AddQueryParameter("arrival_date", checkInDate)
                .AddQueryParameter("departure_date", checkOutDate)
                .AddQueryParameter("units", "metric")
                .AddQueryParameter("currency_code", "INR")
                .AddHeader("X-RapidAPI-Key", _apiKey)
                .AddHeader("X-RapidAPI-Host", _host);
            var parameters = string.Join("&", request.Parameters.Select(p => $"{p.Name}={p.Value}"));
          
            var response = await _client.ExecuteGetAsync(request);
            if (!response.IsSuccessful) return new List<CustomHotelModel>();

            var result = JsonSerializer.Deserialize<HotelResponse>(response.Content);
            if (result?.Data == null) return new List<CustomHotelModel>();

            return result.Data.Hotels.Take(15).Select(hotel => FromHotel(hotel)).ToList();
           

          
        }
        private  CustomHotelModel FromHotel(Hotel hotel)
{
    return new CustomHotelModel
    {
        HotelName = hotel.Property.Name,
        PriceBreakdown = hotel.Property.PriceBreakdown,
        ImageUrl = hotel.Property.PhotoUrls.FirstOrDefault() ?? string.Empty,
        BookingUrl= $"https://www.booking.com/hotel/in/{GenerateUrlSlug(hotel.Property.Name)}.en-gb.html"};
}
   private string GenerateUrlSlug(string hotelName)
    {
        if (string.IsNullOrEmpty(hotelName))
            return string.Empty;
        return hotelName.ToLower()
                        .Replace(" ", "-")
                        .Replace(",", "")
                        .Replace(".", "");
    }

    }
