// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IMessageDisplay
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Integration
{
  public interface IMessageDisplay
  {
    SpriteFont Font { get; }

    Vector2 ShowMessage(string message, params object[] parameters);

    Vector2 ShowMessage(string message, float scale, Color color);

    Vector2 ShowMessage(string message, float seconds, float scale, Color color);

    Vector2 ShowMessage(string message, Vector2 velocity, float scale, Color color);

    Vector2 ShowMessage(
      string message,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color);

    Vector2 ShowMessage(
      string message,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      Matrix matrix);

    Vector2 ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color);

    Vector2 ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      bool centered);

    Vector2 ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      bool centered,
      Matrix matrix);
  }
}
