using System;
using System.Drawing;
using BulletSharp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Timers;

namespace SampleProject
{
    class VisualizationDemo : GameWindow
    {
        Physics physics;
        float angle = 0.0f;
        float frameTime; int fps;
        int a, b, c = 0;
        Camera cam;
        public Timer timer;
        public VisualizationDemo()
            : base(600, 600,
            new GraphicsMode(), "BulletSharp OpenTK Demo")
        {
            VSync = VSyncMode.On;
            physics = new Physics();

            //timer = new Timer(300); //300 is timer tick time in millis
            //timer.Elapsed += timerCallback; // method which should be invoked when timer tick is ready to perform
            //timer.AutoReset = true;
            //timer.Enabled = true; //actually start the timer;
            
            cam = new Camera();
        }
        public static Random rnd = new Random();
       
        

        protected override void OnLoad(System.EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.MidnightBlue);

            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);
            
        }

        protected override void OnUnload(System.EventArgs e)
        {
            physics.ExitPhysics();
            base.OnUnload(e);
        }
        KeyboardState lastKeyState;
        Vector2 lastMousePos = new Vector2();
        Camera flyingCamera = new Camera();
        void ResetCursor()
        {
            OpenTK.Input.Mouse.SetPosition(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
            lastMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            physics.Update((float)e.Time);

            base.OnUpdateFrame(e);
            KeyboardState keyState = OpenTK.Input.Keyboard.GetState();

            if (keyState.IsKeyDown(Key.W))
                cam.Move(0f, 0.1f, 0f);
            if (keyState.IsKeyDown(Key.A))
                cam.Move(-0.1f, 0f, 0f);
            if (keyState.IsKeyDown(Key.S))
                cam.Move(0f, -0.1f, 0f);
            if (keyState.IsKeyDown(Key.D))
                cam.Move(0.1f, 0f, 0f);
            if (keyState.IsKeyDown(Key.Q))
                cam.Move(0f, 0f, 0.1f);
            if (keyState.IsKeyDown(Key.E))
                cam.Move(0f, 0f, -0.1f);

            if (keyState.IsKeyDown(Key.Left))
                cam.Orientation.X -= .05f;
            if (keyState.IsKeyDown(Key.Right))
                cam.Orientation.X += .05f;
            if (keyState.IsKeyDown(Key.Up))
                cam.Orientation.Y += .05f;
            if (keyState.IsKeyDown(Key.Down))
                cam.Orientation.Y -= .05f;

            if ((keyState.IsKeyDown(Key.X)) && lastKeyState.IsKeyUp(Key.X))
            {
                physics.addBox(cam.Position * 2.0f, cam.Orientation * 50);
            }
                if ((keyState.IsKeyDown(Key.C)) && lastKeyState.IsKeyUp(Key.C)) { 
                
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 5; j++)
                        for (int l = 0; l < 5; l++)
                        {
                            physics.addBox(new Vector3(cam.Position.X, cam.Position.Y, cam.Position.Z) + new Vector3(i + 2, j + 2, l + 2), Vector3.Zero);
                        }
            }
            if (keyState.IsKeyDown(Key.I))
                b = 10;
            else
               if (keyState.IsKeyDown(Key.K))
                b = 0;
            else
                b = -10;
            if (keyState.IsKeyDown(Key.J))
                a = -10;
            else
                if (keyState.IsKeyDown(Key.L))
                a = 10;
            else
                a = 0;
            if (keyState.IsKeyDown(Key.U))
                c = -10;
            else
                if (keyState.IsKeyDown(Key.O))
                c = 10;
            else
                c = 0;
            physics.World.Gravity = new Vector3(a, b, c);
            if (keyState.IsKeyDown(Key.F) /*&& lastKeyState.IsKeyUp(Key.F) */)
                this.physics.addBox(new Vector3(0, 20, 0), Vector3.Zero);
 
            if (keyState.IsKeyDown(Key.R))
            {
                for (int i = 0; i < 2; i++)
                {
                    this.physics.addBox(new Vector3(rnd.Next(50), 40, rnd.Next(50)), Vector3.Zero);
                }
                

            }

            if (keyState.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            lastKeyState = keyState;
        }
        
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            frameTime += (float)e.Time;
            fps++;
          
            if (frameTime >= 1)
            {
                Title = "BulletSharp OpenTK Demo, FPS = " + fps.ToString();
                frameTime = fps = 0;
            } 

            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perspective = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, aspect_ratio, 1.0f, 400);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = cam.GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);

            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
            angle += (float)e.Time * 100;


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            InitCube();

            foreach (RigidBody body in physics.World.CollisionObjectArray)
            {
                Matrix4 modelLookAt = body.MotionState.WorldTransform * lookat;
                GL.LoadMatrix(ref modelLookAt);

                if ("Ground".Equals(body.UserObject))
                {
                    DrawCube(Color.Green, 50.0f);
                    continue;
                }
                CustomBox box = body as CustomBox; // the right side returns null if body has RigidBody type, and CustomBox otherwise...

                if (box == null) // body is RegidBody, not CustomBox
                    DrawCube2(Color.Red);
                else
                    DrawCube2(box.color);
                
            }

            UninitCube();

            SwapBuffers();
        }

        private void DrawCube(Color color, float size)
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(color);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, -size, -size);

            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(-size, -size, size);

            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, size, -size);

            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);

            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, -size, size);

            GL.End();
        }

        float[] vertices = new float[] {1,1,1,  -1,1,1,  -1,-1,1,  1,-1,1,
            1,1,1,  1,-1,1,  1,-1,-1,  1,1,-1,
            1,1,1,  1,1,-1,  -1,1,-1,  -1,1,1,
            -1,1,1,  -1,1,-1,  -1,-1,-1,  -1,-1,1,
            -1,-1,-1,  1,-1,-1,  1,-1,1,  -1,-1,1,
            1,-1,-1,  -1,-1,-1,  -1,1,-1,  1,1,-1};

        float[] normals = new float[] {0,0,1,  0,0,1,  0,0,1,  0,0,1,
            1,0,0,  1,0,0,  1,0,0, 1,0,0,
            0,1,0,  0,1,0,  0,1,0, 0,1,0,
            -1,0,0,  -1,0,0, -1,0,0,  -1,0,0,
            0,-1,0,  0,-1,0,  0,-1,0,  0,-1,0,
            0,0,-1,  0,0,-1,  0,0,-1,  0,0,-1};

        byte[] indices = {0,1,2,3,
            4,5,6,7,
            8,9,10,11,
            12,13,14,15,
            16,17,18,19,
            20,21,22,23};

        void InitCube()
        {
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.NormalPointer(NormalPointerType.Float, 0, normals);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vertices);
        }

        void UninitCube()
        {
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
        }

        void DrawCube2(Color color)
        {
            GL.Color3(color);
            GL.DrawElements(PrimitiveType.Quads, 24, DrawElementsType.UnsignedByte, indices);
        }
    }
}
