using Microsoft.EntityFrameworkCore;
using raceday_api.Models;

namespace raceday_api.database
{//start of namespace
    public class result_data
    {//start of class

        private readonly AppDbContext _db;
        public result_data(AppDbContext db) { _db = db; }

        //query that also loads the participant, category and event of a result
        private IQueryable<Result> with_details()
        {
            return _db.Results.Include(r => r.Enrolment).ThenInclude(e => e.Participant)
                              .Include(r => r.Enrolment).ThenInclude(e => e.Category).ThenInclude(c => c.Event);
        }

        //two participants cannot have the same position in a category
        private bool position_taken(int category_id, int position, int exclude_result_id)
        {
            return _db.Results.Any(r => r.Enrolment.CategoryId == category_id
                                     && r.Position == position && r.ResultId != exclude_result_id);
        }

        //method to store the finish time and position of a participant
        public db_result create_result(int user_id, int enrolment_id, TimeSpan finish_time, int position)
        {//start of method
            Enrolment? en = _db.Enrolments.Include(e => e.Participant)
                .Include(e => e.Category).ThenInclude(c => c.Event)
                .Include(e => e.Result)
                .FirstOrDefault(e => e.EnrolmentId == enrolment_id);
            if (en == null) return db_result.fail(404, "Enrolment not found.");
            if (en.Category.Event.OrganiserId != user_id)
                return db_result.fail(403, "You can only record results for your own events.");

            string? problem = validator.check_result(position);
            if (problem != null) return db_result.fail(400, problem);

            if (en.Result != null) return db_result.fail(409, "This enrolment already has a result. Use PUT to update it.");
            if (position_taken(en.CategoryId, position, 0))
                return db_result.fail(409, "That position is already taken in this category.");

            Result result = new Result
            {
                EnrolmentId = en.EnrolmentId,
                FinishTime = finish_time,
                Position = position,
                Status = "Final",
                Enrolment = en
            };
            _db.Results.Add(result);
            _db.SaveChanges();

            return db_result.created(ResultDto.From(result));
        }//end of method

        //method to get the leaderboard of a category
        public db_result get_category_results(int category_id)
        {//start of method
            if (!_db.Categories.Any(c => c.CategoryId == category_id)) return db_result.fail(404, "Category not found.");

            List<Result> items = with_details().AsNoTracking()
                .Where(r => r.Enrolment.CategoryId == category_id)
                .OrderBy(r => r.Position == null).ThenBy(r => r.Position).ToList();
            return db_result.ok(items.Select(ResultDto.From).ToList());
        }//end of method

        //method to get one result by its id
        public db_result get_result_by_id(int id)
        {//start of method
            Result? r = with_details().AsNoTracking().FirstOrDefault(x => x.ResultId == id);
            if (r == null) return db_result.fail(404, "Result not found.");
            return db_result.ok(ResultDto.From(r));
        }//end of method

        //method to update a result
        public db_result update_result(int user_id, int id, TimeSpan finish_time, int position)
        {//start of method
            Result? r = with_details().FirstOrDefault(x => x.ResultId == id);
            if (r == null) return db_result.fail(404, "Result not found.");
            if (r.Enrolment.Category.Event.OrganiserId != user_id)
                return db_result.fail(403, "You can only change results for your own events.");

            string? problem = validator.check_result(position);
            if (problem != null) return db_result.fail(400, problem);

            if (position_taken(r.Enrolment.CategoryId, position, r.ResultId))
                return db_result.fail(409, "That position is already taken in this category.");

            r.FinishTime = finish_time;
            r.Position = position;
            r.Status = "Final";
            _db.SaveChanges();

            return db_result.ok(ResultDto.From(r));
        }//end of method

        //method to get the results of the logged in participant
        public db_result get_my_results(int user_id)
        {//start of method
            List<Result> items = with_details().AsNoTracking()
                .Where(r => r.Enrolment.ParticipantId == user_id).ToList();
            return db_result.ok(items.Select(ResultDto.From).ToList());
        }//end of method

    }//end of class
}//end of namespace
