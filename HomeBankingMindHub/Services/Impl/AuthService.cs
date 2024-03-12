using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace HomeBankingMindHub.Services.Impl
{
    public class AuthService : IAuthService
    {

        public AuthService()
        {
        }

        public ClaimsIdentity AuthenticateUser(Client user)
        {

            // Creación de claims (Datos del usuario a autenticar)
            var claims = new List<Claim>();
            if (user.Email.Equals("admin@gmail.com"))
            {
                claims.Add(new Claim("Admin", user.Email));
            }
            else
            {
                claims.Add(new Claim("Client", user.Email));
            }

            // Creación del objeto que contendrá la info del usuario y el metodo de autenticacion.
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
                );

            return claimsIdentity;
        }
    }
}
