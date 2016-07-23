using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Repositories
{
    public interface IMembershipRepository<AccountsContext, Membership, MembershipDto> : IBaseRepository<AccountsContext, Membership, MembershipDto>
    {
        bool ValidatePassword(int userId, string password);
    }
}
