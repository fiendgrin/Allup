using Microsoft.AspNetCore.Mvc;

namespace Allup.ViewComponents
{
    public class NewsLetterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync() 
        {
            
            return View();
        }
    }
}
