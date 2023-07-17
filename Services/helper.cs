
using restaurant_franchise.Data;
using restaurant_franchise.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace restaurant_franchise.Services {
    public class Utility {
        public AuthDbContext _context;
        public Utility(AuthDbContext options) {
            this._context = options;
        }

         public string CreateToken(User user)
        {
            var RoleUserMap = _context.Roles.Where(x => x.UserId == user.Id).FirstOrDefault(); // role id of that user
            Console.WriteLine(RoleUserMap);
            if (RoleUserMap == null) return "";

            var actualRole = _context.UserRoles.Where(x => x.Id == RoleUserMap.RoleId).FirstOrDefault();

            if (actualRole == null) return "";
            // role assignment in jwt token is baed on assigned role in database 
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, actualRole.Name)
            };
            byte[] secretKey = System.Text.Encoding.UTF8.GetBytes("my top secret key");
            var key = new SymmetricSecurityKey(secretKey);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}