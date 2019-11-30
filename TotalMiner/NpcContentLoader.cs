// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.NpcContentLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner
{
  internal class NpcContentLoader : IThreadWorkItem
  {
    public static StudioForge.Engine.Core.Pool<NpcContentLoader> Pool = new StudioForge.Engine.Core.Pool<NpcContentLoader>();
    private int poolIndex;
    private NpcAnimContent content;

    public string Name
    {
      get
      {
        return "MobContentLoader";
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public void Initialize(int poolIndex, NpcAnimContent content)
    {
      this.poolIndex = poolIndex;
      this.content = content;
    }

    public void Update()
    {
      try
      {
        this.content.LoadContent((InitState) null);
      }
      finally
      {
        NpcContentLoader.Pool.Release(this.poolIndex);
      }
    }
  }
}
