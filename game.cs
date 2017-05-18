using System;
using System.IO;
using OpenTK;
using OpenTK.Input;

namespace template {

    class Game
    {
	    // member variables
	    public Surface screen;
        RayTracer rayTracer;
        // initialize
        public void Init()
	    {
            rayTracer = new RayTracer();
            rayTracer.Init(screen);
	    }
	    // tick: renders one frame
	    public void Tick()
	    {
            screen.Clear(0x444444);
            rayTracer.Render();
        }

        public void Input(InputHandler input)
        {
            if (input.KeyDown(Key.Right))
            {
                rayTracer.scene.sceneObjects[2].position.Y += 0.5f;
            }
            else if (input.KeyDown(Key.Left))
            {
                rayTracer.scene.sceneObjects[2].position.Y -= 0.5f;
            }
            if (input.KeyDown(Key.Up))
            {
                rayTracer.scene.sceneObjects[2].position.Z += 0.5f;
            }
            else if (input.KeyDown(Key.Down))
            {
                rayTracer.scene.sceneObjects[2].position.Z -= 0.5f;
            }
        }
    }

} // namespace Template