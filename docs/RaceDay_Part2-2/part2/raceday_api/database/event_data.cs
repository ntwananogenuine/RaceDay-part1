using Microsoft.EntityFrameworkCore;
using raceday_api.Models;

namespace raceday_api.database
{//start of namespace
    public class event_data
    {//start of class

        private readonly AppDbContext _db;
        public event_data(AppDbContext db) { _db = db; }

        //method to get all the events
        public db_result get_all_events()
        {//start of method
            List<Event> items = _db.Events.AsNoTracking().OrderBy(e => e.EventDate).ToList();
            return db_result.ok(items.Select(EventDto.From).ToList());
        }//end of method

        //method to get one event by its id
        public db_result get_event_by_id(int id)
        {//start of method
            Event? ev = _db.Events.AsNoTracking().FirstOrDefault(e => e.EventId == id);
            if (ev == null) return db_result.fail(404, "Event not found.");
            return db_result.ok(EventDto.From(ev));
        }//end of method

        //method to store a new event for the logged in organiser
        public db_result create_event(int organiser_id, string name, string? description, DateTime event_date,
                                      string location, string event_type, decimal? distance_km)
        {//start of method
            string? problem = validator.check_event(name, description, location, event_type, distance_km);
            if (problem != null) return db_result.fail(400, problem);

            Event ev = new Event
            {
                OrganiserId = organiser_id,
                Name = name.Trim(),
                Description = description,
                EventDate = event_date.Date,
                Location = location.Trim(),
                EventType = event_type,
                DistanceKm = distance_km
            };
            _db.Events.Add(ev);
            _db.SaveChanges();

            return db_result.created(EventDto.From(ev));
        }//end of method

        //method to update an event, only the organiser who owns it can do this
        public db_result update_event(int user_id, int id, string name, string? description, DateTime event_date,
                                      string location, string event_type, decimal? distance_km)
        {//start of method
            Event? ev = _db.Events.Find(id);
            if (ev == null) return db_result.fail(404, "Event not found.");
            if (ev.OrganiserId != user_id) return db_result.fail(403, "You can only change your own events.");

            string? problem = validator.check_event(name, description, location, event_type, distance_km);
            if (problem != null) return db_result.fail(400, problem);

            ev.Name = name.Trim();
            ev.Description = description;
            ev.EventDate = event_date.Date;
            ev.Location = location.Trim();
            ev.EventType = event_type;
            ev.DistanceKm = distance_km;
            _db.SaveChanges();

            return db_result.ok(EventDto.From(ev));
        }//end of method

        //method to delete an event, only the organiser who owns it can do this
        public db_result delete_event(int user_id, int id)
        {//start of method
            Event? ev = _db.Events.Find(id);
            if (ev == null) return db_result.fail(404, "Event not found.");
            if (ev.OrganiserId != user_id) return db_result.fail(403, "You can only delete your own events.");

            _db.Events.Remove(ev);
            _db.SaveChanges();

            return db_result.ok(null, "Event successfully deleted.");
        }//end of method

    }//end of class
}//end of namespace
