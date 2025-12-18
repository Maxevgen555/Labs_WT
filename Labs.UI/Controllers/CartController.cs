// Labs.UI/Controllers/CartController.cs
using Labs.Domain.Models;
using Labs.UI.Extensions;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Labs.UI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private Cart _cart;

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: CartController
        public ActionResult Index()
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(_cart.CartItems);
        }

        [Route("[controller]/add/{id:int}")]
        public async Task<ActionResult> Add(int id, string returnUrl)
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();

            try
            {
                var data = await _productService.GetProductByIdAsync(id);
                if (data.Success && data.Data != null)
                {
                    _cart.AddToCart(data.Data);
                    HttpContext.Session.Set<Cart>("cart", _cart);
                    TempData["SuccessMessage"] = $"Товар {data.Data.Name} добавлен в корзину";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при получении данных о товаре";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return Redirect(returnUrl);
        }
        [Route("[controller]/remove/{id:int}")]
        public ActionResult Remove(int id)
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            _cart.RemoveItems(id);
            HttpContext.Session.Set<Cart>("cart", _cart);
            return RedirectToAction("index");
        }
        [Route("[controller]/clear")]
        public ActionResult Clear()
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            _cart.ClearAll();
            HttpContext.Session.Set<Cart>("cart", _cart);
            TempData["SuccessMessage"] = "Корзина очищена";
            return RedirectToAction("Index");
        }

        [Route("[controller]/update/{id:int}/{qty:int}")]
        public ActionResult Update(int id, int qty)
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();

            if (_cart.CartItems.ContainsKey(id))
            {
                if (qty <= 0)
                {
                    _cart.RemoveItems(id);
                }
                else
                {
                    _cart.CartItems[id].Qty = qty;
                }
                HttpContext.Session.Set<Cart>("cart", _cart);
            }

            return RedirectToAction("Index");
        }
    }
}
