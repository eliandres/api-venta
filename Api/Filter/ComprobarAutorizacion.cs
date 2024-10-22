using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Api.Filter
{
    public class ComprobarAutorizacion: ActionFilterAttribute
    {
          public string IdPermiso { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try {
                var identity = filterContext.HttpContext.User;

                var userData = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
              .Select(c => c.Value).SingleOrDefault();

                if (userData == null)
                {
                    filterContext.Result = new UnauthorizedResult();
                    return;
                }

                if (int.TryParse(userData, out var userId))
                {
                    userId = int.Parse(userData);
                }

                //if (_permiso.FiltrarPermisoxIdUsuarioxIdPermiso(userId, IdPermiso))
                //{
                //    filterContext.Result = new UnauthorizedResult();
                //}
            }
            catch (Exception error){
                throw error;
            }
        }
   
    }
}
