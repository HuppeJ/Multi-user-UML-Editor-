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
using PolyPaint.Templates;
using PolyPaint.Utilitaires;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

namespace PolyPaint.VueModeles
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ConnectionService connectionService;
        private ChatService chatService;
        private IDialogService dialogService;
        private TaskFactory ctxTaskFactory;

        #region Properties
        private string _userName = "a";
        public string username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private AsyncObservableCollection<Room> _rooms = new AsyncObservableCollection<Room>();
        public AsyncObservableCollection<Room> rooms
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

        private string _canvasName = "";
        public string CanvasName
        {
            get { return _canvasName; }
            set
            {
                if (_canvasName == value) return;

                _canvasName = value;
                OnPropertyChanged();
            }
        }

        private string _canvasPrivacy = "Public";
        public string CanvasPrivacy
        {
            get { return _canvasPrivacy; }
            set
            {
                if (_canvasPrivacy == value) return;

                _canvasPrivacy = value;
                OnPropertyChanged();
            }
        }

        private string _canvasProtection = "Unprotected";
        public string CanvasProtection
        {
            get { return _canvasProtection; }
            set
            {
                if (_canvasProtection == value) return;

                _canvasProtection = value;
                OnPropertyChanged();
            }
        }

        private List<Templates.Canvas> _publicCanvase = new List<Templates.Canvas>();
        public List<Templates.Canvas> PublicCanvases
        {
            get { return _publicCanvase; }
            set
            {
                if (_publicCanvase == value) return;

                _publicCanvase = value;
                OnPropertyChanged();
            }
        }

        private List<Templates.Canvas> _privateCanvase = new List<Templates.Canvas>();
        public List<Templates.Canvas> PrivateCanvases
        {
            get { return _privateCanvase; }
            set
            {
                if (_privateCanvase == value) return;

                _privateCanvase = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Connect Command
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new RelayCommand<object>(ConnectionService.Connect, CanConnect));
            }
        }

        private bool CanConnect(object o)
        {
            return !IsConnected;
        }
        #endregion

        #region Initialize Chat Command
        private ICommand _initializeChatCommand;
        public ICommand InitializeChatCommand
        {
            get
            {
                return _initializeChatCommand ?? (_initializeChatCommand = new RelayCommand<object>(chatService.Initialize));
            }
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
            ChatService.CreateUser(username, password);
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
            ChatService.LoginUser(username, password);
            chatService.RequestChatrooms();
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
            ChatService.Disconnect();
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
                return _createUserViewCommand ?? (_createUserViewCommand = new RelayCommand<Object>(GoToCreateUserView));
            }
        }

        private void GoToCreateUserView(object o)
        {
            UserMode = UserModes.CreateUser;
        }
        #endregion

        #region DrawingViewCommand
        private ICommand _drawingViewCommand;
        public ICommand DrawingViewCommand
        {
            get
            {
                return _drawingViewCommand ?? (_drawingViewCommand = new RelayCommand<Object>(GoToDrawingView));
            }
        }

        private void GoToDrawingView(object o)
        {
            UserMode = UserModes.Drawing;
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

        #region BackToGalleryCommand
        private ICommand _backToGalleryCommand;
        public ICommand BackToGalleryCommand
        {
            get
            {
                return _backToGalleryCommand ?? (_backToGalleryCommand = new RelayCommand<Object>(BackToGallery));
            }
        }

        private void BackToGallery(object o)
        {
            UserMode = UserModes.Gallery;
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
            chatService.SendMessage(textMessage, username, DateTimeOffset.Now.ToUnixTimeMilliseconds(), selectedRoom.name);
            textMessage = string.Empty;
        }

        private bool CanSendMessage(object o)
        {
            return (!string.IsNullOrEmpty(textMessage) && !string.IsNullOrWhiteSpace(textMessage)
                && _selectedRoom != null && IsConnected);
        }
        #endregion

        #region CancelCanvasCreation Command
        private ICommand _cancelCanvasCreationCommand;
        public ICommand CancelCanvasCreationCommand
        {
            get
            {
                return _cancelCanvasCreationCommand ?? (_cancelCanvasCreationCommand = new RelayCommand<Object>(CancelCanvasCreation));
            }
        }

        private void CancelCanvasCreation(object o)
        {
            CanvasName = "";
            CanvasPrivacy = "Public";
            CanvasProtection = "Unprotected";
        }
        #endregion

        #region CreateCanvas Command
        private ICommand _createCanvasCommand;
        public ICommand CreateCanvasCommand
        {
            get
            {
                return _createCanvasCommand ?? (_createCanvasCommand = new RelayCommand<Object>(CreateCanvas, CanCreateCanvas));
            }
        }

        private void CreateCanvas(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            int accessibility = CanvasPrivacy == "Public" ? 1 : 0;
            int[] dimensions = { 1, 1 };

            Templates.Canvas canvas = new Templates.Canvas(Guid.NewGuid().ToString(), CanvasName, username, username,
                                                            accessibility, password, new List<BasicShape>(), new List<Link>(), dimensions);

            DrawingService.CreateCanvas(canvas);
        }

        private bool CanCreateCanvas(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            bool unprotectedOrPassword = (CanvasProtection == "Unprotected") || !string.IsNullOrEmpty(password);

            return !string.IsNullOrEmpty(CanvasName) && unprotectedOrPassword;
        }
        #endregion

        #region RefreshCanvases Command
        private ICommand _refreshCanvasesCommand;
        public ICommand RefreshCanvasesCommand
        {
            get
            {
                return _refreshCanvasesCommand ?? (_refreshCanvasesCommand = new RelayCommand<Object>(RefreshCanvases));
            }
        }

        private void RefreshCanvases(object o)
        {
            DrawingService.RefreshCanvases();
        }
        #endregion

        #region ResetSErver Command
        private ICommand _resetServerCommand;
        public ICommand ResetServerCommand
        {
            get
            {
                return _resetServerCommand ?? (_resetServerCommand = new RelayCommand<Object>(ResetServer));
            }
        }

        private void ResetServer(object o)
        {
            DrawingService.ResetServer();
        }
        #endregion

        #region Event Handlers
        private void NewMessage(ChatMessageTemplate message)
        {
            ChatMessage cm = new ChatMessage {
                sender = message.username,
                text = message.message,
                createdAt = DateTimeOffset.FromUnixTimeMilliseconds(message.createdAt).DateTime.ToLocalTime(),
                isOriginNative = (message.username == username)
            };

            if (!_selectedRoom.Chatter.Contains(cm)){
                ctxTaskFactory.StartNew(() => _selectedRoom.Chatter.Add(cm)).Wait();
            }

        }

        private void GetChatrooms(RoomList chatrooms)
        {
            foreach(string room in chatrooms.chatrooms)
            {
                Room newRoom = new Room { name = room };
                if(!rooms.Contains(newRoom))
                {
                    rooms.Add(newRoom);
                }
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
                UserMode = UserModes.Gallery;
                // selectedRoom = rooms.First();
                IsLoggedIn = true;
            }
            else
            {
                dialogService.ShowNotification("Login failed :/");
            }
        }

        private void UpdatePublicCanvases(PublicCanvases canvas)
        {
            PublicCanvases = canvas.publicCanvas;
        }

        private void UpdatePrivateCanvases(PrivateCanvases canvas)
        {
            PrivateCanvases = canvas.privateCanvas;
        }
        #endregion

        public MainWindowViewModel(IDialogService diagSvc)
        {
            dialogService = diagSvc;
            connectionService = new ConnectionService();
            chatService = new ChatService();
            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

            ConnectionService.Connection += Connection;
            ConnectionService.UserCreation += UserCreation;
            ConnectionService.UserLogin += UserLogin;

            DrawingService.UpdatePublicCanvases += UpdatePublicCanvases;
            DrawingService.UpdatePrivateCanvases += UpdatePrivateCanvases;

            chatService.NewMessage += NewMessage;
            chatService.GetChatrooms += GetChatrooms;

            //rooms.Add(new Room { name = "Everyone" });

        }
    }
}