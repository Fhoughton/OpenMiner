// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SkillRankScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class SkillRankScreen : BlockMenuScreen
  {
    private string gamertag;
    private bool isTotal;
    private bool isCombat;
    private int topOfPageIndex;
    private int maxItemsPerPage;
    private int meIndex;
    private SkillType skillType;
    private List<HighScoreSkillRank> table;
    private SkillsScreen parentScreen;

    public SkillRankScreen(
      SkillsScreen parentScreen,
      SkillType skillType,
      string gamertag,
      bool isTotal,
      bool isCombat)
      : base("Skills List", (Player) null)
    {
      this.parentScreen = parentScreen;
      this.gamertag = gamertag;
      this.skillType = skillType;
      this.isTotal = isTotal;
      this.isCombat = isCombat;
      BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, (string) null);
      blockMenuEntry1.LoadContent();
      this.MenuEntries.Add((MenuEntry) blockMenuEntry1);
      BlockMenuEntry blockMenuEntry2 = new BlockMenuEntry((BlockMenuScreen) this, "  Gamertag                         Level                     XP             Rank");
      blockMenuEntry2.LoadContent();
      this.MenuEntries.Add((MenuEntry) blockMenuEntry2);
      this.LoadTable();
      this.maxItemsPerPage = 21;
      this.meIndex = this.GetGamertagIndex(gamertag);
      int num = Math.Min(this.maxItemsPerPage / 2, this.meIndex);
      this.topOfPageIndex = this.table.Count <= this.maxItemsPerPage ? 0 : Math.Max(0, this.meIndex - num);
      this.selectedEntry = this.topOfPageIndex == 0 ? this.meIndex + 2 : (this.meIndex >= 0 ? num + 2 : 2);
      this.LoadPage();
    }

    private int GetGamertagIndex(string gamertag)
    {
      for (int index = 0; index < this.table.Count; ++index)
      {
        if (this.table[index].Gamertag == gamertag)
          return index;
      }
      return -1;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 768;
      this.ItemHeight = 24;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.5f;
      this.ItemsPerPage = this.MenuEntries.Count;
      this.DrawItemLines = this.DrawEntryLines = true;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.LoadHeaders();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return !this.parentScreen.IsServer && !this.CanDeleteEntries ? 0 : 38;
      }
    }

    private bool CanDeleteEntries
    {
      get
      {
        return false;
      }
    }

    private void LoadTable()
    {
      this.table = this.isCombat ? Globals2.GamertagData.GetSkillCombatSortedRank(Globals2.GamertagData.HighScoreData, this.parentScreen.IsServer) : (this.isTotal ? Globals2.GamertagData.GetSkillTotalSortedRank(Globals2.GamertagData.HighScoreData, this.parentScreen.IsServer) : Globals2.GamertagData.GetSkillSortedRank(Globals2.GamertagData.HighScoreData, this.skillType, this.parentScreen.IsServer));
      if (this.table != null)
        return;
      this.table = new List<HighScoreSkillRank>();
    }

    private void LoadHeaders()
    {
      string text = "  Skill: " + (this.isTotal ? "Totals" : (this.isCombat ? "Combat" : this.skillType.ToString()));
      while ((double) this.ItemFont.MeasureString(text).X * (double) this.ItemTextScale < 490.0)
        text += " ";
      this.MenuEntries[0].Text = text + "Total Ranks: " + this.table.Count.ToString();
    }

    private void LoadPage()
    {
      for (int index = this.MenuEntries.Count - 1; index > 1; --index)
        this.MenuEntries.RemoveAt(index);
      for (int topOfPageIndex = this.topOfPageIndex; topOfPageIndex < this.topOfPageIndex + this.maxItemsPerPage && topOfPageIndex < this.table.Count; ++topOfPageIndex)
      {
        SkillRankMenuEntry newItem = this.GetNewItem(topOfPageIndex);
        newItem.LoadContent();
        this.MenuEntries.Add((MenuEntry) newItem);
      }
    }

    private SkillRankMenuEntry GetNewItem(int index)
    {
      SkillRankMenuEntry skillRankMenuEntry = new SkillRankMenuEntry(this, this.skillType, this.table[index], !this.isTotal);
      if (index == this.meIndex)
      {
        skillRankMenuEntry.ColorOverride = Color.Yellow;
        skillRankMenuEntry.OverrideColor = true;
      }
      skillRankMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnBrowseNewPlayer);
      return skillRankMenuEntry;
    }

    public override bool HandleInput(InputState input)
    {
      if (input.IsNewButtonPress(Buttons.LeftTrigger) || input.IsNewButtonPress(Buttons.LeftShoulder))
      {
        this.PageUp();
        return true;
      }
      if (!input.IsNewButtonPress(Buttons.RightTrigger) && !input.IsNewButtonPress(Buttons.RightShoulder))
        return base.HandleInput(input);
      this.PageDown();
      return true;
    }

    protected override void OnSelectUpCore(PlayerIndex playerIndex)
    {
      if (this.selectedEntry > 2)
      {
        --this.selectedEntry;
      }
      else
      {
        if (this.topOfPageIndex <= 0)
          return;
        --this.topOfPageIndex;
        SkillRankMenuEntry newItem = this.GetNewItem(this.topOfPageIndex);
        newItem.LoadContent();
        this.MenuEntries.Insert(2, (MenuEntry) newItem);
        this.MenuEntries.RemoveAt(this.MenuEntries.Count - 1);
      }
    }

    protected override void OnSelectDownCore(PlayerIndex playerIndex)
    {
      if (this.selectedEntry < this.MenuEntries.Count - 1)
      {
        ++this.selectedEntry;
      }
      else
      {
        if (this.topOfPageIndex + this.maxItemsPerPage >= this.table.Count)
          return;
        ++this.topOfPageIndex;
        SkillRankMenuEntry newItem = this.GetNewItem(this.topOfPageIndex + this.maxItemsPerPage - 1);
        newItem.LoadContent();
        this.MenuEntries.RemoveAt(2);
        this.MenuEntries.Add((MenuEntry) newItem);
      }
    }

    private void OnBrowseNewPlayer(object sender, PlayerIndexEventArgs e)
    {
      if (this.parentScreen == null)
        return;
      SkillRankMenuEntry menuEntry = this.MenuEntries[this.selectedEntry] as SkillRankMenuEntry;
      if (menuEntry == null)
        return;
      this.parentScreen.ChangeGamer(menuEntry.Rank.Gamertag);
      this.ExitScreen();
    }

    private void OnDeleteEntry(object sender, PlayerIndexEventArgs e)
    {
      if (this.parentScreen == null)
        return;
      SkillRankMenuEntry menuEntry = this.MenuEntries[this.selectedEntry] as SkillRankMenuEntry;
      if (menuEntry == null)
        return;
      Globals2.GamertagData.HighScoreData.HighScores.Remove(menuEntry.Rank.Gamertag);
      this.LoadTable();
      this.LoadHeaders();
      this.LoadPage();
    }

    private void PageUp()
    {
      if (this.selectedEntry > 2)
      {
        this.selectedEntry = 2;
      }
      else
      {
        if (this.topOfPageIndex <= 0)
          return;
        this.topOfPageIndex = Math.Max(0, this.topOfPageIndex - this.maxItemsPerPage);
        this.LoadPage();
      }
    }

    private void PageDown()
    {
      if (this.selectedEntry < this.MenuEntries.Count - 1)
      {
        this.selectedEntry = this.MenuEntries.Count - 1;
      }
      else
      {
        if (this.topOfPageIndex + this.maxItemsPerPage >= this.table.Count)
          return;
        this.topOfPageIndex = Math.Min(this.table.Count - this.maxItemsPerPage, this.topOfPageIndex + this.maxItemsPerPage);
        this.LoadPage();
      }
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, this.MenuRect.Y + 50, this.MenuRect.Width, 1), Color.White);
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 245, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      if (!this.CanDeleteEntries)
        return;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Delete Entry", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }
  }
}
