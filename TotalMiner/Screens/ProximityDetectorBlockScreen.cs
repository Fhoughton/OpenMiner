// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ProximityDetectorBlockScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ProximityDetectorBlockScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private ProximityDetectorBlock block;

    private string TextureText
    {
      get
      {
        if (this.instance == null || this.instance.Map == null)
          return "";
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.ProximityDetector, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
        return "Texture: " + (textureIdForDrawing == Block.None ? "None" : textureIdForDrawing.ToString());
      }
    }

    private bool HasPermission
    {
      get
      {
        if (this.player.HasPermission(Permissions.Creative))
          return !this.instance.IsInZoneType(this.block.Point, ZoneType.NoEdit, this.player.GamerID);
        return false;
      }
    }

    public ProximityDetectorBlockScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Proximity Detector", player)
    {
      ProximityDetectorBlockScreen detectorBlockScreen = this;
      this.instance = instance;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.ProximityDetector, UpdateBlockMethod.Player, player.GamerID, false) as ProximityDetectorBlock;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => detectorBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(detectorBlockScreen.OnRangeEntered), (int) detectorBlockScreen.block.Range, false), detectorBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.ToggleTargetType(BlockTargetTypes.Admins);
        this.ResetText();
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.ToggleTargetType(BlockTargetTypes.Players);
        this.ResetText();
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.ToggleTargetType(BlockTargetTypes.Mobs);
        this.ResetText();
      });
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => detectorBlockScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, detectorBlockScreen.block.OnEntryScriptName, new ListBoxScreen.OnMenuItemSelected(detectorBlockScreen.OnEntryScriptSelected), false, true), detectorBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num6;
      int num7 = index6 + 1;
      blockMenuEntryList7[index6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => detectorBlockScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, detectorBlockScreen.block.OnExitScriptName, new ListBoxScreen.OnMenuItemSelected(detectorBlockScreen.OnExitScriptSelected), false, true), detectorBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index7 = num7;
      int num8 = index7 + 1;
      blockMenuEntryList8[index7].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetText();
      instance.NetworkManager.BlockTextureChangedReceived += new BlockEventHandler(this.BlockTextureChanged);
    }

    private void ResetText()
    {
      if (this.block == null)
        return;
      this.MenuEntries[0].Text = "Range: " + this.block.Range.ToString();
      this.MenuEntries[1].Text = "Target Admins: " + this.block.IsTargeting(BlockTargetTypes.Admins).ToString();
      this.MenuEntries[2].Text = "Target Players: " + this.block.IsTargeting(BlockTargetTypes.Players).ToString();
      this.MenuEntries[3].Text = "Target Mobs: " + this.block.IsTargeting(BlockTargetTypes.Mobs).ToString();
      this.MenuEntries[4].Text = "Entry Script: " + this.block.OnEntryScriptName;
      this.MenuEntries[5].Text = "Exit Script: " + this.block.OnExitScriptName;
      this.MenuEntries[6].Text = this.TextureText;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 478;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.NetworkManager.BlockTextureChangedReceived -= new BlockEventHandler(this.BlockTextureChanged);
      base.OnScreenRemovedCore();
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, false);
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.ResetText();
    }

    private void OnRangeEntered(double range, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.block.Range = (byte) MyMathHelper.Clamp((int) range, 2, 100);
      this.ResetText();
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.ProximityDetector, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.ProximityDetector, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.ResetText();
      return true;
    }

    private bool OnEntryScriptSelected(MenuEntry script)
    {
      if (this.player.IsAdmin)
      {
        if (script != null)
        {
          string str = (string) script.Tag + script.Text;
          if (this.block.OnEntryScriptName != str)
            this.block.OnEntryScriptName = str;
        }
        else if (this.block.OnEntryScriptName != null)
          this.block.OnEntryScriptName = (string) null;
        this.ResetText();
      }
      return true;
    }

    private bool OnExitScriptSelected(MenuEntry script)
    {
      if (this.player.IsAdmin)
      {
        if (script != null)
        {
          string str = (string) script.Tag + script.Text;
          if (this.block.OnExitScriptName != str)
            this.block.OnExitScriptName = str;
        }
        else if (this.block.OnExitScriptName != null)
          this.block.OnExitScriptName = (string) null;
        this.ResetText();
      }
      return true;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
