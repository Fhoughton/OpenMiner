// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.ParticleManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class ParticleManager : GameObjectBase
  {
    private BlockParticleSystem blockParticles;
    private ItemParticleSystem itemParticlesItems;
    private ItemParticleSystem itemParticlesBlocks;
    private ItemParticleSystem itemParticlesItemsCritical;
    private ItemParticleSystem itemParticlesBlocksCritical;
    private MapTM map;
    private GameInstance instance;
    private Dictionary<long, bool> particlesAttachedToBlocks;

    public bool HasParticlesToRender
    {
      get
      {
        if (this.blockParticles.IndicesUsed.Count <= 0 && this.itemParticlesBlocks.IndicesUsed.Count <= 0 && (this.itemParticlesItems.IndicesUsed.Count <= 0 && this.itemParticlesBlocksCritical.IndicesUsed.Count <= 0))
          return this.itemParticlesItemsCritical.IndicesUsed.Count > 0;
        return true;
      }
    }

    public int ParticleCount
    {
      get
      {
        return this.blockParticles.IndicesUsed.Count + this.itemParticlesBlocks.IndicesUsed.Count + this.itemParticlesItems.IndicesUsed.Count + this.itemParticlesBlocksCritical.IndicesUsed.Count + this.itemParticlesItemsCritical.IndicesUsed.Count;
      }
    }

    public CustomArray<VertexPositionNormalTexture> ItemVertices
    {
      get
      {
        return this.itemParticlesItems.Vertices;
      }
    }

    public CustomArray<VertexPositionNormalTexture> ItemVerticesBlocks
    {
      get
      {
        return this.itemParticlesBlocks.Vertices;
      }
    }

    public CustomArray<VertexPositionNormalTexture> ItemVerticesCritical
    {
      get
      {
        return this.itemParticlesItemsCritical.Vertices;
      }
    }

    public CustomArray<VertexPositionNormalTexture> ItemVerticesBlocksCritical
    {
      get
      {
        return this.itemParticlesBlocksCritical.Vertices;
      }
    }

    public CustomArray<VertexItemBlock2> BlockVertices
    {
      get
      {
        return this.blockParticles.Vertices;
      }
    }

    public int ManagedMemoryUsed
    {
      get
      {
        return this.blockParticles.ManagedMemoryUsed + this.itemParticlesBlocks.ManagedMemoryUsed + this.itemParticlesItems.ManagedMemoryUsed + this.itemParticlesBlocksCritical.ManagedMemoryUsed + this.itemParticlesItemsCritical.ManagedMemoryUsed;
      }
    }

    public int UnmanagedMemoryUsed
    {
      get
      {
        return this.blockParticles.UnmanagedMemoryUsed + this.itemParticlesBlocks.UnmanagedMemoryUsed + this.itemParticlesItems.UnmanagedMemoryUsed + this.itemParticlesBlocksCritical.UnmanagedMemoryUsed + this.itemParticlesItemsCritical.UnmanagedMemoryUsed;
      }
    }

    public ItemParticle? GetParticleFromID(int particleID)
    {
      if (particleID >= this.itemParticlesBlocksCritical.ParticleIDOffset)
        return this.itemParticlesBlocksCritical.GetParticleFromID(particleID);
      if (particleID >= this.itemParticlesItemsCritical.ParticleIDOffset)
        return this.itemParticlesItemsCritical.GetParticleFromID(particleID);
      if (particleID >= this.itemParticlesBlocks.ParticleIDOffset)
        return this.itemParticlesBlocks.GetParticleFromID(particleID);
      return this.itemParticlesItems.GetParticleFromID(particleID);
    }

    public ItemParticle? GetCloseParticle(
      ParticleType type,
      Vector3 pos,
      float distance,
      Item itemID,
      ItemParticleModifier modifier)
    {
      ItemParticle? closeParticle1 = this.itemParticlesBlocksCritical.GetCloseParticle(type, pos, distance, itemID, modifier);
      if (closeParticle1.HasValue)
        return closeParticle1;
      ItemParticle? closeParticle2 = this.itemParticlesItemsCritical.GetCloseParticle(type, pos, distance, itemID, modifier);
      if (closeParticle2.HasValue)
        return closeParticle2;
      ItemParticle? closeParticle3 = this.itemParticlesBlocks.GetCloseParticle(type, pos, distance, itemID, modifier);
      if (closeParticle3.HasValue)
        return closeParticle3;
      ItemParticle? closeParticle4 = this.itemParticlesItems.GetCloseParticle(type, pos, distance, itemID, modifier);
      if (closeParticle4.HasValue)
        return closeParticle4;
      return new ItemParticle?();
    }

    public void SetParticle(ItemParticle particle)
    {
      if (particle.ParticleID >= this.itemParticlesBlocksCritical.ParticleIDOffset)
      {
        int particleIndexFromId = this.itemParticlesBlocksCritical.GetParticleIndexFromID(particle.ParticleID);
        if (particleIndexFromId < 0)
          return;
        this.itemParticlesBlocksCritical.Particles[particleIndexFromId] = particle;
      }
      else if (particle.ParticleID >= this.itemParticlesItemsCritical.ParticleIDOffset)
      {
        int particleIndexFromId = this.itemParticlesItemsCritical.GetParticleIndexFromID(particle.ParticleID);
        if (particleIndexFromId < 0)
          return;
        this.itemParticlesItemsCritical.Particles[particleIndexFromId] = particle;
      }
      else if (particle.ParticleID >= this.itemParticlesBlocks.ParticleIDOffset)
      {
        int particleIndexFromId = this.itemParticlesBlocks.GetParticleIndexFromID(particle.ParticleID);
        if (particleIndexFromId < 0)
          return;
        this.itemParticlesBlocks.Particles[particleIndexFromId] = particle;
      }
      else
      {
        int particleIndexFromId = this.itemParticlesItems.GetParticleIndexFromID(particle.ParticleID);
        if (particleIndexFromId < 0)
          return;
        this.itemParticlesItems.Particles[particleIndexFromId] = particle;
      }
    }

    public void GetSystemsForSaving(out ItemParticleSystem ps1, out ItemParticleSystem ps2)
    {
      ps1 = this.itemParticlesBlocks;
      ps2 = this.itemParticlesItems;
    }

    public ParticleManager(GameInstance instance, MapTM map)
    {
      this.instance = instance;
      this.map = map;
      this.Name = nameof (ParticleManager);
      this.particlesAttachedToBlocks = new Dictionary<long, bool>();
    }

    protected override void LoadContentCore(InitState state)
    {
      this.itemParticlesItems = new ItemParticleSystem(this.instance, this.map, 300, 100, 0, 2, 0);
      this.itemParticlesItems.Initialize(state);
      this.itemParticlesItems.LoadContent(state);
      this.itemParticlesItems.IsEnabled = true;
      this.itemParticlesBlocks = new ItemParticleSystem(this.instance, this.map, 300, 100, 100000000, 2, 0);
      this.itemParticlesBlocks.Initialize(state);
      this.itemParticlesBlocks.LoadContent(state);
      this.itemParticlesBlocks.IsEnabled = true;
      this.itemParticlesItemsCritical = new ItemParticleSystem(this.instance, this.map, 50, 50, 200000000);
      this.itemParticlesItemsCritical.Initialize(state);
      this.itemParticlesItemsCritical.LoadContent(state);
      this.itemParticlesItemsCritical.IsEnabled = true;
      this.itemParticlesBlocksCritical = new ItemParticleSystem(this.instance, this.map, 150, 150, 300000000);
      this.itemParticlesBlocksCritical.Initialize(state);
      this.itemParticlesBlocksCritical.LoadContent(state);
      this.itemParticlesBlocksCritical.IsEnabled = true;
      this.blockParticles = new BlockParticleSystem(this.instance, (Map) this.map, 256, 2, -1);
      this.blockParticles.Initialize(state);
      this.blockParticles.LoadContent(state);
      this.blockParticles.IsEnabled = true;
    }

    protected override void UnloadContentCore()
    {
      base.UnloadContentCore();
      if (this.itemParticlesItems != null)
        this.itemParticlesItems.UnloadContent();
      if (this.itemParticlesBlocks != null)
        this.itemParticlesBlocks.UnloadContent();
      if (this.itemParticlesItemsCritical != null)
        this.itemParticlesItemsCritical.UnloadContent();
      if (this.itemParticlesBlocksCritical != null)
        this.itemParticlesBlocksCritical.UnloadContent();
      if (this.blockParticles == null)
        return;
      this.blockParticles.UnloadContent();
    }

    protected override void UpdateCore(UpdateState state)
    {
      base.UpdateCore(state);
      if (this.itemParticlesItems.IsEnabledField)
        this.itemParticlesItems.Update(state);
      if (this.itemParticlesBlocks.IsEnabledField)
        this.itemParticlesBlocks.Update(state);
      if (this.itemParticlesItemsCritical.IsEnabledField)
        this.itemParticlesItemsCritical.Update(state);
      if (this.itemParticlesBlocksCritical.IsEnabledField)
        this.itemParticlesBlocksCritical.Update(state);
      if (!this.blockParticles.IsEnabledField)
        return;
      this.blockParticles.Update(state);
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
      bool cameFromRemote,
      bool isCriticalParticle)
    {
      Item itemId = item.ItemID;
      if (isCriticalParticle)
      {
        if (itemId > Item.zLastBlockID)
        {
          int index = this.itemParticlesItemsCritical.AddNew(type, age, position, velocity, radius, item, modifier, minPickupAge, particleID, textureIndex, playerID, cameFromRemote);
          if (index >= 0)
            index = this.itemParticlesItemsCritical.Particles[index].ParticleID;
          return index;
        }
        int index1 = this.itemParticlesBlocksCritical.AddNew(type, age, position, velocity, radius, item, modifier, minPickupAge, particleID, textureIndex, playerID, cameFromRemote);
        if (index1 >= 0)
          index1 = this.itemParticlesBlocksCritical.Particles[index1].ParticleID;
        return index1;
      }
      if (itemId > Item.zLastBlockID)
      {
        int index = this.itemParticlesItems.AddNew(type, age, position, velocity, radius, item, modifier, minPickupAge, particleID, textureIndex, playerID, cameFromRemote);
        if (index >= 0)
          index = this.itemParticlesItems.Particles[index].ParticleID;
        return index;
      }
      int index2 = this.itemParticlesBlocks.AddNew(type, age, position, velocity, radius, item, modifier, minPickupAge, particleID, textureIndex, playerID, cameFromRemote);
      if (index2 >= 0)
        index2 = this.itemParticlesBlocks.Particles[index2].ParticleID;
      return index2;
    }

    public int AddBlockNew(
      float age,
      Vector3 position,
      Vector3 velocity,
      Vector3 rotation,
      float radius,
      Color color,
      BlockParticleModifier modifier)
    {
      return this.blockParticles.AddNew(age, position, velocity, rotation, radius, color, modifier);
    }

    public void ClearAll()
    {
      this.itemParticlesItems.ClearAll();
      this.itemParticlesBlocks.ClearAll();
      this.itemParticlesItemsCritical.ClearAll();
      this.itemParticlesBlocksCritical.ClearAll();
      this.blockParticles.ClearAll();
    }

    public void Deactivate(ItemParticle particle)
    {
      if (particle.ParticleID >= this.itemParticlesBlocksCritical.ParticleIDOffset)
        this.itemParticlesBlocksCritical.Deactivate(particle);
      else if (particle.ParticleID >= this.itemParticlesItemsCritical.ParticleIDOffset)
        this.itemParticlesItemsCritical.Deactivate(particle);
      else if (particle.ParticleID >= this.itemParticlesBlocks.ParticleIDOffset)
      {
        this.itemParticlesBlocks.Deactivate(particle);
      }
      else
      {
        if (particle.ParticleID < this.itemParticlesItems.ParticleIDOffset)
          return;
        this.itemParticlesItems.Deactivate(particle);
      }
    }

    private void ClearParticlesAttached(long hash)
    {
      this.itemParticlesItems.ClearParticlesAttached(hash);
      this.itemParticlesBlocks.ClearParticlesAttached(hash);
      this.itemParticlesItemsCritical.ClearParticlesAttached(hash);
      this.itemParticlesBlocksCritical.ClearParticlesAttached(hash);
    }

    public void BuildVertices(Player player)
    {
      this.blockParticles.BuildVertices();
      this.itemParticlesBlocks.BuildVertices(player);
      this.itemParticlesItems.BuildVertices(player);
      this.itemParticlesBlocksCritical.BuildVertices(player);
      this.itemParticlesItemsCritical.BuildVertices(player);
    }

    public void StickParticle(ref ItemParticle particle, long hash)
    {
      particle.AttachedTo = hash;
      lock (this.particlesAttachedToBlocks)
      {
        if (this.particlesAttachedToBlocks.ContainsKey(hash))
          return;
        this.particlesAttachedToBlocks.Add(hash, true);
      }
    }

    public void BlockCleared(GlobalPoint3D p)
    {
      long globalHashCode = this.map.GetGlobalHashCode(p);
      bool flag = false;
      lock (this.particlesAttachedToBlocks)
      {
        if (this.particlesAttachedToBlocks.ContainsKey(globalHashCode))
        {
          flag = true;
          this.particlesAttachedToBlocks.Remove(globalHashCode);
        }
      }
      if (!flag)
        return;
      this.ClearParticlesAttached(globalHashCode);
    }
  }
}
