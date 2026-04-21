namespace Prog7311_Part2.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        public string Name { get; set; }

        public string ContactDetails { get; set; }

        public string Region { get; set; }

        public virtual ICollection<Contract>? Contracts { get; set; }



    }
}
