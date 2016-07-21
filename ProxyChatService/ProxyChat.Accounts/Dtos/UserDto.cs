using ProxyChat.Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime? ModifiedUTC { get; set; }
       
        public long VersionNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        public IEnumerable<DeviceToken> DeviceTokens { get; set; }
    }
}
