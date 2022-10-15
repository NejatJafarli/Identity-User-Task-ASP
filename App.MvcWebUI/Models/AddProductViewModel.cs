using App.Entities.Concrete;

namespace App.MvcWebUI.Models
{
    public class AddProductViewModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; internal set; }
    }
}
