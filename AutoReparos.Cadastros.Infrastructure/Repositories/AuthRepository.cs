using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;
using AutoReparos.Cadastros.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace AutoReparos.Cadastros.Infrastructure.Repositories
{
    public class AuthRepository(UserManager<UsuarioIdentity> userManager) : IAuthRepository
    {
        private readonly UserManager<UsuarioIdentity> _userManager = userManager;

        public async Task<Usuario?> ValidateCredentialsAsync(string email, string password)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);

            if (identityUser == null)
            {
                return null;
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, password);

            if (!isPasswordValid)
            {
                return null;
            }

            return identityUser.ToDomain();
        }
    }
}
