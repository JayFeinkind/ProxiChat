﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public interface IRepositoryResult<TEntity>
    {
        string ResultDescription { get; set; }
    }
}
