using Microsoft.EntityFrameworkCore;
using raceday_api.Models;

namespace raceday_api.database
{//start of namespace
    public class enrolment_data
    {//start of class

        private readonly AppDbContext _db;
        public enrolment_data(AppDbContext db) { _db = db; }

        //method to enter a participant into a category, the bib number is given automatically
        public db_result enrol_participant(int user_id, int category_id)
        {//start of method
            Category? cat = _db.Categories.Include(c => c.Event).FirstOrDefault(c => c.CategoryId == category_id);
            if (cat == null) return db_result.fail(404, "Category not found.");

            //a participant can only enter a category once
            if (_db.Enrolments.Any(e => e.CategoryId == category_id && e.ParticipantId == user_id))
                return db_result.fail(409, "You are already enrolled in this category.");

            //check the category is not full
            List<string> bibs = _db.Enrolments.Where(e => e.CategoryId == category_id)
                .Select(e => e.BibNumber).ToList();
            if (bibs.Count >= cat.MaxParticipants)
                return db_result.fail(409, "This category is full.");

            //next bib number, B001, B002...
            int next = bibs.Select(b => int.TryParse(b.TrimStart('B'), out int n) ? n : 0)
                           .DefaultIfEmpty(0).Max() + 1;

            Enrolment enrolment = new Enrolment
            {
                ParticipantId = user_id,
                CategoryId = category_id,
                BibNumber = "B" + next.ToString("D3"),
                Status = "Registered"
            };
            _db.Enrolments.Add(enrolment);

            //try and catch
            try
            {//start of try
                _db.SaveChanges();
            }//end of try
            catch (DbUpdateException)
            {//start of catch
                return db_result.fail(409, "Could not complete the enrolment, please try again.");
            }//end of catch

            _db.Entry(enrolment).Reference(e => e.Participant).Load();
            enrolment.Category = cat;

            return db_result.created(EnrolmentDto.From(enrolment));
        }//end of method

        //method to get the enrolments of the logged in participant
        public db_result get_my_enrolments(int user_id)
        {//start of method
            List<Enrolment> items = _db.Enrolments.AsNoTracking()
                .Include(e => e.Participant).Include(e => e.Category).ThenInclude(c => c.Event)
                .Where(e => e.ParticipantId == user_id)
                .OrderBy(e => e.EnrolmentDate).ToList();
            return db_result.ok(items.Select(EnrolmentDto.From).ToList());
        }//end of method

        //method to get all the enrolments of a category, only the organiser who owns the event
        public db_result get_category_enrolments(int user_id, int category_id)
        {//start of method
            Category? cat = _db.Categories.Include(c => c.Event).FirstOrDefault(c => c.CategoryId == category_id);
            if (cat == null) return db_result.fail(404, "Category not found.");
            if (cat.Event.OrganiserId != user_id)
                return db_result.fail(403, "You can only view enrolments for your own events.");

            List<Enrolment> items = _db.Enrolments.AsNoTracking()
                .Include(e => e.Participant).Include(e => e.Category).ThenInclude(c => c.Event)
                .Where(e => e.CategoryId == category_id).OrderBy(e => e.BibNumber).ToList();
            return db_result.ok(items.Select(EnrolmentDto.From).ToList());
        }//end of method

        //method to cancel an enrolment, the participant or the organiser who owns the event
        public db_result cancel_enrolment(int user_id, int id)
        {//start of method
            Enrolment? en = _db.Enrolments.Include(e => e.Category).ThenInclude(c => c.Event)
                .FirstOrDefault(e => e.EnrolmentId == id);
            if (en == null) return db_result.fail(404, "Enrolment not found.");

            if (en.ParticipantId != user_id && en.Category.Event.OrganiserId != user_id)
                return db_result.fail(403, "You can only cancel your own enrolment, or enrolments for your own events.");

            _db.Enrolments.Remove(en);
            _db.SaveChanges();

            return db_result.ok(null, "Enrolment successfully cancelled.");
        }//end of method

    }//end of class
}//end of namespace
