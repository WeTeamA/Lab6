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
        /// Код, который подается на компиляцию
        /// </summary>
        string CodeForCompile;
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
        /// <summary>
        /// Переменная для записи ошибок компиляции
        /// </summary>
        StringBuilder err = new StringBuilder();

        public void Compile()
        {
            CodeForCompile = @"using System;
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
namespace Lab6 
   {  
      public class Program
      {
" + textBox_input.Text + @"
      }
   }";
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, CodeForCompile); //Получаем результат исполнения исходного кода при примененных параметрах
            
            #region Отлов ошибок
            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    err.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }
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

        //Метод, подсвечивающий ключевые слова
        public void CheckSyntax()
        {
            Regex KeyWords = new Regex("abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|var");
            string str = DateTime.Now.ToLongTimeString();
            richTextBox.AppendText(str + "\r\n" + "SOmeText" + "\r\n");
            int i = 0;
            while (i <= richTextBox.Text.Length - str.Length)
            {
                i = richTextBox.Text.IndexOf(str, i);
                if (i < 0) break;
                richTextBox.SelectionStart = i;
                richTextBox.SelectionLength = str.Length;
                richTextBox.SelectionColor = Color.Blue;
                i += str.Length;
            }
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
                        Console.SetOut(stringWriter); //Связываем консоль с объектом stringWriter
                        Compile();
                        CheckSyntax();
                        textBox_output.Text = stringWriter.ToString();
                    }
                }
                catch
                {
                    timer.Stop();
                    time = 0;
                    textBox_output.Clear();                 
                    textBox_output.Text = err.ToString();
                    err.Clear();
                }
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
@"public static void Main()
    {

    }
";
           compilerParams.ReferencedAssemblies.AddRange(dll);//Добавляем библиотеки к параметрам компилятора
        }
    }
}
