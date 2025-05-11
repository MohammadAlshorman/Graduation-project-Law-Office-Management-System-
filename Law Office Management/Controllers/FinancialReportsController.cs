using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class FinancialReportsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _env;

        public FinancialReportsController(MyDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _context.FinancialReports
                .Include(r => r.CreatedByNavigation)
                .ToListAsync();
            return View(reports);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FinancialReport model, IFormFile ReportFile)
        {
            if (ReportFile != null && ReportFile.Length > 0)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ReportFile.FileName);
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ReportFile.CopyToAsync(stream);
                }

                model.ReportFile = fileName;
            }

            model.CreatedAt = DateTime.Now;
            model.CreatedBy = 1; // حط ID الأدمن الحالي

            _context.FinancialReports.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var report = await _context.FinancialReports.FindAsync(id);
            if (report == null) return NotFound();

            return View(report);
        }

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(int id, FinancialReport model, IFormFile? newFile)
        {
            var report = await _context.FinancialReports.FindAsync(id);
            if (report == null) return NotFound();

            if (newFile != null && newFile.Length > 0)
            {
                // حذف الملف القديم
                if (!string.IsNullOrEmpty(report.ReportFile))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, "uploads", report.ReportFile);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // رفع الملف الجديد
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(newFile.FileName);
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await newFile.CopyToAsync(stream);
                }

                report.ReportFile = fileName;
            }

            report.Title = model.Title;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.FinancialReports.FindAsync(id);
            if (report == null) return NotFound();

            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.FinancialReports.FindAsync(id);
            if (report == null) return NotFound();

            if (!string.IsNullOrEmpty(report.ReportFile))
            {
                var path = Path.Combine(_env.WebRootPath, "uploads", report.ReportFile);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.FinancialReports.Remove(report);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _context.FinancialReports
                .Include(r => r.CreatedByNavigation)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            if (report == null) return NotFound();

            return View(report);
        }


        public async Task<IActionResult> Download(int id)
        {
            var report = await _context.FinancialReports.FindAsync(id);
            if (report == null || string.IsNullOrEmpty(report.ReportFile))
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads", report.ReportFile);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var contentType = "application/octet-stream";
            var fileName = report.ReportFile;

            return PhysicalFile(filePath, contentType, fileName);
        }

    }
}
