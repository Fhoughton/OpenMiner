// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.BuffLock
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  public static class BuffLock
  {
    public static object CacheLock = new object();
    public static object StreamLock = new object();
  }
}
