using App.Entities.Concrete;

namespace App.MvcWebUI
{
    public class CategoryListViewModel
    {
        public List<Category> Categories { get; set; }
        public int CurrentCategory { get; internal set; }
    }
}