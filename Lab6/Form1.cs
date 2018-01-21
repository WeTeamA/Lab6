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
        Style ErrorCodeStyle = new TextStyle(Brushes.Black, Brushes.LightPink, FontStyle.Regular);
        /// <summary>
        /// Стиль ошибок текста
        /// </summary>
        Style CodeStyle = new TextStyle(Brushes.Black, Brushes.Honeydew, FontStyle.Regular);
        /// <summary>
        /// Объявление классов, методов, делегатов, интерфейсов и структур
        /// </summary>
        string BeforeCode = @"using System; 
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

        ";
        /// <summary>
        /// Код внутри метода Main
        /// </summary>
        string MainCode = "";
        /// <summary>
        /// Код внутри метода Main
        /// </summary>
        string ArterCode;
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
            CorrectCompile();
            code = BeforeCode + @"public void Main()
            {" + "\n" + MainCode + @"}" + "\n" +  ArterCode + @"}}";
            CompilerResults results = CSharpProvider.CompileAssemblyFromSource(Params, code);
            fastColoredTextBox1.ChangedLineColor = Color.Honeydew;
            //Place beg = new Place(1,1);
            // Place end = new Place(1, );
            //Range tbrng = new Range(fastColoredTextBox1, Place );
            // tbrng.SetStyle(CodeStyle);

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    int Line = error.Line - 32;
                    strb.AppendLine(String.Format("Error ({0}): {1} (line {2})", error.ErrorNumber, error.ErrorText, Line));                    
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

        public int FindIndex(MatchCollection mycol, MatchCollection mycolb, MatchCollection mycole, int i)
        {
            int bindex = 0;
            int eindex = 0;
            while (mycol[i].Index > mycolb[bindex].Index)
            {
                bindex++;
            }
            while (mycol[i].Index > mycole[eindex].Index)
            {
                eindex++;
            }
            int j = bindex + 1;
            if (bindex < mycolb.Count)
            {
                while (mycolb[j].Index > mycolb[eindex].Index)
                {
                    eindex++;
                    if (j < mycolb.Count - 1)
                    {
                        j++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return eindex;
        }

        public void CorrectCompile()
        {
            MainCode = fastColoredTextBox1.Text;
            Regex reg = new Regex(@"\w+\s+\w+[(].*[)]|struct|class|interface|delegate");
            MatchCollection mycol = reg.Matches(fastColoredTextBox1.Text);
            Regex regb = new Regex(@"[{]");
            MatchCollection mycolb = regb.Matches(fastColoredTextBox1.Text);
            Regex rege = new Regex(@"[}]");
            MatchCollection mycole = rege.Matches(fastColoredTextBox1.Text);
            List<int> index = new List<int>();
            int i = 0;
            while(i < mycol.Count )
            {
                if (i != 0)
                {
                    if (index[i - 1] < mycol[i].Index)
                    {
                        index.Add(mycole[FindIndex(mycol, mycolb, mycole, i)].Index);
                        ArterCode += "\r\b" + fastColoredTextBox1.Text.Substring(mycol[i].Index, index[i] - mycol[i].Index + 1);
                        MainCode = MainCode.Remove(mycol[i].Index, index[i] - mycol[i].Index + 1);
                        i++;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (index.Count == 0)
                {
                    index.Add(mycole[FindIndex(mycol, mycolb, mycole, i)].Index);
                    ArterCode += fastColoredTextBox1.Text.Substring(mycol[i].Index , index[i] - mycol[i].Index + 1 ) + "\n";
                    MainCode = MainCode.Remove(mycol[i].Index, index[i] - mycol[i].Index + 1);
                    i++;
                }
            }


            int a = 5;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time++;
            if (time >= 1)
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
            Params.ReferencedAssemblies.AddRange(dll);//Добавляем библиотеки к параметрам компилятора
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_output.Clear();
            timer.Stop();
            time = 0;
            timer.Start();
            //We.ChangedRange
        }
    }
}
