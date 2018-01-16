using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Lab6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int time;
        string[] dll = new string[]
        {"System.dll",
         "System.Linq.dll",
         "System.Threading.Tasks.dll",
         "System.Windows.Forms.dll"
        };

        static Dictionary<string, string> providerOptions = new Dictionary<string, string>
        {
            {"CompilerVersion", "v4.0"}
        };
        CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions); //Создаем объект компилятора
        CompilerParameters compilerParams = new CompilerParameters //Задаем параметры компилятору
        {
            GenerateInMemory = true,
            GenerateExecutable = true,
        };
        

        /// <summary>
        /// Компилирует
        /// </summary>
        public void Compile()
        {

            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, textBox_input.Text); //Получаем результат исполнения исходного кода при примененных параметрах

            #region Отлов ошибок
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }
#endregion

            //Используем рефлексию для манипуляциями полученных классов и методов
            Assembly assembly = results.CompiledAssembly; //Получаем скомпилированную сборку в объект типа Assembly
            Type program = assembly.GetType("Lab6.Program"); //
            MethodInfo main = program.GetMethod("Main"); //
            main.Invoke(null, null); //Запускаем метод main
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time++;
            if (time >= 3)
            {
                Compile();
                timer.Stop();
                time = 0;
            }
        }

        private void textBox_input_TextChanged(object sender, EventArgs e)
        {
            timer.Stop();
            time = 0;
            timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_input.Text =
@"namespace Lab6
   {
        public class Program
        {
            public static void Main()
            {

            }
        }
   }";
            compilerParams.ReferencedAssemblies.AddRange(dll);//Добавляем библиотеки к параметрам компилятора
        }
    }
}
