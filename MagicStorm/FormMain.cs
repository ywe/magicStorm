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
using System.IO;

namespace MagicStorm
{
    public partial class FormMain : Form
    {
        Thread game;
        public FormMain()
        {
            InitializeComponent();
        }

        #region запуск игры по кнопке "начать"
        private void button1_Click(object sender, EventArgs e)
        {
            if (game != null && game.IsAlive)
            {
                MessageBox.Show("Запрещено запускать 2 игры из одной формы!");
                return;
            }

            try
            {
                File.Create(edtHistory.Text+"//log.txt");
            }
            catch
            {
                //MessageBox.Show("Не удалось создать файл истории");
                //return;
            }

            //проверка и генерация карты
            int[] map;
            string message = GenerateMap(out map);
            if (message != "")
            {
                MessageBox.Show(message);
                return;
            }

            //запуск в другом потоке
            ParamsFromFormToGame p = new ParamsFromFormToGame()
            {
                 firstAddress = edtPlayer1.Text,
                 secondAddress = edtPlayer2.Text,
                 logfile = edtHistory + "//log.txt",
                 map = map
            };
            game = new Thread(
                new ThreadStart(() => NewGame(p)));
            game.SetApartmentState(ApartmentState.STA);
            game.Start(); 
        }

        

        void NewGame(ParamsFromFormToGame p)
        {
            Application.Run(new Form1(p));
        }
        #endregion

        #region обработка событий контролов
        private void cbPlayer1_CheckedChanged(object sender, EventArgs e)
        {
            edtPlayer1.Enabled = !cbPlayer1.Checked;
        }

        private void cbPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            edtPlayer2.Enabled = !cbPlayer2.Checked;
        }

        private void btnExchange_Click(object sender, EventArgs e)
        {
            string t = edtPlayer1.Text; edtPlayer1.Text = edtPlayer2.Text; edtPlayer2.Text = t;
            bool check = cbPlayer1.Checked; cbPlayer1.Checked = cbPlayer2.Checked; cbPlayer2.Checked = check;
        }

        private void btnPlayer1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                cbPlayer1.Checked = false;
                edtPlayer1.Text = openFileDialog1.FileName;
            }
        }

        private void btnPlayer2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                cbPlayer2.Checked = false;
                edtPlayer2.Text = openFileDialog1.FileName;
            }
        }

        private void cbHistory_CheckedChanged(object sender, EventArgs e)
        {
            edtHistory.Enabled = cbHistory.Checked;
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                cbHistory.Checked = true;
                edtHistory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            lblTime.Text = (trackBar1.Value * 10).ToString() + "%";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cbMap.Items.Add(cbMap.Text);
        }
#endregion

        #region генерация карты
        string GenerateMap(out int[] map)
        {
            Random rand = new Random();
            map = new int[cbMap.Text.Length * 2];
            char[] s = cbMap.Text.ToArray();
            bool[] r = new bool[] { false, false, false, false };
            List<int> stars = new List<int>();
            int i = 0;
            foreach (char c in s)
            {
                if (c >= '1' && c <= '4')
                {
                    map[i] = int.Parse(c + "");
                    r[map[i]-1] = true;
                }
                else if (c == '*')
                {
                    stars.Add(i);
                }
                else
                {
                    return "Карта: неизвестный символ (" + c + ")";
                }
                i++;
            }

            //заполним сначала так, чтобы появился хоть 1 каждого цвета
            for (int j = 0; j < r.Length; j++)
            {
                if (r[j] == false)
                {
                    if (stars.Count == 0)
                    {
                        return "Карта должна содержать камни всех стихий";
                    }
                    int ind = rand.Next(stars.Count);
                    map[stars[ind]] = j + 1;
                    stars.RemoveAt(ind);
                }
            }
            //затем остальное
            foreach (int a in stars)
            {
                map[a] = rand.Next(4) + 1;
            }

            for (int j = map.Length / 2; j < map.Length; j++)
                map[j] = map[map.Length - j - 1];

            return "";
        }
        #endregion

        #region загрузка и сохранение контролов в файл
        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader reader = new StreamReader(Application.StartupPath + "//config.cfg"))
                {
                    string[] ss = reader.ReadLine().Split(' ');
                    cbPlayer1.Checked = bool.Parse(ss[0]);
                    cbPlayer2.Checked = bool.Parse(ss[1]);
                    cbHistory.Checked = bool.Parse(ss[2]);
                    trackBar1.Value = int.Parse(ss[3]);

                    edtPlayer1.Text = reader.ReadLine();
                    edtPlayer2.Text = reader.ReadLine();
                    edtHistory.Text = reader.ReadLine();

                    openFileDialog1.FileName = reader.ReadLine();
                    folderBrowserDialog1.SelectedPath = reader.ReadLine();

                    cbMap.Text = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        cbMap.Items.Add(reader.ReadLine());
                    }
                }
            }
            catch
            {

            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Application.StartupPath + "//config.cfg"))
                {
                    writer.WriteLine(
                        cbPlayer1.Checked.ToString() + " " +
                        cbPlayer2.Checked.ToString() + " " +
                        cbHistory.Checked.ToString() + " " +
                        trackBar1.Value.ToString());
                    writer.WriteLine(edtPlayer1.Text);
                    writer.WriteLine(edtPlayer2.Text);
                    writer.WriteLine(edtHistory.Text);

                    writer.WriteLine(openFileDialog1.FileName);
                    writer.WriteLine(folderBrowserDialog1.SelectedPath);

                    writer.WriteLine(cbMap.Text);
                    foreach (var a in cbMap.Items)
                        writer.WriteLine((string)a);
                }

            }
            catch
            {

            }
        }
        #endregion

    }
}
