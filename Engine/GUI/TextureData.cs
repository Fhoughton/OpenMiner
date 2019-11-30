// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GUI.TextureData
// Assembly: StudioForge.Engine.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DCE0EBE4-EECE-47C9-9CF3-4B51A8FA96BF
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GUI.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.GUI
{
  public class TextureData
  {
    public Color TintColor = Color.White;
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Origin;
    public float Rotation;
    public float Scale;
    public Rectangle? SrRect;
    public Rectangle? DestRect;
    public bool IsBackground;
    public bool StretchToFit;

    public Rectangle GetNewDestRect()
    {
      Rectangle rectangle = new Rectangle();
      if (this.Texture != null)
      {
        rectangle.Width = this.Texture.Width;
        rectangle.Height = this.Texture.Height;
      }
      return rectangle;
    }
  }
}
