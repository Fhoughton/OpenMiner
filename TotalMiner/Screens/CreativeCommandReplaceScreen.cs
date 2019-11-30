// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandReplaceScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandReplaceScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Block to Replace";
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

    private string BlockID1Text
    {
      get
      {
        return "Block To Replace: " + ((Block) this.data.BlockID).ToString();
      }
    }

    private string BlockID2Text
    {
      get
      {
        return "Replace With: " + ((Block) this.data.BlockID1).ToString();
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeReplace;
      }
    }

    public CreativeCommandReplaceScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetReplaceCommandData(player))
    {
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockID1Text));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(((CreativeCommandScreen) this).OnSelectBlock);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockID2Text));
      items[this.baseItemCount + 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectBlock2);
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.BlockID1Text;
      this.MenuEntries[this.baseItemCount + 1].Text = this.BlockID2Text;
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
      else if (this.IsPlayerInsideRegion(this.data.BlockID1, true))
      {
        this.statusText = "A player is located inside the Region";
        this.data.IsValid = false;
      }
      else
      {
        if ((int) this.data.BlockID != (int) this.data.BlockID1)
          return;
        this.statusText = "Must specify a different replacement block";
        this.data.IsValid = false;
      }
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeReplaceDefaults(this.data);
    }

    private void OnSelectBlock2(object sender, PlayerIndexEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.OnBlock2Selected), "Select Replacement Block", this.BlockSelectMode), this.player);
    }

    private bool OnBlock2Selected(Player player, Block block)
    {
      this.data.BlockID1 = (byte) block;
      this.RefreshItemText();
      return true;
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
