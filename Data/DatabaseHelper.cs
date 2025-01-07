using System.Data.SqlClient;
using System.Data;

namespace InventoryManagementApp.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure; // Ensure the command is treated as a stored procedure

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Database error: {exception.Message}");
                throw;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred: {exception.Message}");
                throw;
            }
        }

        public int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure; // Ensure the command is treated as a stored procedure

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Database error: {exception.Message}");
                throw;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred: {exception.Message}");
                throw;
            }
        }
    }
}
