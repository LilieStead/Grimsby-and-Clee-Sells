using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Grimsby_and_Clee_Sells.UserSession
{
    public class JWTGen
    {
        private readonly SecretKeyGen _secretKeyGen;
        public JWTGen(SecretKeyGen secretKeyGen)
        {
            _secretKeyGen = secretKeyGen;
        }
        public string Generate(string userid, string username, string firstname, string lastname)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var kid = "access-token-key";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKeyGen.SecretKey))
            {
                KeyId = kid,
            };
            // make key with userid username firstname and last name
            var desc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim ("sub",userid),
                    new Claim ("username", username),
                    new Claim("firstname", firstname),
                    new Claim("lastname", lastname)
                }),
                //add Expiry after 5 days 
                Expires = DateTime.UtcNow.AddDays(5),
                Issuer = "GrimsbyAndCleeSells",
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };//create the token
            var create = tokenHandler.CreateToken(desc);
            return tokenHandler.WriteToken(create);
        }
    }
}
