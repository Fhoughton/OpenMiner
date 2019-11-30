// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.Starfield
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner.Graphics
{
  internal class Starfield : IHasInitialization, IHasContent, IUnmanagedBuffer
  {
    private Vector3 pos1 = new Vector3(-3f, -3f, 0.0f);
    private Vector3 pos2 = new Vector3(-3f, 3f, 0.0f);
    private Vector3 pos3 = new Vector3(3f, 3f, 0.0f);
    private Vector3 pos4 = new Vector3(3f, -3f, 0.0f);
    private const float size = 3f;
    private const float radius = 1000f;
    public VertexBuffer VertexBuffer;
    private GameInstance instance;

    public Starfield(GameInstance instance)
    {
      this.instance = instance;
    }

    public virtual void Initialize(InitState state)
    {
    }

    public virtual void LoadContent(InitState state)
    {
      GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
      VertexPositionColor[] vertexPositionColorArray = new VertexPositionColor[this.instance.Random.Next(1000, 1200) * 4];
      this.BuildVertices(vertexPositionColorArray);
      this.VertexBuffer = new VertexBuffer(graphicsDevice, typeof (VertexPositionColor), vertexPositionColorArray.Length, BufferUsage.WriteOnly);
      this.VertexBuffer.SetData<VertexPositionColor>(vertexPositionColorArray, 0, vertexPositionColorArray.Length);
    }

    public virtual void UnloadContent()
    {
      if (this.VertexBuffer == null)
        return;
      this.VertexBuffer.Dispose();
    }

    private void BuildVertices(VertexPositionColor[] vertices)
    {
      int num = vertices.Length / 4;
      for (int i = 0; i < num - 2; ++i)
        this.AddStar(vertices, i);
    }

    private void AddStar(VertexPositionColor[] vertices, int i)
    {
      Vector3 objectPosition = new Vector3();
      float num1 = 6.283185f * (float) this.instance.Random.NextDouble();
      float num2 = (float) Math.Acos(2.0 * this.instance.Random.NextDouble() - 1.0);
      objectPosition.X = 1000f * (float) Math.Cos((double) num1) * (float) Math.Sin((double) num2);
      objectPosition.Y = 1000f * (float) Math.Sin((double) num1) * (float) Math.Sin((double) num2);
      objectPosition.Z = 1000f * (float) Math.Cos((double) num2);
      Matrix billboard = Matrix.CreateBillboard(objectPosition, Vector3.Zero, Vector3.Up, new Vector3?(Vector3.Forward));
      float num3 = (float) (this.instance.Random.NextDouble() * 0.5 + 0.5);
      Color color = new Color(new Vector3(num3, num3, num3));
      this.AddStar(vertices, i, billboard, num3, color);
    }

    private void AddStar(
      VertexPositionColor[] vertices,
      int i,
      Matrix bb,
      float size,
      Color color)
    {
      int index1 = i * 4;
      vertices[index1].Color = color;
      VertexPositionColor[] vertexPositionColorArray1 = vertices;
      int index2 = index1;
      int index3 = index2 + 1;
      vertexPositionColorArray1[index2].Position = Vector3.Transform(this.pos1 * size, bb);
      vertices[index3].Color = color;
      VertexPositionColor[] vertexPositionColorArray2 = vertices;
      int index4 = index3;
      int index5 = index4 + 1;
      vertexPositionColorArray2[index4].Position = Vector3.Transform(this.pos2 * size, bb);
      vertices[index5].Color = color;
      VertexPositionColor[] vertexPositionColorArray3 = vertices;
      int index6 = index5;
      int index7 = index6 + 1;
      vertexPositionColorArray3[index6].Position = Vector3.Transform(this.pos3 * size, bb);
      vertices[index7].Color = color;
      vertices[index7].Position = Vector3.Transform(this.pos4 * size, bb);
    }

    public long BufferSize
    {
      get
      {
        long num = 0;
        if (this.VertexBuffer != null)
          num += this.VertexBuffer.BufferSize();
        return num;
      }
    }
  }
}
