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
using System.Text;

namespace PolyPaint.VueModeles
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ConnectionService connectionService;
        private ChatService chatService;
        private IDialogService dialogService;
        private TaskFactory ctxTaskFactory;
        public event Action CloseChatWindow;
        public bool IsChatWindowOpened = false;
        public bool IsChatWindowClosing = false;
        public bool AccessingCanvas = false;

        const string EMPTY_CANVAS = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAAyADIDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD3+iiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD//2Q==";

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

        private AsyncObservableCollection<Room> _joinableRooms = new AsyncObservableCollection<Room>();
        public AsyncObservableCollection<Room> joinableRooms
        {
            get { return _joinableRooms; }
            set
            {
                _joinableRooms = value;
                OnPropertyChanged();
            }
        }

        private Room _selectedRoom;
        public Room selectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                if(value != null)
                {
                    _selectedRoom = value;
                    OnPropertyChanged();
                }
            }
        }

        private Room _selectedJoinRoom;
        public Room selectedJoinRoom
        {
            get { return _selectedJoinRoom; }
            set
            {
                _selectedJoinRoom = value;
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

        private List<Templates.Canvas> _publicCanvases = new List<Templates.Canvas>();
        public List<Templates.Canvas> PublicCanvases
        {
            get { return _publicCanvases; }
            set
            {
                if (_publicCanvases == value) return;

                _publicCanvases = value;
                OnPropertyChanged();
            }
        }

        private List<Templates.Canvas> _privateCanvases = new List<Templates.Canvas>();
        public List<Templates.Canvas> PrivateCanvases
        {
            get { return _privateCanvases; }
            set
            {
                if (_privateCanvases == value) return;

                _privateCanvases = value;
                OnPropertyChanged();
            }
        }

        private Templates.Canvas _selectedCanvas = new Templates.Canvas();
        public Templates.Canvas SelectedCanvas
        {
            get { return _selectedCanvas; }
            set
            {
                if (_selectedCanvas == value) return;

                _selectedCanvas = value;
                OnPropertyChanged();
            }
        }

        private string _classCode = "";
        public string ClassCode
        {
            get { return _classCode; }
            set
            {
                if (_classCode == value) return;

                _classCode = value;
                OnPropertyChanged();
            }
        }

        private int _tutorialPage = 1;
        public int TutorialPage
        {
            get { return _tutorialPage; }
            set
            {
                _tutorialPage = value;
                UpdateTutorial();
                OnPropertyChanged();
            }
        }

        private string _tutorialText = TutorialTexts.INTRO;
        public string TutorialText
        {
            get { return _tutorialText; }
            set
            {
                _tutorialText = value;
                OnPropertyChanged();
            }
        }

        private string _tutorialTextTitle = TutorialTexts.INTRO_TITLE;
        public string TutorialTextTitle
        {
            get { return _tutorialTextTitle; }
            set
            {
                _tutorialTextTitle = value;
                OnPropertyChanged();
            }
        }

        private string _tutorialImageSource = TutorialTexts.INTRO_IMAGE;
        public string TutorialImageSource
        {
            get { return _tutorialImageSource; }
            set
            {
                _tutorialImageSource = value;
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
            // Joins this room by default
            chatService.JoinChatroom("MainRoom");
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

        #region ClosingCommand
        private ICommand _closingCommand;
        public ICommand ClosingCommand
        {
            get
            {
                return _closingCommand ?? (_closingCommand = new RelayCommand<Object>(Closing));
            }
        }

        private void Closing(object o)
        {
            CloseChatWindow?.Invoke();
        }
        #endregion

        #region CreateUserViewCommand
        private ICommand _createUserViewCommand;
        public ICommand CreateUserViewCommand
        {
            get
            {
                return _createUserViewCommand ?? (_createUserViewCommand = new RelayCommand<Object>(GoToCreateUserView, CanGoCreateUser));
            }
        }

        private void GoToCreateUserView(object o)
        {
            UserMode = UserModes.CreateUser;
        }

        private bool CanGoCreateUser(object o)
        {
            return IsConnected;
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

        #region OfflineModeCommand
        private ICommand _offlineModeCommand;
        public ICommand OfflineModeCommand
        {
            get
            {
                return _offlineModeCommand ?? (_offlineModeCommand = new RelayCommand<Object>(OfflineMode));
            }
        }

        private void OfflineMode(object o)
        {
            UserMode = UserModes.Offline;
        }
        #endregion

        #region DrawingHistoryViewCommand
        private ICommand _drawingHistoryViewCommand;
        public ICommand DrawingHistoryViewCommand
        {
            get
            {
                return _drawingHistoryViewCommand ?? (_drawingHistoryViewCommand = new RelayCommand<Object>(GoToDrawingHistoryView));
            }
        }

        private void GoToDrawingHistoryView(object o)
        {
            UserMode = UserModes.History;
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
            IsChatWindowClosing = true;
            CloseChatWindow?.Invoke();
            DrawingService.LeaveCanvas();
            UserMode = UserModes.Gallery;
        }
        #endregion

        #region GoToGalleryCommand
        private ICommand _goToGalleryCommand;
        public ICommand GoToGalleryCommand
        {
            get
            {
                return _goToGalleryCommand ?? (_goToGalleryCommand = new RelayCommand<Object>(GoToGallery));
            }
        }

        private void GoToGallery(object o)
        {
            UserMode = UserModes.Gallery;
        }
        #endregion

        #region AccessTutorialCommand
        private ICommand _accessTutorialCommand;
        public ICommand AccessTutorialCommand
        {
            get
            {
                return _accessTutorialCommand ?? (_accessTutorialCommand = new RelayCommand<Object>(AccessTutorial));
            }
        }

        private void AccessTutorial(object o)
        {
            TutorialPage = 1;
            UserMode = UserModes.Tutorial;
        }
        #endregion

        #region LeaveTutorialCommand
        private ICommand _leaveTutorialCommand;
        public ICommand LeaveTutorialCommand
        {
            get
            {
                return _leaveTutorialCommand ?? (_leaveTutorialCommand = new RelayCommand<Object>(LeaveTutorial));
            }
        }

        private void LeaveTutorial(object o)
        {
            TutorialPage = 1;
            if (!ConnectionService.hasUserDoneTutorial)
            {
                ConnectionService.hasUserDoneTutorial = true;
                ConnectionService.socket.Emit("userHasDoneTutorial", ConnectionService.username);
            }
            UserMode = UserModes.Drawing;
            DrawingService.DrawCanvas(SelectedCanvas);
        }
        #endregion

        #region NextTutorialPageCommand
        private ICommand _nextTutorialPageCommand;
        public ICommand NextTutorialPageCommand
        {
            get
            {
                return _nextTutorialPageCommand ?? (_nextTutorialPageCommand = new RelayCommand<Object>(NextTutorialPage, CanNextTutorialPage));
            }
        }

        private void NextTutorialPage(object o)
        {
            TutorialPage++;
        }

        private bool CanNextTutorialPage(object o)
        {
            // False if it's the last page ?
            return true;
        }
        #endregion

        #region PreviousTutorialPageCommand
        private ICommand _previousTutorialPageCommand;
        public ICommand PreviousTutorialPageCommand
        {
            get
            {
                return _previousTutorialPageCommand ?? (_previousTutorialPageCommand = new RelayCommand<Object>(PreviousTutorialPage, CanPreviousTutorialPage));
            }
        }

        private void PreviousTutorialPage(object o)
        {
            TutorialPage--;
        }

        private bool CanPreviousTutorialPage(object o)
        {
            return TutorialPage > 1;
        }
        #endregion

        #region ChatWindowOpenedCommand
        private ICommand _chatWindowOpenedCommand;
        public ICommand ChatWindowOpenedCommand
        {
            get
            {
                return _chatWindowOpenedCommand ?? (_chatWindowOpenedCommand = new RelayCommand<Object>(ChatWindowOpened));
            }
        }

        private void ChatWindowOpened(object o)
        {
            IsChatWindowOpened = true;
        }
        #endregion

        #region JoinChatroomCommand
        private ICommand _joinChatroomCommand;
        public ICommand JoinChatroomCommand
        {
            get
            {
                return _joinChatroomCommand ?? (_joinChatroomCommand = new RelayCommand<Object>(JoinChatroom, CanJoinRoom));
            }
        }

        private void JoinChatroom(object o)
        {
            Room room = o as Room;
            chatService.JoinChatroom(room.name);
        }

        private bool CanJoinRoom(object o)
        {
            return o as Room != null;
        }
        #endregion

        #region CreateChatroomCommand
        private ICommand _createChatroomCommand;
        public ICommand CreateChatroomCommand
        {
            get
            {
                return _createChatroomCommand ?? (_createChatroomCommand = new RelayCommand<Object>(CreateChatroom, CanCreateRoom));
            }
        }

        private void CreateChatroom(object o)
        {
            string roomName = o as string;
            chatService.CreateChatroom(roomName);
        }

        private bool CanCreateRoom(object o)
        {
            return (o as string != null && o as string != "");
        }
        #endregion

        #region LeaveChatroomCommand
        private ICommand _leaveChatroomCommand;
        public ICommand LeaveChatroomCommand
        {
            get
            {
                return _leaveChatroomCommand ?? (_leaveChatroomCommand = new RelayCommand<Object>(LeaveChatroom));
            }
        }

        private void LeaveChatroom(object o)
        {
            if(o != null)
            {
                Room room = o as Room;
                chatService.LeaveChatroom(room.name);
                rooms.Remove(room);
                if(rooms.Count > 0)
                {
                    selectedRoom = rooms.First();
                }
            }
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
            var passwordBox = o as PasswordBox;
            passwordBox.Password = "";
            CanvasName = "";
            CanvasPrivacy = "Public";
            CanvasProtection = "Unprotected";
        }
        #endregion

        #region CancelCanvasProtectionChange Command
        private ICommand _cancelCanvasProtectionChangeCommand;
        public ICommand CancelCanvasProtectionChangeCommand
        {
            get
            {
                return _cancelCanvasProtectionChangeCommand ?? (_cancelCanvasProtectionChangeCommand = new RelayCommand<Object>(CancelCanvasProtectionChange));
            }
        }

        private void CancelCanvasProtectionChange(object o)
        {
            var passwordBox = o as PasswordBox;
            passwordBox.Password = "";
            CanvasProtection = "Unprotected";
        }
        #endregion

        #region CancelCanvasJoin Command
        private ICommand _cancelCanvasJoinCommand;
        public ICommand CancelCanvasJoinCommand
        {
            get
            {
                return _cancelCanvasJoinCommand ?? (_cancelCanvasJoinCommand = new RelayCommand<Object>(CancelCanvasJoin));
            }
        }

        private void CancelCanvasJoin(object o)
        {
            var passwordBox = o as PasswordBox;
            passwordBox.Password = "";
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
            if (CanvasProtection == "Unprotected") passwordBox.Password = "";
            var password = passwordBox.Password;

            int accessibility = CanvasPrivacy == "Public" ? 1 : 0;
            // Max: W:1520, H1200
            Coordinates dimensions = new Coordinates(500, 380);

            Templates.Canvas canvas = new Templates.Canvas(Guid.NewGuid().ToString(), CanvasName, username, username,
                                                            accessibility, password, new List<BasicShape>(), new List<Link>(), dimensions, EMPTY_CANVAS);

            DrawingService.CreateCanvas(canvas);

            passwordBox.Password = "";
            CanvasPrivacy = "Public";
            CanvasProtection = "Unprotected";
            CanvasName = "";
        }

        private bool CanCreateCanvas(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;
            bool unprotectedOrPassword = (CanvasProtection == "Unprotected") || !string.IsNullOrEmpty(password);

            return !string.IsNullOrEmpty(CanvasName) && unprotectedOrPassword;
        }
        #endregion

        #region ChangeCanvasProtection Command
        private ICommand _changeCanvasProtection;
        public ICommand ChangeCanvasProtectionCommand
        {
            get
            {
                return _changeCanvasProtection ?? (_changeCanvasProtection = new RelayCommand<Object>(ChangeCanvasProtection));
            }
        }

        private void ChangeCanvasProtection(object o)
        {
            var passwordBox = o as PasswordBox;
            var password = passwordBox.Password;

            DrawingService.ChangeCanvasProtection(SelectedCanvas.name, password);

            passwordBox.Password = "";
            CanvasProtection = "Unprotected";
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

        #region JoinUnprotectedCanvas Command
        private ICommand _joinUnprotectedCanvasCommand;
        public ICommand JoinUnprotectedCanvasCommand
        {
            get
            {
                return _joinUnprotectedCanvasCommand ?? (_joinUnprotectedCanvasCommand = new RelayCommand<Object>(JoinUnprotectedCanvas));
            }
        }

        private void JoinUnprotectedCanvas(object o)
        {
            var canvas = o as Templates.Canvas;
            SelectedCanvas = canvas;
            AccessingCanvas = true;
            DrawingService.JoinCanvas(canvas.name, "");
        }
        #endregion

        #region JoinProtectedCanvas Command
        private ICommand _joinProtectedCanvasCommand;
        public ICommand JoinProtectedCanvasCommand
        {
            get
            {
                return _joinProtectedCanvasCommand ?? (_joinProtectedCanvasCommand = new RelayCommand<Object>(JoinProtectedCanvas));
            }
        }

        private void JoinProtectedCanvas(object o)
        {
            var passwordBox = o as PasswordBox;
            AccessingCanvas = true;
            DrawingService.JoinCanvas(SelectedCanvas.name, passwordBox.Password);
        }
        #endregion

        #region SelectProtectedCanvas Command
        private ICommand _selectProtectedCanvasCommand;
        public ICommand SelectProtectedCanvasCommand
        {
            get
            {
                return _selectProtectedCanvasCommand ?? (_selectProtectedCanvasCommand = new RelayCommand<Object>(SelectProtectedCanvas));
            }
        }

        private void SelectProtectedCanvas(object o)
        {
            SelectedCanvas = o as Templates.Canvas;            
        }
        #endregion

        #region ResetServer Command
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
            DrawingService.RefreshCanvases();
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

            foreach (Room room in rooms)
            {
                if (room.name == message.chatroomName && !room.Chatter.Contains(cm))
                {
                    ctxTaskFactory.StartNew(() => room.Chatter.Add(cm)).Wait();
                }
                OnPropertyChanged("selectedRoom");
            }
        }

        private void GetChatrooms(RoomList chatrooms)
        {
            joinableRooms = new AsyncObservableCollection<Room>();
            foreach(string room in chatrooms.chatrooms)
            {
                string roomName = room.Remove(0,10);
                Room newRoom = new Room { name = roomName };
                if(!rooms.Contains(newRoom))
                {
                    joinableRooms.Add(newRoom);
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
                selectedRoom = rooms.First();
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

        private void JoinCanvasRoom(AccessCanvasResponse response)
        {
            if(UserMode != UserModes.Drawing && AccessingCanvas)
            {
                if (response.isPasswordValid)
                {
                    DrawingService.JoinCanvas(response.canvasName, "");

                    if (UserMode == UserModes.Gallery)
                    {
                        IsChatWindowClosing = true;
                        CloseChatWindow?.Invoke();
                    }

                    if (!ConnectionService.hasUserDoneTutorial)
                    {
                        UserMode = UserModes.Tutorial;
                    }
                    else
                    {
                        if (UserMode != UserModes.Drawing)
                        {
                            UserMode = UserModes.Drawing;
                            DrawingService.DrawCanvas(SelectedCanvas);
                        }
                    }
                }
                else
                {
                    dialogService.ShowNotification("You entered the wrong password");
                }
                AccessingCanvas = false;
            }
        }

        private void BackToGallery()
        {
            IsChatWindowClosing = true;
            CloseChatWindow?.Invoke();
            UserMode = UserModes.Gallery;
        }

        private void CreateRoom(CreateChatroomResponse response)
        {
            if (response.isCreated)
            {
                chatService.JoinChatroom(response.chatroomName);
            }
            else
            {
                if(response.chatroomName != "MainRoom")
                {
                    dialogService.ShowNotification("This room already exists");
                }
            }
        }

        private void JoinRoom(JoinChatroomResponse response)
        {
            if (response.isChatroomJoined)
            {
                Room room = new Room { name = response.chatroomName };
                if (!rooms.Contains(room))
                {
                    rooms.Add(room);
                }
                selectedRoom = rooms.ElementAt(rooms.IndexOf(room));
            }
        }

        private void GoToTutorial()
        {
            IsChatWindowClosing = true;
            CloseChatWindow?.Invoke();
            UserMode = UserModes.Tutorial;
        }
        #endregion

        #region Tutorial
        private void UpdateTutorial()
        {
            switch (TutorialPage)
            {
                case 1:
                    TutorialText = TutorialTexts.INTRO;
                    TutorialTextTitle = TutorialTexts.INTRO_TITLE;
                    TutorialImageSource = TutorialTexts.INTRO_IMAGE;
                    break;
                case 2:
                    TutorialText = TutorialTexts.DRAWING_FORMS;
                    TutorialTextTitle = TutorialTexts.DRAWING_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.DRAWING_FORMS_IMAGE;
                    break;
                case 3:
                    TutorialText = TutorialTexts.ADD_DRAWING_FORM;
                    TutorialTextTitle = TutorialTexts.ADD_DRAWING_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.ADD_DRAWING_FORMS_IMAGE;
                    break;
                case 4:
                    TutorialText = TutorialTexts.MOVE_DRAWING_FORM;
                    TutorialTextTitle = TutorialTexts.MOVE_DRAWING_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.MOVE_DRAWING_FORMS_IMAGE;
                    break;
                case 5:
                    TutorialText = TutorialTexts.DELETE_DRAWING_FORM;
                    TutorialTextTitle = TutorialTexts.DELETE_DRAWING_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.DELETE_DRAWING_FORMS_IMAGE;
                    break;
                case 6:
                    TutorialText = TutorialTexts.EDIT_DRAWING_FORM;
                    TutorialTextTitle = TutorialTexts.EDIT_DRAWING_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.EDIT_DRAWING_FORMS_IMAGE;
                    break;
                case 7:
                    TutorialText = TutorialTexts.CONNECTION_FORMS;
                    TutorialTextTitle = TutorialTexts.CONNECTION_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.CONNECTION_FORMS_IMAGE;
                    break;
                case 8:
                    TutorialText = TutorialTexts.ADD_CONNECTION_FORM_BUTTON;
                    TutorialTextTitle = TutorialTexts.ADD_CONNECTION_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.ADD_CONNECTION_FORMS_BUTTON_IMAGE;
                    break;
                case 9:
                    TutorialText = TutorialTexts.ADD_CONNECTION_FORM_LINKED;
                    TutorialTextTitle = TutorialTexts.ADD_CONNECTION_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.ADD_CONNECTION_FORMS_LINKED_IMAGE;
                    break;
                case 10:
                    TutorialText = TutorialTexts.MOVE_CONNECTION_FORM;
                    TutorialTextTitle = TutorialTexts.MOVE_CONNECTION_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.MOVE_CONNECTION_FORMS_IMAGE;
                    break;
                case 11:
                    TutorialText = TutorialTexts.DELETE_CONNECTION_FORM;
                    TutorialTextTitle = TutorialTexts.DELETE_CONNECTION_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.DELETE_CONNECTION_FORMS_IMAGE;
                    break;
                case 12:
                    TutorialText = TutorialTexts.EDIT_CONNECTION_FORM;
                    TutorialTextTitle = TutorialTexts.EDIT_CONNECTION_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.EDIT_CONNECTION_FORMS_IMAGE;
                    break;
                case 13:
                    TutorialText = TutorialTexts.RESIZE_FORMS;
                    TutorialTextTitle = TutorialTexts.RESIZE_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.RESIZE_FORMS_IMAGE;
                    break;
                case 14:
                    TutorialText = TutorialTexts.ROTATE_FORM;
                    TutorialTextTitle = TutorialTexts.ROTATE_FORMS_TITLE;
                    TutorialImageSource = TutorialTexts.ROTATE_FORMS_IMAGE;
                    break;
                case 15:
                    TutorialText = TutorialTexts.TOOLS;
                    TutorialTextTitle = TutorialTexts.TOOLS_TITLE;
                    TutorialImageSource = TutorialTexts.TOOLS_IMAGE;
                    break;
                case 16:
                    TutorialText = TutorialTexts.CUT;
                    TutorialTextTitle = TutorialTexts.CUT_TITLE;
                    TutorialImageSource = TutorialTexts.CUT_IMAGE;
                    break;
                case 17:
                    TutorialText = TutorialTexts.DUPLICATE;
                    TutorialTextTitle = TutorialTexts.DUPLICATE_TITLE;
                    TutorialImageSource = TutorialTexts.DUPLICATE_IMAGE;
                    break;
                case 18:
                    TutorialText = TutorialTexts.STACK;
                    TutorialTextTitle = TutorialTexts.STACK;
                    TutorialImageSource = TutorialTexts.STACK_IMAGE;
                    break;
                case 19:
                    TutorialText = TutorialTexts.LASSO;
                    TutorialTextTitle = TutorialTexts.LASSO_TITLE;
                    TutorialImageSource = TutorialTexts.LASSO_IMAGE;
                    break;
                case 20:
                    TutorialText = TutorialTexts.CLEAR;
                    TutorialTextTitle = TutorialTexts.CLEAR_TITLE;
                    TutorialImageSource = TutorialTexts.CLEAR_IMAGE;
                    break;
                case 21:
                    TutorialText = TutorialTexts.CANVAS_RESIZE;
                    TutorialTextTitle = TutorialTexts.CANVAS_RESIZE_TITLE;
                    TutorialImageSource = TutorialTexts.CANVAS_RESIZE_IMAGE;
                    break;
                case 22:
                    TutorialText = TutorialTexts.END;
                    TutorialTextTitle = TutorialTexts.END_TITLE;
                    TutorialImageSource = TutorialTexts.END_IMAGE;
                    break;
                case 23:
                    TutorialPage = 1;
                    if (!ConnectionService.hasUserDoneTutorial)
                    {
                        ConnectionService.socket.Emit("userHasDoneTutorial", ConnectionService.username);
                        ConnectionService.hasUserDoneTutorial = true;
                    }
                    UserMode = UserModes.Drawing;
                    DrawingService.DrawCanvas(SelectedCanvas);
                    break;
                default:
                    break;
            }
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
            DrawingService.JoinCanvasRoom += JoinCanvasRoom;
            DrawingService.BackToGallery += BackToGallery;
            DrawingService.GoToTutorial += GoToTutorial;

            chatService.NewMessage += NewMessage;
            chatService.GetChatrooms += GetChatrooms;
            chatService.RoomCreation += CreateRoom;
            chatService.RoomJoin += JoinRoom;

            rooms.Add(new Room { name = "MainRoom" });
        }
    }
}