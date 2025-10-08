using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace webBanSach.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var adminId = session.GetInt32("AdminId"); // trùng với session khi login

            if (adminId == null)
            {
                context.Result = new RedirectToActionResult("Login", "AdminAccount", new { area = "Admin" });
            }
        }
    }
}
