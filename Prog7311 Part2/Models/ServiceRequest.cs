namespace Prog7311_Part2.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public decimal Cost { get; set; }

        public int ContractId { get; set; }

        public virtual Contract? Contract { get; set; }
    }
}
