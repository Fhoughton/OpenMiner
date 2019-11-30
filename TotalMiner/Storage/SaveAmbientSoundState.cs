// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveAmbientSoundState
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveAmbientSoundState
  {
    public short SoundID;
    public float Volume;
    public ushort Distance;
    public byte LoopDelay;
    public DayOrNight DayOrNight;
    public bool RequiresPower;
    public GlobalPoint3D Point;
  }
}
