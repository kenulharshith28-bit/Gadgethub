using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GadgetHubWeb.Pages
{
    public class ContactUsModel : PageModel
    {
        [BindProperty]
        public ContactFormModel ContactForm { get; set; } = new();

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Initialize the form
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Here you would typically send the email or save to database
                // For now, we'll just simulate success
                
                // Simulate processing delay
                System.Threading.Thread.Sleep(1000);
                
                SuccessMessage = "Thank you for your message! We'll get back to you within 24 hours.";
                
                // Clear the form
                ContactForm = new ContactFormModel();
                ModelState.Clear();
                
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Sorry, there was an error sending your message. Please try again later.";
                return Page();
            }
        }

        public IActionResult OnPostSubscribeNewsletter(string email)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                ErrorMessage = "Please enter a valid email address.";
                return Page();
            }

            try
            {
                // Here you would typically add the email to your newsletter database
                // For now, we'll just simulate success
                
                SuccessMessage = "Thank you for subscribing to our newsletter! You'll receive your discount coupon shortly.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Sorry, there was an error subscribing to the newsletter. Please try again later.";
                return Page();
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;
    }
}


