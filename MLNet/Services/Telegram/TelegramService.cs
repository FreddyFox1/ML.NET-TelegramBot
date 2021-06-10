using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLNet.Services.Telegram.Abstractions;
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
        //private static List<Command> CommandsList;

        //public static IReadOnlyList<Command> Commands
        //{
        //    get => CommandsList.AsReadOnly();
        //}

        /// <summary>
        /// </summary>
        /// <param name="_logger">Получаем логгер для логгирования</param>
        /// <param name="_telegramKey">Token атвторизации для работы бота Telegram</param>
        public TelegramService(ILogger<TelegramService> _logger,
                               IOptions<TelegramKey> _telegramKey)
        {
            logger = _logger;
            telegramKey = _telegramKey;
            telegramClient = new TelegramBotClient(telegramKey.Value.Key);
        }

        /// <summary>
        /// Подписываемся на событие обновления сообщений, 
        /// и начинаем прослушивать сообщения которые летят к боту
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            SetCommandList();

            logger.LogInformation("Telegram service started");
            telegramClient.OnUpdate += OnUpdateReceived;
            timer = new Timer(a =>
            {
                GetUpdates();
            },
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Telegram service was stoped");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Получение обновлений от Telegram API
        /// </summary>
        public void GetUpdates()
        {
            telegramClient.StartReceiving(Array.Empty<UpdateType>());
        }

        /// <summary>
        /// Отправка уведомлений
        /// </summary>
        public void SendNotification()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Событие получения обновлений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void OnUpdateReceived(object sender, UpdateEventArgs e)
        {
            var message = e.Update.Message;
            if (message == null || message.Type != MessageType.Text) return;
            else
            {
                //foreach (var command in Commands)
                //{
                //    if (command.Contains(message.Text))
                //    {
                //        command.Execute(message, telegramClient);
                //        break;
                //    }
                //}
                logger.LogInformation(message.Text);
            };
        }

        /// <summary>
        /// Создаем и заполняем список команд бота
        /// </summary>
        private void SetCommandList()
        {
            //CommandsList = new List<Command>();
        }

    }

}
