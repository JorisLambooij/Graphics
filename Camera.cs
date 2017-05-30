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

        float viewDistance;

        public Camera(Vector3 position, Vector3 direction, float viewDistance)
        {
            this.position = position;
            this.direction = direction;
            this.viewDistance = viewDistance;
            screenPlane = new Vector2(2, 2);
        }

        public Vector3 screenUp
        {
            get
            {
                return new Vector3(0, 0, 1);
            }
        }
        public Vector3 screenRight
        {
            get
            {
                return new Vector3(0, 1.0f, 0);
            }
        }
    }
}
