using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Dotnet_project.Pages.customers
{
    public class DeleteModel : PageModel
    {
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ILogger<DeleteModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet(int id)
        {
            Id = id;
            // optional: load name for display
            try
            {
                using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=dkte;Uid=root;Pwd=root123;");
                connection.Open();
                using var command = new MySqlCommand("SELECT name FROM customers WHERE id=@CustId", connection);
                command.Parameters.AddWithValue("@CustId", id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Name = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer for delete");
                ErrorMessage = ex.Message;
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                using var connection = new MySqlConnection("Server=localhost;Port=3306;Database=dkte;Uid=root;Pwd=root123;");
                connection.Open();
                using var command = new MySqlCommand("DELETE FROM customers WHERE id=@CustId", connection);
                command.Parameters.AddWithValue("@CustId", Id);
                int rows = command.ExecuteNonQuery();
                if (rows == 0)
                {
                    ErrorMessage = "Customer not found or already deleted.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                ErrorMessage = ex.Message;
                return Page();
            }

            TempData["SuccessMessage"] = "Customer deleted successfully.";
            return RedirectToPage("/Customers/Index");
        }
    }
}