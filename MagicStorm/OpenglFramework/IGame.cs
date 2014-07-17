using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Opengl
{
    /// <summary>
    /// Все, что должен делать главный класс - изменить состояние и вернуть новый кадр назад контроллера
    /// </summary>
    interface IGame
    {
        //если очередной кадр не получен, закрываем окно
        Frame Process(IGetKeyboardState keyboard);
    }
}
