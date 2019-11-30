// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.QuestLoadScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class QuestLoadScreen : BlockMenuScreen
  {
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private bool drawCalled;
    private float drawTimer;
    private bool questsLoaded;
    private string questToDelete;
    private GameInstance instance;

    public QuestLoadScreen(GameInstance instance, Player player)
      : base("Load Quest", player)
    {
      this.instance = instance;
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "Loading Quests..."));
    }

    private int SortItems(MenuEntry e1, MenuEntry e2)
    {
      return e1.Text.CompareTo(e2.Text);
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.ItemsPerPage = 10;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 32, 0, this.arrowTexture.Width, this.arrowTexture.Height);
    }

    private void NewQuestMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "New Quest", "Enter a name for the new Quest (max of 20 characters).", "", new AsyncCallback(this.EndShowKeyboardForNewQuest), (object) null);
    }

    private void EndShowKeyboardForNewQuest(IAsyncResult ar)
    {
      string questName = Globals2.StripFolderName(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (questName.Length <= 0)
        return;
      if (questName.Length > 20)
        questName = questName.Substring(0, 20);
      this.ScreenManager.AddScreen((GameScreen) new QuestEditScreen(this.instance, this.player, questName), this.ControllingPlayer);
    }

    private void OpenQuestMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new QuestEditScreen(this.instance, this.player, (string) this.MenuEntries[this.selectedEntry].Tag), this.ControllingPlayer);
    }

    private void DeleteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.questToDelete = (string) this.MenuEntries[this.selectedEntry].Tag;
      MessageBoxScreenTM messageBoxScreenTm1 = new MessageBoxScreenTM("Confirm deletion of " + this.questToDelete, "Yes Delete it", (string) null, (string) null, "No don't delete it", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm1.TransitionOnTime = TimeSpan.FromSeconds(0.5);
      MessageBoxScreenTM messageBoxScreenTm2 = messageBoxScreenTm1;
      messageBoxScreenTm2.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnDeleteQuest);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm2, new PlayerIndex?(e.PlayerIndex));
    }

    private void OnDeleteQuest(object sender, PlayerIndexEventArgs e)
    {
      ((MessageBoxScreen) sender).ButtonA -= new EventHandler<PlayerIndexEventArgs>(this.OnDeleteQuest);
      this.DeleteQuest(this.questToDelete);
      this.ScreenManager.AddScreen((GameScreen) new QuestLoadScreen(this.instance, this.player), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    private void DeleteQuest(string questName)
    {
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.drawCalled || this.questsLoaded)
        return;
      this.LoadQuests();
      this.questsLoaded = true;
    }

    private void LoadQuests()
    {
      List<MenuEntry> items = new List<MenuEntry>();
      try
      {
        this.LoadQuestFiles(items);
      }
      catch (InvalidOperationException ex)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: Not all Quests could be loaded", "OK", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      finally
      {
        items.Sort(new Comparison<MenuEntry>(this.SortItems));
        this.MenuEntries.Clear();
        BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, "Create New Quest");
        blockMenuEntry1.Selected += new EventHandler<PlayerIndexEventArgs>(this.NewQuestMenuEntrySelected);
        blockMenuEntry1.LoadContent();
        items.Insert(0, (MenuEntry) blockMenuEntry1);
        this.MenuEntries.AddRange((IEnumerable<MenuEntry>) items);
        BlockMenuEntry blockMenuEntry2 = new BlockMenuEntry((BlockMenuScreen) this, "Back");
        blockMenuEntry2.Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
        blockMenuEntry2.LoadContent();
        this.MenuEntries.Add((MenuEntry) blockMenuEntry2);
        this.ResetMenuRect();
      }
    }

    private void LoadQuestFiles(List<MenuEntry> items)
    {
    }

    protected override void DrawBackground()
    {
      this.drawCalled = true;
      this.drawTimer += Services.ElapsedTime;
      base.DrawBackground();
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 160, this.MenuRect.Y + this.MenuRect.Height - 36, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Delete", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (this.itemAtTopOfPage > 0)
      {
        this.arrowRect.Y = this.MenuRect.Y + 64;
        this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
      }
      if (this.itemAtTopOfPage + this.ItemsPerPage >= this.MenuEntries.Count)
        return;
      this.arrowRect.Y = this.MenuRect.Y + this.MenuRect.Height - 80;
      this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, Color.White);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
