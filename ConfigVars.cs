using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDFRobot
{
    public sealed class ConfigVars
    {
        public string RobotID = string.Empty;
        public string IOTToken = string.Empty;
        public string EnpointUrl = string.Empty;
        public string IronMQUrl = string.Empty;
        public string IronMQProjectID = string.Empty;
        public string IronMQToken = string.Empty;
        private ConfigVars()
        {
            RobotID = Environment.GetEnvironmentVariable("robotID");
            IOTToken = Environment.GetEnvironmentVariable("iotToken");
            EnpointUrl = Environment.GetEnvironmentVariable("endpointURL");
            IronMQUrl = Environment.GetEnvironmentVariable("queueURL");
            IronMQProjectID = Environment.GetEnvironmentVariable("IRON_MQ_PROJECT_ID");
            IronMQToken = Environment.GetEnvironmentVariable("IRON_MQ_TOKEN");

            //for local testing
            //RobotID = "ROB000909R";
            //IOTToken = "Bearer 279OC5MYsEqkVwBdWqTMMixlePISTXgfvLUFJJ8luCScsIq8P4svqPfjhEhawnuzAt20KiqvT9ae36XM3YXcFU";
            //EnpointUrl = "https://dashboard.us3.sfdcnow.com/clusters/xcdvudaz0dz3/inputstreams/robot_event_strea001/connections/robot001";
            //IronMQUrl = "https://mq-aws-eu-west-1-1.iron.io/3/projects/57e6b7b6ce60dc0007dd3562/queues/robot_messages";
            //IronMQToken = "HniYT41oHGDUURz0DcXu";
        }
        public static ConfigVars Instance { get { return ConfigVarInstance.Instance; } }

        private class ConfigVarInstance
        {
            static ConfigVarInstance()
            {
            }

            internal static readonly ConfigVars Instance = new ConfigVars();
        }
    }
}
