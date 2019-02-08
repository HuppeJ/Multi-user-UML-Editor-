using PolyPaint.VueModeles;
using System.Collections.ObjectModel;

namespace PolyPaint.Modeles
{
    public class Room : ViewModelBase
    {
        public string name { get; set; }
        public byte[] photo { get; set; }
        public ObservableCollection<ChatMessage> Chatter { get; set; }

        private bool _isLoggedIn = true;
        public bool isLoggedIn
        {
            get { return _isLoggedIn; }
            set { _isLoggedIn = value; OnPropertyChanged(); }
        }

        public Room() { Chatter = new ObservableCollection<ChatMessage>(); }
    }
}