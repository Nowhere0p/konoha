namespace Konoha.Services.Products
{
    public interface IProductClient
    {
        Task<List<ProductResponse>> SearchProductsAsync(string query);
    }
}
