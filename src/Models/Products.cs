using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AmazonProduct
{
    [JsonPropertyName("asin")]
    public string Asin { get; set; } // Amazon Standard Identification Number

    [JsonPropertyName("product_title")]
    public string ProductTitle { get; set; } // Product title

    [JsonPropertyName("product_price")]
    public string ProductPrice { get; set; } // Product price

    [JsonPropertyName("product_original_price")]
    public string ProductOriginalPrice { get; set; } // Original price

    [JsonPropertyName("currency")]
    public string Currency { get; set; } // Currency

    [JsonPropertyName("product_star_rating")]
    public string ProductStarRating { get; set; } // Star rating

    [JsonPropertyName("product_num_ratings")]
    public int ProductNumRatings { get; set; } // Number of ratings

    [JsonPropertyName("product_url")]
    public string ProductUrl { get; set; } // Product URL

    [JsonPropertyName("product_photo")]
    public string ProductPhoto { get; set; } // Product photo URL

    [JsonPropertyName("product_num_offers")]
    public int ProductNumOffers { get; set; } // Number of offers

    [JsonPropertyName("product_minimum_offer_price")]
    public string ProductMinimumOfferPrice { get; set; } // Minimum offer price

    [JsonPropertyName("is_best_seller")]
    public bool IsBestSeller { get; set; } // Is best seller

    [JsonPropertyName("is_amazon_choice")]
    public bool IsAmazonChoice { get; set; } // Is Amazon's choice

    [JsonPropertyName("is_prime")]
    public bool IsPrime { get; set; } // Is Prime

    [JsonPropertyName("climate_pledge_friendly")]
    public bool ClimatePledgeFriendly { get; set; } // Climate pledge friendly

    [JsonPropertyName("sales_volume")]
    public string SalesVolume { get; set; } // Sales volume

    [JsonPropertyName("delivery")]
    public string Delivery { get; set; } // Delivery information

    [JsonPropertyName("has_variations")]
    public bool HasVariations { get; set; } // Has variations

    [JsonPropertyName("product_badge")]
    public string ProductBadge { get; set; } // Product badge
}

public class AmazonResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } // Response status

    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } // Request ID

    [JsonPropertyName("parameters")]
    public AmazonParameters Parameters { get; set; } // Request parameters

    [JsonPropertyName("data")]
    public AmazonData Data { get; set; } // Response data
}

public class AmazonParameters
{
    [JsonPropertyName("query")]
    public string Query { get; set; } // Search query

    [JsonPropertyName("country")]
    public string Country { get; set; } // Country

    [JsonPropertyName("sort_by")]
    public string SortBy { get; set; } // Sort by

    [JsonPropertyName("page")]
    public int Page { get; set; } // Page number

    [JsonPropertyName("is_prime")]
    public bool IsPrime { get; set; } // Is Prime
}

public class AmazonData
{
    [JsonPropertyName("total_products")]
    public int TotalProducts { get; set; } // Total number of products

    [JsonPropertyName("country")]
    public string Country { get; set; } // Country

    [JsonPropertyName("domain")]
    public string Domain { get; set; } // Domain

    [JsonPropertyName("products")]
    public List<AmazonProduct> Products { get; set; } // List of products
}

public class ProductResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } // Product title

    [JsonPropertyName("price")]
    public string Price { get; set; } // Product price

    [JsonPropertyName("rating")]
    public string Rating { get; set; } // Product rating

    [JsonPropertyName("url")]
    public string Url { get; set; } // Product URL

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } // Product photo URL
}
