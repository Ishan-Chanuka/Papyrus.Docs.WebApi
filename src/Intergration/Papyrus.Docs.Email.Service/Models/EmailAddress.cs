namespace Papyrus.Docs.Email.Service.Models
{
    public class EmailAddress(string name, string address)
    {
        public string Name { get; set; } = name;
        public string Address { get; set; } = address;
    }
}
