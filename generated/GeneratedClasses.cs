public class Configuration
{
    public HttpConfiguration Http { get; set; }
    public class HttpConfiguration
    {
        public RoutersConfiguration Routers { get; set; }
        public class RoutersConfiguration
        {
            public ApiConfiguration Api { get; set; }
            public class ApiConfiguration
            {
                public string Rule { get; set; }
                public string Service { get; set; }
                public List<string> Middlewares { get; set; }
                public List<string> Entrypoints { get; set; }

            public TlsConfiguration Tls { get; set; }
            public class TlsConfiguration
            {
                public string Certresolver { get; set; }


        public ServicesConfiguration Services { get; set; }
        public class ServicesConfiguration
        {
            public ApiConfiguration Api { get; set; }
            public class ApiConfiguration
            {
                public LoadbalancerConfiguration Loadbalancer { get; set; }
                public class LoadbalancerConfiguration
                {
                    public ServerConfiguration Server { get; set; }
                    public class ServerConfiguration
                    {
                        public string Port { get; set; }




        public MiddlewaresConfiguration Middlewares { get; set; }
        public class MiddlewaresConfiguration
        {
            public ApiAuthConfiguration ApiAuth { get; set; }
            public class ApiAuthConfiguration
            {
                public BasicauthConfiguration Basicauth { get; set; }
                public class BasicauthConfiguration
                {
                    public List<string> Users { get; set; }

                public List<string> Entrypoints { get; set; }
                public TlsConfiguration Tls { get; set; }
                public class TlsConfiguration
                {
                    public string Certresolver { get; set; }


            public Fail2banConfiguration Fail2ban { get; set; }
            public class Fail2banConfiguration
            {
                public PluginConfiguration Plugin { get; set; }
                public class PluginConfiguration
                {
                    public Fail2banConfiguration Fail2ban { get; set; }
                    public class Fail2banConfiguration
                    {
                        public string LogLevel { get; set; }
                        public string Bantime { get; set; }
                        public string Findtime { get; set; }
                        public string Maxretry { get; set; }
                        public string Enabled { get; set; }






