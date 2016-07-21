using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Repositories
{
    public interface IUserRepository<IDBContext, TEntity, TDto> : IBaseRepository<IDBContext, TEntity, TDto>
    {
    }
}
