// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.HealthBlockScreen
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
  internal class HealthBlockScreen : BlockMenuScreen
  {
    private GameInstance instance;
    private HealthBlock block;

    private string DefenceLevelText
    {
      get
      {
        return "Defence Level: " + (object) this.block.DefenceLevel;
      }
    }

    private string HealthLevelText
    {
      get
      {
        if (this.block.HistoryKey.IsNotEmpty())
          return "Health Level: Not used";
        string str1 = "Health Level: " + (object) this.block.HealthLevel;
        string str2;
        if (this.block.HealthLevel > 0)
          str2 = str1 + "  (Hit Points: " + (object) SkillData.MaxHealth(this.block.HealthLevel) + ")";
        else
          str2 = str1 + "  (Inactive)";
        return str2;
      }
    }

    private string HistoryText
    {
      get
      {
        return "Health History: " + this.block.HistoryKey;
      }
    }

    private string HealthText
    {
      get
      {
        return "Current Health: " + (object) this.block.GetHealth(this.instance);
      }
    }

    private string KillScriptText
    {
      get
      {
        return "Kill Script: " + this.block.KillScript;
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.HealthBlock, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
        return "Texture: " + (textureIdForDrawing == Block.None ? "None" : textureIdForDrawing.ToString());
      }
    }

    public HealthBlockScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Health Block", player)
    {
      HealthBlockScreen healthBlockScreen = this;
      this.instance = instance;
      this.block = instance.MapStrategyTM.GetOrAddDataBlock(p, Block.HealthBlock, UpdateBlockMethod.Player, this.PlayerID, true) as HealthBlock;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.DefenceLevelText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.HealthLevelText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.HistoryText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.HealthText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.KillScriptText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TextureText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => healthBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(healthBlockScreen.OnDefenceLevelEntered), healthBlockScreen.block.DefenceLevel, false), healthBlockScreen.ControllingPlayer));
      blockMenuEntryList[1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => healthBlockScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(healthBlockScreen.OnHealthLevelEntered), healthBlockScreen.block.HealthLevel, false), healthBlockScreen.ControllingPlayer));
      blockMenuEntryList[2].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnHistorySelected);
      blockMenuEntryList[4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => healthBlockScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, healthBlockScreen.block.KillScript, new ListBoxScreen.OnMenuItemSelected(healthBlockScreen.OnKillScriptSelected), false, true), healthBlockScreen.ControllingPlayer));
      blockMenuEntryList[5].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      bool flag = player.HasPermission(Permissions.Creative);
      this.MenuEntries[0].IsEnabled = flag;
      this.MenuEntries[1].IsEnabled = flag && this.block.HistoryKey.IsEmpty();
      this.MenuEntries[2].IsEnabled = flag;
      this.MenuEntries[3].IsEnabled = false;
      this.MenuEntries[4].IsEnabled = player.HasPermissionAny(Permissions.Admin | Permissions.ViewScripts);
      this.MenuEntries[5].IsEnabled = flag;
      instance.NetworkManager.BlockTextureChangedReceived += new BlockEventHandler(this.BlockTextureChanged);
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
      this.MenuEntries[1].ToolTip.Text = "The blocks health is calculated from this Health level.";
      this.MenuEntries[2].ToolTip.Text = "The blocks health is the value of this system history.";
      this.MenuEntries[4].ToolTip.Text = "The selected script will be executed when the block is killed.";
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.NetworkManager.BlockTextureChangedReceived -= new BlockEventHandler(this.BlockTextureChanged);
      base.OnScreenRemovedCore();
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, !this.block.IsCombatEnabled);
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.ResetToggleItems();
    }

    private void OnHealthLevelEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.block.SetHealthLevel((int) value);
      this.ResetToggleItems();
    }

    private void OnDefenceLevelEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.block.DefenceLevel = Math.Min(9999, Math.Max(1, (int) value));
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = this.DefenceLevelText;
      this.MenuEntries[1].Text = this.HealthLevelText;
      this.MenuEntries[2].Text = this.HistoryText;
      this.MenuEntries[3].Text = this.HealthText;
      this.MenuEntries[4].Text = this.KillScriptText;
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
      this.MenuEntries[1].IsEnabled = this.player.HasPermission(Permissions.Creative) && this.block.HistoryKey.IsEmpty();
    }

    private void OnHistorySelected(object sender, PlayerIndexEventArgs e)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter History Key", "", this.block.HistoryKey, new AsyncCallback(this.OnHistoryEntered), (object) null, this.MenuEntries[this.selectedEntry], false);
    }

    private void OnHistoryEntered(IAsyncResult ar)
    {
      string str = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (str == null)
        return;
      this.block.HistoryKey = str;
      this.ResetToggleItems();
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.HealthBlock, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.HealthBlock, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.ResetToggleItems();
      return true;
    }

    private bool OnKillScriptSelected(MenuEntry script)
    {
      if (this.player.IsAdmin)
      {
        this.block.KillScript = script == null ? (string) null : (string) script.Tag + script.Text;
        this.ResetToggleItems();
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
