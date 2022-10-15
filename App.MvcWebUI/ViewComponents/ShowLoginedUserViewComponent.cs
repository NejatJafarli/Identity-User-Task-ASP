using App.MvcWebUI.Models;
using App.MvcWebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Security.Claims;

namespace App.MvcWebUI.ViewComponents
{
    public class ShowLoginedUser : ViewComponent
    {
        private IHttpContextAccessor _httpContext;
        public ShowLoginedUser(IHttpContextAccessor http)
        {
            _httpContext = http;
        }
        public ViewViewComponentResult Invoke()
        {
            var name = _httpContext.HttpContext.User.Identity?.Name;
            var role = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (role is null || name is null)
            {
                var models = new ShowLoginedUserViewModel
                {
                    LoginedUserName = null
                };

                return View(models);
            }

            var model = new ShowLoginedUserViewModel
            {
                LoginedUserName = $"Welcome {role} {name}"
            };

            return View(model);
        }
    }
}
