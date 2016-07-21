using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public enum ResultCode
    {
        Ok,		            
        NoContent,		    
        Created,		    
        NotModified,	    
        InvalidData,	    
        Forbidden,
        NotFound,		    
        DataConflict,	    
        Error,			    
    }
}
