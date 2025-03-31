using System;
using System.Text.Json;
using Konoha.Common;
using Konoha.Models;
using RestSharp;

namespace Konoha.Services.Products;

public class ProductsClient : IProductClient
{
    private readonly ILogger<ProductsClient> _logger;
    private readonly IUserClient _userClient;
    private  readonly IMongoDbService<UserDetails> _mongoDbService;



    public ProductsClient(IUserClient userClient,ILogger<ProductsClient> logger,IMongoDbService<UserDetails> mongoDbService)
    {
        _logger = logger;
        _userClient=userClient;
        _mongoDbService=mongoDbService;
    }

    public async Task AddFavProductAsync(List<FavouriteProductInteraction> body,string userId)
    {   
        try{
                var user=await _userClient.GetUserByIdAsync(userId);
                foreach(var data in body){
                    user.Favourites.Add(data);
                    }
             
                await _mongoDbService.UpdateAsync(user.Id,user);
        }catch(KonohaException ex){
            _logger.LogError(ex.Message);
            throw;
        }
        catch(Exception e){
            throw new KonohaException(KonohaException.InternalServerError,"error adding product to fav");
        }
        
    }

    public async Task<List<ProductResponse>> SearchProductsAsync(string query)
    {
        var formattedQuery = string.Join("+", query.Split(' '));
        var client = new RestClient("https://real-time-amazon-data.p.rapidapi.com");
        client.AddDefaultHeader(
            "x-rapidapi-key",
            "3c2f1d4a9emsh107a34a4497f281p1bd7abjsn50e2fc2df716"
        );
        client.AddDefaultHeader("x-rapidapi-host", "real-time-amazon-data.p.rapidapi.com");

        var request = new RestRequest(
            $"search?query={formattedQuery}&page=1&country=IN&sort_by=LOWEST_PRICE&product_condition=ALL&is_prime=false&deals_and_discounts=NONE",
            Method.Get
        );

        try
        {
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var amazonResponse = JsonSerializer.Deserialize<AmazonResponse>(response.Content);
                if (amazonResponse != null)
                {
                    var products = new List<ProductResponse>();
                    foreach (var product in amazonResponse.Data.Products.Take(15))
                    {
                        products.Add(
                            new ProductResponse
                            {
                                Title = product.ProductTitle,
                                Price = product.ProductPrice,
                                Rating = product.ProductStarRating,
                                ImageUrl = product.ProductPhoto,
                                Url = product.ProductUrl,
                            }
                        );
                    }
                    return products;
                }
                else
                {
                    throw new KonohaException(KonohaException.NotFound, "Product not found");
                }
            }
            else
            {
                _logger.LogError("Error retrieving product: {ErrorMessage}", response.ErrorMessage);
                throw new KonohaException(
                    KonohaException.InternalServerError,
                    "Internal Server error"
                );
            }
        }
        catch (KonohaException ex)
        {
            _logger.LogError("KonohaException: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred: {Message}", ex.Message);
            throw;
        }
    }
}
