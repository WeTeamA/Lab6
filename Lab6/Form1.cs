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
using System.IO;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace Lab6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Переменная для счета времени, прошедшего после написания кода (для активации компиляции)
        /// </summary>
        int time;
        /// <summary>
        /// Текст компилируемого кода
        /// </summary>
        string code;
        /// <summary>
        /// Стиль ошибок текста
        /// </summary>
        Style ErrorCodeStyle = new TextStyle(Brushes.Black, Brushes.LightPink, FontStyle.Bold);
        string[] dll = new string[]
        {"System.dll",
         "System.Linq.dll",
         "System.Threading.Tasks.dll",
         "System.Windows.Forms.dll",
         "mscorlib.dll",
         "System.Data.dll"
        };

        static Dictionary<string, string> Options = new Dictionary<string, string>
        {
            {"CompilerVersion", "v4.0"}
        };
                

        CSharpCodeProvider CSharpProvider = new CSharpCodeProvider(Options);

        CompilerParameters Params = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };
        StringBuilder strb = new StringBuilder();
        public void Compile()
        {

            code = @"using System; 
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
using System.IO; 
namespace Lab6 //Скрыть от пользователя 
{ 
public class Program 
{ 
interface Write 
{ 
void WriteLine(string x); 
} 
public class ui:Write  
{ 
public void WriteLine(string x) 
{ 
Console.WriteLine(x); 
} 
}
static ui UI = new ui();

        " + fastColoredTextBox1.Text + "    }}";
            CompilerResults results = CSharpProvider.CompileAssemblyFromSource(Params, code);

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    strb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));                    
                    int Line = error.Line - 31;
                    Range rng = new Range(fastColoredTextBox1, Line - 1);
                    rng.SetStyle(ErrorCodeStyle);
                }
                throw new InvalidOperationException(strb.ToString());
            }

            Assembly assembly = results.CompiledAssembly;
            Type program = assembly.GetType("Lab6.Program");
            MethodInfo method = program.GetMethod("Main");

            timer.Stop();
            time = 0;

            object obj = results.CompiledAssembly.CreateInstance("Lab6.Program");
            MethodInfo info = obj.GetType().GetMethod("Main");
            var sw = new StringWriter();
            Console.SetOut(sw);
            Console.SetError(sw);
            info.Invoke(obj, null);
            string result = sw.ToString();
            textBox_output.Text += result;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time++;
            if (time >= 2)
            {
                try
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        Console.SetOut(stringWriter);
                        Compile();
                    }

                }
                catch
                {
                    timer.Stop();
                    time = 0;
                    textBox_output.Clear();                 
                    textBox_output.Text = strb.ToString();
                    
                    strb.Clear();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fastColoredTextBox1.Text =
@"public void Main()
{
//Используйте UI.WriteLine(string) для вывода
}";
            Params.ReferencedAssemblies.AddRange(dll);//Добавляем библиотеки к параметрам компилятора
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //e.ChangedRange.SetStyle(BlueStyle, @"class|void|struct|interface|delegate|abstract| as|base|bool|break|byte|case|catch|char|checked|const|continue|decimal|defaultdo|double|else|enum|event|false|finally|fixed|float|for |foreach|if|implicit|array|int|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|volatile|var|", RegexOptions.Multiline);

            textBox_output.Clear();
            timer.Stop();
            time = 0;
            timer.Start();
        }
    }
}
