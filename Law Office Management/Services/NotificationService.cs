//using Law_Office_Management.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Law_Office_Management.Controllers
//{
//    public class NotificationsController : Controller
//    {
//        private readonly MyDbContext _context;

//        public NotificationsController(MyDbContext context)
//        {
//            _context = context;
//        }

//        // عرض جميع الإشعارات (لكل المستخدمين حالياً)
//        public async Task<IActionResult> Index()
//        {
//            var notifications = await _context.Notifications
//                .Include(n => n.User)
//                .OrderByDescending(n => n.CreatedAt)
//                .ToListAsync();

//            return View(notifications);
//        }

//        // تحديد إشعار كمقروء
//        public async Task<IActionResult> MarkAsRead(int id)
//        {
//            var notification = await _context.Notifications.FindAsync(id);
//            if (notification == null)
//                return NotFound();

//            notification.IsRead = true;
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }

//        // حذف إشعار
//        public async Task<IActionResult> Delete(int id)
//        {
//            var notification = await _context.Notifications.FindAsync(id);
//            if (notification == null)
//                return NotFound();

//            _context.Notifications.Remove(notification);
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
using Law_Office_Management.Models;

namespace Law_Office_Management.Services
{
    public class NotificationService
    {
        private readonly MyDbContext _context;

        public NotificationService(MyDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(int userId, string title, string message, string type, int? relatedCaseId = null)
        {
            var noti = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedCaseId = relatedCaseId,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(noti);
            await _context.SaveChangesAsync();
        }
    }
}
