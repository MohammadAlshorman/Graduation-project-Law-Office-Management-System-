namespace Law_Office_Management.Models
{
    public class LawyerDashboardViewModel
    {
        public int MyCasesCount { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksPending { get; set; }
        public int UpcomingSessions { get; set; }
        public int ActiveContracts { get; set; }

        public List<AppTask>? RecentTasks { get; set; }
    }
}
