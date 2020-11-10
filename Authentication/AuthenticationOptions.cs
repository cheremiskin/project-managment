using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace project_managment.Authentication
{
    public static class AuthenticationOptions
    {
        public const string ISSUER = "PmCorp";
        public const string AUDIENCE = "Pm";
        private const string KEY = "A32C3F9H7HD83BALV83OCD910D83JAHFB3KI";
        public const int LIFETIME = 90;
        public const string SECURITY_ALGORITHM = SecurityAlgorithms.HmacSha256;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}