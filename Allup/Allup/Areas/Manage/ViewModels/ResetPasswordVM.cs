using System.ComponentModel.DataAnnotations;

namespace Allup.Areas.Manage.ViewModels
{
    public class ResetPasswordVM
    {
        //public string Id { get; set; }
        //public string Token { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

    }
}
