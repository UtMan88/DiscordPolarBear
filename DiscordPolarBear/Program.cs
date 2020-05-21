using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordPolarBear
{
    class Program
    {
        private readonly IConfiguration config;
        private DiscordSocketClient client;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public Program()
        {
            // create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            // build the configuration and assign to _config          
            config = _builder.Build();
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                client = services.GetRequiredService<DiscordSocketClient>();

                // setup logging and the ready event
                client.Log += Log;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += Log;

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot, config["Token"]);
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
