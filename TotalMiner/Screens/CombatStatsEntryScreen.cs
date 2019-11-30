// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CombatStatsEntryScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CombatStatsEntryScreen : BlockMenuScreen
  {
    private CombatStats stats;
    private Action<CombatStats> onEntered;

    public CombatStatsEntryScreen(
      Player player,
      CombatStats stats,
      CombatStats reset,
      Action<CombatStats> onEntered)
      : base("Combat Stats", player)
    {
      CombatStatsEntryScreen statsEntryScreen = this;
      this.stats = stats;
      this.onEntered = onEntered;
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Reset"));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[0].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => statsEntryScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(statsEntryScreen.OnHealthEntered), statsEntryScreen.stats.HealthLevel, false), statsEntryScreen.ControllingPlayer));
      blockMenuEntryList[1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => statsEntryScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(statsEntryScreen.OnStrengthEntered), statsEntryScreen.stats.StrengthLevel, false), statsEntryScreen.ControllingPlayer));
      blockMenuEntryList[2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => statsEntryScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(statsEntryScreen.OnAttackEntered), statsEntryScreen.stats.AttackLevel, false), statsEntryScreen.ControllingPlayer));
      blockMenuEntryList[3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => statsEntryScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(statsEntryScreen.OnDefenceEntered), statsEntryScreen.stats.DefenceLevel, false), statsEntryScreen.ControllingPlayer));
      blockMenuEntryList[4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => statsEntryScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(statsEntryScreen.OnRangedEntered), statsEntryScreen.stats.RangedLevel, false), statsEntryScreen.ControllingPlayer));
      blockMenuEntryList[5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        statsEntryScreen.stats = reset;
        statsEntryScreen.ResetToggleItems();
      });
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.ResetToggleItems();
    }

    private void OnHealthEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.stats.HealthLevel = Math.Min(9999, Math.Max(1, (int) value));
      this.ResetToggleItems();
    }

    private void OnStrengthEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.stats.StrengthLevel = Math.Min(9999, Math.Max(1, (int) value));
      this.ResetToggleItems();
    }

    private void OnAttackEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.stats.AttackLevel = Math.Min(9999, Math.Max(1, (int) value));
      this.ResetToggleItems();
    }

    private void OnDefenceEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.stats.DefenceLevel = Math.Min(9999, Math.Max(1, (int) value));
      this.ResetToggleItems();
    }

    private void OnRangedEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.stats.RangedLevel = Math.Min(9999, Math.Max(1, (int) value));
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = "Health Level: " + (object) this.stats.HealthLevel + "  (Hit Points: " + (object) SkillData.MaxHealth(this.stats.HealthLevel) + ")";
      this.MenuEntries[1].Text = "Strength Level: " + (object) this.stats.StrengthLevel;
      this.MenuEntries[2].Text = "Attack Level: " + (object) this.stats.AttackLevel;
      this.MenuEntries[3].Text = "Defense Level: " + (object) this.stats.DefenceLevel;
      this.MenuEntries[4].Text = "Ranged Level: " + (object) this.stats.RangedLevel;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 526;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.onEntered == null)
        return;
      this.onEntered(this.stats);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
