// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.ChunkUpdateFlags
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using System;

namespace StudioForge.BlockWorld
{
  [Flags]
  public enum ChunkUpdateFlags : ushort
  {
    None = 0,
    LeftChunkBorder = 1,
    ForwardChunkBorder = 2,
    RightChunkBorder = 4,
    BackChunkBorder = 8,
    UpChunkBorder = 16, // 0x0010
    DownChunkBorder = 32, // 0x0020
    LeftSegmentBorder = 64, // 0x0040
    ForwardSegmentBorder = 128, // 0x0080
    RightSegmentBorder = 256, // 0x0100
    BackSegmentBorder = 512, // 0x0200
    UpSegmentBorder = 1024, // 0x0400
    DownSegmentBorder = 2048, // 0x0800
  }
}
