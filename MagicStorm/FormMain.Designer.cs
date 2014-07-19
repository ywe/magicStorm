namespace MagicStorm
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.cbPlayer1 = new System.Windows.Forms.CheckBox();
            this.edtPlayer1 = new System.Windows.Forms.TextBox();
            this.edtPlayer2 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPlayer1 = new System.Windows.Forms.Button();
            this.btnExchange = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMap = new System.Windows.Forms.ComboBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.cbPlayer2 = new System.Windows.Forms.CheckBox();
            this.btnPlayer2 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbHistory = new System.Windows.Forms.CheckBox();
            this.edtHistory = new System.Windows.Forms.TextBox();
            this.btnHistory = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(924, 438);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(191, 77);
            this.button1.TabIndex = 0;
            this.button1.Text = "Начать игру";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbPlayer1
            // 
            this.cbPlayer1.AutoSize = true;
            this.cbPlayer1.Location = new System.Drawing.Point(43, 93);
            this.cbPlayer1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbPlayer1.Name = "cbPlayer1";
            this.cbPlayer1.Size = new System.Drawing.Size(134, 33);
            this.cbPlayer1.TabIndex = 1;
            this.cbPlayer1.Text = "Человек";
            this.cbPlayer1.UseVisualStyleBackColor = true;
            this.cbPlayer1.CheckedChanged += new System.EventHandler(this.cbPlayer1_CheckedChanged);
            // 
            // edtPlayer1
            // 
            this.edtPlayer1.Location = new System.Drawing.Point(187, 91);
            this.edtPlayer1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtPlayer1.Name = "edtPlayer1";
            this.edtPlayer1.Size = new System.Drawing.Size(857, 35);
            this.edtPlayer1.TabIndex = 3;
            // 
            // edtPlayer2
            // 
            this.edtPlayer2.Location = new System.Drawing.Point(187, 214);
            this.edtPlayer2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtPlayer2.Name = "edtPlayer2";
            this.edtPlayer2.Size = new System.Drawing.Size(857, 35);
            this.edtPlayer2.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Исполняемые файлы|*.exe";
            // 
            // btnPlayer1
            // 
            this.btnPlayer1.Location = new System.Drawing.Point(1071, 90);
            this.btnPlayer1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnPlayer1.Name = "btnPlayer1";
            this.btnPlayer1.Size = new System.Drawing.Size(44, 36);
            this.btnPlayer1.TabIndex = 5;
            this.btnPlayer1.Text = "...";
            this.btnPlayer1.UseVisualStyleBackColor = true;
            this.btnPlayer1.Click += new System.EventHandler(this.btnPlayer1_Click);
            // 
            // btnExchange
            // 
            this.btnExchange.Image = global::MagicStorm.Properties.Resources.icon0;
            this.btnExchange.Location = new System.Drawing.Point(478, 143);
            this.btnExchange.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Size = new System.Drawing.Size(59, 49);
            this.btnExchange.TabIndex = 7;
            this.btnExchange.UseVisualStyleBackColor = true;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(1041, 320);
            this.lblTime.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(74, 29);
            this.lblTime.TabIndex = 8;
            this.lblTime.Text = "100%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 316);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(203, 29);
            this.label2.TabIndex = 9;
            this.label2.Text = "Выберите карту:";
            this.toolTip1.SetToolTip(this.label2, "Введите половину карты, используя цифры 1(огонь), 2(вода), 3(земля), 4(воздух) и " +
        "*(случайно)");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(234, 29);
            this.label3.TabIndex = 10;
            this.label3.Text = "Выберите игроков:";
            // 
            // cbMap
            // 
            this.cbMap.FormattingEnabled = true;
            this.cbMap.Location = new System.Drawing.Point(43, 365);
            this.cbMap.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbMap.Name = "cbMap";
            this.cbMap.Size = new System.Drawing.Size(260, 37);
            this.cbMap.TabIndex = 12;
            this.toolTip1.SetToolTip(this.cbMap, "Введите половину карты, используя цифры 1(огонь), 2(вода), 3(земля), 4(воздух) и " +
        "*(случайно)");
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(793, 361);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.trackBar1.Maximum = 30;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(322, 69);
            this.trackBar1.TabIndex = 13;
            this.trackBar1.Value = 10;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(788, 320);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 29);
            this.label4.TabIndex = 14;
            this.label4.Text = "Время анимации:";
            // 
            // cbPlayer2
            // 
            this.cbPlayer2.AutoSize = true;
            this.cbPlayer2.Location = new System.Drawing.Point(43, 216);
            this.cbPlayer2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbPlayer2.Name = "cbPlayer2";
            this.cbPlayer2.Size = new System.Drawing.Size(134, 33);
            this.cbPlayer2.TabIndex = 15;
            this.cbPlayer2.Text = "Человек";
            this.cbPlayer2.UseVisualStyleBackColor = true;
            this.cbPlayer2.CheckedChanged += new System.EventHandler(this.cbPlayer2_CheckedChanged);
            // 
            // btnPlayer2
            // 
            this.btnPlayer2.Location = new System.Drawing.Point(1071, 213);
            this.btnPlayer2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnPlayer2.Name = "btnPlayer2";
            this.btnPlayer2.Size = new System.Drawing.Size(44, 36);
            this.btnPlayer2.TabIndex = 16;
            this.btnPlayer2.Text = "...";
            this.btnPlayer2.UseVisualStyleBackColor = true;
            this.btnPlayer2.Click += new System.EventHandler(this.btnPlayer2_Click);
            // 
            // cbHistory
            // 
            this.cbHistory.AutoSize = true;
            this.cbHistory.Location = new System.Drawing.Point(417, 319);
            this.cbHistory.Name = "cbHistory";
            this.cbHistory.Size = new System.Drawing.Size(264, 33);
            this.cbHistory.TabIndex = 17;
            this.cbHistory.Text = "Сохранять историю";
            this.toolTip1.SetToolTip(this.cbHistory, "Файл будет содержать input и output для каждого хода");
            this.cbHistory.UseVisualStyleBackColor = true;
            this.cbHistory.CheckedChanged += new System.EventHandler(this.cbHistory_CheckedChanged);
            // 
            // edtHistory
            // 
            this.edtHistory.Enabled = false;
            this.edtHistory.Location = new System.Drawing.Point(417, 367);
            this.edtHistory.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtHistory.Name = "edtHistory";
            this.edtHistory.Size = new System.Drawing.Size(278, 35);
            this.edtHistory.TabIndex = 18;
            this.toolTip1.SetToolTip(this.edtHistory, "Файл будет содержать input и output для каждого хода");
            // 
            // btnHistory
            // 
            this.btnHistory.Location = new System.Drawing.Point(721, 366);
            this.btnHistory.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(41, 36);
            this.btnHistory.TabIndex = 19;
            this.btnHistory.Text = "...";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(329, 367);
            this.button2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(41, 36);
            this.button2.TabIndex = 20;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1144, 542);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.edtHistory);
            this.Controls.Add(this.cbHistory);
            this.Controls.Add(this.btnPlayer2);
            this.Controls.Add(this.cbPlayer2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.cbMap);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.btnExchange);
            this.Controls.Add(this.btnPlayer1);
            this.Controls.Add(this.edtPlayer2);
            this.Controls.Add(this.edtPlayer1);
            this.Controls.Add(this.cbPlayer1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "FormMain";
            this.Text = "Magic Storm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbPlayer1;
        private System.Windows.Forms.TextBox edtPlayer1;
        private System.Windows.Forms.TextBox edtPlayer2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnPlayer1;
        private System.Windows.Forms.Button btnExchange;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbMap;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbPlayer2;
        private System.Windows.Forms.Button btnPlayer2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cbHistory;
        private System.Windows.Forms.TextBox edtHistory;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button button2;
    }
}