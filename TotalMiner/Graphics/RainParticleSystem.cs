// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.RainParticleSystem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.Engine;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Graphics
{
  internal class RainParticleSystem
  {
    public float ParticleDuration;
    public int MaxParticles;
    public float MaxDistance;
    private RainParticleVertex[] particles;
    private DynamicVertexBuffer vertexBuffer;
    private int vertsPerParticle;
    private int maxVertices;
    private int firstActiveParticle;
    private int firstNewParticle;
    private int firstFreeParticle;
    private int firstRetiredParticle;
    private float currentTime;
    private int drawCounter;

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

    public void Initialize()
    {
      this.ParticleDuration = 3f;
      this.MaxDistance = (float) ThreadQueueManager.Instance.GetProcessorScale(30, 45);
      this.MaxParticles = (int) ((double) (ThreadQueueManager.Instance.GetProcessorScale(75, 200) * 60) * (double) this.ParticleDuration);
      this.vertsPerParticle = 2;
      this.maxVertices = this.MaxParticles * this.vertsPerParticle;
      this.particles = new RainParticleVertex[this.maxVertices];
      GraphicStatics.RainShader.MaxDistance.SetValue(this.MaxDistance);
    }

    public void LoadContent()
    {
      this.vertexBuffer = new DynamicVertexBuffer(CoreGlobals.GraphicsDevice, RainParticleVertex.VertexDeclaration, this.maxVertices, BufferUsage.WriteOnly);
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
      while (this.firstActiveParticle != this.firstNewParticle && (double) (this.currentTime - this.particles[this.firstActiveParticle * this.vertsPerParticle].UserData.X) >= (double) this.ParticleDuration)
      {
        this.particles[this.firstActiveParticle * this.vertsPerParticle].UserData.X = (float) this.drawCounter;
        ++this.firstActiveParticle;
        if (this.firstActiveParticle >= this.MaxParticles)
          this.firstActiveParticle = 0;
      }
    }

    private void FreeRetiredParticles()
    {
      while (this.firstRetiredParticle != this.firstActiveParticle && this.drawCounter - (int) this.particles[this.firstRetiredParticle * this.vertsPerParticle].UserData.X >= 3)
      {
        ++this.firstRetiredParticle;
        if (this.firstRetiredParticle >= this.MaxParticles)
          this.firstRetiredParticle = 0;
      }
    }

    public void Draw(Player player, Player virtualPlayer)
    {
      if (this.firstNewParticle != this.firstFreeParticle)
        this.AddNewParticlesToVertexBuffer();
      if (this.firstActiveParticle != this.firstFreeParticle)
      {
        GraphicStatics.RainShader.CurrentTime.SetValue(this.currentTime);
        GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
        graphicsDevice.SetVertexBuffer((VertexBuffer) this.vertexBuffer);
        GraphicStatics.RainShader.Effect.CurrentTechnique.Passes[0].Apply();
        if (this.firstActiveParticle < this.firstFreeParticle)
        {
          graphicsDevice.DrawPrimitives(PrimitiveType.LineList, this.firstActiveParticle * this.vertsPerParticle, this.firstFreeParticle - this.firstActiveParticle);
        }
        else
        {
          graphicsDevice.DrawPrimitives(PrimitiveType.LineList, this.firstActiveParticle * this.vertsPerParticle, this.MaxParticles - this.firstActiveParticle);
          if (this.firstFreeParticle > 0)
            graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, this.firstFreeParticle);
        }
      }
      ++this.drawCounter;
    }

    private void AddNewParticlesToVertexBuffer()
    {
      int vertexStride = 28;
      if (this.firstNewParticle < this.firstFreeParticle)
      {
        this.vertexBuffer.SetData<RainParticleVertex>(this.firstNewParticle * vertexStride * this.vertsPerParticle, this.particles, this.firstNewParticle * this.vertsPerParticle, (this.firstFreeParticle - this.firstNewParticle) * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
      }
      else
      {
        this.vertexBuffer.SetData<RainParticleVertex>(this.firstNewParticle * vertexStride * this.vertsPerParticle, this.particles, this.firstNewParticle * this.vertsPerParticle, (this.MaxParticles - this.firstNewParticle) * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
        if (this.firstFreeParticle > 0)
          this.vertexBuffer.SetData<RainParticleVertex>(0, this.particles, 0, this.firstFreeParticle * this.vertsPerParticle, vertexStride, SetDataOptions.NoOverwrite);
      }
      this.firstNewParticle = this.firstFreeParticle;
    }

    public bool AddParticle(Vector3 position, float velocity, float endY, Color color)
    {
      int num1 = this.firstFreeParticle + 1;
      if (num1 >= this.MaxParticles)
        num1 = 0;
      if (num1 == this.firstRetiredParticle)
        return false;
      int index1 = this.firstFreeParticle * this.vertsPerParticle;
      float num2 = 0.5f;
      float y = position.Y - endY;
      RainParticleVertex rainParticleVertex = new RainParticleVertex();
      rainParticleVertex.UserData = new Vector3(this.currentTime, y, 0.0f);
      rainParticleVertex.Color = color;
      rainParticleVertex.Position = new HalfVector2(position.X, position.Z);
      rainParticleVertex.PosYVel.X = position.Y;
      rainParticleVertex.PosYVel.Y = velocity;
      this.particles[index1] = rainParticleVertex;
      int index2 = index1 + 1;
      rainParticleVertex.PosYVel.X = position.Y + num2;
      rainParticleVertex.UserData.Z = num2 - 0.03f;
      this.particles[index2] = rainParticleVertex;
      this.firstFreeParticle = num1;
      return true;
    }
  }
}
