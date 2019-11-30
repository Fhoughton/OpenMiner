// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ZoneEditOtherScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class ZoneEditOtherScreen : BlockMenuScreen
  {
    private Zone zone;
    private Parser parser;
    private GameInstance instance;
    private ZoneMenuEntry menuEntry;

    private string BoundText
    {
      get
      {
        return string.Format("Region: [{0}, {1}, {2}] [{3}, {4}, {5}]", (object) this.zone.Min.X, (object) this.zone.Min.Y, (object) this.zone.Min.Z, (object) this.zone.Max.X, (object) this.zone.Max.Y, (object) this.zone.Max.Z);
      }
    }

    private string EntryScriptText
    {
      get
      {
        return "Entry Script: " + this.zone.OnEntryScriptName;
      }
    }

    private string ExitScriptText
    {
      get
      {
        return "Exit Script: " + this.zone.OnExitScriptName;
      }
    }

    private string BuilderText
    {
      get
      {
        return "Builder: " + this.zone.Builder + " (" + (object) this.zone.BuilderType + ")";
      }
    }

    private string CombatLevelDiffText
    {
      get
      {
        return "Combat Level Difference: " + (this.zone.CombatLevelDifference > (short) 0 ? this.zone.CombatLevelDifference.ToString() : "Inactive");
      }
    }

    private string SpeedModifierText
    {
      get
      {
        return "Speed Multiplier: " + ((double) this.zone.SpeedMultiplier != 1.0 ? this.zone.SpeedMultiplier.ToString("N3") : "Inactive");
      }
    }

    private string GravityModifierText
    {
      get
      {
        return "Gravity Multiplier: " + ((double) this.zone.GravityMultiplier != 1.0 ? this.zone.GravityMultiplier.ToString("N3") : "Inactive");
      }
    }

    private string EscapeOptionText
    {
      get
      {
        return "Escape Option: " + (this.zone.HasZoneType(ZoneType.NoEscape) ? "Disabled" : "Enabled");
      }
    }

    public ZoneEditOtherScreen(
      GameInstance instance,
      Player player,
      Zone zone,
      ZoneMenuEntry menuEntry)
      : base("Script", player)
    {
      ZoneEditOtherScreen zoneEditOtherScreen = this;
      this.instance = instance;
      this.zone = zone;
      this.menuEntry = menuEntry;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BoundText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BuilderText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EntryScriptText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.ExitScriptText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.CombatLevelDiffText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.SpeedModifierText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.GravityModifierText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, this.EscapeOptionText));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += new EventHandler<PlayerIndexEventArgs>(this.SelectRegion);
      blockMenuEntryList1[num2 - 1].IsEnabled = player.IsGod || !instance.IsDigDeepMode || Globals2.GameProperties.SaveGame.Header.DepthReached == instance.Map.MapHeight;
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("Choose Builder Type", "Clan", "Player", "No Builder", (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player);
        messageBoxScreenTm.ButtonA += new EventHandler<PlayerIndexEventArgs>(zoneEditOtherScreen.SelectClanBuilder);
        messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(zoneEditOtherScreen.SelectPlayerBuilder);
        messageBoxScreenTm.ButtonY += new EventHandler<PlayerIndexEventArgs>(zoneEditOtherScreen.SelectNoBuilder);
        zoneEditOtherScreen.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, zoneEditOtherScreen.ControllingPlayer);
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => zoneEditOtherScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, zone.OnEntryScriptName, new ListBoxScreen.OnMenuItemSelected(zoneEditOtherScreen.OnEntryScriptSelected), false, true), zoneEditOtherScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => zoneEditOtherScreen.ScreenManager.AddScreen((GameScreen) new ScriptListMenuScreen(instance, player, zone.OnExitScriptName, new ListBoxScreen.OnMenuItemSelected(zoneEditOtherScreen.OnExitScriptSelected), false, true), zoneEditOtherScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList6 = blockMenuEntryList1;
      int index5 = num5;
      int index6 = index5 + 1;
      blockMenuEntryList6[index5].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => zoneEditOtherScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(zoneEditOtherScreen.OnCombatLevelDifferenceEntered), (int) zone.CombatLevelDifference, false), zoneEditOtherScreen.ControllingPlayer));
      blockMenuEntryList1[index6].IsEnabled = instance.IsCreativeMode;
      List<BlockMenuEntry> blockMenuEntryList7 = blockMenuEntryList1;
      int index7 = index6;
      int index8 = index7 + 1;
      blockMenuEntryList7[index7].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => zoneEditOtherScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(zoneEditOtherScreen.OnSpeedModifierEntered), zone.SpeedMultiplier, true, true), zoneEditOtherScreen.ControllingPlayer));
      blockMenuEntryList1[index8].IsEnabled = instance.IsCreativeMode;
      List<BlockMenuEntry> blockMenuEntryList8 = blockMenuEntryList1;
      int index9 = index8;
      int num6 = index9 + 1;
      blockMenuEntryList8[index9].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) => zoneEditOtherScreen.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(player, new NumberEntered(zoneEditOtherScreen.OnGravityModifierEntered), zone.GravityMultiplier, true, true), zoneEditOtherScreen.ControllingPlayer));
      List<BlockMenuEntry> blockMenuEntryList9 = blockMenuEntryList1;
      int index10 = num6;
      int num7 = index10 + 1;
      blockMenuEntryList9[index10].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        zone.ToggleType(ZoneType.NoEscape);
        zoneEditOtherScreen.ResetItemText();
      });
      List<BlockMenuEntry> blockMenuEntryList10 = blockMenuEntryList1;
      int index11 = num7;
      int num8 = index11 + 1;
      blockMenuEntryList10[index11].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
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
    }

    private void ResetItemText()
    {
      this.MenuEntries[1].Text = this.BuilderText;
      this.MenuEntries[2].Text = this.EntryScriptText;
      this.MenuEntries[3].Text = this.ExitScriptText;
      this.MenuEntries[4].Text = this.CombatLevelDiffText;
      this.MenuEntries[5].Text = this.SpeedModifierText;
      this.MenuEntries[6].Text = this.GravityModifierText;
      this.MenuEntries[7].Text = this.EscapeOptionText;
    }

    private void SelectPlayerBuilder(object sender, EventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.instance.GetLocalPlayer(this.ControllingPlayer.Value), new Action<NetworkGamer, bool, string>(this.OnPlayerBuilderSelected), true, (string) null, false, false), this.ControllingPlayer);
    }

    private void OnPlayerBuilderSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      if (gamer != null)
      {
        this.zone.BuilderType = ZoneBuilderType.Player;
        if (this.zone.Builder != gamer.Gamertag)
        {
          this.zone.Builder = gamer.Gamertag;
          this.menuEntry.IsChanged = true;
        }
      }
      this.ResetItemText();
    }

    private void SelectRegion(object sender, EventArgs e)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the region bounds", (string) null, this.MenuEntries[0].Text.Substring(8), new AsyncCallback(this.OnBoundEntered), (object) null, this.MenuEntries[0], true);
    }

    private void OnBoundEntered(IAsyncResult ar)
    {
      string command = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (command == null || command.Length <= 0)
        return;
      if (this.parser == null)
        this.parser = new Parser();
      Parser.Token token = new Parser.Token();
      Parser.Token nextToken = this.parser.GetNextToken(command, 0);
      GlobalPoint3D? pointFromToken1 = this.GetPointFromToken(nextToken);
      GlobalPoint3D? pointFromToken2 = this.GetPointFromToken(this.parser.GetNextToken(command, nextToken.EndIndex + 1));
      if (!pointFromToken1.HasValue)
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid minimum region value", this.ControllingPlayer.Value);
      else if (!pointFromToken2.HasValue)
      {
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Invalid maximum region value", this.ControllingPlayer.Value);
      }
      else
      {
        this.zone.Min = GlobalPoint3D.Min(pointFromToken1.Value, pointFromToken2.Value);
        this.zone.Max = GlobalPoint3D.Max(pointFromToken1.Value, pointFromToken2.Value);
        this.MenuEntries[0].Text = this.BoundText;
        this.instance.MapStrategyTM.UpdateZoneBound(this.zone);
      }
    }

    private GlobalPoint3D? GetPointFromToken(Parser.Token token)
    {
      string lexeme = token.Lexeme;
      if (lexeme.Length <= 4 || lexeme.IndexOf(',') == lexeme.LastIndexOf(','))
        return new GlobalPoint3D?();
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      Parser.Token nextToken1 = this.parser.GetNextToken(lexeme, 0, char.MinValue, ',');
      if (!int.TryParse(nextToken1.Lexeme, out globalPoint3D.X))
        return new GlobalPoint3D?();
      Parser.Token nextToken2 = this.parser.GetNextToken(lexeme, nextToken1.EndIndex + 1, char.MinValue, ',');
      if (!int.TryParse(nextToken2.Lexeme, out globalPoint3D.Y))
        return new GlobalPoint3D?();
      if (!int.TryParse(this.parser.GetNextToken(lexeme, nextToken2.EndIndex + 1, char.MinValue, ',').Lexeme, out globalPoint3D.Z))
        return new GlobalPoint3D?();
      MapTM map = this.instance.Map;
      globalPoint3D.Clamp(map.MapBound.Min, map.MapBound.Max - GlobalPoint3D.One);
      return new GlobalPoint3D?(globalPoint3D);
    }

    private void SelectClanBuilder(object sender, EventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ClanListScreen(this.instance.GetLocalPlayer(this.ControllingPlayer.Value), new Action<string>(this.OnClanBuilderSelected), true), this.ControllingPlayer);
    }

    private void SelectNoBuilder(object sender, EventArgs e)
    {
      this.zone.BuilderType = ZoneBuilderType.None;
      this.zone.Builder = (string) null;
      this.ResetItemText();
    }

    private void OnClanBuilderSelected(string clan)
    {
      if (clan != null && clan.Length > 0)
      {
        this.zone.BuilderType = ZoneBuilderType.Clan;
        if (this.zone.Builder != clan)
        {
          this.zone.Builder = clan;
          this.menuEntry.IsChanged = true;
        }
      }
      this.ResetItemText();
    }

    private bool OnEntryScriptSelected(MenuEntry script)
    {
      if (this.player.IsAdmin)
      {
        if (script != null)
        {
          string str = (string) script.Tag + script.Text;
          if (this.zone.OnEntryScriptName != str)
          {
            this.zone.OnEntryScriptName = str;
            this.menuEntry.IsChanged = true;
          }
        }
        else if (this.zone.OnEntryScriptName != null)
        {
          this.zone.OnEntryScriptName = (string) null;
          this.menuEntry.IsChanged = true;
        }
        this.ResetItemText();
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
          if (this.zone.OnExitScriptName != str)
          {
            this.zone.OnExitScriptName = str;
            this.menuEntry.IsChanged = true;
          }
        }
        else if (this.zone.OnExitScriptName != null)
        {
          this.zone.OnExitScriptName = (string) null;
          this.menuEntry.IsChanged = true;
        }
        this.ResetItemText();
      }
      return true;
    }

    private void OnCombatLevelDifferenceEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      short num = (short) MyMathHelper.Clamp((int) number, 0, 200);
      if ((int) num == (int) this.zone.CombatLevelDifference)
        return;
      this.zone.CombatLevelDifference = num;
      this.menuEntry.IsChanged = true;
      this.ResetItemText();
    }

    private void OnSpeedModifierEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      float num = MathHelper.Clamp((float) Math.Round(number, 3), 0.0f, 10f);
      if ((double) num == 0.0)
        num = 1f;
      if ((double) num == (double) this.zone.SpeedMultiplier)
        return;
      this.zone.SpeedMultiplier = num;
      this.menuEntry.IsChanged = true;
      this.ResetItemText();
    }

    private void OnGravityModifierEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      float num = MathHelper.Clamp((float) Math.Round(number, 3), -10f, 10f);
      if ((double) num == 0.0)
        num = 1f;
      if ((double) num == (double) this.zone.GravityMultiplier)
        return;
      this.zone.GravityMultiplier = num;
      this.menuEntry.IsChanged = true;
      this.ResetItemText();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
