namespace HobbyMood.Models
{
    public class ServiceResponse
    {
        public enum ServiceStatus
        {
            Created,
            Updated,
            Deleted,
            NotFound,
            Error
        }

        public ServiceStatus Status { get; set; }
        public int? CreatedId { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
