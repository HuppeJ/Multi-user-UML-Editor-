using System;

namespace PolyPaint.Enums
{
    public static class TutorialTexts
    {
        public static string INTRO_TITLE = "Tutorial Introduction";
        public static string INTRO = "Welcome to the PolyPaint tutorial. You can navigate through the steps with the \"Previous\" and the \"Next\" buttons."
                                      + Environment.NewLine + Environment.NewLine + "You can leave the tutorial at anytime with the \"Leave Tutorial\" button in the top right corner";

        public static string DRAWING_FORMS_TITLE = "Drawing Forms";
        public static string DRAWING_FORMS = "First, let's go through the drawing forms and the actions you can perform on them. The drawing forms that are available are:"
                                               + Environment.NewLine + "-Classes"
                                               + Environment.NewLine + "-Roles"
                                               + Environment.NewLine + "-Activities"
                                               + Environment.NewLine + "-Artefacts";

        public static string ADD_DRAWING_FORMS_TITLE = "Add Drawing Forms";
        public static string ADD_DRAWING_FORM = "You can add a drawing form by selecting the form you want to add in the menu bar and then click where you want to add it on the canvas.";

        public static string MOVE_DRAWING_FORMS_TITLE = "Move Drawing Forms";
        public static string MOVE_DRAWING_FORM = "You can move a drawing form by first selecting it on the canvas by simply clicking on it. Then, you can drag it wherever you want on the canvas.";

        public static string DELETE_DRAWING_FORMS_TITLE = "Delete Drawing Forms";
        public static string DELETE_DRAWING_FORM = "You can delete a drawing form by selecting it and then click on the Thrashcan button that will show up at the top right of the selection.";

        public static string EDIT_DRAWING_FORMS_TITLE = "Edit Drawing Forms";
        public static string EDIT_DRAWING_FORM = "You can edit a drawing form by selecting it and then click on the Pen button that will show up at the top right of the selection."
                                                + Environment.NewLine + "You can edit:"
                                                + Environment.NewLine + "-The name of the form"
                                                + Environment.NewLine + "-The border style"
                                                + Environment.NewLine + "-The border color"
                                                + Environment.NewLine + "-The form fill color";

        public static string CONNECTION_FORMS_TITLE = "Connection Forms";
        public static string CONNECTION_FORMS = "Now, let's go through the connection forms and the actions you can perform on them.";

        public static string ADD_CONNECTION_FORMS_TITLE = "Add Drawing Forms";
        public static string ADD_CONNECTION_FORM_BUTTON = "You can add a connection form in several ways. First, you can add one by selecting the connection form from the menu bar and then click and drag where you want to add it on the canvas.";
        public static string ADD_CONNECTION_FORM_LINKED = "Another way you can add a connection form is by selecting a drawing form and then click and drag from the red adorner around the drawing form."
                                                    + Environment.NewLine + "However, there are some constraints on link creation:"
                                                    + Environment.NewLine + "A role can only have a unidirectional links towards an activity"
                                                    + Environment.NewLine + "An artefact can only have a unidirectional links towards an activity"
                                                    + Environment.NewLine + "An activity can only have a unidirectional links towards an artefact";

        public static string MOVE_CONNECTION_FORMS_TITLE = "Move Connection Forms";
        public static string MOVE_CONNECTION_FORM = "You can move a drawing form by first selecting it on the canvas by simply clicking on it. Then, you can drag it wherever you want on the canvas.";

        public static string DELETE_CONNECTION_FORMS_TITLE = "Delete Connection Forms";
        public static string DELETE_CONNECTION_FORM = "You can delete a connection form by selecting it and then click on the Thrashcan button that will show up at the top right of the selection.";

        public static string EDIT_CONNECTION_FORMS_TITLE = "Edit Connection Forms";
        public static string EDIT_CONNECTION_FORM = "You can edit a connection form by selecting it and then click on the Pen button that will show up at the top right of the selection."
                                                + Environment.NewLine + "You can edit:"
                                                + Environment.NewLine + "-The name of the connection"
                                                + Environment.NewLine + "-The connection style"
                                                + Environment.NewLine + "-The connection color"
                                                + Environment.NewLine + "-The connection type"
                                                + Environment.NewLine + "-The connection thickness"
                                                + Environment.NewLine + "-The connection multiplicity";

        public static string RESIZE_FORMS_TITLE = "Resize Forms";
        public static string RESIZE_FORMS = "You can resize a form by selecting it and then";

        public static string ROTATE_FORMS_TITLE = "Rotate Forms";
        public static string ROTATE_FORM = "You can rotate a form by selecting it and then";
    }
}