// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NPCSpawnScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Achievements;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class NPCSpawnScreen : BlockMenuScreen
  {
    private int maxInstances = 20;
    private GameInstance instance;
    private NpcSpawnBlock block;

    private string NameText
    {
      get
      {
        return "Name: " + this.block.Name;
      }
    }

    private string BehaviourText
    {
      get
      {
        return "Behaviour: " + (this.block.BehaviourTree.IsNotEmpty() ? this.block.BehaviourTree : "Default");
      }
    }

    private string DialogText
    {
      get
      {
        return "Dialog: " + this.block.DialogTree;
      }
    }

    private string TimeOfDayText
    {
      get
      {
        return "Time: " + (this.block.DayOrNight == DayOrNight.Day ? "Day time" : (this.block.DayOrNight == DayOrNight.Night ? "Night time" : "Always"));
      }
    }

    private string TimerText
    {
      get
      {
        return "Timer: " + this.block.SpawnFrequency.ToString();
      }
    }

    private string MaxActiveInstancesText
    {
      get
      {
        string str = this.block.MaxActiveInstances.ToString();
        if (this.block.MaxActiveInstances >= this.maxInstances)
          str += " (Max)";
        return "Max Active Instances: " + str;
      }
    }

    private string PowerText
    {
      get
      {
        return "Requires Power: " + (this.block.RequiresPower ? "Yes" : "No");
      }
    }

    private string CombatStatsText
    {
      get
      {
        return string.Format("Combat Stats: {0}  [Level: {1}]", this.IsCustomStats ? (object) "Custom" : (object) "Default", (object) SkillData.CombatLevel(this.block.CombatStats));
      }
    }

    private string LootText
    {
      get
      {
        return string.Format("Loot Table: {0}", this.block.LootTable.Count > 0 ? (object) "Custom" : (object) "Default");
      }
    }

    private string InventoryText
    {
      get
      {
        return string.Format("Inventory: {0}", this.block.Inventory == null || !this.block.Inventory.HasItems() ? (object) "Empty" : (object) "Custom");
      }
    }

    private string KillScriptText
    {
      get
      {
        return "Kill Script: " + this.block.KillScript;
      }
    }

    private bool IsCustomStats
    {
      get
      {
        if (this.block.ActorType != ActorType.None)
          return !this.block.CombatStats.IsEqual(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) this.block.ActorType].LevelType]);
        return false;
      }
    }

    private string ShowOwnerText
    {
      get
      {
        return "Show Owner: " + (this.block.ShowOwnerData ? "Yes" : "No");
      }
    }

    private string TextureText
    {
      get
      {
        Block textureIdForDrawing = this.instance.Map.GetBlockTextureIDForDrawing(Block.NPCSpawn, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point));
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

    public NPCSpawnScreen(GameInstance instance, Player player, GlobalPoint3D p)
      : base("Mob Spawn", player)
    {
      NPCSpawnScreen npcSpawnScreen = this;
      this.instance = instance;
      this.block = instance.MapStrategyTM.AddNpcSpawnBlock(p, UpdateBlockMethod.Player);
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Type: " + this.block.ActorType.ToString()));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.NameText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BehaviourText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.DialogText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.MaxActiveInstancesText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TimerText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TimeOfDayText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.CombatStatsText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.LootText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.InventoryText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.KillScriptText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.PowerText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.ShowOwnerText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.TextureText));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new UnlockablesScreen(instance, player, (Unlockable) null, false, false, new Action<Player, ActorType>(npcSpawnScreen.OnActorTypeSelected), npcSpawnScreen.block.ActorType, (GameScreen) null), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(this.EditName);
      blockMenuEntryList[2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new BehaviourMenuScreen(instance, player, npcSpawnScreen.block), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new BehaviourMenuScreen(instance, player, npcSpawnScreen.block), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(npcSpawnScreen.OnMaxActiveInstancesEntered), npcSpawnScreen.block.MaxActiveInstances, false), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(npcSpawnScreen.OnSpawnFrequencyEntered), npcSpawnScreen.block.SpawnFrequency, true, false), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[6].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.DayOrNight = this.block.DayOrNight != DayOrNight.None ? (this.block.DayOrNight != DayOrNight.Day ? DayOrNight.None : DayOrNight.Night) : DayOrNight.Day;
        this.RefreshMenuText();
      });
      blockMenuEntryList[7].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        CombatStats reset = new CombatStats();
        reset.SetFromXML(Globals1.NpcLevelData[(int) Globals1.NpcTypeData[(int) npcSpawnScreen.block.ActorType].LevelType]);
        npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new CombatStatsEntryScreen(player, npcSpawnScreen.block.CombatStats, reset, new Action<CombatStats>(npcSpawnScreen.OnCustomStatsEntered)), npcSpawnScreen.ControllingPlayer);
      });
      blockMenuEntryList[8].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new LootTableScreen(player, npcSpawnScreen.block.LootTable, new Action(npcSpawnScreen.OnLootTableEntered)), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[9].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (npcSpawnScreen.block.Inventory == null)
          npcSpawnScreen.block.Inventory = new Inventory(10, 0, 0);
        npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new ShopScreen(instance, player, npcSpawnScreen.block.Inventory, new Action(npcSpawnScreen.OnInventoryEntered)), npcSpawnScreen.ControllingPlayer);
      });
      blockMenuEntryList[10].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => npcSpawnScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, npcSpawnScreen.block.KillScript, new ListBoxScreen.OnMenuItemSelected(npcSpawnScreen.OnKillScriptSelected), false, true), npcSpawnScreen.ControllingPlayer));
      blockMenuEntryList[11].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        this.block.RequiresPower = !this.block.RequiresPower;
        this.RefreshMenuText();
      });
      blockMenuEntryList[12].Selected += new EventHandler<PlayerIndexEventArgs>(this.ShowOwner);
      blockMenuEntryList[13].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnTextureSelected);
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      blockMenuEntryList[8].IsEnabled = instance.IsCreativeMode;
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.RefreshMenuItemAccess();
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
      this.MenuEntries[10].ToolTip.Text = "The selected script will be executed every time a player opens the NPCs dialogue screen.";
      this.MenuEntries[11].ToolTip.Text = "The selected script will be executed every time a mob that spawned from this block is killed.";
    }

    protected override void OnScreenRemovedCore()
    {
      this.instance.NetworkManager.BlockTextureChangedReceived -= new BlockEventHandler(this.BlockTextureChanged);
      base.OnScreenRemovedCore();
      if (this.block.ActorType != ActorType.None)
        this.instance.MapStrategyTM.AddDataBlock((DataBlock) this.block, UpdateBlockMethod.Player, true);
      this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.block, this.block.ActorType == ActorType.None);
    }

    private void BlockTextureChanged(object sender, BlockEventArgs e)
    {
      this.MenuEntries[this.MenuEntries.Count - 2].Text = this.TextureText;
    }

    private void RefreshMenuText()
    {
      this.MenuEntries[0].Text = "Type: " + this.block.ActorType.ToString();
      this.MenuEntries[1].Text = this.NameText;
      this.MenuEntries[2].Text = this.BehaviourText;
      this.MenuEntries[3].Text = this.DialogText;
      this.MenuEntries[4].Text = this.MaxActiveInstancesText;
      this.MenuEntries[5].Text = this.TimerText;
      this.MenuEntries[6].Text = this.TimeOfDayText;
      this.MenuEntries[7].Text = this.CombatStatsText;
      this.MenuEntries[8].Text = this.LootText;
      this.MenuEntries[9].Text = this.InventoryText;
      this.MenuEntries[10].Text = this.KillScriptText;
      this.MenuEntries[11].Text = this.PowerText;
      this.MenuEntries[12].Text = this.ShowOwnerText;
      this.MenuEntries[13].Text = this.TextureText;
    }

    private void RefreshMenuItemAccess()
    {
      bool hasPermission = this.HasPermission;
      bool flag1 = this.player != null && this.block.OwnerGamertag == this.player.Gamertag;
      bool flag2 = !this.block.ShowOwnerData || flag1;
      this.MenuEntries[0].IsEnabled = hasPermission;
      this.MenuEntries[1].IsEnabled = hasPermission & flag2;
      this.MenuEntries[3].IsEnabled = hasPermission & flag2;
      this.MenuEntries[9].IsEnabled = hasPermission;
      this.MenuEntries[10].IsEnabled = this.player.HasPermissionAny(Permissions.Admin | Permissions.ViewScripts) & flag2;
      this.MenuEntries[12].IsEnabled = flag1;
      this.MenuEntries[13].IsEnabled = hasPermission;
    }

    private void OnActorTypeSelected(Player player, ActorType actorType)
    {
      if (player == null || !this.HasPermission || this.block.ActorType == actorType)
        return;
      this.block.SetActorType(actorType);
      this.block.Name = actorType.ToString();
      this.block.ShowOwnerData = false;
      this.block.DialogText = (string) null;
      this.block.DialogTree = "System\\Dialog\\" + actorType.ToString();
      this.block.OwnerGamertag = player.Gamertag;
      this.block.OwnerHasAvatarUnlocked = player.Unlockables == null || !player.Unlockables.GetUnlockable(actorType).IsUnlocked ? UnlockType.Locked : UnlockType.Unlocked;
      this.RefreshMenuText();
      this.RefreshMenuItemAccess();
    }

    private void EditName(object sender, PlayerIndexEventArgs e)
    {
      int num = this.HasPermission ? 1 : 0;
    }

    private void OnNameEntered(string text, object notused)
    {
      if (text == null || text.Length <= 0)
        return;
      if (text.Length > 50)
        text = text.Substring(0, 50);
      this.block.Name = Utils.StripChars(text, 32, 160);
      this.RefreshMenuText();
    }

    private void ShowOwner(object sender, PlayerIndexEventArgs e)
    {
      if (!this.HasPermission)
        return;
      this.block.ShowOwnerData = !this.block.ShowOwnerData;
      this.RefreshMenuText();
    }

    private void OnSpawnFrequencyEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.block.SpawnFrequency = number < 2.0 ? 2f : (float) number;
      this.MenuEntries[this.selectedEntry].Text = this.TimerText;
    }

    private void OnMaxActiveInstancesEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.block.MaxActiveInstances = MyMathHelper.Clamp((int) number, 0, this.maxInstances);
      this.MenuEntries[this.selectedEntry].Text = this.MaxActiveInstancesText;
    }

    private void OnCustomStatsEntered(CombatStats stats)
    {
      this.block.CombatStats = stats;
      this.RefreshMenuText();
    }

    private void OnLootTableEntered()
    {
      this.RefreshMenuText();
    }

    private void OnInventoryEntered()
    {
      this.RefreshMenuText();
    }

    private void OnTextureSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.SelectTextureBlockCallBack), "Select Block Texture", BlockSelectMode.SelectingBlockTexture, Block.NPCSpawn, (int) this.instance.Map.GetAuxHighDataNoCache(this.block.Point)), this.ControllingPlayer);
    }

    private bool SelectTextureBlockCallBack(Player player, Block textureID)
    {
      if (textureID == Block.None || this.instance.Map.ChangeBlockTexture(player, this.block.Point, Block.NPCSpawn, textureID) == MapTM.BlockTextureChangeResult.None && this.instance.Map.IsHost)
        return false;
      this.RefreshMenuText();
      return true;
    }

    private bool OnKillScriptSelected(MenuEntry script)
    {
      if (this.player.IsAdmin)
      {
        this.block.KillScript = script == null ? (string) null : (string) script.Tag + script.Text;
        this.RefreshMenuText();
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
