// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.CreativeCommandReplaceClipboardScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class CreativeCommandReplaceClipboardScreen : CreativeCommandScreen
  {
    protected override string SelectBlockText
    {
      get
      {
        return "Select Block to Replace";
      }
    }

    protected override bool IsBoundUsed
    {
      get
      {
        return false;
      }
    }

    protected override bool IsPercentUsed
    {
      get
      {
        return true;
      }
    }

    private string BlockID1Text
    {
      get
      {
        return "Block To Replace: " + ((Block) this.data.BlockID).ToString();
      }
    }

    private string BlockID2Text
    {
      get
      {
        return "Replace With: " + ((Block) this.data.BlockID1).ToString();
      }
    }

    protected override BlockSelectMode BlockSelectMode
    {
      get
      {
        return BlockSelectMode.CreativeReplace;
      }
    }

    public CreativeCommandReplaceClipboardScreen(GameInstance instance, Player player)
      : base(instance, player, CreativeCommandReplaceClipboardScreen.GetCommandData(instance, player))
    {
      this.data.Map = player.ClipboardModel.Map;
      this.data.Min = GlobalPoint3D.Zero;
      this.data.Max = this.data.Map.MapSize - GlobalPoint3D.One;
      this.data.XMin = this.data.XMax = GlobalPoint3D.MaxValue;
    }

    private static CreativeOperationData GetCommandData(
      GameInstance instance,
      Player player)
    {
      CreativeOperationData clipboardCommandData = instance.CreativeModeHelper.GetReplaceClipboardCommandData(player);
      clipboardCommandData.Map = player.ClipboardModel.Map;
      clipboardCommandData.Min = GlobalPoint3D.Zero;
      clipboardCommandData.Max = clipboardCommandData.Map.MapSize - GlobalPoint3D.One;
      clipboardCommandData.XMin = clipboardCommandData.XMax = GlobalPoint3D.MaxValue;
      clipboardCommandData.OnCompletion = new Action<CreativeOperationData>(CreativeCommandReplaceClipboardScreen.OnClipboardReplaceComplete);
      return clipboardCommandData;
    }

    private static void OnClipboardReplaceComplete(CreativeOperationData op)
    {
      op.Map.Regions[0].Chunks[0].LoadMesh(true, true);
    }

    protected override void AddParamItems(List<BlockMenuEntry> items)
    {
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockID1Text));
      items[this.baseItemCount].Selected += new EventHandler<PlayerIndexEventArgs>(((CreativeCommandScreen) this).OnSelectBlock);
      items.Add(new BlockMenuEntry((BlockMenuScreen) this, this.BlockID2Text));
      items[this.baseItemCount + 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectBlock2);
    }

    protected override void RefreshItemTextCore()
    {
      this.MenuEntries[this.baseItemCount].Text = this.BlockID1Text;
      this.MenuEntries[this.baseItemCount + 1].Text = this.BlockID2Text;
    }

    protected override void BuildMessageText()
    {
      if (!this.player.IsClipboardEquipped)
      {
        this.statusText = "Must have clipboard equipped";
        this.data.IsValid = false;
      }
      else
      {
        if ((int) this.data.BlockID != (int) this.data.BlockID1)
          return;
        this.statusText = "Must specify a different replacement block";
        this.data.IsValid = false;
      }
    }

    protected override void UpdateDefaults()
    {
      this.player.SetCreativeReplaceClipboardDefaults(this.data);
    }

    private void OnSelectBlock2(object sender, PlayerIndexEventArgs e)
    {
      this.instance.AddScreen((GameScreen) new BlockSelectionScreen(this.instance, this.player, new SelectBlockCallBack(this.OnBlock2Selected), "Select Replacement Block", this.BlockSelectMode), this.player);
    }

    private bool OnBlock2Selected(Player player, Block block)
    {
      this.data.BlockID1 = (byte) block;
      this.RefreshItemText();
      return true;
    }

    protected override bool OnExecuteCore()
    {
      if (this.data.Seed == 0)
      {
        this.data.IsCustomSeed = false;
        this.data.Seed = this.instance.Random.Next();
      }
      this.instance.CreativeCommandQueue.Execute(this.data, true);
      return true;
    }
  }
}
