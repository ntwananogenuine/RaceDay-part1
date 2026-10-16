using Microsoft.AspNetCore.Mvc;
using raceday_api.database;
using raceday_api.Models;

namespace raceday_api.Controllers
{//start of namespace
    [Route("api/auth")]
    [ApiController]
    public class auth_user : api_base
    {//start of class

        private readonly user_data _users;

        public auth_user(AppDbContext db)
        {//start of constructor
            //creating an object for user_data
            _users = new user_data(db);
        }//end of constructor

        /// <summary>Registers a new user as an Organiser or Participant.</summary>
        /// <remarks>Parameters: fullname, email, password (min 8 characters), role ("Organiser" or "Participant"). Returns 201 with the new user (no password), 400 for invalid info or role, 409 if the email is already registered.</remarks>
        //post http request to store the new user in the database
        [HttpPost("register")]
        public IActionResult register_user(string fullname, string email, string password, string role)
        {//start of method
            return respond(_users.register_user(fullname, email, password, role));
        }//end of method

        /// <summary>Logs in and starts a session that remembers the user's id and role.</summary>
        /// <remarks>Parameters: email, password. Returns 200 with the user and a session cookie, or 401 for an incorrect email or password.</remarks>
        //post http request to log in
        [HttpPost("login")]
        public IActionResult login_user(string email, string password)
        {//start of method
            db_result result = _users.login_user(email, password);

            //if the login was successful keep the user's id and role in the session
            if (result.status == 200 && result.data is UserDto user)
            {//start of if
                HttpContext.Session.Clear();
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Role", user.Role);
            }//end of if

            return respond(result);
        }//end of method

        /// <summary>Logs out by ending the current session.</summary>
        /// <remarks>No parameters. Returns 200.</remarks>
        [HttpPost("logout")]
        public IActionResult logout_user()
        {//start of method
            HttpContext.Session.Clear();
            return Ok("Logged out.");
        }//end of method

    }//end of class
}//end of namespace
