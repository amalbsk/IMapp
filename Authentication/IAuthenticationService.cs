namespace InventoryManagementApp.Authentication
{
    public interface IAuthenticationService
    {
        void Register(string username, string password);
        bool Login(string username, string password);
        string GetCurrentUser();
        void Logout();
    }
}
