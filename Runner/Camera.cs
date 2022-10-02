using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Camera
    {
        public Vector3 Position = new Vector3(2, 1.8f, 0);
        public Vector3 Direction = new Vector3(0, 0, 1);
        public Vector3 Up = Vector3.Up; // 0 1 0 -- points upward
        public float NearPlane = 0.01f;
        public float FarPlane = 32;
        public float AspectRatio = 1;
        public float FOV = MathHelper.Pi / 3;   // 60 degrees

        public Vector3 Target
        {
            get
            {
                return Position + Direction;
            }
            set
            {
                Direction = Vector3.Normalize(value - Position);
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(Direction, Up));
            }
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(Position, Target, Up);
            }
        }

        public Matrix Projection
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(FOV, AspectRatio, NearPlane, FarPlane);
            }
        }
    }
}
