// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.MainOptionsMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens2
{
  internal class MainOptionsMenu : NewGuiMenu2
  {
    private GameSettings gameSettings;
    private SaveMapHead header;
    private Window inputContainer;
    private InputProfile tempProfile;
    private InputItem origItem;
    private Keys lastNonCtrlKey;
    private bool shiftEscaped;
    private bool lastKeyWasValid;

    public override string Name
    {
      get
      {
        return "Options";
      }
    }

    private string SpawnMobText
    {
      get
      {
        if (!this.header.PassiveMobs && !this.header.EnemyMobs)
          return "Off";
        if (this.header.PassiveMobs && !this.header.EnemyMobs)
          return "Passive Only";
        return !this.header.PassiveMobs && this.header.EnemyMobs ? "Enemy Only" : "Passive And Enemy";
      }
    }

    public MainOptionsMenu(GameInstance instance, Player player)
      : base(instance, player)
    {
      this.gameSettings = Globals2.GameSettings;
      this.header = Globals2.GameProperties.SaveGame.Header;
    }

    protected override void InitWindows(Texture2D backTexture)
    {
      base.InitWindows(backTexture);
      this.InitMainContainer();
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
    }

    private bool IsInputWinOpen
    {
      get
      {
        if (this.inputContainer != null)
          return this.inputContainer.IsVisible;
        return false;
      }
    }

    protected override void ResetCanvasTabData()
    {
      if (this.IsInputWinOpen)
        this.canvas.SlidingScroll = true;
      else
        base.ResetCanvasTabData();
    }

    private void InitMainContainer()
    {
      Rectangle winRect = this.canvas.WinRect;
      this.canvas.OffsetMin.X = -300;
      this.canvas.OffsetMin.Y = -150;
      this.canvas.OffsetMax.X = 300;
      this.canvas.OffsetMax.Y = 150;
      int num1 = 120;
      int y1 = 110;
      int width1 = 280;
      int width2 = 220;
      int height1 = 34;
      int num2 = 4;
      int num3 = 7;
      int height2 = height1 * num3 + num2 * (num3 - 1);
      float textScale = 0.6f;
      Window window1 = new Window((string) null, winRect.Width / 2 - 100 - (width1 + 1 + width2), y1, width1 + 1 + width2, height2)
      {
        Name = "mainContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window1);
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int y2;
      int x1 = y2 = 0;
      TextBox textBox1;
      Window window2 = (Window) (textBox1 = new TextBox("Music Volume:", x1, y2, width1, height1, textScale));
      window2.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window2);
      Slider slider1;
      Slider slider2 = slider1 = new Slider(x1 + width1 + 1, y2, width2, height1, textScale);
      textBox1 = (TextBox) slider1;
      Window window3 = (Window) slider1;
      this.initialNavigable = (Window) slider1;
      window3.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider2.SetValue(this.gameSettings.MusicVolume);
      slider2.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderMusicVolume);
      window1.AddChild((StudioForge.Engine.Core.Node) window3);
      int y3 = y2 + (height1 + num2);
      Window window4 = (Window) (textBox1 = new TextBox("Sound Volume:", x1, y3, width1, height1, textScale));
      window4.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window4);
      Slider slider3;
      Slider slider4 = slider3 = new Slider(x1 + width1 + 1, y3, width2, height1, textScale);
      textBox1 = (TextBox) slider3;
      Window window5 = (Window) slider3;
      window5.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider4.SetValue(this.gameSettings.SoundVolume);
      slider4.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderSoundVolume);
      window1.AddChild((StudioForge.Engine.Core.Node) window5);
      int y4 = y3 + (height1 + num2);
      Window window6 = (Window) (textBox1 = new TextBox("Game Difficulty:", x1, y4, width1, height1, textScale));
      window6.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window6);
      DropDown dropDown1;
      DropDown dropDown2 = dropDown1 = new DropDown(this.header.GameDifficulty.ToString(), x1 + width1 + 1, y4, width2, height1, 124, textScale);
      textBox1 = (TextBox) dropDown1;
      Window window7 = (Window) dropDown1;
      window7.Colors = (Window.ColorProfile) Colors.ButtonColors;
      if (window7.IsEnabled = this.header.GameDifficulty != GameDifficulty.Legendary || this.IsGodOrTester)
      {
        dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateGameDifficulty);
        ((ITextInputWindow) dropDown2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateGameDifficulty);
      }
      window1.AddChild((StudioForge.Engine.Core.Node) window7);
      int y5 = y4 + (height1 + num2);
      Window window8 = (Window) (textBox1 = new TextBox("Toggle Combat:", x1, y5, width1, height1, textScale));
      window8.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window8);
      TextBox textBox2;
      Window window9 = (Window) (textBox2 = new TextBox(this.OnOff(this.header.CombatEnabled), x1 + width1 + 1, y5, width2, height1, textScale));
      window9.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window9.IsEnabledFunc = new Func<bool>(this.IsEnabledCombat);
      textBox2.ClickHandler += new Window.WindowHandler(this.ClickToggleCombat);
      window1.AddChild((StudioForge.Engine.Core.Node) window9);
      int y6 = y5 + (height1 + num2);
      Window window10 = (Window) (textBox1 = new TextBox("XP Multiplier:", x1, y6, width1, height1, textScale));
      window10.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window10);
      DataField dataField1;
      DataField dataField2 = dataField1 = new DataField(this.header.XPMultiplier.ToString(), x1 + width1 + 1, y6, width2, height1, textScale);
      textBox1 = (TextBox) dataField1;
      Window window11 = (Window) dataField1;
      window11.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      window11.IsEnabledFunc = new Func<bool>(this.IsEnabledXPMultiplier);
      ((ITextInputWindow) dataField2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateXPMultiplier);
      window1.AddChild((StudioForge.Engine.Core.Node) window11);
      int y7 = y6 + (height1 + num2);
      Window window12 = (Window) (textBox1 = new TextBox("Combat Level Difference:", x1, y7, width1, height1, textScale));
      window12.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window12);
      DataField dataField3;
      DataField dataField4 = dataField3 = new DataField(this.header.CombatLevelDifference.ToString(), x1 + width1 + 1, y7, width2, height1, textScale);
      textBox1 = (TextBox) dataField3;
      Window window13 = (Window) dataField3;
      window13.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      window13.IsEnabledFunc = new Func<bool>(this.IsEnabledCombat);
      ((ITextInputWindow) dataField4).OnValidateInput = new Action<ITextInputWindow>(this.ValidateCombatLevelDiff);
      window1.AddChild((StudioForge.Engine.Core.Node) window13);
      int y8 = y7 + (height1 + num2);
      Window window14 = (Window) (textBox1 = new TextBox("Clan Protection:", x1, y8, width1, height1, textScale));
      window14.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window14);
      TextBox textBox3;
      Window window15 = (Window) (textBox3 = new TextBox(this.OnOff(this.header.ClanProtection), x1 + width1 + 1, y8, width2, height1, textScale));
      window15.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window15.IsEnabledFunc = new Func<bool>(this.IsEnabledCombat);
      textBox3.ClickHandler += new Window.WindowHandler(this.ClickClanProtection);
      window1.AddChild((StudioForge.Engine.Core.Node) window15);
      int num4 = y8 + (height1 + num2);
      num1 = 120;
      int y9 = (int) ((double) window1.Position.Y + (double) window1.Size.Y + (double) height1 + (double) num2 + (double) num2);
      int num5 = 7;
      int height3 = height1 * num5 + num2 * (num5 - 1);
      Window window16 = new Window((string) null, winRect.Width / 2 - 100 - (width1 + 1 + width2), y9, width1 + 1 + width2, height3)
      {
        Name = "creativeContainer"
      };
      window16.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window16);
      int y10;
      int x2 = y10 = 0;
      Window window17 = (Window) (textBox1 = new TextBox("Finite Resources:", x2, y10, width1, height1, textScale));
      window17.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window17);
      TextBox textBox4;
      Window window18 = (Window) (textBox4 = new TextBox(this.OnOff(this.header.FiniteMode), x2 + width1 + 1, y10, width2, height1, textScale));
      window18.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window18.IsEnabled = this.instance.IsCreativeMode || this.IsGodOrTester;
      textBox4.ClickHandler += new Window.WindowHandler(this.ClickFiniteResources);
      window16.AddChild((StudioForge.Engine.Core.Node) window18);
      int y11 = y10 + (height1 + num2);
      Window window19 = (Window) (textBox1 = new TextBox("Skill System:", x2, y11, width1, height1, textScale));
      window19.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window19);
      string text1 = this.instance.IsSkillsEnabled ? "On " + (this.header.SkillsLocal ? "(Local)" : "(Global)") : "Off";
      int x3 = x2 + width1 + 1;
      int y12 = y11;
      int width3 = width2;
      int height4 = height1;
      double num6 = (double) textScale;
      TextBox textBox5;
      Window window20 = (Window) (textBox5 = new TextBox(text1, x3, y12, width3, height4, (float) num6));
      window20.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox5.ClickHandler += new Window.WindowHandler(this.ClickSkillSystem);
      window16.AddChild((StudioForge.Engine.Core.Node) window20);
      int y13 = y11 + (height1 + num2);
      Window window21 = (Window) (textBox1 = new TextBox("Day/Night Cycle:", x2, y13, width1, height1, textScale));
      window21.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window21);
      TextBox textBox6;
      Window window22 = (Window) (textBox6 = new TextBox(this.OnOff(this.header.DayNightActive), x2 + width1 + 1, y13, width2, height1, textScale));
      window22.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window22.IsEnabled = this.instance.IsCreativeMode || this.IsGodOrTester;
      textBox6.ClickHandler += new Window.WindowHandler(this.ClickDayNightCycle);
      window16.AddChild((StudioForge.Engine.Core.Node) window22);
      int y14 = y13 + (height1 + num2);
      Window window23 = (Window) (textBox1 = new TextBox("Weather:", x2, y14, width1, height1, textScale));
      window23.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window23);
      TextBox textBox7;
      Window window24 = (Window) (textBox7 = new TextBox(this.OnOff(this.header.WeatherActive), x2 + width1 + 1, y14, width2, height1, textScale));
      window24.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window24.IsEnabled = this.instance.IsCreativeMode || this.IsGodOrTester;
      textBox7.ClickHandler += new Window.WindowHandler(this.ClickWeather);
      window16.AddChild((StudioForge.Engine.Core.Node) window24);
      int y15 = y14 + (height1 + num2);
      Window window25 = (Window) (textBox1 = new TextBox("Wind Factor:", x2, y15, width1, height1, textScale));
      window25.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window25);
      DataField dataField5;
      DataField dataField6 = dataField5 = new DataField(this.header.WindFactor.ToString(), x2 + width1 + 1, y15, width2, height1, textScale);
      textBox1 = (TextBox) dataField5;
      Window window26 = (Window) dataField5;
      window26.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      ((ITextInputWindow) dataField6).OnValidateInput = new Action<ITextInputWindow>(this.ValidateWindFactor);
      window16.AddChild((StudioForge.Engine.Core.Node) window26);
      int y16 = y15 + (height1 + num2);
      Window window27 = (Window) (textBox1 = new TextBox("Natural Mobs:", x2, y16, width1, height1, textScale));
      window27.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window27);
      DropDown dropDown3;
      DropDown dropDown4 = dropDown3 = new DropDown(this.SpawnMobText, x2 + width1 + 1, y16, width2, height1, 124, textScale);
      textBox1 = (TextBox) dropDown3;
      Window window28 = (Window) dropDown3;
      window28.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      window28.IsEnabled = this.instance.IsCreativeMode || this.IsGodOrTester;
      dropDown4.PopulateList = new Action<Window, List<string>, string>(this.PopulateNaturalMobs);
      ((ITextInputWindow) dropDown4).OnValidateInput = new Action<ITextInputWindow>(this.ValidateNaturalMobs);
      window16.AddChild((StudioForge.Engine.Core.Node) window28);
      int y17 = y16 + (height1 + num2);
      Window window29 = (Window) (textBox1 = new TextBox("Keep Items On Death:", x2, y17, width1, height1, textScale));
      window29.Colors = (Window.ColorProfile) Colors.LabelColors;
      window16.AddChild((StudioForge.Engine.Core.Node) window29);
      TextBox textBox8;
      Window window30 = (Window) (textBox8 = new TextBox(this.OnOff(this.header.KeepItemsOnDeath), x2 + width1 + 1, y17, width2, height1, textScale));
      window30.IsEnabled = this.instance.IsCreativeMode || this.IsGodOrTester;
      window30.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox8.ClickHandler += new Window.WindowHandler(this.ClickKeepItemsOnDeath);
      window16.AddChild((StudioForge.Engine.Core.Node) window30);
      num4 = y17 + (height1 + num2);
      num1 = 120;
      int y18 = (int) ((double) window16.Position.Y + (double) window16.Size.Y + (double) height1 + (double) num2 + (double) num2);
      int num7 = 4;
      int height5 = height1 * num7 + num2 * (num7 - 1);
      Window window31 = new Window((string) null, winRect.Width / 2 - 100 - (width1 + 1 + width2), y18, width1 + 1 + width2, height5)
      {
        Name = "notificationsContainer"
      };
      window31.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window31);
      int y19;
      int x4 = y19 = 0;
      Window window32 = (Window) (textBox1 = new TextBox("Visual Notifications:", x4, y19, width1, height1, textScale));
      window32.Colors = (Window.ColorProfile) Colors.LabelColors;
      window31.AddChild((StudioForge.Engine.Core.Node) window32);
      TextBox textBox9;
      Window window33 = (Window) (textBox9 = new TextBox(this.OnOff(this.gameSettings.HasNotification(NotificationType.Visual)), x4 + width1 + 1, y19, width2, height1, textScale));
      window33.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox9.ClickHandler += new Window.WindowHandler(this.ClickVisualNotifications);
      window31.AddChild((StudioForge.Engine.Core.Node) window33);
      int y20 = y19 + (height1 + num2);
      Window window34 = (Window) (textBox1 = new TextBox("Audio Notifications:", x4, y20, width1, height1, textScale));
      window34.Colors = (Window.ColorProfile) Colors.LabelColors;
      window31.AddChild((StudioForge.Engine.Core.Node) window34);
      TextBox textBox10;
      Window window35 = (Window) (textBox10 = new TextBox(this.OnOff(this.gameSettings.HasNotification(NotificationType.Audio)), x4 + width1 + 1, y20, width2, height1, textScale));
      window35.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox10.ClickHandler += new Window.WindowHandler(this.ClickAudioNotifications);
      window31.AddChild((StudioForge.Engine.Core.Node) window35);
      int y21 = y20 + (height1 + num2);
      Window window36 = (Window) (textBox1 = new TextBox("Song Notifications:", x4, y21, width1, height1, textScale));
      window36.Colors = (Window.ColorProfile) Colors.LabelColors;
      window31.AddChild((StudioForge.Engine.Core.Node) window36);
      TextBox textBox11;
      Window window37 = (Window) (textBox11 = new TextBox(this.OnOff(this.gameSettings.HasNotification(NotificationType.Song)), x4 + width1 + 1, y21, width2, height1, textScale));
      window37.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox11.ClickHandler += new Window.WindowHandler(this.ClickSongNotifications);
      window31.AddChild((StudioForge.Engine.Core.Node) window37);
      int y22 = y21 + (height1 + num2);
      Window window38 = (Window) (textBox1 = new TextBox("Receive Text Messages:", x4, y22, width1, height1, textScale));
      window38.Colors = (Window.ColorProfile) Colors.LabelColors;
      window31.AddChild((StudioForge.Engine.Core.Node) window38);
      TextBox textBox12;
      Window window39 = (Window) (textBox12 = new TextBox(this.OnOff(this.gameSettings.HasNotification(NotificationType.TextMsg)), x4 + width1 + 1, y22, width2, height1, textScale));
      window39.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox12.ClickHandler += new Window.WindowHandler(this.ClickReceiveTextMessages);
      window31.AddChild((StudioForge.Engine.Core.Node) window39);
      num4 = y22 + (height1 + num2);
      int num8 = 3;
      int y23 = 110;
      int width4 = 280;
      int height6 = height1 * num8 + num2 * (num8 - 1);
      Window window40 = new Window((string) null, winRect.Width / 2 + 100, y23, width4, height6)
      {
        Name = "optionsContainer"
      };
      window40.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window40);
      int y24;
      int x5 = y24 = 0;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window41 = (Window) (textBox1 = new TextBox("Input Profiles", x5, y24, width4, height1, textScale));
      window41.Colors = (Window.ColorProfile) Colors.ButtonAltColors;
      window41.ClickHandler += new Window.WindowHandler(this.ClickInputBindings);
      window40.AddChild((StudioForge.Engine.Core.Node) window41);
      int y25 = y24 + (height1 + num2);
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window42 = (Window) (textBox1 = new TextBox("Reset Music Shuffle", x5, y25, width4, height1, textScale));
      window42.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window42.ClickHandler += new Window.WindowHandler(this.ClickResetMusicShuffle);
      window40.AddChild((StudioForge.Engine.Core.Node) window42);
      int y26 = y25 + (height1 + num2);
      Window window43 = (Window) (textBox1 = new TextBox("Item Options", x5, y26, width4, height1, textScale));
      window43.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window43.IsEnabled = this.player.IsAdmin;
      window43.ClickHandler += new Window.WindowHandler(this.ClickItemOptions);
      window40.AddChild((StudioForge.Engine.Core.Node) window43);
      num4 = y26 + (height1 + num2);
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      int width5 = 280;
      int width6 = 220;
      int y27 = (int) ((double) window40.Position.Y + (double) window40.Size.Y + (double) height1 + (double) num2 + (double) num2);
      int num9 = 13;
      int height7 = height1 * num9 + num2 * (num9 - 1);
      Window window44 = new Window((string) null, winRect.Width / 2 + 100, y27, width5 + 1 + width6, height7)
      {
        Name = "playerContainer"
      };
      window44.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window44);
      int y28;
      int x6 = y28 = 0;
      Window window45 = (Window) (textBox1 = new TextBox("Default Menu:", x6, y28, width5, height1, textScale));
      window45.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window45);
      string text2 = Globals2.UseOldMenu ? "Old" : "New";
      int x7 = x6 + width5 + 1;
      int y29 = y28;
      int width7 = width6;
      int height8 = height1;
      double num10 = (double) textScale;
      TextBox textBox13;
      Window window46 = (Window) (textBox13 = new TextBox(text2, x7, y29, width7, height8, (float) num10));
      window46.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox13.ClickHandler += new Window.WindowHandler(this.ClickMenu);
      window44.AddChild((StudioForge.Engine.Core.Node) window46);
      int y30 = y28 + (height1 + num2);
      Window window47 = (Window) (textBox1 = new TextBox("HUD:", x6, y30, width5, height1, textScale));
      window47.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window47);
      TextBox textBox14;
      Window window48 = (Window) (textBox14 = new TextBox(this.OnOff(this.player.Settings.HudVisible), x6 + width5 + 1, y30, width6, height1, textScale));
      window48.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox14.ClickHandler += new Window.WindowHandler(this.ClickHUD);
      window44.AddChild((StudioForge.Engine.Core.Node) window48);
      int y31 = y30 + (height1 + num2);
      Window window49 = (Window) (textBox1 = new TextBox("Mini Map:", x6, y31, width5, height1, textScale));
      window49.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window49);
      TextBox textBox15;
      Window window50 = (Window) (textBox15 = new TextBox(this.OnOff(this.player.Settings.MapVisible), x6 + width5 + 1, y31, width6, height1, textScale));
      window50.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox15.ClickHandler += new Window.WindowHandler(this.ClickMiniMap);
      window50.IsEnabled = this.player.IsGod || !this.instance.IsLegendaryDifficulty && this.player.HasPermission(Permissions.Map);
      window44.AddChild((StudioForge.Engine.Core.Node) window50);
      int y32 = y31 + (height1 + num2);
      Window window51 = (Window) (textBox1 = new TextBox("Blueprint Finder:", x6, y32, width5, height1, textScale));
      window51.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window51);
      TextBox textBox16;
      Window window52 = (Window) (textBox16 = new TextBox(this.OnOff(this.player.Settings.BlueprintFinderVisible), x6 + width5 + 1, y32, width6, height1, textScale));
      window52.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window52.IsEnabled = this.player.IsGod || this.instance.IsDigDeepMode && !this.instance.IsLegendaryDifficulty;
      textBox16.ClickHandler += new Window.WindowHandler(this.ClickBlueprintFinder);
      window44.AddChild((StudioForge.Engine.Core.Node) window52);
      int y33 = y32 + (height1 + num2);
      Window window53 = (Window) (textBox1 = new TextBox("Nameplates:", x6, y33, width5, height1, textScale));
      window53.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window53);
      TextBox textBox17;
      Window window54 = (Window) (textBox17 = new TextBox(this.player.GetNameplateSettingText(), x6 + width5 + 1, y33, width6, height1, textScale));
      window54.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox17.ClickHandler += new Window.WindowHandler(this.ClickNameplates);
      window44.AddChild((StudioForge.Engine.Core.Node) window54);
      int y34 = y33 + (height1 + num2);
      Window window55 = (Window) (textBox1 = new TextBox("Mob Nameplates:", x6, y34, width5, height1, textScale));
      window55.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window55);
      TextBox textBox18;
      Window window56 = (Window) (textBox18 = new TextBox(this.player.GetMobNameplateSettingText(), x6 + width5 + 1, y34, width6, height1, textScale));
      window56.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox18.ClickHandler += new Window.WindowHandler(this.ClickMobNameplates);
      window44.AddChild((StudioForge.Engine.Core.Node) window56);
      int y35 = y34 + (height1 + num2);
      Window window57 = (Window) (textBox1 = new TextBox("Display XP Gains:", x6, y35, width5, height1, textScale));
      window57.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window57);
      TextBox textBox19;
      Window window58 = (Window) (textBox19 = new TextBox(this.OnOff(this.player.Settings.DisplayXPGains), x6 + width5 + 1, y35, width6, height1, textScale));
      window58.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox19.ClickHandler += new Window.WindowHandler(this.ClickDisplayXPGains);
      window44.AddChild((StudioForge.Engine.Core.Node) window58);
      int y36 = y35 + (height1 + num2);
      Window window59 = (Window) (textBox1 = new TextBox("Hotbar Transparency:", x6, y36, width5, height1, textScale));
      window59.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window59);
      TextBox textBox20;
      Window window60 = (Window) (textBox20 = new TextBox(this.player.Settings.GetHotbarTransparencyText(), x6 + width5 + 1, y36, width6, height1, textScale));
      window60.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox20.ClickHandler += new Window.WindowHandler(this.ClickHotbarTransparency);
      window44.AddChild((StudioForge.Engine.Core.Node) window60);
      int y37 = y36 + (height1 + num2);
      Window window61 = (Window) (textBox1 = new TextBox("Compass:", x6, y37, width5, height1, textScale));
      window61.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window61);
      string text3 = this.player.Settings.CompassTop ? "Top" : "Bottom";
      int x8 = x6 + width5 + 1;
      int y38 = y37;
      int width8 = width6;
      int height9 = height1;
      double num11 = (double) textScale;
      TextBox textBox21;
      Window window62 = (Window) (textBox21 = new TextBox(text3, x8, y38, width8, height9, (float) num11));
      window62.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox21.ClickHandler += new Window.WindowHandler(this.ClickCompass);
      window44.AddChild((StudioForge.Engine.Core.Node) window62);
      int y39 = y37 + (height1 + num2);
      Window window63 = (Window) (textBox1 = new TextBox("Autoplace Speed:", x6, y39, width5, height1, textScale));
      window63.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window63);
      TextBox textBox22;
      Window window64 = (Window) (textBox22 = new TextBox(this.player.Settings.GetAutoPlaceSettingText(), x6 + width5 + 1, y39, width6, height1, textScale));
      window64.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox22.ClickHandler += new Window.WindowHandler(this.ClickAutoPlaceSpeed);
      window44.AddChild((StudioForge.Engine.Core.Node) window64);
      int y40 = y39 + (height1 + num2);
      Window window65 = (Window) (textBox1 = new TextBox("Bobbing:", x6, y40, width5, height1, textScale));
      window65.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window65);
      TextBox textBox23;
      Window window66 = (Window) (textBox23 = new TextBox(this.OnOff(this.player.Settings.Bobbing), x6 + width5 + 1, y40, width6, height1, textScale));
      window66.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox23.ClickHandler += new Window.WindowHandler(this.ClickBobbing);
      window44.AddChild((StudioForge.Engine.Core.Node) window66);
      int y41 = y40 + (height1 + num2);
      Window window67 = (Window) (textBox1 = new TextBox("Wield:", x6, y41, width5, height1, textScale));
      window67.Colors = (Window.ColorProfile) Colors.LabelColors;
      window44.AddChild((StudioForge.Engine.Core.Node) window67);
      TextBox textBox24;
      Window window68 = (Window) (textBox24 = new TextBox(this.player.Settings.WieldType.ToString(), x6 + width5 + 1, y41, width6, height1, textScale));
      window68.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox24.ClickHandler += new Window.WindowHandler(this.ClickWield);
      window44.AddChild((StudioForge.Engine.Core.Node) window68);
      num4 = y41 + (height1 + num2);
    }

    private void LoadInputProfile(InputProfile profile, bool clone)
    {
      if (profile == null)
        profile = Globals2.GetInputProfile("");
      if (this.inputContainer != null)
      {
        this.inputContainer.RemoveSelf();
        this.inputContainer = (Window) null;
      }
      this.tempProfile = !clone || !profile.Name.IsNotEmpty() ? profile : profile.Clone(this.player.Gamertag);
      this.InitInputContainer();
    }

    private void InitInputContainer()
    {
      int x1 = 120;
      int y1 = 550;
      int width1 = 320;
      int width2 = 240;
      int height1 = 34;
      int height2 = 28;
      int num1 = 2;
      int num2 = this.tempProfile.InputScheme.Count + 11;
      int height3 = height1 * 11 + height2 * this.tempProfile.InputScheme.Count + num1 * (num2 - 1);
      float textScale1 = 0.6f;
      float textScale2 = 0.55f;
      bool flag = this.tempProfile.Account == this.player.Gamertag || this.IsGodOrTester;
      Window window1 = this.inputContainer = new Window((string) null, x1, y1, width1 + (1 + width2) * 3, height3)
      {
        Name = "inputContainer"
      };
      window1.Colors = Window.TransparentColorProfile;
      this.canvas.AddChild((StudioForge.Engine.Core.Node) window1);
      int y2;
      int x2 = y2 = 0;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      TextBox textBox1;
      if (this.tempProfile.Name.IsEmpty())
      {
        Window window2 = (Window) (textBox1 = new TextBox("This is the default input profile and it's input bindings cannot be changed.", x2, y2 - (height1 + num1) * 2 - num1, width1 + width2 * 3 + 3, height1 - 2, textScale1));
        window2.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window2);
        Window window3 = (Window) (textBox1 = new TextBox("To change input bindings, create a new profile by entering a unique name in the field below.", x2, y2 - (height1 + num1 + num1 + num1 + 2), width1 + width2 * 3 + 3, height1 - 2, textScale1));
        window3.Colors = (Window.ColorProfile) Colors.LabelLowAlphaColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window3);
      }
      Window window4 = (Window) (textBox1 = new TextBox("Profile:", x2, y2, width1, height1, textScale1));
      window4.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window4);
      Window window5 = (Window) (textBox1 = new TextBox("Profile:", x2, y2, width1, height1, textScale1));
      window5.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window5);
      DropDown dropDown1;
      DropDown dropDown2 = dropDown1 = new DropDown(this.tempProfile.Name, x2 + width1 + 1, y2, width2 * 3 + 2, height1, 400, textScale1);
      textBox1 = (TextBox) dropDown1;
      Window window6 = (Window) dropDown1;
      window6.Colors = (Window.ColorProfile) Colors.DataFieldColors;
      window6.AddFlags(Window.WinFlags.KeepItemsSorted);
      dropDown2.GetNewInputHandler = (GetTextInputHander) null;
      dropDown2.PopulateList = new Action<Window, List<string>, string>(this.PopulateInputProfiles);
      ((ITextInputWindow) dropDown2).OnValidateInput = new Action<ITextInputWindow>(this.ValidateInputProfile);
      window1.AddChild((StudioForge.Engine.Core.Node) window6);
      int y3 = y2 + (height1 + num1) + (height1 + num1);
      int x3 = 0;
      int num3 = y3;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      Window window7 = (Window) (textBox1 = new TextBox("Mouse Smoothing:", x3, y3, width1, height1, textScale1));
      window7.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window7);
      Slider slider1;
      Slider slider2 = slider1 = new Slider(x3 + width1 + 1, y3, width2, height1, textScale1);
      textBox1 = (TextBox) slider1;
      Window window8 = (Window) slider1;
      window8.IsEnabled = flag;
      slider2.SetValue((float) this.tempProfile.MouseLookAtSmoothing / 10f);
      slider2.Text = this.tempProfile.MouseLookAtSmoothing.ToString();
      window8.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider2.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderMouseSmoothing);
      window1.AddChild((StudioForge.Engine.Core.Node) window8);
      int y4 = y3 + (height1 + num1);
      Window window9 = (Window) (textBox1 = new TextBox("Mouse Sensitivity:", x3, y4, width1, height1, textScale1));
      window9.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window9);
      Slider slider3;
      Slider slider4 = slider3 = new Slider(x3 + width1 + 1, y4, width2, height1, textScale1);
      textBox1 = (TextBox) slider3;
      Window window10 = (Window) slider3;
      window10.IsEnabled = flag;
      slider4.SetValue(this.tempProfile.MouseSensitivity);
      window10.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider4.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderMouseSensitivity);
      window1.AddChild((StudioForge.Engine.Core.Node) window10);
      int y5 = y4 + (height1 + num1);
      Window window11 = (Window) (textBox1 = new TextBox("Gamepad Sensitivity:", x3, y5, width1, height1, textScale1));
      window11.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window11);
      Slider slider5;
      Slider slider6 = slider5 = new Slider(x3 + width1 + 1, y5, width2, height1, textScale1);
      textBox1 = (TextBox) slider5;
      Window window12 = (Window) slider5;
      window12.IsEnabled = flag;
      slider6.SetValue(this.tempProfile.GamePadSensitivity);
      window12.Colors = (Window.ColorProfile) Colors.ButtonColors;
      slider6.DragSliderHandler += new Window.WindowDragHandler(this.DragSliderGamepadSensitivity);
      window1.AddChild((StudioForge.Engine.Core.Node) window12);
      int y6 = y5 + (height1 + num1);
      Window window13 = (Window) (textBox1 = new TextBox("Gamepad Invert Y:", x3, y6, width1, height1, textScale1));
      window13.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window13);
      TextBox textBox2;
      Window window14 = (Window) (textBox2 = new TextBox(this.OnOff(this.tempProfile.GamePadInvertY), x3 + width1 + 1, y6, width2, height1, textScale1));
      window14.IsEnabled = flag;
      window14.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox2.ClickHandler += new Window.WindowHandler(this.ClickGamePadInvertY);
      window1.AddChild((StudioForge.Engine.Core.Node) window14);
      int y7 = y6 + (height1 + num1);
      Window window15 = (Window) (textBox1 = new TextBox("Gamepad Rumble:", x3, y7, width1, height1, textScale1));
      window15.Colors = (Window.ColorProfile) Colors.LabelColors;
      window1.AddChild((StudioForge.Engine.Core.Node) window15);
      TextBox textBox3;
      Window window16 = (Window) (textBox3 = new TextBox(this.OnOff(this.tempProfile.GamePadRumble), x3 + width1 + 1, y7, width2, height1, textScale1));
      window16.IsEnabled = flag;
      window16.Colors = (Window.ColorProfile) Colors.ButtonColors;
      textBox3.ClickHandler += new Window.WindowHandler(this.ClickGamePadRumble);
      window1.AddChild((StudioForge.Engine.Core.Node) window16);
      int y8 = num3;
      int x4 = width1 + 1 + width2 * 2 + 2;
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window17 = (Window) (textBox1 = new TextBox("Apply and Save", x4, y8, width2, height1, textScale1));
      window17.IsEnabled = flag;
      window17.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window17.ClickHandler += new Window.WindowHandler(this.ClickInputBindingsApplyAndSave);
      window1.AddChild((StudioForge.Engine.Core.Node) window17);
      int y9 = y8 + (height1 + num1);
      Window window18 = (Window) (textBox1 = new TextBox("Restore Defaults", x4, y9, width2, height1, textScale1));
      window18.IsEnabled = flag;
      window18.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window18.ClickHandler += new Window.WindowHandler(this.ClickInputBindingsRestoreDefaults);
      window1.AddChild((StudioForge.Engine.Core.Node) window18);
      int y10 = y9 + (height1 + num1);
      Window window19 = (Window) (textBox1 = new TextBox("Delete Profile", x4, y10, width2, height1, textScale1));
      window19.IsEnabled = flag;
      window19.Colors = (Window.ColorProfile) Colors.ButtonWarnColors;
      window19.ClickHandler += new Window.WindowHandler(this.ClickInputBindingsDeleteProfile);
      window1.AddChild((StudioForge.Engine.Core.Node) window19);
      int y11 = y10 + (height1 + num1);
      Window window20 = (Window) (textBox1 = new TextBox("Cancel", x4, y11, width2, height1, textScale1));
      window20.Colors = (Window.ColorProfile) Colors.ButtonColors;
      window20.ClickHandler += new Window.WindowHandler(this.ClickInputBindingsCancel);
      window1.AddChild((StudioForge.Engine.Core.Node) window20);
      int y12 = y11 + (height1 + num1) + (height1 + num1) + (height1 + num1);
      int x5 = 0;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      Window window21 = (Window) (textBox1 = new TextBox("Gameplay Inputs", x5, y12, width1, height1, textScale1));
      window21.Colors = (Window.ColorProfile) Colors.Heading1;
      window1.AddChild((StudioForge.Engine.Core.Node) window21);
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      Window window22 = (Window) (textBox1 = new TextBox("Keyboard", x5 + width1 + 1, y12, width2, height1, textScale1));
      window22.Colors = (Window.ColorProfile) Colors.Heading1;
      window1.AddChild((StudioForge.Engine.Core.Node) window22);
      Window window23 = (Window) (textBox1 = new TextBox("Mouse", x5 + width1 + 1 + width2 + 1, y12, width2, height1, textScale1));
      window23.Colors = (Window.ColorProfile) Colors.Heading1;
      window1.AddChild((StudioForge.Engine.Core.Node) window23);
      Window window24 = (Window) (textBox1 = new TextBox("Gamepad", x5 + width1 + 1 + width2 + 1 + width2 + 1, y12, width2, height1, textScale1));
      window24.Colors = (Window.ColorProfile) Colors.Heading1;
      window1.AddChild((StudioForge.Engine.Core.Node) window24);
      int y13 = y12 + (height1 + num1);
      foreach (KeyValuePair<ushort, InputItem> keyValuePair in this.tempProfile.InputScheme)
      {
        if (keyValuePair.Key == (ushort) 200)
        {
          int y14 = y13 + (height1 + num1);
          TextBox.DefaultTextAlignX = WinTextAlignX.Left;
          Window window2 = (Window) (textBox1 = new TextBox("User Interface Inputs", x5, y14, width1, height1, textScale1));
          window2.Colors = (Window.ColorProfile) Colors.Heading2;
          window1.AddChild((StudioForge.Engine.Core.Node) window2);
          TextBox.DefaultTextAlignX = WinTextAlignX.Center;
          Window window3 = (Window) (textBox1 = new TextBox("Keyboard", x5 + width1 + 1, y14, width2, height1, textScale1));
          window3.Colors = (Window.ColorProfile) Colors.Heading2;
          window1.AddChild((StudioForge.Engine.Core.Node) window3);
          Window window25 = (Window) (textBox1 = new TextBox("Mouse", x5 + width1 + 1 + width2 + 1, y14, width2, height1, textScale1));
          window25.Colors = (Window.ColorProfile) Colors.Heading2;
          window1.AddChild((StudioForge.Engine.Core.Node) window25);
          Window window26 = (Window) (textBox1 = new TextBox("Gamepad", x5 + width1 + 1 + width2 + 1 + width2 + 1, y14, width2, height1, textScale1));
          window26.Colors = (Window.ColorProfile) Colors.Heading2;
          window1.AddChild((StudioForge.Engine.Core.Node) window26);
          y13 = y14 + (height1 + num1);
        }
        string str = keyValuePair.Key < (ushort) 200 ? ((PlayerInput) keyValuePair.Key).ToString() : ((GuiInput) keyValuePair.Key).ToString();
        TextBox.DefaultTextAlignX = WinTextAlignX.Left;
        Window window27 = (Window) (textBox1 = new TextBox(str.ToString(), x5, y13, width1, height2, textScale2));
        window27.Colors = (Window.ColorProfile) Colors.ButtonConstColors;
        window1.AddChild((StudioForge.Engine.Core.Node) window27);
        TextBox.DefaultTextAlignX = WinTextAlignX.Center;
        DataField dataField1;
        DataField dataField2 = dataField1 = new DataField(this.GetKeyText(keyValuePair.Key, keyValuePair.Value), x5 + width1 + 1, y13, width2, height2, textScale2);
        TextBox textBox4 = (TextBox) dataField1;
        Window window28 = (Window) dataField1;
        textBox4.MinTextEdge = (ushort) 12;
        if (window28.IsEnabled = flag && keyValuePair.Value.EnabledKey)
        {
          window28.Colors = (Window.ColorProfile) Colors.DataFieldColors;
          window28.Tag = (object) keyValuePair.Key;
          ((ITextInputWindow) dataField2).OnBeginInput = new Action<ITextInputWindow>(this.InputBindingBeginKeyInput);
          ((ITextInputWindow) dataField2).OnRawInput = new RawInputFunc(this.InputBindingRawInputKey);
        }
        window1.AddChild((StudioForge.Engine.Core.Node) window28);
        DataField dataField3;
        DataField dataField4 = dataField3 = new DataField(this.GetMouseButtonText(keyValuePair.Key, keyValuePair.Value), x5 + width1 + 1 + width2 + 1, y13, width2, height2, textScale2);
        TextBox textBox5 = (TextBox) dataField3;
        Window window29 = (Window) dataField3;
        textBox5.MinTextEdge = (ushort) 12;
        if (window29.IsEnabled = flag && keyValuePair.Value.EnabledMouseButton)
        {
          window29.AddFlags(Window.WinFlags.OwnsMouseWheel);
          window29.Colors = (Window.ColorProfile) Colors.DataFieldColors;
          window29.Tag = (object) keyValuePair.Key;
          ((ITextInputWindow) dataField4).OnBeginInput = new Action<ITextInputWindow>(this.InputBindingBeginInput);
          ((ITextInputWindow) dataField4).OnRawInput = new RawInputFunc(this.InputBindingRawInputMouse);
        }
        window1.AddChild((StudioForge.Engine.Core.Node) window29);
        DataField dataField5;
        DataField dataField6 = dataField5 = new DataField(this.GetGamepadText(keyValuePair.Key, keyValuePair.Value), x5 + width1 + 1 + width2 + 1 + width2 + 1, y13, width2, height2, textScale2);
        TextBox textBox6 = (TextBox) dataField5;
        Window window30 = (Window) dataField5;
        textBox6.MinTextEdge = (ushort) 12;
        if (window30.IsEnabled = flag && keyValuePair.Value.EnabledButton)
        {
          window30.Colors = (Window.ColorProfile) Colors.DataFieldColors;
          window30.Tag = (object) keyValuePair.Key;
          ((ITextInputWindow) dataField6).OnBeginInput = new Action<ITextInputWindow>(this.InputBindingBeginInput);
          ((ITextInputWindow) dataField6).OnRawInput = new RawInputFunc(this.InputBindingRawInputGamepad);
        }
        window1.AddChild((StudioForge.Engine.Core.Node) window30);
        y13 += height2 + num1;
      }
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
    }

    private bool UseSpecialKey(int id)
    {
      if (id != 85 && id != 140 && (id != 120 && id != 121) && (id != 122 && id != 123 && id != 130))
        return id == 131;
      return true;
    }

    private string GetKeyText(ushort id, InputItem item)
    {
      string str = item.Key.ToString();
      if (item.Key != Keys.None)
      {
        if (str.StartsWith("Oem", StringComparison.OrdinalIgnoreCase) && str.Length > 3)
          str = str.Substring(3);
        if (item.Key >= Keys.D0 && item.Key <= Keys.D9 && str.Length > 1)
          str = str.Substring(1);
        if (item.KeyShift)
          str = "Shift + " + str;
        if (item.KeyCtrl)
          str = "Ctrl + " + str;
        if (item.KeyAlt)
          str = "Alt + " + str;
        if (this.UseSpecialKey((int) id))
          str = "Special + " + str;
      }
      return str;
    }

    private string GetMouseButtonText(ushort id, InputItem item)
    {
      string str = item.MouseButton.ToString();
      if (item.MouseButton != StudioForge.Engine.Integration.MouseButtons.None)
      {
        if (item.MouseShift)
          str = "Shift + " + str;
        if (item.MouseCtrl)
          str = "Ctrl + " + str;
        if (item.MouseAlt)
          str = "Alt + " + str;
        if (this.UseSpecialKey((int) id))
          str = "Special + " + str;
      }
      return str;
    }

    private string GetGamepadText(ushort id, InputItem item)
    {
      string str = item.Button.ToString();
      if (item.Button > (Buttons) 0)
      {
        if (this.UseSpecialKey((int) id))
          str = "Special + " + str;
      }
      else
        str = "None";
      return str;
    }

    private bool IsEnabledXPMultiplier()
    {
      return this.instance.IsLocalSkillsEnabled;
    }

    private bool IsEnabledCombat()
    {
      if (this.instance.IsPeacefulDifficulty || !this.instance.IsCreativeMode)
        return this.IsGodOrTester;
      return true;
    }

    private void DragSliderMusicVolume(object sender, WindowDragEventArgs e)
    {
      int num = (int) ((double) (float) e.Tag * 100.0);
      CoreGlobals.AudioManager.MusicVolume = this.gameSettings.MusicVolume = (float) num / 100f;
      ((TextBox) sender).Text = num.ToString() + "%";
    }

    private void DragSliderSoundVolume(object sender, WindowDragEventArgs e)
    {
      int num = (int) ((double) (float) e.Tag * 100.0);
      CoreGlobals.AudioManager.SoundVolume = this.gameSettings.SoundVolume = (float) num / 100f;
      TotalMinerGame.Instance.AudioManagerFiles.SoundVolume = CoreGlobals.AudioManager.SoundVolume;
      ((TextBox) sender).Text = num.ToString() + "%";
    }

    private void DragSliderMouseSensitivity(object sender, WindowDragEventArgs e)
    {
      int num = (int) ((double) (float) e.Tag * 100.0);
      this.tempProfile.MouseSensitivity = (float) num / 100f;
      ((TextBox) sender).Text = num.ToString() + "%";
    }

    private void DragSliderGamepadSensitivity(object sender, WindowDragEventArgs e)
    {
      int num = (int) ((double) (float) e.Tag * 100.0);
      this.tempProfile.GamePadSensitivity = (float) num / 100f;
      ((TextBox) sender).Text = num.ToString() + "%";
    }

    private void PopulateGameDifficulty(Window win, List<string> list, string input)
    {
      list.Clear();
      if (this.header.GameMode != GameMode.Survival)
        list.Add(GameDifficulty.Peaceful.ToString());
      list.Add(GameDifficulty.Easy.ToString());
      list.Add(GameDifficulty.Normal.ToString());
    }

    private void ValidateGameDifficulty(ITextInputWindow win)
    {
      GameDifficulty? nullable1 = Utils.GetEnumFromString<GameDifficulty>(win.Text);
      if (!nullable1.HasValue)
      {
        nullable1 = new GameDifficulty?(this.header.GameDifficulty);
      }
      else
      {
        GameDifficulty? nullable2 = nullable1;
        if ((nullable2.GetValueOrDefault() != GameDifficulty.Peaceful ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
        {
          string canEnablePeaceful = this.instance.CanEnablePeaceful;
          if (canEnablePeaceful != null)
          {
            this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot enable Peaceful mode because " + canEnablePeaceful, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
            return;
          }
        }
        this.header.GameDifficulty = nullable1.Value;
        this.header.CombatEnabled = this.header.GameDifficulty != GameDifficulty.Peaceful;
        ((TextBox) ((StudioForge.Engine.Core.Node) win).NextSibling.NextSibling).Text = this.OnOff(this.header.CombatEnabled);
      }
      win.Text = this.header.GameDifficulty.ToString();
    }

    private void ClickToggleCombat(object sender, WindowEventArgs e)
    {
      if (this.instance.IsCombatEnabled)
      {
        string canDisableCombat = this.instance.CanDisableCombat;
        if (canDisableCombat != null)
        {
          this.screenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You cannot disable Combat because " + canDisableCombat, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), new PlayerIndex?(this.playerIndex));
          return;
        }
      }
      this.header.CombatEnabled = !this.header.CombatEnabled;
      ((TextBox) e.Window).Text = this.OnOff(this.header.CombatEnabled);
    }

    private void ValidateXPMultiplier(ITextInputWindow win)
    {
      float result;
      if (float.TryParse(win.Text, out result) && (double) result > 0.0)
        this.header.XPMultiplier = MathHelper.Clamp((float) Math.Round((double) result, 2), 0.01f, 100f);
      win.Text = this.header.XPMultiplier.ToString();
    }

    private void ValidateCombatLevelDiff(ITextInputWindow win)
    {
      int result;
      if (int.TryParse(win.Text, out result) && result > 0)
        this.header.CombatLevelDifference = (short) MyMathHelper.Clamp(result, 0, 100);
      win.Text = this.header.CombatLevelDifference.ToString();
    }

    private void ClickClanProtection(object sender, WindowEventArgs e)
    {
      this.header.ClanProtection = !this.header.ClanProtection;
      ((TextBox) e.Window).Text = this.OnOff(this.header.ClanProtection);
    }

    private void ClickFiniteResources(object sender, WindowEventArgs e)
    {
      bool isSkillsEnabled = this.instance.IsSkillsEnabled;
      this.header.FiniteMode = !this.header.FiniteMode;
      ((TextBox) e.Window).Text = this.OnOff(this.header.FiniteMode);
      ((TextBox) e.Window.NextSibling.NextSibling).Text = this.instance.IsSkillsEnabled ? "On " + (this.header.SkillsLocal ? "(Local)" : "(Global)") : "Off";
      this.instance.SkillSystemChanged(isSkillsEnabled);
    }

    private void ClickSkillSystem(object sender, WindowEventArgs e)
    {
      if (!this.header.FiniteMode)
        return;
      this.instance.ToggleSkillSystem();
      ((TextBox) e.Window).Text = this.instance.IsSkillsEnabled ? "On " + (this.header.SkillsLocal ? "(Local)" : "(Global)") : "Off";
    }

    private void ClickDayNightCycle(object sender, WindowEventArgs e)
    {
      this.header.DayNightActive = !this.header.DayNightActive;
      ((TextBox) e.Window).Text = this.OnOff(this.header.DayNightActive);
    }

    private void ClickWeather(object sender, WindowEventArgs e)
    {
      this.header.WeatherActive = !this.header.WeatherActive;
      if (!this.header.WeatherActive)
        this.instance.MapStrategyTM.RemoveAllWeather();
      ((TextBox) e.Window).Text = this.OnOff(this.header.WeatherActive);
    }

    private void ValidateWindFactor(ITextInputWindow win)
    {
      float result;
      if (float.TryParse(win.Text, out result) && (double) result > 0.0)
        this.instance.Wind.WindFactor = MathHelper.Clamp(result, 0.0f, 3f);
      win.Text = this.instance.Wind.WindFactor.ToString();
    }

    private void PopulateNaturalMobs(Window win, List<string> list, string input)
    {
      list.Clear();
      list.Add("Off");
      list.Add("Passive Only");
      list.Add("Enemy Only");
      list.Add("Passive and Enemy");
    }

    private void ValidateNaturalMobs(ITextInputWindow win)
    {
      switch (win.Text)
      {
        case "Passive Only":
          this.header.PassiveMobs = true;
          this.header.EnemyMobs = false;
          break;
        case "Enemy Only":
          this.header.PassiveMobs = false;
          this.header.EnemyMobs = true;
          break;
        case "Passive and Enemy":
          this.header.PassiveMobs = this.header.EnemyMobs = true;
          break;
        default:
          this.header.PassiveMobs = this.header.EnemyMobs = false;
          break;
      }
    }

    private void ClickKeepItemsOnDeath(object sender, WindowEventArgs e)
    {
      this.header.KeepItemsOnDeath = !this.header.KeepItemsOnDeath;
      ((TextBox) e.Window).Text = this.OnOff(this.header.KeepItemsOnDeath);
    }

    private void ClickGamePadInvertY(object sender, WindowEventArgs e)
    {
      this.tempProfile.GamePadInvertY = !this.tempProfile.GamePadInvertY;
      ((TextBox) e.Window).Text = this.OnOff(this.tempProfile.GamePadInvertY);
    }

    private void ClickGamePadRumble(object sender, WindowEventArgs e)
    {
      this.tempProfile.GamePadRumble = !this.tempProfile.GamePadRumble;
      ((TextBox) e.Window).Text = this.OnOff(this.tempProfile.GamePadRumble);
    }

    private void ClickMenu(object sender, WindowEventArgs e)
    {
      Globals2.UseOldMenu = !Globals2.UseOldMenu;
      ((TextBox) e.Window).Text = Globals2.UseOldMenu ? "Old" : "New";
      TextFileParser.WriteBool("game.ini", "OldMenu", Globals2.UseOldMenu);
    }

    private void ClickHUD(object sender, WindowEventArgs e)
    {
      this.player.Settings.HudVisible = !this.player.Settings.HudVisible;
      ((TextBox) e.Window).Text = this.OnOff(this.player.Settings.HudVisible);
    }

    private void ClickMiniMap(object sender, WindowEventArgs e)
    {
      this.player.Settings.MapVisible = !this.player.Settings.MapVisible;
      ((TextBox) e.Window).Text = this.OnOff(this.player.Settings.MapVisible);
    }

    private void ClickBlueprintFinder(object sender, WindowEventArgs e)
    {
      this.player.Settings.BlueprintFinderVisible = !this.player.Settings.BlueprintFinderVisible;
      ((TextBox) e.Window).Text = this.OnOff(this.player.Settings.BlueprintFinderVisible);
    }

    private void ClickNameplates(object sender, WindowEventArgs e)
    {
      this.player.ToggleNameplateSetting();
      ((TextBox) e.Window).Text = this.player.GetNameplateSettingText();
    }

    private void ClickMobNameplates(object sender, WindowEventArgs e)
    {
      this.player.ToggleMobNameplateSetting();
      ((TextBox) e.Window).Text = this.player.GetMobNameplateSettingText();
    }

    private void ClickDisplayXPGains(object sender, WindowEventArgs e)
    {
      this.player.Settings.DisplayXPGains = !this.player.Settings.DisplayXPGains;
      ((TextBox) e.Window).Text = this.OnOff(this.player.Settings.DisplayXPGains);
    }

    private void ClickHotbarTransparency(object sender, WindowEventArgs e)
    {
      this.player.Settings.ToggleHotbarTransparency();
      ((TextBox) e.Window).Text = this.player.Settings.GetHotbarTransparencyText();
    }

    private void ClickCompass(object sender, WindowEventArgs e)
    {
      this.player.Settings.CompassTop = !this.player.Settings.CompassTop;
      ((TextBox) e.Window).Text = this.player.Settings.CompassTop ? "Top" : "Bottom";
    }

    private void ClickBobbing(object sender, WindowEventArgs e)
    {
      this.player.Settings.Bobbing = !this.player.Settings.Bobbing;
      ((TextBox) e.Window).Text = this.OnOff(this.player.Settings.Bobbing);
    }

    private void ClickAutoPlaceSpeed(object sender, WindowEventArgs e)
    {
      this.player.ToggleAutoPlace();
      ((TextBox) e.Window).Text = this.player.Settings.GetAutoPlaceSettingText();
    }

    private void ClickWield(object sender, WindowEventArgs e)
    {
      WieldType wieldType = this.player.Settings.WieldType;
      if (this.player.Settings.WieldType == WieldType.BothHands)
        this.player.Settings.WieldType = WieldType.LeftHand;
      else
        ++this.player.Settings.WieldType;
      ((TextBox) e.Window).Text = this.player.Settings.WieldType.ToString();
      this.player.OnWieldTypeChanged(wieldType);
    }

    private void ClickVisualNotifications(object sender, WindowEventArgs e)
    {
      this.gameSettings.ToggleNotification(NotificationType.Visual);
      ((TextBox) e.Window).Text = this.OnOff(this.gameSettings.HasNotification(NotificationType.Visual));
    }

    private void ClickAudioNotifications(object sender, WindowEventArgs e)
    {
      this.gameSettings.ToggleNotification(NotificationType.Audio);
      ((TextBox) e.Window).Text = this.OnOff(this.gameSettings.HasNotification(NotificationType.Audio));
    }

    private void ClickSongNotifications(object sender, WindowEventArgs e)
    {
      this.gameSettings.ToggleNotification(NotificationType.Song);
      ((TextBox) e.Window).Text = this.OnOff(this.gameSettings.HasNotification(NotificationType.Song));
    }

    private void ClickReceiveTextMessages(object sender, WindowEventArgs e)
    {
      this.gameSettings.ToggleNotification(NotificationType.TextMsg);
      ((TextBox) e.Window).Text = this.OnOff(this.gameSettings.HasNotification(NotificationType.TextMsg));
    }

    private void ClickInputBindingsMainOptions(object sender, WindowEventArgs e)
    {
      this.InputBindingsBackMainOptions();
    }

    private void InputBindingsBackMainOptions()
    {
      this.canvas.FindChild("mainContainer").IsVisible = true;
      this.canvas.FindChild("creativeContainer").IsVisible = true;
      this.canvas.FindChild("notificationsContainer").IsVisible = true;
      this.canvas.FindChild("optionsContainer").IsVisible = true;
      this.canvas.FindChild("playerContainer").IsVisible = true;
      this.inputContainer.IsVisible = false;
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
      this.canvas.SetMouse(Point.Zero);
      this.ResetCanvasTabData();
    }

    private void ClickInputBindings(object sender, WindowEventArgs e)
    {
      this.canvas.FindChild("mainContainer").IsVisible = false;
      this.canvas.FindChild("creativeContainer").IsVisible = false;
      this.canvas.FindChild("notificationsContainer").IsVisible = false;
      this.canvas.FindChild("optionsContainer").IsVisible = false;
      this.canvas.FindChild("playerContainer").IsVisible = false;
      if (this.inputContainer == null)
        this.LoadInputProfile(Globals2.GetInputProfile(InputManager1.OrigProfile.Name), true);
      this.inputContainer.IsVisible = true;
      this.canvas.AdjustSizeToContainAllChildren(this.screenRect);
      this.canvas.SlidingScroll = true;
      this.canvas.SetMouse(Point.Zero);
    }

    private void ClickInputBindingsApplyAndSave(object sender, WindowEventArgs e)
    {
      Globals2.AddOrUpdateInputProfile(this.tempProfile);
      Globals2.SaveInputProfiles();
      InputManager1.Initialize(this.tempProfile);
      TextFileParser.WriteString("game.ini", "InputProfile", this.tempProfile.Name);
      this.InputBindingsBackMainOptions();
    }

    private void ClickInputBindingsRestoreDefaults(object sender, WindowEventArgs e)
    {
      InputManager1.RestoreDefaults(this.tempProfile);
      this.LoadInputProfile(this.tempProfile, false);
    }

    private void ClickInputBindingsDeleteProfile(object sender, WindowEventArgs e)
    {
      Globals2.DeleteInputProfile(this.tempProfile);
      Globals2.SaveInputProfiles();
      this.LoadInputProfile(Globals2.GetInputProfile(""), false);
    }

    private void ClickInputBindingsCancel(object sender, WindowEventArgs e)
    {
      this.InputBindingsBackMainOptions();
    }

    private void PopulateInputProfiles(Window win, List<string> list, string input)
    {
      list.Clear();
      foreach (InputProfile inputProfile in Globals2.InputProfiles)
      {
        if (inputProfile.Name.IsNotEmpty())
          list.Add(inputProfile.Name);
      }
    }

    private void ValidateInputProfile(ITextInputWindow win)
    {
      if (this.tempProfile.Name.Equals(win.Text, StringComparison.OrdinalIgnoreCase))
        return;
      if (win.Text == null)
        win.Text = "";
      InputProfile profile = Globals2.GetInputProfile(win.Text);
      bool clone = true;
      if (profile == null)
      {
        profile = this.tempProfile.Clone(this.player.Gamertag);
        profile.Name = win.Text;
        clone = false;
      }
      this.LoadInputProfile(profile, clone);
    }

    private void DragSliderMouseSmoothing(object sender, WindowDragEventArgs e)
    {
      this.tempProfile.MouseLookAtSmoothing = (byte) ((double) (float) e.Tag * 10.0);
      ((TextBox) sender).Text = this.tempProfile.MouseLookAtSmoothing.ToString();
    }

    private void ClickResetMusicShuffle(object sender, WindowEventArgs e)
    {
      this.instance.ResetMusicShuffle();
    }

    private void ClickItemOptions(object sender, WindowEventArgs e)
    {
      this.screenManager.AddScreen((GameScreen) new ItemOptionsScreen(this.instance, this.player, Block.ItemShop), new PlayerIndex?(this.playerIndex));
      this.ExitScreen();
    }

    private void InputBindingBeginInput(ITextInputWindow window)
    {
      window.Text = "";
    }

    private void InputBindingBeginKeyInput(ITextInputWindow window)
    {
      window.Text = "";
      ushort tag = (ushort) window.Tag;
      InputItem inputItem = this.tempProfile.InputScheme[tag];
      this.origItem = inputItem;
      inputItem.Key = Keys.None;
      this.tempProfile.InputScheme[tag] = inputItem;
      this.shiftEscaped = false;
    }

    private bool IsControlKey(Keys key)
    {
      switch (key)
      {
        case Keys.LeftShift:
        case Keys.RightShift:
        case Keys.LeftControl:
        case Keys.RightControl:
        case Keys.LeftAlt:
        case Keys.RightAlt:
          return true;
        default:
          return false;
      }
    }

    private Keys GetKeyInput(Keys[] keys, out bool alt, out bool control, out bool shift)
    {
      alt = shift = control = false;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      bool flag6 = false;
      Keys keys1 = Keys.None;
      if (keys != null && keys.Length > 0)
      {
        for (int index = 0; index < keys.Length; ++index)
        {
          switch (keys[index])
          {
            case Keys.LeftShift:
              flag5 = true;
              break;
            case Keys.RightShift:
              flag6 = true;
              break;
            case Keys.LeftControl:
              flag3 = true;
              break;
            case Keys.RightControl:
              flag4 = true;
              break;
            case Keys.LeftAlt:
              flag1 = true;
              break;
            case Keys.RightAlt:
              flag2 = true;
              break;
            default:
              if (keys1 == Keys.None)
              {
                keys1 = keys[index];
                break;
              }
              break;
          }
        }
        alt = flag1 || flag2;
        shift = flag5 || flag6;
        control = flag3 || flag4;
        if (keys1 == Keys.None)
        {
          if (flag1)
          {
            keys1 = Keys.LeftAlt;
            alt = false;
          }
          else if (flag2)
          {
            keys1 = Keys.RightAlt;
            alt = false;
          }
          if (flag3)
          {
            keys1 = Keys.LeftControl;
            control = false;
          }
          else if (flag4)
          {
            keys1 = Keys.RightControl;
            control = false;
          }
          if (flag5)
          {
            keys1 = Keys.LeftShift;
            shift = false;
          }
          else if (flag6)
          {
            keys1 = Keys.RightShift;
            shift = false;
          }
        }
      }
      return keys1;
    }

    private Keys GetNonControlKey(Keys[] keys, out bool alt, out bool control, out bool shift)
    {
      alt = shift = control = false;
      Keys keys1 = Keys.None;
      if (keys != null && keys.Length > 0)
      {
        for (int index = 0; index < keys.Length; ++index)
        {
          switch (keys[index])
          {
            case Keys.LeftShift:
            case Keys.RightShift:
              shift = true;
              break;
            case Keys.LeftControl:
            case Keys.RightControl:
              control = true;
              break;
            case Keys.LeftAlt:
            case Keys.RightAlt:
              alt = true;
              break;
            default:
              if (keys1 == Keys.None)
              {
                keys1 = keys[index];
                break;
              }
              break;
          }
        }
      }
      return keys1;
    }

    private Keys GetNonControlKey(Keys[] keys)
    {
      if (keys != null && keys.Length > 0)
      {
        for (int index = 0; index < keys.Length; ++index)
        {
          switch (keys[index])
          {
            case Keys.LeftShift:
            case Keys.RightShift:
            case Keys.LeftControl:
            case Keys.RightControl:
            case Keys.LeftAlt:
            case Keys.RightAlt:
              continue;
            default:
              return keys[index];
          }
        }
      }
      return Keys.None;
    }

    private bool InputBindingRawInputKey(ITextInputWindow window, out bool endInput)
    {
      Keys[] pressedKeys = InputManager.GetPressedKeys(this.playerIndex);
      Keys[] pressedKeysPrev = InputManager.GetPressedKeysPrev(this.playerIndex);
      if (pressedKeys != null && pressedKeys.Length > 0 && !this.shiftEscaped)
      {
        this.lastKeyWasValid = true;
        ushort tag = (ushort) window.Tag;
        InputItem origItem = this.tempProfile.InputScheme[tag];
        bool flag = origItem.Key != Keys.None && !this.IsControlKey(origItem.Key);
        Keys keyInput = this.GetKeyInput(pressedKeys, out origItem.KeyAlt, out origItem.KeyCtrl, out origItem.KeyShift);
        if (keyInput == Keys.Escape)
        {
          if (origItem.KeyShift)
          {
            if (this.IsNoneAValidKey(tag))
            {
              origItem.Key = Keys.None;
              origItem.KeyAlt = origItem.KeyCtrl = origItem.KeyShift = false;
              this.shiftEscaped = true;
            }
          }
          else
            origItem = this.origItem;
          this.tempProfile.InputScheme[tag] = origItem;
          window.Text = this.GetKeyText(tag, origItem);
        }
        else if (!this.IsControlKey(keyInput) || !flag)
        {
          if (this.IsValidInputKey(tag, keyInput))
          {
            origItem.Key = keyInput;
            this.tempProfile.InputScheme[tag] = origItem;
            window.Text = this.GetKeyText(tag, origItem);
          }
          else
            this.lastKeyWasValid = false;
        }
        if (keyInput != Keys.None)
          this.lastNonCtrlKey = keyInput;
        endInput = flag && (keyInput == Keys.None || this.IsControlKey(keyInput));
      }
      else
        endInput = (pressedKeys == null || pressedKeys.Length < 1) && (pressedKeysPrev != null && pressedKeysPrev.Length > 0 && this.lastKeyWasValid);
      return true;
    }

    private bool IsValidInputKey(ushort key, Keys newKey)
    {
      if (key < (ushort) 200 && key == (ushort) 52)
        return !this.IsControlKey(newKey);
      return true;
    }

    private bool IsNoneAValidKey(ushort key)
    {
      return key >= (ushort) 200 || key != (ushort) 52;
    }

    private bool InputBindingRawInputMouse(ITextInputWindow window, out bool endInput)
    {
      ushort tag = (ushort) window.Tag;
      InputItem inputItem = this.tempProfile.InputScheme[tag];
      StudioForge.Engine.Integration.MouseButtons buttonPressedNew = InputManager.GetMouseButtonPressedNew(this.playerIndex);
      if (buttonPressedNew != StudioForge.Engine.Integration.MouseButtons.None)
      {
        inputItem.MouseButton = buttonPressedNew;
        int nonControlKey = (int) this.GetNonControlKey(InputManager.GetPressedKeys(this.playerIndex), out inputItem.MouseAlt, out inputItem.MouseCtrl, out inputItem.MouseShift);
        this.tempProfile.InputScheme[tag] = inputItem;
        window.Text = this.GetMouseButtonText(tag, inputItem);
        endInput = false;
      }
      else
      {
        endInput = InputManager.IsMouseButtonReleasedNew(this.playerIndex, inputItem.MouseButton);
        if (!endInput)
        {
          bool alt;
          bool control;
          bool shift;
          if (this.GetNonControlKey(InputManager.GetPressedKeys(this.playerIndex), out alt, out control, out shift) == Keys.Escape)
          {
            if (shift)
            {
              inputItem.MouseButton = StudioForge.Engine.Integration.MouseButtons.None;
              inputItem.MouseAlt = inputItem.MouseShift = inputItem.MouseCtrl = false;
              this.tempProfile.InputScheme[tag] = inputItem;
            }
            window.Text = this.GetMouseButtonText(tag, inputItem);
            endInput = false;
          }
          else
            endInput = this.GetNonControlKey(InputManager.GetPressedKeysPrev(this.playerIndex)) == Keys.Escape;
        }
      }
      return true;
    }

    private bool InputBindingRawInputGamepad(ITextInputWindow window, out bool endInput)
    {
      ushort tag = (ushort) window.Tag;
      InputItem inputItem = this.tempProfile.InputScheme[tag];
      Buttons buttonPressedNew = InputManager.GetButtonPressedNew(this.playerIndex);
      if (buttonPressedNew != (Buttons) 0)
      {
        inputItem.Button = buttonPressedNew;
        this.tempProfile.InputScheme[tag] = inputItem;
        window.Text = this.GetGamepadText(tag, inputItem);
        endInput = false;
      }
      else
      {
        endInput = InputManager.IsButtonReleasedNew(this.playerIndex, inputItem.Button);
        if (!endInput)
        {
          bool alt;
          bool control;
          bool shift;
          if (this.GetNonControlKey(InputManager.GetPressedKeys(this.playerIndex), out alt, out control, out shift) == Keys.Escape)
          {
            if (shift)
            {
              inputItem.Button = (Buttons) 0;
              this.tempProfile.InputScheme[tag] = inputItem;
            }
            window.Text = this.GetGamepadText(tag, inputItem);
            endInput = false;
          }
          else
            endInput = this.GetNonControlKey(InputManager.GetPressedKeysPrev(this.playerIndex)) == Keys.Escape;
        }
      }
      return true;
    }
  }
}
