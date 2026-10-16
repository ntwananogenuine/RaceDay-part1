using Microsoft.AspNetCore.Mvc;
using raceday_api.Auth;
using raceday_api.database;

namespace raceday_api.Controllers
{//start of namespace
    [ApiController]
    public class manage_categories : api_base
    {//start of class

        private readonly category_data _categories;

        public manage_categories(AppDbContext db)
        {//start of constructor
            _categories = new category_data(db);
        }//end of constructor

        /// <summary>Lists the categories of an event (anyone can view).</summary>
        /// <remarks>Parameter: event_id. Returns 200 with the categories, or 404 if the event does not exist.</remarks>
        [HttpGet("api/events/{event_id:int}/categories")]
        public IActionResult get_categories_for_event(int event_id)
        {//start of method
            return respond(_categories.get_categories_for_event(event_id));
        }//end of method

        /// <summary>Adds a category to an event. Only the Organiser who owns the event can do this.</summary>
        /// <remarks>Parameters: event_id, name, distance_km, max_participants. Returns 201 with the category, 400, 401, 403, 404.</remarks>
        [HttpPost("api/events/{event_id:int}/categories")]
        [session_authorize(role_names.Organiser)]
        public IActionResult create_category(int event_id, string name, decimal distance_km, int max_participants)
        {//start of method
            return respond(_categories.create_category(current_user_id, event_id, name, distance_km, max_participants));
        }//end of method

        /// <summary>Updates a category. Only the Organiser who owns the event can do this.</summary>
        /// <remarks>Parameters: id, name, distance_km, max_participants. Returns 200 with the category, 400, 401, 403, 404.</remarks>
        [HttpPut("api/categories/{id:int}")]
        [session_authorize(role_names.Organiser)]
        public IActionResult update_category(int id, string name, decimal distance_km, int max_participants)
        {//start of method
            return respond(_categories.update_category(current_user_id, id, name, distance_km, max_participants));
        }//end of method

        /// <summary>Deletes a category with its enrolments and results. Only the Organiser who owns the event can do this.</summary>
        /// <remarks>Parameter: id. Returns 200, 401, 403, 404.</remarks>
        [HttpDelete("api/categories/{id:int}")]
        [session_authorize(role_names.Organiser)]
        public IActionResult delete_category(int id)
        {//start of method
            return respond(_categories.delete_category(current_user_id, id));
        }//end of method

    }//end of class
}//end of namespace
