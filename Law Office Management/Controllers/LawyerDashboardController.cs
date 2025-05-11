using Law_Office_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Law_Office_Management.Controllers
{
    public class LawyerDashboardController : Controller
    {
        private readonly MyDbContext _context;

        public LawyerDashboardController(MyDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentLawyerId()
        {
            var email = HttpContext.Session.GetString("Username");
            return _context.Lawyers.FirstOrDefault(l => l.Email == email)?.LawyerId;
        }

        public async Task<IActionResult> Index()
        {
            var lawyerId = GetCurrentLawyerId();
            if (lawyerId == null)
                return RedirectToAction("Login", "Auth");

            var myTasks = await _context.AppTasks
                .Where(t => t.LawyerId == lawyerId)
                .ToListAsync();

            var viewModel = new LawyerDashboardViewModel
            {
                MyCasesCount = await _context.Cases.CountAsync(c => c.LawyerId == lawyerId),
                TasksCompleted = myTasks.Count(t => t.IsCompleted == true),
                TasksPending = myTasks.Count(t => t.IsCompleted == false || t.IsCompleted == null),
                UpcomingSessions = await _context.CourtSessions.CountAsync(s => s.Case.LawyerId == lawyerId && s.SessionDate > DateTime.Now),
                //ActiveContracts = await _context.Contracts.CountAsync(c => c.LawyerId == lawyerId && (c.EndDate == null || c.EndDate > DateTime.Today)),
                RecentTasks = myTasks.OrderByDescending(t => t.TaskId).Take(5).ToList()
            };

            return View(viewModel);
        }
    }
}
