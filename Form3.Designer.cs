namespace WinFormsApp3
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            button1 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            button2 = new Button();
            button3 = new Button();
            listView1 = new ListView();
            label4 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Enabled = false;
            label1.FlatStyle = FlatStyle.Flat;
            label1.ForeColor = Color.Black;
            label1.Location = new Point(92, 50);
            label1.Name = "label1";
            label1.Size = new Size(71, 20);
            label1.TabIndex = 0;
            label1.Text = "firstname";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(92, 86);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 1;
            label2.Text = "lastname";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Location = new Point(92, 134);
            label3.Name = "label3";
            label3.Size = new Size(102, 20);
            label3.TabIndex = 2;
            label3.Text = "phonenumber";
            // 
            // button1
            // 
            button1.Location = new Point(220, 261);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 3;
            button1.Text = "edit";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(207, 50);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(207, 83);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(125, 27);
            textBox2.TabIndex = 6;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(207, 127);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(125, 27);
            textBox3.TabIndex = 7;
            // 
            // button2
            // 
            button2.Image = (Image)resources.GetObject("button2.Image");
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.Location = new Point(-2, 1);
            button2.Name = "button2";
            button2.Size = new Size(88, 49);
            button2.TabIndex = 15;
            button2.Text = "Home";
            button2.TextAlign = ContentAlignment.MiddleRight;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(462, 261);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 17;
            button3.Text = "Delete";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // listView1
            // 
            listView1.Location = new Point(427, 50);
            listView1.Name = "listView1";
            listView1.Size = new Size(581, 167);
            listView1.TabIndex = 18;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(12, 214);
            label4.Name = "label4";
            label4.Size = new Size(57, 20);
            label4.TabIndex = 19;
            label4.Text = "Gender";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.BackColor = Color.Transparent;
            radioButton1.Location = new Point(140, 210);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(63, 24);
            radioButton1.TabIndex = 20;
            radioButton1.TabStop = true;
            radioButton1.Text = "male";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.BackColor = Color.Transparent;
            radioButton2.Location = new Point(295, 210);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(76, 24);
            radioButton2.TabIndex = 21;
            radioButton2.TabStop = true;
            radioButton2.Text = "female";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1146, 450);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(label4);
            Controls.Add(listView1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form3";
            Text = "Form3";
            Load += Form3_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Button button1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private PictureBox pictureBox1;
        private TableLayoutPanel tableLayoutPanel1;
        private Button button2;
        private Button button3;
        private ListView listView1;
        private Label label4;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
    }
}