using System;
using PolyPaint.Modeles;
using Quobject.SocketIoClientDotNet.Client;
using System.Web.Script.Serialization;

namespace PolyPaint.Services
{
    class ConnectionService
    {
        public static event Action<bool> Connection;
        public static event Action<bool> UserCreation;
        public static event Action<bool> UserLogin;

        public static event Action ConnectionReconnecting;
        public static event Action ConnectionReconnected;
        public static event Action ConnectionClosed;
        
        public static string username = null;

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        // private static string url = "https://projet-3-228722.appspot.com";
        private static string url = "http://localhost:8010";
        // private static string url = "http://10.200.3.1:5020";
        public static Socket socket;

        public static void Connect(object o)
        {
            socket = IO.Socket(url);

            socket.On(Socket.EVENT_CONNECT, () => {
                Connection?.Invoke(true);
            });

            socket.On(Socket.EVENT_RECONNECT, () =>
            {
                Connection?.Invoke(true);
            });

            socket.On("createUserResponse", (data) => {
                bool isUserCreated = serializer.Deserialize<dynamic>((string)data)["isUserCreated"];

                UserCreation?.Invoke(isUserCreated);
            });

            socket.On("loginUserResponse", (data) =>
            {
                bool isLoginSuccessful = serializer.Deserialize<dynamic>((string)data)["isLoginSuccessful"];

                if(!isLoginSuccessful)
                {
                    username = null;
                }

                UserLogin?.Invoke(isLoginSuccessful);
            });

            socket.On("disconnect", (data) =>
            {
                Connection?.Invoke(false);
            });
        }

        public static void Disconnect()
        {
            socket.Emit("logoutUser");
        }

        public static void Close()
        {
            socket.Emit("disconnect");
        }

        public static void CreateUser(string username, string password)
        {
            User user = new User()
            {
                username = username,
                password = password
            };

            string serializedResult = serializer.Serialize(user);
            
            socket.Emit("createUser", serializedResult);
        }

        public static void LoginUser(string username, string password)
        {
            User user = new User()
            {
                username = username,
                password = password
            };

            string serializedResult = serializer.Serialize(user);

            socket.Emit("loginUser", serializedResult);

            ConnectionService.username = username;

        }

        private static void Disconnected()
        {
            ConnectionClosed?.Invoke();
        }

        private static void Reconnected()
        {
            ConnectionReconnected?.Invoke();
        }

        private static void Reconnecting()
        {
            ConnectionReconnecting?.Invoke();
        }
    }
}