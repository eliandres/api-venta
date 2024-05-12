using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Api.Filter
{
    public class ComprobarAutorizacion: ActionFilterAttribute
    {
          public string IdPermiso { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try {
                var identity = filterContext.HttpContext.User;
            }
            catch (Exception error){ 
            
            }
        }
   
    }
}
