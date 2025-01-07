namespace InventoryManagementApp.Authentication
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using InventoryManagementApp.Data;

    public class AuthenticationService : IAuthenticationService
    {
        private readonly DatabaseHelper _dbHelper;
        private string? _currentUser = null;

        public AuthenticationService(string connectionString)
        {
            _dbHelper = new DatabaseHelper(connectionString);
        }

        public void Register(string username, string password)
        {
            var parameters = new SqlParameter[] {
                new SqlParameter("@Username", username),
                new SqlParameter("@PasswordHash", password)
            };

            try
            {
                _dbHelper.ExecuteNonQuery("sp_RegisterUser", parameters);
                Console.WriteLine("User registered successfully!");
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Database error during registration: {exception.Message}");
            }
        }

        public bool Login(string username, string password)
        {
            var parameters = new SqlParameter[] {
                new SqlParameter("@Username", username)
            };

            try
            {
                var result = _dbHelper.ExecuteQuery("sp_LoginUser", parameters);

                if (result.Rows.Count == 0)
                {
                    Console.WriteLine("Invalid username or password.");
                    return false;
                }

                string storedPassword = result.Rows[0]["PasswordHash"].ToString();
                if (password == storedPassword)
                {
                    _currentUser = username;
                    Console.WriteLine($"Welcome, {username}.");
                    return true;
                }

                Console.WriteLine("Invalid username or password.");
                return false;
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Database error during login: {exception.Message}");
                return false;
            }
        }

        public string GetCurrentUser()
        {
            return _currentUser ?? "No user is currently logged in.";
        }

        public void Logout()
        {
            Console.WriteLine($"User {_currentUser} has logged out.");
            _currentUser = null;
        }
    }
}
