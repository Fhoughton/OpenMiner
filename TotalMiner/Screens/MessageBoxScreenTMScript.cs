// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.MessageBoxScreenTMScript
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class MessageBoxScreenTMScript : MessageBoxScreenTM
  {
    private string aScript;
    private string xScript;
    private string yScript;
    private GlobalPoint3D? aPoint;
    private GlobalPoint3D? xPoint;
    private GlobalPoint3D? yPoint;
    private bool disableCancel;

    public MessageBoxScreenTMScript(
      Player player,
      string message,
      string aText,
      string aScript,
      GlobalPoint3D? aPoint,
      string xText,
      string xScript,
      GlobalPoint3D? xPoint,
      string yText,
      string yScript,
      GlobalPoint3D? yPoint,
      string bText,
      bool disableCancel)
      : base(message != null ? message : "", aText, xText, yText, bText, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player)
    {
      this.aScript = aScript;
      this.xScript = xScript;
      this.yScript = yScript;
      this.aPoint = aPoint;
      this.xPoint = xPoint;
      this.yPoint = yPoint;
      this.IsPopup = false;
      this.disableCancel = disableCancel;
    }

    public override void LoadContent()
    {
      base.LoadContent();
      if (this.aScript != null)
        this.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnButtonA);
      if (this.xScript != null)
        this.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.OnButtonX);
      if (this.yScript == null)
        return;
      this.ButtonY += new EventHandler<PlayerIndexEventArgs>(this.OnButtonY);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      this.ButtonA -= new EventHandler<PlayerIndexEventArgs>(this.OnButtonA);
      this.ButtonX -= new EventHandler<PlayerIndexEventArgs>(this.OnButtonX);
      this.ButtonY -= new EventHandler<PlayerIndexEventArgs>(this.OnButtonY);
    }

    private void OnButtonA(object sender, PlayerIndexEventArgs e)
    {
      this.Player.GameInstance.ExecuteScript(this.aScript, new ScriptExecuteData()
      {
        Actor = (Actor) this.Player,
        ScriptOffset = this.aPoint
      }, true);
    }

    private void OnButtonX(object sender, PlayerIndexEventArgs e)
    {
      this.Player.GameInstance.ExecuteScript(this.xScript, new ScriptExecuteData()
      {
        Actor = (Actor) this.Player,
        ScriptOffset = this.xPoint
      }, true);
    }

    private void OnButtonY(object sender, PlayerIndexEventArgs e)
    {
      this.Player.GameInstance.ExecuteScript(this.yScript, new ScriptExecuteData()
      {
        Actor = (Actor) this.Player,
        ScriptOffset = this.yPoint
      }, true);
    }

    public override bool HandleInput(InputState input)
    {
      if (input.IsNewButtonPress(Buttons.Start))
      {
        this.ExitScreen();
        this.ScreenManager.AddScreen((GameScreen) new PauseMenuScreen(this.Player.GameInstance, this.Player), this.ControllingPlayer);
        return true;
      }
      PlayerIndex playerIndex;
      if (this.disableCancel && input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
        return true;
      return base.HandleInput(input);
    }
  }
}
