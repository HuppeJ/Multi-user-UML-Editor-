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

            string[] methods = { "Equals", "GetHashCode", "GetType", "Finalize", "MemberwiseClone", "ToString" };
            baseMethods.AddRange(methods);
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
            /*
            CSharpParser parser = new CSharpParser();
            SyntaxTree syntaxTree = parser.Parse(CodeTextBox.Text, "code.cs");
            CSharpUnresolvedFile file = syntaxTree.ToTypeSystem();

            foreach (IUnresolvedTypeDefinition type in file.TopLevelTypeDefinitions)
            {
                name = type.Name;

                foreach (IUnresolvedMethod method in type.Methods)
                {
                    if(method.Accessibility == Accessibility.Public)
                    {
                        methods.Add("+" + method.Name + ": " + method.ReturnType);
                    }
                    else if(method.Accessibility == Accessibility.Private)
                    {
                        methods.Add("-" + method.Name + ": " + method.ReturnType);
                    }
                    else
                    {
                        methods.Add("#" + method.Name + ": " + method.ReturnType);
                    }
                }

                foreach (IUnresolvedProperty property in type.Properties)
                {
                    if (property.Accessibility == Accessibility.Public)
                    {
                        methods.Add("+" + property.Name + ": " + property.ReturnType);
                    }
                    else if (property.Accessibility == Accessibility.Private)
                    {
                        methods.Add("-" + property.Name + ": " + property.ReturnType);
                    }
                    else
                    {
                        methods.Add("#" + property.Name + ": " + property.ReturnType);
                    }
                }
            }

            string source = CodeTextBox.Text;
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                WarningLevel = 4
            };

            parameters.ReferencedAssemblies.Add("System.dll");

            var provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);

            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                    MessageBox.Show(error.ToString());
                return;
            }

            var assembly = results.CompiledAssembly;
            var types = assembly.GetTypes();

            foreach (Type type in types)
            {
                PropertyInfo[] privateProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                PropertyInfo[] publicProperties = type.GetProperties();
                

                foreach (PropertyInfo info in publicProperties)
                {
                    properties.Add("+" + info.Name + ": " + info.PropertyType);
                }

                foreach (PropertyInfo info in privateProperties)
                {
                    properties.Add("-" + info.Name + ": " + info.PropertyType);
                }

                MethodInfo[] privateMethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo[] publicMethods = type.GetMethods();
                

                foreach (MethodInfo info in publicMethods)
                {
                    if (!baseMethods.Contains(info.Name))
                    {
                        
                    }
                }

                foreach (MethodInfo info in privateMethods)
                {
                    if (!baseMethods.Contains(info.Name))
                    {
                        if (info.IsPrivate)
                        {
                            methods.Add("-" + info.Name + ": " + info.ReturnType.Name);
                        }
                        else
                        {
                            methods.Add("#" + info.Name + ": " + info.ReturnType.Name);
                        }
                    }
                }
            }
            */
        }

        private void ImportClass(object sender, EventArgs e)
        {

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
