using Microsoft.EntityFrameworkCore;
using raceday_api.Models;

namespace raceday_api.database
{//start of namespace
    public class category_data
    {//start of class

        private readonly AppDbContext _db;
        public category_data(AppDbContext db) { _db = db; }

        //method to get the categories of an event
        public db_result get_categories_for_event(int event_id)
        {//start of method
            if (!_db.Events.Any(e => e.EventId == event_id)) return db_result.fail(404, "Event not found.");

            List<Category> items = _db.Categories.AsNoTracking()
                .Where(c => c.EventId == event_id).OrderBy(c => c.DistanceKm).ToList();
            return db_result.ok(items.Select(CategoryDto.From).ToList());
        }//end of method

        //method to add a category to an event, only the organiser who owns the event
        public db_result create_category(int user_id, int event_id, string name, decimal distance_km, int max_participants)
        {//start of method
            Event? ev = _db.Events.Find(event_id);
            if (ev == null) return db_result.fail(404, "Event not found.");
            if (ev.OrganiserId != user_id) return db_result.fail(403, "You can only add categories to your own events.");

            string? problem = validator.check_category(name, distance_km, max_participants);
            if (problem != null) return db_result.fail(400, problem);

            Category cat = new Category
            {
                EventId = event_id,
                Name = name.Trim(),
                DistanceKm = distance_km,
                MaxParticipants = max_participants
            };
            _db.Categories.Add(cat);
            _db.SaveChanges();

            return db_result.created(CategoryDto.From(cat));
        }//end of method

        //method to update a category
        public db_result update_category(int user_id, int id, string name, decimal distance_km, int max_participants)
        {//start of method
            Category? cat = _db.Categories.Include(c => c.Event).FirstOrDefault(c => c.CategoryId == id);
            if (cat == null) return db_result.fail(404, "Category not found.");
            if (cat.Event.OrganiserId != user_id) return db_result.fail(403, "You can only change categories of your own events.");

            string? problem = validator.check_category(name, distance_km, max_participants);
            if (problem != null) return db_result.fail(400, problem);

            cat.Name = name.Trim();
            cat.DistanceKm = distance_km;
            cat.MaxParticipants = max_participants;
            _db.SaveChanges();

            return db_result.ok(CategoryDto.From(cat));
        }//end of method

        //method to delete a category
        public db_result delete_category(int user_id, int id)
        {//start of method
            Category? cat = _db.Categories.Include(c => c.Event).FirstOrDefault(c => c.CategoryId == id);
            if (cat == null) return db_result.fail(404, "Category not found.");
            if (cat.Event.OrganiserId != user_id) return db_result.fail(403, "You can only delete categories of your own events.");

            _db.Categories.Remove(cat);
            _db.SaveChanges();

            return db_result.ok(null, "Category successfully deleted.");
        }//end of method

    }//end of class
}//end of namespace
