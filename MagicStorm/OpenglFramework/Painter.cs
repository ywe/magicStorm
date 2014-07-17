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
    class Painter
    {

        public static void DrawFrame(Frame frame, Dictionary<string, int> spriteCodes)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glLoadIdentity();
           // Gl.glRotated(-frame.camera.angleDeg, 0, 0,1); //todo поворот камеры относительно центра
            List<object> obj; List<bool> applyCamera;
            frame.IAmPainterAndIWantToDrawEverythingHere(out obj, out applyCamera);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glColor3b(255, 0, 0);
            Gl.glVertex2d(0, 0);
            Gl.glColor3b(255, 0, 0);
            Gl.glVertex2d(0, 5);
            Gl.glColor3b(255, 0, 0);
            Gl.glVertex2d(5, 5);
            Gl.glColor3b(255, 0, 0);
            Gl.glVertex2d(5, 0);
            Gl.glEnd();
            for (int i = 0; i < obj.Count; i++)
            {
                if (obj[i] is Sprite)
                {
                    Sprite sprite = (Sprite)obj[i];
                    Gl.glPushMatrix();
                    if(applyCamera[i])
                        Gl.glTranslated(sprite.pos.x - frame.camera.x, sprite.pos.y - frame.camera.y, 0);
                    else
                        Gl.glTranslated(sprite.pos.x, sprite.pos.y, 0);
                    Gl.glRotated(sprite.pos.angleDeg, 0, 0, 1);
                    if (sprite.color == null)
                        DrawTexture(sprite, spriteCodes[sprite.name.ToString()]);
                    else
                        DrawRectangle(sprite);
                    Gl.glPopMatrix();
                }
                else if (obj[i] is Text)
                {
                    Text text = (Text)obj[i]; 
                    foreach (Sprite spr in text.GetSpritesWithRelativePos())
                    {
                        Gl.glPushMatrix();
                        if (applyCamera[i])
                            Gl.glTranslated(spr.pos.x - frame.camera.x, spr.pos.y - frame.camera.y, 0);
                        else
                            Gl.glTranslated(spr.pos.x, spr.pos.y, 0);
                        Gl.glRotated(spr.pos.angleDeg, 0, 0, 1);
                        DrawTexture(spr, spriteCodes[spr.texture]);
                        Gl.glPopMatrix();
                    }
                }
            }

            
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glFinish();
            Glut.glutSwapBuffers();
        }
        private static void DrawRectangle(Sprite sprite)
        {
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glColor4bv(sprite.color);
            Gl.glVertex2d(-sprite.width / 2, -sprite.height / 2);
            Gl.glVertex2d(sprite.width / 2, -sprite.height / 2);
            Gl.glVertex2d(sprite.width / 2, sprite.height / 2);
            Gl.glVertex2d(-sprite.width / 2, sprite.height / 2);
            Gl.glColor3b(255,0,0);
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(5,0);
            Gl.glVertex2d(5, 5);
            Gl.glVertex2d(0, 5);
            Gl.glEnd();
        }

        private static void DrawTexture(Sprite sprite, int textureCode)
        {
           // if (IsSpriteOutScreen(sprite)) return; наверное опенгл и сам это делает

            int hor = Config.Sprites[sprite.texture].horFrames;
            int vert = Config.Sprites[sprite.texture].vertFrames;

            double horPart = 1d/hor, vertPart = 1d/ vert; 
            double bottom = 1- (sprite.frame / hor+1) * vertPart;
            double top = 1- sprite.frame / hor  * vertPart;
            double right= (sprite.frame % hor+1) * horPart;
            double left = sprite.frame % hor  * horPart;

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureCode);

            Gl.glBegin(Gl.GL_QUADS);
                // указываем поочередно вершины и текстурные координаты
                Gl.glTexCoord2d(left, top);
                Gl.glVertex2d(-sprite.width/2, -sprite.height/2);
                Gl.glTexCoord2d(right, top);
                Gl.glVertex2d(sprite.width / 2, -sprite.height / 2);
                Gl.glTexCoord2d(right, bottom);
                Gl.glVertex2d(sprite.width / 2, sprite.height / 2);
                Gl.glTexCoord2d(left, bottom);
                Gl.glVertex2d(-sprite.width / 2, sprite.height / 2);
            Gl.glEnd();

        }

        static bool IsSpriteOutScreen(Sprite sprite)
        {
            double radius = Math.Sqrt(sprite.width*sprite.width + sprite.height * sprite.height) / 2;

            return (sprite.pos.x + radius < 0 || sprite.pos.y + radius < 0 ||
                sprite.pos.x - radius > Config.ScreenWidth || sprite.pos.y - radius > Config.ScreenHeight);
            
        }
    }
}
