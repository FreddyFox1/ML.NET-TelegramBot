using Microsoft.AspNetCore.Hosting;
using MLNet.MagicML;
using MLNet.Services.Telegram.Abstractions;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MLNet.Services.Telegram.Commands
{
    public class ImageCommand : Command
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _Model;
        public override string Name => "Image";


        public ImageCommand(IWebHostEnvironment environment)
        {
            _environment = environment;
            _Model = Path.Combine(_environment.WebRootPath, "models", "MLModel.zip";
        }

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var Image = await SaveImage(message, client);
            var MessageId = message.MessageId;
            var ChatId = message.Chat.Id;

            if (Image != null)
            {
                ModelInput MLData = new ModelInput()
                {
                    ImageSource = Image
                };

                ConsumeModel consumer = new ConsumeModel(_Model);

                var predictionResult = consumer.Predict(MLData);


                var answer = predictionResult.Score[1] > predictionResult.Score[0] && predictionResult.Score[1] > 0.6
                    ? $"С вероятностью { Math.Round(predictionResult.Score[1], 2) * 100 } % \n Это собака :)"
                    : predictionResult.Score[0] > predictionResult.Score[1] && predictionResult.Score[0] > 0.6
                    ? $"С вероятностью { Math.Round(predictionResult.Score[0], 2) * 100 } % \n Это кот :)"
                    : "Я не понимаю кто это :(";

                await client.SendTextMessageAsync(ChatId, answer, replyToMessageId: MessageId);
            }

            DeleteImageFromDisk(Image);
        }

        private async Task<string> SaveImage(Message message, TelegramBotClient client)
        {
            string distinationPath = Path.Combine(_environment.WebRootPath, "images", GetUniqueFileName());

            if (message.Type == MessageType.Photo)
            {
                var file = await client.GetFileAsync(message.Photo[1].FileId);
                using (FileStream fs = new FileStream(distinationPath, FileMode.Create))
                {
                    await client.DownloadFileAsync(file.FilePath, fs);
                }
            }
            else if (message.Type == MessageType.Document)
            {
                var file = await client.GetFileAsync(message.Document.FileId);
                using (FileStream fs = new FileStream(distinationPath, FileMode.Create))
                {
                    await client.DownloadFileAsync(file.FilePath, fs);
                }
            }

            return distinationPath;
        }

        private string GetUniqueFileName()
        {
            return Guid.NewGuid().ToString() + ".png";
        }

        private void DeleteImageFromDisk(string path)
        {
            System.IO.File.Delete(path);
        }
    }
}
