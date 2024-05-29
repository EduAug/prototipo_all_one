
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using all_one_backend.Models;
using all_one_backend.DTOs;
using all_one_backend.Auth;

namespace all_one_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AllOneDatabContext _context;
        private readonly CassandraDAO _cassandraDAO;
        private readonly IJwtHandler _jwtHandler;

        public UsersController(AllOneDatabContext context, CassandraDAO cassandraDAO, IJwtHandler jwtHandler)
        {
            _context = context;
            _cassandraDAO = cassandraDAO;
            _jwtHandler = jwtHandler;
        }

        [HttpGet("getAll")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpGet("friendsOf/{userId}")]
        public async Task<IActionResult> GetUserFriends(int userId) 
        {
            try
            {
                var user = await _context.Users.Include(f => f.Friends).FirstOrDefaultAsync(u => u.Id == userId);
                if(user == null) return NotFound("User not found.");
                var foundFriends = user.Friends.Select(friend => new { Id = friend.Id, DisplayName = friend.DisplayName }).ToList();
                return Ok(foundFriends);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
                return StatusCode(500, $"Something went wrong while fetching user friends: {E.Message}");
            }
        }

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound("User not found");

            var userProfile = new
            {
                user.DisplayName,
                user.Email,
                user.Password,
                user.Birthday,
                user.Latitude,
                user.Longitude
            };

            return Ok(userProfile);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserCreationDTO user)
        {
            try
            {
                var nUser = new User
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Password = user.Password,
                    Birthday = user.Birthday,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude
                };
                _context.Users.Add(nUser);

                foreach (var topicIdThatTheUserSubbedTo in user.TopicIds)
                {
                    var thisTopic = await _context.Topics
                        .Include(t => t.Users)
                        .FirstOrDefaultAsync(t => t.Id == topicIdThatTheUserSubbedTo);
                    thisTopic.Users.Add(nUser);
                }

                await _context.SaveChangesAsync();

                foreach (var topicThatTheUserSubbed in user.TopicIds)
                {
                    var topic = await _context.Topics.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == topicThatTheUserSubbed);
                    topic.Subscribers = topic.Users.Count;
                }

                await _context.SaveChangesAsync();

                return Ok($"User {user.DisplayName} created successfully!");
            } catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong, at {ex.Message}");
                return StatusCode(500, "There was an error while creating this user.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            var userLn = await _context.Users.SingleOrDefaultAsync(un => un.DisplayName == user.UserName && un.Password == user.Password);
            if (userLn == null)
            {
                return BadRequest("Incorrect Username/Password");
            }

            var token = _jwtHandler.GenerateToken(userLn.Id, userLn.DisplayName);

            return Ok(new { Token = token });
        }

        [HttpGet("findUsers")]
        public async Task<IActionResult> FindUsers(int userId, int maxDistance)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Topics)
                    .Include(u => u.Friends)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return NotFound("User not found");

                var orderedUsers = await _context.Users
                    .Where(u => u.Id != userId && !u.Friends.Any(f => f.Id == userId))
                    .Include(u => u.Topics)
                    .ToListAsync();

                orderedUsers = FilterUsersByDistance(orderedUsers, user.Latitude, user.Longitude, maxDistance);

                orderedUsers = orderedUsers.Where(u => u.Topics.Any(t => user.Topics.Any(ut => ut.Id == t.Id))).ToList();

                var userInfos = orderedUsers.Select(u => new
                {
                    u.Id,
                    u.DisplayName,
                    CommonTopicsCount = u.Topics.Count(t => user.Topics.Any(ut => ut.Id == t.Id)),
                    MatchingTopics = u.Topics.Where(t => user.Topics.Any(ut => ut.Id == t.Id)).Select(t => t.TopicName).ToList(),
                    Distance = CalculateDistance(user.Latitude, user.Longitude, u.Latitude, u.Longitude),
                    u.Latitude,
                    u.Longitude
                })
                    .OrderByDescending(u => u.CommonTopicsCount)
                    .ThenBy(u => u.Distance)
                    .ToList();

                return Ok(userInfos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding users: {ex.Message}");
                return StatusCode(500, "Error finding users");
            }
        }
        private List<User> FilterUsersByDistance(List<User> users, decimal? userLatitude, decimal? userLongitude, int maxDistance)
        {
            var filteredUsers = users.Where(u => CalculateDistance(userLatitude, userLongitude, u.Latitude, u.Longitude) <= maxDistance).ToList();
            return filteredUsers;
        }
        public static double CalculateDistance(decimal? lat1, decimal? lon1, decimal? lat2, decimal? lon2)
        {
            var dLat = DegreeToRadian(Convert.ToDouble(lat2) - Convert.ToDouble(lat1));
            var dLon = DegreeToRadian(Convert.ToDouble(lon2) - Convert.ToDouble(lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreeToRadian(Convert.ToDouble(lat1))) * Math.Cos(DegreeToRadian(Convert.ToDouble(lat2))) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = 6371 * c;
            return distance;
        }
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        [HttpPost("addFriend")]
        public async Task<IActionResult> AddFriend([FromBody] FriendRequestDTO uIds)
        {
            var uId1 = uIds.user1Id;
            var uId2 = uIds.user2Id;
            var user1 = await _context.Users.Include(z => z.Friends).FirstOrDefaultAsync(a => a.Id == uId1);
            var user2 = await _context.Users.Include(x => x.Friends).FirstOrDefaultAsync(b => b.Id == uId2);
            
            if(user1 == null || user2 == null) return NotFound("One or both users not found");
            if (user1.Friends.Contains(user2)) return BadRequest("User is already friends with you");
            
            user1.Friends.Add(user2);
            user2.Friends.Add(user1);

            await _context.SaveChangesAsync();
            return Ok($"Friendship establised between {user1.DisplayName} & {user2.DisplayName}");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserCreationDTO multipurposeDTO)
        {
            var topicsToAdd = new List<Topic>();
            var topicsToRemove = new List<Topic>();
            try
            {
                var user = await _context.Users.Include(t => t.Topics).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return NotFound("User not found");

                user.DisplayName = multipurposeDTO.DisplayName;
                user.Email = multipurposeDTO.Email;
                user.Password = multipurposeDTO.Password;
                user.Birthday = multipurposeDTO.Birthday;
                user.Latitude = multipurposeDTO.Latitude;
                user.Longitude = multipurposeDTO.Longitude;

                user.Topics.Clear();

                foreach (var topicId in multipurposeDTO.TopicIds)
                {
                    var thisTopic = await _context.Topics
                        .Include(t => t.Users)
                        .FirstOrDefaultAsync(t => t.Id == topicId);
                    if (thisTopic != null)
                    {
                        user.Topics.Add(thisTopic);
                        topicsToAdd.Add(thisTopic);
                    }
                }

                foreach (var topic in user.Topics.ToList())
                {
                    if (!multipurposeDTO.TopicIds.Contains(topic.Id))
                    {
                        topic.Users.Remove(user);
                        topicsToRemove.Add(topic);
                    }
                }

                foreach (var remove in topicsToRemove)
                    remove.Subscribers = remove.Users.Count;

                foreach (var add in topicsToAdd)
                    add.Subscribers = add.Users.Count;

                await _context.SaveChangesAsync();

                return Ok("User data updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user data: {ex.Message}");
                return StatusCode(500, "An error occurred while updating user data");
            }
        }

        [HttpDelete("final/delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                // Disable foreign key constraints
                await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");

                var userToDelete = await _context.Users.Include(f => f.Friends).Include(t => t.Topics).FirstOrDefaultAsync(u => u.Id == userId);
                if (userToDelete == null) return NotFound("User not found");

                foreach (var friend in userToDelete.Friends)
                {
                    friend.Users.Remove(userToDelete);
                }

                foreach (var topic in userToDelete.Topics)
                {
                    topic.Users.Remove(userToDelete);
                }

                await _context.SaveChangesAsync();

                _context.Users.Remove(userToDelete);

                await _context.SaveChangesAsync();

                // Re-enable foreign key constraints
                await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");

                return Ok("Goodbye user!");
            }
            catch (Exception E)
            {
                Console.WriteLine($"Something went wrong while deleting user: {E.Message}");
                return StatusCode(500, "An internal error occurred while trying to delete the user");
            }
        }

    }
}
