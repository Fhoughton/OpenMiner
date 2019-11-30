// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BlockClearedEventArgs
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine.GamerServices;

namespace StudioForge.BlockWorld
{
  public struct BlockClearedEventArgs
  {
    public MapBlock BlockData;
    public GlobalPoint3D Point;
    public ClearBlockResult Result;
    public UpdateBlockMethod Method;
    public GamerID PlayerID;
    public bool IgnoreFiniteModePickupRestriction;
  }
}
