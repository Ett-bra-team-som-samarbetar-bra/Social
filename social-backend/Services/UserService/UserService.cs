using Microsoft.EntityFrameworkCore.Storage;
using SocialBackend.Exceptions;

namespace SocialBackend.Services
{
    public interface IUserService
    {
        Task DeleteUser(int userId);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(UserIdRequest request);
        Task UpdatePassword(UpdatePasswordRequest request, int userId);
    }
    public class UserService(DatabaseContext dbContext, IPasswordHelper passwordHelper) : IUserService
    {
        private readonly IDatabaseContext _db = dbContext;
        private readonly IPasswordHelper _passwordHelper = passwordHelper;

        public async Task<User> GetUserById(UserIdRequest request)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId)
                ?? throw new UserNotFoundException($"No user with Id {request.UserId} was found");
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task DeleteUser(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new UserNotFoundException($"No user with Id {userId} was found");
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdatePassword(UpdatePasswordRequest request, int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new UserNotFoundException($"No user with Id {userId} was found");
            user.PasswordHash = _passwordHelper.HashPassword(request.NewPassword);
        }
    }
}