// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IFrameRateCounter
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

namespace StudioForge.Engine.Integration
{
  public interface IFrameRateCounter
  {
    int FrameRate { get; }

    int SpriteCalls { get; set; }

    int DrawCalls { get; set; }

    int Primitives { get; set; }

    string DebugString { get; }
  }
}
