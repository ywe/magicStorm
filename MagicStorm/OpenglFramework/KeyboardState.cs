using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MagicStorm.Opengl
{
    /// <summary>
    /// Учитывает все клавиши на клавиатуре, клик левой и правой кнопкой мыши, позицию указателя 
    /// </summary>
    class KeyboardState :IGetKeyboardState
    {
        List<byte> pressedKeys = new List<byte>();
        string enteredString = "";
        int[] _actionTime; // для всех кнопок время, в течение которого они нажаты
        
        string englishEquvalents = @"F<DULT:PBQRKVYJGHCNEA{WXIO}SM"">Zf,dult;pbqrkvyjghcnea[wxio]sm'.z";
        char engE = '~', enge='`';
        public KeyboardState()
        {
            _actionTime = new int[256];
            for (int i = 0; i < _actionTime.Length; i++) _actionTime[i] = 0;
        }

        /// <summary>
        /// Метод для контроллера. Класс должен выполнить некоторые действия, когда очередная итерация таймера завершена
        /// </summary>
        public void StepEnded()
        {
            MouseClick = false;
            MouseRightClick = false;
            for (int i = 0; i < _actionTime.Length; i++)
                if (_actionTime[i] > 0)
                    _actionTime[i]++;

            pressedKeys.Clear();
            enteredString = "";
        }

        //-------------------------------------------------------------------------
        public void KeyPress(byte key)
        {
            enteredString += Encoding.GetEncoding(1251).GetString(new byte[] { key });
            if (key == 168) key = (byte)engE;
            else if (key == 184) key = (byte)enge;
            else if (key >= 192)
            {
                char k = englishEquvalents[key - 192];
                key = (byte)k;
            }
            if (key >= (byte)'a' && key <= (byte)'z') key -= 32;
           

            if(_actionTime[key] == 0) _actionTime[key] = 1;

            pressedKeys.Add(key);
        }
        public void KeyUp(byte key)
        {
            if (key == 168) key = (byte)engE;
            else if (key == 184) key = (byte)enge;
            else if (key >= 192)
            {
                char k = englishEquvalents[key - 192];
                key = (byte)k;
            }
            if (key >= (byte)'a' && key <= (byte)'z') key -= 32;
            _actionTime[key] = 0;
        }
        public void RefreshKeys()
        {
            foreach (Key key in Config.Keys.Values)
            {
                //Key.K
               // Keyboard.
            }
        }
        //узнать время, в течение которого нажата кнопка действия клавиатуры
        public int GetActionTime(EKeyboardAction action) { return _actionTime[Config.Keys[action]]; }
        public List<byte> GetPressedKeys(){ return pressedKeys; }
        public string GetEnteredString() { return enteredString; }
        //Свойства мыши
        /// <summary>
        /// на экране, верхний угол это (0,0)
        /// </summary>
        public Point2 MousePosScreen { get; set; }
        /// <summary>
        /// На карте, с учетом сдвига камеры
        /// </summary>
        public Point2 MousePosMap { get; set; }
        public bool MouseClick { get; set; }
        public bool MouseRightClick { get; set; }
    }
}
