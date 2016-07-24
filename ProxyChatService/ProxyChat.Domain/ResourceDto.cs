using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Domain
{
    public class ResourceDto
    {
        [DataMember(EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime CreatedUTC { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public long VersionNumber { get; set; }
    }
}
