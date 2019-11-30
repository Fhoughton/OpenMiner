// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolReplaceTextureMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.Screens;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class CreativeToolReplaceTextureMenu : CreativeToolMenu
  {
    private TextBox blockWin;
    private TextBox block1Win;
    private TextBox block2Win;

    protected override string SelectBlockText
    {
      get
      {
        return "Select the Target Block";
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
        Block blockId = (Block) this.data.BlockID;
        if (!this.map.UsesBlockTextureTable(blockId))
          return "Invalid";
        return this.GetBlockDesc((Block) this.data.BlockID, this.map.GetBlockTextureID(blockId, (int) this.data.BlockID1));
      }
    }

    private string BlockID2Text
    {
      get
      {
        Block blockId = (Block) this.data.BlockID;
        if (!this.map.UsesBlockTextureTable(blockId))
          return "Invalid";
        return this.GetBlockDesc((Block) this.data.BlockID, this.map.GetBlockTextureID(blockId, (int) this.data.BlockID2));
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
          return ItemData2.ForDisplay(this.instance, (Item) blockID);
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

    public CreativeToolReplaceTextureMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      Action onApplied)
      : base(parentScreen, instance, player, instance.CreativeModeHelper.GetReplaceTextureCommandData(player), onApplied)
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
      Window window1 = (Window) new TextBox("Target Block:", x, y, w, h, scale);
      window1.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window1);
      Window window2 = (Window) (this.blockWin = new TextBox(this.BlockIDText, x + w + 1, y, w2, h, scale));
      window2.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window2.ClickHandler += new Window.WindowHandler(this.ClickBlockType);
      container.AddChild((Node) window2);
      y += h + g;
      Window window3 = (Window) new TextBox("Texture to Replace:", x, y, w, h, scale);
      window3.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window3);
      Window window4 = (Window) (this.block1Win = new TextBox(this.BlockID1Text, x + w + 1, y, w2, h, scale));
      window4.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window4.ClickHandler += new Window.WindowHandler(this.ClickBlockID1);
      container.AddChild((Node) window4);
      y += h + g;
      Window window5 = (Window) new TextBox("Replace with Texture:", x, y, w, h, scale);
      window5.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window5);
      Window window6 = (Window) (this.block2Win = new TextBox(this.BlockID2Text, x + w + 1, y, w2, h, scale));
      window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window6.ClickHandler += new Window.WindowHandler(this.ClickBlockID2);
      container.AddChild((Node) window6);
      y += h + g;
      return y;
    }

    protected override void RefreshWindowTextCore()
    {
      this.blockWin.Text = this.BlockIDText;
      this.block1Win.Text = this.BlockID1Text;
      this.block2Win.Text = this.BlockID2Text;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeReplaceTextureDefaults(this.data);
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

    protected void ClickBlockType(object sender, WindowEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.OnBlockTypeSelected), this.SelectBlockText, this.BlockSelectMode, Block.None, 0), this.player);
    }

    private bool OnBlockTypeSelected(Player player, Item item, int slotID, object tagData)
    {
      this.data.BlockID = (byte) ItemData.ConvertItemIDToBlockID(item);
      this.RefreshWindowText();
      return true;
    }

    protected void ClickBlockID1(object sender, WindowEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.OnBlock1Selected), "Select the Texture to Replace", this.ReplaceSelectMode, (Block) this.data.BlockID, 0), this.player);
    }

    private bool OnBlock1Selected(Player player, Item item, int slotID, object tagData)
    {
      this.data.BlockID1 = (byte) slotID;
      this.RefreshWindowText();
      return true;
    }

    protected void ClickBlockID2(object sender, WindowEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectItemCallBack(this.OnBlock2Selected), "Select Replacement Texture", this.ReplaceSelectMode, (Block) this.data.BlockID, 0), this.player);
    }

    private bool OnBlock2Selected(Player player, Item item, int slotID, object tagData)
    {
      this.data.BlockID2 = (byte) slotID;
      this.RefreshWindowText();
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
