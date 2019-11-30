// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.AIBehaviourTreeDesignWindow
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.AI;

namespace StudioForge.TotalMiner.Screens2
{
  internal class AIBehaviourTreeDesignWindow : BehaviourTreeDesignWindow
  {
    private NpcBase trackNPC;
    private Window nodeTrackWin;
    private Window lastTrackedNodeWin;

    protected override bool CanShowPropertiesEditor
    {
      get
      {
        if (this.trackNPC == null)
          return base.CanShowPropertiesEditor;
        return false;
      }
    }

    public AIBehaviourTreeDesignWindow(
      PlayerIndex playerIndex,
      int x,
      int y,
      int w,
      int h,
      BehaviourTree target,
      NpcBase trackNPC,
      Texture2D iconTextures,
      Point nodeWindowsOffset)
      : base(playerIndex, x, y, w, h, target != null ? target : trackNPC.BehaviourTree, iconTextures, nodeWindowsOffset)
    {
      this.trackNPC = trackNPC;
    }

    public override void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
      if (this.trackNPC != null && this.trackNPC.BehaviourTree == this.nodeTree)
      {
        if (this.trackNPC.IsDeadOrInactiveOrDisabled)
        {
          this.trackNPC.BehaviourTree.TrackType = BehaviourTrackType.None;
          this.TrackWin((Window) this.designTree.Win, StudioForge.TotalMiner.Colors.GrayTrack);
        }
        else
        {
          BehaviourTreeNode lastNode = ((INPCBehaviour) this.trackNPC).LastNode;
          if (lastNode != null)
          {
            DesignerNode designerNode = this.GetDesignerNode((object) lastNode);
            Window win = (Window) null;
            if (designerNode != null)
              win = (Window) designerNode.Win;
            Window.ColorProfile color = lastNode.Status == BehaviourTreeNodeStatus.Success ? StudioForge.TotalMiner.Colors.GreenTrack : (lastNode.Status == BehaviourTreeNodeStatus.Failure ? StudioForge.TotalMiner.Colors.RedTrack : StudioForge.TotalMiner.Colors.BlueTrack);
            this.TrackWin(win, color);
          }
        }
      }
      base.Draw(spriteBatch, bound, scale, alpha, isEnabled);
    }

    private void TrackWin(Window win, Window.ColorProfile color)
    {
      if (win == this.lastTrackedNodeWin)
        return;
      int num = 7;
      if (this.nodeTrackWin == null)
      {
        this.nodeTrackWin = new Window((string) null, 0, 0, win.Size.X, win.Size.Y)
        {
          BorderThickness = num
        };
        this.nodeTrackWin.Colors = Window.TransparentColorProfile;
      }
      this.lastTrackedNodeWin = win;
      this.nodeTrackWin.IsVisible = win != null;
      this.nodeTrackWin.RemoveSelf();
      if (win != null)
      {
        win.AddChild((Node) this.nodeTrackWin);
        this.nodeTrackWin.IsVisible = true;
        this.nodeTrackWin.Colors = color;
        this.nodeTrackWin.Size = win.Size;
      }
      else
        this.nodeTrackWin.IsVisible = false;
    }
  }
}
