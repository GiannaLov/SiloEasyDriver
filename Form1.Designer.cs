namespace SiloEasyDriver
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            button2 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(34, 103);
            button1.Name = "button1";
            button1.Size = new Size(161, 33);
            button1.TabIndex = 0;
            button1.Text = "Install EasyDriver";
            button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(49, 35);
            label1.Name = "label1";
            label1.Size = new Size(300, 17);
            label1.TabIndex = 2;
            label1.Text = "A tool that simplifies SteamVR controller bindings.";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(25, 169);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(381, 97);
            textBox1.TabIndex = 3;
            textBox1.Text = "How to use:\r\n\r\n1.Click the \"Install EasyDriver\" button.\r\n2.Restart SteamVR and the software you are using.";
            // 
            // button2
            // 
            button2.Location = new Point(209, 103);
            button2.Name = "button2";
            button2.Size = new Size(190, 33);
            button2.TabIndex = 5;
            button2.Text = "Restore the official driver";
            button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(427, 287);
            Controls.Add(button2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button1);
            MaximizeBox = false;
            Name = "Form1";
            Text = "SiloEasyDriver v1.0";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private TextBox textBox1;
        private Button button2;
    }
}
