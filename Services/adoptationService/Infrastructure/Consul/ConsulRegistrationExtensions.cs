using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AnimalCatalog.API.Infrastructure.Consul
{
    public static class ConsulRegistrationExtensions
    {
        public static IApplicationBuilder RegisterWithConsul(
            this IApplicationBuilder app,
            IConfiguration config,
            IHostApplicationLifetime lifetime)
        {
            var consulCfg = config.GetSection("Consul");
            var consulAddr = consulCfg.GetValue<string>("Address");
            var serviceId = consulCfg.GetValue<string>("ServiceId");
            var serviceName = consulCfg.GetValue<string>("ServiceName");
            var serviceAddr = consulCfg.GetValue<string>("ServiceAddress");
            var servicePort = consulCfg.GetValue<int>("ServicePort");
            var tags = consulCfg.GetSection("Tags").Get<string[]>() ?? Array.Empty<string>();
            var intervalStr = consulCfg.GetValue<string>("HealthCheckInterval") ?? "00:00:10";
            var deregStr = consulCfg.GetValue<string>("DeregisterCriticalServiceAfter") ?? "00:01:00";

            var intervalTs = ParseDuration(intervalStr, TimeSpan.FromSeconds(10));
            var deregTs = ParseDuration(deregStr, TimeSpan.FromMinutes(1));

            var serviceUri = new Uri(serviceAddr);
            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Address = serviceUri.Host,
                Port = servicePort,
                Tags = tags,
                Check = new AgentServiceCheck
                {
                    HTTP = $"{serviceAddr}:{servicePort}/health",
                    Interval = intervalTs,
                    DeregisterCriticalServiceAfter = deregTs
                }
            };

            lifetime.ApplicationStarted.Register(() =>
            {
                _ = Task.Run(async () =>
                {
                    using var consul = new ConsulClient(c => c.Address = new Uri(consulAddr));
                    var attempt = 0;
                    var maxAttempts = 10;

                    while (attempt < maxAttempts)
                    {
                        try
                        {
                            attempt++;
                            await consul.Agent.ServiceRegister(registration);
                            Console.WriteLine($"[Consul] Registrado: {serviceName}:{servicePort} ({serviceId})");
                            break;
                        }
                        catch (HttpRequestException ex)
                        {
                            Console.WriteLine($"[Consul] Tentativa {attempt}/{maxAttempts} falhou: {ex.Message}");
                            await Task.Delay(TimeSpan.FromSeconds(Math.Min(5 * attempt, 30)));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Consul] Erro inesperado no registro: {ex}");
                            await Task.Delay(TimeSpan.FromSeconds(10));
                        }
                    }
                });
            });

            lifetime.ApplicationStopping.Register(() =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var consul = new ConsulClient(c => c.Address = new Uri(consulAddr));
                        await consul.Agent.ServiceDeregister(serviceId);
                        Console.WriteLine($"[Consul] Deregistrado: {serviceId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Consul] Falha ao deregistrar: {ex.Message}");
                    }
                });
            });

            return app;
        }

        private static TimeSpan ParseDuration(string input, TimeSpan fallback)
        {
            if (string.IsNullOrWhiteSpace(input)) return fallback;
            if (TimeSpan.TryParse(input, CultureInfo.InvariantCulture, out var ts)) return ts;

            var s = input.Trim().ToLowerInvariant();
            if (s.EndsWith("ms") && double.TryParse(s[..^2], NumberStyles.Float, CultureInfo.InvariantCulture, out var ms))
                return TimeSpan.FromMilliseconds(ms);
            if (s.EndsWith("s") && double.TryParse(s[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var sec))
                return TimeSpan.FromSeconds(sec);
            if (s.EndsWith("m") && double.TryParse(s[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var min))
                return TimeSpan.FromMinutes(min);
            if (s.EndsWith("h") && double.TryParse(s[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var hr))
                return TimeSpan.FromHours(hr);

            return fallback;
        }
    }
}
