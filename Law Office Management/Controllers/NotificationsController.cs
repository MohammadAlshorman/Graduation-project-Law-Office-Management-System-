using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly MyDbContext _context;

        public NotificationsController(MyDbContext context)
        {
            _context = context;
        }

        // عرض الإشعارات حسب الدور
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            var notificationsQuery = _context.Notifications
                .Include(n => n.User)
                .Include(n => n.RelatedCase)
                .OrderByDescending(n => n.CreatedAt)
                .AsQueryable();

            if (role == "Lawyer")
            {
                notificationsQuery = notificationsQuery.Where(n => n.UserId == userId);
            }

            var notifications = await notificationsQuery.ToListAsync();
            return View(notifications);
        }

        // صفحة إنشاء إشعار يدوي (Admin فقط)
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            ViewData["Users"] = _context.Users.ToList();
            ViewData["Cases"] = _context.Cases.ToList();
            return View();
        }

        // إضافة إشعار جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification notification)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            if (!ModelState.IsValid)
            {
                ViewData["Users"] = _context.Users.ToList();
                ViewData["Cases"] = _context.Cases.ToList();
                return View(notification);
            }

            notification.CreatedAt = DateTime.Now;
            notification.IsRead = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // تغيير حالة الإشعار إلى "مقروء"
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            // فقط المستخدم نفسه أو الإدمن يمكنه تعديل الإشعار
            if (role != "Admin" && notification.UserId != userId)
                return Unauthorized();

            notification.IsRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // حذف إشعار (Admin فقط)
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            if (id == null) return NotFound();

            var notification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.RelatedCase)
                .FirstOrDefaultAsync(m => m.NotificationId == id);

            if (notification == null) return NotFound();
            return View(notification);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return Unauthorized();

            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}