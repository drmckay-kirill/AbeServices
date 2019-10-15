namespace AbeServices.Common.Protocols
{
    public interface IAbeAuthBuilder
    {
        byte[] BuildStepOne(string[] accessPolicy, string sharedKey);
    }
}