using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Law_Office_Management.Models;
using Law_Office_Management.Services;

namespace Law_Office_Management.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly MyDbContext _context;

        private readonly NotificationService _notificationService;

        public InvoicesController(MyDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }


        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices.Include(i => i.Client).ToListAsync();
            return View(invoices);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Invoices
                .Include(i => i.Client)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            ViewData["ClientList"] = _context.Clients.ToList();
            return View();
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                invoice.CreatedAt = DateTime.Now;
                _context.Add(invoice);
               _context.Add(invoice);
await _context.SaveChangesAsync();

// 🔔 إرسال تنبيه تلقائي للعميل عند إصدار فاتورة
var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == invoice.ClientId);
if (client != null)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == client.Email);
    if (user != null)
    {
        await _notificationService.CreateAsync(
            userId: user.UserId,
            title: "💰 فاتورة جديدة",
            message: $"تم إصدار فاتورة جديدة بقيمة {invoice.Amount} JD، تستحق بتاريخ {invoice.DueDate?.ToString("yyyy-MM-dd")}",
            type: "فاتورة"
        );
    }
}

return RedirectToAction(nameof(Index));

            }
            ViewData["ClientList"] = _context.Clients.ToList();
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            ViewData["ClientList"] = _context.Clients.ToList();
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientList"] = _context.Clients.ToList();
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Invoices
                .Include(i => i.Client)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
