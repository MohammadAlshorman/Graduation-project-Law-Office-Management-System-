using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Law_Office_Management.Models;
using Law_Office_Management.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Law_Office_Management.Controllers
{
    public class AppTasksController : Controller
    {
        private readonly MyDbContext _context;
        private readonly NotificationService _notificationService;

        public AppTasksController(MyDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private string? GetUserRole() => HttpContext.Session.GetString("Role");
        private int? GetLawyerId() => _context.Lawyers.FirstOrDefault(l => l.UserId == HttpContext.Session.GetInt32("UserId"))?.LawyerId;

        public async Task<IActionResult> Index()
        {
            var role = GetUserRole();
            if (role == "Lawyer")
            {
                var lawyerId = GetLawyerId();
                if (lawyerId == null) return Unauthorized();

                var tasks = await _context.AppTasks
                    .Where(t => t.LawyerId == lawyerId)
                    .Include(t => t.Lawyer)
                    .ToListAsync();

                return View(tasks);
            }
            else if (role == "Admin")
            {
                var tasks = await _context.AppTasks.Include(t => t.Lawyer).ToListAsync();
                return View(tasks);
            }
            else
            {
                return Unauthorized();
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.AppTasks.Include(t => t.Lawyer).FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null) return NotFound();

            var role = GetUserRole();
            var lawyerId = GetLawyerId();

            if (role == "Lawyer" && task.LawyerId != lawyerId) return Forbid();

            return View(task);
        }

        public IActionResult Create()
        {
            if (GetUserRole() != "Admin") return Forbid();

            ViewData["LawyerList"] = _context.Lawyers.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppTask task)
        {
            if (GetUserRole() != "Admin") return Forbid();

            if (!ModelState.IsValid)
            {
                ViewData["LawyerList"] = _context.Lawyers.ToList();
                return View(task);
            }

            _context.Add(task);
            await _context.SaveChangesAsync();

            // 🔔 إرسال إشعار
            var lawyer = await _context.Lawyers.FindAsync(task.LawyerId);
            if (lawyer?.UserId != null)
            {
                await _notificationService.CreateAsync(
                    userId: lawyer.UserId.Value,
                    title: "📌 مهمة جديدة",
                    message: $"تم تعيين مهمة: {task.Title}، تنتهي بتاريخ {task.DueDate?.ToString("yyyy-MM-dd")}",
                    type: "مهمة"
                );
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (GetUserRole() != "Admin") return Forbid();

            var task = await _context.AppTasks.FindAsync(id);
            if (task == null) return NotFound();

            ViewData["LawyerList"] = _context.Lawyers.ToList();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppTask task)
        {
            if (GetUserRole() != "Admin") return Forbid();
            if (id != task.TaskId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LawyerList"] = _context.Lawyers.ToList();
            return View(task);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (GetUserRole() != "Admin") return Forbid();

            var task = await _context.AppTasks.Include(t => t.Lawyer).FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (GetUserRole() != "Admin") return Forbid();

            var task = await _context.AppTasks.FindAsync(id);
            if (task != null)
            {
                _context.AppTasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var task = await _context.AppTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = true;
            _context.Update(task);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Task marked as completed.";
            return RedirectToAction(nameof(Index));
        }

    }
}
