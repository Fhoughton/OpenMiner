// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Sounds
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.TotalMiner.API;
using System;

namespace StudioForge.TotalMiner
{
  public static class Sounds
  {
    private static ITMGame instance;

    /// <summary>Implementation detail. Consumers should not call.</summary>
    /// <param name="game">Game Instance</param>
    public static void Initialize(ITMGame game)
    {
      Sounds.instance = game;
    }

    private static SoundType GetSoundType(ItemSoundType type)
    {
      switch (type)
      {
        case ItemSoundType.Step:
          return SoundType.Footprint;
        case ItemSoundType.Mine:
        case ItemSoundType.Dig:
        case ItemSoundType.Chop:
          return SoundType.Mining;
        case ItemSoundType.Hit:
          return SoundType.Interact;
        default:
          return SoundType.None;
      }
    }

    public static bool PlaySound(ItemSoundGroup groupID)
    {
      return Sounds.PlaySound(groupID, ItemSoundType.Use);
    }

    public static bool PlaySound(Item itemID, ItemSoundType type)
    {
      return Sounds.PlaySound(itemID, type, false);
    }

    public static bool PlaySound(ItemSoundGroup groupID, ItemSoundType type)
    {
      return Sounds.PlaySound(Sounds.GetGroupSound(groupID, type), (AudioEmitter) null, (ITMActor) null, SoundType.None);
    }

    public static bool PlaySound(ItemSoundGroup groupID, ItemSoundType type, GlobalPoint3D p)
    {
      return Sounds.PlaySound(groupID, type, Sounds.instance.World.Map.GetBlockCenter(p));
    }

    public static bool PlaySound(ItemSoundGroup groupID, ItemSoundType type, Vector3 pos)
    {
      return Sounds.PlaySound(Sounds.GetGroupSound(groupID, type), pos, (ITMActor) null, Sounds.GetSoundType(type));
    }

    public static bool PlaySound(ItemSoundGroup groupID, ItemSoundType type, AudioEmitter emitter)
    {
      return Sounds.PlaySound(Sounds.GetGroupSound(groupID, type), emitter, (ITMActor) null, Sounds.GetSoundType(type));
    }

    public static bool PlaySound(
      ItemSoundGroup groupID,
      ItemSoundType type,
      ITMActor emitter,
      bool broadcast)
    {
      return Sounds.PlaySound(Sounds.GetGroupSound(groupID, type), emitter.AudioEmitter, broadcast ? emitter : (ITMActor) null, broadcast ? Sounds.GetSoundType(type) : SoundType.None);
    }

    public static bool PlaySound(
      Item itemID,
      ItemSoundType type,
      GlobalPoint3D p,
      ITMActor broadcaster)
    {
      return Sounds.PlaySound(itemID, type, Sounds.instance.World.Map.GetBlockCenter(p), broadcaster);
    }

    public static bool PlaySound(
      Item itemID,
      ItemSoundType type,
      Vector3 pos,
      ITMActor broadcaster)
    {
      return Sounds.PlaySound(Sounds.GetItemSound(itemID, type, false), pos, broadcaster, Sounds.GetSoundType(type));
    }

    public static bool PlaySound(
      Item itemID,
      ItemSoundType type,
      ITMActor emitter,
      bool broadcast)
    {
      return Sounds.PlaySound(itemID, type, emitter.AudioEmitter, broadcast ? emitter : (ITMActor) null);
    }

    public static bool PlaySound(Item itemID, ITMActor emitter, bool broadcast)
    {
      return Sounds.PlaySound(itemID, emitter.AudioEmitter, broadcast ? emitter : (ITMActor) null);
    }

    public static bool PlaySound(Item itemID, ItemSoundType type, bool exactSound)
    {
      return Sounds.PlaySound(Sounds.GetItemSound(itemID, type, exactSound), (AudioEmitter) null, (ITMActor) null, SoundType.None);
    }

    public static bool PlaySound(Item itemID, AudioEmitter emitter, ITMActor broadcaster)
    {
      return Sounds.PlaySound(itemID, ItemSoundType.Use, emitter, broadcaster);
    }

    public static bool PlaySound(Item itemID, ItemSoundType type, ITMActor emitter)
    {
      return Sounds.PlaySound(Sounds.GetItemSound(itemID, type, false), emitter.AudioEmitter, emitter, Sounds.GetSoundType(type));
    }

    public static bool PlaySound(
      Item itemID,
      ItemSoundType type,
      AudioEmitter emitter,
      ITMActor broadcaster)
    {
      return Sounds.PlaySound(Sounds.GetItemSound(itemID, type, false), emitter, broadcaster, Sounds.GetSoundType(type));
    }

    public static bool PlaySound(ITMActor c, ActorSoundType type)
    {
      if (c != null && c.ActorType != ActorType.None)
      {
        string key = (string) null;
        ActorAudioDataXML actorAudioDataXml = Globals1.NpcAudioData[(int) c.ActorType];
        switch (type)
        {
          case ActorSoundType.Pain:
            key = actorAudioDataXml.AudioPain;
            break;
          case ActorSoundType.Strike:
            key = actorAudioDataXml.AudioStrike;
            break;
          case ActorSoundType.Warning:
            key = actorAudioDataXml.AudioWarning;
            break;
          case ActorSoundType.Death:
            key = actorAudioDataXml.AudioDeath;
            break;
        }
        if (key != null)
          return Sounds.PlaySoundCore(key, c.AudioEmitter, c, SoundType.Spoken);
      }
      return false;
    }

    public static bool PlaySoundDirectKey(
      string key,
      AudioEmitter emitter,
      ITMActor broadcaster,
      SoundType soundType)
    {
      return Sounds.PlaySoundCore(key, emitter, broadcaster, soundType);
    }

    private static bool PlaySound(
      string[] keys,
      Vector3 pos,
      ITMActor broadcaster,
      SoundType soundType)
    {
      AudioEmitter emitter;
      if (broadcaster != null)
      {
        emitter = broadcaster.AudioEmitter;
        emitter.Forward = Vector3.Normalize(broadcaster.Position - pos);
      }
      else
        emitter = new AudioEmitter()
        {
          DopplerScale = 1f,
          Position = pos,
          Up = Vector3.Up,
          Forward = Vector3.Up
        };
      return Sounds.PlaySound(keys, emitter, broadcaster, soundType);
    }

    private static bool PlaySound(
      string[] keys,
      AudioEmitter emitter,
      ITMActor broadcaster,
      SoundType soundType)
    {
      if (keys != null && keys.Length > 0)
        return Sounds.PlaySoundCore(keys[Sounds.instance != null ? Sounds.instance.Random.Next(keys.Length) : 0], emitter, broadcaster, soundType);
      return false;
    }

    private static bool PlaySoundCore(
      string key,
      AudioEmitter emitter,
      ITMActor broadcaster,
      SoundType soundType)
    {
      bool flag = false;
      if (CoreGlobals.AudioManager != null && CoreGlobals.Game.IsActive && key != null)
      {
        if (key.Length > 0)
        {
          try
          {
            if (emitter != null)
            {
              AudioListener closestListener = Sounds.instance.World.GetClosestListener(emitter.Position);
              if (closestListener != null)
                flag = CoreGlobals.AudioManager.PlaySound(key, emitter, closestListener);
              else
                broadcaster = (ITMActor) null;
            }
            else
              flag = CoreGlobals.AudioManager.PlaySound(key);
            if (soundType != SoundType.None)
              Sounds.instance.World.BroadcastSound(emitter.Position, broadcaster, soundType);
          }
          catch (Exception ex)
          {
          }
        }
      }
      return flag;
    }

    private static string[] GetItemSound(Item itemID, ItemSoundType type, bool exact)
    {
      string[] strArray = Sounds.GetSound(Globals1.ItemSoundData[(int) itemID].Sounds, type);
      if (strArray == null && !exact)
        strArray = Sounds.GetGroupSound(itemID, type);
      return strArray;
    }

    private static string[] GetGroupSound(Item itemID, ItemSoundType type)
    {
      return Sounds.GetGroupSound(Globals1.ItemSoundData[(int) itemID].Group, type);
    }

    private static string[] GetGroupSound(ItemSoundGroup groupID, ItemSoundType type)
    {
      for (; groupID != ItemSoundGroup.None; groupID = Globals1.ItemSoundGroups[(int) groupID].Parent)
      {
        string[] sound = Sounds.GetSound(Globals1.ItemSoundGroups[(int) groupID].Sounds, type);
        if (sound != null)
          return sound;
      }
      return (string[]) null;
    }

    private static string[] GetSound(ItemSoundXML sound, ItemSoundType type)
    {
      string[] strArray = (string[]) null;
      switch (type)
      {
        case ItemSoundType.Step:
          return sound.Step;
        case ItemSoundType.Dig:
          strArray = sound.Dig;
          type = ItemSoundType.Mine;
          break;
        case ItemSoundType.Chop:
          strArray = sound.Chop;
          type = ItemSoundType.Mine;
          break;
        case ItemSoundType.Hit:
          strArray = sound.Hit;
          type = ItemSoundType.Use;
          break;
      }
      if (strArray == null)
      {
        switch (type)
        {
          case ItemSoundType.Mine:
            strArray = sound.Mine;
            break;
          case ItemSoundType.Use:
            strArray = sound.Use;
            break;
          case ItemSoundType.UseFail:
            strArray = sound.UseFail;
            break;
        }
      }
      return strArray;
    }
  }
}
