public interface IUserService
{
    Task DeleteUser(UserIdRequest request);
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(UserIdRequest request);
    Task UpdatePassword(UpdatePasswordRequest request, int userId);
}