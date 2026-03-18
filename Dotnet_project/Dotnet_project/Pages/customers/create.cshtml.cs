using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dotnet_project.Pages.customers
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
        }

        [BindProperty, Required(ErrorMessage = "Enter the name")]
        public string Name { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Enter the email"),
            EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Enter the phone number")]
        public string Phone { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // keep values so user can correct
                return Page();
            }

            try
            {
                using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=dkte;Uid=root;Pwd=root123;");
                connection.Open();
                using var command = new MySqlCommand(
                    "INSERT INTO Customers (name, email, phone) VALUES (@CustName, @CustEmail, @CustPhone)",
                    connection);
                command.Parameters.AddWithValue("@CustName", Name);
                command.Parameters.AddWithValue("@CustEmail", Email);
                command.Parameters.AddWithValue("@CustPhone", Phone);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // log and show error to user
                _logger.LogError(ex, "Error inserting customer");
                ErrorMessage = ex.Message;
                return Page();
            }

            TempData["SuccessMessage"] = "Customer created successfully.";
            return RedirectToPage("/Customers/Index");
        }
    }
}
