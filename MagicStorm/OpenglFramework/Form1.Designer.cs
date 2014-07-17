namespace MagicStorm
{
    partial class Form1
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
            this.CGL = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.SuspendLayout();
            // 
            // CGL
            // 
            this.CGL.AccumBits = ((byte)(0));
            this.CGL.AutoCheckErrors = false;
            this.CGL.AutoFinish = false;
            this.CGL.AutoMakeCurrent = true;
            this.CGL.AutoSwapBuffers = true;
            this.CGL.BackColor = System.Drawing.Color.Black;
            this.CGL.ColorBits = ((byte)(32));
            this.CGL.DepthBits = ((byte)(16));
            this.CGL.Location = new System.Drawing.Point(188, 179);
            this.CGL.Name = "CGL";
            this.CGL.Size = new System.Drawing.Size(50, 50);
            this.CGL.StencilBits = ((byte)(0));
            this.CGL.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 305);
            this.Controls.Add(this.CGL);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl CGL;
    }
}

