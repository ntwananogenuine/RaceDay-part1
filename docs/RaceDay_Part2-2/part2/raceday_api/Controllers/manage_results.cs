using Microsoft.AspNetCore.Mvc;
using raceday_api.Auth;
using raceday_api.database;

namespace raceday_api.Controllers
{//start of namespace
    [ApiController]
    public class manage_results : api_base
    {//start of class

        private readonly result_data _results;

        public manage_results(AppDbContext db)
        {//start of constructor
            _results = new result_data(db);
        }//end of constructor

        /// <summary>Records the finish time and position of a participant. Only the Organiser who owns the event can do this.</summary>
        /// <remarks>Parameters: enrolment_id, finish_time (hh:mm:ss), position. Returns 201 with the result, 400, 401, 403, 404, or 409 if the enrolment already has a result or the position is taken.</remarks>
        [HttpPost("api/enrolments/{enrolment_id:int}/result")]
        [session_authorize(role_names.Organiser)]
        public IActionResult create_result(int enrolment_id, TimeSpan finish_time, int position)
        {//start of method
            return respond(_results.create_result(current_user_id, enrolment_id, finish_time, position));
        }//end of method

        /// <summary>Gets the leaderboard (results ordered by position) of a category (anyone can view).</summary>
        /// <remarks>Parameter: category_id. Returns 200 with the results, or 404 if the category does not exist.</remarks>
        [HttpGet("api/categories/{category_id:int}/results")]
        public IActionResult get_category_results(int category_id)
        {//start of method
            return respond(_results.get_category_results(category_id));
        }//end of method

        /// <summary>Gets one result by its id (anyone can view).</summary>
        /// <remarks>Parameter: id. Returns 200 with the result, or 404.</remarks>
        [HttpGet("api/results/{id:int}")]
        public IActionResult get_result_by_id(int id)
        {//start of method
            return respond(_results.get_result_by_id(id));
        }//end of method

        /// <summary>Updates a result. Only the Organiser who owns the event can do this.</summary>
        /// <remarks>Parameters: id, finish_time (hh:mm:ss), position. Returns 200 with the result, 400, 401, 403, 404, or 409 if the position is taken.</remarks>
        [HttpPut("api/results/{id:int}")]
        [session_authorize(role_names.Organiser)]
        public IActionResult update_result(int id, TimeSpan finish_time, int position)
        {//start of method
            return respond(_results.update_result(current_user_id, id, finish_time, position));
        }//end of method

        /// <summary>Lists the results of the logged in Participant.</summary>
        /// <remarks>No parameters. Returns 200 with the results, 401, or 403 (not a Participant).</remarks>
        [HttpGet("api/users/me/results")]
        [session_authorize(role_names.Participant)]
        public IActionResult get_my_results()
        {//start of method
            return respond(_results.get_my_results(current_user_id));
        }//end of method

    }//end of class
}//end of namespace
