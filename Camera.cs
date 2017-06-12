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
        Vector3 direction;
        public Vector2 screenPlane;
        public float aspectRatio = 1;

        Vector3 right, up;

        public float screenPlaneSize, xScreenPlaneRange, yScreenPlaneRange;
        float viewDistance, angle;

        public Camera(Vector3 position, Vector3 direction, float angle, float aspectRatio)
        {
            this.position = position;
            this.angle = angle;
            this.aspectRatio = aspectRatio;

            //Constante afstand van camera tot screenPlane.
            viewDistance = 1f;

            xScreenPlaneRange = (float)Math.Tan(angle / 2f) * viewDistance;
            yScreenPlaneRange = xScreenPlaneRange / aspectRatio;

            Direction = direction;

            //ScreenPlaneSize();

            //this.right = (Vector3.Cross(Vector3.UnitZ, direction)).Normalized() * xScreenPlaneRange;
            //this.up = (Vector3.Cross(direction, right)).Normalized() * yScreenPlaneRange;
        }

        void ScreenPlaneSize()
        {
            //screenPlaneRange geeft aan hoe lang elke kant van een as is.
            //Bijvoorbeeld:
            //screenPlaneRange - 2 --> x-as gaat van -2 tot 2 en y-as gaat van -2 tot 2.

            //screenPlaneSize geeft aan hoe breed en hoog de screenPlane in totaal is.
            screenPlaneSize = xScreenPlaneRange * 2f;

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
                if(normalized.X == 0 && normalized.Y == 0 && (normalized.Z == 1 || normalized.Z == -1))
                {
                    if (normalized.Z == 1)
                        Direction = new Vector3(0.1f, 0, 1);
                    else
                        Direction = new Vector3(0.1f, 0, -1);
                }
                else
                {
                    right = (Vector3.Cross(Vector3.UnitZ, normalized)).Normalized() * xScreenPlaneRange;
                    up = (Vector3.Cross(normalized, right)).Normalized() * yScreenPlaneRange;
                    direction = normalized;
                }
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
