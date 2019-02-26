using System;
using PolyPaint.Modeles;

namespace PolyPaint.Services
{
    class ChatService : ConnectionService
    {
        public event Action<ChatMessageTemplate> NewMessage;

        public ChatService()
        {
            
        }
        
        public void Initialize(object o)
        {
            socket.On("messageSent", (data) =>
            {
                ChatMessageTemplate message = serializer.Deserialize<ChatMessageTemplate>((string)data);

                NewMessage?.Invoke(message);
            });
        }
        
        public void SendMessage(string text, string sender, long timestamp)
        {
            ChatMessageTemplate chatMessage = new ChatMessageTemplate() {
                text = text,
                sender = sender,
                createdAt = timestamp
            };

            socket.Emit("sendMessage", serializer.Serialize(chatMessage));
        }
    }
}