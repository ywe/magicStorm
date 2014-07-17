using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Drawing;
using System.Diagnostics;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.DevIl;
using System.Windows.Input;

namespace MagicStorm.Opengl
{
    class MainController 
    {

        int _windowCode;
        Dictionary<string, int> _textureCodes;

        int _windowWidth, _windowHeight; //пригодится, чтобы пересчитывать координаты мышки в игровые

        KeyboardState _keyboardState;
        IGame _game;
        public MainController(IGame game, int windowWidth, int windowHeight, bool tryFullScreen = false)
        {
            _keyboardState = new KeyboardState();
            _game = game;
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;

            //инициализация openGL
            _windowCode = OpenglInitializer.CreateWindow(windowWidth, windowHeight, tryFullScreen);
            OpenglInitializer.SetDisplayModes();
            _textureCodes =  OpenglInitializer.LoadTextures();
        }


        public void SetMainLoop()
        {
            //отлов действий пользователя
            Glut.glutMotionFunc(ClickedMotion);
            Glut.glutMouseFunc(Mouse);
            Glut.glutPassiveMotionFunc(PassiveMotion);
            Glut.glutKeyboardFunc(KeyPress);
            Glut.glutKeyboardUpFunc(KeyUp);
            Glut.glutSpecialFunc(KeySpecial);
            Glut.glutSpecialUpFunc(KeySpecialUp);
            
            //старт игрового цикла
            Glut.glutTimerFunc(Config.TimePerFrame, MainProcess, 0);
            Glut.glutMainLoop();
        }


        
        Frame _curFrame = new Frame();
        void MainProcess(int value)
        {
            //сразу засекаем следующие миллисекунды. Если рисование затянется, оно вызовется, как только дорисуем прежнее.
            Glut.glutTimerFunc(Config.TimePerFrame, MainProcess, 0);

            VerySpecialKeys(); //читаем альт шифт контрол
            _curFrame = _game.Process(_keyboardState);
            _keyboardState.StepEnded(); //игра считала кнопки, время классу сделать плановые действия
            
            //рисуем, если есть что рисовать
            if (_curFrame == null) CloseWindow();
            else Painter.DrawFrame((Frame)_curFrame, _textureCodes);

        }

        //------------------------------------------------------------------------------------
        //Дальше несущественный код
        //-------------------------------------

        
        public void PassiveMotion(int x, int y)
        {

            _keyboardState.MousePosScreen = new Point2(Config.ScreenWidth * ((double)x / _windowWidth), Config.ScreenHeight * ((double)y / _windowHeight));
            
            _keyboardState.MousePosMap = new Point2(
                _keyboardState.MousePosScreen.x + _curFrame.camera.x, 
                _keyboardState.MousePosScreen.y + _curFrame.camera.y );

        }

        public void Mouse(int button, int state, int x, int y)
        {
            if (state == Glut.GLUT_DOWN )
            {
                _keyboardState.MouseClick = button == Glut.GLUT_LEFT_BUTTON;
                _keyboardState.MouseRightClick = button == Glut.GLUT_RIGHT_BUTTON;
            }
        }

        public void ClickedMotion(int x, int y)
        {
            PassiveMotion(x, y); // все равно одинаковые действие
        }

        public void KeyUp(byte key, int x, int y)
        {
            //не знаю, зачем опенгл при нажатом ctrl меняет коды
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0 ||
                (Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) > 0)
            {
                if (key >= 1 && key <= 26) key += 96;
            }
            _keyboardState.KeyUp(key);
        }

        public void KeyPress(byte key, int x, int y)
        {
            //не знаю, зачем опенгл при нажатом ctrl меняет коды
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) >0||
                (Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down)>0)
            {
                if (key >= 1 && key <= 26) key +=96;
            }
            _keyboardState.KeyPress(key);
        }

        public void KeySpecialUp(int key, int x, int y)
        {
            if (key >= 0 && key <= 11) _keyboardState.KeyUp((byte)(key + 131)); //f1-f12
            if (key >= 100 && key <= 103) _keyboardState.KeyUp((byte)(key -63)); //arrows
        }

        public void KeySpecial(int key, int x, int y)
        {
            if (key >= 0 && key <= 11) _keyboardState.KeyPress((byte)(key + 131)); //f1-f12
            if (key >= 100 && key <= 103) _keyboardState.KeyPress((byte)(key - 63)); //arrows
        }

        bool lshift, rshift, lctrl, rctrl, lalt, ralt;
        public void VerySpecialKeys()
        {
            #region ручная проверка, слишком важные клавиши, не мог обойти
            if ((Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > 0 )
            {
                if (!lshift)
                {
                    lshift = true;
                    _keyboardState.KeyPress(160);
                }
            }
            else
            {
                if (lshift)
                {
                    lshift = false;
                    _keyboardState.KeyUp(160);
                }
            }

            if ((Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) > 0)
            {
                if (!rshift)
                {
                    rshift = true;
                    _keyboardState.KeyPress(161);
                }
            }
            else
            {
                if (rshift)
                {
                    rshift = false;
                    _keyboardState.KeyUp(161);
                }
            }

            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) > 0)
            {
                if (!lctrl)
                {
                    lctrl = true;
                    _keyboardState.KeyPress(162);
                }
            }
            else
            {
                if (lctrl)
                {
                    lctrl = false;
                    _keyboardState.KeyUp(162);
                }
            }

            if ((Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) > 0)
            {
                if (!rctrl)
                {
                    rctrl = true;
                    _keyboardState.KeyPress(163);
                }
            }
            else
            {
                if (rctrl)
                {
                    rctrl = false;
                    _keyboardState.KeyUp(163);
                }
            }

            if ((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) > 0)
            {
                if (!lalt)
                {
                    lalt = true;
                    _keyboardState.KeyPress(164);
                }
            }
            else
            {
                if (lalt)
                {
                    lalt = false;
                    _keyboardState.KeyUp(164);
                }
            }

            if ((Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) > 0)
            {
                if (!ralt)
                {
                    ralt = true;
                    _keyboardState.KeyPress(165);
                }
            }
            else
            {
                if (ralt)
                {
                    ralt = false;
                    _keyboardState.KeyUp(165);
                }
            }
            #endregion
        }

        void CloseWindow()
        {
            Glut.glutLeaveGameMode();
            Glut.glutDestroyWindow(_windowCode);
        }











    }
}

