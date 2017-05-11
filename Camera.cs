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
        Vector3 position;
        Vector3 direction;
        Vector2 screenPlane;

        float viewDistance;

        public Camera(Vector3 position, Vector3 direction, float viewDistance)
        {
            this.position = position;
            this.direction = direction;
            this.viewDistance = viewDistance;
            screenPlane = new Vector2(2, 2);



        }

        Vector3 screenUp
        {
            get
            {

            }
        }

    }
}
