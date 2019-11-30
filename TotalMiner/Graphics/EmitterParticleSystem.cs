// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.EmitterParticleSystem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Renderers;
using System;

namespace StudioForge.TotalMiner.Graphics
{
  internal class EmitterParticleSystem
  {
    public float ParticleDuration;
    public int MaxParticles;
    public float MaxDistance;
    public int SpawnedCount;
    private EmitterParticleVertex[] particles;
    private DynamicVertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private int vertsPerParticle;
    private int maxVertices;
    private int firstActiveParticle;
    private int firstNewParticle;
    private int firstFreeParticle;
    private int firstRetiredParticle;
    private int currentSpawnCount;
    private float currentTime;
    private float spawnCountTimer;
    private int drawCounter;
    private MapTM map;

    public bool HasParticlesToDraw
    {
      get
      {
        return this.firstActiveParticle != this.firstFreeParticle;
      }
    }

    public IndexBuffer IndexBuffer
    {
      get
      {
        return this.indexBuffer;
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
      this.ParticleDuration = 8f;
      this.MaxParticles = ThreadQueueManager.Instance.GetProcessorScale(4000, 10000);
      this.vertsPerParticle = 8;
      this.maxVertices = this.MaxParticles * this.vertsPerParticle;
      this.particles = new EmitterParticleVertex[this.maxVertices];
    }

    public void LoadContent()
    {
      this.vertexBuffer = new DynamicVertexBuffer(CoreGlobals.GraphicsDevice, EmitterParticleVertex.VertexDeclaration, this.maxVertices, BufferUsage.WriteOnly);
      for (int index = 0; index < this.MaxParticles; ++index)
      {
        this.particles[index * this.vertsPerParticle].Corner = new Short4(-1f, -1f, 1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 1].Corner = new Short4(-1f, 1f, 1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 2].Corner = new Short4(1f, 1f, 1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 3].Corner = new Short4(1f, -1f, 1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 4].Corner = new Short4(-1f, -1f, -1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 5].Corner = new Short4(-1f, 1f, -1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 6].Corner = new Short4(1f, 1f, -1f, 0.0f);
        this.particles[index * this.vertsPerParticle + 7].Corner = new Short4(1f, -1f, -1f, 0.0f);
      }
      short[] data = new short[this.MaxParticles * 36];
      for (int index1 = 0; index1 < this.MaxParticles; ++index1)
      {
        int index2 = index1 * 36;
        short num = (short) (index1 * 8);
        data[index2] = num;
        data[index2 + 1] = (short) ((int) num + 1);
        data[index2 + 2] = (short) ((int) num + 2);
        data[index2 + 3] = num;
        data[index2 + 4] = (short) ((int) num + 2);
        data[index2 + 5] = (short) ((int) num + 3);
        int index3 = index2 + 6;
        data[index3] = (short) ((int) num + 3);
        data[index3 + 1] = (short) ((int) num + 2);
        data[index3 + 2] = (short) ((int) num + 6);
        data[index3 + 3] = (short) ((int) num + 3);
        data[index3 + 4] = (short) ((int) num + 6);
        data[index3 + 5] = (short) ((int) num + 7);
        int index4 = index3 + 6;
        data[index4] = (short) ((int) num + 7);
        data[index4 + 1] = (short) ((int) num + 6);
        data[index4 + 2] = (short) ((int) num + 5);
        data[index4 + 3] = (short) ((int) num + 7);
        data[index4 + 4] = (short) ((int) num + 5);
        data[index4 + 5] = (short) ((int) num + 4);
        int index5 = index4 + 6;
        data[index5] = (short) ((int) num + 4);
        data[index5 + 1] = (short) ((int) num + 5);
        data[index5 + 2] = (short) ((int) num + 1);
        data[index5 + 3] = (short) ((int) num + 4);
        data[index5 + 4] = (short) ((int) num + 1);
        data[index5 + 5] = num;
        int index6 = index5 + 6;
        data[index6] = (short) ((int) num + 1);
        data[index6 + 1] = (short) ((int) num + 5);
        data[index6 + 2] = (short) ((int) num + 6);
        data[index6 + 3] = (short) ((int) num + 1);
        data[index6 + 4] = (short) ((int) num + 6);
        data[index6 + 5] = (short) ((int) num + 2);
        int index7 = index6 + 6;
        data[index7] = (short) ((int) num + 4);
        data[index7 + 1] = num;
        data[index7 + 2] = (short) ((int) num + 3);
        data[index7 + 3] = (short) ((int) num + 4);
        data[index7 + 4] = (short) ((int) num + 3);
        data[index7 + 5] = (short) ((int) num + 7);
      }
      this.indexBuffer = new IndexBuffer(CoreGlobals.GraphicsDevice, IndexElementSize.SixteenBits, data.Length, BufferUsage.WriteOnly);
      this.indexBuffer.SetData<short>(data);
    }

    public void UnloadContent()
    {
      if (this.vertexBuffer != null)
        this.vertexBuffer.Dispose();
      if (this.indexBuffer != null)
        this.indexBuffer.Dispose();
      this.particles = (EmitterParticleVertex[]) null;
    }

    public void Update()
    {
      this.currentTime += Services.ElapsedTime;
      this.RetireActiveParticles();
      this.FreeRetiredParticles();
      if (this.firstActiveParticle == this.firstFreeParticle)
        this.currentTime = 0.0f;
      if (this.firstRetiredParticle == this.firstActiveParticle)
        this.drawCounter = 0;
      this.spawnCountTimer += Services.ElapsedTime;
      if ((double) this.spawnCountTimer < 1.0)
        return;
      --this.spawnCountTimer;
      this.SpawnedCount = this.currentSpawnCount;
      this.currentSpawnCount = 0;
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
        GraphicStatics.ParticleShader.CurrentTime.SetValue(this.currentTime);
        GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
        graphicsDevice.SetVertexBuffer((VertexBuffer) this.vertexBuffer);
        graphicsDevice.Indices = this.indexBuffer;
        GraphicStatics.ParticleShader.Effect.CurrentTechnique.Passes[0].Apply();
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
      int vertexStride = 64;
      if (this.firstNewParticle < this.firstFreeParticle)
      {
        this.vertexBuffer.SetData<EmitterParticleVertex>(this.firstNewParticle * vertexStride * this.vertsPerParticle, this.particles, this.firstNewParticle * this.vertsPerParticle, (this.firstFreeParticle - this.firstNewParticle) * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
      }
      else
      {
        this.vertexBuffer.SetData<EmitterParticleVertex>(this.firstNewParticle * vertexStride * this.vertsPerParticle, this.particles, this.firstNewParticle * this.vertsPerParticle, (this.MaxParticles - this.firstNewParticle) * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
        if (this.firstFreeParticle > 0)
          this.vertexBuffer.SetData<EmitterParticleVertex>(0, this.particles, 0, this.firstFreeParticle * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
      }
      this.firstNewParticle = this.firstFreeParticle;
    }

    public bool AddParticle(Vector3 pos, ref ParticleData data)
    {
      int index1;
      lock (this.particles)
      {
        int num = this.firstFreeParticle + 1;
        if (num >= this.MaxParticles)
          num = 0;
        if (num == this.firstRetiredParticle)
          return false;
        index1 = this.firstFreeParticle * this.vertsPerParticle;
        this.firstFreeParticle = num;
      }
      pos.X += data.EmitPosOffset.X;
      pos.Y += data.EmitPosOffset.Y;
      pos.Z += data.EmitPosOffset.Z;
      PcgRandom random = this.map.Random;
      Vector3 vector3_1 = new Vector3();
      vector3_1.X = data.EmitPosVariance.X;
      vector3_1.Y = data.EmitPosVariance.Y;
      vector3_1.Z = data.EmitPosVariance.Z;
      if ((double) vector3_1.X != 0.0)
        pos.X += (float) (random.NextDouble() * (double) vector3_1.X * 2.0) - vector3_1.X;
      if ((double) vector3_1.Y != 0.0)
        pos.Y += (float) (random.NextDouble() * (double) vector3_1.Y * 2.0) - vector3_1.Y;
      if ((double) vector3_1.Z != 0.0)
        pos.Z += (float) (random.NextDouble() * (double) vector3_1.Z * 2.0) - vector3_1.Z;
      Vector3 vector3_2 = new Vector3();
      vector3_2.X = data.Velocity.X;
      vector3_2.Y = data.Velocity.Y;
      vector3_2.Z = data.Velocity.Z;
      Vector3 vector3_3 = new Vector3();
      vector3_3.X = data.VelocityVariance.X;
      vector3_3.Y = data.VelocityVariance.Y;
      vector3_3.Z = data.VelocityVariance.Z;
      if ((double) vector3_3.X != 0.0)
        vector3_2.X += (float) (random.NextDouble() * (double) vector3_3.X * 2.0) - vector3_3.X;
      if ((double) vector3_3.Y != 0.0)
        vector3_2.Y += (float) (random.NextDouble() * (double) vector3_3.Y * 2.0) - vector3_3.Y;
      if ((double) vector3_3.Z != 0.0)
        vector3_2.Z += (float) (random.NextDouble() * (double) vector3_3.Z * 2.0) - vector3_3.Z;
      HalfVector4 halfVector4 = new HalfVector4(data.Size.X * 64f, data.Size.Y * 64f, data.Size.Z * 64f, data.Size.W);
      float num1 = (float) data.Duration / 1000f;
      short gravity = data.Gravity;
      this.particles[index1].Corner = new Short4(-1f, -1f, 1f, (float) gravity);
      this.particles[index1 + 1].Corner = new Short4(-1f, 1f, 1f, (float) gravity);
      this.particles[index1 + 2].Corner = new Short4(1f, 1f, 1f, (float) gravity);
      this.particles[index1 + 3].Corner = new Short4(1f, -1f, 1f, (float) gravity);
      this.particles[index1 + 4].Corner = new Short4(-1f, -1f, -1f, (float) gravity);
      this.particles[index1 + 5].Corner = new Short4(-1f, 1f, -1f, (float) gravity);
      this.particles[index1 + 6].Corner = new Short4(1f, 1f, -1f, (float) gravity);
      this.particles[index1 + 7].Corner = new Short4(1f, -1f, -1f, (float) gravity);
      for (int index2 = 0; index2 < this.vertsPerParticle; ++index2)
      {
        EmitterParticleVertex particle = this.particles[index2 + index1];
        particle.Position.X = pos.X;
        particle.Position.Y = pos.Y;
        particle.Position.Z = pos.Z;
        particle.Velocity.X = vector3_2.X;
        particle.Velocity.Y = vector3_2.Y;
        particle.Velocity.Z = vector3_2.Z;
        particle.Size.PackedValue = halfVector4.PackedValue;
        particle.Rotation.X = data.Rotation;
        particle.Rotation.Y = data.WindFactor;
        particle.Time.X = this.currentTime;
        particle.Time.Y = num1;
        particle.Color1 = data.StartColor;
        particle.Color2 = data.EndColor;
        this.particles[index2 + index1] = particle;
      }
      ++this.currentSpawnCount;
      return true;
    }
  }
}
