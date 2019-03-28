using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
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
                var privateProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                var publicProperties = type.GetProperties();
                var privateMethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                var publicMethods = type.GetMethods();
            }
        }

        private void Close(object sender, EventArgs e)
        {
            windowDrawing.ClosePopups();
        }
    }
}
