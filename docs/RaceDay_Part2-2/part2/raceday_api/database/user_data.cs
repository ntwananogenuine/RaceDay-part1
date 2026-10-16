using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using raceday_api.Models;

namespace raceday_api.database
{//start of namespace
    public class user_data
    {//start of class

        //the object that talks to the raceday database
        private readonly AppDbContext _db;
        public user_data(AppDbContext db) { _db = db; }

        //method to store a new user (register)
        public db_result register_user(string fullname, string email, string password, string role)
        {//start of method

            //check the info the user typed in
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length > 100)
                return db_result.fail(400, "Full name is required (max 100 characters).");

            string clean_email = (email ?? "").Trim().ToLowerInvariant();
            if (clean_email.Length > 150 || !new EmailAddressAttribute().IsValid(clean_email))
                return db_result.fail(400, "A valid email is required.");

            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return db_result.fail(400, "Password must be at least 8 characters.");

            //the user can only be an Organiser or a Participant
            Role? found_role = _db.Roles.FirstOrDefault(r => r.RoleName == role);
            if (found_role == null)
                return db_result.fail(400, "Role must be 'Organiser' or 'Participant'.");

            //one account per email
            if (_db.Users.Any(u => u.Email == clean_email))
                return db_result.fail(409, "Email already registered.");

            //store the user, the password is hashed and never stored in its original form
            User user = new User
            {
                RoleId = found_role.RoleId,
                FullName = fullname.Trim(),
                Email = clean_email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = found_role
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            return db_result.created(UserDto.From(user));
        }//end of method

        //method to check the email and password of a user (login)
        public db_result login_user(string email, string password)
        {//start of method
            string clean_email = (email ?? "").Trim().ToLowerInvariant();

            User? user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == clean_email);

            //compare the typed password with the stored hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(password ?? "", user.PasswordHash))
                return db_result.fail(401, "Invalid email or password.");

            return db_result.ok(UserDto.From(user));
        }//end of method

        //method to get the logged in user's profile
        public db_result get_profile(int user_id)
        {//start of method
            User? user = _db.Users.Include(u => u.Role).AsNoTracking().FirstOrDefault(u => u.UserId == user_id);
            if (user == null) return db_result.fail(401, "Please log in again.");
            return db_result.ok(UserDto.From(user));
        }//end of method

        //method to update the logged in user's own profile
        public db_result update_profile(int user_id, string fullname, string email)
        {//start of method
            if (string.IsNullOrWhiteSpace(fullname) || fullname.Length > 100)
                return db_result.fail(400, "Full name is required (max 100 characters).");

            string clean_email = (email ?? "").Trim().ToLowerInvariant();
            if (clean_email.Length > 150 || !new EmailAddressAttribute().IsValid(clean_email))
                return db_result.fail(400, "A valid email is required.");

            User? user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.UserId == user_id);
            if (user == null) return db_result.fail(401, "Please log in again.");

            if (_db.Users.Any(u => u.Email == clean_email && u.UserId != user_id))
                return db_result.fail(409, "Email already in use.");

            user.FullName = fullname.Trim();
            user.Email = clean_email;
            _db.SaveChanges();

            return db_result.ok(UserDto.From(user));
        }//end of method

    }//end of class
}//end of namespace
