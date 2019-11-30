// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IGeometricPrimitive
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Integration
{
  public interface IGeometricPrimitive : IDisposable
  {
    BasicEffect BasicEffect { get; }

    void Draw(Effect effect);

    void Draw(Matrix world, ICamera camera, Color color, GlobalBasicEffectUpdate update);

    void Draw(Matrix world, Color color, GlobalBasicEffectUpdate update);

    void Draw(
      Matrix world,
      Matrix view,
      Matrix projection,
      Color color,
      GlobalBasicEffectUpdate update);
  }
}
