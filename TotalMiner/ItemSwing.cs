// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemSwing
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner
{
  internal class ItemSwing : IHasUpdate
  {
    public ItemSwing.SwingAnimData AnimData;
    public ItemSwing.SwingAnimData AnimDataFPV;
    public Bobbing Bobbing;
    private Item itemID;
    private InventoryHand handType;
    private Actor owner;
    private float swingTimer;
    private float animationTimer;
    private float fullSwingTime;
    private float extendAnimationTime;
    private float retractAnimationTime;
    private float swingTimeOverride;
    private float userSwingTimeOverride;
    private bool smoothRetract;
    private bool mayNeedSwingTypeResetOnStartSwing;
    private ItemSwing.SwingState swingState;
    private bool isLocalPlayer;
    private Vector3 swingOrigPosition;
    private Vector3 swingOrigYawPitchRoll;

    public event EventHandler SwingStart;

    public event EventHandler SwingFullyExtended;

    public event EventHandler SwingComplete;

    protected void RaiseSwingStart()
    {
      if (this.SwingStart == null)
        return;
      this.SwingStart((object) this, EventArgs.Empty);
    }

    protected void RaiseSwingFullyExtended()
    {
      if (this.SwingFullyExtended == null)
        return;
      this.SwingFullyExtended((object) this, EventArgs.Empty);
    }

    protected void RaiseSwingComplete()
    {
      if (this.SwingComplete == null)
        return;
      this.SwingComplete((object) this, EventArgs.Empty);
    }

    public bool IsEnabled { get; set; }

    public bool IsSwinging
    {
      get
      {
        return this.swingState != ItemSwing.SwingState.None;
      }
    }

    public bool IsSwingExtended
    {
      get
      {
        return this.swingState == ItemSwing.SwingState.ExtendPause;
      }
    }

    public bool HasSwingExtended
    {
      get
      {
        return this.swingState != ItemSwing.SwingState.SwingExtend;
      }
    }

    public bool IsBlock
    {
      get
      {
        return Globals1.ItemTypeData[(int) this.itemID].Use == ItemUse.Block;
      }
    }

    public float SwingProgress
    {
      get
      {
        return this.swingTimer / this.SwingTime;
      }
    }

    protected float SwingExtendedPause
    {
      get
      {
        return ItemData.GetItemSwingExtendedPauseTime(this.itemID);
      }
    }

    public float SwingTime
    {
      get
      {
        if ((double) this.userSwingTimeOverride > 0.0)
          return this.userSwingTimeOverride;
        if ((double) this.swingTimeOverride > 0.0)
          return this.swingTimeOverride;
        float num = ItemData.GetItemSwingTime(this.itemID);
        if ((double) num == 0.0)
          num = 0.3f;
        return num;
      }
    }

    public void SetUserSwingTimeOverride(float swingTime)
    {
      this.userSwingTimeOverride = swingTime;
      this.InitializeSwingTimeData();
    }

    public ItemSwing(Actor owner, InventoryHand handType)
    {
      this.owner = owner;
      this.handType = handType;
      this.isLocalPlayer = owner.IsPlayer && owner.IsLocalGamer;
      this.Bobbing = new Bobbing(owner, handType);
    }

    public void Initialize(Item itemID)
    {
      this.itemID = itemID;
      this.ResetSwingParameters();
      this.ResetSwing();
    }

    public void ResetSwing()
    {
      this.swingState = ItemSwing.SwingState.None;
      this.AnimData.CurrPosition = this.swingOrigPosition;
      this.AnimData.CurrYawPitchRoll = this.swingOrigYawPitchRoll;
      this.AnimDataFPV.CurrPosition = this.swingOrigPosition;
      this.AnimDataFPV.CurrYawPitchRoll = this.swingOrigYawPitchRoll;
      this.AnimDataFPV.PositionInterpolator.Reset();
      this.AnimDataFPV.RotationInterpolator.Reset();
      this.Bobbing.Initialize(this.IsBlock ? 0.005f : 0.01f);
    }

    private void ResetSwingParameters()
    {
      ItemSwingDataXML itemSwingData = this.GetItemSwingData();
      this.userSwingTimeOverride = 0.0f;
      this.swingTimeOverride = itemSwingData.SwingTime;
      this.swingOrigPosition = itemSwingData.RestPosition;
      this.swingOrigYawPitchRoll = itemSwingData.RestRotation;
      if (this.AnimData == null)
        this.AnimData = new ItemSwing.SwingAnimData();
      this.AnimData.CurrPosition = this.swingOrigPosition;
      this.AnimData.CurrYawPitchRoll = this.swingOrigYawPitchRoll;
      this.AnimData.ExtendedPosition = itemSwingData.ExtendedPosition;
      this.AnimData.ExtendedYawPitchRoll = itemSwingData.ExtendedRotation;
      this.AnimData.PositionInterpolator.Reset();
      this.AnimData.RotationInterpolator.Reset();
      if (this.AnimDataFPV == null)
        this.AnimDataFPV = new ItemSwing.SwingAnimData();
      this.AnimDataFPV.CurrPosition = this.swingOrigPosition;
      this.AnimDataFPV.CurrYawPitchRoll = this.swingOrigYawPitchRoll;
      this.AnimDataFPV.ExtendedPosition = itemSwingData.ExtendedPositionFPV;
      this.AnimDataFPV.ExtendedYawPitchRoll = itemSwingData.ExtendedRotationFPV;
      if ((double) this.AnimDataFPV.ExtendedPosition.X == 0.0 && (double) this.AnimDataFPV.ExtendedPosition.Y == 0.0 && (double) this.AnimDataFPV.ExtendedPosition.Z == 0.0)
        this.AnimDataFPV.ExtendedPosition = itemSwingData.ExtendedPosition;
      if ((double) this.AnimDataFPV.ExtendedYawPitchRoll.X == 0.0 && (double) this.AnimDataFPV.ExtendedYawPitchRoll.Y == 0.0 && (double) this.AnimDataFPV.ExtendedYawPitchRoll.Z == 0.0)
        this.AnimDataFPV.ExtendedYawPitchRoll = itemSwingData.ExtendedRotation;
      this.AnimDataFPV.PositionInterpolator.Reset();
      this.AnimDataFPV.RotationInterpolator.Reset();
      this.InitializeSwingTimeData();
      this.AnimData.CircularY = itemSwingData.CircularY;
      this.AnimData.CircularZ = itemSwingData.CircularZ;
      this.AnimData.TDY = (float) Math.Cos(1.57079637050629) * this.AnimData.CircularY;
      this.AnimData.TDZ = (float) Math.Sin(1.57079637050629) * this.AnimData.CircularZ;
      this.AnimDataFPV.CircularY = (double) itemSwingData.CircularYFPV == 0.0 ? itemSwingData.CircularY : itemSwingData.CircularYFPV;
      this.AnimDataFPV.CircularZ = itemSwingData.CircularZ;
      this.AnimDataFPV.TDY = (float) Math.Cos(1.57079637050629) * this.AnimDataFPV.CircularY;
      this.AnimDataFPV.TDZ = (float) Math.Sin(1.57079637050629) * this.AnimDataFPV.CircularZ;
    }

    private void InitializeSwingTimeData()
    {
      this.fullSwingTime = this.SwingTime;
      if (this.itemID == Item.Hand && !this.owner.IsPlayer)
        this.fullSwingTime = Globals1.NpcAIData[(int) Globals1.NpcTypeData[(int) this.owner.ActorType].AIType].StrikeDelay;
      float itemSwingPauseTime = ItemData.GetItemSwingPauseTime(this.itemID);
      this.smoothRetract = ItemData.GetItemSwingRetractSmooth(this.itemID);
      this.retractAnimationTime = ItemData.GetItemSwingRetractTime(this.itemID);
      if ((double) this.retractAnimationTime < 0.0)
        this.retractAnimationTime = (float) (((double) this.fullSwingTime - (double) itemSwingPauseTime - (double) this.SwingExtendedPause) * 0.5);
      this.extendAnimationTime = this.fullSwingTime - itemSwingPauseTime - this.SwingExtendedPause - this.retractAnimationTime;
    }

    private ItemSwingDataXML GetItemSwingData()
    {
      ItemSwingDataXML itemSwingDataXml = Globals1.ItemSwingData[(int) Globals1.ItemTypeData[(int) this.itemID].Swing];
      this.mayNeedSwingTypeResetOnStartSwing = false;
      if (itemSwingDataXml.SwingType == ItemSwingType.Arrow)
      {
        this.mayNeedSwingTypeResetOnStartSwing = true;
        if (this.IsPlayerHaveBowInOtherhand)
          itemSwingDataXml = Globals1.ItemSwingData[12];
      }
      return itemSwingDataXml;
    }

    private bool IsPlayerHaveBowInOtherhand
    {
      get
      {
        if (this.owner != null && this.owner.IsPlayer)
        {
          Hand hand = this.owner.GetHand(this.handType);
          if (hand != null)
            return ItemData.IsSubType(hand.OtherHand.ItemID, ItemSubType.Bow);
        }
        return false;
      }
    }

    public void StartSwing()
    {
      if (this.swingState != ItemSwing.SwingState.None)
        return;
      this.ChangeState(ItemSwing.SwingState.SwingExtend);
      this.userSwingTimeOverride = 0.0f;
    }

    public void Update(UpdateState state)
    {
      this.swingTimer += Services.ElapsedTime;
      switch (this.swingState)
      {
        case ItemSwing.SwingState.SwingExtend:
          this.UpdateSwingExtend();
          break;
        case ItemSwing.SwingState.ExtendPause:
          this.UpdateExtendPause();
          break;
        case ItemSwing.SwingState.SwingRetract:
          this.UpdateSwingRetract();
          break;
        case ItemSwing.SwingState.EndSwingPause:
          this.UpdateEndSwingPause();
          break;
        default:
          this.Bobbing.Update(state);
          Player owner = this.owner as Player;
          if (owner != null && owner.Settings.Bobbing)
          {
            this.AnimDataFPV.CurrPosition = this.swingOrigPosition + this.Bobbing.Position;
            break;
          }
          break;
      }
      if (this.swingState == ItemSwing.SwingState.None)
        return;
      this.Bobbing.Reset();
    }

    private void UpdateSwingExtend()
    {
      this.UpdateAnimation();
      if (this.AnimData.PositionInterpolator.IsActive)
        return;
      this.ChangeState(ItemSwing.SwingState.ExtendPause);
    }

    private void UpdateExtendPause()
    {
      if ((double) this.swingTimer < (double) (this.extendAnimationTime + this.SwingExtendedPause))
        return;
      this.ChangeState(ItemSwing.SwingState.SwingRetract);
    }

    private void UpdateSwingRetract()
    {
      this.UpdateAnimation();
      if (this.AnimData.PositionInterpolator.IsActive)
        return;
      this.ChangeState(ItemSwing.SwingState.EndSwingPause);
    }

    private void UpdateEndSwingPause()
    {
      if ((double) this.swingTimer < (double) this.fullSwingTime)
        return;
      this.ChangeState(ItemSwing.SwingState.None);
    }

    private void UpdateAnimation()
    {
      this.animationTimer += Services.ElapsedTime;
      float num = (float) ((double) this.animationTimer / ((double) this.extendAnimationTime + (double) this.retractAnimationTime) * 6.28318548202515 + 1.57079637050629);
      float dy = (float) Math.Cos((double) num);
      float dz = (float) Math.Sin((double) num);
      this.UpdateAnimData(this.AnimData, dy, dz);
      this.UpdateAnimData(this.AnimDataFPV, dy, dz);
    }

    private void UpdateAnimData(ItemSwing.SwingAnimData data, float dy, float dz)
    {
      data.PositionInterpolator.Update();
      data.RotationInterpolator.Update();
      data.CurrPosition = data.PositionInterpolator.CurrentValue;
      data.CurrPosition.Y = data.CurrPosition.Y - dy * data.CircularY - data.TDY;
      data.CurrPosition.Z = data.CurrPosition.Z + dz * data.CircularZ - data.TDZ;
      data.CurrYawPitchRoll = data.RotationInterpolator.CurrentValue;
    }

    private void ChangeState(ItemSwing.SwingState newState)
    {
      switch (newState)
      {
        case ItemSwing.SwingState.None:
          this.AnimData.CurrPosition = this.swingOrigPosition;
          this.AnimDataFPV.CurrPosition = this.swingOrigPosition;
          this.RaiseSwingComplete();
          break;
        case ItemSwing.SwingState.SwingExtend:
          if (this.mayNeedSwingTypeResetOnStartSwing)
            this.ResetSwingParameters();
          this.swingTimer = 0.0f;
          this.animationTimer = 0.0f;
          this.AnimData.PositionInterpolator.Start(this.AnimData.CurrPosition, this.AnimData.ExtendedPosition, (double) this.extendAnimationTime, false);
          this.AnimData.RotationInterpolator.Start(this.swingOrigYawPitchRoll, this.AnimData.ExtendedYawPitchRoll, (double) this.extendAnimationTime, true);
          this.AnimDataFPV.PositionInterpolator.Start(this.AnimDataFPV.CurrPosition, this.AnimDataFPV.ExtendedPosition, (double) this.extendAnimationTime, false);
          this.AnimDataFPV.RotationInterpolator.Start(this.swingOrigYawPitchRoll, this.AnimDataFPV.ExtendedYawPitchRoll, (double) this.extendAnimationTime, true);
          this.RaiseSwingStart();
          break;
        case ItemSwing.SwingState.ExtendPause:
          this.RaiseSwingFullyExtended();
          break;
        case ItemSwing.SwingState.SwingRetract:
          this.AnimData.PositionInterpolator.Start(this.AnimData.ExtendedPosition, this.swingOrigPosition, (double) this.retractAnimationTime, this.smoothRetract);
          this.AnimData.RotationInterpolator.Start(this.AnimData.ExtendedYawPitchRoll, this.swingOrigYawPitchRoll, (double) this.retractAnimationTime, this.smoothRetract);
          this.AnimDataFPV.PositionInterpolator.Start(this.AnimDataFPV.ExtendedPosition, this.swingOrigPosition, (double) this.retractAnimationTime, this.smoothRetract);
          this.AnimDataFPV.RotationInterpolator.Start(this.AnimDataFPV.ExtendedYawPitchRoll, this.swingOrigYawPitchRoll, (double) this.retractAnimationTime, this.smoothRetract);
          break;
        case ItemSwing.SwingState.EndSwingPause:
          this.AnimData.CurrPosition = this.swingOrigPosition;
          this.AnimDataFPV.CurrPosition = this.swingOrigPosition;
          break;
      }
      this.swingState = newState;
    }

    private enum SwingState
    {
      None,
      SwingExtend,
      ExtendPause,
      SwingRetract,
      EndSwingPause,
    }

    public class SwingAnimData
    {
      public Vec3Interpolator PositionInterpolator = new Vec3Interpolator();
      public Vec3Interpolator RotationInterpolator = new Vec3Interpolator();
      public Vector3 CurrPosition;
      public Vector3 CurrYawPitchRoll;
      public Vector3 ExtendedPosition;
      public Vector3 ExtendedYawPitchRoll;
      public float CircularY;
      public float CircularZ;
      public float TDY;
      public float TDZ;
    }
  }
}
