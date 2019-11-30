// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptedMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptedMenuScreen : BlockMenuScreen
  {
    public GlobalPoint3D? ScriptOffset;
    public GlobalPoint3D? BlockOffset;
    private bool hasCancelItem;

    public ScriptedMenuScreen(
      Player player,
      GlobalPoint3D? scriptOffset,
      GlobalPoint3D? blockOffset)
      : base("Scripted Menu", player)
    {
      this.ScriptOffset = scriptOffset;
      this.BlockOffset = blockOffset;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.ItemsPerPage = 14;
      this.HighlightRect.Width = 144;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      foreach (MenuEntry menuEntry in this.MenuEntries)
      {
        int num = (int) ((double) this.Font.MeasureString(menuEntry.Text).X * (double) this.ItemTextScale + 96.0);
        if (num > this.HighlightRect.Width)
          this.HighlightRect.Width = num;
      }
      base.LoadContent();
    }

    public void AddMenuEntry(MenuEntry entry)
    {
      this.MenuEntries.Add(entry);
    }

    public void AddCancelMenuEntry()
    {
      this.AddCancelMenuEntry("Cancel");
    }

    public void AddCancelMenuEntry(string text)
    {
      BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, text);
      blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.Add((MenuEntry) blockMenuEntry);
      this.hasCancelItem = true;
    }

    public override bool HandleInput(InputState input)
    {
      if (this.hasCancelItem)
      {
        if (input.IsNewButtonPress(Buttons.Start))
        {
          this.ExitScreen();
          this.ScreenManager.AddScreen((GameScreen) new PauseMenuScreen(this.player.GameInstance, this.player), this.ControllingPlayer);
          return true;
        }
      }
      else
      {
        PlayerIndex playerIndex;
        if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
          return false;
      }
      return base.HandleInput(input);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
