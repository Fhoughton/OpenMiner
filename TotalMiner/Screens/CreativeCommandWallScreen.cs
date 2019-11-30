// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandWallScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandWallScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Block for the " + (this.IsPath ? "Path" : "Wall");
      }
    }

    protected override bool IsBoundUsed
    {
      get
      {
        return false;
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return false;
      }
    }

    private string BlockIDText
    {
      get
      {
        return "Block: " + ((Block) this.data.BlockID).ToString();
      }
    }

    private string WidthText
    {
      get
      {
        return "Width: " + this.data.BlockID1.ToString();
      }
    }

    private string HeightText
    {
      get
      {
        return "Height: " + this.data.BlockID2.ToString();
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeFill;
      }
    }

    private bool IsPath
    {
      get
      {
        return this.data.BlockID2 < (byte) 2;
      }
    }

    public CreativeCommandWallScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetWallCommandData(player))
    {
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockIDText));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(((CreativeCommandScreen) this).OnSelectBlock);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.WidthText));
      items[this.baseItemCount + 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterWidth);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.HeightText));
      items[this.baseItemCount + 2].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterHeight);
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.BlockIDText;
      this.MenuEntries[this.baseItemCount + 1].Text = this.WidthText;
      this.MenuEntries[this.baseItemCount + 2].Text = this.HeightText;
    }

    protected override void BuildMessageText()
    {
      if (this.markerCount >= 2)
        return;
      this.statusText = "Must have at least 2 markers";
      this.data.IsValid = false;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeWallDefaults(this.data);
    }

    private void OnEnterWidth(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnWidthEntered), (int) this.data.BlockID1, false), this.ControllingPlayer);
    }

    private void OnWidthEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.BlockID1 = (byte) MyMathHelper.Clamp((int) value, 1, 16);
      this.RefreshItemText();
    }

    private void OnEnterHeight(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnHeightEntered), (int) this.data.BlockID2, false), this.ControllingPlayer);
    }

    private void OnHeightEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.BlockID2 = (byte) MyMathHelper.Clamp((int) value, 1, 16);
      this.RefreshItemText();
    }

    protected override bool OnExecuteCore()
    {
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      this.SendNetworkCommand();
      return true;
    }
  }
}
