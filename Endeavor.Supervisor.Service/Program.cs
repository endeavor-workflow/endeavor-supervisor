using Endeavor.Supervisor.Messaging;
using Endeavor.Supervisor.Persistence;
using Endeavor.Supervisor.Polling;
using Endeavor.Supervisor.Service.Persistence;
using Keryhe.Messaging.RabbitMQ.Extensions;
using Keryhe.Polling;
using Keryhe.Polling.Delay;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Endeavor.Supervisor.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.AddTransient<IDelay, FibonacciDelay>();
                    services.Configure<FibonacciOptions>(hostContext.Configuration.GetSection("FibonacciOptions"));

                    services.AddSingleton<IConnectionFactory, ConnectionFactory>();
                    services.AddTransient<IRepository, SupervisorRepository>();

                    services.AddTransient<IPoller<TaskToBeWorked>, ReadyTaskPoller>();
                    services.AddRabbitMQPublisher<TaskToBeWorked>(hostContext.Configuration.GetSection("RabbitMQPublisher"));

                    services.AddHostedService<Supervisor>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                });
    }
}
