// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GamerServices.GamerStates
// Assembly: StudioForge.Engine.GamerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EA07B8F-6C00-417B-9E82-CD1E4EB140B6
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GamerServices.dll

using System;

namespace StudioForge.Engine.GamerServices
{
  [Flags]
  public enum GamerStates
  {
    Local = 1,
    Host = 2,
    HasVoice = 4,
    Guest = 8,
    MutedByLocalUser = 16, // 0x00000010
    PrivateSlot = 32, // 0x00000020
    Ready = 64, // 0x00000040
  }
}
