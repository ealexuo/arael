using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IEncryptPasswordServicio
    {
        string HashPassword(string password);

        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
