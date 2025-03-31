using System.Collections.Generic;
using Konoha.common;
using Konoha.Common;
using Konoha.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Konoha.Controllers
{
    [Route("api/v1.0/products")]
    [ApiController]
    public class ProductsController(
        IProductClient productClient,
        ILogger<ProductsController> logger
    ) : ControllerBase
    {
        private readonly IProductClient _productService = productClient;
        private readonly ILogger<ProductsController> _logger = logger;

        [AllowAnonymous]
        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchProducts(string query)
        {
            try
            {
                return Ok(await _productService.SearchProductsAsync(query));
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while searching for products.", ex);
                throw new KonohaException(
                    KonohaException.InternalServerError,
                    "An error occurred while searching for products."
                );
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "USER")]

         [HttpGet("favourite")]
        public async Task<IActionResult> AddFavourite([FromBody] List<FavouriteProductInteraction> interaction )
        {
            try
            {   
                  var userId = User.FindFirst(CustomClaimTypes.UserId)?.Value;
                    await _productService.AddFavProductAsync(interaction,userId);
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while searching for products.", ex);
                throw new KonohaException(
                    KonohaException.InternalServerError,
                    "An error occurred while searching for products."
                );
            }
        }

    }
}
