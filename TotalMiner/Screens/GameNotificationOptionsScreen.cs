// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.GameNotificationOptionsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class GameNotificationOptionsScreen : BlockMenuScreen
  {
    private GameSettings settings;

    public GameNotificationOptionsScreen(GameSettings settings)
      : base("Notification Options", (Player) null)
    {
      GameNotificationOptionsScreen notificationOptionsScreen = this;
      this.settings = settings;
      List<BlockMenuEntry> blockMenuEntryList1 = new List<BlockMenuEntry>();
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList1.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      int num1 = 0;
      List<BlockMenuEntry> blockMenuEntryList2 = blockMenuEntryList1;
      int index1 = num1;
      int num2 = index1 + 1;
      blockMenuEntryList2[index1].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        settings.ToggleNotification(NotificationType.Visual);
        notificationOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList3 = blockMenuEntryList1;
      int index2 = num2;
      int num3 = index2 + 1;
      blockMenuEntryList3[index2].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        settings.ToggleNotification(NotificationType.Audio);
        notificationOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList4 = blockMenuEntryList1;
      int index3 = num3;
      int num4 = index3 + 1;
      blockMenuEntryList4[index3].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        settings.ToggleNotification(NotificationType.Song);
        notificationOptionsScreen.ResetToggleItems();
      });
      List<BlockMenuEntry> blockMenuEntryList5 = blockMenuEntryList1;
      int index4 = num4;
      int num5 = index4 + 1;
      blockMenuEntryList5[index4].Selected += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        settings.ToggleNotification(NotificationType.TextMsg);
        notificationOptionsScreen.ResetToggleItems();
      });
      blockMenuEntryList1[blockMenuEntryList1.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList1.ToArray());
      this.ResetToggleItems();
    }

    private void ResetToggleItems()
    {
      this.MenuEntries[0].Text = "Visual Notifications: " + (this.settings.HasNotification(NotificationType.Visual) ? "On" : "Off");
      this.MenuEntries[1].Text = "Audio Notifications: " + (this.settings.HasNotification(NotificationType.Audio) ? "On" : "Off");
      this.MenuEntries[2].Text = "Song Notifications: " + (this.settings.HasNotification(NotificationType.Song) ? "On" : "Off");
      this.MenuEntries[3].Text = "Receive Text Messages: " + (this.settings.HasNotification(NotificationType.TextMsg) ? "On" : "Off");
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 574;
      this.Font = CoreGlobals.GameFont;
      this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
