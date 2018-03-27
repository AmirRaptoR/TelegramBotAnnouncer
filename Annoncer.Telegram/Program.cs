using System;
using System.Linq;
using System.Threading.Tasks;
using Annoncer.Telegram.Properties;
using Microsoft.Owin.Hosting;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Annoncer.Telegram
{
    internal class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient(Settings.Default.ApiKey);
        private static IDisposable _webServer;

        private static void Main(string[] args)
        {
            InitializeWeb();
            InitializeBot();

            Console.ReadLine();

            _webServer.Dispose();
            Bot.StopReceiving();
            Console.WriteLine("All system shutdown. Press any key to exit ...");
            Console.ReadKey();
        }

        private static void InitializeWeb()
        {
            _webServer = WebApp.Start<Startup>("http://+:" + Settings.Default.HttpPort);
            Console.WriteLine("Web initialized and ready");
        }

        private static void InitializeBot()
        {
            var me = Bot.GetMeAsync().Result;
            Console.WriteLine($"Telegram bot initialized and ready [bot = {me.Username}]");
            Bot.OnMessage += BotOnMessageReceived;
            Bot.StartReceiving();
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            var txt = message.Text.Split(' ').First();
            var userId = messageEventArgs.Message.Chat.Id.ToString();
            if (Settings.Default.AllowSubs)
            {
                if (txt == "/subscribe")
                {
                    SubscribersList.Add(userId);
                    Bot.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Subscription done");
                }

                if (txt == "/unsubscribe")
                {
                    SubscribersList.Remove(userId);
                    Bot.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Unsubscription done");
                }
            }
        }

        public static async Task Announce(string message)
        {
            foreach (var user in SubscribersList.GetList())
            {
                await Bot.SendTextMessageAsync(user, "Event : \r\n" + message);
            }
            Console.WriteLine($"Message sent to [{SubscribersList.GetList().Count()}] subscribers");
        }
    }
}