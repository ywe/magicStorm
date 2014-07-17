using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.DevIl;

namespace MagicStorm.Opengl
{
    class OpenglInitializer
    {
        static OpenglInitializer()
        {
            Glut.glutInit(); // инициализация библиотеки Glut
        }

        #region public static methods
        public static int CreateWindow(int width, int height, bool tryFullScreen )
        {
            //!!! полный экран здесь не сделан, да и нафиг он нужен для 2д игры

            //Делаем окошко в левом верхнем углу
            Glut.glutInitWindowSize(width, height);
            Glut.glutInitWindowPosition(0, 0);
            return Glut.glutCreateWindow(Config.WindowName);
        }

        public static void SetDisplayModes()
        {
            Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            //поставили 2д режим с нужным размером экрана в игровых координатах
            Glu.gluOrtho2D(0.0, Config.ScreenWidth, Config.ScreenHeight, 0 );

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glClearColor(0, 0, 0, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //настройка работы с текстурами 
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

        }

        public static Dictionary<string, int> LoadTextures()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            foreach (var tex in Config.Sprites)
            {
                int code = TextureInit(Config.Sprites[tex.Key].file);
                if (code != -1) res.Add(tex.Key, code);
            }
            return res;
        }
        #endregion

        #region private texture load and make
        private static int TextureInit(string file)
        {
            if (Il.ilLoadImage(file))
            {
               
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        return MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                    case 32:
                        return MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                }
            }
            return -1;
        }

        private static int MakeGlTexture(int format, IntPtr pixels, int w, int h)
        {
            // индетефекатор текстурного объекта
            int texObject;

            // генерируем текстурный объект
            Gl.glGenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);
            //Gl.glTexFilterFuncSGIS
            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            // создаем RGB или RGBA текстуру
            switch (format)
            {
                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
            }

            return texObject;
        }
        #endregion
    }
}
