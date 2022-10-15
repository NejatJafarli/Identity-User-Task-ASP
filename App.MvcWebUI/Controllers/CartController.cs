using App.Business.Abstract;
using App.Entities.Concrete;
using App.MvcWebUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.MvcWebUI.Controllers
{
    public class CartController : Controller
    {
        private ICartSessionService _cartSessionService;
        private ICartService _cartService;
        private IProductService _productService;

        public CartController(ICartSessionService cartSessionService, ICartService cartService, IProductService productService)
        {
            _cartSessionService = cartSessionService;
            _cartService = cartService;
            _productService = productService;
        }


        public IActionResult AddToCart(int productId)
        {
            var productToBeAdded=_productService.GetById(productId);

            var cart = _cartSessionService.GetCart();

            _cartService.AddToCart(cart, productToBeAdded);

            _cartSessionService.SetCart(cart);

            TempData.Add("message", String.Format("Your product, {0} was added successfully to cart!", productToBeAdded.ProductName));

            return RedirectToAction("Index", "Product");
        }


        public IActionResult List()
        {
            var cart=_cartSessionService.GetCart();
            CartListViewModel cartListViewModel = new CartListViewModel
            {
                Cart = cart
            };
            return View(cartListViewModel);
        }

        public IActionResult Remove(int productId)
        {
            var cart = _cartSessionService.GetCart();

            _cartService.RemoveFromCart(cart, productId);
            _cartSessionService.SetCart(cart);
            TempData.Add("message", "Your Product , {0} was removed successfully from cart!");

            return RedirectToAction("List");
        }


        public IActionResult Complete()
        {
            var shippingDetailsViewModel = new ShippingDetailsViewModel
            {
                ShippingDetails=new ShippingDetails()
            };
            return View(shippingDetailsViewModel);
        }

        [HttpPost]
        public IActionResult Complete(ShippingDetails shippingDetails)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            TempData.Add("message", String.Format("Thank you {0}, you order is in process", shippingDetails.FirstName));
            return View();
        }
    }
}
