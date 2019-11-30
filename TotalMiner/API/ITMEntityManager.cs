// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMEntityManager
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner.API
{
  public interface ITMEntityManager
  {
    /// <summary>Add an Entity to the world.</summary>
    /// <param name="comPack">The name of the Component Pack the the entities model component resides in.</param>
    /// <param name="comName">The name of the component used as the entities model.</param>
    /// <param name="entity">An entity object.</param>
    void AddEntity(string comPack, string comName, Entity entity);

    /// <summary>Remove an Entity from the world.</summary>
    /// <param name="entity">The entity to remove.</param>
    void RemoveEntity(Entity entity);
  }
}
