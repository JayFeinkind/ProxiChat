using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyChat.Accounts.Models;
using ProxyChat.Domain;
using ProxyChat.Accounts.Dtos;

namespace ProxyChat.Accounts.Repositories
{
    public class UserRepository : BaseRepository<AccountsContext, User, UserDto>, IUserRepository<AccountsContext, User, UserDto>
    {

    }
}
