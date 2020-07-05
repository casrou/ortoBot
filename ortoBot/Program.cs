using Microsoft.Extensions.Configuration;
using Nanoleaf.Client;
using Nanoleaf.Client.Interfaces;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ortoBot
{
    class Program
    {
        private static string version = "1.3.6";
        private static OrtoBot ortoBot;
        private static Settings settings;

        static async Task Main(string[] args)
        {
            SetupConfiguration();
            SetupLogging();            

            Setup();

            await ortoBot.Start();

            while (true)
            {
                await ortoBot.ProcessQueueAsync();
                await Task.Delay(1000);
            }
        }

        private static void Setup()
        {
            PrintWelcome();

            if (settings.debug)
            {
                Log.Information("DEBUG: " + settings.debug.ToString().ToUpper());
            }

            ortoBot = new OrtoBot(settings);

            ConnectNanoleaf();

            //await ConnectDiscord();

            ConnectTwitch();            

            Log.Information("");
        }
        private static void ConnectNanoleaf()
        {
            Log.Information("Connecting to Nanoleaf... " +
                $"({settings.ip}, {settings.authToken})");
            INanoleafClient nanoleaf;
            if (settings.ip == "" || settings.authToken == "")
            {
                nanoleaf = new FakeNanoleafClient(settings);
            }
            else
            {
                nanoleaf = new SimpleNanoleafClient(settings.ip, settings.authToken);
            }            
            Log.Information($"Nanoleaf connected!");
            ortoBot.SetNanoleaf(nanoleaf);
        }

        //private static async Task ConnectDiscord()
        //{
        //    Log.Information("Setting up Discord client..." +
        //        $"({settings.discord_token})");
        //    SimpleDiscordSocketClient discordClient = new SimpleDiscordSocketClient(settings);
        //    await discordClient.Initialize(settings.discord_token);
        //    ortoBot.SetDiscordClient(discordClient);
        //}

        private static void ConnectTwitch()
        {
            Log.Information("Setting up Twitch client..." +
                $"({settings.botUsername}, {settings.botOauth}, {settings.botJoinChannel})");
            SimpleTwitchClient twitchClient;
            if (settings.botJoinChannel == "")
            {
                twitchClient = new FakeTwitchClient(settings, ortoBot);
            }
            else
            {
                twitchClient = new SimpleTwitchClient(settings, ortoBot);
            }
            ortoBot.SetTwitchClient(twitchClient);

            if (settings.channelId != "" && settings.oauth != "")
            {
                Log.Information("Connecting to Twitch PubSub... " +
                $"({settings.channelId}, {settings.oauth})");
                SimpleTwitchPubSub twitchPubSub = new SimpleTwitchPubSub(settings, ortoBot);
                ortoBot.SetTwitchPubSub(twitchPubSub);
            }            
        }

        private static void PrintWelcome()
        {
            Log.Information("\u250C\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2510");
            Log.Information($"    ortoBot {version}     ");
            Log.Information("\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518");
        }

        private static void SetupLogging()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(SimpleExceptionHandler);

            var logConfig = new LoggerConfiguration()                
                .WriteTo.Console(outputTemplate:
                    "{Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs\\ortoLog.txt", rollingInterval: RollingInterval.Day);

            if (settings.debug) 
            {
                logConfig.MinimumLevel.Debug();
            } 
            else
            {
                logConfig.MinimumLevel.Information();
            }

            Log.Logger = logConfig.CreateLogger();
        }

        static void SimpleExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine(e);
            //ortoBot.HandleException(e);
            Log.Fatal(e.ToString());
        }

        private static void SetupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();
            settings = new Settings();
            configuration.Bind(settings);
        }
    }
}
