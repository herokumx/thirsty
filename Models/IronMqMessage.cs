﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDFRobot.Models
{
    public class IronMQMessage
    {
        public string id { get; set; }
        public string body { get; set; }
        public string reservation_id { get; set; }
        public int reserved_count { get; set; }
        public string available_at { get; set; }
    }
}
