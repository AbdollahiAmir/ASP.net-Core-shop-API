using BackEnd.DTOs.Users;

namespace BackEnd.IServices
{
    public interface IUsersService
    {
        Task<IReadOnlyList<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(string id);
        Task<(bool Succeeded, IEnumerable<string> Errors)> EditAsync(string id, EditUserDto dto);
        Task<(bool Succeeded, IEnumerable<string> Errors)> DeleteAsync(string id, string? currentUserId);
    }
}
