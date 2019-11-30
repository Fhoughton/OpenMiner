// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ParticleModifiers
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner
{
  internal class ParticleModifiers
  {
    private float SlowGravity = GameInstance.SlowGravity * 40f;
    private float Gravity = GameInstance.Gravity * 40f;
    private float bounce = 0.6f;
    private Vector3 minPickupVel = new Vector3(-45f);
    private Vector3 maxPickupVel = new Vector3(45f);
    private float pickupToEyeOffset = -0.2f;
    private ParticleModifiers.CollisionData data = new ParticleModifiers.CollisionData();
    private Map map;
    private GameInstance instance;
    public ItemParticleModifier BlockPickupParticleModifier;
    public ItemParticleModifier BlueprintPickupParticleModifier;
    public ItemParticleModifier MiningParticleModifier;
    public ItemParticleModifier GrenadeParticleModifier;
    public ItemParticleModifier ProjectileParticleModifier;
    public ItemParticleModifier SliderParticleModifier;
    public ItemParticleModifier LavaParticleModifier;
    public ItemParticleModifier ModelExplodeParticleModifier;
    public BlockParticleModifier ItemCrumbleParticleModifier;

    public ParticleModifiers(GameInstance instance, Map map)
    {
      this.instance = instance;
      this.map = map;
      this.BlockPickupParticleModifier = new ItemParticleModifier(this.BlockPickupParticleModifierCore);
      this.BlueprintPickupParticleModifier = new ItemParticleModifier(this.BlueprintPickupParticleModifierCore);
      this.MiningParticleModifier = new ItemParticleModifier(this.MiningParticleModifierCore);
      this.GrenadeParticleModifier = new ItemParticleModifier(this.GrenadeParticleModifierCore);
      this.ProjectileParticleModifier = new ItemParticleModifier(this.ProjectileParticleModifierCore);
      this.SliderParticleModifier = new ItemParticleModifier(this.SliderParticleModifierCore);
      this.LavaParticleModifier = new ItemParticleModifier(this.LavaParticleModifierCore);
      this.ItemCrumbleParticleModifier = new BlockParticleModifier(this.ItemCrumbleParticleModifierCore);
      this.ModelExplodeParticleModifier = new ItemParticleModifier(this.ModelExplodeParticleModifierCore);
    }

    private bool BlockPickupParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      bool flag = false;
      Vector3 vector3_1 = new Vector3();
      if ((double) particle.MinPickupAge < 0.0)
      {
        foreach (Actor moveableCharacter in this.instance.AllMoveableCharacters)
        {
          if (!moveableCharacter.IsDeadOrInactiveOrDisabled && moveableCharacter.HasPermission(Permissions.Adventure, false) && moveableCharacter.Properties.CanPickup.Value)
          {
            vector3_1.X = moveableCharacter.EyePosition.X;
            vector3_1.Y = moveableCharacter.EyePosition.Y + this.pickupToEyeOffset;
            vector3_1.Z = moveableCharacter.EyePosition.Z;
            float num = Vector3.DistanceSquared(vector3_1, particle.Position);
            if ((double) num < 4.0 && moveableCharacter.HasRoomForPickup(particle.Item))
            {
              flag = true;
              if ((double) num < 1.0)
                return !moveableCharacter.PickupItem(particle.Item, particle.ParticleID);
              Vector3 vector3_2 = vector3_1 - particle.Position;
              vector3_2.Normalize();
              particle.Velocity += vector3_2 * 1.1f;
              particle.Velocity = Vector3.Clamp(particle.Velocity, this.minPickupVel, this.maxPickupVel);
            }
          }
        }
      }
      if (!flag)
      {
        GlobalPoint3D point = this.map.GetPoint(particle.Position);
        if (!this.map.IsBlockPassable(this.map.GetBlockID(point + GlobalPoint3D.Down)))
        {
          Vector3 position = this.map.GetPosition(point);
          particle.Velocity.X *= 0.95f;
          particle.Velocity.Z *= 0.95f;
          if ((double) particle.Position.Y < (double) position.Y - (double) this.map.TileSize * 0.800000011920929)
          {
            particle.Position.Y = position.Y - this.map.TileSize * 0.8f;
            particle.Velocity.Y = (float) (-(double) this.SlowGravity * 0.5);
          }
          else if ((double) particle.Position.Y < (double) position.Y - (double) this.map.TileSize * 0.5)
            particle.Velocity.Y -= this.SlowGravity * 0.25f;
          else
            particle.Velocity.Y += this.SlowGravity * 0.25f;
        }
        else
          particle.Velocity.Y += this.SlowGravity;
        Vector3 liquidFlowDirection = this.map.GetLiquidFlowDirection(point);
        particle.Velocity.X += liquidFlowDirection.X;
        particle.Velocity.Z += liquidFlowDirection.Z;
        particle.Rotation += 3f * elapsed;
      }
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      this.CheckParticleCollisionAgainstWorld_NoYCheck(ref particle);
      return true;
    }

    private bool BlueprintPickupParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      if (!this.BlockPickupParticleModifierCore(ref particle, elapsed))
        return false;
      int durability = (int) particle.Item.Durability;
      if (durability < Blueprints.BlueprintList.Length)
        Blueprints.BlueprintList[durability].Point = this.map.GetPoint(particle.Position);
      return true;
    }

    private bool TorchLightParticleModifierCore(ref BlockParticle particle, float elapsed)
    {
      particle.Velocity.Y -= this.SlowGravity * 0.1f;
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      return true;
    }

    private bool MiningParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.SlowGravity;
      if ((double) particle.Velocity.LengthSquared() < 0.0399999991059303)
        return false;
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      return this.CheckParticleCollisionAgainstWorld(ref particle);
    }

    private bool GrenadeParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.SlowGravity;
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      particle.Rotation += 10f * elapsed;
      GlobalPoint3D point = this.map.GetPoint(particle.Position);
      Block blockId = (Block) this.map.GetBlockID(point);
      if (blockId == (Block) this.map.OutOfBoundsBlockID)
        return (double) particle.Velocity.Y > 0.0;
      if (this.CreateBlastOnBlockImpact(ref particle, blockId, 24, 5))
        return false;
      BoundingBox box = new BoundingBox();
      Vector3 position1 = particle.Position;
      position1.X -= particle.Radius;
      position1.Y -= particle.Radius;
      position1.Z -= particle.Radius;
      box.Min = position1;
      Vector3 position2 = particle.Position;
      position2.X += particle.Radius;
      position2.Y += particle.Radius;
      position2.Z += particle.Radius;
      box.Max = position2;
      HitTarget firstHitTarget = this.instance.GetFirstHitTarget(box, HitTargetOptions.All, false);
      if (firstHitTarget.Target == null || !(firstHitTarget.Target.GamerID != particle.PlayerID))
        return true;
      if (firstHitTarget.Target.IsEnabledField && firstHitTarget.Target.IsLocalGamer)
        this.instance.CreateBlast(point, Item.Grenade, 24f, 5, particle.PlayerID);
      return false;
    }

    private bool CreateBlastOnBlockImpact(
      ref ItemParticle particle,
      Block block,
      int strength,
      int radius)
    {
      Block block1 = block;
      if ((uint) block1 <= 53U)
      {
        if (block1 == Block.None || block1 == Block.Water || block1 == Block.Teleport)
          goto label_6;
      }
      else if (block1 == Block.Rope || block1 == Block.Fire || block1 == Block.zLastBlockID)
        goto label_6;
      if (this.map.IsHost)
        this.instance.CreateBlast(this.map.GetPoint(particle.Position), Item.Grenade, (float) strength, radius, particle.PlayerID);
      return true;
label_6:
      return false;
    }

    private bool ProjectileParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.SlowGravity;
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      if ((double) particle.Position.Y >= (double) this.map.MapBound.Max.Y * (double) this.map.TileSize)
        return true;
      GlobalPoint3D point = this.map.GetPoint(particle.Position);
      Block blockId = (Block) this.map.GetBlockID(point);
      if (blockId == (Block) this.map.OutOfBoundsBlockID)
        return (double) particle.Velocity.Y > 0.0;
      if (particle.Item.ItemID == Item.BoomArrow && this.CreateBlastOnBlockImpact(ref particle, blockId, 10, 2))
        return false;
      Player player = (Player) null;
      if (!this.map.IsBlockPassable((byte) blockId) && !this.map.IsBlockIcon((byte) blockId) && this.instance.GetBlockBox(point, blockId).Contains(particle.Position) != ContainmentType.Disjoint)
      {
        bool flag = false;
        int num = 4;
        Item textureIdForDrawing = (Item) this.instance.Map.GetBlockTextureIDForDrawing(blockId, point);
        Sounds.PlaySound(textureIdForDrawing, ItemSoundType.Mine, point, (ITMActor) null);
        if (Globals1.BlockData[(int) blockId].Buffer == (byte) 3)
        {
          switch (blockId)
          {
            case Block.Leaves:
            case Block.PineLeaves:
            case Block.SteelPortcullis:
            case Block.MapleLeaves:
              flag = true;
              break;
            case Block.Glass:
            case Block.StainedGlassPane:
            case Block.OneWayGlass:
            case Block.StainedGlass:
              if (player == null)
                player = this.instance.GetPlayer(particle.PlayerID);
              if ((player == null || player.HasPermission(Permissions.Edit)) && this.instance.ClearBlock(point, player != null ? UpdateBlockMethod.Player : UpdateBlockMethod.Strategy, particle.PlayerID, false))
              {
                this.map.Commit();
                Sounds.PlaySound(textureIdForDrawing, ItemSoundType.Mine, point, (ITMActor) null);
                break;
              }
              break;
          }
        }
        else
          flag = true;
        if (flag)
        {
          particle.Age = 10f;
          particle.Modifier = new ItemParticleModifier(this.ProjectileParticleStoppedModifierCore);
          this.MoveStoppedProjectileParticlePositionBackToBlockFace(ref particle);
          this.instance.ParticleManager.StickParticle(ref particle, this.map.GetGlobalHashCode(point));
          switch (blockId)
          {
            case Block.HealthBlock:
              HealthBlock dataBlock = ((MapStrategyTM) this.map.MapStrategy).GetDataBlock(point) as HealthBlock;
              if (dataBlock != null)
              {
                if (player == null)
                  player = this.instance.GetPlayer(particle.PlayerID);
                dataBlock.Struck(this.instance, (Actor) player, SkillType.Ranged, particle.Item.ItemID);
                num = 0;
                break;
              }
              break;
            case Block.ScriptBlock:
              this.instance.SetPower(point, true, particle.PlayerID);
              particle.Type |= ParticleType.SetPower;
              break;
          }
        }
        for (int index = 0; index < num; ++index)
          this.instance.AddMiningParticle(point);
        GlobalPoint3D pp;
        if (this.map.IsHost && particle.Item.ItemID == Item.FireArrow && (ItemData2.GetBurnTime((MapTM) this.map, point, (Item) blockId) > (ushort) 0 && this.GetPrecendingBlock(this.map, point, particle.Velocity, out pp) == Block.None))
          this.instance.StartLiveFire(point, blockId, pp, UpdateBlockMethod.Player, particle.PlayerID, true);
        return flag;
      }
      HitTarget firstHitTarget = this.instance.GetFirstHitTarget(particle.Box, HitTargetOptions.All, false);
      if (firstHitTarget.Target != null)
      {
        if (player == null)
          player = this.instance.GetPlayer(particle.PlayerID);
        if (firstHitTarget.Target != player)
        {
          if (firstHitTarget.Target.IsEnabledField && !particle.CameFromRemote)
          {
            TargetingSystem.Target((INPCBehaviour) player, (INPCBehaviour) firstHitTarget.Target);
            Vector3.Normalize(particle.Velocity);
            if (firstHitTarget.Target.Struck((Actor) player, SkillType.Ranged, particle.Item.ItemID, firstHitTarget.IsCriticalHit) && particle.Item.ItemID == Item.BoomArrow)
              this.instance.CreateBlast(point, Item.Grenade, 10f, 2, particle.PlayerID);
          }
          return false;
        }
      }
      return true;
    }

    private bool ProjectileParticleStoppedModifierCore(ref ItemParticle particle, float elapsed)
    {
      return true;
    }

    private Block GetPrecendingBlock(
      Map map,
      GlobalPoint3D p,
      Vector3 velocity,
      out GlobalPoint3D pp)
    {
      velocity.Normalize();
      velocity.X *= map.TileSize * 0.75f;
      velocity.Y *= map.TileSize * 0.75f;
      velocity.Z *= map.TileSize * 0.75f;
      pp = map.GetPoint(map.GetBlockCenter(p) - velocity);
      return (Block) map.GetBlockID(pp);
    }

    private void MoveStoppedProjectileParticlePositionBackToBlockFace(ref ItemParticle particle)
    {
      Vector3 vector3_1 = particle.Velocity * 0.1f * Services.ElapsedTime;
      Vector3 vector3_2 = Vector3.Normalize(particle.Velocity) * (particle.Radius * 0.5f);
      GlobalPoint3D point = this.map.GetPoint(particle.Position + vector3_2);
      for (GlobalPoint3D p = point; (p == point || !this.map.IsPassable(p)) && this.map.IsValidPoint(p); p = this.map.GetPoint(particle.Position + vector3_2))
        particle.Position -= vector3_1;
    }

    private bool SliderParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.SlowGravity;
      particle.Position += particle.Velocity * elapsed;
      Vector3 position = particle.Position;
      position.Y -= this.map.TileSize;
      GlobalPoint3D point = this.map.GetPoint(position);
      if (point.Y < this.map.MapBound.Min.Y)
      {
        point.Y = this.map.MapBound.Min.Y;
      }
      else
      {
        HitTarget firstHitTarget = this.instance.GetFirstHitTarget(particle.Box, HitTargetOptions.PlayersAndNpcs, true);
        if (firstHitTarget.Target != null)
        {
          double damageAndDisplay = (double) firstHitTarget.Target.TakeDamageAndDisplay(DamageType.BlockFallingOnHead, -particle.Velocity.Y, Vector3.Zero);
          this.instance.AddMiningParticles(point, (Block) particle.Item.ItemID);
          return false;
        }
      }
      byte blockId = this.map.GetBlockID(point);
      if (blockId <= (byte) 0)
        return true;
      if (this.map.IsBlockPassable(blockId) || this.instance.Random.Next(5) == 0)
      {
        this.instance.AddMiningParticles(point, (Block) particle.Item.ItemID);
      }
      else
      {
        ++point.Y;
        this.map.SetBlockData(point, (byte) particle.Item.ItemID, (byte) ((uint) particle.TextureIndex << 4), UpdateBlockMethod.Strategy, GamerID.Sys1, false);
        this.map.Commit();
      }
      return false;
    }

    private bool LavaParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.SlowGravity;
      particle.Position += particle.Velocity * elapsed;
      if ((double) particle.Position.Y < (double) this.map.MapBound.Min.Y * (double) this.map.TileSize)
        return false;
      BoundingBox box = new BoundingBox();
      Vector3 position1 = particle.Position;
      position1.X -= particle.Radius;
      position1.Y -= particle.Radius;
      position1.Z -= particle.Radius;
      box.Min = position1;
      Vector3 position2 = particle.Position;
      position2.X += particle.Radius;
      position2.Y += particle.Radius;
      position2.Z += particle.Radius;
      box.Max = position2;
      HitTarget firstHitTarget = this.instance.GetFirstHitTarget(box, HitTargetOptions.All, true);
      if (firstHitTarget.Target != null)
      {
        double damageAndDisplay = (double) firstHitTarget.Target.TakeDamageAndDisplay(DamageType.Burning, 5f, particle.Velocity * 0.1f);
        return false;
      }
      if ((double) particle.Velocity.Y > 0.0)
        return true;
      return this.CheckParticleCollisionAgainstWorld(ref particle);
    }

    private bool ItemCrumbleParticleModifierCore(ref BlockParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.SlowGravity * 0.5f;
      if ((double) particle.Velocity.LengthSquared() < 0.0399999991059303)
        return false;
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      return this.CheckParticleCollisionAgainstWorld(ref particle);
    }

    private bool ModelExplodeParticleModifierCore(ref ItemParticle particle, float elapsed)
    {
      particle.Velocity.Y += this.Gravity * 0.5f;
      particle.Position.X += particle.Velocity.X * elapsed;
      particle.Position.Y += particle.Velocity.Y * elapsed;
      particle.Position.Z += particle.Velocity.Z * elapsed;
      return this.CheckParticleCollisionAgainstWorld(ref particle);
    }

    private bool CheckParticleCollisionAgainstWorld(ref ItemParticle particle)
    {
      Vector3 position = particle.Position;
      if (!this.map.IsValidPoint(this.map.GetPoint(position)))
        return false;
      if ((double) particle.Velocity.Y < 0.0)
      {
        position.Y -= particle.Radius;
        GlobalPoint3D point = this.map.GetPoint(position);
        if (this.map.IsValidPoint(point) && !this.map.IsBlockPassable(this.map.GetBlockID(point)))
        {
          particle.Position.Y = (float) point.Y * this.map.TileSize + this.map.TileSize + particle.Radius;
          particle.Velocity.Y = (float) (-(double) particle.Velocity.Y * 0.300000011920929);
          particle.Velocity.X *= 0.9f;
          particle.Velocity.Z *= 0.9f;
        }
      }
      else if ((double) particle.Velocity.Y > 0.0)
      {
        position.Y += particle.Radius;
        GlobalPoint3D point = this.map.GetPoint(position);
        if (this.map.IsValidPoint(point) && !this.map.IsBlockPassable(this.map.GetBlockID(point)))
          particle.Velocity.Y = 0.0f;
      }
      this.CheckParticleCollisionAgainstWorld_NoYCheck(ref particle);
      return true;
    }

    private void CheckParticleCollisionAgainstWorld_NoYCheck(ref ItemParticle particle)
    {
      Vector3 position = particle.Position;
      Vector3 vector3_1 = this.map.MapBound.Max.ToVector3();
      if ((double) position.Y >= (double) vector3_1.Y)
        return;
      Vector3 vector3_2 = this.map.MapBound.Min.ToVector3();
      if ((double) particle.Velocity.X < 0.0)
      {
        position.X -= particle.Radius;
        if ((double) position.X < (double) vector3_2.X || !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.X = -particle.Velocity.X * this.bounce;
        position.X += particle.Radius;
      }
      else if ((double) particle.Velocity.X > 0.0)
      {
        position.X += particle.Radius;
        if ((double) position.X >= (double) vector3_1.X || !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.X = -particle.Velocity.X * this.bounce;
        position.X -= particle.Radius;
      }
      if ((double) particle.Velocity.Z < 0.0)
      {
        position.Z -= particle.Radius;
        if ((double) position.Z < (double) vector3_2.Z || !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.Z = -particle.Velocity.Z * this.bounce;
        position.Z += particle.Radius;
      }
      else
      {
        if ((double) particle.Velocity.Z <= 0.0)
          return;
        position.Z += particle.Radius;
        if ((double) position.Z < (double) vector3_1.Z && this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          return;
        particle.Velocity.Z = -particle.Velocity.Z * this.bounce;
      }
    }

    private bool CheckParticleCollisionAgainstWorld(ref BlockParticle particle)
    {
      Vector3 position = particle.Position;
      Vector3 vector3_1 = this.map.MapBound.Max.ToVector3();
      if ((double) position.Y >= (double) vector3_1.Y)
        return true;
      Vector3 vector3_2 = this.map.MapBound.Min.ToVector3();
      if ((double) position.Y < (double) vector3_2.Y || (double) position.X < (double) vector3_2.X || ((double) position.X >= (double) vector3_1.X || (double) position.Z < (double) vector3_2.Z) || (double) position.Z >= (double) vector3_1.Z)
        return false;
      if ((double) particle.Velocity.Y < 0.0)
      {
        position.Y -= particle.Radius;
        GlobalPoint3D point = this.map.GetPoint(position);
        if (!this.map.IsBlockPassable(this.map.GetBlockID(point)))
        {
          particle.Position.Y = (float) point.Y * this.map.TileSize + this.map.TileSize + particle.Radius;
          particle.Velocity.Y = (float) (-(double) particle.Velocity.Y * 0.300000011920929);
          particle.Velocity.X *= 0.9f;
          particle.Velocity.Z *= 0.9f;
        }
      }
      else if ((double) particle.Velocity.Y > 0.0)
      {
        position.Y += particle.Radius;
        if (this.map.IsValidPoint(this.map.GetPoint(position)) && !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.Y = 0.0f;
      }
      this.CheckParticleCollisionAgainstWorld_NoYCheck(ref particle);
      return true;
    }

    private void CheckParticleCollisionAgainstWorld_NoYCheck(ref BlockParticle particle)
    {
      Vector3 position = particle.Position;
      Vector3 vector3_1 = this.map.MapBound.Max.ToVector3();
      if ((double) position.Y >= (double) vector3_1.Y)
        return;
      Vector3 vector3_2 = this.map.MapBound.Min.ToVector3();
      if ((double) particle.Velocity.X < 0.0)
      {
        position.X -= particle.Radius;
        if ((double) position.X < (double) vector3_2.X || !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.X = -particle.Velocity.X * this.bounce;
        position.X += particle.Radius;
      }
      else if ((double) particle.Velocity.X > 0.0)
      {
        position.X += particle.Radius;
        if ((double) position.X >= (double) vector3_1.X || !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.X = -particle.Velocity.X * this.bounce;
        position.X -= particle.Radius;
      }
      if ((double) particle.Velocity.Z < 0.0)
      {
        position.Z -= particle.Radius;
        if ((double) position.Z < (double) vector3_2.Z || !this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          particle.Velocity.Z = -particle.Velocity.Z * this.bounce;
        position.Z += particle.Radius;
      }
      else
      {
        if ((double) particle.Velocity.Z <= 0.0)
          return;
        position.Z += particle.Radius;
        if ((double) position.Z < (double) vector3_1.Z && this.map.IsBlockPassable(this.map.GetBlockID(this.map.GetPoint(position))))
          return;
        particle.Velocity.Z = -particle.Velocity.Z * this.bounce;
      }
    }

    protected virtual void CheckWorldCollision(ref ItemParticle particle)
    {
      float num1 = 0.1f;
      float num2 = particle.Radius - num1;
      float y = particle.Radius * 2f;
      this.data.Velocity = particle.Velocity;
      this.data.Box.Min = particle.Position + particle.Velocity - new Vector3(num2, 0.0f, num2);
      this.data.Box.Max = particle.Position + particle.Velocity + new Vector3(num2, y, num2);
      particle.Velocity = this.ClipWorldEdge(this.data.Box, this.data.Velocity);
      this.data.Box.Min = particle.Position + particle.Velocity - new Vector3(num2, 0.0f, num2);
      this.data.Box.Max = particle.Position + particle.Velocity + new Vector3(num2, y, num2);
      this.CheckWorldCollisionY(this.data);
      this.data.Box.Min = particle.Position + particle.Velocity - new Vector3(particle.Radius, -num1, num2);
      this.data.Box.Max = particle.Position + particle.Velocity + new Vector3(particle.Radius, y - num1, num2);
      this.CheckWorldCollisionX(this.data);
      this.data.Box.Min = particle.Position + particle.Velocity - new Vector3(num2, -num1, particle.Radius);
      this.data.Box.Max = particle.Position + particle.Velocity + new Vector3(num2, y - num1, particle.Radius);
      this.CheckWorldCollisionZ(this.data);
      particle.Velocity = this.data.Velocity;
    }

    private Vector3 ClipWorldEdge(BoundingBox box, Vector3 velocity)
    {
      if ((double) velocity.Y < 0.0)
      {
        if ((double) box.Min.Y + (double) velocity.Y < (double) this.map.MapBound.Min.Y * (double) this.map.TileSize)
          velocity.Y = 0.0f;
      }
      else if ((double) velocity.Y > 0.0 && (double) box.Max.Y + (double) velocity.Y >= (double) this.map.MapBound.Max.Y * (double) this.map.TileSize)
        velocity.Y = 0.0f;
      if ((double) velocity.X < 0.0)
      {
        if ((double) box.Min.X + (double) velocity.X < (double) this.map.MapBound.Min.X * (double) this.map.TileSize)
          velocity.X = 0.0f;
      }
      else if ((double) velocity.X > 0.0 && (double) box.Max.X + (double) velocity.X >= (double) this.map.MapBound.Max.X * (double) this.map.TileSize)
        velocity.X = 0.0f;
      if ((double) velocity.Z < 0.0)
      {
        if ((double) box.Min.Z + (double) velocity.Z < (double) this.map.MapBound.Min.Z * (double) this.map.TileSize)
          velocity.Z = 0.0f;
      }
      else if ((double) velocity.Z > 0.0 && (double) box.Max.Z + (double) velocity.Z >= (double) this.map.MapBound.Max.Z * (double) this.map.TileSize)
        velocity.Z = 0.0f;
      return velocity;
    }

    private void CheckWorldCollisionY(ParticleModifiers.CollisionData data)
    {
      if ((double) data.Velocity.Y < 0.0)
      {
        data.Box.Max = new Vector3(data.Box.Max.X, data.Box.Min.Y, data.Box.Max.Z);
        data.Normal = Vector3.Up;
        data.Point = this.map.GetPoint(data.Box.Min.X + (float) (((double) data.Box.Max.X - (double) data.Box.Min.X) * 0.5), data.Box.Min.Y, data.Box.Min.Z + (float) (((double) data.Box.Max.Z - (double) data.Box.Min.Z) * 0.5));
        this.ClipVelocity(data);
        if ((double) data.Velocity.Y != 0.0)
          return;
        data.Velocity.X *= 0.95f;
        data.Velocity.Z *= 0.95f;
      }
      else
      {
        if ((double) data.Velocity.Y <= 0.0)
          return;
        data.Box.Min = new Vector3(data.Box.Min.X, data.Box.Max.Y, data.Box.Min.Z);
        data.Normal = Vector3.Down;
        data.Point = this.map.GetPoint(data.Box.Min.X + (float) (((double) data.Box.Max.X - (double) data.Box.Min.X) * 0.5), data.Box.Max.Y, data.Box.Min.Z + (float) (((double) data.Box.Max.Z - (double) data.Box.Min.Z) * 0.5));
        this.ClipVelocity(data);
      }
    }

    private void CheckWorldCollisionX(ParticleModifiers.CollisionData data)
    {
      if ((double) data.Velocity.X < 0.0)
      {
        data.Box.Max = new Vector3(data.Box.Min.X, data.Box.Max.Y, data.Box.Max.Z);
        data.Normal = Vector3.Right;
        data.Point = this.map.GetPoint(data.Box.Min.X, data.Box.Min.Y + (float) (((double) data.Box.Max.Y - (double) data.Box.Min.Y) * 0.5), data.Box.Min.Z + (float) (((double) data.Box.Max.Z - (double) data.Box.Min.Z) * 0.5));
        this.ClipVelocity(data);
      }
      else
      {
        if ((double) data.Velocity.X <= 0.0)
          return;
        data.Box.Min = new Vector3(data.Box.Max.X, data.Box.Min.Y, data.Box.Min.Z);
        data.Normal = Vector3.Left;
        data.Point = this.map.GetPoint(data.Box.Max.X, data.Box.Min.Y + (float) (((double) data.Box.Max.Y - (double) data.Box.Min.Y) * 0.5), data.Box.Min.Z + (float) (((double) data.Box.Max.Z - (double) data.Box.Min.Z) * 0.5));
        this.ClipVelocity(data);
      }
    }

    private void CheckWorldCollisionZ(ParticleModifiers.CollisionData data)
    {
      if ((double) data.Velocity.Z < 0.0)
      {
        data.Box.Max = new Vector3(data.Box.Max.X, data.Box.Max.Y, data.Box.Min.Z);
        data.Normal = Vector3.Backward;
        data.Point = this.map.GetPoint(data.Box.Min.X + (float) (((double) data.Box.Max.X - (double) data.Box.Min.X) * 0.5), data.Box.Min.Y + (float) (((double) data.Box.Max.Y - (double) data.Box.Min.Y) * 0.5), data.Box.Min.Z);
        this.ClipVelocity(data);
      }
      else
      {
        if ((double) data.Velocity.Z <= 0.0)
          return;
        data.Box.Min = new Vector3(data.Box.Min.X, data.Box.Min.Y, data.Box.Max.Z);
        data.Normal = Vector3.Forward;
        data.Point = this.map.GetPoint(data.Box.Min.X + (float) (((double) data.Box.Max.X - (double) data.Box.Min.X) * 0.5), data.Box.Min.Y + (float) (((double) data.Box.Max.Y - (double) data.Box.Min.Y) * 0.5), data.Box.Max.Z);
        this.ClipVelocity(data);
      }
    }

    private void ClipVelocity(ParticleModifiers.CollisionData data)
    {
      if (this.map.IsBlockPassable(this.map.GetBlockID(data.Point)))
        return;
      BoundingBox blockBox = this.instance.GetBlockBox(data.Point);
      if (!data.Box.Intersects(blockBox))
        return;
      float num = Vector3.Dot(data.Normal, data.Velocity);
      data.Normal *= num;
      data.Velocity -= data.Normal;
    }

    private class CollisionData
    {
      public BoundingBox Box;
      public Vector3 Velocity;
      public Vector3 Normal;
      public GlobalPoint3D Point;
    }
  }
}
