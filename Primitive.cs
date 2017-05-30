using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template
{
    abstract class Primitive
    {
        public Vector3 position;
        public Vector3 color;

        public Bitmap texture;

        public float refractionIndex;
        public float transparency;
        public float reflection;

        public Primitive(Vector3 position, Vector3 color, float refractionIndex = 1, float transparency = 0, float reflection = 0)
        {
            this.position = position;
            this.color = color;

            this.refractionIndex = refractionIndex;
            this.transparency = transparency;
            this.reflection = reflection;


        }

        public abstract Intersection intersectPrimitive(Ray ray);

        public abstract Vector3 colorFromTexture(Vector3 position);
    }
    
    class Sphere : Primitive
    {
        public float radius;

        //to tilt/rotate the sphere (only for textures spheres)
        public float thetaOffset = 0;
        public float phiOffset = 0;

        public static float RadiansToPercentage = (float) (0.5f / Math.PI);

        public Sphere(Vector3 position, float radius, Vector3 color) : base (position, color)
        {
            this.radius = radius;
        }

        public override Intersection intersectPrimitive(Ray ray)
        {
            if( (ray.origin - position).LengthSquared < radius * radius)
            {
                // ray starts inside the sphere
                Vector3 o_c = ray.origin - position;
                float dot = Vector3.Dot(ray.direction, o_c);
                float sqrt = dot * dot - o_c.LengthSquared + radius * radius;
                float d = -dot + (float) Math.Sqrt(sqrt);

                Intersection inter = new Intersection();

                inter.origin = ray.origin;
                inter.direction = ray.direction;

                inter.intersectionPoint = ray.origin + d * ray.direction;

                inter.distance = d;
                inter.normal = (this.position - inter.intersectionPoint).Normalized();
                inter.collider = this;

                return inter;
            }
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
        
        public override Vector3 colorFromTexture(Vector3 position)
        {
            if (this.texture == null)
                throw new Exception("This object (Sphere) does not have a texture");

            Vector3 normal = position - this.position;

            // calculate circular angles theta(x/y) and phi(z)
            float phi = Vector3.CalculateAngle(Vector3.UnitZ, normal);
            float theta = (float) (Math.Atan(normal.Y / normal.X) + Math.PI) + thetaOffset;

            // use those angles to determine x and y on the texture
            int x = (int) ( (1 - theta * RadiansToPercentage) * texture.Width);
            int y = (int) ( 2 * phi * RadiansToPercentage * texture.Height);
            /*
            if (y < 0)
            {
                y = -y;
                x += texture.Width / 2;
            }
            else if (y >= texture.Height)
            {
                y = 2 * texture.Height - y - 1;
                x += texture.Width / 2;
            }
            */
            if (x < 0)
                x += texture.Width;
            else if (x >= texture.Width)
                x -= texture.Width;
            
            Color color = texture.GetPixel(x, y);
            float colorRatio = 1f / 255f;
            return new Vector3(color.R * colorRatio, color.G * colorRatio, color.B * colorRatio);
        }
    }

    class Plane : Primitive
    {
        public Vector3 normal;

        public float textureScale;

        public Plane(Vector3 position, Vector3 normal, Vector3 color) : base (position, color)
        {
            this.normal = normal.Normalized();
            textureScale = 1;
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
            
            i.normal = this.normal.Normalized();
            i.collider = this;

            return i;
        }

        public override Vector3 colorFromTexture(Vector3 position)
        {
            //throw new NotImplementedException();
            int i = 0;
            Vector3 v = this.position - position;
            if (this.position.Z != 0)
                i = 0;
            float dot = Vector3.Dot(Vector3.UnitZ, normal);
            if (dot != 0)
            {
                // not a vertical plane

                Vector3 xVec = Vector3.Cross(Vector3.UnitY, normal);
                Vector3 yVec = Vector3.Cross(normal, xVec);

                float xLength = Vector3.Dot(v, xVec);
                float yLength = Vector3.Dot(v, yVec);

                int x = (int) (textureScale * texture.Width * xLength);
                int y = (int) (textureScale * texture.Height * yLength);

                while (x < 0)
                    x += texture.Width;
                while (y < 0)
                    y += texture.Height;

                x = x % texture.Width;
                y = y % texture.Height;

                Color color = texture.GetPixel(x, y);
                float colorRatio = 1f / 255f;
                return new Vector3(color.R * colorRatio, color.G * colorRatio, color.B * colorRatio);
            }
            else
            {
                // vertical plane
                //throw new Exception("Vertical plane not supported :(");


                Vector3 xVec = Vector3.Cross(Vector3.UnitZ, normal);
                Vector3 yVec = Vector3.Cross(normal, xVec);

                float xLength = Vector3.Dot(v, xVec);
                float yLength = Vector3.Dot(v, yVec);

                int x = (int)(textureScale * texture.Width * xLength);
                int y = (int)(textureScale * texture.Height * yLength);

                while (x < 0)
                    x += texture.Width;
                while (y < 0)
                    y += texture.Height;

                x = x % texture.Width;
                y = y % texture.Height;

                Color color = texture.GetPixel(x, y);
                float colorRatio = 1f / 255f;
                return new Vector3(color.R * colorRatio, color.G * colorRatio, color.B * colorRatio);
            }
        }
    }

    
    class Triangle : Primitive
    {
        public Vector3 normal;

        public Vector3 p2;
        public Vector3 p3;

        //Always define the points counter-clockwise!!!
        public Triangle(Vector3 position, Vector3 p2, Vector3 p3, Vector3 color) : base(position, color)
        {
            this.p2 = p2;
            this.p3 = p3;

            Vector3 p1p2 = p2 - position;
            Vector3 p1p3 = p3 - position;
            this.normal = Vector3.Cross(p1p3, p1p2);
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
            
            // now we know that we at least hit the plane in which the triangle is found,
            // now to check whether the intersection point is within the triangle (normalized)

            Vector3 p1p2 = (p2 - position).Normalized();
            Vector3 p2p3 = (p3 - p2).Normalized();
            Vector3 p3p1 = (position - p3).Normalized();

            // the vectors from the triangle vertices to the intersection point iPoint (normalized too)
            Vector3 p1i = (iPoint - position).Normalized();
            Vector3 p2i = (iPoint - p2).Normalized();
            Vector3 p3i = (iPoint - p3).Normalized();
            
            // the angle at 'position'
            float alpha = (float)Math.Acos(Vector3.Dot(p1p2, -p3p1));
            // the angle at 'position' (between p1p2 and the iPoint)
            float alpha2 = (float)Math.Acos(Vector3.Dot(p1p2, p1i));
            // only if all angles are smaller than the "original" ones AND positive, we are within the triangle
            if (alpha2 > alpha)
                return null;
                
            // the angle at p2
            float beta = (float)Math.Acos(Vector3.Dot(-p1p2, p2p3));
            // the angle at p2 (between p2p3 and the iPoint)
            float beta2 = (float)Math.Acos(Vector3.Dot(p2p3, p2i));
            // only if all angles are smaller than the "original" ones AND positive, we are within the triangle
            if (beta2 > beta)
                return null;
                
            // the angle at p3
            float gamma = (float)Math.Acos(Vector3.Dot(-p2p3, p3p1));
            // the angle at p3 (between p3p1 and the iPoint)
            float gamma2 = (float)Math.Acos(Vector3.Dot(p3p1, p3i));
            // only if all angles are smaller than the "original" ones AND positive, we are within the triangle
            if (gamma2 > gamma)
                return null;
                
            Intersection i = new Intersection(ray, -t);

            i.normal = this.normal;
            i.collider = this;

            return i;
        }

        public override Vector3 colorFromTexture(Vector3 position)
        {
            throw new NotImplementedException();
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
        public enum Refract { Outside, Inside };
        public Refract refract;

        public Vector3 origin;
        public Vector3 direction;

        public bool hitSkybox;
        public float distanceTraveled;
        public Vector3 color;
        
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;

            hitSkybox = false;
            color = Vector3.Zero;
            distanceTraveled = 0;

            refract = Refract.Outside;
        }
    }
}
