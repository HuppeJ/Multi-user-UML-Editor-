using System;
using PolyPaint.Modeles;
using Quobject.SocketIoClientDotNet.Client;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Threading;

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
        public static bool hasUserDoneTutorial = false;

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        // private static string url = "https://projet-3-228722.appspot.com";
        private static string url = "http://localhost:5020";
        // private static string url = "http://localhost:8010";
        // private static string url = "http://10.200.3.1:5020";
        // private static string url = "http://10.200.31.156:5020";

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
                Application.Current?.Dispatcher?.Invoke(new Action(() => { UserCreation(isUserCreated); }), DispatcherPriority.Render);

            });

            socket.On("loginUserResponse", (data) =>
            {
                bool isLoginSuccessful = serializer.Deserialize<dynamic>((string)data)["isLoginSuccessful"];

                if(!isLoginSuccessful)
                {
                    username = null;
                } else
                {
                    socket.Emit("hasUserDoneTutorial", username);
                }
                
                Application.Current?.Dispatcher?.Invoke(new Action(() => { UserLogin(isLoginSuccessful); }), DispatcherPriority.Render);

            });

            socket.On("hasUserDoneTutorialResponse", (data =>
            {
                hasUserDoneTutorial = serializer.Deserialize<dynamic>((string)data)["hasUserDoneTutorial"];
            }));

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