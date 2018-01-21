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
using ICSharpCode.TextEditor;
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
        /// Код, который подается на компиляцию
        /// </summary>
        string CodeForCompile;
        string Code = "";
        string Others;
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
            GenerateExecutable = false,
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
        public void Main()
         {
" + Code + @"
         }
" + Others + @"
      }
   }";
            RichTextBox Box = new RichTextBox();
            Box.Text = CodeForCompile;
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, Box.Text); //Получаем результат исполнения исходного кода при примененных параметрах

            #region Отлов ошибок (Тут надо исправить, чтобы линия с ошибкой определялась однозначно)
            if (results.Errors.HasErrors)
            {
                int count = 0;
                foreach (CompilerError error in results.Errors)
                {
                    err.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    for (int i = 0; i < richTextBox.Lines.Count(); i++)
                    {
                        if (Box.Lines[error.Line-1] == richTextBox.Lines[i])
                        { 
                            count = i;
                            break;
                        }
                    }
                    count++;
                    err.AppendLine("Ошибка в строке/символе: " + count.ToString() + "/" + error.Column.ToString());
                }
                throw new Exception();
            }
            #endregion

            //Используем рефлексию для манипуляциями полученных классов и методов
            object ObjectOfProgram = results.CompiledAssembly.CreateInstance("Lab6.Program");
            MethodInfo main = ObjectOfProgram.GetType().GetMethod("Main");

            timer.Stop();
            time = 0;

            main.Invoke(ObjectOfProgram, null); //Запускаем метод main 
        }

        /// <summary>
        /// ВО ВРЕМЯ ПЕЧАТИ КОДА подсвечивает его синтаксис
        /// </summary>
        public void CheckSyntax()
        {
            int Cursor = richTextBox.SelectionStart;

            richTextBox.SelectionStart = 0;
            richTextBox.SelectionLength = richTextBox.Text.Length;
            richTextBox.SelectionColor = Color.White;

            Regex KeyWords = new Regex("abstract |as |base |bool |break |byte |case |catch |char |checked |class |const |continue |decimal |default |delegate |do |double |else |enum |event |explicit |extern |false |finally |fixed |float |for |foreach |goto |if |implicit |in |int |interface |internal |is |lock |long |namespace |new |null |object |operator |out |override |params |private |protected |public |readonly |ref |return |sbyte |sealed |short |sizeof |stackalloc |static |string |struct |switch |this |throw |true |try |typeof |uint |ulong |unchecked |unsafe |ushort |using |virtual |void |volatile |var ");
            MatchCollection Matches = KeyWords.Matches(richTextBox.Text);

            if (Matches.Count != 0)
            {
                foreach (Match Match in Matches)
                {
                    richTextBox.SelectionStart = Match.Index;
                    richTextBox.SelectionLength = Match.Length;
                    richTextBox.SelectionColor = Color.LightGreen;
                }
            }
            richTextBox.SelectionStart = Cursor;
            richTextBox.SelectionLength = 0;
        }

        /// <summary>
        /// ПЕРЕД КОМПИЛЯЦИЕЙ перебрасывает все классы и т.д. за метод Main
        /// </summary>
        public void CheckCodeStruct()
        {
            if (CheckStruct() == true)
            {
                int Cursor = richTextBox.SelectionStart;
                Code = richTextBox.Text;
                Regex OthersElements = new Regex("class |delegate |enum |interface |struct |void ");
                while (true)
                {
                    MatchCollection Matches = OthersElements.Matches(Code);
                    if (Matches.Count != 0)
                    {
                        int k = 0; //Счетчик кавычек
                        char[] OthersChar = new char[Code.Length]; //Временный массив Char для правильного аргументирования
                        for (int i = Matches[0].Index + Matches[0].Length; i < Code.Length; i++)
                        {
                            if (Code[i] == '{')
                                k++;
                            if (Code[i] == '}')
                            {
                                k--;
                                if (k == 0)
                                {
                                    Code.CopyTo(Matches[0].Index, OthersChar, 0, i - Matches[0].Index + 1);
                                    Code = Code.Remove(Matches[0].Index, i - Matches[0].Index + 1);
                                    OthersChar = OthersChar.Where(x => x != 0).ToArray();
                                    Others += "\r\n" + new string(OthersChar);
                                    break;
                                }
                            }
                        }

                    }
                    else break;
                }
                richTextBox.SelectionStart = Cursor;
                richTextBox.SelectionLength = 0;
            }
            else err.AppendLine("Где-то пропущена скобка '}' или '{'");
        }

        /// <summary>
        /// ПОСЛЕ КОМПИЛЯЦИИ подсвечивает строку с ошибкой, если она есть
        /// </summary>
        public bool CheckStruct()
        {
            int k = 0;
                for (int i = 0; i < richTextBox.Text.Length; i++)
                {
                    if (richTextBox.Text[i] == '{')
                        k++;
                    if (richTextBox.Text[i] == '}')
                    {
                        k--;
                    }
                }
            if (k == 0)
                return true;
            else
                return false;
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
                        err.Clear();
                        Console.SetOut(stringWriter); //Связываем консоль с объектом stringWriter
                        CheckCodeStruct();
                        if (err.Length == 0)
                            Compile();
                        else
                            throw new Exception();
                        Others = "";
                        textBox_output.Text = stringWriter.ToString();
                    }
                }
                 catch
                {
                    timer.Stop();
                    time = 0;
                    textBox_output.Clear();
                    textBox_output.Text = err.ToString();
                    Others = "";
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           compilerParams.ReferencedAssemblies.AddRange(dll);//Добавляем библиотеки к параметрам компилятора
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            timer.Stop();
            time = 0;
            timer.Start();
        }
    }
}
