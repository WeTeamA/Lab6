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
        int sec;
        string[] dll = new string[]
        {"System.dll",
"System.Windows.Forms.dll",
"System.Linq.dll",
"mscorlib.dll",
"System.Threading.Tasks.dll",
"System.Data.dll"
        };

        static Dictionary<string, string> providerOptions = new Dictionary<string, string>
{
{"CompilerVersion", "v4.0"}
};

        CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

        CompilerParameters compilerParams = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };
        public int GetLine(string text)
        {
            Regex reg = new Regex("\r\n");
            MatchCollection lines = reg.Matches(text);
            return lines.Count+1;
        }
        StringBuilder strb = new StringBuilder();
        public void Run()
        {
            string text = compil + getStr(richTextBox1.Text)[1] + "public void Main(){\r\n" + getStr(richTextBox1.Text)[0] + "\r\n}}}";
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, text);
            int lineText = GetLine(text);
            string[] masspull = text.Split('\n');
            for (int i=0; i<masspull.Count();i++)
            {
                masspull[i] = masspull[i].Replace("\r", "");
            }
            string[] mass = richTextBox1.Text.Split('\n');
            for (int i = 0; i < mass.Count(); i++)
            {
                mass[i] = mass[i].Replace("\r", "");
               // mass[i] = mass[i].Replace("\"", "");
            }
            if (results.Errors.HasErrors)
            {
                int line=0;
                foreach (CompilerError error in results.Errors)
                {
                        for (int k=0;k<mass.Count();k++)
                        {
                            if (masspull[error.Line-1] == mass[k])
                            {
                                line = k+1;
                                strb.AppendLine(String.Format("Error ({0}): {1}. Ошибка в строке {2}", error.ErrorNumber, error.ErrorText, line));
                            }
                        }
                    
                }
                throw new InvalidOperationException(strb.ToString());
            }

            Assembly assembly = results.CompiledAssembly;
            Type program = assembly.GetType("Lab6.Program");
            MethodInfo method = program.GetMethod("Main");

            timer.Stop();
            sec = 0;

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
            sec++;
            if (sec >= 4)
            {
                try
                {
                    using (StringWriter stringW = new StringWriter())
                    {
                        Console.SetOut(stringW);
                        Run();

                    }

                }
                catch
                {
                    timer.Stop();
                    sec = 0;
                    textBox_output.Clear();
                    textBox_output.Text = strb.ToString();
                    strb.Clear();
                }
            }
        
        }
        public string[] getStr(string block)
        {
            string[] result = new string[2];
            string p = block;

            Regex outside = new Regex("class|void|struct|interface|delegate");
            MatchCollection mycollection = outside.Matches(block);
           
            while (mycollection.Count != 0)
            {
                int check = 0;
                int k = 0;
                for (int i = mycollection[0].Index; i <= block.Length - 1; i++)
                {
                    switch (p[i])
                    {
                        case '{':
                            check++;
                            break;
                        case '}':
                            check--;
                            k ++;
                            break;
                    }
                    if ((check==0)&&(k>0))
                    {
                        result[1] += p.Substring(mycollection[0].Index, i- mycollection[0].Index+1);
                        p=p.Replace(p.Substring(mycollection[0].Index, i - mycollection[0].Index + 1), "");
                        mycollection = outside.Matches(p);
                        break;
                    }
                }
            }
            result[0] = p;
            return result;
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
namespace Lab6 
{ 
    public class Program 
    {  
        public static class UI
        { 
            public static string WriteLine(string x) 
                {
                Console.WriteLine(x); 
                return x;
                }
        }

";
        private void Form1_Load(object sender, EventArgs e)
        {
            compilerParams.ReferencedAssemblies.AddRange(dll);
        }
        /* private void richTextBox1_TextChanged(object sender, EventArgs e)
         {
            // int cur = richTextBox1.SelectionStart;
             textBox_output.Clear();
             timer.Stop();
             sec = 0;
             timer.Start();
            /* Regex reg = new Regex(@"string |WriteText|object |int |uint |sbyte |shor |ushor |long |ulong |float |double |char |bool |decimal |public|private|protected|void|static|delegate|enum|new|class\s[A-z,0-9]{1,10}|struct\s[A-z,0-9]{1,10}|interface\s[A-z,0-9]{1,10}|const");
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
         }*/
        Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.ChangedRange.SetStyle(BlueStyle, @"class|void|struct|interface|delegate|abstract|	as|base|bool|break|byte|case|catch|char|checked|const|continue|decimal|defaultdo|double|else|enum|event|false|finally|fixed|float|for |foreach|if|implicit|array|int|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|volatile|var|", RegexOptions.Multiline);
            textBox_output.Clear();
            timer.Stop();
            sec = 0;
            timer.Start();

        }
    }
}