using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MagicStorm.Opengl;
using MagicStorm.Game;

namespace MagicStorm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CGL.InitializeContexts();
            InputLanguage.CurrentInputLanguage
                = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var screen = Screen.AllScreens;
            Rectangle monitorSize = screen[0].WorkingArea;
            int w = monitorSize.Right - (this.Size.Width - this.ClientRectangle.Width); 
            int h = monitorSize.Bottom - (this.Size.Height - this.ClientRectangle.Height);
            MainController _mainController = new MainController(new 
                MagicStorm.Game.Game(), w, h);
            _mainController.SetMainLoop();
        }



    }
}
