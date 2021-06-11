using MLNet.Services.Telegram.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MLNet.Services.Telegram.Commands
{
    public class StartCommand : Command
    {
        public override string Name => "/start";

        public async override void Execute(Message message, TelegramBotClient client)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Отправь мне фотографию кота или собаки и я угадаю, кто на ней изображен :)");
        }
    }
}
