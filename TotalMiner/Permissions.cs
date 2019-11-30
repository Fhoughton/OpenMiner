// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Permissions
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System;

namespace StudioForge.TotalMiner
{
  [Flags]
  public enum Permissions : ushort
  {
    None = 0,
    Adventure = 1,
    Edit = 2,
    Creative = 4,
    Fly = 8,
    Map = 16, // 0x0010
    Save = 32, // 0x0020
    Admin = 64, // 0x0040
    Grief = 128, // 0x0080
    VoiceChat = 256, // 0x0100
    Spectate = 512, // 0x0200
    SystemShops = 1024, // 0x0400
    ViewScripts = 2048, // 0x0800
    TextChat = 4096, // 0x1000
  }
}
