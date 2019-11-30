// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolFloodMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class CreativeToolFloodMenu : CreativeToolMenu
  {
    private TextBox blockWin;

    protected override string SelectBlockText
    {
      get
      {
        return "Select the Block to Flood with";
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return false;
      }
    }

    public CreativeToolFloodMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      Action onApplied)
      : base(parentScreen, instance, player, instance.CreativeModeHelper.GetFloodCommandData(player), onApplied)
    {
    }

    protected override int InitWindowsExtra(
      Window container,
      int x,
      int y,
      int w,
      int w2,
      int h,
      int g,
      float scale)
    {
      Window window1 = (Window) new TextBox("Block:", x, y, w, h, scale);
      window1.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window1);
      Window window2 = (Window) (this.blockWin = new TextBox(this.BlockIDText, x + w + 1, y, w2, h, scale));
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(((CreativeToolMenu) this).ClickBlockID);
      container.AddChild((Node) window2);
      y += h + g;
      return y;
    }

    protected override void RefreshWindowTextCore()
    {
      this.blockWin.Text = this.BlockIDText;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeFloodDefaults(this.data);
    }

    protected override void BuildMessageText()
    {
      if (this.markerCount < 1)
      {
        this.statusText = "Must have at least 1 marker";
        this.data.IsValid = false;
      }
      else
      {
        if (this.data.BlockID != (byte) 0)
          return;
        this.statusText = "Must select a block";
        this.data.IsValid = false;
      }
    }

    protected override bool OnExecuteCore()
    {
      this.instance.CreativeModeHelper.RunFlood(this.player, (Block) this.data.BlockID);
      return true;
    }
  }
}
