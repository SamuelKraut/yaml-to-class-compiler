public class Configuration
{
    public class HttpConfiguration
    {
        public class RoutersConfiguration
        {
            public class ApiConfiguration
            {
                public string Rule { get; set; }
                public string Service { get; set; }
                public List<String> Middlewares { get; set; }
                public List<String> Entrypoints { get; set; }
            }
            public ApiConfiguration Api { get; set; }
            public class TlsConfiguration
            {
                public string Certresolver { get; set; }
            }
            public TlsConfiguration Tls { get; set; }
        }
        public RoutersConfiguration Routers { get; set; }
        public class ServicesConfiguration
        {
            public class ApiConfiguration
            {
                public class LoadbalancerConfiguration
                {
                    public class ServerConfiguration
                    {
                        public string Port { get; set; }
                    }
                    public ServerConfiguration Server { get; set; }
                }
                public LoadbalancerConfiguration Loadbalancer { get; set; }
            }
            public ApiConfiguration Api { get; set; }
        }
        public ServicesConfiguration Services { get; set; }
        public class MiddlewaresConfiguration
        {
            public class ApiauthConfiguration
            {
                public class BasicauthConfiguration
                {
                    public List<String> Users { get; set; }
                }
                public BasicauthConfiguration Basicauth { get; set; }
                public List<String> Entrypoints { get; set; }
                public class TlsConfiguration
                {
                    public string Certresolver { get; set; }
                }
                public TlsConfiguration Tls { get; set; }
            }
            public ApiauthConfiguration Apiauth { get; set; }
            public class Fail2banConfiguration
            {
                public class PluginConfiguration
                {
                    public class Fail2banConfiguration
                    {
                        public string Loglevel { get; set; }
                        public string Bantime { get; set; }
                        public string Findtime { get; set; }
                        public string Maxretry { get; set; }
                        public string Enabled { get; set; }
                    }
                    public Fail2banConfiguration Fail2ban { get; set; }
                }
                public PluginConfiguration Plugin { get; set; }
            }
            public Fail2banConfiguration Fail2ban { get; set; }
        }
        public MiddlewaresConfiguration Middlewares { get; set; }
    }
    public HttpConfiguration Http { get; set; }
}
