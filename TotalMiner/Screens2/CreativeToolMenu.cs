// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.CreativeToolMenu
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GUI;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal abstract class CreativeToolMenu
  {
    protected MapTM map;
    protected GameInstance instance;
    protected PauseMenuScreen2 parentScreen;
    protected Player player;
    protected PlayerIndex playerIndex;
    protected CreativeOperationData data;
    protected string statusText;
    protected int markerCount;
    protected NewGuiMenu blockSelectMenu;
    private TextBox statusWin;
    private TextBox applyWin;
    private Action onApplied;

    protected abstract bool IsPercentUsed { get; }

    protected virtual bool IsSeedUsed
    {
      get
      {
        return this.IsPercentUsed;
      }
    }

    protected virtual bool IsClearMarkersUsed
    {
      get
      {
        return false;
      }
    }

    protected virtual string SelectBlockText
    {
      get
      {
        return "Select Block";
      }
    }

    protected string BlockIDText
    {
      get
      {
        Item blockId = (Item) this.data.BlockID;
        if (blockId == Item.None)
          return "None";
        return ItemData2.ForDisplay(this.instance, blockId);
      }
    }

    protected virtual BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeFill;
      }
    }

    protected int RegionSizeInBlocks
    {
      get
      {
        return (this.data.Max.X - this.data.Min.X + 1) * (this.data.Max.Z - this.data.Min.Z + 1) * (this.data.Max.Y - this.data.Min.Y + 1);
      }
    }

    protected int RegionSizeInBlocks2D
    {
      get
      {
        return (this.data.Max.X - this.data.Min.X + 1) * (this.data.Max.Z - this.data.Min.Z + 1);
      }
    }

    public CreativeToolMenu(
      PauseMenuScreen2 parentScreen,
      GameInstance instance,
      Player player,
      CreativeOperationData data,
      Action onApplied)
    {
      this.parentScreen = parentScreen;
      this.instance = instance;
      this.player = player;
      this.map = data.Map;
      this.data = data;
      this.playerIndex = player.PlayerIndex;
      this.onApplied = onApplied;
    }

    public Window InitWindows()
    {
      int x = 0;
      int y1 = 0;
      int num1 = 250;
      int num2 = 500;
      int num3 = 34;
      int g = 4;
      int num4 = 8;
      int height = num3 * num4 + g * (num4 - 1);
      float num5 = 0.6f;
      Window container = new Window((string) null, x, y1, num1 + 1 + num2, height)
      {
        Name = "toolContainer"
      };
      container.Colors = Window.TransparentColorProfile;
      TextBox.DefaultTextAlignX = WinTextAlignX.Left;
      Window window1 = (Window) new TextBox("Command:", x, y1, num1, num3, num5);
      window1.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window1);
      Window window2 = (Window) new TextBox(this.data.Desc, x + num1 + 1, y1, num2, num3, num5);
      window2.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window2);
      int y2 = y1 + (num3 + g);
      Window window3 = (Window) new TextBox("Status:", x, y2, num1, num3, num5);
      window3.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window3);
      Window window4 = (Window) (this.statusWin = new TextBox((string) null, x + num1 + 1, y2, num2, num3, num5));
      window4.Colors = (Window.ColorProfile) Colors.LabelColors;
      container.AddChild((Node) window4);
      int y3 = y2 + (num3 + g);
      if (this.IsPercentUsed)
      {
        Window window5 = (Window) new TextBox("Percent:", x, y3, num1, num3, num5);
        window5.Colors = (Window.ColorProfile) Colors.LabelColors;
        container.AddChild((Node) window5);
        DataField dataField;
        Window window6 = (Window) (dataField = new DataField(this.data.Percent.ToString(), x + num1 + 1, y3, num2, num3, num5));
        window6.Colors = (Window.ColorProfile) Colors.DataFieldColors;
        ((ITextInputWindow) dataField).OnValidateInput = new Action<ITextInputWindow>(this.ValidatePercent);
        container.AddChild((Node) window6);
        y3 += num3 + g;
      }
      if (this.IsSeedUsed)
      {
        Window window5 = (Window) new TextBox("Seed:", x, y3, num1, num3, num5);
        window5.Colors = (Window.ColorProfile) Colors.LabelColors;
        container.AddChild((Node) window5);
        DataField dataField;
        Window window6 = (Window) (dataField = new DataField(this.data.Seed.ToString(), x + num1 + 1, y3, num2, num3, num5));
        window6.Colors = (Window.ColorProfile) Colors.DataFieldColors;
        ((ITextInputWindow) dataField).OnValidateInput = new Action<ITextInputWindow>(this.ValidateSeed);
        container.AddChild((Node) window6);
        y3 += num3 + g;
      }
      if (this.IsClearMarkersUsed)
      {
        Window window5 = (Window) new TextBox("Clear Markers:", x, y3, num1, num3, num5);
        window5.Colors = (Window.ColorProfile) Colors.LabelColors;
        container.AddChild((Node) window5);
        Window window6 = (Window) new TextBox(this.OnOff(this.data.ClearMarkers), x + num1 + 1, y3, num2, num3, num5);
        window6.Colors = (Window.ColorProfile) Colors.ButtonColors;
        window6.ClickHandler += new Window.WindowHandler(this.ClickClearMarkers);
        container.AddChild((Node) window6);
        y3 += num3 + g;
      }
      int num6 = this.InitWindowsExtra(container, x, y3, num1, num2, num3, g, num5);
      TextBox.DefaultTextAlignX = WinTextAlignX.Center;
      int y4 = num6 + (num3 + g);
      TextBox textBox = new TextBox("Apply", x, y4, num1 + 1 + num2, num3, num5);
      textBox.Name = "apply";
      Window parent = (Window) (this.applyWin = textBox);
      parent.IsEnabled = false;
      parent.Colors = (Window.ColorProfile) Colors.ButtonColors;
      parent.ClickHandler += new Window.WindowHandler(this.ClickApply);
      container.AddChild((Node) parent);
      if (InputManager.IsUsingGamePad)
        GraphicStatics.AddIcon(parent, GraphicStatics.ButtonTexture(Buttons.Y), new Rectangle((num1 + 1 + num2) / 2 + 40, (parent.Size.Y - 20) / 2 + 1, 18, 18));
      int num7 = y4 + (num3 + g);
      this.RefreshWindowText();
      return container;
    }

    protected abstract int InitWindowsExtra(
      Window container,
      int x,
      int y,
      int w,
      int w2,
      int h,
      int g,
      float scale);

    protected void RefreshWindowText()
    {
      this.markerCount = this.instance.CreativeModeHelper.MarkerBlockCount(this.player.GamerID);
      this.data.IsValid = true;
      this.statusText = "No Errors. Ready to Apply.";
      this.BuildMessageText();
      this.statusWin.Text = this.statusText;
      this.statusWin.Colors = this.data.IsValid ? (Window.ColorProfile) Colors.StatusGreen : (Window.ColorProfile) Colors.StatusRed;
      this.RefreshWindowTextCore();
      this.UpdateDefaults();
      this.applyWin.IsEnabled = this.data.IsValid;
    }

    protected virtual void RefreshWindowTextCore()
    {
    }

    protected abstract void UpdateDefaults();

    protected abstract void BuildMessageText();

    protected string OnOff(bool o)
    {
      return !o ? "Off" : "On";
    }

    private void ValidatePercent(ITextInputWindow win)
    {
      int result;
      if (int.TryParse(win.Text, out result))
        this.data.Percent = (byte) MyMathHelper.Clamp(result, 0, 100);
      win.Text = this.data.Percent.ToString();
      this.RefreshWindowText();
    }

    private void ValidateSeed(ITextInputWindow win)
    {
      int result;
      if (int.TryParse(win.Text, out result))
        this.data.Seed = MyMathHelper.Clamp(result, 0, int.MaxValue);
      win.Text = this.data.Seed.ToString();
      this.RefreshWindowText();
    }

    public void ClickBlockID(object sender, WindowEventArgs e)
    {
      if (this.blockSelectMenu == null)
        this.blockSelectMenu = (NewGuiMenu) new BlockSelectMenu(this.instance, this.player, this.SelectBlockText, this.BlockSelectMode, new Action<Item>(this.OnBlockSelected));
      this.parentScreen.PushOtherTab(this.blockSelectMenu);
    }

    protected virtual void OnBlockSelected(Item block)
    {
      this.data.BlockID = (byte) block;
      this.RefreshWindowText();
      this.parentScreen.PopOtherTab();
    }

    protected virtual bool OnBlockSelected(Player player, Block block)
    {
      this.data.BlockID = (byte) block;
      this.RefreshWindowText();
      return true;
    }

    protected void ClickClearMarkers(object sender, WindowEventArgs e)
    {
      this.data.ClearMarkers = !this.data.ClearMarkers;
      ((TextBox) e.Window).Text = this.OnOff(this.data.ClearMarkers);
    }

    protected void ClickApply(object sender, WindowEventArgs e)
    {
      if (this.data.IsValid)
      {
        if (!this.OnExecuteCore() || this.onApplied == null)
          return;
        this.onApplied();
      }
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("Some setup is invalid", this.playerIndex);
    }

    protected abstract bool OnExecuteCore();

    protected void SendNetworkCommand()
    {
      NetworkManager.Instance.SendCreativeCommand(this.data.Command, (Block) this.data.BlockID, (Block) this.data.BlockID1, (Block) this.data.BlockID2, this.data.Percent, this.data.Seed, this.data.ClearMarkers, this.data.Desc, this.data.Point, this.data.Min, this.data.Max, this.data.XMin, this.data.XMax, this.data.GamerID, this.data.Data);
    }

    protected bool WillCommandAffectNoEditZone()
    {
      BoundingBox box1 = Globals2.GetBox(this.data.Min, this.data.Max, 0.01f);
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
      {
        for (int index = 0; index < mapStrategy.Zones.Count; ++index)
        {
          Zone zone = mapStrategy.Zones[index];
          if (zone.HasZoneType(ZoneType.NoEdit))
          {
            BoundingBox box2 = Globals2.GetBox(zone.Min, zone.Max, 0.01f);
            if (box2.Intersects(box1))
            {
              BoundingBox box3 = Globals2.GetBox(this.data.XMin, this.data.XMax, 0.01f);
              bool flag = (double) box3.Min.X <= (double) box2.Min.X && (double) box3.Max.X >= (double) box2.Max.X && (double) box3.Min.Z <= (double) box2.Min.Z && (double) box3.Max.Z >= (double) box2.Max.Z;
              if (zone.HasZoneType(ZoneType.Spawn) || !flag && this.player != null && !this.player.IsAdmin && (zone.BuilderType == ZoneBuilderType.None || zone.BuilderType == ZoneBuilderType.Player && this.player.Gamertag != zone.Builder || zone.BuilderType == ZoneBuilderType.Clan && this.player.ClanName != zone.Builder))
                return true;
            }
          }
        }
      }
      return false;
    }

    protected bool IsPlayerInsideRegion(byte blockID, bool allowNoClip)
    {
      if (!this.map.IsBlockPassable(blockID))
      {
        foreach (Gamer allEnabledGamer in NetworkManager.Instance.AllEnabledGamers)
        {
          Player tag = allEnabledGamer.Tag as Player;
          if (tag != null && (!allowNoClip || !this.instance.IsCreativeMode || !this.player.IsAdmin))
          {
            GlobalPoint3D point1 = this.map.GetPoint(tag.Position);
            GlobalPoint3D point2 = this.map.GetPoint(tag.EyePosition);
            if (this.IsInsideRegion(point1) || this.IsInsideRegion(point2))
              return true;
          }
        }
      }
      return false;
    }

    private bool IsInsideRegion(GlobalPoint3D p)
    {
      if (p.X >= this.data.Min.X && p.Y >= this.data.Min.Y && (p.Z >= this.data.Min.Z && p.X <= this.data.Max.X) && p.Y <= this.data.Max.Y)
        return p.Z <= this.data.Max.Z;
      return false;
    }
  }
}
