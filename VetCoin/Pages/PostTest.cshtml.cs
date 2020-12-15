using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VetCoin.Pages
{
    public class PostTestModel : PageModel
    {
        static public List<string> Guids { get; set; } = new List<string>();

        [BindProperty]
        public string PostGuid { get; set; }



        public void OnGet()
        {
            PostGuid = Guid.NewGuid().ToString();
        }
        public async Task OnPostAsync()
        {
            await Task.Delay(500);
            Guids.Add(PostGuid);
            PostGuid = Guid.NewGuid().ToString();
        }
    }
}
