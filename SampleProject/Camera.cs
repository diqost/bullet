using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Windows.Forms;

namespace SampleProject
{
    class Camera
    {
        //public Camera(Vector3 eyePos, Vector3 lookAt)
        //{
        //    this.EyePosX = eyePos[0];
        //    this.EyePosY = eyePos[1];
        //    this.EyePosZ = eyePos[2];

        //    this.LookAtX = lookAt[0];
        //    this.LookAtY = lookAt[1];
        //    this.LookAtZ = lookAt[2];
        //}

        //public Vector3 getEyePosVector()
        //{
        //    return new Vector3(EyePosX, EyePosY, EyePosZ);
        //}
        //public Vector3 getLookAtVector()
        //{
        //    return new Vector3(LookAtX, LookAtY, LookAtZ);
        //}

        public Vector3 Position = Vector3.Zero;
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f,0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }
        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();

            Vector3 forward = new Vector3((float)Math.Sin((float)Orientation.X), 0, (float)Math.Cos((float)Orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }
        public void AddRotation(float x, float y)
        {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
        }

    }

}