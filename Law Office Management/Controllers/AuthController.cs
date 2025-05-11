using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Law_Office_Management.Models;

namespace Law_Office_Management.Controllers
{
    public class AuthController : Controller
    {
        private readonly MyDbContext _context;

        public AuthController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password && u.IsActive == true);

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username ?? "");
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Dashboard");

                case "Lawyer":
                    return RedirectToAction("Index", "LawyerDashboard");

                default:
                    return RedirectToAction("AccessDenied", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
