// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.HitTarget
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner
{
  internal struct HitTarget
  {
    public Actor Target;
    public float Distance;
    public bool IsCriticalHit;

    public void Clear()
    {
      this.Target = (Actor) null;
      this.Distance = float.MaxValue;
      this.IsCriticalHit = false;
    }
  }
}
