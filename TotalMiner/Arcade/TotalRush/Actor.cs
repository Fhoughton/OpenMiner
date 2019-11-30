// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Arcade.TotalRush.Actor
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Arcade.TotalRush
{
  internal class Actor
  {
    public ActorType ActorType;
    public bool IsAlive;
    public bool IsDestroyed;
    public bool IsVulnerable;
    public Actor Parent;
    public int ChildCount;
    public float Scale;
    public Vector2 Position;
    public Vector2 WorldPosition;
    public Vector2 Speed;
    public Vector2 Velocity;
    public Vector2 Velocity2;
    public Vector2 Aim;
    public Vector2 OriginalPosition;
    public Vector2 TargetPosition;
    public float Rotation;
    public float RotationSpeed;
    public float TotalTargetTravelTime;
    public float CurrentTargetTravelTime;
    public float DestroyedAge;
    public int Hitpoints;
    public int MaxPosY;
    public Vector2 HitBoxCenter;
    public Vector2 HitBoxOffset;
    public float HitBoxRadius;
    public float HitBoxRadius2;
    public ActorType DropType;
    public int DropChance;
    public Action<Actor> DestroyedHandler;
    public int ExhaustTimer;
    public int TailShipCount;
    public int TailShipID;
    public CustomArray<BulletPattern> BulletPatterns;
    public int BulletsFired;
    public int Score;
  }
}
