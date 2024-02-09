using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Grimsby_and_Clee_Sells.UserSession
{
    public class DecodeJWT
    {
        private readonly SecretKeyGen _secretKeyGen;
        public DecodeJWT(SecretKeyGen secretKeyGen)
        {
            _secretKeyGen = secretKeyGen;
        }
        public (string userid, string username, string firstname, string lastname) DecodeToken (string token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var param = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKeyGen.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = "GrimsbyAndCleeSells",
                ValidateAudience = false
            };

            try
            {
                //return validtoken,userID,Username, Firstname,Lastname 
                var validtoken = tokenhandler.ValidateToken(token, param, out SecurityToken validationToken);
                var userID = (validationToken as JwtSecurityToken)?.Claims?.FirstOrDefault(claim => claim.Type == "sub");
                var Username = (validationToken as JwtSecurityToken)?.Claims?.FirstOrDefault(claim => claim.Type == "username");
                var Firstname = (validationToken as JwtSecurityToken)?.Claims?.FirstOrDefault(claim => claim.Type == "firstname");
                var Lastname = (validationToken as JwtSecurityToken)?.Claims?.FirstOrDefault(claim => claim.Type == "lastname");

                if (userID != null && Username != null && Firstname != null && Lastname != null)
                {
                    return(userID.Value,  Username.Value, Firstname.Value, Lastname.Value);
                }
                //error handing if api could return users details 
                return (null, null, null, null);
            }
            catch (Exception ex)
            {
                return(null, null, null, null);
            }
        }
    }
}
