namespace Konoha.Services.Products
{
    public interface IProductClient
    {
        Task<List<ProductResponse>> SearchProductsAsync(string query);
        Task AddFavProductAsync(List<FavouriteProductInteraction> body,string userId);
    }
}
