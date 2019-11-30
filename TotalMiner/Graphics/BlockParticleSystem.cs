// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.BlockParticleSystem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class BlockParticleSystem : GameObjectBase
  {
    public object ParticleListSemaphore = new object();
    private Vector3[] normals = new Vector3[6]
    {
      Vector3.Up,
      Vector3.Left,
      Vector3.Forward,
      Vector3.Right,
      Vector3.Backward,
      Vector3.Down
    };
    private int capacity;
    private BlockParticle[] particles;
    private LinkedList<int> indicesUsed;
    private Stack<int> indicesUnused;
    private CustomArray<VertexItemBlock2> vertices;
    private CustomArray<VertexItemBlock2> buildVertices1;
    private CustomArray<VertexItemBlock2> buildVertices2;
    private Map map;
    private GameInstance instance;
    private int updateCycle;
    private int updateCycleFreq;

    public int Capacity
    {
      get
      {
        return this.capacity;
      }
    }

    public BlockParticle[] Particles
    {
      get
      {
        return this.particles;
      }
    }

    public LinkedList<int> IndicesUsed
    {
      get
      {
        return this.indicesUsed;
      }
    }

    public CustomArray<VertexItemBlock2> Vertices
    {
      get
      {
        return this.vertices;
      }
    }

    public int ManagedMemoryUsed
    {
      get
      {
        return this.particles.Length * 50 + this.vertices.Array.Length * VertexItemBlock2.vertexDeclaration.VertexStride + this.buildVertices1.Array.Length * VertexItemBlock2.vertexDeclaration.VertexStride + this.buildVertices2.Array.Length * VertexItemBlock2.vertexDeclaration.VertexStride;
      }
    }

    public int UnmanagedMemoryUsed
    {
      get
      {
        return this.vertices.Array.Length * VertexItemBlock2.vertexDeclaration.VertexStride;
      }
    }

    public BlockParticleSystem(GameInstance instance, Map map, int capacity)
      : this(instance, map, capacity, 1, 0)
    {
    }

    public BlockParticleSystem(
      GameInstance instance,
      Map map,
      int capacity,
      int updateCycleFreq,
      int updateCycleStart)
    {
      this.instance = instance;
      this.capacity = capacity;
      this.map = map;
      this.updateCycle = updateCycleStart;
      this.updateCycleFreq = updateCycleFreq;
      this.Name = nameof (BlockParticleSystem);
    }

    protected override void InitializeCore(InitState state)
    {
      this.particles = new BlockParticle[this.capacity];
      this.indicesUsed = new LinkedList<int>();
      this.indicesUnused = new Stack<int>(this.capacity);
      for (int index = 0; index < this.capacity; ++index)
        this.indicesUnused.Push(index);
    }

    protected override void LoadContentCore(InitState state)
    {
      this.buildVertices1 = new CustomArray<VertexItemBlock2>(this.capacity * 24, 0.0f);
      this.buildVertices2 = new CustomArray<VertexItemBlock2>(this.capacity * 24, 0.0f);
      this.vertices = this.buildVertices2;
    }

    protected override void UnloadContentCore()
    {
      base.UnloadContentCore();
      this.buildVertices1.Clear();
      this.buildVertices2.Clear();
      this.vertices.Clear();
      this.indicesUnused.Clear();
      this.indicesUsed.Clear();
    }

    protected override void UpdateCore(UpdateState state)
    {
      if (++this.updateCycle < this.updateCycleFreq)
        return;
      this.updateCycle = 0;
      float elapsed = Services.ElapsedTime * (float) this.updateCycleFreq;
      LinkedListNode<int> next;
      lock (this.ParticleListSemaphore)
      {
        for (LinkedListNode<int> node = this.indicesUsed.First; node != null; node = next)
        {
          next = node.Next;
          int index = node.Value;
          BlockParticle particle = this.particles[index];
          particle.Age -= elapsed;
          if ((double) particle.Age <= 0.0)
          {
            this.Deactivate(node);
          }
          else
          {
            bool flag = false;
            if (particle.Modifier != null)
            {
              if (!particle.Modifier(ref particle, elapsed))
              {
                this.Deactivate(node);
                flag = true;
              }
            }
            else
              particle.Position += particle.Velocity * elapsed;
            if (!flag)
              this.particles[index] = particle;
          }
        }
      }
    }

    public void ClearAll()
    {
      lock (this.ParticleListSemaphore)
      {
        LinkedListNode<int> node = this.indicesUsed.First;
        LinkedListNode<int> next;
        for (; node != null; node = next)
        {
          next = node.Next;
          this.Deactivate(node);
        }
      }
    }

    public void BuildVertices()
    {
      if (this.updateCycleFreq > 1 && this.updateCycle != 1)
        return;
      CustomArray<VertexItemBlock2> buildVertices = this.vertices == this.buildVertices2 ? this.buildVertices1 : this.buildVertices2;
      buildVertices.Count = 0;
      lock (this.ParticleListSemaphore)
      {
        for (LinkedListNode<int> linkedListNode = this.indicesUsed.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          int index = linkedListNode.Value;
          GlobalPoint3D point = this.map.GetPoint(this.particles[index].Position);
          float light = point.Y < 0 || point.Y >= this.map.MapBound.Max.Y ? 1f : this.map.GetLightNormalized(point);
          this.AddBlock(buildVertices, light, ref this.particles[index]);
        }
      }
      this.vertices = buildVertices;
    }

    private void AddBlock(
      CustomArray<VertexItemBlock2> buildVertices,
      float light,
      ref BlockParticle particle)
    {
      VertexItemBlock2 t = new VertexItemBlock2();
      t.Color = particle.Color * light;
      Matrix fromYawPitchRoll = Matrix.CreateFromYawPitchRoll(particle.Rotation.X, particle.Rotation.Y, particle.Rotation.Z);
      float radius = particle.Radius;
      Vector3 position = particle.Position;
      for (int index = 0; index < this.normals.Length; ++index)
      {
        Vector3 normal = this.normals[index];
        Vector3 vector2 = new Vector3(normal.Y, normal.Z, normal.X);
        Vector3 vector3_1 = Vector3.Cross(normal, vector2);
        t.Position.W = (float) index;
        Vector3 vector3_2 = Vector3.Transform((normal - vector2 - vector3_1) * radius, fromYawPitchRoll) + position;
        t.Position.X = vector3_2.X;
        t.Position.Y = vector3_2.Y;
        t.Position.Z = vector3_2.Z;
        buildVertices.Add(t);
        vector3_2 = Vector3.Transform((normal - vector2 + vector3_1) * radius, fromYawPitchRoll) + position;
        t.Position.X = vector3_2.X;
        t.Position.Y = vector3_2.Y;
        t.Position.Z = vector3_2.Z;
        buildVertices.Add(t);
        vector3_2 = Vector3.Transform((normal + vector2 + vector3_1) * radius, fromYawPitchRoll) + position;
        t.Position.X = vector3_2.X;
        t.Position.Y = vector3_2.Y;
        t.Position.Z = vector3_2.Z;
        buildVertices.Add(t);
        vector3_2 = Vector3.Transform((normal + vector2 - vector3_1) * radius, fromYawPitchRoll) + position;
        t.Position.X = vector3_2.X;
        t.Position.Y = vector3_2.Y;
        t.Position.Z = vector3_2.Z;
        buildVertices.Add(t);
      }
    }

    public void Deactivate(LinkedListNode<int> node)
    {
      lock (this.ParticleListSemaphore)
      {
        this.indicesUnused.Push(node.Value);
        this.indicesUsed.Remove(node);
      }
    }

    public void Deactivate(int i)
    {
      lock (this.ParticleListSemaphore)
      {
        this.indicesUnused.Push(i);
        this.indicesUsed.Remove(i);
      }
    }

    public int AddNew(
      float age,
      Vector3 position,
      Vector3 velocity,
      Vector3 rotation,
      float radius,
      Color color,
      BlockParticleModifier modifier)
    {
      lock (this.ParticleListSemaphore)
      {
        if (this.indicesUnused.Count == 0)
          return -1;
        int index = this.indicesUnused.Pop();
        this.indicesUsed.AddLast(index);
        BlockParticle particle = this.particles[index];
        particle.Age = (double) age == 0.0 ? float.MaxValue : age;
        particle.Position = position;
        particle.Rotation = rotation;
        particle.Velocity = velocity;
        particle.Radius = radius;
        particle.Color = color;
        particle.Modifier = modifier;
        this.particles[index] = particle;
        return index;
      }
    }
  }
}
