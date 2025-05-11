using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Law_Office_Management.Controllers
{
    public class ContactMessagesController : Controller
    {
        private readonly MyDbContext _context;

        public ContactMessagesController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            return View(messages);
        }
    }
}
