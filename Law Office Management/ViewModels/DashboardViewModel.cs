namespace Law_Office_Management.Models
{
    public class DashboardViewModel
    {
        public int LawyersCount { get; set; }
        public int ClientsCount { get; set; }
        public int CasesCount { get; set; }
        public int ActiveContractsCount { get; set; }

        public int TasksCompleted { get; set; }
        public int TasksPending { get; set; }

        public int TodayAttendance { get; set; }
        public int UnreadNotifications { get; set; }

        public List<AppTask> LatestTasks { get; set; } = new();
        public List<Notification> LatestNotifications { get; set; } = new();
        public List<CourtSession> LatestSessions { get; set; } = new();

        public List<ContactMessage> LatestMessages { get; set; }

    }

}
