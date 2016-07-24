using ProxyChat.Accounts.Models;
using ProxyChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Accounts.Dtos
{
    [DataContract]
    public class UserDto : ResourceDto
    {
        [DataMember(EmitDefaultValue = false)]
        public DateTime? ModifiedUTC { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public IEnumerable<DeviceToken> DeviceTokens { get; set; }

        public Guid? ResetPasswordToken { get; set; }
    }
}
