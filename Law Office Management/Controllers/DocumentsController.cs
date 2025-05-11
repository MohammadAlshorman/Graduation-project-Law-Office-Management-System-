using Law_Office_Management.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(MyDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var documents = await _context.Documents
                .Include(d => d.Case)
                .Include(d => d.UploadedByNavigation)
                .ToListAsync();
            return View(documents);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            ViewData["CaseList"] = _context.Cases.ToList();
            ViewData["UserList"] = _context.Users.ToList();
            return View();
        }

        // POST: Documents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "documents");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                document.FileName = file.FileName;
                document.FilePath = "/uploads/documents/" + fileName;
                document.FileType = Path.GetExtension(file.FileName);
                document.UploadDate = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CaseList"] = _context.Cases.ToList();
            ViewData["UserList"] = _context.Users.ToList();
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return NotFound();

            ViewData["CaseList"] = _context.Cases.ToList();
            ViewData["UserList"] = _context.Users.ToList();
            return View(doc);
        }

        // POST: Documents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Document doc)
        {
            if (id != doc.DocumentId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(doc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CaseList"] = _context.Cases.ToList();
            ViewData["UserList"] = _context.Users.ToList();
            return View(doc);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var doc = await _context.Documents
                .Include(d => d.Case)
                .Include(d => d.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.DocumentId == id);

            if (doc == null) return NotFound();
            return View(doc);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc != null)
            {
                // حذف الملف من السيرفر أيضًا
                var fullPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                _context.Documents.Remove(doc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var doc = await _context.Documents
                .Include(d => d.Case)
                .Include(d => d.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.DocumentId == id);

            if (doc == null) return NotFound();
            return View(doc);
        }

        // GET: Documents/Download/5
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null) return NotFound();

            var document = await _context.Documents.FindAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, document.FilePath.TrimStart('/'));
            var mimeType = "application/octet-stream";
            return PhysicalFile(filePath, mimeType, document.FileName);
        }
    }
}
