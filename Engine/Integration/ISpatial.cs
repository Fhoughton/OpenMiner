// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.ISpatial
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Integration
{
  public interface ISpatial : ISpatialNode
  {
    Vector3 Scale { get; }

    Vector3 Up { get; }

    Vector3 Right { get; }

    Vector3 Forward { get; }

    BoundingBox BoundingBox { get; }

    Matrix WorldMatrix { get; }
  }
}
