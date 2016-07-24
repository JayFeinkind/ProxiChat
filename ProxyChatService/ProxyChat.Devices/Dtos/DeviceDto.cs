﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyChat.Devices.Dtos
{
    /// <summary>
    /// NOTE: Device identifier is a unique identifier generated by mobile device.  It is only unique by product so
    /// eventually a column will need to added to hold the manufacturer to avoid collision.
    /// </summary>
    public class DeviceDto
    {
        public int Id { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime? ModifiedUTC { get; set; }
        
        public long VersionNumber { get; set; }
        public string DeviceIdentifier { get; set; }
    }
}