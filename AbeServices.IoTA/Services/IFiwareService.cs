namespace AbeServices.IoTA.Services
{
    public interface IFiwareService
    {
        byte[] Authorize(string entityName, string sessionId, byte[] body, bool read);  
    }
}