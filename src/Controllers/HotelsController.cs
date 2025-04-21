using Microsoft.AspNetCore.Mvc;
using Konoha.Services.HotelService;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Konoha.Controllers
{
    [ApiController]
    [Route("api/v1.0/hotels")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelController> _logger;

        public HotelController(IHotelService hotelService, ILogger<HotelController> logger)
        {
            _hotelService = hotelService;
            _logger = logger;
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> SearchHotels(
            [FromQuery] string city,
            [FromQuery] DateOnly checkIn,
            [FromQuery] DateOnly checkOut
            )
        {
            try
            {
                _logger.LogInformation("Starting hotel search for city: {city}, checkIn: {checkIn}, checkOut: {checkOut}", 
                    city, checkIn, checkOut);

                if (string.IsNullOrEmpty(city))
                {
                    _logger.LogWarning("City parameter is null or empty");
                    return BadRequest("City is required.");
                }

                var destinationId = await _hotelService.GetDestinationIdAsync(city);
                if (string.IsNullOrEmpty(destinationId))
                {
                    _logger.LogWarning("Destination not found for city: {City}", city);
                    return NotFound($"Destination not found for city: {city}");
                }
                System.Console.WriteLine(destinationId);

                var hotelJson = await _hotelService.SearchHotelsAsync(
                    destinationId, 
                    checkIn.ToString("yyyy-MM-dd"), 
                    checkOut.ToString("yyyy-MM-dd"));
                System.Console.WriteLine(JsonSerializer.Serialize(hotelJson));
                if (hotelJson == null)
                {
                    _logger.LogWarning("No hotels found for city: {City}", city);
                    return NotFound($"No hotels found for city: {city}");
                }

                _logger.LogInformation("Successfully retrieved hotels for city: {City}", city);
                
                return Ok(hotelJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching hotels for city: {City}");
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }
    }
}
