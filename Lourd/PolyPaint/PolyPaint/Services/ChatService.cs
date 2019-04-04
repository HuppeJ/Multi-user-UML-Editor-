﻿using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using PolyPaint.Modeles;
using PolyPaint.Templates;

namespace PolyPaint.Services
{
    class ChatService : ConnectionService
    {
        public event Action<ChatMessageTemplate> NewMessage;
        public event Action<CreateChatroomResponse> RoomCreation;
        public event Action<JoinChatroomResponse> RoomJoin;
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
                RoomList roomlist = serializer.Deserialize<RoomList>((string)data);
                GetChatrooms?.Invoke(roomlist);
            });

            socket.On("createChatroomResponse", (data) =>
            {
                CreateChatroomResponse response = serializer.Deserialize<CreateChatroomResponse>((string)data);
                RoomCreation?.Invoke(response);
            });

            socket.On("joinChatroomResponse", (data) =>
            {
                JoinChatroomResponse response = serializer.Deserialize<JoinChatroomResponse>((string)data);
                RoomJoin?.Invoke(response);
                RequestChatrooms();
            });

            socket.On("leaveChatroomResponse", (data) =>
            {
                RequestChatrooms();
            });

            socket.On("chatroomCreated", (data) =>
            {
                RequestChatrooms();
            });

            // Makes sure the chatroom "Everyone" is on the server
            //CreateChatroom("MainRoom");
        }
        
        public void SendMessage(string message, string username, long timestamp, string chatroomName)
        {
            ChatMessageTemplate chatMessage = new ChatMessageTemplate() {
                message = message,
                username = username,
                createdAt = timestamp,
                chatroomName = chatroomName
            };

            socket.Emit("sendMessage", serializer.Serialize(chatMessage));
        }

        public void RequestChatrooms()
        {
            socket.Emit("getChatrooms");
        }

        public void JoinChatroom(string chatroomName)
        {
            socket.Emit("joinChatroom", serializer.Serialize(new EditChatroomData(username, chatroomName)));
        }

        public void LeaveChatroom(string chatroomName)
        {
            socket.Emit("leaveChatroom", serializer.Serialize(new EditChatroomData(username, chatroomName)));
        }

        public void CreateChatroom(string chatroomName)
        {
            socket.Emit("createChatroom", serializer.Serialize(new Chatroom(chatroomName)));
        }
    }
}