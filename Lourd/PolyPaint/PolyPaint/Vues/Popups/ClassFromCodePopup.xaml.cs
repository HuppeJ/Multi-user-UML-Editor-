using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using System.Diagnostics;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for ClassFromCodePopup.xaml
    /// </summary>
    public partial class ClassFromCodePopup
    {
        private WindowDrawing windowDrawing = null;
        public List<string> baseMethods = new List<string>();

        public ClassFromCodePopup()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            var parent = Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            windowDrawing = (WindowDrawing)parent;
        }

        private void GenerateClass(object sender, EventArgs e)
        {
            windowDrawing.ClosePopups();

            List<string> methods = new List<string>();
            List<string> properties = new List<string>();
            string name = "";

            CSharpParser parserTest = new CSharpParser();
            SyntaxTree tree = parserTest.Parse(CodeTextBox.Text);

            CSharpCodeVistor visitor = new CSharpCodeVistor();
            tree.AcceptVisitor(visitor);

            methods = visitor.methods;
            properties = visitor.properties;
            name = visitor.name;

            windowDrawing.DrawClass(name, properties, methods);
        }

        private void ImportClass(object sender, EventArgs e)
        {
            windowDrawing.SelectCSFile();
        }

        private void Close(object sender, EventArgs e)
        {
            windowDrawing.ClosePopups();
        }
    }

    class CSharpCodeVistor : DepthFirstAstVisitor
    {
        private bool globalParametersDone = false;
        public List<string> methods = new List<string>();
        public List<string> properties = new List<string>();
        public string name = "";

        public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            name = constructorDeclaration.Name;
            base.VisitConstructorDeclaration(constructorDeclaration);
        }

        public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            globalParametersDone = true;
            string accessibility = methodDeclaration.FirstChild.ToString();
            if(accessibility == "public")
            {
                methods.Add("+" + methodDeclaration.Name + ": " + methodDeclaration.ReturnType.ToString());
            }
            else if(accessibility == "private")
            {
                methods.Add("-" + methodDeclaration.Name + ": " + methodDeclaration.ReturnType.ToString());
            }
            else
            {
                methods.Add("#" + methodDeclaration.Name + ": " + methodDeclaration.ReturnType.ToString());
            }
            
            base.VisitMethodDeclaration(methodDeclaration);
        }

        public override void VisitVariableInitializer(VariableInitializer variableInitializer)
        {
            if (!globalParametersDone)
            {
                string accessibility = variableInitializer.Parent.FirstChild.ToString();

                var parent = variableInitializer.Parent.Children.GetEnumerator();
                parent.MoveNext();
                parent.MoveNext();
                var type = parent.Current.ToString(); ;
                
                if (accessibility == "public")
                {
                    properties.Add("+" + variableInitializer.Name + ": " + type);
                }
                else if (accessibility == "private")
                {
                    properties.Add("-" + variableInitializer.Name + ": " + type);
                }
                else
                {
                    properties.Add("#" + variableInitializer.Name + ": " + type);
                }
            }
            base.VisitVariableInitializer(variableInitializer);
        }
    }
}
