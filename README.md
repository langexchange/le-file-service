Consul config:
{
  "Consul": {
    "Enabled": true,
    "PingEnabled": true,
    "PingEndpoint": "ping",
    "PingInterval": 30,
    "RemoveAfterInterval": 61,
    "RequestRetries": 3,
    "Service": "file-service",
    "Address": "http://le-file-service:80",
    "Port": "80"
  },
  
  Setup consul
  var address = consulOptions.Address;
  var uri = new Uri(consulOptions.Address);
  _registration = new AgentServiceRegistration
  {
      Address = uri.Host,
      Port = consulOptions.Port
  };

  var httpCheck = new AgentServiceCheck
  {
      HTTP = $"http://{uri.Host}{(_registration.Port > 0 ? $":{_registration.Port}" : string.Empty)}/{pingEndpoint}"
  };
  _registration.Checks = new[] { httpCheck };
