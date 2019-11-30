// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ZoneMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class ZoneMenuEntry : BlockMenuEntry
  {
    public bool IsChanged;
    public Zone Zone;
    private Texture2D checkboxOn;
    private Texture2D checkboxOff;
    private ZoneEditScreen screen;
    private GameInstance instance;
    private Player player;

    public ZoneMenuEntry(
      ZoneEditScreen screen,
      GameInstance instance,
      Player player,
      Zone zone,
      string name)
      : base((BlockMenuScreen) screen, name)
    {
      this.screen = screen;
      this.Zone = zone;
      this.instance = instance;
      this.player = player;
      this.ColorHighlighted = Color.DarkGray;
      this.SelectLeft += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (--screen.Column != -1)
          return;
        screen.Column = screen.MaxColumns - 1;
      });
      this.SelectRight += (EventHandler<PlayerIndexEventArgs>) ((o, e) =>
      {
        if (++screen.Column != screen.MaxColumns)
          return;
        screen.Column = 0;
      });
      this.checkboxOn = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOn");
      this.checkboxOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
    }

    public void OnSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      switch (this.screen.Column)
      {
        case 0:
          this.ToggleAll();
          break;
        case 1:
          if (this.instance.IsDigDeepMode && this.Zone.Min.Y < -200)
          {
            this.screen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Spawn zones cannot be deeper than 200 in Dig Deep", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.screen.ControllingPlayer);
            break;
          }
          if (!this.IsValidSpawnZone(this.Zone) && !this.Zone.HasZoneType(ZoneType.Spawn))
          {
            this.screen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Spawn zones must be clear of blocks and at least 2 blocks high", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.screen.ControllingPlayer);
            break;
          }
          this.Toggle(ZoneType.Spawn);
          break;
        case 2:
          this.Toggle(ZoneType.NoFly);
          break;
        case 3:
          this.Toggle(ZoneType.NoCombat);
          break;
        case 4:
          this.Toggle(ZoneType.NoMobs);
          break;
        case 5:
          if (!this.Zone.HasZoneType(ZoneType.Spawn))
          {
            this.Toggle(ZoneType.NoEdit);
            break;
          }
          this.screen.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Spawn zones must be non-edit", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.screen.ControllingPlayer);
          break;
        case 6:
          this.screen.ScreenManager.AddScreen((GameScreen) new ZoneEditOtherScreen(this.instance, this.player, this.Zone, this), this.screen.ControllingPlayer);
          break;
      }
    }

    private void Toggle(ZoneType type)
    {
      if (type == ZoneType.NoMobs && this.instance.IsSurvivalMode)
        return;
      this.Zone.ToggleType(type);
      if (this.Zone.HasZoneType(ZoneType.Spawn) || this.Zone.HasZoneType(ZoneType.Moving))
        this.Zone.SetType(ZoneType.NoEdit, true);
      this.IsChanged = true;
    }

    private void ToggleAll()
    {
      if (!this.Zone.HasZoneType(ZoneType.NoCombat))
      {
        this.Zone.ZoneType = ZoneType.NoEdit | ZoneType.NoCombat | ZoneType.NoFly | ZoneType.NoMobs;
        if (this.instance.IsSurvivalMode)
          this.Zone.SetType(ZoneType.NoMobs, false);
      }
      else
        this.Zone.ZoneType = ZoneType.None;
      this.IsChanged = true;
    }

    private bool IsValidSpawnZone(Zone zone)
    {
      if (zone.Max.Y - zone.Min.Y > 0)
        return this.instance.Map.IsOnly(zone.Min, zone.Max, new Map.IsOnlyQualifier(this.IsValidSpawnZoneBlock));
      return false;
    }

    private bool IsValidSpawnZoneBlock(byte blockID)
    {
      if (blockID != (byte) 0)
        return this.instance.Map.IsBlockPassable(blockID);
      return true;
    }

    private bool IsValidMovingZone(Zone zone)
    {
      return this.instance.Map.IsOnly(zone.Min, zone.Max, new Map.IsOnlyQualifier(this.IsValidMovingZoneBlock));
    }

    private bool IsValidMovingZoneBlock(byte blockID)
    {
      return true;
    }

    public override void Update(MenuScreen screen, bool isSelected)
    {
      base.Update(screen, isSelected);
      if (this.screen.Column == 6)
        this.ToolTip.Text = "Press A to see other zone options not shown on this main zone screen.";
      else
        this.ToolTip.Text = (string) null;
    }

    public override void Draw(Vector2 position, int index, bool isSelected)
    {
      Color color = this.IsEnabled ? (isSelected ? this.ColorSelected : this.ColorUnselected) : this.ColorDisabled;
      color = new Color((int) color.R, (int) color.G, (int) color.B, (int) this.Screen.TransitionAlpha);
      if (isSelected)
        this.DrawHighLight(position, this.ColorHighlighted);
      this.DrawItem(position, color);
      this.DrawTexture(position, color);
    }

    private void DrawItem(Vector2 position, Color color)
    {
      position.X += 43f;
      position.Y += 4f;
      this.Screen.SpriteBatch.DrawString(this.Screen.ItemFont, this.Text, position + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 16f;
      position.Y -= 2f;
      position.X += 354f;
      this.Screen.SpriteBatch.Draw(this.Zone.HasZoneType(ZoneType.Spawn) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 66f;
      this.Screen.SpriteBatch.Draw(this.Zone.HasZoneType(ZoneType.NoFly) ? this.checkboxOff : this.checkboxOn, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 53f;
      this.Screen.SpriteBatch.Draw(this.Zone.HasZoneType(ZoneType.NoCombat) ? this.checkboxOff : this.checkboxOn, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 66f;
      this.Screen.SpriteBatch.Draw(this.instance.IsSurvivalMode || !this.Zone.HasZoneType(ZoneType.NoMobs) ? this.checkboxOn : this.checkboxOff, position, new Rectangle?(), this.instance.IsSurvivalMode ? Color.DarkGray : Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 65f;
      this.Screen.SpriteBatch.Draw(this.Zone.HasZoneType(ZoneType.NoEdit) ? this.checkboxOff : this.checkboxOn, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
      position.X += 63f;
      this.Screen.SpriteBatch.Draw(!this.Zone.HasOtherOptions ? this.checkboxOff : this.checkboxOn, position, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, this.Screen.ItemTextScale, SpriteEffects.None, 0.0f);
    }

    protected override void DrawHighLight(Vector2 position, Color color)
    {
      Rectangle highlightRect = ((PanelMenuScreen) this.Screen).HighlightRect;
      highlightRect.X += (int) position.X;
      highlightRect.Y += (int) position.Y - 4;
      Color color1 = this.ColorHighlighted * ((float) this.Screen.TransitionAlpha / (float) byte.MaxValue);
      Color fillColor = this.ColorHighlighted * (float) ((double) this.Screen.TransitionAlpha / (double) byte.MaxValue * 0.5);
      switch (this.screen.Column)
      {
        case 1:
          highlightRect.X += 384;
          highlightRect.Width = 75;
          break;
        case 2:
          highlightRect.X += 464;
          highlightRect.Width = 49;
          break;
        case 3:
          highlightRect.X += 515;
          highlightRect.Width = 52;
          break;
        case 4:
          highlightRect.X += 577;
          highlightRect.Width = 60;
          break;
        case 5:
          highlightRect.X += 647;
          highlightRect.Width = 49;
          break;
        case 6:
          highlightRect.X += 704;
          highlightRect.Width = 62;
          break;
        default:
          highlightRect.Width = 376;
          break;
      }
      this.Screen.SpriteBatch.DrawFilledBox(highlightRect, 1, color1, fillColor);
      this.lastHighLightRect = highlightRect;
    }
  }
}
