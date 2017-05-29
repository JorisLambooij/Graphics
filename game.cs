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

        protected float angularSteps = (float) (2 * Math.PI / 12);
        public void Input(InputHandler input)
        {
            if (input.KeyDown(Key.Right))
            {
                ((Sphere)rayTracer.scene.sceneObjects[3]).thetaOffset += angularSteps;
            }
            else if (input.KeyDown(Key.Left))
            {
                ((Sphere)rayTracer.scene.sceneObjects[3]).thetaOffset -= angularSteps;
            }
            if (input.KeyDown(Key.Up))
            {
                ((Sphere)rayTracer.scene.sceneObjects[3]).phiOffset += 0.5f * angularSteps;
            }
            else if (input.KeyDown(Key.Down))
            {
                ((Sphere)rayTracer.scene.sceneObjects[3]).phiOffset -= 0.5f * angularSteps;
            }

            if (input.KeyDown(Key.W))
            {
                rayTracer.camera.position.X++;
            }
            else if (input.KeyDown(Key.S))
            {
                rayTracer.camera.position.X--;
            }
        }
    }

} // namespace Template