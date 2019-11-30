// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.IThreadWorkItem
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

namespace StudioForge.Engine.Core
{
  public interface IThreadWorkItem
  {
    bool IsSleeping { get; }

    bool CanWait { get; }

    string Name { get; }

    void Update();
  }
}
