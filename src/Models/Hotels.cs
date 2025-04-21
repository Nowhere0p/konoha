using System.Text.Json.Serialization;

namespace Konoha.Models.HotelService;
public class DestinationSearchResponse
{
    [JsonPropertyName("data")]
    public List<DestinationData> Data { get; set; } = new();
}

public class DestinationData
{
    
    [JsonPropertyName("dest_id")]
    public string? DestId { get; set; }

}




//search hotels response
public class HotelResponse
{
 
    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public class Data
{
    [JsonPropertyName("hotels")]
    public List<Hotel> Hotels { get; set; }
}

public class Hotel
{
    [JsonPropertyName("accessibilityLabel")]
    public string AccessibilityLabel { get; set; }

    [JsonPropertyName("property")]
    public Property Property { get; set; }
}

public class Property
{


    [JsonPropertyName("photoUrls")]
    public List<string> PhotoUrls { get; set; }


    [JsonPropertyName("priceBreakdown")]
    public PriceBreakdown PriceBreakdown { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }


    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}



public class PriceBreakdown
{
    [JsonPropertyName("grossPrice")]
    public Price GrossPrice { get; set; }

}

public class Price
{
    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }
}
//my


public class CustomHotelModel
{
    [JsonPropertyName("hotel_name")]
    public string HotelName { get; set; }

    [JsonPropertyName("price_breakdown")]
    public PriceBreakdown PriceBreakdown { get; set; }

    [JsonPropertyName("url")]
    public string BookingUrl { get; set; }
    
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; }

 



}