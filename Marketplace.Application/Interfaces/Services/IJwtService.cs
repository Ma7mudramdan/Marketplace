using Marketplace.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(IIdentityUser user, IList<string> roles);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
    }
}
