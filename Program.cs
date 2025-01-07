// Title        : Inventory Management System Application
// Author       : Amal Biju
// Created on   : 22/12/2024
// Updated on   : 31/12/2024
// Reviewed by  : Sabapathi Shanmugam
// Reviewed at  : 28/12/2024

namespace InventoryManagementApp
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json; 
    using InventoryManagementApp.Logging;
    using InventoryManagementApp.Exceptions;
    using InventoryManagementApp.Business;
    using InventoryManagementApp.Authentication;
    using InventoryManagementApp.Services;
    using System.Text.RegularExpressions;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var configService = host.Services.GetRequiredService<IConfigurationService>();

            var connectionString = configService.GetConnectionString("DefaultConnection");
            var logger = host.Services.GetRequiredService<ILoggerService>();
            var inventory = new Inventory(connectionString, logger);
            var authService = new AuthenticationService(connectionString);

            bool isAuthenticated = false;

            var menuOptions = new List<string>
            {
                "1. Register",
                "2. Login",
                "3. Add Product to Inventory",
                "4. Display All Products",
                "5. Find a Product",
                "6. Delete a Product",
                "7. Logout",
                "8. Exit"
            };

            var userCredentials = new Dictionary<string, string>();

            while (true)
            {
                Console.WriteLine("\nWelcome to the Inventory Management Application!");
                Console.WriteLine("Please choose an option:");

                if (!isAuthenticated)
                {
                    Console.WriteLine("1. Register");
                    Console.WriteLine("2. Login");
                }
                else
                {
                    Console.WriteLine("3. Add Product to Inventory");
                    Console.WriteLine("4. Display All Products");
                    Console.WriteLine("5. Find a Product");
                    Console.WriteLine("6. Delete a Product");
                    Console.WriteLine("7. Logout");
                }

                Console.WriteLine("8. Exit");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        if (!isAuthenticated)
                            RegisterUser(authService);
                        break;

                    case "2":
                        if (!isAuthenticated)
                        {
                            LoginUser(authService);
                            isAuthenticated = true;
                        }
                        break;

                    case "3":
                        if (isAuthenticated)
                            AddProductToInventory(inventory);
                        break;

                    case "4":
                        if (isAuthenticated)
                            inventory.DisplayAllProducts();
                        break;

                    case "5":
                        if (isAuthenticated)
                            FindProduct(inventory);
                        break;

                    case "6":
                        if (isAuthenticated)
                            DeleteProduct(inventory);
                        break;

                    case "7":
                        if (isAuthenticated)
                        {
                            LogoutUser(authService);
                            isAuthenticated = false;
                        }
                        break;

                    case "8":
                        Console.WriteLine("Thank you for using the Inventory Management Application!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void RegisterUser(AuthenticationService authService)
        {
            Console.WriteLine("Enter a username:");
            string username = Console.ReadLine() ?? string.Empty;

            string password;
            do
            {
                Console.WriteLine("Enter a password:");
                password = ReadPassword();

                if (!IsValidPassword(password))
                {
                    Console.WriteLine("Password must be at least 8 characters long, contain at least one uppercase letter, and one number.");
                }
            } while (!IsValidPassword(password));

            authService.Register(username, password);
        }

        private static void LoginUser(AuthenticationService authService)
        {
            bool isAuthenticated = false;

            while (!isAuthenticated)
            {
                Console.WriteLine("Enter your username:");
                string username = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Enter your password:");
                string password = ReadPassword();

                try
                {
                    isAuthenticated = authService.Login(username, password);

                    if (!isAuthenticated)
                    {
                        Console.WriteLine("Invalid credentials. Please try again.");
                    }
                }
                catch (SqlException exception)
                {
                    Console.WriteLine($"Database error during login: {exception.Message}");
                    isAuthenticated = false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"An unexpected error occurred: {exception.Message}");
                    isAuthenticated = false;
                }
            }

            Console.WriteLine("Login successful!");
        }

        private static void LogoutUser(IAuthenticationService authService)
        {
            authService.Logout();
        }

        private static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        private static bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, @"[0-9]"))
                return false;

            return true;
        }

        private static void AddProductToInventory(Inventory inventory)
        {
            Console.WriteLine("\nEnter the product name:");
            string name = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Product name cannot be empty.");
                return;
            }

            Console.WriteLine("Enter the quantity:");
            if (int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Enter the price:");
                if (decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    inventory.AddProduct(name, quantity, price);
                }
                else
                {
                    Console.WriteLine("Invalid price. Please enter a valid number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid quantity. Please enter a valid number.");
            }
        }

        private static void FindProduct(Inventory inventory)
        {
            Console.WriteLine("\nEnter the product name to find:");
            string productName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(productName))
            {
                Console.WriteLine("Product name cannot be empty.");
                return;
            }
            inventory.FindProduct(productName);
        }

        private static void DeleteProduct(Inventory inventory)
        {
            Console.WriteLine("\nEnter the product name to delete:");
            string productName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(productName))
            {
                Console.WriteLine("Product name cannot be empty.");
                return;
            }
            inventory.DeleteProduct(productName);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<ILoggerService, LoggerService>();
                    services.AddSingleton<IConfigurationService, ConfigurationService>();
                });
    }
}