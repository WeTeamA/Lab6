﻿using System;
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
        /// <summary>
        /// Текст компилируемого кода
        /// </summary>
        string code;
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

        " + textBox_input.Text + "    }}";
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, code); //Получаем результат исполнения исходного кода при примененных параметрах
            
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
            string nspace = "...";

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time++;
            if (time >= 2)
            {
                try
                {
                    
                    // var sw = new StringWriter();
                    // TextWriter sww = Console.Out;
                    //  Console.SetOut(sw);
                    // Console.SetError(sw);
                    //Console.SetCursorPosition(0, 0);
                    // textBox_output.Text = Console.ReadLine();
                    //Console.WriteLine("You are travelling north at a speed of 10m/s");

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
            int cur = textBox_input.SelectionStart;
            textBox_output.Clear();
            timer.Stop();
            time = 0;
            timer.Start();
            Regex reg = new Regex(@"string|break|var|foreach|for|new|while|if |int |in |else|return|object|until|sbyte|shor|object|true|false|ushor|switch|case|null|long |ulong |float |double |char |bool |decimal |public|private|protected|void|static|delegate|enum|new|class\s[A-z,0-9]{1,10}|struct\s[A-z,0-9]{1,10}|interface\s[A-z,0-9]{1,10}|const");
            MatchCollection mycol = reg.Matches(textBox_input.Text);
            Regex rego = new Regex(@"^\S*$");
            MatchCollection mycolo = reg.Matches(textBox_input.Text);
            if (mycol.Count != 0)
            {
                foreach (Match m in mycol)
                {
                    textBox_input.SelectionStart = m.Index;
                    textBox_input.SelectionLength = m.Length;
                    textBox_input.SelectionColor = Color.Blue;
                }
                foreach (Match m in mycolo)
                {
                    textBox_input.SelectionStart = m.Index;
                    textBox_input.SelectionLength = m.Length;
                    textBox_input.SelectionColor = Color.Blue;
                }
                textBox_input.SelectionStart = cur;
                textBox_input.SelectionLength = 0;
                textBox_input.SelectionColor = Color.Black;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_input.Text =
@"public static void Main()
{
//Используйте UI.WriteLine(string) для вывода
}";
            compilerParams.ReferencedAssemblies.AddRange(dll);//Добавляем библиотеки к параметрам компилятора
        }
    }
}
