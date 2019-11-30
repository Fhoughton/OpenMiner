// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptBlockScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptBlockScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private ScriptBlock block;

    private string PowerOnScriptText
    {
      get
      {
        return "Powered On: " + this.block.PowerOnScript;
      }
    }

    private string PowerOffScriptText
    {
      get
      {
        return "Powered Off: " + this.block.PowerOffScript;
      }
    }

    private string LookAtRangeText
    {
      get
      {
        return "Look At Range: " + ((double) this.block.PlayerLookRange > 0.0 ? this.block.PlayerLookRange.ToString() : "Disabled");
      }
    }

    private string ActAsPressurePlateText
    {
      get
      {
        return "Act as Pressure Plate: " + (this.block.ActAsPressurePlate ? "Yes" : "No");
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.ScriptBlock, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
        return "Texture: " + (textureIdForDrawing == Block.None ? "None" : textureIdForDrawing.ToString());
      }
    }

    public ScriptBlockScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Script", player)
    {
      ScriptBlockScreen scriptBlockScreen = this;
      this.instance = instance;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.ScriptBlock, UpdateBlockMethod.Player, this.PlayerID, true) as ScriptBlock;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.PowerOnScriptText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.PowerOffScriptText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.ActAsPressurePlateText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.LookAtRangeText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TextureText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => scriptBlockScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, scriptBlockScreen.block.PowerOnScript, new ListBoxScreen.OnMenuItemSelected(scriptBlockScreen.OnPowerOnScriptSelected), false, true), scriptBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => scriptBlockScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, scriptBlockScreen.block.PowerOffScript, new ListBoxScreen.OnMenuItemSelected(scriptBlockScreen.OnPowerOffScriptSelected), false, true), scriptBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.ActAsPressurePlate = !this.block.ActAsPressurePlate;
        this.MenuEntries[2].Text = this.ActAsPressurePlateText;
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => scriptBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(scriptBlockScreen.OnLookAtRangeEntered), scriptBlockScreen.block.PlayerLookRange, true, false), scriptBlockScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int num6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index6 = num6;
      int num7 = index6 + 1;
      blockMenuEntryList7[index6].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      instance.NetworkManager.BlockTextureChangedReceived += new BlockEventHandler(this.BlockTextureChanged);
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.ScriptBlock, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.ScriptBlock, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
      return true;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 575;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.MenuEntries[0].ToolTip.Text = "Select the script to be executed when this script block is powered on.";
      this.MenuEntries[1].ToolTip.Text = "Select the script to be executed when this script block is powered off.";
      this.MenuEntries[2].ToolTip.Text = "If the script block is acting as a pressure plate, then the script selected in the Powered On field will be executed when a player stands on the script block (if no other players are already standing on it) and the script selected in the Powered Off field will be executed when all players have stepped off the script block.";
      this.MenuEntries[3].ToolTip.Text = "If a Look At Range is specified, then the script selected in the Powered On field will be executed when a player looks directly at the script block within the range specified (distance in blocks) and the script selected in the Powered Off field will be executed when the player looks away from the script block (or moves out of range).";
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.NetworkManager.BlockTextureChangedReceived -= new BlockEventHandler(this.BlockTextureChanged);
      base.OnScreenRemovedCore();
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, this.block.PowerOnScript == null && this.block.PowerOffScript == null);
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
    }

    private bool OnPowerOnScriptSelected(MenuEntry script)
    {
      this.block.PowerOnScript = script == null ? (string) null : (string) script.Tag + script.Text;
      this.MenuEntries[0].Text = this.PowerOnScriptText;
      return true;
    }

    private bool OnPowerOffScriptSelected(MenuEntry script)
    {
      this.block.PowerOffScript = script == null ? (string) null : (string) script.Tag + script.Text;
      this.MenuEntries[1].Text = this.PowerOffScriptText;
      return true;
    }

    private void OnLookAtRangeEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.block.PlayerLookRange = MathHelper.Clamp((float) number, 0.0f, 16f);
      this.MenuEntries[3].Text = this.LookAtRangeText;
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
