namespace Lab6
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox_input = new System.Windows.Forms.TextBox();
            this.textBox_output = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox_input
            // 
            this.textBox_input.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_input.Location = new System.Drawing.Point(12, 12);
            this.textBox_input.Multiline = true;
            this.textBox_input.Name = "textBox_input";
            this.textBox_input.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_input.Size = new System.Drawing.Size(697, 419);
            this.textBox_input.TabIndex = 0;
            this.textBox_input.TextChanged += new System.EventHandler(this.textBox_input_TextChanged);
            // 
            // textBox_output
            // 
            this.textBox_output.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBox_output.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_output.ForeColor = System.Drawing.SystemColors.Window;
            this.textBox_output.Location = new System.Drawing.Point(12, 437);
            this.textBox_output.Multiline = true;
            this.textBox_output.Name = "textBox_output";
            this.textBox_output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_output.Size = new System.Drawing.Size(697, 153);
            this.textBox_output.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(12, 12);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(697, 211);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 602);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.textBox_output);
            this.Controls.Add(this.textBox_input);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_input;
        private System.Windows.Forms.TextBox textBox_output;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}

