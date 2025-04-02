using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestShop.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        public List<UserInfo> listUsers = new List<UserInfo>();

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 5; // users per page

        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }
        }
    }

    public class UserInfo
    {
        public int id;
        public string firstname;
        public string lastname;
        public string email;
        public string phone;
        public string address;
        public string password;
        public string role;
        public string createdAt;
    }
}
