using System.Collections.Generic;
using BulletSharp;
using OpenTK;
using System;
using System.Timers;
using System.Drawing;
//using OpenTK.Graphics;

namespace SampleProject
{
    class CustomBox : RigidBody // Create class CustomBox subclass of RigidBody
    {
        private const float boxSize = 1; // one of cube size 
        private const float boxMass = 1; // cube mass
        private static CollisionShape boxColShape = new BoxShape(boxSize); // shape of box
        private static Vector3 localInertia = boxColShape.CalculateLocalInertia(boxMass); // intertia of a box
        private static RigidBodyConstructionInfo rbInfo = 
                        new RigidBodyConstructionInfo(boxMass, null, boxColShape, localInertia); // info for creation a box
        public Color color = Color.Red; // default value if not set

        /// <summary>
        /// LOOK AT PRIVATE it means that constructor is not visible 
        /// outside the class, it is used only in createBox method of this class below..
        /// </summary>
        /// <param name="constructionInfo"> info for creation box </param>
        private CustomBox(RigidBodyConstructionInfo constructionInfo) : base(constructionInfo)
        {
        }
        /// <summary>
        /// // LOOK static keyword means that this method belongs to class not an object, so
        /// to invoke it you should write CustomBox.CreateCustomBox(...) 
        ///
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <returns>
        /// LOOKS IT'S RETURN CustomBox so you should use it like:
        /// var box = CustomBox.CreateCustomBox(...);
        /// world.addRigidBody(box); (box is subclass of RigidBody)
        /// </returns>
        public static CustomBox CreateCustomBox(Vector3 position, Color color)
        {

            Matrix4 startTransform = Matrix4.CreateTranslation(position);

            // using motionstate is recommended, it provides interpolation capabilities
            // and only synchronizes 'active' objects
            rbInfo.MotionState = new DefaultMotionState(startTransform);

            CustomBox box = new CustomBox(rbInfo);

            // make it drop from a height
            box.UserObject = "CustomBox";
            box.color = color;
            return box;
        }
      
    }
    class Physics
    {
  
        public DiscreteDynamicsWorld World { get; set; }
        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        List<CollisionShape> collisionShapes = new List<CollisionShape>();
        CollisionConfiguration collisionConf;
        CollisionShape boxColShape;
        const float fromX = 0, toX = (float)Math.PI;
        const float stepX = 0.1f;
        public int functionOffsetY = 5;
        public int functionAmplitude = 5;
        public int functionPeriod = 5;
        public int functionDensity = 3;
        public double offsetXByTime = 0;
        public const int FunctionZWidth = 5;
        public RigidBody ground;

        List<RigidBody> functionBoxes = new List<RigidBody>();

        public Timer timer;
        public static Random rnd = new Random();
        public void addBox(Vector3 position, Vector3 impulse)
        {
            var randomColor = Color.FromArgb(1, rnd.Next(256), rnd.Next(256), rnd.Next(256));
            var body = CustomBox.CreateCustomBox(position, randomColor);
            body.UserObject = "Box";
            World.AddRigidBody(body);
            body.ApplyCentralImpulse(impulse);
        }
        public Physics()
        {
            // collision configuration contains default setup for memory, collision setup
            collisionConf = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConf);

            broadphase = new DbvtBroadphase();
            World = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
            World.Gravity = new Vector3(0, -10, 0);

            // create the ground
            CollisionShape groundShape = new BoxShape(50, 50, 50);
            collisionShapes.Add(groundShape);
            ground = LocalCreateRigidBody(0, Matrix4.CreateTranslation(0, -50, 0), groundShape);
            ground.UserObject = "Ground";

            // create a few dynamic rigidbodies
            const float mass = 1.0f;

            boxColShape = new BoxShape(1);
           
            collisionShapes.Add(boxColShape);
            Vector3 localInertia = boxColShape.CalculateLocalInertia(mass);

            var rbInfo = new RigidBodyConstructionInfo(mass, null, boxColShape, localInertia);

            int boxNumber = 0;
            for (float curX = fromX; curX < toX; curX += stepX)
            {
                int curXOffset = boxNumber / FunctionZWidth * functionDensity;
                for (int z = 0; z < FunctionZWidth; ++z)
                {
                    Vector3 boxPos = new Vector3(curX + curXOffset, (float)Math.Cos(functionPeriod * curX) * functionAmplitude + functionOffsetY, z);
                    Matrix4 startTransform = Matrix4.CreateTranslation(boxPos);

                    // using motionstate is recommended, it provides interpolation capabilities
                    // and only synchronizes 'active' objects
                    rbInfo.MotionState = new DefaultMotionState(startTransform);

                    RigidBody body = new RigidBody(rbInfo);

                    // make it drop from a height
                    // body.Translate(new Vector3(0, 0, 0));

                    World.AddRigidBody(body);
                    body.Gravity = Vector3.Zero;
                    functionBoxes.Add(body);
                    boxNumber++;
                }
            }
            timer = new Timer(100);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
          
                      
                    
            

            rbInfo.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            offsetXByTime += 0.05f;
        
            int boxNumber = 0;


            for (float curX = fromX; curX < toX; curX += stepX)
            {
                int curXOffset = boxNumber / FunctionZWidth * functionDensity;
                for (int z = 0; z < FunctionZWidth; ++z)
                {
                    RigidBody body = functionBoxes[boxNumber];
                    if (body == null)
                        continue;
                    Vector3 boxPos = new Vector3(curX + curXOffset, (float)Math.Sin(functionPeriod * (curX + offsetXByTime)) * functionAmplitude + functionOffsetY, z + z * 3);
                 
                    lock (body)
                    {
                        body.Activate();
                        body.ProceedToTransform(Matrix4.CreateTranslation(boxPos));
                    }
                  
                  
                    boxNumber++;

                }
            }
            

        }
        /// <summary>
        /// Spawn a wall of cubes (rigid bodies) in the world
        /// </summary>
        /// <param name="leftBottomPosition"></param>
        /// <param name="size">actual number of cubes in X, Y, and Z axis, if x=1,y=3,z=5 , than 1*3*5 cubes will be spawn</param>
        private void spawnWall(Vector3 leftBottomPosition, Vector3 size)
        {
            for (int x = 0; x < size[0]; x++)           
                for (int y = 0; x< size[1];y++)
                    for (int z = 0; z < size[2]; z++)
                    {
                        addBox(leftBottomPosition + new Vector3(x, y, z),Vector3.Zero);
                    }
            
        }
        
        public virtual void Update(float elapsedTime)
        {
            World.StepSimulation(elapsedTime);

            for (int i = 0; i < World.Dispatcher.NumManifolds; ++i)
            {
                PersistentManifold manifold = World.Dispatcher.GetManifoldByIndexInternal(i);
                RigidBody A = manifold.Body0 as RigidBody;
                RigidBody B = manifold.Body1 as RigidBody;
               

                if (A.UserObject != null && A.UserObject.Equals("Ground"))
                {
                     World.RemoveRigidBody(B);
                }
                else
                    if (B.UserObject != null && B.UserObject.Equals("Ground"))
                {
 
                    World.RemoveRigidBody(A);
                }
            }

            //  Console.WriteLine(elapsedTime);
        }
        public void ExitPhysics()
        {
            //remove/dispose constraints
            int i;
            for (i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            //delete collision shapes
            foreach (CollisionShape shape in collisionShapes)
                shape.Dispose();
            collisionShapes.Clear();

            World.Dispose();
            broadphase.Dispose();
            if (dispatcher != null)
            {
                dispatcher.Dispose();
            }
            collisionConf.Dispose();
        }

        public RigidBody LocalCreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);

            World.AddRigidBody(body);

            return body;
        }
    }
}
