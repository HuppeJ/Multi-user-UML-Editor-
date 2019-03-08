using System;
using System.Collections.Generic;
using PolyPaint.Modeles;
using PolyPaint.Templates;

namespace PolyPaint.Services
{
    class ChatService : ConnectionService
    {
        public event Action<ChatMessageTemplate> NewMessage;
        public event Action<RoomList> GetChatrooms;

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

            socket.On("getChatroomsResponse", (data) =>
            {
                string[] rooms = serializer.Deserialize<string[]>((string)data);
                RoomList roomList = new RoomList();
                roomList.rooms = rooms;
                GetChatrooms?.Invoke(roomList);
            });

            socket.Emit("createChatroom", "Room1");
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

        public void RequestChatrooms()
        {
            socket.Emit("getChatrooms");
        }
    }
}