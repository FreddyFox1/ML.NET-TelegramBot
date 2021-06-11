using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLNet.Services.Telegram.Abstractions;
using MLNet.Services.Telegram.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace MLNet.Services.Telegram
{
    internal class TelegramService : IHostedService, ITelegram
    {
        private static ILogger<TelegramService> logger;
        private Timer timer;
        private static TelegramBotClient telegramClient;
        private readonly IOptions<TelegramKey> telegramKey;
        private static List<Command> Commands;

        public TelegramService(ILogger<TelegramService> _logger,
                               IOptions<TelegramKey> _telegramKey)
        {
            logger = _logger;
            telegramKey = _telegramKey;
            telegramClient = new TelegramBotClient(telegramKey.Value.Key);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            CreateCommandList();
            logger.LogInformation("Telegram: Command list was created");

            telegramClient.OnUpdate += OnUpdateReceived;

            timer = new Timer(a =>
            {
                GetUpdates();
            },
            null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Telegram service was stoped");
            return Task.CompletedTask;
        }

        public void GetUpdates()
        {
            logger.LogInformation("Telegram receiving updates now");
            telegramClient.StartReceiving(Array.Empty<UpdateType>());
        }

        private static async void OnUpdateReceived(object sender, UpdateEventArgs e)
        {
            var message = e.Update.Message;

            if (message.Photo != null)
            {
                message.Text = "Image";
            }

            if (message.Text != null)
            {
                foreach (var command in Commands)
                {
                    if (command.Contains(message.Text))
                    {
                        command.Execute(message, telegramClient);
                        break;
                    }
                }
            }
            else return;
        }

        private static void CreateCommandList()
        {
            logger.LogInformation("Telegram: Creating command list");
            Commands = new List<Command>();
            Commands.Add(new StartCommand());
            Commands.Add(new ImageCommand());
        }

    }

}
