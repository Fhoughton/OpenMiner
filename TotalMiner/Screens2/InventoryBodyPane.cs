// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.InventoryBodyPane
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class InventoryBodyPane : InventoryPane
  {
    public InventoryBodyPane(
      NewGuiMenu2 parentTab,
      Inventory inventory,
      Point slotSize,
      InventorySlotWinFlags flags,
      Action<InventorySlotWin, bool> itemSelected)
      : base(parentTab, inventory, 0, slotSize, flags, itemSelected)
    {
    }

    public new Window InitWindows()
    {
      Window window = (Window) null;
      EquipmentInventory inventory = this.inventory as EquipmentInventory;
      if (inventory != null)
      {
        this.screenRect = new Rectangle(0, 0, 836, 900);
        this.renderProfile = new RenderProfile()
        {
          Sampler = SamplerState.PointClamp
        };
        window = this.mainWin = new Window((string) null, 0, 0, this.screenRect.Width, this.screenRect.Height)
        {
          Name = "main"
        };
        window.Colors = Colors.GreenTrack;
        window.RenderProfile = this.renderProfile;
        window.LoadTexture("Textures\\Boy2D");
        window.Texture.TintColor = Color.WhiteSmoke * 0.2f;
        Rectangle newDestRect = window.Texture.GetNewDestRect();
        newDestRect.X = 14;
        newDestRect.Y = 32;
        window.Texture.DestRect = new Rectangle?(newDestRect);
        this.screenRect.Width = window.Size.X = 28 + window.Texture.Texture.Width;
        int num = 4;
        int x1 = this.slotSize.X + 40;
        int y1 = num;
        int y2 = this.slotSize.Y;
        window.AddChild((Node) this.NewSlotWin(x1, y1, inventory.HeadIndex));
        int y3 = y1 + (y2 + num + 26);
        window.AddChild((Node) this.NewSlotWin(x1, y3, inventory.NeckIndex));
        int y4 = y3 + (y2 + num + 8);
        window.AddChild((Node) this.NewSlotWin(x1, y4, inventory.BodyIndex));
        int y5 = y4 + (y2 + num + 32);
        window.AddChild((Node) this.NewSlotWin(x1, y5, inventory.LegsIndex));
        int y6 = y5 + (y2 + num + 16);
        window.AddChild((Node) this.NewSlotWin(x1, y6, inventory.FeetIndex));
        int x2 = x1 - (this.slotSize.X + 4 + 36);
        int y7 = y6 - ((y2 + num) * 2 - 4);
        window.AddChild((Node) this.NewSlotWin(x2, y7, inventory.RightSideIndex));
        int x3 = x2 + ((this.slotSize.X + 4) * 2 + 72);
        window.AddChild((Node) this.NewSlotWin(x3, y7, inventory.LeftSideIndex));
      }
      return window;
    }
  }
}
