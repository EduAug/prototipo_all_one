using all_one_backend.DTOs;
using all_one_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace all_one_backend.Controllers
{
    [ApiController]
    [Route("deeper/[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly AllOneDatabContext _context;

        public TopicsController(AllOneDatabContext context)
        {
            _context = context;
        }

        [HttpGet("byName")]
        public IActionResult GetAllTopics(string thisTopicName, int? userId) 
        {
            if (userId != null)
            {
                var user = _context.Users
                    .Include(u => u.Topics)
                    .FirstOrDefault(u => u.Id == userId);

                if(user == null) return NotFound("User not found.");

                var userTopicIds = user.Topics.Select(t => t.Id).ToList();

                var matchingTopics = _context.Topics
                    .Where(t => t.TopicName.Contains(thisTopicName.ToLower()) && !userTopicIds.Contains(t.Id) && t.TotalVotes >= 75)
                    .Select(t => new { Id = t.Id, Name = t.TopicName })
                    .ToList();

                return Ok(matchingTopics);
            }
            else
            {
                var matchingTopics = _context.Topics
                    .Where(a => a.TopicName.Contains(thisTopicName.ToLower()) && a.TotalVotes >= 75)
                    .Select(b => new { Id = b.Id, Name = b.TopicName })
                    .ToList();

                return Ok(matchingTopics);
            }
        }

        [HttpGet("topApproved")]
        public async Task<IActionResult> GetTopFiveApprovedTopics()
        {
            var topApprovedTopics = await _context.Topics
                .Where(t => t.TotalVotes >= 75)
                .OrderByDescending(t => t.Subscribers)
                .Take(5)
                .Select(t => new { Id = t.Id, Name = t.TopicName, Subscribers = t.Subscribers })
                .ToListAsync();

            return Ok(topApprovedTopics);
        }

        [HttpPost("subscribe")]
        public IActionResult SubscribeToTopic(int userId, int topicId)
        {
            var user = _context.Users.Include(u => u.Topics).FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound("User not found");

            var topic = _context.Topics.Include(t => t.Users).FirstOrDefault(t => t.Id == topicId);
            if (topic == null) return NotFound("Topic not found");

            if (user.Topics.Any(t => t.Id == topicId)) return BadRequest($"User already subscribed to {topic.TopicName}");

            user.Topics.Add(topic);
            topic.Subscribers++;

            _context.SaveChanges();

            return Ok($"Subscribed user {user.DisplayName} to topic {topic.TopicName}");
        }

        [HttpGet("{userId}/topics")]
        public IActionResult GetUserTopics(int userId)
        {
            var user = _context.Users.Include(u => u.Topics).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var subscribedTopics = user.Topics.Select(t => new { t.Id, t.TopicName }).ToList();

            return Ok(subscribedTopics);
        }

        // ------------------------------------- Voting v | Fetching ^ ----------

        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestTopic([FromBody] SuggestTopicDTO suggestedTopic)
        {
            if (string.IsNullOrWhiteSpace(suggestedTopic.InformedTopic)) return BadRequest("Can't suggest an empty topic moron");

            string treadtedTopic = Regex.Replace(suggestedTopic.InformedTopic,"[^a-zA-Z0-9]", "").ToUpper();

            var topics = await _context.Topics.Include(t=>t.Users).Include(t=>t.Votes).ToListAsync();

            int alreadyExistingOrSimilarTopicCounter = topics.Count(t =>
            Regex.Replace(t.TopicName,"[^a-zA-Z0-9]","").ToUpper().Contains(treadtedTopic));

            const int maxExistingOrSimilarTopic = 1;

            if(alreadyExistingOrSimilarTopicCounter >= maxExistingOrSimilarTopic) return BadRequest("Similar topic already exists, try looking for it");

            Topic newTopic = new Topic
            {
                TopicName = suggestedTopic.InformedTopic,
                Subscribers = 0,
                TotalVotes = 1
            };

            _context.Topics.Add(newTopic);
            await _context.SaveChangesAsync();

            Vote newVote = new Vote
            {
                UserId = suggestedTopic.UserId,
                TopicId = newTopic.Id,
                VoteDate = DateTime.Now
            };

            _context.Votes.Add(newVote);
            await _context.SaveChangesAsync();

            return Ok(new { newVote.UserId, newVote.TopicId, newVote.VoteDate, suggestedTopic.InformedTopic });
        }

        [HttpGet("getUnapproved")]
        public async Task<IActionResult> GetUnapprovedTopics(int userId, string? queriedTopic)
        {
            var user = await _context.Users.Include(u => u.Votes).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("User not found");

            var userVotedTopicIds = user.Votes.Select(v=>v.TopicId).ToList();

            if(!string.IsNullOrWhiteSpace(queriedTopic))
            {
                var treatedQuery = Regex.Replace(queriedTopic, "[^a-zA-Z0-9]", "").ToUpper();
                var matchingUnapprovedTopics = await _context.Topics
                    .Where(
                        t => t.TotalVotes < 75 &&
                        !userVotedTopicIds.Contains(t.Id)
                        )
                    .OrderByDescending(t => t.TotalVotes)
                    .ToListAsync();

                var returnTopics = matchingUnapprovedTopics
                    .Where(t => Regex.Replace(t.TopicName, "[^a-zA-Z0-9]", "").ToUpper().Contains(treatedQuery))
                    .Select(t => new { 
                        Id = t.Id, 
                        Name = t.TopicName, 
                        Votes = t.TotalVotes 
                    })
                    .ToList();

                return Ok(returnTopics);
            }
            else
            {
                var unapprovedTopics = await _context.Topics
                    .Where(
                        t => t.TotalVotes < 75 &&
                        !userVotedTopicIds.Contains(t.Id)
                        )
                    .OrderByDescending (t => t.TotalVotes)
                    .Select(t => new { Id = t.Id, Name = t.TopicName, Votes = t.TotalVotes })
                    .ToListAsync();

                return Ok(unapprovedTopics);
            }
        }

        [HttpPost("voteForTopic")]
        public async Task<IActionResult> VoteForTopic([FromBody] VoteForTopicDTO votingDTO)
        {
            if(votingDTO.userId <= 0 || votingDTO.topicId <= 0) return BadRequest("One or more invalid ID");

            var user = await _context.Users
                .Include(u=> u.Topics)
                .Include(u=>u.Votes)
                .FirstOrDefaultAsync(u => u.Id == votingDTO.userId);
            if (user == null) return BadRequest("User not found");

            var topic = await _context.Topics
                .Include(t => t.Votes)
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == votingDTO.topicId);
            if (topic == null) return BadRequest("Topic not found");

            var alreadyVoted = await _context.Votes.FirstOrDefaultAsync(v=>v.UserId == votingDTO.userId && v.TopicId == votingDTO.topicId);
            if (alreadyVoted != null) return BadRequest("User already voted for this topic.");

            Vote newV = new Vote
            {
                UserId = votingDTO.userId,
                TopicId = votingDTO.topicId,
                VoteDate = DateTime.UtcNow
            };

            await _context.Votes.AddAsync(newV);
            topic.TotalVotes += 1;

            if(topic.TotalVotes >= 75)
            {
                var usersWhoVotedForTopic = await _context.Users
                    .Where(u => u.Votes.Any(v => v.TopicId == topic.Id))
                    .Include(u => u.Topics)
                    .ToListAsync();

                foreach (var u in usersWhoVotedForTopic)
                {
                    if(!u.Topics.Any(t=> t.Id == topic.Id))
                    {
                        u.Topics.Add(topic);
                    }
                }
            }
            await _context.SaveChangesAsync();

            return Ok("Vote submitted successfully!");
        }
    }
}
