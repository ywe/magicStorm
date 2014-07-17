using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm
{
    /// <summary>
    /// Кадр, который игра обязана вернуть контроллеру для отрисовки. Можно добавлять спрайты, тексты
    /// и другой фрейм(добавится все содержимое, кроме камеры). Порядок отрисовки тот, в котором добавляли. 
    /// Если хотим добавить меню (чтобы картинки и текст не двигались вместе с камерой),
    /// добавляем его как фрейм, в конструкторе есть булевская переменная для этого.
    /// </summary>
    class Frame
    {
        List<object> drawedObjects = new List<object>(); //тут в правильном порядке
        List<bool> applyCamera = new List<bool>(); //соответствующий лист - применять ли сдвиг камеры

    
        /// <summary>
        /// левый верхний угол
        /// </summary>
        public Point2 camera ;
        

        public void Add(params Sprite[] sprites)
        {
            foreach (Sprite spr in sprites)
            {
                this.drawedObjects.Add(spr);
                this.applyCamera.Add(true);
            }
        }

        public void Add(params Text[] texts)
        {
            foreach (Text txt in texts)
            {
                this.drawedObjects.Add(txt);
                this.applyCamera.Add(true);
            }
        }

        /// <summary>
        /// складывает 2 фрейма(только камера не меняется). Если добавить фрейм как GUI, он не будет смещаться вместе с камерой.
        /// </summary>
        public void Add(bool isGUI, params Frame[] frames)
        {
            foreach(Frame frame in frames){
                this.drawedObjects.AddRange(frame.drawedObjects);
                for (int i = 0; i < frame.drawedObjects.Count; i++)
                    this.applyCamera.Add(!isGUI);
            }
        }

        /// <summary>
        /// в игре не вызывать
        /// </summary>
        public void IAmPainterAndIWantToDrawEverythingHere(out List<object> obj, out List<bool> applyCamera)
        {
            obj = this.drawedObjects;
            applyCamera = this.applyCamera;
        }
    }
}
