using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Flier
    {
        Vector3 position;
        float length;
        float speed;

        VertexBuffer vb;
        BasicEffect effect;
        Matrix world;

        public Flier(Vector3 playerPosition, float playerSpeed, GraphicsDevice device)
        {
            Random rnd = new Random();

            length = 1 + (float)rnd.NextDouble() * 11;
            Vector3 displacement = new Vector3((float)rnd.NextDouble() * 16 - 8, 4.5f + (float)rnd.NextDouble() * 4, -(length+1));
            position = playerPosition + displacement;

            speed = MathHelper.Max(2 * playerSpeed, 2 * playerSpeed + (float)rnd.NextDouble() * 0.01f);

            Color colour = new Color(new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

            world = Matrix.CreateTranslation(position);

            vb = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 4, BufferUsage.WriteOnly);
            var data = new VertexPositionColor[4] {
                new VertexPositionColor(new Vector3(0, 0, 0), Color.Black),
                new VertexPositionColor(new Vector3(0, 0, length), colour),
                new VertexPositionColor(new Vector3(0.1f, 0, length), Color.Black),
                new VertexPositionColor(new Vector3(0.1f, 0, 0), colour),
            };

            vb.SetData(data);

            // Effect
            effect = new BasicEffect(device);

            effect.VertexColorEnabled = true;

            //Fog
            effect.FogEnabled = true;
            effect.FogColor = new Vector3(0, 0, 0);
            effect.FogStart = 16;
            effect.FogEnd = 20;

            effect.World = world;
        }

        public void Draw(Camera cam)
        {
            var device = vb.GraphicsDevice;
            effect.View = cam.View;
            effect.Projection = cam.Projection;
            device.SetVertexBuffer(vb);
            effect.World = world;
            effect.CurrentTechnique.Passes[0].Apply();

            vb.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
        }

        public void Step(int dT)
        {
            position.Z += dT * speed;
            world = Matrix.CreateTranslation(position);
        }

        public bool IsFarAway(Vector3 playerPosition)
        {
            return Vector3.Distance(playerPosition, position) > 24;
        }
    }
}
