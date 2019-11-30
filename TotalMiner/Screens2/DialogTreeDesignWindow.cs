// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.DialogTreeDesignWindow
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.AI;

namespace StudioForge.TotalMiner.Screens2
{
  internal class DialogTreeDesignWindow : BehaviourTreeDesignWindow
  {
    public DialogTreeDesignWindow(
      PlayerIndex playerIndex,
      int x,
      int y,
      int w,
      int h,
      BehaviourTree target,
      Texture2D iconTextures,
      Point nodeWindowsOffset)
      : base(playerIndex, x, y, w, h, target, iconTextures, nodeWindowsOffset)
    {
    }

    protected override Window.ColorProfile GetNodeBackColor(DesignerNode node)
    {
      DialogNode tag = node.Tag as DialogNode;
      if (tag == null)
        return base.GetNodeBackColor(node);
      if (DialogNode.GetBranchDepth((Node) tag) % 2 != 1)
        return (Window.ColorProfile)StudioForge.TotalMiner.Colors.DialogGold;
      return (Window.ColorProfile)StudioForge.TotalMiner.Colors.DialogSilver;
    }

    public override bool CanAddNode(DesignerNode parent)
    {
      if (parent.Parent == this.designTree && parent.Tag is DialogNode)
        return this.IsFirstDialogChild(parent.Parent, (Node) parent);
      return true;
    }

    private bool IsFirstDialogChild(Node parent, Node child)
    {
      if (parent == null || child == null)
        return false;
      if (child == parent.FirstChild)
        return true;
      for (DesignerNode nextSibling = parent.FirstChild.NextSibling as DesignerNode; nextSibling != null; nextSibling = nextSibling.NextSibling as DesignerNode)
      {
        if (nextSibling.Tag is DialogNode)
          return nextSibling == child;
      }
      return false;
    }
  }
}
