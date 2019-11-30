// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandFloodScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandFloodScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Block to Flood with";
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

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeFill;
      }
    }

    public CreativeCommandFloodScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetFloodCommandData(player))
    {
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockIDText));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(((CreativeCommandScreen) this).OnSelectBlock);
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.BlockIDText;
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
        this.statusText = "Must select a block to flood with";
        this.data.IsValid = false;
      }
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeFloodDefaults(this.data);
    }

    protected override bool OnExecuteCore()
    {
      this.instance.CreativeModeHelper.RunFlood(this.player, (Block) this.data.BlockID);
      return true;
    }
  }
}
