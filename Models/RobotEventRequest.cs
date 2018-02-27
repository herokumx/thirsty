using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDFRobot.Models
{
    public class RobotEventRequest
    {
        public string robotID { get; set; }
        public string factoryID { get; set; }
        public string robotEvent { get; set; }
        public string controllerEvent { get; set; }
        public int tempC { get; set; }
        public int timestamp { get; set; }
    }
}
