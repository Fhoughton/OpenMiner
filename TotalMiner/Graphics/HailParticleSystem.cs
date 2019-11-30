// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.HailParticleSystem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Renderers;
using System;

namespace StudioForge.TotalMiner.Graphics
{
  internal class HailParticleSystem
  {
    public float ParticleDuration;
    public int MaxParticles;
    public float MaxDistance;
    private HailParticleVertex[] particles;
    private DynamicVertexBuffer vertexBuffer;
    private int vertsPerParticle;
    private int maxVertices;
    private int firstActiveParticle;
    private int firstNewParticle;
    private int firstFreeParticle;
    private int firstRetiredParticle;
    private float currentTime;
    private int drawCounter;
    private MapTM map;

    public bool HasParticlesToDraw
    {
      get
      {
        return this.firstActiveParticle != this.firstFreeParticle;
      }
    }

    public int ManagedMemoryUsed
    {
      get
      {
        return this.particles.Length * 64;
      }
    }

    public int UnmanagedMemoryUsed
    {
      get
      {
        return this.vertexBuffer.VertexCount * 64;
      }
    }

    public void Initialize(MapTM map)
    {
      this.map = map;
      this.ParticleDuration = 2.5f;
      this.MaxDistance = (float) ThreadQueueManager.Instance.GetProcessorScale(25, 30);
      this.MaxParticles = Math.Min((int) ((double) (ThreadQueueManager.Instance.GetProcessorScale(20, 30) * 60) * (double) this.ParticleDuration), GameInstance.Instance.EmitterParticleSystem.MaxParticles);
      this.vertsPerParticle = 8;
      this.maxVertices = this.MaxParticles * this.vertsPerParticle;
      this.particles = new HailParticleVertex[this.maxVertices];
      GraphicStatics.HailShader.MaxDistance.SetValue(this.MaxDistance);
    }

    public void LoadContent()
    {
      this.vertexBuffer = new DynamicVertexBuffer(CoreGlobals.GraphicsDevice, HailParticleVertex.VertexDeclaration, this.maxVertices, BufferUsage.WriteOnly);
    }

    public void UnloadContent()
    {
      if (this.vertexBuffer == null)
        return;
      this.vertexBuffer.Dispose();
    }

    public void Update()
    {
      this.currentTime += Services.ElapsedTime;
      this.RetireActiveParticles();
      this.FreeRetiredParticles();
      if (this.firstActiveParticle == this.firstFreeParticle)
        this.currentTime = 0.0f;
      if (this.firstRetiredParticle != this.firstActiveParticle)
        return;
      this.drawCounter = 0;
    }

    private void RetireActiveParticles()
    {
      while (this.firstActiveParticle != this.firstNewParticle && (double) (this.currentTime - this.particles[this.firstActiveParticle * this.vertsPerParticle].Time.X) >= (double) this.ParticleDuration)
      {
        this.particles[this.firstActiveParticle * this.vertsPerParticle].Time.X = (float) this.drawCounter;
        ++this.firstActiveParticle;
        if (this.firstActiveParticle >= this.MaxParticles)
          this.firstActiveParticle = 0;
      }
    }

    private void FreeRetiredParticles()
    {
      while (this.firstRetiredParticle != this.firstActiveParticle && this.drawCounter - (int) this.particles[this.firstRetiredParticle * this.vertsPerParticle].Time.X >= 3)
      {
        ++this.firstRetiredParticle;
        if (this.firstRetiredParticle >= this.MaxParticles)
          this.firstRetiredParticle = 0;
      }
    }

    public void Draw(Player player, Player virtualPlayer, MapRenderer renderer)
    {
      if (this.firstNewParticle != this.firstFreeParticle)
        this.AddNewParticlesToVertexBuffer();
      if (this.firstActiveParticle != this.firstFreeParticle)
      {
        GraphicStatics.HailShader.CurrentTime.SetValue(this.currentTime);
        GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
        graphicsDevice.SetVertexBuffer((VertexBuffer) this.vertexBuffer);
        graphicsDevice.Indices = GameInstance.Instance.EmitterParticleSystem.IndexBuffer;
        GraphicStatics.HailShader.Effect.CurrentTechnique.Passes[0].Apply();
        if (this.firstActiveParticle < this.firstFreeParticle)
        {
          int num = this.firstFreeParticle - this.firstActiveParticle;
          graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.firstActiveParticle * this.vertsPerParticle, 0, num * this.vertsPerParticle, 0, num * 12);
        }
        else
        {
          Math.Max(this.MaxParticles - this.firstActiveParticle, this.firstFreeParticle);
          int num = this.MaxParticles - this.firstActiveParticle;
          graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.firstActiveParticle * this.vertsPerParticle, 0, num * this.vertsPerParticle, 0, num * 12);
          if (this.firstFreeParticle > 0)
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.firstFreeParticle * this.vertsPerParticle, 0, this.firstFreeParticle * 12);
        }
      }
      ++this.drawCounter;
    }

    private void AddNewParticlesToVertexBuffer()
    {
      int vertexStride = 28;
      if (this.firstNewParticle < this.firstFreeParticle)
      {
        this.vertexBuffer.SetData<HailParticleVertex>(this.firstNewParticle * vertexStride * this.vertsPerParticle, this.particles, this.firstNewParticle * this.vertsPerParticle, (this.firstFreeParticle - this.firstNewParticle) * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
      }
      else
      {
        this.vertexBuffer.SetData<HailParticleVertex>(this.firstNewParticle * vertexStride * this.vertsPerParticle, this.particles, this.firstNewParticle * this.vertsPerParticle, (this.MaxParticles - this.firstNewParticle) * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
        if (this.firstFreeParticle > 0)
          this.vertexBuffer.SetData<HailParticleVertex>(0, this.particles, 0, this.firstFreeParticle * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
      }
      this.firstNewParticle = this.firstFreeParticle;
    }

    public bool AddParticle(
      Vector3 position,
      float velocity,
      float endY,
      float size,
      Color color)
    {
      int num1 = this.firstFreeParticle + 1;
      if (num1 >= this.MaxParticles)
        num1 = 0;
      if (num1 == this.firstRetiredParticle)
        return false;
      int index1 = this.firstFreeParticle * this.vertsPerParticle;
      float num2 = size * 0.5f;
      float y = position.Y - endY;
      color.A = (byte) this.map.Random.Next((int) byte.MaxValue);
      position.X -= num2;
      position.Z += num2;
      this.particles[index1].Position = new Vector4(position, velocity);
      this.particles[index1].Time = new Vector2(this.currentTime, y);
      this.particles[index1].Color = color;
      int index2 = index1 + 1;
      this.particles[index2] = this.particles[index2 - 1];
      this.particles[index2].Position.Y += size;
      int index3 = index2 + 1;
      this.particles[index3] = this.particles[index3 - 1];
      this.particles[index3].Position.X += size;
      int index4 = index3 + 1;
      this.particles[index4] = this.particles[index4 - 1];
      this.particles[index4].Position.Y -= size;
      int index5 = index4 + 1;
      this.particles[index5] = this.particles[index5 - 4];
      this.particles[index5].Position.Z -= size;
      int index6 = index5 + 1;
      this.particles[index6] = this.particles[index6 - 1];
      this.particles[index6].Position.Y += size;
      int index7 = index6 + 1;
      this.particles[index7] = this.particles[index7 - 1];
      this.particles[index7].Position.X += size;
      int index8 = index7 + 1;
      this.particles[index8] = this.particles[index8 - 1];
      this.particles[index8].Position.Y -= size;
      this.firstFreeParticle = num1;
      return true;
    }
  }
}
