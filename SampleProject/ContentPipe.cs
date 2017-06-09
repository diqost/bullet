using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
namespace SampleProject
{
    class Texture2D
    {
        public int id;
        public int width, height;
        public Texture2D(int Id, int Width, int Height)
        {
            this.id = Id;
            this.width = Width;
            this.height = Height;
        }
    }
    class ContentPipe
    {
        public static Texture2D LoadTexture(string filepath)
        {
            Bitmap bitmap = new Bitmap(filepath);
            int id = GL.GenTexture();
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            bitmap.UnlockBits(bmpData);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            return new Texture2D(id, bitmap.Width, bitmap.Height);
        }

    }
}
