// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandSphereScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandSphereScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Block for the Sphere";
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return true;
      }
    }

    protected override bool IsClearMarkersUsed
    {
      get
      {
        return false;
      }
    }

    private string BlockIDText
    {
      get
      {
        return "Block: " + ((Block) this.data.BlockID).ToString();
      }
    }

    private string RadiusText
    {
      get
      {
        return "Radius: " + this.data.BlockID1.ToString();
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeFill;
      }
    }

    public CreativeCommandSphereScreen(GameInstance instance, Player player)
      : base(instance, player, instance.CreativeModeHelper.GetSphereCommandData(player))
    {
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockIDText));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(((CreativeCommandScreen) this).OnSelectBlock);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.RadiusText));
      items[this.baseItemCount + 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnEnterRadius);
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.BlockIDText;
      this.MenuEntries[this.baseItemCount + 1].Text = this.RadiusText;
    }

    protected override void BuildMessageText()
    {
      if (this.markerCount >= 1)
        return;
      this.statusText = "Must have at least 1 marker";
      this.data.IsValid = false;
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeSphereDefaults(this.data);
    }

    private void OnEnterRadius(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnRadiusEntered), (int) this.data.BlockID1, false), this.ControllingPlayer);
    }

    private void OnRadiusEntered(double value, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      this.data.BlockID1 = (byte) MyMathHelper.Clamp((int) value, 1, 150);
      this.RefreshItemText();
    }

    protected override bool OnExecuteCore()
    {
      bool flag = this.data.Seed == 0;
      int num = 0;
      lock (this.map.MapStrategyTM.MarkerBlocks)
      {
        foreach (MarkerBlock markerBlock in this.map.MapStrategyTM.MarkerBlocks)
        {
          if (markerBlock.GamerID == this.data.GamerID)
          {
            if (flag)
            {
              this.data.IsCustomSeed = false;
              this.data.Seed = this.instance.Random.Next();
            }
            this.data.Point = markerBlock.Point;
            this.instance.CreativeCommandQueue.Execute(this.data, true);
            this.SendNetworkCommand();
            num += (int) this.data.BlockID1;
            if (num > 100)
              break;
          }
        }
      }
      this.instance.CreativeModeHelper.RemoveMarkers(this.data.GamerID, true);
      return true;
    }
  }
}
