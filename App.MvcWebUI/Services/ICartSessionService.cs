using App.Entities.Concrete;

namespace App.MvcWebUI.Services
{
    public interface ICartSessionService
    {
        Cart GetCart();
        void SetCart(Cart cart);
    }
}
