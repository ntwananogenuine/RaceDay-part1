using Microsoft.AspNetCore.Mvc;
using raceday_api.Auth;
using raceday_api.database;

namespace raceday_api.Controllers
{//start of namespace
    [ApiController]
    public class manage_events : api_base
    {//start of class

        private readonly event_data _events;

        public manage_events(AppDbContext db)
        {//start of constructor
            _events = new event_data(db);
        }//end of constructor

        /// <summary>Lists all the events (anyone can view).</summary>
        /// <remarks>No parameters. Returns 200 with the events ordered by date.</remarks>
        [HttpGet("api/events")]
        public IActionResult get_all_events()
        {//start of method
            return respond(_events.get_all_events());
        }//end of method

        /// <summary>Gets one event by its id (anyone can view).</summary>
        /// <remarks>Parameter: id. Returns 200 with the event, or 404 if it does not exist.</remarks>
        [HttpGet("api/events/{id:int}")]
        public IActionResult get_event_by_id(int id)
        {//start of method
            return respond(_events.get_event_by_id(id));
        }//end of method

        /// <summary>Creates an event for the logged in Organiser.</summary>
        /// <remarks>Parameters: name, description (optional), event_date, location, event_type (Run, Walk or Cycle), distance_km (optional). Returns 201 with the event, 400 for invalid info, 401 if not logged in, 403 if not an Organiser.</remarks>
        [HttpPost("api/events")]
        [session_authorize(role_names.Organiser)]
        public IActionResult create_event(string name, string? description, DateTime event_date,
                                          string location, string event_type, decimal? distance_km)
        {//start of method
            return respond(_events.create_event(current_user_id, name, description, event_date, location, event_type, distance_km));
        }//end of method

        /// <summary>Updates an event. Only the Organiser who owns it can do this.</summary>
        /// <remarks>Parameters: id, name, description (optional), event_date, location, event_type, distance_km (optional). Returns 200 with the event, 400, 401, 403 (not the owner or not an Organiser), 404.</remarks>
        [HttpPut("api/events/{id:int}")]
        [session_authorize(role_names.Organiser)]
        public IActionResult update_event(int id, string name, string? description, DateTime event_date,
                                          string location, string event_type, decimal? distance_km)
        {//start of method
            return respond(_events.update_event(current_user_id, id, name, description, event_date, location, event_type, distance_km));
        }//end of method

        /// <summary>Deletes an event with its categories, enrolments and results. Only the Organiser who owns it can do this.</summary>
        /// <remarks>Parameter: id. Returns 200, 401, 403 (not the owner or not an Organiser), 404.</remarks>
        [HttpDelete("api/events/{id:int}")]
        [session_authorize(role_names.Organiser)]
        public IActionResult delete_event(int id)
        {//start of method
            return respond(_events.delete_event(current_user_id, id));
        }//end of method

    }//end of class
}//end of namespace
