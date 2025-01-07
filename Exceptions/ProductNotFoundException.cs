namespace InventoryManagementApp.Exceptions
{
    using System;

    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() : base("Product not found in the inventory.")
        {
        }

        public ProductNotFoundException(string message) : base(message)
        {
        }

        public ProductNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
