using System;

namespace PolyPaint.Enums
{
    public static class TutorialTexts
    {
        public static string INTRO_TITLE = "Tutorial Introduction";
        public static string INTRO = "Welcome to the PolyPaint tutorial. You can navigate through the steps with the \"Previous\" and the \"Next\" buttons."
                                      + Environment.NewLine + Environment.NewLine + "You can leave the tutorial at anytime with the \"Leave Tutorial\" button in the top right corner";
        public static string INTRO_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string DRAWING_FORMS_TITLE = "Drawing Forms";
        public static string DRAWING_FORMS = "First, let's go through the drawing forms and the actions you can perform on them. The drawing forms that are available are:"
                                               + Environment.NewLine + "-Classes"
                                               + Environment.NewLine + "-Roles"
                                               + Environment.NewLine + "-Activities"
                                               + Environment.NewLine + "-Artefacts"
                                               + Environment.NewLine + "-Comments";
        public static string DRAWING_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string ADD_DRAWING_FORMS_TITLE = "Add Drawing Forms";
        public static string ADD_DRAWING_FORM = "You can add a drawing form by selecting the form you want to add in the menu bar and then click where you want to add it on the canvas.";
        public static string ADD_DRAWING_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string MOVE_DRAWING_FORMS_TITLE = "Move Drawing Forms";
        public static string MOVE_DRAWING_FORM = "You can move a drawing form by first selecting it on the canvas by simply clicking on it. Then, you can drag it wherever you want on the canvas.";
        public static string MOVE_DRAWING_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string DELETE_DRAWING_FORMS_TITLE = "Delete Drawing Forms";
        public static string DELETE_DRAWING_FORM = "You can delete a drawing form by selecting it and then click on the Thrashcan button that will show up at the top right of the selection.";
        public static string DELETE_DRAWING_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string EDIT_DRAWING_FORMS_TITLE = "Edit Drawing Forms";
        public static string EDIT_DRAWING_FORM = "You can edit a drawing form by selecting it and then click on the Pen button that will show up at the top right of the selection."
                                                + Environment.NewLine + "You can edit:"
                                                + Environment.NewLine + "-The name of the form"
                                                + Environment.NewLine + "-The border style"
                                                + Environment.NewLine + "-The border color"
                                                + Environment.NewLine + "-The form fill color";
        public static string EDIT_DRAWING_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string CONNECTION_FORMS_TITLE = "Connection Forms";
        public static string CONNECTION_FORMS = "Now, let's go through the connection forms and the actions you can perform on them.";
        public static string CONNECTION_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string ADD_CONNECTION_FORMS_TITLE = "Add Drawing Forms";
        public static string ADD_CONNECTION_FORM_BUTTON = "You can add a connection form in several ways. First, you can add one by selecting the connection form from the menu bar and then click and drag where you want to add it on the canvas.";
        public static string ADD_CONNECTION_FORMS_BUTTON_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string ADD_CONNECTION_FORM_LINKED = "Another way you can add a connection form is by selecting a drawing form and then click and drag from the red adorner around the drawing form."
                                                    + Environment.NewLine + "However, there are some constraints on link creation:"
                                                    + Environment.NewLine + "A role can only have a unidirectional links towards an activity"
                                                    + Environment.NewLine + "An artefact can only have a unidirectional links towards an activity"
                                                    + Environment.NewLine + "An activity can only have a unidirectional links towards an artefact";
        public static string ADD_CONNECTION_FORMS_LINKED_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string MOVE_CONNECTION_FORMS_TITLE = "Move Connection Forms";
        public static string MOVE_CONNECTION_FORM = "You can move a drawing form by first selecting it on the canvas by simply clicking on it. Then, you can drag it wherever you want on the canvas.";
        public static string MOVE_CONNECTION_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string DELETE_CONNECTION_FORMS_TITLE = "Delete Connection Forms";
        public static string DELETE_CONNECTION_FORM = "You can delete a connection form by selecting it and then click on the Thrashcan button that will show up at the top right of the selection.";
        public static string DELETE_CONNECTION_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string EDIT_CONNECTION_FORMS_TITLE = "Edit Connection Forms";
        public static string EDIT_CONNECTION_FORM = "You can edit a connection form by selecting it and then click on the Pen button that will show up at the top right of the selection."
                                                + Environment.NewLine + "You can edit:"
                                                + Environment.NewLine + "-The name of the connection"
                                                + Environment.NewLine + "-The connection style"
                                                + Environment.NewLine + "-The connection color"
                                                + Environment.NewLine + "-The connection type"
                                                + Environment.NewLine + "-The connection thickness"
                                                + Environment.NewLine + "-The connection multiplicity";
        public static string EDIT_CONNECTION_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string RESIZE_FORMS_TITLE = "Resize Forms";
        public static string RESIZE_FORMS = "You can resize a form by selecting it and then dragging one of the gray adorners around the form. You can only resize if one form is selected.";
        public static string RESIZE_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string ROTATE_FORMS_TITLE = "Rotate Forms";
        public static string ROTATE_FORM = "You can rotate a form by selecting it and then dragging the blue square adorner at the top. You can only rotate if one form is selected.";
        public static string ROTATE_FORMS_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string TOOLS_TITLE = "PolyPaint Tools";
        public static string TOOLS = "We will now go through the different tools available in PolyPaint.";
        public static string TOOLS_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string CUT_TITLE = "Cut Tool";
        public static string CUT = "The Cut Tool allows you to remove a form from the canvas.";
        public static string CUT_IMAGE = "/Resources/TutorialImages/Tutorial.png";


        public static string DUPLICATE_TITLE = "Paste / Duplicate Tool";
        public static string DUPLICATE = "This tool allows you to do two things:"
                 + Environment.NewLine + "First, if you have selected forms, this tool will duplicate all the selected forms."
                 + Environment.NewLine + "Second, if you have no selected forms, this tool will paste the last stroke that you cut from the canvas.";
        public static string DUPLICATE_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string STACK_TITLE = "Stack Tool";
        public static string STACK = "This tool allows you to remove several forms from the canvas, and keep them is store to add them later on."
             + Environment.NewLine + "You can only stack the forms that you added to the canvas during your current session.";
        public static string STACK_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string LASSO_TITLE = "Lasso Tool";
        public static string LASSO = "This tool allows you to select multiple forms at the same time. Once you select it, you can click and drag the mouse around the forms you want to select.";
        public static string LASSO_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string CLEAR_TITLE = "Clear Canvas Tool";
        public static string CLEAR = "This tool allows you to delete every form on the canvas.";
        public static string CLEAR_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string CLASS_CODE_TITLE = "Class Code Tool";
        public static string CLASS_CODE = "This tool allows you to create a class form from a C# class file. You can either copy and paste your code in the toolbox or import the file from your computer.";
        public static string CLASS_CODE_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string CLASS_WIDTH_TITLE = "Class Width Tool";
        public static string CLASS_WIDTH = "This tool allows you to adjust the width of all the classes to the width of the largest class on the canvas.";
        public static string CLASS_WIDTH_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string FORMS_ALIGNMENT_TITLE = "Alignment Tool";
        public static string FORMS_ALIGNMENT = "This tool allows you to align the forms that you are currently selecting."
                       + Environment.NewLine + "You can align your forms to the left or to the center. The buttons to access the tool appear when you select forms.";
        public static string FORMS_ALIGNMENT_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string CANVAS_RESIZE_TITLE = "Canvas Resize Tool";
        public static string CANVAS_RESIZE = "You can change the canvas' size by dragging the adorners around it.";
        public static string CANVAS_RESIZE_IMAGE = "/Resources/TutorialImages/Tutorial.png";

        public static string END_TITLE = "The End";
        public static string END = "You have now completed the tutorial! Have fun using PolyPaint!";
        public static string END_IMAGE = "/Resources/TutorialImages/Tutorial.png";
    }
}