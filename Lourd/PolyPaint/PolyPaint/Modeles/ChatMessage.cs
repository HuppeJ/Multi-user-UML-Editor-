using System;

namespace PolyPaint.Modeles
{
    public class ChatMessage
    {
        public string text { get; set; }
        public string sender { get; set; }
        public DateTime createdAt { get; set; }
        public bool isOriginNative { get; set; }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                ChatMessage p = (ChatMessage)obj;
                return ((text == p.text) && (sender == p.sender) && (createdAt == p.createdAt)
                    && (isOriginNative == p.isOriginNative));
            }
        }
    }
}
