using Allup.Models;

namespace Allup.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task<List<Category>> GetCategoriesAsync();

    }
}
