// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.NotifyRecipient
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  public enum NotifyRecipient : byte
  {
    None = 0,
    Local = 1,
    Remote = 2,
    Admin = 4,
    Clan = 8,
    Global = Remote | Local, // 0x03
  }
}
