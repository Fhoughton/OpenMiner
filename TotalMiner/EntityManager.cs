// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.EntityManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class EntityManager : ITMEntityManager
  {
    private const int maxEntityTypes = 100;
    public int ActiveInstanceCount;
    public int ActiveDrawCount;
    private GameInstance instance;
    private List<EntityData> entities;

    void ITMEntityManager.AddEntity(string comPack, string comName, Entity entity)
    {
      this.AddEntity(comPack, comName, entity);
    }

    void ITMEntityManager.RemoveEntity(Entity entity)
    {
      this.RemoveEntity(entity);
    }

    public List<EntityData> ActiveEntities
    {
      get
      {
        return this.entities;
      }
    }

    public EntityManager(GameInstance instance)
    {
      this.instance = instance;
      this.entities = new List<EntityData>();
    }

    public void AddEntity(string comPack, string comName, Entity entity)
    {
      int dirNum = this.instance.VoxelModelManager.GetDirNum(comPack);
      for (int index = 0; index < this.entities.Count; ++index)
      {
        MapModel model = this.entities[index].Content.Model;
        if (model.DirNum == dirNum && model.ComName == comName)
        {
          this.entities[index].Entities.Add(entity);
          entity.Radius = (float) ((double) Math.Max(Math.Max(model.ModelSize.X, model.ModelSize.Z), model.ModelSize.Y) * (double) entity.Scale * 0.5);
          entity.CenterOffY = (float) (-(double) entity.Scale + (double) model.ModelSize.Y * (double) entity.Scale * 0.5);
          return;
        }
      }
      lock (this.entities)
      {
        if (this.entities.Count >= 100)
          return;
        EntityData entityData = new EntityData();
        entityData.Content = new EntityContentFrame();
        entityData.Content.LoadContent(this.instance, dirNum, comName);
        entityData.Entities = new List<Entity>();
        entityData.Entities.Add(entity);
        this.entities.Add(entityData);
        MapModel model = entityData.Content.Model;
        entity.Radius = (float) ((double) Math.Max(Math.Max(model.ModelSize.X, model.ModelSize.Z), model.ModelSize.Y) * (double) entity.Scale * 0.5);
        entity.CenterOffY = (float) (-(double) entity.Scale + (double) model.ModelSize.Y * (double) entity.Scale * 0.5);
      }
    }

    public void RemoveEntity(Entity entity)
    {
      for (int index = 0; index < this.entities.Count; ++index)
      {
        EntityData entity1 = this.entities[index];
        if (entity.ContentID == entity1.Content.ContentID)
        {
          entity1.Entities.Remove(entity);
          break;
        }
      }
    }

    public void Update()
    {
      this.ActiveInstanceCount = 0;
      foreach (EntityData entity1 in this.entities)
      {
        foreach (Entity entity2 in entity1.Entities)
          entity2.Update();
        this.ActiveInstanceCount += entity1.Entities.Count;
      }
    }
  }
}
