using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template
{
    class Camera
    {
        public Vector3 position;
        public Vector3 direction;
        public Vector2 screenPlane;

        public Vector3 right, up;

        public float screenPlaneSize, screenPlaneRange;
        float viewDistance, angle;

        public Camera(Vector3 position, Vector3 direction, float angle)
        {
            this.position = position;
            this.direction = direction;
            this.angle = angle;

            //Constante afstand van camera tot screenPlane.
            viewDistance = 1f;
            ScreenPlaneSize();

            this.right = (Vector3.Cross(Vector3.UnitZ, direction)).Normalized() * screenPlaneRange;
            this.up = (Vector3.Cross(direction, right)).Normalized() * screenPlaneRange;
        }

        void ScreenPlaneSize()
        {
            //screenPlaneRange geeft aan hoe lang elke kant van een as is.
            //Bijvoorbeeld:
            //screenPlaneRange - 2 --> x-as gaat van -2 tot 2 en y-as gaat van -2 tot 2.
            screenPlaneRange = (float)Math.Tan(angle / 2f) * viewDistance;

            //screenPlaneSize geeft aan hoe breed en hoog de screenPlane in totaal is.
            screenPlaneSize = screenPlaneRange * 2f;

            screenPlane = new Vector2(screenPlaneSize, screenPlaneSize);

            Console.WriteLine();
            Console.WriteLine("screenPlane = " + screenPlane);
            Console.WriteLine("screenUp = " + screenUp);
            Console.WriteLine("screenRight = " + screenRight);
        }

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                Vector3 normalized = value.Normalized();
                right = (Vector3.Cross(Vector3.UnitZ, normalized)).Normalized() * screenPlaneRange;
                up = (Vector3.Cross(normalized, right)).Normalized() * screenPlaneRange;
            }
        }

        public Vector3 screenUp
        {
            get
            {
                return up;
                // return new Vector3(0, 0, screenPlaneRange);
            }
        }
        public Vector3 screenRight
        {
            get
            {
                return right;
                // return new Vector3(0, screenPlaneRange, 0);
            }
        }
    }
}
