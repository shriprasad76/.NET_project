using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dotnet_project.Pages.customers
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty, Required(ErrorMessage = "Enter the name")]
        public string Name { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Enter the email"),
            EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Enter the phone number")]
        public string Phone { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet(int id)
        {
            try
            {
                using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=dkte;Uid=root;Pwd=root123;");
                connection.Open();
                using var command = new MySqlCommand("SELECT * FROM customers WHERE id = @CustId", connection);
                command.Parameters.AddWithValue("@CustId", id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Id = reader.GetInt32(0);
                    Name = reader.GetString(1);
                    Email = reader.GetString(2);
                    Phone = reader.GetString(3);
                }
                else
                {
                    _logger.LogWarning("Customer not found: {Id}", id);
                    ErrorMessage = "Customer not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer details");
                ErrorMessage = ex.Message;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=dkte;Uid=root;Pwd=root123;");
                connection.Open();
                using var command = new MySqlCommand(
                    "UPDATE customers SET name=@CustName, email=@CustEmail, phone=@CustPhone WHERE id=@CustId", connection);
                command.Parameters.AddWithValue("@CustName", Name);
                command.Parameters.AddWithValue("@CustEmail", Email);
                command.Parameters.AddWithValue("@CustPhone", Phone);
                command.Parameters.AddWithValue("@CustId", Id);
                int rows = command.ExecuteNonQuery();
                if (rows == 0)
                {
                    ErrorMessage = "No rows updated. Customer may not exist.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                ErrorMessage = ex.Message;
                return Page();
            }

            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToPage("/Customers/Index");
        }
    }
}