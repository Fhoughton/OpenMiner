// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandFillScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandFillScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Fill Block";
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return true;
      }
    }

    protected override bool IsClearMarkersUsed
    {
      get
      {
        return true;
      }
    }

    private string BlockIDText
    {
      get
      {
        return "Fill Block: " + ((Block) this.data.BlockID).ToString();
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeFill;
      }
    }

    public CreativeCommandFillScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetFillCommandData(player))
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
      else if (this.WillCommandAffectNoEditZone())
      {
        this.statusText = "Region containes a No Edit zone";
        this.data.IsValid = false;
      }
      else
      {
        if (!this.IsPlayerInsideRegion(this.data.BlockID, true))
          return;
        this.statusText = "A player is located inside the Region";
        this.data.IsValid = false;
      }
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeFillDefaults(this.data);
    }

    protected override bool OnExecuteCore()
    {
      if (this.data.Seed == 0)
      {
        this.data.IsCustomSeed = false;
        this.data.Seed = this.instance.Random.Next();
      }
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      this.player.Raise_CreativeFill(this.data.Min, this.map.BuildBlockData((MapChunk) null, this.data.BlockID, (byte) 0, (byte) 0, (byte) 0));
      this.SendNetworkCommand();
      return true;
    }
  }
}
