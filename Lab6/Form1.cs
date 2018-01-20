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
            err.Clear();
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
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, CodeForCompile); //Получаем результат исполнения исходного кода при примененных параметрах
            
            #region Отлов ошибок
            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    err.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    throw new Exception();
                }
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
            int Cursor = richTextBox.SelectionStart;

            Regex OthersElements = new Regex("class |delegate |enum |interface |struct |void ");
            MatchCollection Matches = OthersElements.Matches(richTextBox.Text);

            if (Matches.Count != 0)
            {
                int lenBefore;
                int lenAfter;
                int len = 0;
                Code = richTextBox.Text;
                foreach (Match Match in Matches)
                {
                    richTextBox.SelectionStart = Match.Index; //Начальное положение курсора перед ключевым словом
                    richTextBox.SelectionLength = Match.Length; //Длина ключевого слова
                    int k = 0; //Счетчик кавычек
                    char[] OthersChar = new char[richTextBox.Text.Length]; //Временный массив Char для правильного аргументирования
                    for (int i = Match.Index + Match.Length; i < richTextBox.Text.Length; i++)
                    {
                        if (richTextBox.Text[i] == '{')
                            k++;
                        if (richTextBox.Text[i] == '}')
                        { 
                            k--;
                            if (k == 0)
                            {
                                Code.CopyTo(Match.Index - len, OthersChar, 0, i - Match.Index + 1);
                                lenBefore = Code.Length;
                                Code = Code.Remove(Match.Index - len, i - Match.Index + 1);
                                lenAfter = Code.Length;
                                len = lenBefore - lenAfter;
                                OthersChar = OthersChar.Where(x => x != 0).ToArray();
                                Others += "\r\n" + new string(OthersChar);
                                break;
                            }
                        }
                    }
                }
            }
            richTextBox.SelectionStart = Cursor;
            richTextBox.SelectionLength = 0;
        }

        /// <summary>
        /// ПОСЛЕ КОМПИЛЯЦИИ подсвечивает строку с ошибкой, если она есть
        /// </summary>
        public void CheckErrors()
        {

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
                        CheckCodeStruct();
                        Compile();
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

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckSyntax();
            timer.Stop();
            time = 0;
            timer.Start();
        }
    }
}
