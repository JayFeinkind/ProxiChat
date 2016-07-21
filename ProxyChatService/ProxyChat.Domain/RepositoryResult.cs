using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public class RepositoryResult<TEntity> : IRepositoryResult<TEntity>
    {
        public string ResultDescription { get; set; }
    }
}
