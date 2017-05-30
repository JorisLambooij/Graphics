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

            float speed = 0.5f;

            // forward / backward
            if (input.KeyDown(Key.W))
                rayTracer.camera.position += rayTracer.camera.Direction * speed;
            
            else if (input.KeyDown(Key.S))
                rayTracer.camera.position -= rayTracer.camera.Direction * speed;

            // strafe left / right
            if (input.KeyDown(Key.A))
                rayTracer.camera.position -= rayTracer.camera.screenRight * speed;

            else if (input.KeyDown(Key.D))
                rayTracer.camera.position += rayTracer.camera.screenRight * speed;

            // turn left / right
            if (input.KeyDown(Key.Q))
                rayTracer.camera.Direction -= rayTracer.camera.screenRight * 0.05f;
        
            else if (input.KeyDown(Key.E))
                rayTracer.camera.Direction += rayTracer.camera.screenRight * 0.05f;

        }
    }

} // namespace Template