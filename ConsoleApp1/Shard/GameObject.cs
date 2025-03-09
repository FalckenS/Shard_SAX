/*
*
*   Anything that is going to be an interactable object in your game should extend from GameObject.  
*       It handles the life-cycle of the objects, some useful general features (such as tags), and serves 
*       as the convenient facade to making the object work with the physics system.  It's a good class, Bront.
*   @author Michael Heron
*   @version 1.0
*   
*/

using OpenTK.Mathematics;
using SDL2;
using System;
using System.Collections.Generic;

namespace Shard
{
    class GameObject
    {
        private Transform3D transform;
        private bool transient;
        private bool toBeDestroyed;
        private bool visible;
        private PhysicsBody myBody;
        private List<string> tags;

        public void addTag(string str)
        {
            if (tags.Contains(str))
            {
                return;
            }

            tags.Add(str);
        }

        public void removeTag(string str)
        {
            tags.Remove(str);
        }

        public bool checkTag(string tag)
        {
            return tags.Contains(tag);
        }

        public String getTags()
        {
            string str = "";

            foreach (string s in tags)
            {
                str += s;
                str += ";";
            }

            return str;
        }

        public void setPhysicsEnabled()
        {
            MyBody = new PhysicsBody(this);
        }


        public bool queryPhysicsEnabled()
        {
            if (MyBody == null)
            {
                return false;
            }
            return true;
        }

        internal Transform3D Transform
        {
            get => transform;
        }

        internal Transform Transform2D
        {
            get => (Transform)transform;
        }


        public bool Visible
        {
            get => visible;
            set => visible = value;
        }
        public bool Transient { get => transient; set => transient = value; }
        public bool ToBeDestroyed { get => toBeDestroyed; set => toBeDestroyed = value; }
        internal PhysicsBody MyBody { get => myBody; set => myBody = value; }

        public virtual void initialize()
        {
        }

        public virtual void update()
        {

        }

        public virtual void physicsUpdate()
        {
        }

        public virtual void prePhysicsUpdate()
        {
        }

        public GameObject()
        {
            GameObjectManager.getInstance().addGameObject(this);

            transform = new Transform3D(this);
            visible = false;

            ToBeDestroyed = false;
            tags = new List<string>();

            this.initialize();

        }

        public void checkDestroyMe()
        {

            if (!transient)
            {
                return;
            }

            if (Transform.X > 0 && Transform.X < Bootstrap.getDisplay().getWidth())
            {
                if (Transform.Y > 0 && Transform.Y < Bootstrap.getDisplay().getHeight())
                {
                    return;
                }
            }


            ToBeDestroyed = true;

        }

        public virtual void killMe()
        {
            PhysicsManager.getInstance().removePhysicsObject(myBody);

            myBody = null;
            transform = null;
        }


    }

    class CubeObject : GameObject
    {
        float w, h, d;
        RenderParams renderParams;

        public CubeObject(float tx, float ty, float tz, // translation
                          float rx, float ry, float rz, // rotation
                          float sx, float sy, float sz,  // scale
                          float _w, float _h, float _d) // w h d
        {
            Transform.X = tx;
            Transform.Y = ty;
            Transform.Z = tz;
            Transform.Rotx = rx;
            Transform.Roty = ry;
            Transform.Rotz = rz;
            Transform.Scalex = sx;
            Transform.Scaley = sy;
            Transform.Scalez = sz;
            w = _w;
            h = _h;
            d = _d;
        }

        public float W { get => w; set => w = value; }
        public float H { get => h; set => h = value; }
        public float D { get => d; set => d = value; }

        public RenderParams RParams { get => renderParams; set => renderParams = value; }

        public Matrix4 calcModel()
        {
            Matrix4 trans = Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Transform.Rotx));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Transform.Roty));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Transform.Rotz));
            Matrix4 scale = Matrix4.CreateScale(Transform.Scalex, Transform.Scaley, Transform.Scalez);
            return scale * rotZ * rotY * rotX * trans;
            // return rotZ * rotY * rotX * trans;

        }
    }

    enum LightSourceType
    {
        Point,
        Dir
    }

    class LightObject : GameObject
    {
        Vector3 color;
        Vector3 position;
        LightSourceType type;
        Vector3 ambient;
        Vector3 diffuse;
        Vector3 specular;

        public LightObject(float tx, float ty, float tz, // translation
                          float rx, float ry, float rz, // rotation
                          float sx, float sy, float sz,
                          Vector3 _color, LightSourceType _type)
        {
            Transform.X = tx;
            Transform.Y = ty;
            Transform.Z = tz;
            Transform.Rotx = rx;
            Transform.Roty = ry;
            Transform.Rotz = rz;
            Transform.Scalex = sx;
            Transform.Scaley = sy;
            Transform.Scalez = sz;
            color = _color;
            position = new Vector3(tx, ty, tz);
            type = _type;
        }

        public Matrix4 calcModel()
        {
            Matrix4 trans = Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Transform.Rotx));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Transform.Roty));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Transform.Rotz));
            Matrix4 scale = Matrix4.CreateScale(Transform.Scalex, Transform.Scaley, Transform.Scalez);
            return scale * rotZ * rotY * rotX * trans;
        }

        public Vector3 getColor()
        {
            return color;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public LightSourceType getType()
        {
            return type;
        }

        public Vector3 Ambient { get => ambient; set => ambient = value; }
        public Vector3 Diffuse { get => diffuse; set => diffuse = value; }
        public Vector3 Specular { get => specular; set => specular = value; }

    }

    class Player : GameObject, InputListener
    {
        private Camera _camera;

        public Player(float tx, float ty, float tz)
        {
            Transform.X = tx;
            Transform.Y = ty;
            Transform.Z = tz;
            _camera = new Camera(new Vector3(tx, ty, tz), 1.0f);

            Bootstrap.getInput().addListener(this);
        }

        public Camera GetCamera()
        {
            return _camera;
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (Bootstrap.getRunningGame().isRunning() == false)
            {
                return;
            }

            float amount = (float)(10 * Bootstrap.getDeltaTime());

            if (eventType == "KeyPressed")
            {

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    _camera.Position += amount * _camera.Right;
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    _camera.Position -= amount * _camera.Right;
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_W)
                {
                    Vector3 v = new Vector3(_camera.Front.X, 0, _camera.Front.Z);
                    v.Normalize();
                    _camera.Position += amount * new Vector3(v);
                    
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_S)
                {
                    Vector3 v = new Vector3(_camera.Front.X, 0, _camera.Front.Z);
                    v.Normalize();
                    _camera.Position -= amount * new Vector3(v);
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_Q)
                {
                    _camera.Position += amount * new Vector3(0, 1.0f, 0);
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_E)
                {
                    _camera.Position -= amount * new Vector3(0, 1.0f, 0);
                }

            }

            if (eventType == "MouseMotion")
            {
                float sensitivity = 0.2f;
                //var deltaX = inp.X - inp.Lx;
                //var deltaY = inp.Y - inp.Ly;
                //_camera.Yaw += deltaX * sensitivity;
                //_camera.Pitch -= deltaY * sensitivity;
                _camera.Yaw += inp.Dx * sensitivity;
                _camera.Pitch -= inp.Dy * sensitivity;
            }


        }

    }  

}
