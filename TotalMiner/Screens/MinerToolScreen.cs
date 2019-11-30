// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MinerToolScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal abstract class MinerToolScreen : GameScreen
  {
    protected Player player;
    protected Rectangle screenRect;
    protected SpriteBatchSafe spriteBatch;

    public Player Player
    {
      get
      {
        return this.player;
      }
    }

    protected GamerID PlayerID
    {
      get
      {
        if (this.player == null)
          return GamerID.Sys1;
        return this.player.GamerID;
      }
    }

    protected MinerToolScreen(Player player)
    {
      this.player = player;
      this.TransitionOnTime = TimeSpan.Zero;
      InputManager1.PushVirtualMouse();
    }

    public override int FadeBackBufferAlpha
    {
      get
      {
        return 20;
      }
    }

    public override void LoadContent()
    {
      base.LoadContent();
      this.borderColor = GraphicStatics.WindowBorderColor;
      this.clientBackColor = GraphicStatics.WindowClientColor;
      this.UpdateMatrix();
      NetworkManager.Instance.GamerJoined += new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
      NetworkManager.Instance.GamerLeft += new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
    }

    protected override void OnScreenRemovedCore()
    {
      NetworkManager.Instance.GamerJoined -= new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
      NetworkManager.Instance.GamerLeft -= new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
      base.OnScreenRemovedCore();
      InputManager1.PopVirtualMouse();
    }

    private void GamerJoinedEventHandler(object sender, GamerEventArgs e)
    {
      this.UpdateMatrix();
    }

    private void GamerLeftEventHandler(object sender, GamerEventArgs e)
    {
      this.UpdateMatrix();
    }

    protected virtual void UpdateMatrix()
    {
      if (this.player == null)
        return;
      Rectangle screenRect = this.screenRect;
      if (screenRect.Width == this.GraphicsDevice.Viewport.Width && screenRect.Height == this.GraphicsDevice.Viewport.Height)
      {
        this.Matrix = Matrix.CreateScale((float) this.player.Viewport.Width / (float) screenRect.Width, (float) this.player.Viewport.Height / (float) screenRect.Height, 1f);
      }
      else
      {
        screenRect.Inflate(48, 48);
        this.Matrix = this.player.GetScreenMatrix(screenRect);
      }
    }
  }
}
