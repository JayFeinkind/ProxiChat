using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public interface IRepositoryResult<TEntity>
    {
        string ResultDescription { get; set; }

        TEntity ResultData { get; set; }

        ResultCode ResultCode { get; set; }

        Exception Exception { get; set; }
    }
}
