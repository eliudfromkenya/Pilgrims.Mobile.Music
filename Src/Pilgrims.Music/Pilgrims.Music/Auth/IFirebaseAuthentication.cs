using System.Threading.Tasks;
using Pilgrims.Music.Models;

namespace Pilgrims.Music.Auth
{
    public interface IFirebaseAuthentication
    {
        Task<UserModel> LoginWithEmailAndPassword(string email, string password);
        Task<bool> RegisterWithEmailAndPassword(string username, string email, string password);
        Task<bool> ForgetPassword(string email);
        string GetUsername();
        string GetUserId();
        bool IsLoggedIn();
        bool LogOut();
    }
}
