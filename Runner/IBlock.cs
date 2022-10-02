using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    interface IBlock
    {
        void Draw(Camera cam);
        bool Collision(Vector3 playerPos);
        bool ShotCollision(Vector3 playerPos, Vector3 playerDirection);
        Vector3 GetPosition();
        Vector3 GetNextPosition();
    }
}
