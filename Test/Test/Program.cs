using System;
using OpenTK;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing;
using SDL2;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Compute.OpenCL;


namespace Test
{
    class Program
    {

        

        static void Main(string[] args)
        {

            DisplayTextGL displayEngineGL = new DisplayTextGL();


            displayEngineGL.initialize();

            // 主循环
            bool running = true;
            while (running)
            {
                // 处理 SDL 事件
                SDL.SDL_Event e;
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    {
                        running = false;
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                    {
                        if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                        {
                            running = false;
                        }
                    }
                }

                // 渲染
                displayEngineGL.clearDisplay();

                displayEngineGL.showText("This is sample text", 60.0f, 50.0f, 0.6f, new Vector3(0.5f, 0.8f, 0.2f), new Vector2(1f, 0f));
                displayEngineGL.showText("Hello world", 200.0f, 100.0f, 1.0f, new Vector3(0.8f, 0.2f, 0.2f), new Vector2(1.0f, 1.2f));
                displayEngineGL.display();

                displayEngineGL.swapBuffer();

            }

            displayEngineGL.destroy();
        }

    }
}


