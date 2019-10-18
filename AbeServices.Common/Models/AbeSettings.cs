namespace AbeServices.Common.Models
{
    public class AbeSettings
    {
        public string KeyServiceUrl { get; set;}
        public string SharedKey { get; set; }
        public string Id { get; set; }
        public string KeyServiceId { get; set; }
        public string AuthorityId { get; set; }
        public string[] Attributes { get; set; }
    }
}