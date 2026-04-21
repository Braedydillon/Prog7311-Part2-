namespace Prog7311_Part2.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Status { get; set; }

        public string Servicelevel { get; set; }


        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }

    }
}
