// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.IGameObject
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Core
{
  public interface IGameObject : IHasUpdate, IRecycled, IHasInitialization, IHasContent
  {
    List<IGameObject> Children { get; }

    bool HandleInput(InputState input, PlayerIndex playerIndex);
  }
}
