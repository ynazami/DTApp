using System.Collections.Generic;
using System.Threading.Tasks;
using DTApp.API.Helper;
using DTApp.API.Models;

namespace DTApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;

         Task<bool> SaveAll();

         Task<PagedList<User>> GetUsers(UserParams userParams);

         Task<User> GetUser(int userID);

         Task<Photo> GetPhoto(int id);

         Task<Like> GetLike(int userId, int recipientId);
    }
}