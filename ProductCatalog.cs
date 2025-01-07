namespace InventoryManagementApp.Business
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;

    public class ProductCatalog
    {
        private readonly IConfiguration _configuration;

        // Store product details (Name, Quantity, Price)
        private readonly Dictionary<string, Product> _products = new Dictionary<string, Product>();

        public ProductCatalog(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadProductDetails();
        }

        // Load products from configuration 
        private void LoadProductDetails()
        {
            var productConfig = _configuration.GetSection("ProductCatalog");

            foreach (var product in productConfig.GetChildren())
            {
                string productName = product.Key;

                if (!decimal.TryParse(product.Value, out decimal price))
                {
                    price = 0; // Default price or throw an exception
                    Console.WriteLine($"Warning: Invalid price format for product {product.Key}.");

                }

                if (!int.TryParse(_configuration.GetValue<string>($"StockLimits:{productName}"), out int quantity))
                {
                    quantity = 0; // Default quantity or throw an exception
                }

                _products[productName] = new Product(productName, quantity, price);
            }
        }

        // Get all available products (name, quantity)
        public Dictionary<string, int> GetAvailableProducts()
        {
            var availableProducts = new Dictionary<string, int>();

            foreach (var product in _products)
            {
                availableProducts[product.Key] = product.Value.Quantity;
            }

            return availableProducts;
        }

        // Find a product by its name
        public Product? FindProduct(string productName)
        {
            return _products.ContainsKey(productName) ? _products[productName] : null;
        }


        // Add a new product or update an existing product
        public void AddProduct(Product product)
        {
            if (_products.ContainsKey(product.Name))
            {
                // Update existing product
                _products[product.Name].Quantity += product.Quantity;
            }
            else
            {
                _products.Add(product.Name, product);
            }
        }

        // Delete a product by name
        public bool DeleteProduct(string productName)
        {
            if (_products.ContainsKey(productName))
            {
                _products.Remove(productName);
                return true;
            }

            return false;
        }

        // Get product details (price)
        public decimal GetProductPrice(string productName)
        {
            return _products.ContainsKey(productName) ? _products[productName].Price : 0.00M;
        }
    }

    // Product class to represent product details (Name, Quantity, Price)
    public class Product
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Product(string name, int quantity, decimal price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
        }
    }
}
