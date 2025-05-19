using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Law_Office_Management.Filters
{
    public class SessionCheckFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value?.ToLower();

            // استثناءات: صفحات مسموح الدخول لها بدون تسجيل
            if (path != null && (
                path == "/" ||
               
        path.Contains("/home/index") ||
        path.Contains("/auth/login") ||
        path.Contains("/auth/register") ||
        path.Contains("/home/about_us") ||
        path.Contains("/home/contactus") ||
        path.Contains("/home/contactusirbid") ||
        path.Contains("/home/bookconsultation") ||
        path.Contains("/home/privacy") ||
        path.Contains("/home/teams") ||
        path.Contains("/css") ||
        path.Contains("/js") ||
        path.Contains("/images")
            ))
            {
                return;
            }

            var userId = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // ✅ إعادة التوجيه إلى صفحة تسجيل الدخول
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
