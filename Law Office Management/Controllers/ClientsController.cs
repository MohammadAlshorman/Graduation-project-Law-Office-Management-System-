using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Law_Office_Management.Models;

namespace Law_Office_Management.Controllers
{
    public class ClientsController : Controller
    {
        private readonly MyDbContext _context;

        public ClientsController(MyDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            return View(await _context.Clients.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Cases)
                .Include(c => c.Contracts)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            if (client == null) return NotFound();

            return View(client);
        }


        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (!ModelState.IsValid) return View(client);

            _context.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id != client.ClientId) return NotFound();
            if (!ModelState.IsValid) return View(client);

            try
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clients.Any(e => e.ClientId == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
