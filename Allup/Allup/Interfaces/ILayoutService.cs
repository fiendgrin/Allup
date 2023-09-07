using Allup.Models;
using Allup.ViewModels.BasketVMs;


namespace Allup.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<List<BasketVM>> GetBasketsAsync();

    }
}
