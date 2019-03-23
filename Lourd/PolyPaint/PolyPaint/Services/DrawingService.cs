using Newtonsoft.Json.Linq;
using PolyPaint.CustomInk;
using PolyPaint.Enums;
using PolyPaint.Templates;
using Quobject.SocketIoClientDotNet.Client;
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
        public static event Action<JoinCanvasRoomResponse> JoinCanvasRoom;
        public static event Action<InkCanvasStrokeCollectedEventArgs> AddStroke;
        public static event Action<StrokeCollection> RemoveStrokes;
        public static event Action<InkCanvasStrokeCollectedEventArgs> UpdateStroke;
        public static event Action<StrokeCollection> UpdateSelection;
        public static event Action<StrokeCollection> UpdateDeselection;

        public static event Action<PublicCanvases> UpdatePublicCanvases;
        public static event Action<PrivateCanvases> UpdatePrivateCanvases;
        public static event Action BackToGallery;

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static string canvasName;

        public DrawingService()
        {
            
        }

        public static void Initialize(object o)
        {
            socket.On(Socket.EVENT_RECONNECT, () =>
            {
                if(canvasName != null)
                {
                    JoinCanvas(canvasName);
                }
            });

            socket.On("createCanvasResponse", (data) =>
            {
                CreateCanvasResponse response = serializer.Deserialize<CreateCanvasResponse>((string)data);
                if (response.isCreated)
                {
                    canvasName = response.canvasName;
                    RefreshCanvases();
                }
            });

            socket.On("canvasCreated", (data) =>
            {
                RefreshCanvases();
            });

            socket.On("updateCanvasPasswordResponse", (data) =>
            {
                UpdateCanvasPasswordResponse response = serializer.Deserialize<UpdateCanvasPasswordResponse>((string)data);
                if (response.isPasswordUpdated)
                {
                    RefreshCanvases();
                }
            });
            
            socket.On("canvasPasswordUpdated", (data) =>
            {
                LeaveCanvas();
                Application.Current.Dispatcher.Invoke(new Action(() => { BackToGallery(); }), DispatcherPriority.Render);
            });

            socket.On("joinCanvasRoomResponse", (data) =>
            {
                JoinCanvasRoomResponse response = serializer.Deserialize<JoinCanvasRoomResponse>((string)data);
                if (response.isCanvasRoomJoined)
                {
                    canvasName = response.canvasName;
                }

                Application.Current.Dispatcher.Invoke(new Action(() => { JoinCanvasRoom(response); }), DispatcherPriority.Render);
            });

            socket.On("getPublicCanvasResponse", (data) =>
            {
                PublicCanvases canvases = serializer.Deserialize<PublicCanvases>((string)data);

                Application.Current.Dispatcher.Invoke(new Action(() => { UpdatePublicCanvases(canvases); }), DispatcherPriority.Render);
            });

            socket.On("getPrivateCanvasResponse", (data) =>
            {
                PrivateCanvases canvases = serializer.Deserialize<PrivateCanvases>((string)data);

                Application.Current.Dispatcher.Invoke(new Action(() => { UpdatePrivateCanvases(canvases); }), DispatcherPriority.Render);
            });

            socket.On("formCreated", (data) =>
            {
                dynamic response = JObject.Parse((string)data);
                if (!username.Equals((string)response.username))
                {
                    CustomStroke customStroke = createShapeStroke(response.forms[0]);
                    InkCanvasStrokeCollectedEventArgs eventArgs = new InkCanvasStrokeCollectedEventArgs(customStroke);
                    Application.Current.Dispatcher.Invoke(new Action(() => { AddStroke(eventArgs); }), DispatcherPriority.ContextIdle);
                }
            });

            socket.On("formsDeleted", (data) =>
            {
                dynamic response = JObject.Parse((string)data);
                if (!username.Equals((string)response.username))
                {
                    StrokeCollection strokes = new StrokeCollection();
                    foreach (dynamic shape in response.forms)
                    {
                        strokes.Add(createShapeStroke(shape));
                    }
                    Application.Current.Dispatcher.Invoke(new Action(() => { RemoveStrokes(strokes); }), DispatcherPriority.ContextIdle);
                }
            });

            socket.On("formsUpdated", (data) =>
            {
                dynamic response = JObject.Parse((string)data);
                if (!username.Equals((string)response.username))
                {
                    foreach (dynamic shape in response.forms)
                    {
                        InkCanvasStrokeCollectedEventArgs eventArgs = new InkCanvasStrokeCollectedEventArgs(createShapeStroke(shape));
                        Application.Current.Dispatcher.Invoke(new Action(() => { UpdateStroke(eventArgs); }), DispatcherPriority.Render);
                    }
                }
            });

            socket.On("formsSelected", (data) =>
            {
                dynamic response = JObject.Parse((string)data);
                if (!username.Equals((string)response.username))
                {
                    StrokeCollection strokes = new StrokeCollection();
                    foreach (dynamic shape in response.forms)
                    {
                        strokes.Add(createShapeStroke(shape));
                        Application.Current.Dispatcher.Invoke(new Action(() => { UpdateSelection(strokes); }), DispatcherPriority.Render);
                    }
                }
            });

            socket.On("formsDeselected", (data) =>
            {
                dynamic response = JObject.Parse((string)data);
                if (!username.Equals((string)response.username))
                {
                    StrokeCollection strokes = new StrokeCollection();
                    foreach (dynamic shape in response.forms)
                    {
                        strokes.Add(createShapeStroke(shape));
                        Application.Current.Dispatcher.Invoke(new Action(() => { UpdateDeselection(strokes); }), DispatcherPriority.Render);
                    }
                }
            });
            RefreshCanvases();
        }

        public static void CreateCanvas(Templates.Canvas canvas)
        {
            EditCanevasData editCanevasData = new EditCanevasData(username, canvas);
            socket.Emit("createCanvas", serializer.Serialize(editCanevasData));
        }

        public static void ChangeCanvasProtection(string canvasName, string password)
        {
            EditGalleryData editGallerysData = new EditGalleryData(username, canvasName, password);
            socket.Emit("updateCanvasPassword", serializer.Serialize(editGallerysData));
        }

        public static void JoinCanvas(string roomName, string password)
        {
            EditGalleryData editGalleryData = new EditGalleryData(username, roomName, password);
            socket.Emit("joinCanvasRoom", serializer.Serialize(editGalleryData));
        }

        public static void LeaveCanvas()
        {
            EditGalleryData editGalleryData = new EditGalleryData(username, canvasName);
            socket.Emit("leaveCanvasRoom", serializer.Serialize(editGalleryData));
        }

        public static void RefreshCanvases()
        {
            socket.Emit("getPublicCanvas");
            socket.Emit("getPrivateCanvas", username);
        }

        public static void ResetServer()
        {
            socket.Emit("resetServerState");
        }

        public static void CreateShape(ShapeStroke shapeStroke)
        {
            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(shapeStroke);
            socket.Emit("createForm", serializer.Serialize(createUpdateFormsData(strokes)));
        }

        public static void RemoveShapes(StrokeCollection strokes)
        {
            socket.Emit("deleteForms", serializer.Serialize(createUpdateFormsData(strokes)));
        }

        public static void UpdateShapes(StrokeCollection strokes)
        {
            socket.Emit("updateForms", serializer.Serialize(createUpdateFormsData(strokes)));
        }

        public static void SelectShapes(StrokeCollection strokes)
        {
            socket.Emit("selectForms", serializer.Serialize(createUpdateFormsData(strokes)));
        }

        public static void DeselectShapes(StrokeCollection strokes)
        {
            socket.Emit("deselectForms", serializer.Serialize(createUpdateFormsData(strokes)));
        }

        private static UpdateFormsData createUpdateFormsData(StrokeCollection strokes)
        {
            List<BasicShape> forms = new List<BasicShape>();
            foreach (CustomStroke customStroke in strokes)
            {
                if (!customStroke.isLinkStroke())
                {
                    forms.Add((customStroke as ShapeStroke).GetBasicShape());
                }
            }

            return new UpdateFormsData(username, canvasName, forms);
        }

        private static ShapeStroke createShapeStroke(dynamic shape)
        {
            StylusPointCollection points = new StylusPointCollection();

            StylusPoint point = new StylusPoint((double)shape.shapeStyle.coordinates.x, (double)shape.shapeStyle.coordinates.y);
            points.Add(point);

            ShapeStroke shapeStroke;
            StrokeTypes type = (StrokeTypes) shape.type;

            switch (type)
            {
                case StrokeTypes.CLASS_SHAPE:
                    shapeStroke = new ClassStroke(shape.ToObject<ClassShape>(), points);
                    break;
                case StrokeTypes.ARTIFACT:
                    shapeStroke = new ArtifactStroke(shape.ToObject<BasicShape>(), points);
                    break;
                case StrokeTypes.ACTIVITY:
                    shapeStroke = new ActivityStroke(shape.ToObject<BasicShape>(), points);
                    break;
                case StrokeTypes.ROLE:
                    shapeStroke = new ActorStroke(shape.ToObject<BasicShape>(), points);
                    break;
                //case StrokeTypes.COMMENT:
                //    customStroke = new CommentStroke(points);
                //    break;
                //case StrokeTypes.PHASE:
                //    customStroke = new PhaseStroke(points);
                //    break;
                default:
                    shapeStroke = new ClassStroke(shape.ToObject<ClassShape>(), points);
                    break;

            }
            shapeStroke.guid = Guid.Parse((string)shape.id);

            return shapeStroke;
        }

    }
}
