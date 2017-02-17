using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Slalom.Stacks.Domain;

namespace Slalom.Stacks.TestStack
{
    public class Scenario
    {
        public Scenario()
        {
            ClaimsPrincipal.ClaimsPrincipalSelector = () => this.User;
        }

        public InMemoryEntityContext EntityContext { get; set; } = new InMemoryEntityContext();

        public ClaimsPrincipal User { get; set; }

        public Scenario WithUser(string userName, params string[] roles)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            this.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return this;
        }

        public Scenario WithData(params IAggregateRoot[] items)
        {
            this.EntityContext.AddAsync(items).Wait();

            return this;
        }

        public Scenario AsAdmin()
        {
            this.WithUser("admin@admin.com", "Administrator");

            return this;
        }
    }
}