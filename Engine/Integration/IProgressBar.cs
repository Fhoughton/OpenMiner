// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IProgressBar
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

namespace StudioForge.Engine.Integration
{
  public interface IProgressBar
  {
    string Text { get; set; }

    float Progress { get; }

    float Factor { get; set; }

    object Tag { get; set; }

    void Reset();

    void Reset(float value);

    void AddProgress(float increment);
  }
}
