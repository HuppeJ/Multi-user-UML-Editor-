﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Templates
{
    class EditGalleryData
    {
        public string username { get; set; }
        public string canevasName { get; set; }
        public string password { get; set; }

        public EditGalleryData() {}

        public EditGalleryData(string username, string canevasName)
        {
            this.username = username;
            this.canevasName = canevasName;
        }

        public EditGalleryData(string username, string canevasName, string password)
        {
            this.username = username;
            this.canevasName = canevasName;
            this.password = password;
        }
    }

    class EditCanevasData
    {
        public string username { get; set; }
        public Canvas canevas { get; set; }

        public EditCanevasData(string username, Canvas canevas)
        {
            this.username = username;
            this.canevas = canevas;
        }
    }

    class UpdateFormsData
    {
        public string username { get; set; }
        public string canevasName { get; set; }
        public List<BasicShape> forms { get; set; }

        public UpdateFormsData() { }

        public UpdateFormsData(string username, string canevasName, List<BasicShape> forms)
        {
            this.username = username;
            this.canevasName = canevasName;
            this.forms = forms;
        }
    }

    class CreateCanvasResponse
    {
        public bool isCreated { get; set; }
        public string canvasName { get; set; }
    }

    class JoinCanvasRoomResponse
    {
        public bool isCanvasRoomJoined { get; set; }
        public string canvasName { get; set; }
    }
}