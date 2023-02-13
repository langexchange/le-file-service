﻿using System;

namespace LE.Library.LE.Consul
{
    public class ConsulOptions
    {
        public string Url { get; set; }
        public string Service { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
        public string PingEndpoint { get; set; }
        public int PingInterval { get; set; }
        public int RemoveAfterInterval { get; set; }
        public int RequestRetries { get; set; }
        public bool SkipLocalhostDockerDnsReplace { get; set; }
    }
}
