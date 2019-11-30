// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandReplaceTextureScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandReplaceTextureScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Target Block";
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
        return "Target Block: " + ((Block) this.data.BlockID).ToString();
      }
    }

    private string BlockID1Text
    {
      get
      {
        Block blockId = (Block) this.data.BlockID;
        if (!this.map.UsesBlockTextureTable(blockId))
          return "Texture to Replace: Invalid";
        return "Texture to Replace: " + this.GetBlockDesc((Block) this.data.BlockID, this.map.GetBlockTextureID(blockId, (int) this.data.BlockID1));
      }
    }

    private string BlockID2Text
    {
      get
      {
        Block blockId = (Block) this.data.BlockID;
        if (!this.map.UsesBlockTextureTable(blockId))
          return "Replace With Texture: Invalid";
        return "Replace With Texture: " + this.GetBlockDesc((Block) this.data.BlockID, this.map.GetBlockTextureID(blockId, (int) this.data.BlockID2));
      }
    }

    private string GetBlockDesc(Block target, Block blockID)
    {
      switch (target)
      {
        case Block.StainedGlassPane:
          return "Stained Glass Pane";
        case Block.CoverBlock:
          if (blockID != Block.CoverBlock)
            return MapTM.CoverBlockTop[(int) blockID].ToString();
          return MapTM.CoverBlockTop[0].ToString();
        case Block.ArcadeMachine:
          if (blockID == Block.Grass)
            return "Total Invaders";
          return blockID != Block.Dirt ? "ArcadeMachine" : "Total Rush";
        case Block.StainedGlass:
          return "Stained Glass";
        default:
          return blockID.ToString();
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.SelectingBlockForReplaceTexture;
      }
    }

    private BlockSelectMode ReplaceSelectMode
    {
      get
      {
        switch ((Block) this.data.BlockID)
        {
          case Block.StainedGlassPane:
          case Block.StainedGlass:
            return BlockSelectMode.SelectingStainedGlass;
          case Block.ArcadeMachine:
            return BlockSelectMode.SelectingArcadeGame;
          default:
            return BlockSelectMode.SelectingUsedBlockTexture;
        }
      }
    }

    public CreativeCommandReplaceTextureScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetReplaceTextureCommandData(player))
    {
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockIDText));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectBlockType);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockID1Text));
      items[this.baseItemCount + 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectBlock1);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockID2Text));
      items[this.baseItemCount + 2].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectBlock2);
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.BlockIDText;
      this.MenuEntries[this.baseItemCount + 1].Text = this.BlockID1Text;
      this.MenuEntries[this.baseItemCount + 2].Text = this.BlockID2Text;
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
      else if (!this.map.UsesBlockTextureTable((Block) this.data.BlockID))
      {
        this.statusText = "Must specify a block that supports retexture";
        this.data.IsValid = false;
      }
      else if ((int) this.data.BlockID1 == (int) this.data.BlockID2)
      {
        this.statusText = "Must specify a different replacement texture";
        this.data.IsValid = false;
      }
      else
      {
        if (this.map.GetBlockTextureID((Block) this.data.BlockID, (int) this.data.BlockID1) != this.map.GetBlockTextureID((Block) this.data.BlockID, (int) this.data.BlockID2))
          return;
        this.statusText = "Must specify a different replacement texture";
        this.data.IsValid = false;
      }
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeReplaceTextureDefaults(this.data);
    }

    protected override int HighlightRectWidth
    {
      get
      {
        return 672;
      }
    }

    private void OnSelectBlockType(object sender, PlayerIndexEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.OnBlockTypeSelected), this.SelectBlockText, this.BlockSelectMode, Block.None, 0), this.player);
    }

    private bool OnBlockTypeSelected(Player player, Item item, int slotID, object tagData)
    {
      this.data.BlockID = (byte) ItemData.ConvertItemIDToBlockID(item);
      this.RefreshItemText();
      return true;
    }

    private void OnSelectBlock1(object sender, PlayerIndexEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.OnBlock1Selected), "Select the Texture to Replace", this.ReplaceSelectMode, (Block) this.data.BlockID, 0), this.player);
    }

    private bool OnBlock1Selected(Player player, Item item, int slotID, object tagData)
    {
      this.data.BlockID1 = (byte) slotID;
      this.RefreshItemText();
      return true;
    }

    private void OnSelectBlock2(object sender, PlayerIndexEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.OnBlock2Selected), "Select Replacement Texture", this.ReplaceSelectMode, (Block) this.data.BlockID, 0), this.player);
    }

    private bool OnBlock2Selected(Player player, Item item, int slotID, object tagData)
    {
      this.data.BlockID2 = (byte) slotID;
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
