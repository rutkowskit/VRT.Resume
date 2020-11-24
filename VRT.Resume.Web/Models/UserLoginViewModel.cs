using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace VRT.Resume.Web.Models
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
                PersonId = ""
            };            
        }
        private UserLoginViewModel()
        {
        }
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

            if (claims == null || !claims.ContainsKey(ClaimTypes.Email))
                return null;
            return new UserLoginViewModel()
            {
                Email = claims.GetValueOrDefault(ClaimTypes.Email),
                FirstName = claims.GetValueOrDefault(ClaimTypes.GivenName),
                LastName = claims.GetValueOrDefault(ClaimTypes.Surname),
                PersonId = claims.GetValueOrDefault("PersonId")
            };            
        }
    }
}