using System;
using System.Collections.Generic;
public class Config
{
    public HttpConfig Http { get; set; }
    public class HttpConfig
    {
        public class RoutersConfig
        {
            public ApiConfig Api { get; set; }
            public class ApiConfig
            {
                public string Rule { get; set; };
                public string Service { get; set; };
                public IEnumerable<string> Middlewares { get; set; };
                public IEnumerable<string> Entrypoints { get; set; };
            }

            public class TlsConfig
            {
                public string CertResolver { get; set; };
            }
        }

        public class ServicesConfig
        {
            public ApiConfig Api { get; set; }
            public class ApiConfig
            {
                public LoadbalancerConfig Loadbalancer { get; set; }
                public class LoadbalancerConfig
                {
                    public ServerConfig Server { get; set; }
                    public class ServerConfig
                    {
                        public int Port { get; set; };
                    }
                }
            }
        }

        public class MiddlewaresConfig
        {
            public ApiAuthConfig ApiAuth { get; set; }
            public Fail2banConfig Fail2Ban { get; set; }
            public class ApiAuthConfig
            {
                public IEnumerable<string> Entrypoints { get; set; };
                public BasicAuthConfig BasicAuth { get; set; }
                public class BasicAuthConfig 
                {
                    public IEnumerable<string> Users { get; set; };
                    
                }
            }
            public class Fail2banConfig
            {
                public PluginConfig Plugin { get; set; }
                public class PluginConfig
                {
                    public string Loglevel { get; set; };
                    public string Bantime { get; set; };
                    public string Findtime { get; set; };
                    public int Maxretry { get; set; };
                    public bool Enabled { get; set; };
                }
            }
        }
    }
}

