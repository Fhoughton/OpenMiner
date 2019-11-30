// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.WifiReceiverScreen
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
  internal class WifiReceiverScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private GlobalPoint3D point;
    private ushort frequency1;
    private ushort frequency2;
    private ushort origFrequency1;
    private ushort origFrequency2;
    private BinaryOperatorType gate;
    private BinaryOperatorType origGate;
    private WifiReceiverBlock block;

    private string Frequency1Text
    {
      get
      {
        return "Input Frequency 1: " + (this.frequency1 == (ushort) 0 ? "Disabled" : this.frequency1.ToString());
      }
    }

    private string Frequency2Text
    {
      get
      {
        return "Input Frequency 2: " + (this.frequency2 == (ushort) 0 ? "Disabled" : this.frequency2.ToString());
      }
    }

    private string LogicGateText
    {
      get
      {
        return "Logic Gate: " + this.gate.ToString();
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.WifiReceiver, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
        return "Texture: " + (textureIdForDrawing == Block.None ? "None" : textureIdForDrawing.ToString());
      }
    }

    public WifiReceiverScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Receiver", player)
    {
      WifiReceiverScreen wifiReceiverScreen = this;
      this.instance = instance;
      this.point = p;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.WifiReceiver, UpdateBlockMethod.Player, this.PlayerID, true) as WifiReceiverBlock;
      this.origFrequency1 = this.frequency1 = this.block.Frequency1;
      this.origFrequency2 = this.frequency2 = this.block.Frequency2;
      this.origGate = this.gate = this.block.Gate;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.Frequency1Text));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.Frequency2Text));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.LogicGateText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TextureText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int index1 = 0;
      blockMenuEntryList1[index1].IsEnabled = player.HasPermission(Permissions.Creative);
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index2 = index1;
      int index3 = index2 + 1;
      blockMenuEntryList2[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => wifiReceiverScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(wifiReceiverScreen.OnFrequency1Entered), (int) wifiReceiverScreen.frequency1, false), wifiReceiverScreen.ControllingPlayer));
      blockMenuEntryList1[index3].IsEnabled = player.HasPermission(Permissions.Creative);
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index4 = index3;
      int index5 = index4 + 1;
      blockMenuEntryList3[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => wifiReceiverScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(wifiReceiverScreen.OnFrequency2Entered), (int) wifiReceiverScreen.frequency2, false), wifiReceiverScreen.ControllingPlayer));
      blockMenuEntryList1[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnGateEntered);
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index6 = index5;
      int num1 = index6 + 1;
      blockMenuEntryList4[index6].IsEnabled = this.frequency1 > (ushort) 0 && this.frequency2 > (ushort) 0;
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index7 = num1;
      int num2 = index7 + 1;
      blockMenuEntryList5[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index8 = num2;
      int num3 = index8 + 1;
      blockMenuEntryList6[index8].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      instance.NetworkManager.BlockTextureChangedReceived += new BlockEventHandler(this.BlockTextureChanged);
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.WifiReceiver, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.WifiReceiver, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
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
      this.block.Frequency1 = this.frequency1;
      this.block.Frequency2 = this.frequency2;
      this.block.Gate = this.gate;
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, false);
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
    }

    private void OnFrequency1Entered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number < 0.0 || number >= (double) ushort.MaxValue)
        return;
      this.frequency1 = (ushort) number;
      this.MenuEntries[0].Text = this.Frequency1Text;
      this.ValidateBlock();
      this.MenuEntries[2].IsEnabled = this.frequency1 > (ushort) 0 && this.frequency2 > (ushort) 0;
    }

    private void OnFrequency2Entered(double number, bool isCancelled, object state)
    {
      if (isCancelled || number < 0.0 || number >= (double) ushort.MaxValue)
        return;
      this.frequency2 = (ushort) number;
      this.MenuEntries[1].Text = this.Frequency2Text;
      this.ValidateBlock();
      this.MenuEntries[2].IsEnabled = this.frequency1 > (ushort) 0 && this.frequency2 > (ushort) 0;
    }

    private void OnGateEntered(object sender, PlayerIndexEventArgs e)
    {
      int num;
      if ((num = (int) (this.gate + 1)) > 6)
        num = 0;
      this.gate = (BinaryOperatorType) num;
      this.MenuEntries[2].Text = this.LogicGateText;
    }

    private void ValidateBlock()
    {
      if ((int) this.frequency1 != (int) this.frequency2)
        return;
      this.frequency2 = (ushort) 0;
      this.MenuEntries[1].Text = this.Frequency2Text;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
