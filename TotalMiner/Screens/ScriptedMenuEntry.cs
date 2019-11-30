// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ScriptedMenuEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;

namespace StudioForge.TotalMiner.Screens
{
  internal class ScriptedMenuEntry : BlockMenuEntry
  {
    private ScriptMenuParam param;

    public ScriptedMenuEntry(ScriptedMenuScreen screen, ScriptMenuParam param)
      : base((BlockMenuScreen) screen, param.Text)
    {
      this.param = param;
    }

    protected void OnSelectEntry(PlayerIndex playerIndex)
    {
      if (GameInstance.Instance == null)
        this.Screen.ExitScreen();
      if (this.param.Script.IsNotEmpty())
      {
        ScriptedMenuScreen screen = this.Screen as ScriptedMenuScreen;
        if (screen != null)
        {
          GlobalPoint3D? nullable1 = screen.BlockOffset;
          GlobalPoint3D? nullable2 = screen.ScriptOffset;
          if (this.param.Point.HasValue)
          {
            switch (this.param.Coord)
            {
              case ScriptCoordType.None:
              case ScriptCoordType.Absolute:
                GlobalPoint3D globalPoint3D1 = (GlobalPoint3D) this.param.Point.Value;
                GlobalPoint3D? nullable3;
                if (!nullable2.HasValue)
                {
                  nullable3 = new GlobalPoint3D?(globalPoint3D1);
                }
                else
                {
                  GlobalPoint3D? nullable4 = nullable2;
                  GlobalPoint3D globalPoint3D2 = globalPoint3D1;
                  nullable3 = nullable4.HasValue ? new GlobalPoint3D?(nullable4.GetValueOrDefault() + globalPoint3D2) : new GlobalPoint3D?();
                }
                nullable2 = nullable3;
                break;
              default:
                GlobalPoint3D globalPoint3D3 = (GlobalPoint3D) this.param.Point.Value;
                GlobalPoint3D? nullable5;
                if (!nullable1.HasValue)
                {
                  nullable5 = new GlobalPoint3D?(globalPoint3D3);
                }
                else
                {
                  GlobalPoint3D? nullable4 = nullable1;
                  GlobalPoint3D globalPoint3D2 = globalPoint3D3;
                  nullable5 = nullable4.HasValue ? new GlobalPoint3D?(nullable4.GetValueOrDefault() + globalPoint3D2) : new GlobalPoint3D?();
                }
                nullable1 = nullable5;
                break;
            }
          }
          ScriptExecuteData data = new ScriptExecuteData()
          {
            Actor = (Actor) ((BlockMenuScreen) this.Screen).Player,
            ScriptOffset = nullable2,
            BlockOffset = nullable1
          };
          GameInstance.Instance.ExecuteScript(this.param.Script, data, true);
        }
        this.Screen.ExitScreen();
      }
      else
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
    }
  }
}
