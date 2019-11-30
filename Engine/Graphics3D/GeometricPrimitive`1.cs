// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.GeometricPrimitive`1
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public abstract class GeometricPrimitive<V> : IRecycled, IHasInitialization, IGeometricPrimitive, IDisposable
    where V : struct
  {
    private List<V> vertices = new List<V>();
    private List<ushort> indices = new List<ushort>();
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private BasicEffect basicEffect;
    private bool batch;

    public BasicEffect BasicEffect
    {
      get
      {
        return this.basicEffect;
      }
    }

    public VertexBuffer VertexBuffer
    {
      get
      {
        return this.vertexBuffer;
      }
    }

    public IndexBuffer IndexBuffer
    {
      get
      {
        return this.indexBuffer;
      }
    }

    public bool IsRecyclable { get; set; }

    public void Initialize(InitState state)
    {
    }

    protected GeometricPrimitive(BasicEffect effect)
    {
      this.batch = effect != null;
      this.basicEffect = effect;
    }

    protected void AddVertex(V vert)
    {
      this.vertices.Add(vert);
    }

    protected void AddIndex(int index)
    {
      if (index > (int) ushort.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (index));
      this.indices.Add((ushort) index);
    }

    protected int CurrentVertex
    {
      get
      {
        return this.vertices.Count;
      }
    }

    protected void InitializePrimitive(GraphicsDevice graphicsDevice)
    {
      this.vertexBuffer = new VertexBuffer(graphicsDevice, typeof (V), this.vertices.Count, BufferUsage.None);
      this.vertexBuffer.SetData<V>(this.vertices.ToArray());
      this.indexBuffer = new IndexBuffer(graphicsDevice, typeof (ushort), this.indices.Count, BufferUsage.None);
      this.indexBuffer.SetData<ushort>(this.indices.ToArray());
      if (this.batch)
        return;
      this.basicEffect = new BasicEffect(graphicsDevice);
    }

    ~GeometricPrimitive()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.vertexBuffer != null)
        this.vertexBuffer.Dispose();
      if (this.indexBuffer != null)
        this.indexBuffer.Dispose();
      if (this.basicEffect == null)
        return;
      this.basicEffect.Dispose();
    }

    public void Draw(BasicEffect effect, StudioForge.Engine.Integration.GlobalBasicEffectUpdate update)
    {
      if (update != null)
        update(effect);
      this.Draw((Effect) effect);
    }

    public void Draw(Effect effect)
    {
      GraphicsDevice graphicsDevice = effect.GraphicsDevice;
      graphicsDevice.SetVertexBuffer(this.vertexBuffer);
      graphicsDevice.Indices = this.indexBuffer;
      for (int index = 0; index < effect.CurrentTechnique.Passes.Count; ++index)
      {
        effect.CurrentTechnique.Passes[index].Apply();
        int primitiveCount = this.indices.Count / 3;
        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Count, 0, primitiveCount);
      }
      ++CoreGlobals.FrameRateCounter.DrawCalls;
    }

    public void Draw(Matrix world, ICamera camera, Color color, StudioForge.Engine.Integration.GlobalBasicEffectUpdate update)
    {
      if (this.basicEffect.FogEnabled = camera.FogEnabled)
      {
        this.basicEffect.FogColor = camera.LenseColor.ToVector3();
        this.basicEffect.FogEnd = camera.FarClip;
        this.basicEffect.FogStart = camera.FogStart;
      }
      this.Draw(world, camera.ViewMatrix, camera.ProjectionMatrix, color, update);
    }

    public void Draw(Matrix world, Color color, StudioForge.Engine.Integration.GlobalBasicEffectUpdate update)
    {
      this.Draw(world, CoreGlobals.Camera, color, update);
    }

    public void Draw(
      Matrix world,
      Matrix view,
      Matrix projection,
      Color color,
      StudioForge.Engine.Integration.GlobalBasicEffectUpdate update)
    {
      this.basicEffect.World = world;
      this.basicEffect.View = view;
      this.basicEffect.Projection = projection;
      this.basicEffect.DiffuseColor = color.ToVector3();
      this.basicEffect.Alpha = (float) color.A / (float) byte.MaxValue;
      this.Draw(this.basicEffect, update);
    }
  }
}
