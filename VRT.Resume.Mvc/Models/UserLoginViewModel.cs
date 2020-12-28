using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace VRT.Resume.Mvc.Models
{
    public class UserLoginViewModel
    {
        public static UserLoginViewModel Empty
        {
            get => new UserLoginViewModel()
            {
                Email = "",
                FirstName = "",
                LastName = "",
                PersonId = "",
                UserId = ""
            };            
        }
        private UserLoginViewModel()
        {
        }
        public string UserId { get; private set; }
        public string Email { get; private set; }        
        public string FirstName { get; private set; }
        public string LastName { get; private set; }  
        public string PersonId { get; private set; }
        
        public string GetInitials()
        {
            var result = new List<char>(2);
            if (!string.IsNullOrEmpty(FirstName) && FirstName.Length > 0)
                result.Add(FirstName[0]);
            if (!string.IsNullOrEmpty(LastName) && LastName.Length > 0)
                result.Add(LastName[0]);            
            
            return result.Count>0 
                ? new string(result.ToArray())
                : "?";
        }

        internal static UserLoginViewModel Create(ClaimsIdentity identity)
        {
            var claims = identity?.Claims
                ?.GroupBy(g => g.Type)
                .ToDictionary(k => k.Key, v => v.Select(s => s.Value).FirstOrDefault());

            if (claims == null || claims.Count==0)
                return null;
            var userId = claims.GetValueOrDefault(ClaimTypes.Email, CreateUserId(identity));                
            return new UserLoginViewModel()
            {
                UserId = userId,
                Email = claims.GetValueOrDefault(ClaimTypes.Email),
                FirstName = claims.GetValueOrDefault(ClaimTypes.GivenName, "?"),
                LastName = claims.GetValueOrDefault(ClaimTypes.Surname, "?"),
                PersonId = claims.GetValueOrDefault("PersonId")
            };            
        }
        private static string CreateUserId(ClaimsIdentity identity)
        {
            var id = identity?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (id == null) return null;
            return $"{id.Value}@{id.Issuer}";
        }
    }
}