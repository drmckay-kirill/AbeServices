namespace AbeServices.Common.Protocols
{
    public interface IAbeAuthBuilder
    {
         T GetStepData<T>(byte[] data);
        byte[] BuildStepOne(string[] accessPolicy, string sharedKey);
    }
}