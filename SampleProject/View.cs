using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SampleProject
{
   

    class View
    {
        Vector3 Position;
        public double Rotation; // in radians, positive == clockwise
        public double Zoom;
        public View(Vector3 startPos, double rotation, double zoom)
        {
            this.Position = startPos;
            this.Rotation = rotation;
            this.Zoom = zoom;
        }
        public void Update()
        {

        }     
        public void ApplyTransform()
        {
            Matrix4 transform = Matrix4.Identity;
            transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(-Position.X, -Position.Y, 0f));
            transform = Matrix4.Mult(transform, Matrix4.CreateRotationZ(-(float)Rotation));
            transform = Matrix4.Mult(transform, Matrix4.CreateScale((float)Zoom,(float)Zoom, 1f));

            GL.MultMatrix(ref transform);
            
        }
    }
}
