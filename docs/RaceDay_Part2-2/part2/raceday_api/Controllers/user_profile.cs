using Microsoft.AspNetCore.Mvc;
using raceday_api.Auth;
using raceday_api.database;

namespace raceday_api.Controllers
{//start of namespace
    [Route("api/users/me")]
    [ApiController]
    [session_authorize]
    public class user_profile : api_base
    {//start of class

        private readonly user_data _users;

        public user_profile(AppDbContext db)
        {//start of constructor
            _users = new user_data(db);
        }//end of constructor

        /// <summary>Returns the profile of the logged in user.</summary>
        /// <remarks>No parameters. Returns 200 with the profile, or 401 if not logged in.</remarks>
        //get request method to get the user's own info
        [HttpGet]
        public IActionResult get_profile()
        {//start of method
            return respond(_users.get_profile(current_user_id));
        }//end of method

        /// <summary>Updates the logged in user's own profile.</summary>
        /// <remarks>Parameters: fullname, email. Returns 200 with the updated profile, 400 for invalid info, 401 if not logged in, 409 if the email belongs to someone else.</remarks>
        //Put request to update the user's info
        [HttpPut]
        public IActionResult update_profile(string fullname, string email)
        {//start of method
            return respond(_users.update_profile(current_user_id, fullname, email));
        }//end of method

    }//end of class
}//end of namespace
