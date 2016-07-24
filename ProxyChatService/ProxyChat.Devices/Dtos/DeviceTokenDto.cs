using ProxyChat.Devices.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Devices.Dtos
{
    public class DeviceTokenDto : ResourceDto
    {
        public DateTime? ModifiedUTC { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public int DeviceId { get; set; }

        public Device Device { get; set; }
        public User User { get; set; }
    }
}
