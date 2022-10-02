using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class DiagonalBlock : IBlock
    {
        Vector3 position;
        Vector3 colour;
        bool goRight;
        Dictionary<string, Texture2D> textures;

        VertexBuffer vb;
        BasicEffect effect;
        Matrix world;

        public DiagonalBlock(Vector3 position, Vector3 colour, bool goRight, Dictionary<string, Texture2D> textures, GraphicsDevice device)
        {
            this.position = position;
            this.colour = colour;
            this.goRight = goRight;
            this.textures = textures;

            world = Matrix.CreateTranslation(position);

            Vector3 leftNormal = Vector3.Normalize(new Vector3(4, 0, 2));
            Vector3 rightNormal = Vector3.Normalize(new Vector3(-4, 0, -2));

            vb = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, 8, BufferUsage.WriteOnly);
            var data = new VertexPositionNormalTexture[8] {
                // Left
                new VertexPositionNormalTexture(new Vector3(0, 4, 4), leftNormal, new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(0, 4, 0), leftNormal, new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 4), leftNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), leftNormal, new Vector2(1, 1)),

                // Bottom
                new VertexPositionNormalTexture(new Vector3(4, 0, 4), new Vector3(0, 1, 0), new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(4, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)),

                // Right
                new VertexPositionNormalTexture(new Vector3(4, 4, 4), rightNormal, new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(4, 4, 0), rightNormal, new Vector2(1, 1)),
            };

            Vector3 displacement;
            if (!goRight)
                displacement = new Vector3(-2, 0, 0);
            else
                displacement = new Vector3(2, 0, 0);

            data[0] = new VertexPositionNormalTexture(data[0].Position + displacement, data[0].Normal, data[0].TextureCoordinate);
            data[2] = new VertexPositionNormalTexture(data[2].Position + displacement, data[2].Normal, data[2].TextureCoordinate);
            data[4] = new VertexPositionNormalTexture(data[4].Position + displacement, data[4].Normal, data[4].TextureCoordinate);
            data[6] = new VertexPositionNormalTexture(data[6].Position + displacement, data[6].Normal, data[6].TextureCoordinate);

            vb.SetData(data);

            // Effect
            effect = new BasicEffect(device);
            effect.TextureEnabled = true;
            effect.Texture = textures["defaultTexture"];

            effect.EmissiveColor = colour;
            effect.DiffuseColor = colour;

            //Fog
            effect.FogEnabled = true;
            effect.FogColor = new Vector3(0, 0, 0);
            effect.FogStart = 8;
            effect.FogEnd = 12;

            effect.World = world;
        }

        public void Draw(Camera cam)
        {
            var device = vb.GraphicsDevice;
            effect.View = cam.View;
            effect.Projection = cam.Projection;
            device.SetVertexBuffer(vb);
            effect.CurrentTechnique.Passes[0].Apply();

            vb.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
        }

        public bool Collision(Vector3 playerPos)
        {
            if (!goRight)
            {
                if (Vector3.Cross(new Vector3(-2, 0, 4), playerPos - position).Y > 0
                    && Vector3.Cross(new Vector3(-2, 0, 4), playerPos - (position + new Vector3(4, 0, 0))).Y < 0)
                    return false;
                else
                    return true;
            }
            else
            {
                if (Vector3.Cross(new Vector3(2, 0, 4), playerPos - position).Y > 0
                    && Vector3.Cross(new Vector3(2, 0, 4), playerPos - (position + new Vector3(4, 0, 0))).Y < 0)
                    return false;
                else
                    return true;
            }
        }

        public bool ShotCollision(Vector3 playerPos, Vector3 playerDirection)
        {
            return false;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector3 GetNextPosition()
        {
            if (!goRight)
                return position + new Vector3(-2, 0, 4);
            else
                return position + new Vector3(2, 0, 4);
        }
    }
}
