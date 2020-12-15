using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Filters
{
    public class MultiPostFilter : IAsyncPageFilter
    {
        static public List<string> GurdGuids { get; set; } = new List<string>();

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HttpContext.Request.Method.ToLower() == "post")
            {
                var gurdGuid = context.HttpContext.Request.Form["__RequestVerificationToken"];

                if(!string.IsNullOrEmpty(gurdGuid))
                {
                    if(GurdGuids.Contains(gurdGuid))
                    {
                        
                        context.Result = new RedirectResult("/MultiPost");
                        return;
                        
                    }
                    else
                    {
                        
                        GurdGuids.Add(gurdGuid);
                    }
                }
            }
            await next.Invoke();
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {



            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
