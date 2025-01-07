namespace InventoryManagementApp.Business
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using InventoryManagementApp.Logging;
    using InventoryManagementApp.Data;

    public class Inventory
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly ILoggerService _logger;

        public Inventory(string connectionString, ILoggerService logger)
        {
            _dbHelper = new DatabaseHelper(connectionString);
            _logger = logger;
        }

        public void AddProduct(string productName, int quantity, decimal price)
        {
            string query = "sp_AddProduct";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName", productName),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@Price", price)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
            Console.WriteLine($"{quantity} units of '{productName}' added to inventory.");
            _logger.LogInformation($"Added {quantity} units of '{productName}' to inventory.");
        }

        public void DisplayAllProducts()
        {
            string query = "sp_DisplayAllProducts";
            var products = _dbHelper.ExecuteQuery(query);

            if (products.Rows.Count == 0)
            {
                Console.WriteLine("No products in the inventory.");
                _logger.LogInformation("Inventory is empty.");
                return;
            }

            Console.WriteLine("\nProducts in Inventory:");
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine("| {0, -10} | {1, -20} | {2, -10} | {3, -10} | {4, -20} |", "ID", "Name", "Quantity", "Price", "Created At");
            Console.WriteLine("--------------------------------------------------------------------------------------");

            foreach (DataRow row in products.Rows)
            {
                Console.WriteLine("| {0, -10} | {1, -20} | {2, -10} | {3, -10:C} | {4, -20} |", row["ProductId"], row["ProductName"], row["Quantity"], row["Price"], row["CreatedAt"]);
            }

            Console.WriteLine("--------------------------------------------------------------------------------------");
            _logger.LogInformation("Displayed all products in the inventory.");
        }
        public void FindProduct(string productName)
        {
            string query = "sp_FindProduct";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName", productName)
            };

            var products = _dbHelper.ExecuteQuery(query, parameters);

            if (products.Rows.Count == 0)
            {
                Console.WriteLine($"Product '{productName}' not found in inventory.");
                _logger.LogError($"Product '{productName}' not found in inventory.");
                return;
            }

            var row = products.Rows[0];
            Console.WriteLine("\nFound Product:");
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine("| {0, -10} | {1, -20} | {2, -10} | {3, -10} | {4, -20} |", "ID", "Name", "Quantity", "Price", "Created At");
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine("| {0, -10} | {1, -20} | {2, -10} | {3, -10:C} | {4, -20} |", row["ProductId"], row["ProductName"], row["Quantity"], row["Price"], row["CreatedAt"]);
            Console.WriteLine("--------------------------------------------------------------------------------------");
            
            _logger.LogInformation($"Found product '{productName}' in inventory.");
        }

        public void DeleteProduct(string productName)
        {
            string query = "sp_DeleteProduct";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductName", productName)
            };

            int rowsAffected = _dbHelper.ExecuteNonQuery(query, parameters);

            if (rowsAffected == 0)
            {
                Console.WriteLine($"Product '{productName}' not found in inventory.");
                _logger.LogError($"Failed to delete product '{productName}' - not found.");
                return;
            }

            Console.WriteLine($"Product '{productName}' has been deleted from the inventory.");
            _logger.LogInformation($"Deleted product '{productName}' from inventory.");
        }
    }
}

