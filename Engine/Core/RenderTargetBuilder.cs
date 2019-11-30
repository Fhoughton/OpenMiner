// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.RenderTargetBuilder
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Core
{
  public static class RenderTargetBuilder
  {
    public static RenderTarget2D CreateRenderTarget(
      GraphicsDevice device,
      int width,
      int height,
      bool mipMap,
      SurfaceFormat surface,
      DepthFormat depth)
    {
      return new RenderTarget2D(device, width, height, mipMap, surface, depth);
    }
  }
}
