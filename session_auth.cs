using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace raceday_api.Auth
{//start of namespace

    //the two roles in the system
    public static class role_names
    {
        public const string Organiser = "Organiser";
        public const string Participant = "Participant";
    }

    //checks the session before a request reaches the method
    //no session = 401 (not logged in), wrong role = 403 (not allowed)
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class session_authorizeAttribute : Attribute, IAuthorizationFilter
    {//start of class
        private readonly string[] _roles;
        public session_authorizeAttribute(params string[] roles) => _roles = roles;

        public void OnAuthorization(AuthorizationFilterContext context)
        {//start of method
            var session = context.HttpContext.Session;

            //not logged in
            if (session.GetInt32("UserId") == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //logged in but the role is not allowed to use this endpoint
            if (_roles.Length > 0 && !_roles.Contains(session.GetString("Role")))
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "Your role is not allowed to use this endpoint."
                })
                { StatusCode = 403 };
            }
        }//end of method
    }//end of class

}//end of namespace
