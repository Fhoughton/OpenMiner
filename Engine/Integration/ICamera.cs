// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.ICamera
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Integration
{
  public interface ICamera : IHasUpdate
  {
    Vector3 Position { get; }

    Matrix ViewMatrix { get; }

    Matrix ProjectionMatrix { get; }

    bool FogEnabled { get; }

    Color LenseColor { get; }

    float FogStart { get; }

    float NearClip { get; }

    float FarClip { get; }
  }
}
