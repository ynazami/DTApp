using System.Collections.Generic;
using System.Threading.Tasks;
using DTApp.API.Helper;
using DTApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace DTApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            this._context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(m => m.LikerId == userId && m.LikeeId == recipientId);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int userID)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == userID);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(p => p.LastActive).AsQueryable();
            users = users.Where(p => p.Id != userParams.UserId && p.Gender == userParams.Gender);

            

            if(userParams.MinimumAge != 18 || userParams.MaximumAge != 99)
            {
                var minDOB = DateTime.Today.AddYears(-userParams.MaximumAge-1);
                var maxDOB = DateTime.Today.AddYears(-userParams.MinimumAge);
                users = users.Where(m => m.DateOfBirth >= minDOB && m.DateOfBirth <= maxDOB);
            }

            

            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);    
                users = users.Where(p => userLikers.Contains(p.Id));
            }

            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);    
                users = users.Where(p => userLikees.Contains(p.Id));
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy)) {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(p => p.Created);
                        break;
                    default:
                        users = users.OrderByDescending(p => p.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int userId, bool likers)
        {
            var user = await _context.Users.Include(p => p.Likees).Include(p => p.Likers).FirstOrDefaultAsync(p => p.Id == userId);

            if(likers)
            {
                return user.Likers.Where(u => u.LikeeId == userId).Select(p => p.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == userId).Select(p => p.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.Include(p => p.Sender).ThenInclude(m => m.Photos)
                                            .Include(p => p.Recipient).ThenInclude(m => m.Photos).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(p => p.Sender).ThenInclude(m => m.Photos)
                                            .Include(p => p.Recipient).ThenInclude(m => m.Photos).AsQueryable();
            switch(messageParams.MessageContainer)    
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && m.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && m.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId &&  m.IsRead == false && m.RecipientDeleted == false);
                    break;
            }
            messages = messages.OrderByDescending(m => m.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetConversation(int senderId, int recipientId)
        {
            var messages = await _context.Messages.Include(p => p.Sender).ThenInclude(m => m.Photos)
                                            .Include(p => p.Recipient).ThenInclude(m => m.Photos).Where(
                                                m => (m.SenderId == senderId && m.SenderDeleted == false && m.RecipientId == recipientId) ||
                                                     (m.RecipientId == senderId && m.RecipientDeleted == false && m.SenderId == recipientId)
                                            ).OrderByDescending(m => m.MessageSent).ToListAsync();
            return messages;
        }

        
    }
}