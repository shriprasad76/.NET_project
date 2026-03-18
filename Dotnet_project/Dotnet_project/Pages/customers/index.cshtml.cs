using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dotnet_project.Pages.customers
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // list of customers exposed to the Razor page
        public List<CustomerInfo> listCustomers { get; set; } = new();

        public void OnGet()
        {
            try
            {
                using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=dkte;Uid=root;Pwd=root123;");
                connection.Open();
                using var command = new MySqlCommand("SELECT * FROM customers", connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listCustomers.Add(new CustomerInfo
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customers: {ex.Message}");
            }
        }
    }

    public class CustomerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}