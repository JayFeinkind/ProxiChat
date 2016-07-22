using ProxyChat.Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Dtos
{
    public class DeviceTokenDto
    {
        public int Id { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime? ModifiedUTC { get; set; }
       
        public long VersionNumber { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public int DeviceId { get; set; }

        public Device Device { get; set; }
        public User User { get; set; }
    }
}
