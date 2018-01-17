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
        string[] dll = new string[]
        {"System.dll",
"System.Linq.dll",
"System.Threading.Tasks.dll",
"System.Windows.Forms.dll",
"mscorlib.dll",
"System.Data.dll"
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
        StringBuilder sb = new StringBuilder();
        public void Compile()
        {
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, compil + richTextBox1.Text + "}}"); //Получаем результат исполнения исходного кода при примененных параметрах 

            #region Отлов ошибок 
            if (results.Errors.HasErrors)
            {
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

            timer.Stop();
            time = 0;

            main.Invoke(null, null); //Запускаем метод main 
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time++;
            if (time >= 3)
            {
                try
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        Console.SetOut(stringWriter);
                        Compile();
                        string consoleOutput = stringWriter.ToString();
                        textBox_output.Text = consoleOutput;
                    }

                }
                catch
                {
                    timer.Stop();
                    time = 0;
                    textBox_output.Clear();
                    textBox_output.Text = sb.ToString();
                    sb.Clear();
                }
            }
        }

        private void textBox_input_TextChanged(object sender, EventArgs e)
        {


        }
        string compil = @"using System; 
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
interface OutPut 
{ 
void Out(string x); 
} 
public class WriteText:OutPut 
{ 
public void Out(string x) 
{ 
Console.WriteLine(x); 
} 
}";
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text =
            @"public static void Main() 
{ 
WriteText s = new WriteText(); // s.Out(string) выведет строку в TextBox 

}";
            compilerParams.ReferencedAssemblies.AddRange(dll); 
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int cur = richTextBox1.SelectionStart;
            textBox_output.Clear();
            timer.Stop();
            time = 0;
            timer.Start();
            Regex reg = new Regex(@"string |object |int |uint |sbyte |shor |ushor |long |ulong |float |double |char |bool |decimal |public|private|protected|void|static|delegate|enum|new|class\s[A-z,0-9]{1,10}|struct\s[A-z,0-9]{1,10}|interface\s[A-z,0-9]{1,10}|const");
            MatchCollection mycol = reg.Matches(richTextBox1.Text);
            if (mycol.Count != 0)
            {
                foreach (Match m in mycol)
                {
                    richTextBox1.SelectionStart = m.Index;
                    richTextBox1.SelectionLength = m.Length;
                    richTextBox1.SelectionColor = Color.Blue;
                }
                richTextBox1.SelectionStart = cur;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = Color.Black;
            }
        }
    }
}