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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}