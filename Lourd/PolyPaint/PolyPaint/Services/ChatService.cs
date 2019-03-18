using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using PolyPaint.Modeles;
using PolyPaint.Templates;

namespace PolyPaint.Services
{
    class ChatService : ConnectionService
    {
        public event Action<ChatMessageTemplate> NewMessage;
        public event Action<RoomList> GetChatrooms;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

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

                RoomList rooms = serializer.Deserialize<RoomList>((string)data);
                //GetChatrooms?.Invoke(roomList);
                int i = 0;
            });
            
            socket.Emit("createChatroom", serializer.Serialize(new Chatroom("Everyone")));
        }
        
        public void SendMessage(string message, string sender, long timestamp, string chatroomName)
        {
            ChatMessageTemplate chatMessage = new ChatMessageTemplate() {
                message = message,
                sender = sender,
                createdAt = timestamp,
                chatroomName = chatroomName
            };

            socket.Emit("sendMessage", serializer.Serialize(chatMessage));
        }

        public void RequestChatrooms()
        {
            socket.Emit("getChatrooms");
        }
    }
}