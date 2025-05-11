using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class ContractsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContractsController(MyDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Lawyer)
                .ToListAsync();

            return View(contracts);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Lawyer)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["Clients"] = _context.Clients.ToList();
            ViewData["Lawyers"] = _context.Lawyers.ToList();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Clients"] = _context.Clients.ToList();
                ViewData["Lawyers"] = _context.Lawyers.ToList();
                return View(contract);
            }

            if (file != null && file.Length > 0)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "contracts");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                contract.FilePath = "/contracts/" + fileName;
            }

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            ViewData["Clients"] = _context.Clients.ToList();
            ViewData["Lawyers"] = _context.Lawyers.ToList();
            return View(contract);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile file)
        {
            if (id != contract.ContractId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Clients"] = _context.Clients.ToList();
                ViewData["Lawyers"] = _context.Lawyers.ToList();
                return View(contract);
            }

            try
            {
                if (file != null && file.Length > 0)
                {
                    var folderPath = Path.Combine(_env.WebRootPath, "contracts");
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    contract.FilePath = "/contracts/" + fileName;
                }

                _context.Update(contract);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Contracts.Any(e => e.ContractId == id))
                    return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Lawyer)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
