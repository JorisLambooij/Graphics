using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template
{
    abstract class Primitive
    {
        public Vector3 position;
        public Vector3 color;

        public Primitive(Vector3 position, Vector3 color)
        {
            this.position = position;
            this.color = color;
        }

        public abstract Intersection intersectPrimitive(Ray ray);
    }

    class Sphere : Primitive
    {
        public float radius;

        public Sphere(Vector3 position, float radius, Vector3 color) : base (position, color)
        {
            this.radius = radius;
        }

        public override Intersection intersectPrimitive(Ray ray)
        {
            Vector3 c = position - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = Vector3.Dot(q, q);
            if (p2 > radius * radius || t <= 0)
            {
                return null;
            }
            t -= (float) Math.Sqrt(radius * radius - p2);

            Intersection i = new Intersection();

            i.origin = ray.origin;
            i.direction = ray.direction;

            //i.intersectionPoint = this.position + q;
            i.intersectionPoint = ray.origin + t * ray.direction;


            i.distance = (i.intersectionPoint - i.origin).Length;

            i.normal = (i.intersectionPoint - this.position).Normalized();
            i.collider = this;

            return i;
        }
    }

    class Plane : Primitive
    {
        public Vector3 normal;

        public Plane(Vector3 position, Vector3 normal, Vector3 color) : base (position, color)
        {
            this.normal = normal;
        }

        public override Intersection intersectPrimitive(Ray ray)
        {
            // the intersection point
            Vector3 iPoint;
            // the length of the ray from origin to intersection point
            float d = -Vector3.Dot(normal, position);
            float t = (Vector3.Dot(normal, ray.origin) + d)/ Vector3.Dot(normal, ray.direction);
            iPoint = ray.origin + -t * ray.direction;

            // wrong direction, return null
            if (t >= 0)
                return null;

            Intersection i = new Intersection(ray, -t);
            
            i.normal = this.normal;
            i.collider = this;

            return i;
        }
    }
    class Triangle : Primitive
    {
        public Vector3 normal;

        public Triangle(Vector3 position, Vector3 normal, Vector2 p1, Vector2 p2, Vector2 p3, Vector3 color) : base(position, color)
        {
            this.normal = normal;
        }

        public override Intersection intersectPrimitive(Ray ray)
        {
            // the intersection point
            Vector3 iPoint;
            // the length of the ray from origin to intersection point
            float d = -Vector3.Dot(normal, position);
            float t = (Vector3.Dot(normal, ray.origin) + d) / Vector3.Dot(normal, ray.direction);
            iPoint = ray.origin + -t * ray.direction;

            // wrong direction, return null
            if (t >= 0)
                return null;

            Intersection i = new Intersection(ray, -t);

            i.normal = this.normal;
            i.collider = this;

            return i;
        }
    }

    class Intersection
    {
        public Vector3 origin;
        public Vector3 direction;

        public Vector3 intersectionPoint;
        public Vector3 normal;

        public Primitive collider;
        //protected Vector3 color;

        public float distance;
        
        public Intersection()
        {
            //distance = float.PositiveInfinity;
        }

        public Intersection( Ray ray, float distance )
        {
            this.distance = distance;
            this.origin = ray.origin;
            this.direction = ray.direction;
            this.intersectionPoint = ray.origin + distance * ray.direction;
        }

        public Vector3 Color
        {
            get
            {
                if (collider != null)
                    return collider.color / (distance * distance);
                else
                    return Vector3.Zero;
            }
        }
    }

    class Light
    {
        public Vector3 position;
        public float intensity;
        public Vector3 color;

        public Light(Vector3 position, float intensity, Vector3 color)
        {
            this.position = position;
            this.intensity = intensity;
            this.color = color;
        }
    }

    class Ray
    {
        public Vector3 origin;
        public Vector3 direction;
        public float distance;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
}
