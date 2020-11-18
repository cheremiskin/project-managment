using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace project_managment.Authentication
{
    public static class AuthenticationOptions
    {
        public const string Issuer = "PmCorp";
        public const string Audience = "Pm";
        private const string Key = "A32C3F9H7HD83BALV83OCD910D83JAHFB3KI";
        public const int Lifetime = 90;
        public const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}