using Allup.Models;

namespace Allup.ViewModels.AccountVMs
{
    public class ProfileVM
    {
        public ProfileAcoountVM ProfileAcoountVM { get; set; }

        public IEnumerable<Address> Addresses { get; set; }

        public Address Address { get; set; }
    }
}
