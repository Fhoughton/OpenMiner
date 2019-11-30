// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.TextMessageViewLogScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class TextMessageViewLogScreen : BlockMenuScreen
  {
    private int logCount;
    private int tempSel;
    private GameInstance instance;

    private string FilterTypeText
    {
      get
      {
        return "Filter: " + (this.player != null ? Utils.InsertSpacesBeforeCapitals(this.player.TextMessageFilterType.ToString()) : "None");
      }
    }

    public TextMessageViewLogScreen(GameInstance instance, Player player)
      : this(instance, player, (string) null)
    {
    }

    protected TextMessageViewLogScreen(GameInstance instance, Player player, string selected)
      : base("View Text Message Log", player)
    {
      this.instance = instance;
      this.ItemTextScale = 0.4f;
      this.tempSel = -1;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, this.FilterTypeText);
      blockMenuEntry1.TextOffsetEx = new Vector2(-16f, 0.0f);
      BlockMenuEntry blockMenuEntry2 = blockMenuEntry1;
      blockMenuEntryList2.Add(blockMenuEntry2);
      blockMenuEntryList1[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.FilterTypeSelected);
      for (int index = instance.ChatLog.Count - 1; index >= 0; --index)
      {
        string text = instance.ChatLog[index];
        bool flag = true;
        switch (player.TextMessageFilterType)
        {
          case TextMessageViewLogScreen.FilterType.PrivateMessages:
            int num;
            if (!this.IsMsgSentTo(text, "You"))
              num = this.IsMsgNotSentTo(text, "All", "Admins", "Clan") ? 1 : 0;
            else
              num = 1;
            flag = num != 0;
            break;
          case TextMessageViewLogScreen.FilterType.PublicMessages:
            flag = this.IsMsgSentTo(text, "All");
            break;
          case TextMessageViewLogScreen.FilterType.ClanMessages:
            flag = this.IsMsgSentTo(text, "Clan");
            break;
          case TextMessageViewLogScreen.FilterType.AdminMessages:
            flag = this.IsMsgSentTo(text, "Admins");
            break;
          case TextMessageViewLogScreen.FilterType.AllMessages:
            flag = !text.StartsWith("[Notify ", StringComparison.OrdinalIgnoreCase);
            break;
          case TextMessageViewLogScreen.FilterType.AdminNotifications:
            flag = text.StartsWith("[Notify to Admins]", StringComparison.OrdinalIgnoreCase);
            break;
          case TextMessageViewLogScreen.FilterType.AllNotifications:
            flag = text.StartsWith("[Notify ", StringComparison.OrdinalIgnoreCase);
            break;
        }
        if (flag)
        {
          TextMessageLogMenuEntry messageLogMenuEntry1 = new TextMessageLogMenuEntry((BlockMenuScreen) this, text);
          messageLogMenuEntry1.TextOffsetEx = new Vector2(-16f, 0.0f);
          TextMessageLogMenuEntry messageLogMenuEntry2 = messageLogMenuEntry1;
          blockMenuEntryList1.Add((BlockMenuEntry) messageLogMenuEntry2);
          if (this.tempSel < 0 && selected != null && selected == instance.ChatLog[index])
            this.tempSel = blockMenuEntryList1.Count - 1;
        }
      }
      int num1 = 5 - blockMenuEntryList1.Count;
      for (int index = 0; index < num1; ++index)
      {
        List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
        BlockMenuEntry blockMenuEntry3 = new BlockMenuEntry((BlockMenuScreen) this, (string) null);
        blockMenuEntry3.IsEnabled = false;
        BlockMenuEntry blockMenuEntry4 = blockMenuEntry3;
        blockMenuEntryList3.Add(blockMenuEntry4);
      }
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry5 = new BlockMenuEntry((BlockMenuScreen) this, "Clear Chat Log");
      blockMenuEntry5.TextOffsetEx = new Vector2(-16f, 0.0f);
      BlockMenuEntry blockMenuEntry6 = blockMenuEntry5;
      blockMenuEntryList4.Add(blockMenuEntry6);
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.ClearChatLogSelected);
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      BlockMenuEntry blockMenuEntry7 = new BlockMenuEntry((BlockMenuScreen) this, "Close");
      blockMenuEntry7.TextOffsetEx = new Vector2(-16f, 0.0f);
      BlockMenuEntry blockMenuEntry8 = blockMenuEntry7;
      blockMenuEntryList5.Add(blockMenuEntry8);
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.logCount = instance.ChatLog.Count;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = false;
      this.DrawPanel = true;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      this.ItemHeight = 20;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.5f;
      this.ItemsPerPage = 26;
      base.LoadContent();
      if (this.tempSel < 0)
        return;
      this.SetSelectedEntry(this.tempSel);
    }

    protected override int MenuRectWidthExt
    {
      get
      {
        return 1054 - this.HighlightRect.Width;
      }
    }

    protected override void ResetMenuRect()
    {
      base.ResetMenuRect();
      this.PanelRect.X += 2;
      this.PanelRect.Width -= 2;
    }

    private bool IsMsgNotSentTo(string text, params string[] recipients)
    {
      if (recipients != null)
      {
        foreach (string recipient in recipients)
        {
          if (this.IsMsgSentTo(text, recipient))
            return false;
        }
      }
      return true;
    }

    private bool IsMsgSentTo(string text, string recipient)
    {
      int num1 = 0;
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] == ']')
        {
          num1 = index;
          break;
        }
      }
      int num2 = 0;
      for (int index = num1 - 1; index > 0; --index)
      {
        if (text[index] == '>')
        {
          num2 = index;
          break;
        }
      }
      if (num1 - num2 > 2)
        return text.Substring(num2 + 2, num1 - num2 - 2).Equals(recipient, StringComparison.OrdinalIgnoreCase);
      return false;
    }

    private void FilterTypeSelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.player == null)
        return;
      if (this.player.TextMessageFilterType >= TextMessageViewLogScreen.FilterType.AllNotifications)
        this.player.TextMessageFilterType = TextMessageViewLogScreen.FilterType.None;
      else
        ++this.player.TextMessageFilterType;
      this.RestartScreen();
    }

    private void ClearChatLogSelected(object sender, PlayerIndexEventArgs e)
    {
      this.instance.ChatLog.Clear();
      this.logCount = this.instance.ChatLog.Count;
      this.ExitScreen();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.instance.ChatLog.Count == this.logCount)
        return;
      this.RestartScreen();
    }

    private void RestartScreen()
    {
      this.ExitScreen();
      this.ScreenManager.AddScreen((GameScreen) new TextMessageViewLogScreen(this.instance, this.player, this.selectedEntry >= 0 ? this.MenuEntries[this.selectedEntry].Text : (string) null), this.ControllingPlayer);
    }

    protected override void DrawCore()
    {
      base.DrawCore();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    public enum FilterType
    {
      None,
      PrivateMessages,
      PublicMessages,
      ClanMessages,
      AdminMessages,
      AllMessages,
      AdminNotifications,
      AllNotifications,
    }
  }
}
