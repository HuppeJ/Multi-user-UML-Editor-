using System;
using PolyPaint.Modeles;
using Quobject.SocketIoClientDotNet.Client;
using System.Web.Script.Serialization;

namespace PolyPaint.Services
{
    class ConnectionService
    {
        public event Action<bool> Connection;
        public event Action<bool> UserCreation;
        public event Action<bool> UserLogin;

        public event Action ConnectionReconnecting;
        public event Action ConnectionReconnected;
        public event Action ConnectionClosed;

        protected JavaScriptSerializer serializer;
        //private string url = "https://projet-3-228722.appspot.com";
        private static string url = "http://localhost:5020";
        protected static Socket socket;

        public ConnectionService()
        {
            serializer = new JavaScriptSerializer();
        }

        public void Connect(object o)
        {
            socket = IO.Socket(url);

            socket.On(Socket.EVENT_CONNECT, () => {
                Connection?.Invoke(true);
            });

            socket.On(Socket.EVENT_RECONNECT, () =>
            {
                Connection?.Invoke(true);
                socket.Emit("joinChatroom");
            });

            socket.On("createUserResponse", (data) => {
                bool isUserCreated = serializer.Deserialize<dynamic>((string)data)["isUserCreated"];

                UserCreation?.Invoke(isUserCreated);
            });

            socket.On("loginUserResponse", (data) =>
            {
                bool isLoginSuccessful = serializer.Deserialize<dynamic>((string)data)["isLoginSuccessful"];

                // TODO : ou le mettre? Le client ne devrait pas la joindre par defaut quand il login, sur le serveur?
                socket.Emit("joinChatroom");

                UserLogin?.Invoke(isLoginSuccessful);
            });

            socket.On("disconnect", (data) =>
            {
                Connection?.Invoke(false);
            });
        }

        public void Disconnect()
        {
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