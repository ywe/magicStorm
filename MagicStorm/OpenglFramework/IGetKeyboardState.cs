using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Opengl
{
    /// <summary>
    /// Про коды клавиш:
    /// везде, где есть латинская буква, код клавиши равен коду большой латинской буквы ('ф'='A' = 65). 
    /// где нет латинской буквы, с зажатым и нет shift получаются разные коды.
    /// f1-f12 : 131-142.
    /// shift,ctrl,alt : 160-165.
    /// Остальное по стандарту. 
    /// </summary>
    interface IGetKeyboardState
    {
        //есть целых 3 способа узнать состояние клавиш
        /// <summary>
        /// Узнать время, в течение которого зажато действие клавиатуры
        /// </summary>
        int GetActionTime(EKeyboardAction action);
        /// <summary>
        /// коды клавиш
        /// </summary>
        List<byte> GetPressedKeys();
        /// <summary>
        /// Текст. русские и английские, большие и маленькиe буквы
        /// </summary>
        string GetEnteredString();


        //Мышка
        /// <summary>
        /// на экране, верхний угол это (0,0)
        /// </summary>
        Point2 MousePosScreen { get;  }
        /// <summary>
        /// На карте, с учетом сдвига камеры
        /// </summary>
        Point2 MousePosMap { get; }
        bool MouseClick { get;  }
        bool MouseRightClick { get;  }
    }
}
