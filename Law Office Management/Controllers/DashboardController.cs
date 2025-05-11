using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MyDbContext _context;

        public DashboardController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var viewModel = new DashboardViewModel
            {
                LawyersCount = await _context.Lawyers.CountAsync(),
                ClientsCount = await _context.Clients.CountAsync(),
                CasesCount = await _context.Cases.CountAsync(),

                ActiveContractsCount = await _context.Contracts
                    .CountAsync(c => c.EndDate == null || c.EndDate > today),

                TasksCompleted = await _context.AppTasks.CountAsync(t => t.IsCompleted == true),
                TasksPending = await _context.AppTasks.CountAsync(t => t.IsCompleted == false || t.IsCompleted == null),

                TodayAttendance = await _context.AttendanceRecords
                    .CountAsync(a => a.CheckInTime.HasValue && a.CheckInTime.Value.Date == DateTime.Today),

                UnreadNotifications = await _context.Notifications.CountAsync(n => n.IsRead == false),

                LatestTasks = await _context.AppTasks
                    .OrderByDescending(t => t.TaskId)
                    .Take(5).ToListAsync(),

                LatestNotifications = await _context.Notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5).ToListAsync(),

                LatestSessions = await _context.CourtSessions
                    .Include(s => s.Case)
                    .ThenInclude(c => c.Lawyer)
                    .OrderByDescending(s => s.SessionDate)
                    .Take(5).ToListAsync(),

                LatestMessages = await _context.ContactMessages
    .OrderByDescending(m => m.SentAt)
    .Take(5)
    .ToListAsync(),

            };

            return View(viewModel);
        }
    }
}
