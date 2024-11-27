using System;

namespace TCS_Cliente.Services
{
    public interface IUserService
    {
        string Authenticate(string email, string senha); // Método para autenticação do usuário
    }
}
