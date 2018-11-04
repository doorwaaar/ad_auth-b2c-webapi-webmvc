using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogCornerAuth.WebB2C.Models
{
   

    public class IndexModel : PageModel
    {
        private readonly ApiClient _apiClient;

        public string[] Values { get; set; }

        public IndexModel(ApiClient apiClient)
        {
            this._apiClient = apiClient;
        }

        public void OnGet()
        {
            Values = _apiClient.GetValues().Result;
        }
    }
}