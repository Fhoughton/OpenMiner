// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IHasNeighbours`1
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using System.Collections.Generic;

namespace StudioForge.Engine.Integration
{
  public interface IHasNeighbours<T>
  {
    IEnumerable<T> Neighbours { get; }
  }
}
