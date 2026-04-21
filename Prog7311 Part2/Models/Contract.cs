namespace Prog7311_Part2.Models
{
    //Defining the contracts status as an enum for better type safety and readability
    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }
    public class Contract
    {
        public int ContractId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public ContractStatus Status { get; set; }

              

        public string Servicelevel { get; set; }


        public string? DocumentPath { get; set; } // Stores the PDF filename


        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }
        public virtual ICollection<ServiceRequest>? ServiceRequests { get; set; }

    }
}
