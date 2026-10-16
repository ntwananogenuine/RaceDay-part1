using Microsoft.AspNetCore.Mvc;
using raceday_api.database;

namespace raceday_api.Controllers
{//start of namespace
    //shared by all the controllers
    public abstract class api_base : ControllerBase
    {//start of class

        //id of the user who is logged in (stored in the session)
        protected int current_user_id => HttpContext.Session.GetInt32("UserId") ?? 0;

        //turns what the database class returned into an http response
        protected IActionResult respond(db_result result)
        {//start of method
            if (result.data != null)
            {
                return StatusCode(result.status, result.data);
            }
            return StatusCode(result.status, result.message);
        }//end of method

    }//end of class
}//end of namespace
