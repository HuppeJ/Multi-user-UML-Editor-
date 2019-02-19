/**
 * Inspired from: Meshack Musundi
 * Source: https://www.codeproject.com/Articles/1181555/SignalChat-WPF-SignalR-Chat-Application
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PolyPaint.Services;
using PolyPaint.Enums;
using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace PolyPaint.VueModeles
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ChatService chatService;
        private IDialogService dialogService;
        private TaskFactory ctxTaskFactory;

        private string _userName;
        public string username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Room> _rooms = new ObservableCollection<Room>();
        public ObservableCollection<Room> rooms
        {
            get { return _rooms; }
            set
            {
                _rooms = value;
                OnPropertyChanged();
            }
        }

        private Room _selectedRoom;
        public Room selectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
            }
        }

        private UserModes _userMode;
        public UserModes UserMode
        {
            get { return _userMode; }
            set
            {
                _userMode = value;
                OnPropertyChanged();
            }
        }

        private string _textMessage;
        public string textMessage
        {
            get { return _textMessage; }
            set
            {
                _textMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        #region Connect Command
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new RelayCommand<object>(chatService.Connect, CanConnect));
            }
        }

        private bool CanConnect(object o)
        {
            return !IsConnected;// && UserName.Length >= 2;
        }
        #endregion

        #region CreateUser Command
        private ICommand _createUserCommand;
        public ICommand CreateUserCommand
        {
            get
            {
                return _createUserCommand ?? (_createUserCommand = new RelayCommand<object>(Create, CanCreate));

            }
        }

        private void Create(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            chatService.CreateUser(username, password);
        }

        private bool CanCreate(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && IsConnected;// && UserName.Length >= 2;
        }
        #endregion

        #region LoginUser Command
        private ICommand _loginUserCommand;
        public ICommand LoginUserCommand
        {
            get
            {
                return _loginUserCommand ?? (_loginUserCommand = new RelayCommand<Object>(Login, CanLogin));
            }
        }

        private void Login(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            chatService.LoginUser(username, password);
        }

        private bool CanLogin(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && IsConnected;// && UserName.Length >= 2;
        }
        #endregion

        #region Logout Command
        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new RelayCommand<Object>(Logout, CanLogout));
            }
        }

        private void Logout(object o)
        {
            UserMode = UserModes.Login;
            _isLoggedIn = false;
            textMessage = string.Empty;
            _selectedRoom?.Chatter.Clear();
            chatService.Disconnect();
        }

        private bool CanLogout(object o)
        {
            return IsConnected;//&& IsLoggedIn;
        }
        #endregion

        #region CreateUserViewCommand
        private ICommand _createUserViewCommand;
        public ICommand CreateUserViewCommand
        {
            get
            {
                return _createUserViewCommand ?? (_createUserViewCommand = new RelayCommand<Object>(CreateUserView));
            }
        }

        private void CreateUserView(object o)
        {
            UserMode = UserModes.CreateUser;
        }
        #endregion

        #region BackToLoginCommand
        private ICommand _backToLoginCommand;
        public ICommand BackToLoginCommand
        {
            get
            {
                return _backToLoginCommand ?? (_backToLoginCommand = new RelayCommand<Object>(BackToLogin));
            }
        }

        private void BackToLogin(object o)
        {
            UserMode = UserModes.Login;
        }
        #endregion

        #region Send Message Command
        private ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand<Object>(SendMessage, CanSendMessage));
            }
        }

        private void SendMessage(object o)
        {
            chatService.SendMessage(textMessage, username, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            textMessage = string.Empty;
        }

        private bool CanSendMessage(object o)
        {
            return (!string.IsNullOrEmpty(textMessage) && !string.IsNullOrWhiteSpace(textMessage)
                && _selectedRoom != null && IsConnected);
        }
        #endregion

        #region Event Handlers
        private void NewMessage(ChatMessageTemplate message)
        {
            ChatMessage cm = new ChatMessage {
                sender = message.sender,
                text = message.text,
                createdAt = DateTimeOffset.FromUnixTimeMilliseconds(message.createdAt).DateTime.ToLocalTime(),
                isOriginNative = (message.sender == username)
            };


            //Console.WriteLine("newMessage from modelView");


            if (!_selectedRoom.Chatter.Contains(cm)){
                ctxTaskFactory.StartNew(() => _selectedRoom.Chatter.Add(cm)).Wait();
            }

        }

        private void Connection(bool isConnected)
        {
            IsConnected = isConnected;

            if (!IsConnected)
            {
                dialogService.ShowNotification("Connection to server failed :/");
            }
        }

        private void UserCreation(bool isUserCreated)
        {
            if(isUserCreated)
            {
                UserMode = UserModes.Login;
            }
            else
            {
                dialogService.ShowNotification("The user could not be created");
            }
        }

        private void UserLogin(bool isLoginSuccessful)
        {
            if (isLoginSuccessful)
            {
                //Console.WriteLine("log in from model to view");
                UserMode = UserModes.Chat;
                selectedRoom = rooms.First();
                IsLoggedIn = true;
            }
            else
            {
                dialogService.ShowNotification("Login failed :/");
            }
        }
        #endregion

        public MainWindowViewModel(IDialogService diagSvc)
        {
            dialogService = diagSvc;
            chatService = new ChatService();
            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

            chatService.Connection += Connection;
            chatService.UserCreation += UserCreation;
            chatService.UserLogin += UserLogin;
            chatService.NewMessage += NewMessage;

            rooms.Add(new Room { name = "Everyone" });
        }

    }
}