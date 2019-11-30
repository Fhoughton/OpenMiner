// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.ChunkFlags
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;

namespace StudioForge.BlockWorld
{
  [Flags]
  public enum ChunkFlags : uint
  {
    None = 0,
    Generated = 1,
    Generating = 2,
    Decorated = 4,
    Decorating = 8,
    LightDirty = 16, // 0x00000010
    Lighting = 32, // 0x00000020
    MeshDirty = 64, // 0x00000040
    MeshLoaded = 128, // 0x00000080
    MeshLoading = 256, // 0x00000100
    Committed = 512, // 0x00000200
    NotUsed2 = 1024, // 0x00000400
    NotUsed3 = 2048, // 0x00000800
    UserEdited = 4096, // 0x00001000
    HasSpecialBlocks = 8192, // 0x00002000
    ReceivedFromHost = 32768, // 0x00008000
    ChunkIsAllAir = 65536, // 0x00010000
    ChunkIsAllSolid = 131072, // 0x00020000
  }
}
