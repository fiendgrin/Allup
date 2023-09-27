using System.ComponentModel.DataAnnotations;

namespace Allup.ViewModels.AccountVMs
{
    public class ProfileAcoountVM
    {
        [StringLength(255)]
        public string Name { get; set; }
        public string SurName { get; set; }
        [StringLength(255)]
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password),Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
    }
}
