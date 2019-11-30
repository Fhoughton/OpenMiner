// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMEnvManager
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.API
{
  public interface ITMEnvManager
  {
    void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      bool transmit);

    void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      int visibility,
      bool transmit);

    void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      Color color,
      bool transmit);

    /// <summary>Add a Fog section.</summary>
    /// <param name="center">The center of the section.</param>
    /// <param name="radius">The radius of the section.</param>
    /// <param name="duration">The duration of the section in seconds.</param>
    /// <param name="transitDuration">The on/off transition duration time in seconds.</param>
    /// <param name="intensity">The intensity of the fog. Normalized. 0 = Weak. 1 = Opaque.</param>
    /// <param name="color">The color of the fog.</param>
    /// <param name="visibility">The view distance in blocks through the fog.</param>
    /// <param name="transmit">Transmit the section addition to remote gamers.</param>
    void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      int visibility,
      bool transmit);

    /// <summary>Remove a Fog section.</summary>
    /// <param name="center">The center of the section to remove.</param>
    /// <param name="transmit">Transmit the section removal to remote gamers.</param>
    void RemoveFog(GlobalPoint3D center, bool transmit);

    void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      bool transmit);

    void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      Color color,
      bool transmit);

    void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      float minSize,
      float maxSize,
      bool transmit);

    /// <summary>Add a Hail section.</summary>
    /// <param name="center">The center of the section.</param>
    /// <param name="radius">The radius of the section.</param>
    /// <param name="duration">The duration of the section in seconds.</param>
    /// <param name="transitDuration">The on/off transition duration time in seconds.</param>
    /// <param name="intensity">The intensity of the hail. Normalized. 0 = Weak. 1 = Intense.</param>
    /// <param name="color">The color of the hail.</param>
    /// <param name="minSize">The minimum size of a hail stone in meters. e.g. 0.01 = 1 cm.</param>
    /// <param name="maxSize">The maximum size of a hail stone in meters. e.g. 0.01 = 1 cm.</param>
    /// <param name="transmit">Transmit the section addition to remote gamers.</param>
    void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      float minSize,
      float maxSize,
      bool transmit);

    /// <summary>Remove a Hail section.</summary>
    /// <param name="center">The center of the section to remove.</param>
    /// <param name="transmit">Transmit the section removal to remote gamers.</param>
    void RemoveHail(GlobalPoint3D center, bool transmit);

    void AddRain(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      bool transmit);

    /// <summary>Add a Rain section.</summary>
    /// <param name="center">The center of the section.</param>
    /// <param name="radius">The radius of the section.</param>
    /// <param name="duration">The duration of the section in seconds.</param>
    /// <param name="transitDuration">The on/off transition duration time in seconds.</param>
    /// <param name="intensity">The intensity of the rain. Normalized. 0 = Weak. 1 = Strong.</param>
    /// <param name="color">The color of the rain.</param>
    /// <param name="transmit">Transmit the section addition to remote gamers.</param>
    void AddRain(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      bool transmit);

    /// <summary>Remove a Rain section.</summary>
    /// <param name="center">The center of the section to remove.</param>
    /// <param name="transmit">Transmit the section removal to remote gamers.</param>
    void RemoveRain(GlobalPoint3D center, bool transmit);
  }
}
