using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Dtos
{
    public class MembershipDto
    {
        public int Id { get; set; }
        public DateTime CreatedUTC { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] Salt { get; set; }
        public string TextPassword { get; set; }
    }
}
