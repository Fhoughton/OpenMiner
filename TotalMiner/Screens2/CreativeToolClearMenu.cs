// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolClearMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class CreativeToolClearMenu : CreativeToolMenu
  {
    private TextBox blockWin;

    protected override string SelectBlockText
    {
      get
      {
        return "Select the Block to Clear";
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return true;
      }
    }

    private new string BlockIDText
    {
      get
      {
        if (this.data.BlockID != (byte) 0)
          return ItemData2.ForDisplay(this.instance, (Item) this.data.BlockID);
        return "All";
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeClear;
      }
    }

    public CreativeToolClearMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      Action onApplied)
      : base(parentScreen, instance, player, instance.CreativeModeHelper.GetClearCommandData(player), onApplied)
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
      Window window1 = (Window) new TextBox("Block to Clear:", x, y, w, h, scale);
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
      this.player.SetCreativeClearDefaults(this.data);
    }

    protected override void BuildMessageText()
    {
      if (this.markerCount < 2)
      {
        this.statusText = "Must have at least 2 markers";
        this.data.IsValid = false;
      }
      else if (this.RegionSizeInBlocks > CreativeModeHelper.MaxRegionBlocks && !this.player.IsGod)
      {
        this.statusText = "Region exceeds maximum of " + (object) CreativeModeHelper.MaxRegionBlocks + " blocks";
        this.data.IsValid = false;
      }
      else
      {
        if (!this.WillCommandAffectNoEditZone())
          return;
        this.statusText = "Region containes a No Edit zone";
        this.data.IsValid = false;
      }
    }

    protected override bool OnExecuteCore()
    {
      if (this.data.Seed == 0)
      {
        this.data.IsCustomSeed = false;
        this.data.Seed = this.instance.Random.Next();
      }
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      this.SendNetworkCommand();
      return true;
    }
  }
}
