namespace PolyPaint.Templates
{
    class EditChatroomData
    {
        public string username { get; set; }
        public string chatroomName { get; set; }

        public EditChatroomData()
        {
        }

        public EditChatroomData(string username, string chatroomName)
        {
            this.username = username;
            this.chatroomName = chatroomName;
        }
    }
}