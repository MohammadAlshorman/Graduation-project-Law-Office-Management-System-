using System.Diagnostics;
using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Law_Office_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult about_us()
        {
            return View();
        }

        public IActionResult teams()
        {
            return View();
        }
        public IActionResult BookConsultation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactUS()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUS(ContactMessage model)
        {
            string recaptchaResponse = Request.Form["g-recaptcha-response"];
            string secretKey = "6LdTbSsrAAAAAOnBHka2FyYsj0K9VMFFLSiwp2A5";

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}",
                    null);

                var json = await response.Content.ReadAsStringAsync();
                var captchaObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                bool success = captchaObj.Value<bool>("success");

                if (!success)
                {
                    TempData["ErrorMessage"] = "يرجى تأكيد أنك لست روبوت.";
                    return RedirectToAction("ContactUS");
                }
            }

            if (ModelState.IsValid)
            {
                model.SentAt = DateTime.Now;
                _context.ContactMessages.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Your message was sent successfully.";
                return RedirectToAction("ContactUS");
            }

            TempData["ErrorMessage"] = "An error occurred while sending the message";
            return View(model);
        }

        [HttpGet]
        public IActionResult ContactUsIrbid()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUsIrbid(ContactMessage model)
        {
            string recaptchaResponse = Request.Form["g-recaptcha-response"];
            string secretKey = "6LdTbSsrAAAAAOnBHka2FyYsj0K9VMFFLSiwp2A5";

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}",
                    null);

                var json = await response.Content.ReadAsStringAsync();
                var captchaObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                bool success = captchaObj.Value<bool>("success");

                if (!success)
                {
                    TempData["ErrorMessage"] = "يرجى تأكيد أنك لست روبوت.";
                    return RedirectToAction("ContactUsIrbid");
                }
            }

            if (ModelState.IsValid)
            {
                model.SentAt = DateTime.Now;
                _context.ContactMessages.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Your message was sent successfully.";
                return RedirectToAction("ContactUsIrbid");
            }

            TempData["ErrorMessage"] = "An error occurred while sending the message";
            return View(model);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
