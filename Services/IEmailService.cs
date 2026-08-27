namespace CosmosCrudApi.Services
{
    public interface IEmailService
    {
        Task SendRegistrationEmailAsync(string email, string name);
    }
}