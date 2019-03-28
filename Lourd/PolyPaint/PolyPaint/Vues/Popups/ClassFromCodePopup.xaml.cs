using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for ClassFromCodePopup.xaml
    /// </summary>
    public partial class ClassFromCodePopup : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private WindowDrawing windowDrawing = null;
        List<string> baseMethods = new List<string>();

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

            string source = CodeTextBox.Text;
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

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
                var name = type.Name;
                PropertyInfo[] privateProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                PropertyInfo[] publicProperties = type.GetProperties();
                List<string> properties = new List<string>();

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
                List<string> methods = new List<string>();

                foreach (MethodInfo info in publicMethods)
                {
                    if (!baseMethods.Contains(info.Name))
                    {
                        methods.Add("+" + info.Name + ": " + info.ReturnType.Name);
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

                windowDrawing.DrawClass(name, properties, methods);
            }
        }

        private void Close(object sender, EventArgs e)
        {
            windowDrawing.ClosePopups();
        }
    }
}
