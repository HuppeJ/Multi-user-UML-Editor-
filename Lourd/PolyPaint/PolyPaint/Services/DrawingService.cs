using Newtonsoft.Json.Linq;
using PolyPaint.CustomInk;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;

namespace PolyPaint.Services
{
    class DrawingService: ConnectionService
    {
        public static event Action<string> JoinCanvasRoom;
        public static event Action<InkCanvasStrokeCollectedEventArgs> AddStroke;
        public static event Action<CustomStroke> UpdateStroke;

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static string canvasName;

        public DrawingService()
        {
            
        }

        public static void Initialize(object o)
        {

            socket.On("joinCanvasTestResponse", (data) =>
            {
                //string joinCanvas = serializer.Deserialize<string>((string)data);

                //JoinCanvasRoom?.Invoke(joinCanvas);
            });

            socket.On("CanvasUpdateTestResponse", (data) =>
            {
                BasicShape updatedStroke = serializer.Deserialize<BasicShape>((string)data);

                UpdateStroke?.Invoke(createStroke(updatedStroke));
            });

            socket.On("createCanvasResponse", (data) =>
            {
                CreateCanvasResponse response = serializer.Deserialize<CreateCanvasResponse>((string)data);
                if (response.isCreated)
                {
                    canvasName = response.canvasName;
                }
            });

            socket.On("joinCanvasRoomResponse", (data) =>
            {
                JoinCanvasRoomResponse response = serializer.Deserialize<JoinCanvasRoomResponse>((string)data);
                if (response.isCanvasRoomJoined)
                {
                    canvasName = response.canvasName;
                }
            });


            socket.On("formCreated", (data) =>
            {
                dynamic response = JObject.Parse((string)data);
                CustomStroke customStroke = createStroke(response.forms[0]);
                InkCanvasStrokeCollectedEventArgs eventArgs = new InkCanvasStrokeCollectedEventArgs(customStroke);
                Application.Current.Dispatcher.Invoke(new Action(() => { AddStroke(eventArgs); }), DispatcherPriority.ContextIdle);
            });
        }

        public static void CreateCanvas(Templates.Canvas canvas)
        {
            EditCanevasData editCanevasData = new EditCanevasData(username, canvas);
            socket.Emit("createCanvas", serializer.Serialize(editCanevasData));
        }

        public static void JoinCanvas(string roomName)
        {
            EditGalleryData editGalleryData = new EditGalleryData(username, roomName);
            socket.Emit("joinCanvasRoom", serializer.Serialize(editGalleryData));
        }

        public static void CreateShape(CustomStroke customStroke)
        {
            BasicShape basicShape = customStroke.GetBasicShape();
            List<BasicShape> forms = new List<BasicShape>();
            forms.Add(basicShape);

            UpdateFormsData updateFormsData = new UpdateFormsData(username, canvasName, forms);
            socket.Emit("createForm", serializer.Serialize(updateFormsData));
        }

        public static void UpdateShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> linksTo, List<string> linksFrom)
        {
            BasicShape updatedShape = new BasicShape()
            {
                id = id,
                type = type,
                name = name,
                shapeStyle = shapeStyle,
                linksTo = linksTo,
                linksFrom = linksFrom
            };

            socket.Emit("CanvasUpdateTest", serializer.Serialize(updatedShape));
        }

        private static CustomStroke createStroke(dynamic shape)
        {
            StylusPointCollection points = new StylusPointCollection();

            StylusPoint point = new StylusPoint((double)shape.shapeStyle.coordinates.x, (double)shape.shapeStyle.coordinates.y);
            points.Add(point);

            CustomStroke customStroke;
            StrokeTypes type = (StrokeTypes) shape.type;

            switch (type)
            {
                case StrokeTypes.CLASS_SHAPE:
                    customStroke = new ClassStroke(shape.ToObject<ClassShape>(), points);
                    break;
                case StrokeTypes.ARTIFACT:
                    customStroke = new ArtifactStroke(shape.ToObject<BasicShape>(), points);
                    break;
                case StrokeTypes.ACTIVITY:
                    customStroke = new ActivityStroke(shape.ToObject<BasicShape>(), points);
                    break;
                case StrokeTypes.ROLE:
                    customStroke = new ActorStroke(shape.ToObject<BasicShape>(), points);
                    break;
                //case StrokeTypes.COMMENT:
                //    customStroke = new CommentStroke(points);
                //    break;
                //case StrokeTypes.PHASE:
                //    customStroke = new PhaseStroke(points);
                //    break;
                default:
                    customStroke = new ClassStroke(shape.ToObject<ClassShape>(), points);
                    break;

            }
            customStroke.guid = Guid.Parse((string)shape.id);

            return customStroke;
        }

    }
}
