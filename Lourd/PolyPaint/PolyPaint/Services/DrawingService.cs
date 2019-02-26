using PolyPaint.Templates;
using System;
using System.Collections.Generic;

namespace PolyPaint.Services
{
    class DrawingService: ConnectionService
    {
        public event Action<BasicShape> AddStroke;
        public event Action<BasicShape> UpdateStroke;
        public event Action<string> JoinCanvasRoom;

        public DrawingService()
        {
            
        }

        public void Initialize(object o)
        {
            socket.On("joinCanvasTestResponse", (data) =>
            {
                string joinCanvas = serializer.Deserialize<string>((string)data);

                JoinCanvasRoom?.Invoke(joinCanvas);
            });

            socket.On("canvasUpdateTestResponse", (data) =>
            {
                BasicShape updatedStroke = serializer.Deserialize<BasicShape>((string)data);

                UpdateStroke?.Invoke(updatedStroke);
            });
        }

        // TODO: Ajouter le nom de la room quand ça va être mis sur le serveur
        public void JoinCanvas()
        {
            socket.Emit("joinCanvasTest");
        }

        public void UpdateShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> links)
        {
            BasicShape updatedShape = new BasicShape()
            {
                id = id,
                type = type,
                name = name,
                shapeStyle = shapeStyle,
                links = links
            };

            socket.Emit("canvasUpdateTest", serializer.Serialize(updatedShape));
        }
    }
}
