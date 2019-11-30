// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CharacterEffect
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.TotalMiner.API;

namespace StudioForge.TotalMiner
{
  public abstract class CharacterEffect
  {
    public string Name;
    public float Timer;
    public float Interval;
    public float Duration;
    public string History;

    public bool Update(ITMActor receiver, ITMActor applier)
    {
      return this.UpdateCore(receiver, applier);
    }

    protected abstract bool UpdateCore(ITMActor owner, ITMActor applier);
  }
}
