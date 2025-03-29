using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BestShop.Pages.Auth
{
    //[RequireNoAuth]
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }


            successMessage = "Please check your email and click on the reset password link";
        }
    }
}
