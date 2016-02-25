using MABMoney.Services.DTO;

namespace MABMoney.Services
{
    public interface IUserServices
    {
        UserDTO Get(int id);
        UserDTO GetByEmailAddress(string email);
        UserDTO GetByPasswordResetGUID(string guid);
        void Save(UserDTO dto);
        void ResetPassword(string guid, string newPassword);
        void Delete(int id);
    }
}
