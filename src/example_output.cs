using System;
using System.Collections.Generic;
public static class Config
{
    public static class Http
    {
        public static class Routers
        {
            public static class Api
            {
                public static string Rule;
                public static string Service;
                public static IEnumerable<string> Middlewares;
                public static IEnumerable<string> Entrypoints;
            }

            public static class TLS
            {
                public static string CertResolver;
            }
        }

        public static class Services
        {
            public static class Api
            {
                public static class Loadbalancer
                {
                    public static class Server
                    {
                        public static int Port;
                    }
                }
            }
        }

        public static class Middlewares
        {
            public static class ApiAuth
            {
                public static IEnumerable<string> Entrypoints;
                public static class BasicAuth 
                {
                    public static IEnumerable<string> Users;
                    
                }
            }
            public static class Fail2ban
            {
                public static class Plugin
                {
                    public static string Loglevel;
                    public static string Bantime;
                    public static string Findtime;
                    public static int Maxretry;
                    public static bool Enabled;
                }
            }
        }
    }
}

