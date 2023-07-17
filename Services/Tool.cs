using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using restaurant_franchise.Data;

namespace restaurant_franchise.Services
{
    public static class Tool
    {

        private static string[] fileExtension = { "jpg", "jepg", "png", "gif" };
        public static string ClientDomain()
        {
            return "https://localhost:44461";
        }

        public static void Username(string? token, out string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var data = tokenHandler.ReadJwtToken(SplitBerear(token));
            IEnumerable<Claim> claims = data.Claims;
            username = claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
        }

        public static void validImages(string extension, out bool isValid, out string nameStamp)
        {
            bool valid = false;
            string stamp = "";
            if (extension != null)
            {
                var fileType = extension.Split(".")[extension.Split(".").Length - 1];
                for (int i = 0; i < fileExtension.Length; i++)
                {
                    if (fileExtension[i] == fileType)
                    {
                        stamp = DateTime.Now.Ticks.ToString() + '.' + fileType;
                        valid = true;
                    }
                }
                isValid = valid;
            }
            if (stamp == "") {
                nameStamp = "";
            }
            nameStamp = stamp;
            isValid = valid;
        }

        private static string SplitBerear(string token)
        {
            string? JwtToken = null;
            string[]? ActualToken = token.Split(" ");
            if (ActualToken.Length == 2)
            {
                int c = 0;
                foreach (var i in ActualToken)
                {
                    if (c == 1)
                    {
                        JwtToken = i;
                    }
                    c++;
                }
            }
            return JwtToken;
        }


        public static void ValidateJWT(string? token, out bool status, out string role)
        {
            // string? JwtToken = null;
            // if (token == null)
            // {
            //     status = false;
            // }
            // else
            // {
            //     string[]? ActualToken = token.Split(" ");
            //     if (ActualToken.Length == 2)
            //     {
            //         int c = 0;
            //         foreach (var i in ActualToken)
            //         {
            //             if (c == 1)
            //             {
            //                 JwtToken = i;
            //             }
            //             c++;
            //         }
            //     }
            // }
            string JwtToken = SplitBerear(token);
            string ClientRole = "";

            if (JwtToken != null)
            {
                byte[] secretKey = System.Text.Encoding.UTF8.GetBytes("my top secret key");
                var tokenHandler = new JwtSecurityTokenHandler();
                var data = tokenHandler.ReadJwtToken(JwtToken);

                IEnumerable<Claim> claims = data.Claims;
                string singleRole = claims.First(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                ClientRole = singleRole;
                try
                {
                    tokenHandler.ValidateToken(JwtToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                    status = true;
                }
                catch
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
            if (ClientRole != "")
            {
                role = ClientRole;
            }
            else
            {
                role = "";
            }
        }


    }
}