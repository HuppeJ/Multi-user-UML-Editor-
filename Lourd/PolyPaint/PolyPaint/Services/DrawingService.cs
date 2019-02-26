using System;

namespace PolyPaint.Services
{
    class DrawingService: ConnectionService
    {
        public event Action<string> NewStroke;

        public DrawingService()
        {

        }

    }
}
