// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MemoryUsageCalculator
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using System;

namespace StudioForge.TotalMiner
{
  internal class MemoryUsageCalculator : TimedThreadWorkItem
  {
    public long TotalMeshSize;
    public long ManagedMemorySize;
    public long UnmanagedMemorySize;
    public long LastGCTotalMemory;
    private long totalMeshSize;
    private long managedMemorySize;
    private long unmanagedMemorySize;
    private long lastGCTotalMemory;
    private GameInstance instance;

    public override string Name
    {
      get
      {
        return nameof (MemoryUsageCalculator);
      }
    }

    public MemoryUsageCalculator(GameInstance instance, PriorityLevel priority, int sleepTime)
      : base(priority, sleepTime)
    {
      this.instance = instance;
    }

    protected override void UpdateCore()
    {
      this.lastGCTotalMemory = GC.GetTotalMemory(false);
      this.managedMemorySize = 0L;
      this.managedMemorySize += (long) VoxelMeshBuilder.VertexPoolSize;
      this.managedMemorySize += (long) Map.RLEStreamBufferManager.ArraySize;
      this.managedMemorySize += (long) TotalMinerGame.Instance.ScreenManager.MemorySizeInBytes;
      this.managedMemorySize += (long) this.instance.ParticleSystemManagedMemoryUsed;
      for (int index = 0; index < MapLightingByChunkThreadedWrapper.LightingPool.List.Length; ++index)
      {
        if (MapLightingByChunkThreadedWrapper.LightingPool.List[index] != null)
          this.managedMemorySize += MapLightingByChunkThreadedWrapper.LightingPool.List[index].MemorySize;
      }
      this.totalMeshSize = 0L;
      for (int index = 0; index < Map.LiveMaps.Count; ++index)
      {
        Map liveMap = Map.LiveMaps[index];
        if (liveMap != null)
        {
          this.managedMemorySize += (long) liveMap.MemorySize;
          this.totalMeshSize += this.GetMeshSize(liveMap);
        }
      }
      this.unmanagedMemorySize = 0L;
      this.unmanagedMemorySize += CoreGlobals.AudioManager.BufferSize;
      this.unmanagedMemorySize += (long) (3686400 * Globals2.DeviceVirtualization);
      this.unmanagedMemorySize += (long) (3686400 * Globals2.DeviceVirtualization);
      this.unmanagedMemorySize += (long) (3686400 * this.instance.LocalPlayerCount);
      this.unmanagedMemorySize += GraphicStatics.BufferSize();
      this.unmanagedMemorySize += this.instance.MapRenderer.BufferSize;
      this.unmanagedMemorySize += MapChunkContent.IndexBuffer.BufferSize();
      this.unmanagedMemorySize += (long) this.instance.ParticleSystemUnmanagedMemoryUsed;
      if (this.instance.NpcManager != null)
        this.unmanagedMemorySize += this.instance.NpcManager.TotalBufferSize;
      this.unmanagedMemorySize += (long) TotalMinerGame.Instance.ScreenManager.MemorySizeInBytesUnmanaged;
      this.unmanagedMemorySize += (long) MapTopViewScreen.StaticMemorySizeInBytesUnmanaged;
      this.TotalMeshSize = this.totalMeshSize;
      this.ManagedMemorySize = this.managedMemorySize;
      this.UnmanagedMemorySize = this.unmanagedMemorySize;
      this.LastGCTotalMemory = this.lastGCTotalMemory;
    }

    private long GetMeshSize(Map map)
    {
      long num = 0;
      if (map != null && map.Regions != null)
      {
        foreach (MapRegion mapRegion in map.Regions.Values)
          num += (long) ((MapRegionTM) mapRegion).TotalMeshSize;
      }
      return num;
    }
  }
}
