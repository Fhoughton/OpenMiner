// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.ItemParticleSystem
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class ItemParticleSystem : GameObjectBase
  {
    public object ParticleListSemaphore = new object();
    public readonly int ParticleIDOffset;
    private int capacity;
    private int drawCapacity;
    private ItemParticle[] particles;
    private LinkedList<int> indicesUsed;
    private Stack<int> indicesUnused;
    private int particleCounter;
    private CustomArray<VertexPositionNormalTexture> vertices;
    private CustomArray<VertexPositionNormalTexture> buildVertices1;
    private CustomArray<VertexPositionNormalTexture> buildVertices2;
    private VoxelMeshBuilder helper;
    private GameInstance instance;
    private MapTM map;
    private int updateCycle;
    private int updateCycleFreq;

    public int Capacity
    {
      get
      {
        return this.capacity;
      }
    }

    public ItemParticle[] Particles
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

    public CustomArray<VertexPositionNormalTexture> Vertices
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
        return this.particles.Length * 84 + this.vertices.Array.Length * VertexPositionNormalTexture.VertexDeclaration.VertexStride + this.buildVertices1.Array.Length * VertexPositionNormalTexture.VertexDeclaration.VertexStride + this.buildVertices2.Array.Length * VertexPositionNormalTexture.VertexDeclaration.VertexStride;
      }
    }

    public int UnmanagedMemoryUsed
    {
      get
      {
        return this.vertices.Array.Length * VertexPositionNormalTexture.VertexDeclaration.VertexStride;
      }
    }

    public ItemParticleSystem(
      GameInstance instance,
      MapTM map,
      int capacity,
      int drawCapacity,
      int particleIDOffset)
      : this(instance, map, capacity, drawCapacity, particleIDOffset, 1, 0)
    {
    }

    public ItemParticleSystem(
      GameInstance instance,
      MapTM map,
      int capacity,
      int drawCapacity,
      int particleIDOffset,
      int updateCycleFreq,
      int updateCycleStart)
    {
      this.instance = instance;
      this.capacity = capacity;
      this.drawCapacity = drawCapacity;
      this.map = map;
      this.ParticleIDOffset = particleIDOffset;
      this.updateCycle = updateCycleStart;
      this.updateCycleFreq = updateCycleFreq;
      this.Name = nameof (ItemParticleSystem);
    }

    protected override void InitializeCore(InitState state)
    {
      this.particles = new ItemParticle[this.capacity];
      this.indicesUsed = new LinkedList<int>();
      this.indicesUnused = new Stack<int>(this.capacity);
      for (int index = 0; index < this.capacity; ++index)
        this.indicesUnused.Push(index);
    }

    protected override void LoadContentCore(InitState state)
    {
      this.helper = new VoxelMeshBuilder();
      this.buildVertices1 = new CustomArray<VertexPositionNormalTexture>(this.drawCapacity * 24, 0.0f);
      this.buildVertices2 = new CustomArray<VertexPositionNormalTexture>(this.drawCapacity * 24, 0.0f);
      this.vertices = this.buildVertices2;
    }

    protected override void UnloadContentCore()
    {
      base.UnloadContentCore();
      if (this.helper == null)
        return;
      this.helper.UnloadContent();
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
          ItemParticle particle = this.particles[index];
          particle.Age -= elapsed;
          particle.MinPickupAge -= elapsed;
          if ((double) particle.Age <= 0.0)
          {
            this.UpdateParticleDeathByOldAge(particle);
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
            {
              particle.Position.X += particle.Velocity.X * elapsed;
              particle.Position.Y += particle.Velocity.Y * elapsed;
              particle.Position.Z += particle.Velocity.Z * elapsed;
            }
            if (!flag)
            {
              if ((double) particle.Age > 5.0 && !this.instance.IsInDestructable(particle.Item.ItemID))
              {
                switch ((Block) this.map.GetBlockID(particle.Position))
                {
                  case Block.Lava:
                    particle.Age = 5f;
                    break;
                  case Block.Fire:
                    particle.Age = 10f;
                    break;
                }
              }
              this.particles[index] = particle;
            }
          }
        }
      }
    }

    private void UpdateParticleDeathByOldAge(ItemParticle particle)
    {
      if (!this.map.IsHost || particle.HasType(ParticleType.Loot | ParticleType.Debris) || (particle.Item.ItemID != Item.Sapling || !this.instance.IsFiniteResources))
        return;
      GlobalPoint3D point = this.map.GetPoint(particle.Position);
      if (!this.map.IsValidPoint(point) || !BlockData.IsGrassOrDirt((Block) this.map.GetBlockIDNoCache(point + GlobalPoint3D.Down)))
        return;
      byte auxData;
      this.instance.AddBlock(point, Block.Sapling, out auxData, UpdateBlockMethod.Strategy, GamerID.Sys1, false, true, true, point + GlobalPoint3D.Down, BlockFace.Up, 0, Item.None, (object) null);
    }

    public void BuildVertices(Player player)
    {
      if (this.updateCycleFreq > 1 && this.updateCycle != 1)
        return;
      CustomArray<VertexPositionNormalTexture> verts = this.vertices == this.buildVertices2 ? this.buildVertices1 : this.buildVertices2;
      verts.Count = 0;
      int num = this.drawCapacity + 1;
      lock (this.ParticleListSemaphore)
      {
        for (LinkedListNode<int> linkedListNode = this.indicesUsed.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (--num > 0)
          {
            ItemParticle particle = this.particles[linkedListNode.Value];
            GlobalPoint3D point = this.map.GetPoint(particle.Position);
            if (point.Y >= 0)
            {
              float light1;
              if ((double) particle.OwnLight > 0.0)
                light1 = particle.OwnLight;
              else if (point.Y >= this.map.MapBound.Max.Y)
              {
                light1 = 1f;
              }
              else
              {
                light1 = this.map.GetLightNormalized(point);
                if ((double) light1 == 0.0)
                  light1 = this.map.GetLightNormalized(this.map.GetMaxNeighbourLight(point));
              }
              if (particle.IsType(ParticleType.Projectile))
              {
                this.helper.BuildProjectile(this.map, verts, particle.Position, particle.Velocity, particle.Radius, (ushort) particle.Item.ItemID, light1);
              }
              else
              {
                float light2 = light1 * 0.75f;
                this.helper.BuildPickup(this.map, verts, particle.Position, particle.Radius, particle.Rotation, (ushort) particle.Item.ItemID, particle.TextureIndex, light2, player);
              }
            }
          }
          else
            break;
        }
      }
      this.vertices = verts;
    }

    public int GetParticleIndexFromID(int particleID)
    {
      for (LinkedListNode<int> linkedListNode = this.indicesUsed.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        if (this.particles[linkedListNode.Value].ParticleID == particleID)
          return linkedListNode.Value;
      }
      return -1;
    }

    public ItemParticle? GetParticleFromID(int particleID)
    {
      for (LinkedListNode<int> linkedListNode = this.indicesUsed.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        if (this.particles[linkedListNode.Value].ParticleID == particleID)
          return new ItemParticle?(this.particles[linkedListNode.Value]);
      }
      return new ItemParticle?();
    }

    public int AddNew(
      ParticleType type,
      float age,
      Vector3 position,
      Vector3 velocity,
      float radius,
      InventoryItem item,
      ItemParticleModifier modifier,
      float minPickupAge,
      int particleID,
      byte textureIndex,
      GamerID playerID,
      bool cameFromRemote)
    {
      lock (this.ParticleListSemaphore)
      {
        if (this.indicesUnused.Count == 0)
          return -1;
        int index;
        try
        {
          index = this.indicesUnused.Pop();
          this.indicesUsed.AddLast(index);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(10, ex);
          return -1;
        }
        ItemParticle particle = this.particles[index];
        particle.Type = type;
        particle.Age = (double) age == 0.0 ? float.MaxValue : age;
        particle.Position = position;
        particle.Rotation = 0.0f;
        particle.Velocity = velocity;
        particle.Radius = radius;
        particle.Item = item;
        particle.OwnLight = (float) ItemData.GetParticleLight(item.ItemID) / this.map.MaxLight;
        particle.Modifier = modifier;
        particle.MinPickupAge = minPickupAge;
        particle.ParticleID = particleID <= 0 ? ++this.particleCounter + this.ParticleIDOffset : particleID;
        particle.PlayerID = playerID;
        particle.CameFromRemote = cameFromRemote;
        particle.TextureIndex = textureIndex;
        this.particles[index] = particle;
        return index;
      }
    }

    public ItemParticle? GetCloseParticle(
      ParticleType type,
      Vector3 pos,
      float distance,
      Item itemID,
      ItemParticleModifier modifier)
    {
      LinkedListNode<int> linkedListNode = this.indicesUsed.First;
      distance *= distance;
      for (; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        ItemParticle particle = this.particles[linkedListNode.Value];
        if (particle.Item.ItemID == itemID && particle.Modifier == modifier && ((particle.Type & type) == type && (double) Vector3.DistanceSquared(pos, particle.Position) <= (double) distance))
          return new ItemParticle?(particle);
      }
      return new ItemParticle?();
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

    public void ClearParticlesAttached(long hash)
    {
      lock (this.ParticleListSemaphore)
      {
        LinkedListNode<int> node = this.indicesUsed.First;
        LinkedListNode<int> next;
        for (; node != null; node = next)
        {
          next = node.Next;
          if (this.particles[node.Value].AttachedTo == hash)
            this.Deactivate(node);
        }
      }
    }

    public void Deactivate(ItemParticle particle)
    {
      lock (this.ParticleListSemaphore)
      {
        LinkedListNode<int> node = this.indicesUsed.First;
        int particleId = particle.ParticleID;
        for (; node != null; node = node.Next)
        {
          if (this.particles[node.Value].ParticleID == particleId)
          {
            this.Deactivate(node);
            break;
          }
        }
      }
    }

    private void Deactivate(LinkedListNode<int> node)
    {
      lock (this.ParticleListSemaphore)
      {
        try
        {
          this.CleanupParticle(node.Value);
          this.indicesUnused.Push(node.Value);
          this.indicesUsed.Remove(node);
        }
        catch (InvalidOperationException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(9, (Exception) ex);
        }
        catch (IndexOutOfRangeException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(9, (Exception) ex);
        }
      }
    }

    private void CleanupParticle(int i)
    {
      ItemParticle particle = this.particles[i];
      if (!particle.HasType(ParticleType.SetPower))
        return;
      this.instance.SetPower(this.map.GetPointFromGlobalHash(particle.AttachedTo), false, particle.PlayerID);
    }
  }
}
