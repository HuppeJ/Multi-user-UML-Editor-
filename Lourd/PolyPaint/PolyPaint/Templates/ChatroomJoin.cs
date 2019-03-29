namespace PolyPaint.Templates
{
    class EditChatroomData
    {
        public string chatroomName { get; set; }
        public string username { get; set; }

        public EditChatroomData()
        {
        }

        public EditChatroomData(string chatroomName, string username)
        {
            this.chatroomName = chatroomName;
            this.username = username;
        }
    }
}