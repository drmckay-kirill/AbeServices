namespace AbeServices.Common.Protocols
{
    public interface IKeyDistributionBuilder
    {
         byte[] BuildStepOne(string key, string abonentId, string keyServiceId, string authorityId, string[] abonentAttributes);
    }
}