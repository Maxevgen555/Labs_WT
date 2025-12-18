// Labs.UI/ViewComponents/CartViewComponent.cs
using Labs.Domain.Models;
using Labs.UI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Labs.UI.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(cart);
        }
    }
}
