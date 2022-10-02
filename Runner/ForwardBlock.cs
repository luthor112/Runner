using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class ForwardBlock : IBlock
    {
        Vector3 position;
        Vector3 colour;
        bool hasObstacle;
        bool hasTarget;
        Dictionary<string, Texture2D> textures;

        VertexBuffer vb;
        BasicEffect effect;
        Matrix world;

        bool horizontalObstacle;
        Vector3 obstacleStart;
        Vector3 obstacleEnd;
        Vector3 targetStart;
        Vector3 targetEnd;

        public ForwardBlock(Vector3 position, Vector3 colour, bool hasObstacle, bool hasTarget, Dictionary<string, Texture2D> textures, GraphicsDevice device)
        {
            this.position = position;
            this.colour = colour;
            this.hasObstacle = hasObstacle;
            this.hasTarget = hasTarget;
            this.textures = textures;

            world = Matrix.CreateTranslation(position);

            vb = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, 20, BufferUsage.WriteOnly);
            var data = new VertexPositionNormalTexture[20] {
                // Left
                new VertexPositionNormalTexture(new Vector3(0, 4, 4), new Vector3(1, 0, 0), new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(0, 4, 0), new Vector3(1, 0, 0), new Vector2(0, 1)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 4), new Vector3(1, 0, 0), new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector2(1, 1)),

                // Bottom
                new VertexPositionNormalTexture(new Vector3(4, 0, 4), new Vector3(0, 1, 0), new Vector2(0, 0)),
                new VertexPositionNormalTexture(new Vector3(4, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)),

                // Right
                new VertexPositionNormalTexture(new Vector3(4, 4, 4), new Vector3(-1, 0, 0), new Vector2(1, 0)),
                new VertexPositionNormalTexture(new Vector3(4, 4, 0), new Vector3(-1, 0, 0), new Vector2(1, 1)),

                // Placeholder for obstacle
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),

                // Placeholder for target
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
                new VertexPositionNormalTexture(),
            };

            if (hasObstacle)
            {
                Random rnd = new Random();
                horizontalObstacle = rnd.Next(10) < 5;
                int posUp = rnd.Next(4);
                int posLeft = rnd.Next(4);

                if (horizontalObstacle)
                {
                    var obstacleData = new VertexPositionNormalTexture[8] {
                        // Top
                        new VertexPositionNormalTexture(new Vector3(4, 1, 1), new Vector3(0, 1, 0), new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector2(0, 1)),
                        new VertexPositionNormalTexture(new Vector3(4, 1, 0), new Vector3(0, 1, 0), new Vector2(0.25f, 0)),
                        new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector2(0.25f, 1)),

                        // Center
                        new VertexPositionNormalTexture(new Vector3(4, 0, 0), new Vector3(0, 0, -1), new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector2(0, 1)),

                        // Bottom
                        new VertexPositionNormalTexture(new Vector3(4, 0, 1), new Vector3(0, -1, 0), new Vector2(0.25f, 0)),
                        new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(0, -1, 0), new Vector2(0.25f, 1)),
                    };

                    Vector3 displacement = new Vector3(0, posLeft, posUp);

                    for (int i = 0; i < 8; i++)
                    {
                        obstacleData[i] = new VertexPositionNormalTexture(
                            obstacleData[i].Position + displacement,
                            obstacleData[i].Normal,
                            obstacleData[i].TextureCoordinate);
                    }

                    obstacleStart = obstacleData[5].Position + position;
                    obstacleEnd = obstacleData[0].Position + position;

                    obstacleData.CopyTo(data, 8);
                }
                else
                {
                    var obstacleData = new VertexPositionNormalTexture[8] {
                        // Left
                        new VertexPositionNormalTexture(new Vector3(0, 4, 1), new Vector3(-1, 0, 0), new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(-1, 0, 0), new Vector2(0, 1)),
                        new VertexPositionNormalTexture(new Vector3(0, 4, 0), new Vector3(-1, 0, 0), new Vector2(0.25f, 0)),
                        new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector2(0.25f, 1)),

                        // Center
                        new VertexPositionNormalTexture(new Vector3(1, 4, 0), new Vector3(0, 0, -1), new Vector2(0, 0)),
                        new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector2(0, 1)),

                        // Right
                        new VertexPositionNormalTexture(new Vector3(1, 4, 1), new Vector3(1, 0, 0), new Vector2(0.25f, 0)),
                        new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector2(0.25f, 1)),
                    };

                    Vector3 displacement = new Vector3(posLeft, 0, posUp);

                    for (int i = 0; i < 8; i++)
                    {
                        obstacleData[i] = new VertexPositionNormalTexture(
                            obstacleData[i].Position + displacement,
                            obstacleData[i].Normal,
                            obstacleData[i].TextureCoordinate);
                    }

                    obstacleStart = obstacleData[3].Position + position;
                    obstacleEnd = obstacleData[6].Position + position;

                    obstacleData.CopyTo(data, 8);
                }
            }

            if (hasTarget)
            {
                Random rnd = new Random();

                Vector3 displacement = new Vector3(rnd.Next(8) / 2f, rnd.Next(4) / 2f, rnd.Next(4) / 2f - 0.1f);

                var targetData = new VertexPositionNormalTexture[4] {
                    new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), new Vector3(0, 0, -1), new Vector2(0, 0)),
                    new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector2(0, 1)),
                    new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, -1), new Vector2(1, 0)),
                    new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 0, -1), new Vector2(1, 1)),
                };

                for (int i = 0; i < 4; i++)
                {
                    targetData[i] = new VertexPositionNormalTexture(
                        targetData[i].Position + displacement,
                        targetData[i].Normal,
                        targetData[i].TextureCoordinate);
                }

                targetStart = targetData[1].Position + position;
                targetEnd = targetData[2].Position + position;

                targetData.CopyTo(data, 16);
            }

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
            if (hasObstacle)
            {
                vb.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 6);
            }
            if (hasTarget)
            {
                effect.Texture = textures["targetTexture"];
                effect.CurrentTechnique.Passes[0].Apply();
                vb.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 16, 2);
                effect.Texture = textures["defaultTexture"];
                effect.CurrentTechnique.Passes[0].Apply();
            }
        }

        public bool Collision(Vector3 playerPos)
        {
            if (playerPos.X <= position.X || playerPos.X >= position.X + 4)
            {
                return true;
            }
            else if (hasObstacle)
            {
                if (horizontalObstacle)
                {
                    if (playerPos.Y >= obstacleStart.Y && (playerPos.Y - 1.8f) <= obstacleEnd.Y
                        && playerPos.Z >= obstacleStart.Z && playerPos.Z <= obstacleEnd.Z)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (playerPos.X >= obstacleStart.X && playerPos.X <= obstacleEnd.X
                        && playerPos.Z >= obstacleStart.Z && playerPos.Z <= obstacleEnd.Z)
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ShotCollision(Vector3 playerPos, Vector3 playerDirection)
        {
            if (!hasTarget)
            {
                return false;
            }
            else
            {
                float k = (targetStart.Z - playerPos.Z) / playerDirection.Z;
                Vector3 inPlane = playerPos + k * playerDirection;
                if (inPlane.X >= targetStart.X && inPlane.X <= targetEnd.X
                    && inPlane.Y >= targetStart.Y && inPlane.Y <= targetEnd.Y)
                {
                    hasTarget = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector3 GetNextPosition()
        {
            return position + new Vector3(0, 0, 4);
        }
    }
}
