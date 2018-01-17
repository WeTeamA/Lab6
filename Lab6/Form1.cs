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
        StringBuilder strb = new StringBuilder();
        public void Run()
        {
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, compil + richTextBox1.Text + "}}"); 

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    strb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
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
            if (sec >= 2)
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
interface OutPut 
{ 
void Out(string x); 
} 
public class WriteText:OutPut 
{ 
public  void Out(string x) 
{
Console.WriteLine(x); 
}
}";
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text =
            @"public void Main() 
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
            sec = 0;
            timer.Start();
            Regex reg = new Regex(@"string |WriteText|object |int |uint |sbyte |shor |ushor |long |ulong |float |double |char |bool |decimal |public|private|protected|void|static|delegate|enum|new|class\s[A-z,0-9]{1,10}|struct\s[A-z,0-9]{1,10}|interface\s[A-z,0-9]{1,10}|const");
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