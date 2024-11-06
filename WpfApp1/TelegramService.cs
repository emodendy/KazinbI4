using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WpfApp1.DB;

namespace WpfApp1.Services
{
    public class TelegramService
    {
        private readonly TelegramBotClient _botClient;

        public TelegramService(string botToken)
        {
            _botClient = new TelegramBotClient(botToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { } 
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Бот {me.Username} запущен");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            if (message == null || message.Type != MessageType.Text || !message.Text.StartsWith("/"))
                return;

            var chatId = message.Chat.Id;
            var messageText = message.Text;
            string username = update.Message.Chat.Username;

            if (messageText == "/start")
            {
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Ваш Chat ID: {chatId}",
                cancellationToken: cancellationToken
                );
                var user = ConnectionClass.connect.User.FirstOrDefault(log => log.Telegram == username);
                if (user != null)
                {
                    user.TelegramChatID = Convert.ToInt32(chatId);
                    ConnectionClass.connect.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"Пользователь с именем {username} не найден в базе данных.");
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Вашего пользователя не найдено в базе данных.",
                        cancellationToken: cancellationToken
                    );
                }
                return;
            }
            Console.WriteLine($"Получено сообщение '{messageText}' в чате {chatId}.");

            
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Произошла ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

        public void Stop()
        {

        }

        
    public async Task SendTextMessageAsync(long chatId, string message)
    {
        await _botClient.SendTextMessageAsync(chatId, message);
    }
    }
}
