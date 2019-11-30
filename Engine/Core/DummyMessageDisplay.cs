// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.DummyMessageDisplay
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public class DummyMessageDisplay : IMessageDisplay
  {
    public List<string> Messages = new List<string>();

    private void AddMessage(string message)
    {
      this.Messages.Add(message);
    }

    SpriteFont IMessageDisplay.Font
    {
      get
      {
        return (SpriteFont) null;
      }
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      params object[] parameters)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      float scale,
      Color color)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      float seconds,
      float scale,
      Color color)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      Vector2 velocity,
      float scale,
      Color color)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      Matrix matrix)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      bool centered)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }

    Vector2 IMessageDisplay.ShowMessage(
      string message,
      Vector2 position,
      Vector2 velocity,
      float seconds,
      float scale,
      Color color,
      bool centered,
      Matrix matrix)
    {
      this.AddMessage(message);
      return Vector2.Zero;
    }
  }
}
