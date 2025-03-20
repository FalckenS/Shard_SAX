using OpenTK.Mathematics;
using SDL2;
using System.Collections.Generic;

namespace Shard
{
    class Player : GameObject, InputListener
    {
        private Camera _camera;

        private Dictionary<string, ModelObject> _models;
        private Dictionary<string, Vector3> _modelOffsets;

        public Matrix4 ModelMatrix;

        public Player(float tx, float ty, float tz)
        {
            Transform.X = tx;
            Transform.Y = ty;
            Transform.Z = tz;
            _camera = new Camera(new Vector3(tx, ty, tz), 1.0f);

            _models = new Dictionary<string, ModelObject>();
            _modelOffsets = new Dictionary<string, Vector3>();

            Bootstrap.getInput().addListener(this);
        }

        public void LinkModel(string name, ModelObject model)
        {
            _models[name] = model;

        }

        public void SetModelOffset(string name, Vector3 offest)
        {
            _modelOffsets[name] = offest;
            UpdateModelTransform();
        }

        private void UpdateModelTransform()
        {
            float modelRotY = -_camera.Yaw - 90 + 180;
            float modelRotX = -_camera.Pitch;

            foreach (var modelName in _models.Keys)
            {
                Vector4 pos = new Vector4(_modelOffsets[modelName], 1.0f) * Matrix4.Invert(_camera.GetViewMatrix());
                _models[modelName].TransMatrix = Matrix4.CreateTranslation(new Vector3(pos));
                _models[modelName].RotMatrix = Matrix4.Invert(_camera.GetRotationMatrix());
            }
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
                float sensitivity = 0.15f;
                _camera.Yaw += inp.Dx * sensitivity;
                _camera.Pitch -= inp.Dy * sensitivity;
            }

            UpdateModelTransform();

        }

    }
}
