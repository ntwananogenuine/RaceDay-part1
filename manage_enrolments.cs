using Microsoft.AspNetCore.Mvc;
using raceday_api.Auth;
using raceday_api.database;

namespace raceday_api.Controllers
{//start of namespace
    [ApiController]
    public class manage_enrolments : api_base
    {//start of class

        private readonly enrolment_data _enrolments;

        public manage_enrolments(AppDbContext db)
        {//start of constructor
            _enrolments = new enrolment_data(db);
        }//end of constructor

        /// <summary>Enters the logged in Participant into a category. A bib number is given automatically.</summary>
        /// <remarks>Parameter: category_id. Returns 201 with the enrolment, 401, 403 (not a Participant), 404, or 409 if already enrolled or the category is full.</remarks>
        [HttpPost("api/categories/{category_id:int}/enrol")]
        [session_authorize(role_names.Participant)]
        public IActionResult enrol_participant(int category_id)
        {//start of method
            return respond(_enrolments.enrol_participant(current_user_id, category_id));
        }//end of method

        /// <summary>Lists the enrolments of the logged in Participant.</summary>
        /// <remarks>No parameters. Returns 200 with the enrolments, 401, or 403 (not a Participant).</remarks>
        [HttpGet("api/users/me/enrolments")]
        [session_authorize(role_names.Participant)]
        public IActionResult get_my_enrolments()
        {//start of method
            return respond(_enrolments.get_my_enrolments(current_user_id));
        }//end of method

        /// <summary>Lists all the enrolments of a category. Only the Organiser who owns the event can do this.</summary>
        /// <remarks>Parameter: category_id. Returns 200 with the enrolments, 401, 403, 404.</remarks>
        [HttpGet("api/categories/{category_id:int}/enrolments")]
        [session_authorize(role_names.Organiser)]
        public IActionResult get_category_enrolments(int category_id)
        {//start of method
            return respond(_enrolments.get_category_enrolments(current_user_id, category_id));
        }//end of method

        /// <summary>Cancels an enrolment. Allowed for the enrolled Participant or the Organiser who owns the event.</summary>
        /// <remarks>Parameter: id. Returns 200, 401, 403, 404.</remarks>
        [HttpDelete("api/enrolments/{id:int}")]
        [session_authorize]
        public IActionResult cancel_enrolment(int id)
        {//start of method
            return respond(_enrolments.cancel_enrolment(current_user_id, id));
        }//end of method

    }//end of class
}//end of namespace
