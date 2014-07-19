using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MagicStorm.Game.DataClasses;
using System.Threading;

namespace MagicStorm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread game = new Thread(
                new ThreadStart(() => NewGame(new ParamsFromFormToGame())));
            game.SetApartmentState(ApartmentState.STA);
            game.Start();
        }

        void NewGame(ParamsFromFormToGame p)
        {
            Application.Run( new Form1(p));
        }
    }
}
