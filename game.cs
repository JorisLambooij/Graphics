using System;
using System.IO;
using OpenTK;

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
            /*screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffffff );
            screen.Line(2, 20, 160, 20, 0xff0000);



            Plane plane = new Plane(new Vector3(0,0,0), new Vector3(0, 0, 1), new Vector3(1,1,1));

            Ray ray = new Ray(new Vector3(5f, 5f, 2), new Vector3(1, 0, 0));

            Intersection i = plane.intersectPrimitive(ray);

            screen.Print("Intersection Point: " + i.intersectionPoint, 20, 20, 0xffffff);
            */
            rayTracer.Render();
        }
    }

} // namespace Template