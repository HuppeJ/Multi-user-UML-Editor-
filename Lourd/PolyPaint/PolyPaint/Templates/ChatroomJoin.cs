namespace PolyPaint.Templates
{
    class ChatroomJoin
    {
        public string chatroomName { get; set; }
        public string username { get; set; }

        public ChatroomJoin()
        {
        }

        public ChatroomJoin(string chatroomName, string username)
        {
            this.chatroomName = chatroomName;
            this.username = username;
        }
    }
}