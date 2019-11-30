// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.GamerServices.GameDefaults
// Assembly: StudioForge.Engine.GamerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3EA07B8F-6C00-417B-9E82-CD1E4EB140B6
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.GamerServices.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.GamerServices
{
  public class GameDefaults
  {
    public bool AccelerateWithButtons
    {
      get
      {
        return false;
      }
    }

    public bool AutoAim
    {
      get
      {
        return false;
      }
    }

    public bool AutoCenter
    {
      get
      {
        return false;
      }
    }

    public bool BrakeWithButtons
    {
      get
      {
        return false;
      }
    }

    public ControllerSensitivity ControllerSensitivity
    {
      get
      {
        return ControllerSensitivity.Medium;
      }
    }

    public GameDifficulty GameDifficulty
    {
      get
      {
        return GameDifficulty.Normal;
      }
    }

    public bool InvertYAxis
    {
      get
      {
        return false;
      }
    }

    public bool ManualTransmission
    {
      get
      {
        return false;
      }
    }

    public bool MoveWithRightThumbStick
    {
      get
      {
        return false;
      }
    }

    public Color? PrimaryColor
    {
      get
      {
        return new Color?();
      }
    }

    public Color? SecondaryColor
    {
      get
      {
        return new Color?();
      }
    }

    public RacingCameraAngle RacingCameraAngle
    {
      get
      {
        return RacingCameraAngle.Back;
      }
    }
  }
}
