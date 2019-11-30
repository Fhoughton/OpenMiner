// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.MapSerializer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Storage
{
  internal class MapSerializer
  {
    public Dictionary<long, DataBlock> DataBlocks = new Dictionary<long, DataBlock>();
    public Dictionary<long, byte> BlocksReceivingPower = new Dictionary<long, byte>();
    public Dictionary<long, byte> BlocksDeliveringPower = new Dictionary<long, byte>();
    public List<string> SignTextCache = new List<string>();

    public void Serialize(string filename, MapTM map)
    {
      if (filename == null || map == null)
        return;
      this.SerializeBinary(filename, map);
    }

    private void SerializeBinary(string filename, MapTM map)
    {
      using (Stream output = FileSystem.OpenWrite(filename))
      {
        using (BinaryWriter writer = new BinaryWriter(output))
          this.SerializeBinary(writer, map, false);
      }
    }

    public void SerializeBinary(BinaryWriter writer, MapTM map, bool omitDataBlocks)
    {
      writer.Write(294);
      this.SerializeDataBlocks(writer, map, omitDataBlocks);
      this.SerializeSignTextCache(writer, map);
      this.SerializePower(writer, map);
    }

    private void SerializeDataBlocks(BinaryWriter writer, MapTM map, bool omitDataBlocks)
    {
      int num = 0;
      long position1 = writer.BaseStream.Position;
      writer.Write(num);
      if (!omitDataBlocks)
      {
        MapStrategyTM mapStrategy = map.MapStrategy as MapStrategyTM;
        if (mapStrategy != null)
        {
          Dictionary<long, DataBlock> dataBlocks = mapStrategy.DataBlocks;
          if (dataBlocks != null)
          {
            lock (dataBlocks)
            {
              foreach (KeyValuePair<long, DataBlock> keyValuePair in dataBlocks)
              {
                MapChunk chunk = map.GetChunk(keyValuePair.Value.Point);
                if (chunk != null && chunk.IsChunkFlagsSet(ChunkFlags.UserEdited))
                {
                  writer.Write(keyValuePair.Key);
                  writer.Write((byte) keyValuePair.Value.ClassType);
                  keyValuePair.Value.WriteState(writer);
                  ++num;
                }
              }
            }
          }
        }
      }
      long position2 = writer.BaseStream.Position;
      writer.BaseStream.Position = position1;
      writer.Write(num);
      writer.BaseStream.Position = position2;
    }

    private void SerializeSignTextCache(BinaryWriter writer, MapTM map)
    {
      lock (map.SignTextCache)
      {
        int num = 0;
        long position = writer.BaseStream.Position;
        writer.Write(num);
        Dictionary<long, DataBlock> dataBlocks = map.MapStrategyTM.DataBlocks;
        for (int index = 0; index < map.SignTextCache.Count; ++index)
        {
          string str = map.SignTextCache[index];
          if (str != null && str.Length > 0 && SignBlock.IsTextUsed(dataBlocks, index))
          {
            ++num;
            writer.Write((ushort) index);
            writer.Write(str);
          }
        }
        writer.BaseStream.Position = position;
        writer.Write(num);
        writer.BaseStream.Position = writer.BaseStream.Length;
      }
    }

    private void SerializePower(BinaryWriter writer, MapTM map)
    {
      MapStrategyTM mapStrategy = map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
      {
        Dictionary<long, byte> blocksReceivingPower = mapStrategy.BlocksReceivingPower;
        if (blocksReceivingPower != null)
        {
          lock (blocksReceivingPower)
          {
            writer.Write(blocksReceivingPower.Count);
            foreach (KeyValuePair<long, byte> keyValuePair in blocksReceivingPower)
            {
              writer.Write(keyValuePair.Key);
              writer.Write(keyValuePair.Value);
            }
          }
        }
        else
          writer.Write(0);
        Dictionary<long, byte> blocksDeliveringPower = mapStrategy.BlocksDeliveringPower;
        if (blocksDeliveringPower != null)
        {
          lock (blocksDeliveringPower)
          {
            writer.Write(blocksDeliveringPower.Count);
            foreach (KeyValuePair<long, byte> keyValuePair in blocksDeliveringPower)
            {
              writer.Write(keyValuePair.Key);
              writer.Write(keyValuePair.Value);
            }
          }
        }
        else
          writer.Write(0);
      }
      else
      {
        writer.Write(0);
        writer.Write(0);
      }
    }

    public static MapSerializer Deserialize(string filename)
    {
      return MapSerializer.DeserializeBinary(filename);
    }

    private static MapSerializer DeserializeBinary(string filename)
    {
      MapSerializer data = new MapSerializer();
      try
      {
        using (Stream stream = FileSystem.OpenRead(filename))
        {
          byte[] buffer = new byte[stream.Length];
          stream.Read(buffer, 0, (int) stream.Length);
          using (MemoryStream memoryStream = new MemoryStream(buffer))
          {
            using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
              MapSerializer.DeserializeBinary(reader, data);
          }
        }
      }
      catch (FileNotFoundException ex)
      {
        return data;
      }
      catch (Exception ex)
      {
        data.SignTextCache.Clear();
        data.BlocksReceivingPower.Clear();
        data.BlocksDeliveringPower.Clear();
        Services.ExceptionReporter.ReportExceptionCaught(30, ex);
        throw ex;
      }
      return data;
    }

    public static void DeserializeBinary(BinaryReader reader, MapSerializer data)
    {
      int version = reader.ReadInt32();
      data.DeserializeDataBlocks(reader, version);
      data.DeserializeSignTextCache(reader, version);
      data.DeserializePower(reader, version);
    }

    private void DeserializeDataBlocks(BinaryReader reader, int version)
    {
      this.DataBlocks.Clear();
      int num = reader.ReadInt32();
      try
      {
        for (int index = 0; index < num; ++index)
        {
          long key = reader.ReadInt64();
          DataBlock dataBlock = this.ReadDataBlockState(reader, version);
          if (dataBlock != null)
            this.DataBlocks.Add(key, dataBlock);
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(27, ex);
      }
    }

    private void DeserializeSignTextCache(BinaryReader reader, int version)
    {
      this.SignTextCache.Clear();
      if (version <= 116)
        return;
      try
      {
        int num1 = reader.ReadInt32();
        if (version > 180)
        {
          for (int index = 0; index < num1; ++index)
          {
            int num2 = (int) reader.ReadUInt16();
            string str = reader.ReadString();
            while (this.SignTextCache.Count < num2)
              this.SignTextCache.Add((string) null);
            this.SignTextCache.Add(str);
          }
        }
        else
        {
          for (int index = 0; index < num1; ++index)
            this.SignTextCache.Add(reader.ReadString());
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(26, ex);
      }
    }

    private void DeserializePower(BinaryReader reader, int version)
    {
      this.BlocksReceivingPower.Clear();
      this.BlocksDeliveringPower.Clear();
      if (version <= 120)
        return;
      try
      {
        int num1 = reader.ReadInt32();
        for (int index = 0; index < num1; ++index)
          this.BlocksReceivingPower.Add(reader.ReadInt64(), reader.ReadByte());
        int num2 = reader.ReadInt32();
        for (int index = 0; index < num2; ++index)
          this.BlocksDeliveringPower.Add(reader.ReadInt64(), reader.ReadByte());
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(25, ex);
      }
    }

    private DataBlock ReadDataBlockState(BinaryReader reader, int version)
    {
      DataBlockType type = (DataBlockType) reader.ReadByte();
      if (version < 136)
        ++type;
      DataBlock dataBlock = MapSerializer.CreateDataBlock(type);
      if (dataBlock != null)
      {
        dataBlock.ReadState(reader, version);
        if (version < 143)
          this.ConvertDataBlockPre143(dataBlock, version);
      }
      return dataBlock;
    }

    private void ConvertDataBlockPre143(DataBlock block, int version)
    {
      if (block.ClassType != DataBlockType.Shop)
        return;
      this.ConvertShopPre143(block as ShopBlock, version);
    }

    private void ConvertShopPre143(ShopBlock block, int version)
    {
      for (int index = 0; index < block.Inventory.Count; ++index)
      {
        InventoryItem inventoryItem = block.Inventory[index];
        if (inventoryItem.ItemID != Item.None && inventoryItem.ItemID != Item.Book)
        {
          inventoryItem.Durability = ItemData.GetItemDurability(inventoryItem.ItemID);
          block.Inventory[index] = inventoryItem;
        }
      }
    }

    public static DataBlock CreateDataBlock(DataBlockType type)
    {
      switch (type)
      {
        case DataBlockType.ParticleEmitter:
          return (DataBlock) new ParticleEmitterBlock();
        case DataBlockType.Door:
          return (DataBlock) new DoorBlock();
        case DataBlockType.Marker:
          return (DataBlock) new StudioForge.TotalMiner.Blocks.MarkerBlock();
        case DataBlockType.Shop:
          return (DataBlock) new ShopBlock();
        case DataBlockType.Chest:
          return (DataBlock) new ChestBlock();
        case DataBlockType.Furnace:
          return (DataBlock) new FurnaceBlock();
        case DataBlockType.Bookcase:
          return (DataBlock) new BookcaseBlock();
        case DataBlockType.AmbientSound:
          return (DataBlock) new AmbientSoundBlock();
        case DataBlockType.SentryTurret:
          return (DataBlock) new SentryTurretBlock();
        case DataBlockType.ProximityDetector:
          return (DataBlock) new ProximityDetectorBlock();
        case DataBlockType.Fire:
          return (DataBlock) new FireBlock();
        case DataBlockType.NPCSpawn:
          return (DataBlock) new NpcSpawnBlock();
        case DataBlockType.Script:
          return (DataBlock) new ScriptBlock();
        case DataBlockType.Sign:
          return (DataBlock) new SignBlock();
        case DataBlockType.MobSpawn:
          return (DataBlock) new NpcSpawnBlock() { IsOldMobSpawnBlock = true };
        case DataBlockType.Teleport:
          return (DataBlock) new TeleportBlock();
        case DataBlockType.WifiReceiver:
          return (DataBlock) new WifiReceiverBlock();
        case DataBlockType.WifiTransmitter:
          return (DataBlock) new WifiTransmitterBlock();
        case DataBlockType.Crop:
          return (DataBlock) new CropBlock();
        case DataBlockType.Blueprint:
          return (DataBlock) new BlueprintBlock();
        case DataBlockType.WisdomScroll:
          return (DataBlock) new WisdomScrollBlock();
        case DataBlockType.Sundial:
          return (DataBlock) new SundialBlock();
        case DataBlockType.Book:
          return (DataBlock) new BookBlock();
        case DataBlockType.Health:
          return (DataBlock) new HealthBlock();
        default:
          return (DataBlock) null;
      }
    }
  }
}
