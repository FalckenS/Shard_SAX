/*
*
*   SDL provides an input layer, and we're using that.  This class tracks input, anchors it to the 
*       timing of the game loop, and converts the SDL events into one that is more abstract so games 
*       can be written more interchangeably.
*   @author Michael Heron
*   @version 1.0
*   
*/

using OpenTK.Windowing.Desktop;
using SDL2;
using System;
using System.Runtime.InteropServices;

namespace Shard
{

    // We'll be using SDL2 here to provide our underlying input system.
    class InputFramework3D : InputSystem
    {

        double tick, timeInterval;
        bool mouse_first_move = true;
        int lx = 0, ly = 0;

        public override void getInput()
        {

            SDL.SDL_Event ev;
            InputEvent ie;
            

            //tick += Bootstrap.getDeltaTime();

            //if (tick < timeInterval)
            //{
            //    return;
            //}

            //while (tick >= timeInterval)
            //{
                while (SDL.SDL_PollEvent(out ev) != 0)
                {

                    ie = new InputEvent();

                    if (ev.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
                    {
                        //if (mouse_first_move)
                        //{
                        //    lx = ev.motion.x;
                        //    ly = ev.motion.y;
                        //    mouse_first_move = false;
                        //}
                        //else
                        //{
                        //    SDL.SDL_MouseMotionEvent mot;

                        //    mot = ev.motion;

                        //    ie.X = mot.x;
                        //    ie.Y = mot.y;
                        //    ie.Lx = lx;
                        //    ie.Ly = ly;

                        //    Debug.getInstance().log("MouseMotion: " + ie.X + ", " + ie.Y);

                        //    informListeners(ie, "MouseMotion");

                        //    lx = ev.motion.x;
                        //    ly = ev.motion.y;
                        //}
                        SDL.SDL_MouseMotionEvent mot = ev.motion;

                        ie.Dx = mot.xrel;
                        ie.Dy = mot.yrel;
                        Debug.getInstance().log("MouseMotion: " + ie.Dx + ", " + ie.Dy);
                        informListeners(ie, "MouseMotion");
                    }

                    if (ev.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
                    {
                        SDL.SDL_MouseButtonEvent butt;

                        butt = ev.button;

                        ie.Button = (int)butt.button;
                        ie.X = butt.x;
                        ie.Y = butt.y;

                        informListeners(ie, "MouseDown");
                    }

                    if (ev.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
                    {
                        SDL.SDL_MouseButtonEvent butt;

                        butt = ev.button;

                        ie.Button = (int)butt.button;
                        ie.X = butt.x;
                        ie.Y = butt.y;

                        informListeners(ie, "MouseUp");
                    }

                    if (ev.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
                    {
                        SDL.SDL_MouseWheelEvent wh;

                        wh = ev.wheel;

                        ie.X = (int)wh.direction * wh.x;
                        ie.Y = (int)wh.direction * wh.y;

                        informListeners(ie, "MouseWheel");
                    }
                }

                int arraySize;
                IntPtr origArray = SDL.SDL_GetKeyboardState(out arraySize);
                byte[] keys = new byte[arraySize];
                Marshal.Copy(origArray, keys, 0, arraySize);
                for (int i = 0; i < arraySize; i++)
                {
                    if (keys[i] == 1)
                    {
                        ie = new InputEvent();
                        ie.Key = i;
                        Debug.getInstance().log("KeyPressed: " + ie.Key);
                        informListeners(ie, "KeyPressed");
                    }
                }

                //tick -= timeInterval;
            //}
        }




        public override void initialize()
        {
            tick = 0;
            timeInterval = 1.0 / 60.0;
        }

    }
}