// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.WifiTransmitterScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class WifiTransmitterScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private GlobalPoint3D point;
    private ushort frequency;
    private ushort origFrequency;
    private WifiTransmitterBlock block;

    private string FrequencyText
    {
      get
      {
        return "Output Frequency: " + (this.frequency == (ushort) 0 ? "Disabled" : this.frequency.ToString());
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.WifiTransmitter, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
        return "Texture: " + (textureIdForDrawing == Block.None ? "None" : textureIdForDrawing.ToString());
      }
    }

    public WifiTransmitterScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Transmitter", player)
    {
      WifiTransmitterScreen transmitterScreen = this;
      this.instance = instance;
      this.point = p;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.WifiTransmitter, UpdateBlockMethod.Player, this.PlayerID, true) as WifiTransmitterBlock;
      this.origFrequency = this.frequency = this.block.Frequency;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.FrequencyText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TextureText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => transmitterScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(transmitterScreen.OnNumberEntered), (int) transmitterScreen.frequency, false), transmitterScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.selectedEntry = 1;
      instance.NetworkManager.BlockTextureChangedReceived += new BlockEventHandler(this.BlockTextureChanged);
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.WifiTransmitter, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.WifiTransmitter, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
      return true;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 430;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.NetworkManager.BlockTextureChangedReceived -= new BlockEventHandler(this.BlockTextureChanged);
      base.OnScreenRemovedCore();
      this.block.Frequency = this.frequency;
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, false);
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
    }

    private void OnNumberEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number < 0.0 || number >= (double) ushort.MaxValue)
        return;
      this.frequency = (ushort) number;
      this.MenuEntries[0].Text = this.FrequencyText;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
