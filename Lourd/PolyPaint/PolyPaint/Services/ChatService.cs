using System;
using PolyPaint.Modeles;
using Quobject.SocketIoClientDotNet.Client;
using System.Web.Script.Serialization;

namespace PolyPaint.Services
{
    class ChatService
    {
        public event Action<bool> Connection;
        public event Action<bool> UserCreation;
        public event Action<bool> UserLogin;
        public event Action<ChatMessageTemplate> NewMessage;

        public event Action ConnectionReconnecting;
        public event Action ConnectionReconnected;
        public event Action ConnectionClosed;

        private JavaScriptSerializer serializer;
        private static string url = "https://projet-3-228722.appspot.com";
        //private static string url = "http://localhost:8080";
        private static Socket socket = IO.Socket(url);

        public ChatService()
        {
            serializer = new JavaScriptSerializer();
        }

        public void Connect(object o)
        {
            // socket = IO.Socket(url, new IO.Options() { ForceNew = true });
            //Console.WriteLine("connect");


            socket.On(Socket.EVENT_CONNECT, () => {
                //Console.WriteLine("on connect");
                Connection?.Invoke(true);
            });

            socket.On(Socket.EVENT_RECONNECT, () =>
            {
                Connection?.Invoke(true);
                socket.Emit("joinChatroom");
            });

            socket.On("createUserResponse", (data) => {
                //Console.WriteLine("on create");
                
                bool isUserCreated = serializer.Deserialize<dynamic>((string) data)["isUserCreated"];

                UserCreation?.Invoke(isUserCreated);
            });

            socket.On("loginUserResponse", (data) =>
            {
                //Console.WriteLine("on login");
                bool isLoginSuccessful = serializer.Deserialize<dynamic>((string) data)["isLoginSuccessful"];

                // TODO : ou le mettre? Le client ne devrait pas la joindre par defaut quand il login, sur le serveur?
                socket.Emit("joinChatroom");

                UserLogin?.Invoke(isLoginSuccessful);
            });

            socket.On("messageSent", (data) =>
            {

                ChatMessageTemplate message = serializer.Deserialize<ChatMessageTemplate>((string) data);
                //Console.WriteLine("on receive: " + message.text);

                NewMessage?.Invoke(message);
            });

            socket.On("disconnect", (data) =>
            {
                Connection?.Invoke(false);
            });

            // Connect to server
            // socket.Connect();
        }

        public void Disconnect()
        {
            // disconnect from the server
            socket.Emit("logoutUser");
        }

        public void CreateUser(string username, string password)
        {
            User user = new User()
            {
                username = username,
                password = password
            };

            string serializedResult = serializer.Serialize(user);

            //Console.WriteLine(serializedResult);

            socket.Emit("createUser", serializedResult);
        }

        public void LoginUser(string username, string password)
        {
            User user = new User()
            {
                username = username,
                password = password
            };

            string serializedResult = serializer.Serialize(user);

            socket.Emit("loginUser", serializedResult);

        }

        public void SendMessage(string text, string sender, long timestamp)
        {
            ChatMessageTemplate chatMessage = new ChatMessageTemplate() {
                text = text,
                sender = sender,
                createdAt = timestamp
            };

            //Console.WriteLine("on send" + chatMessage.text);


            socket.Emit("sendMessage", serializer.Serialize(chatMessage));

        }


        private void Disconnected()
        {
            ConnectionClosed?.Invoke();
        }

        private void Reconnected()
        {
            ConnectionReconnected?.Invoke();
        }

        private void Reconnecting()
        {
            ConnectionReconnecting?.Invoke();
        }
    }
}