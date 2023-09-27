using System.ComponentModel.DataAnnotations;

namespace Allup.Areas.Manage.ViewModels
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
