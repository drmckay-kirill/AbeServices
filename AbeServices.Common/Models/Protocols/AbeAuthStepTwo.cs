namespace AbeServices.Common.Models.Protocols
{
    public class AbeAuthStepTwo
    {
        public string[] AccessPolicy { get; set; }
        public string[] AbonentAttributes { get; set; }
        public byte[] CT { get; set; }
        public byte[] Z { get; set; }
    }
}