// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GameState.PlayerIndexEventArgs
// Assembly: StudioForge.Engine.Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4214C167-4C85-4E65-8D0A-403DABFB3D82
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Game.dll

using Microsoft.Xna.Framework;
using System;

namespace StudioForge.Engine.GameState
{
  public class PlayerIndexEventArgs : EventArgs
  {
    private PlayerIndex playerIndex;

    public PlayerIndexEventArgs(PlayerIndex playerIndex)
    {
      this.playerIndex = playerIndex;
    }

    public PlayerIndexEventArgs(PlayerIndex? playerIndex)
    {
      this.playerIndex = playerIndex.HasValue ? playerIndex.Value : PlayerIndex.One;
    }

    public PlayerIndex PlayerIndex
    {
      get
      {
        return this.playerIndex;
      }
    }
  }
}
