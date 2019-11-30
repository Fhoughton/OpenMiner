// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Rumble
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;

namespace StudioForge.TotalMiner
{
  internal class Rumble
  {
    private bool isEnabled;
    private float timer;
    private Player player;
    private float lastRumbleLeftStrength;
    private float lastRumbleRightStrength;

    public Rumble(Player player)
    {
      this.isEnabled = false;
      this.player = player;
    }

    public void Update()
    {
      if (!this.isEnabled)
        return;
      if ((double) this.timer > 0.0)
      {
        this.timer -= Services.ElapsedTime;
      }
      else
      {
        this.isEnabled = false;
        this.timer = 0.0f;
        GamePad.SetVibration(this.player.PlayerIndex, 0.0f, 0.0f);
      }
    }

    public void StartRumble(RumbleType type)
    {
      this.StartRumble(type, 1f);
    }

    public void StartRumble(RumbleType type, float strength)
    {
      switch (type)
      {
        case RumbleType.ClearBlock:
          this.SetNewRumble(0.15f, 0.0f, 0.15f);
          break;
        case RumbleType.Explosion:
          this.SetNewRumble(0.75f, 0.5f, 0.5f);
          break;
        case RumbleType.Fall:
        case RumbleType.Damage:
          this.SetNewRumble(0.25f, 0.1f * strength, 0.1f * strength);
          break;
        case RumbleType.Strike:
          this.SetNewRumble(0.1f, 0.0f, strength);
          break;
        default:
          this.SetNewRumbleCore(0.0f, 0.0f, 0.0f);
          break;
      }
    }

    public void SetNewRumble(float timer, float leftStrength, float rightStrength)
    {
      if (!InputManager1.Profile.GamePadRumble || this.isEnabled && (double) leftStrength <= (double) this.lastRumbleLeftStrength && (double) rightStrength <= (double) this.lastRumbleRightStrength)
        return;
      this.SetNewRumbleCore(timer, leftStrength, rightStrength);
    }

    private void SetNewRumbleCore(float timer, float leftStrength, float rightStrength)
    {
      this.timer = timer;
      GamePad.SetVibration(this.player.PlayerIndex, leftStrength, rightStrength);
      this.lastRumbleLeftStrength = leftStrength;
      this.lastRumbleRightStrength = rightStrength;
      this.isEnabled = (double) timer > 0.0;
    }

    public void Suspend()
    {
      this.isEnabled = false;
      GamePad.SetVibration(this.player.PlayerIndex, 0.0f, 0.0f);
    }

    public void Resume()
    {
      this.isEnabled = true;
    }
  }
}
