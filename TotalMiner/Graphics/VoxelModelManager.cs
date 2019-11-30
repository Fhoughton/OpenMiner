// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.VoxelModelManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Graphics
{
  internal class VoxelModelManager
  {
    public readonly string RootDir;
    private GameInstance instance;
    private Dictionary<string, int> dirNumbers;
    private Dictionary<string, MapModel> modelCache;
    private bool isSystem;

    public VoxelModelManager(GameInstance instance, string rootDir, bool isSystem)
    {
      this.instance = instance;
      this.RootDir = rootDir;
      this.isSystem = isSystem;
      this.dirNumbers = new Dictionary<string, int>();
      this.modelCache = new Dictionary<string, MapModel>();
      if (this.RootDir == null || this.RootDir.Length <= 0 || this.RootDir[rootDir.Length - 1] == '\\')
        return;
      this.RootDir += (string) (object) '\\';
    }

    public void UnloadContent()
    {
      if (this.modelCache == null)
        return;
      foreach (MapModel mapModel in this.modelCache.Values)
        mapModel.UnloadContent();
      this.modelCache.Clear();
    }

    public bool HasComponent(string componentPack, string componentName, bool checkSystemTemp)
    {
      if (checkSystemTemp && this.HasComponent(this.GetDirNum("System Temp"), componentPack + "_" + componentName))
        return true;
      return this.HasComponent(this.GetDirNum(componentPack), componentName);
    }

    public bool HasComponent(int dirNum, string componentName)
    {
      if (dirNum < 0)
        return false;
      string path = Globals2.ComponentPath(this.RootDir, dirNum) + componentName + ".com";
      if (!this.isSystem)
        return FileSystem.IsFileExist(path);
      return TitleFileSystem.IsFileExist(path);
    }

    public bool IsComponentLoaded(string componentPack, string componentName, bool isMeshLoaded)
    {
      int dirNum = this.GetDirNum(componentPack);
      if (dirNum >= 0)
      {
        lock (this.modelCache)
        {
          MapModel mapModel;
          if (this.modelCache.TryGetValue(this.GetKey(dirNum, componentName), out mapModel))
            return !isMeshLoaded || mapModel.Map.GetChunk(GlobalPoint3D.Zero).IsMeshLoaded;
        }
      }
      return false;
    }

    public MapModel LoadModComponent(string componentName, bool buildMesh)
    {
      string errorDesc;
      return this.LoadComponent(0, componentName, buildMesh, out errorDesc, (Action<bool, object>) null, (object) null);
    }

    public MapModel LoadComponent(
      string componentPack,
      string componentName,
      bool buildMesh)
    {
      string errorDesc;
      return this.LoadComponent(componentPack, componentName, buildMesh, out errorDesc);
    }

    public MapModel LoadComponent(
      string componentPack,
      string componentName,
      bool buildMesh,
      out string errorDesc)
    {
      return this.LoadComponent(this.GetDirNum(componentPack), componentName, buildMesh, out errorDesc);
    }

    public MapModel LoadComponent(int dirNum, string componentName, bool buildMesh)
    {
      string errorDesc;
      return this.LoadComponent(dirNum, componentName, buildMesh, out errorDesc, (Action<bool, object>) null, (object) null);
    }

    public MapModel LoadComponent(
      string componentPack,
      string componentName,
      bool buildMesh,
      bool checkSystemTemp)
    {
      string errorDesc;
      if (checkSystemTemp)
      {
        MapModel mapModel = this.LoadComponent(this.GetDirNum("System Temp"), componentPack + "_" + componentName, buildMesh, out errorDesc, (Action<bool, object>) null, (object) null);
        if (mapModel != null)
          return mapModel;
      }
      return this.LoadComponent(componentPack, componentName, buildMesh, out errorDesc);
    }

    public MapModel LoadComponent(
      int dirNum,
      string componentName,
      bool buildMesh,
      out string errorDesc)
    {
      return this.LoadComponent(dirNum, componentName, buildMesh, out errorDesc, (Action<bool, object>) null, (object) null);
    }

    public MapModel LoadComponent(
      int dirNum,
      string componentName,
      bool buildMesh,
      out string errorDesc,
      Action<bool, object> action,
      object state)
    {
      if (dirNum >= 0)
      {
        lock (this.modelCache)
        {
          LoadComponentResult loadComponentResult;
          if (action != null)
            loadComponentResult = new LoadComponentResult()
            {
              Action = action,
              State = state,
              VoxelModelManager = this
            };
          else
            loadComponentResult = (LoadComponentResult) null;
          LoadComponentResult result = loadComponentResult;
          MapModel mapModel = this.LoadFromCache(dirNum, componentName, buildMesh, result);
          if (mapModel == null)
            return this.AddToCache(dirNum, componentName, buildMesh, out errorDesc, result);
          errorDesc = (string) null;
          return mapModel;
        }
      }
      else
      {
        errorDesc = "Invalid component pack file";
        return (MapModel) null;
      }
    }

    public byte[] LoadComponentRawData(string comPack, string comName, bool checkSystemTemp)
    {
      if (checkSystemTemp)
      {
        byte[] numArray = this.LoadComponentRawData(this.GetDirNum("System Temp"), comPack, comName);
        if (numArray != null)
          return numArray;
      }
      return this.LoadComponentRawData(this.GetDirNum(comPack), comPack, comName);
    }

    private byte[] LoadComponentRawData(int dirnum, string comPack, string comName)
    {
      if (dirnum >= 0)
      {
        try
        {
          using (Stream fileToRead = this.GetFileToRead(Globals2.ComponentPath(this.RootDir, dirnum) + comName + ".com"))
          {
            byte[] buffer = new byte[fileToRead.Length];
            fileToRead.Read(buffer, 0, (int) fileToRead.Length);
            return buffer;
          }
        }
        catch (Exception ex)
        {
        }
      }
      return (byte[]) null;
    }

    private string GetKey(int dirNum, string componentName)
    {
      return dirNum.ToString() + "_" + componentName;
    }

    private MapModel LoadFromCache(
      int dirNum,
      string componentName,
      bool buildMesh,
      LoadComponentResult result)
    {
      if (this.modelCache.Count > 0)
      {
        MapModel mapModel = (MapModel) null;
        if (this.modelCache.TryGetValue(this.GetKey(dirNum, componentName), out mapModel))
        {
          if (buildMesh)
          {
            if (result != null)
              result.Model = mapModel;
            mapModel.LoadContent(buildMesh, result?.Action, (object) result);
          }
          return mapModel;
        }
      }
      return (MapModel) null;
    }

    private MapModel AddToCache(
      int dirNum,
      string componentName,
      bool buildMesh,
      out string errorDescription,
      LoadComponentResult result)
    {
      try
      {
        string filename = Globals2.ComponentPath(this.RootDir, dirNum) + componentName + ".com";
        errorDescription = (string) null;
        using (Stream fileToRead = this.GetFileToRead(filename))
        {
          byte[] numArray = new byte[fileToRead.Length];
          fileToRead.Read(numArray, 0, (int) fileToRead.Length);
          using (MemoryStream memoryStream = new MemoryStream(numArray, false))
          {
            using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
            {
              int version = reader.ReadInt32();
              if (Globals2.CheckHash(numArray, version))
              {
                MapTM map = this.ReadComponentData(reader, version);
                map.Name = componentName;
                MapModel mapModel = new MapModel(this.instance, map);
                mapModel.DirNum = dirNum;
                mapModel.ComName = componentName;
                mapModel.Initialize((InitState) null);
                if (result != null)
                  result.Model = mapModel;
                mapModel.LoadContent(buildMesh, result?.Action, (object) result);
                this.modelCache.Add(this.GetKey(dirNum, componentName), mapModel);
                return mapModel;
              }
              errorDescription = "This component has been tampered with and cannot be loaded";
              return (MapModel) null;
            }
          }
        }
      }
      catch (IOException ex)
      {
        errorDescription = ex.Message;
        return (MapModel) null;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(80, ex);
        errorDescription = ex.Message;
        return (MapModel) null;
      }
    }

    private MapTM ReadComponentData(BinaryReader reader, int version)
    {
      int x = reader.ReadInt32();
      int y = reader.ReadInt32();
      int z = reader.ReadInt32();
      double num = (double) reader.ReadSingle();
      BoxInt boxInt = new BoxInt();
      boxInt.Max = new GlobalPoint3D(x, y, z);
      Point3D regionSize = new Point3D(boxInt.Max);
      Point3D chunkSize = regionSize;
      MapStrategyTM mapStrategyTm = new MapStrategyTM(this.instance);
      MapTM mapTm = new MapTM(this.instance, (string) null, 1f, false, boxInt, boxInt, regionSize, chunkSize, Globals1.BlockData, 15, 0, (ushort) 1, 4, (MapStrategy) mapStrategyTm, true, false);
      mapTm.SeaLevel = (ushort) 0;
      mapTm.PregenerateRegions(true, true, (IProgressBar) null);
      mapTm.GetChunk(GlobalPoint3D.Zero).ReadData(reader, version, true);
      if (version > 72)
      {
        mapTm.BlockTextures = MapLoader.ReadBlockTextures(reader, version);
        if (version > 114)
        {
          MapSerializer data = new MapSerializer();
          MapSerializer.DeserializeBinary(reader, data);
          mapStrategyTm.ReplaceDataBlocks(data.DataBlocks);
          mapTm.SignTextCache = data.SignTextCache;
        }
      }
      this.ConvertLightData((Map) mapTm);
      return mapTm;
    }

    private Stream GetFileToRead(string filename)
    {
      if (!this.isSystem)
        return FileSystem.OpenRead(filename);
      return TitleFileSystem.OpenFile(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private void ConvertLightData(Map map)
    {
      foreach (MapRegion mapRegion in map.Regions.Values)
      {
        mapRegion.HeightMap.SetHeight((ushort) 0);
        foreach (MapChunk chunk in mapRegion.Chunks)
        {
          chunk.SetChunkFlag(ChunkFlags.Generated | ChunkFlags.Decorated | ChunkFlags.LightDirty | ChunkFlags.ReceivedFromHost);
          map.UpdateHeightData(chunk, false);
          int next = MapLightingByChunkThreadedWrapper.Pool.GetNext();
          MapLightingByChunkThreadedWrapper chunkThreadedWrapper = MapLightingByChunkThreadedWrapper.Pool.List[next];
          chunkThreadedWrapper.Initialize(this.instance, map, next, chunk, false);
          chunkThreadedWrapper.Update();
          MapLightingByChunkThreadedWrapper.Pool.Release(next);
        }
      }
    }

    private void ConvertHeight(MapChunk chunk)
    {
      int num1 = 0;
      int num2 = 0;
      MapRegion region = chunk.Region;
      lock (chunk.RleLock)
      {
        RLEStreamByte blockData = chunk.BlockData;
        byte data;
        lock (BuffLock.StreamLock)
        {
          byte[] numArray = Map.RLEStreamBufferManager.Stream[(int) blockData.StreamID];
          for (; num2 < blockData.StreamSize - 2; num2 += 2)
          {
            int num3 = (int) numArray[blockData.StreamIndex + num2];
            data = numArray[blockData.StreamIndex + num2 + 1];
            for (int index = 0; index <= num3; ++index)
              this.ConvertHeight(region, chunk, num1++, data);
          }
          data = numArray[blockData.StreamIndex + num2 + 1];
        }
        while (num1 < region.Map.ChunkLength)
          this.ConvertHeight(region, chunk, num1++, data);
      }
    }

    private void ConvertHeight(MapRegion region, MapChunk chunk, int index, byte data)
    {
      if (data == (byte) 0)
        return;
      Point3D point = chunk.GetPoint(index);
      if ((int) region.GetHeight(point) >= point.Y)
        return;
      region.SetHeight(point.X, point.Z, (ushort) point.Y, (ushort) point.Y);
    }

    public static void MergeBlockTextureIndexes(GameInstance instance, MapModel model)
    {
      if (model.HasMergedBlockTextures)
        return;
      MapTM map1 = model.Map;
      if (map1.BlockTextures != null)
      {
        int chunkLength = map1.ChunkLength;
        MapChunk chunk = map1.GetChunk(GlobalPoint3D.Zero);
        MapTM map2 = instance.Map;
        for (int mapIndex = 0; mapIndex < chunkLength; ++mapIndex)
        {
          Block data1 = (Block) chunk.BlockData.GetData(chunk, mapIndex);
          if (map2.UsesBlockTextureTable(data1))
          {
            byte data2 = chunk.AuxData.GetData(chunk, mapIndex);
            int textureIndex = (int) data2 >> 4;
            Block blockTextureId1 = map2.GetBlockTextureID(data1, textureIndex);
            Block blockTextureId2 = map1.GetBlockTextureID(data1, textureIndex);
            if (blockTextureId1 != blockTextureId2)
            {
              int num1 = map2.GetOrAddBlockTextureIndex(data1, blockTextureId2);
              if (num1 != textureIndex)
              {
                if (num1 == -1)
                  num1 = textureIndex;
                byte num2 = (byte) (((int) data2 & 15) + (num1 << 4));
                chunk.AuxData.SetData(chunk, mapIndex, num2);
              }
            }
          }
        }
      }
      model.HasMergedBlockTextures = true;
    }

    public static void SaveComponent(MapModel model)
    {
      if (model == null || model.DirNum <= 0 || model.ComName == null)
        return;
      VoxelModelManager.SaveComponent(model.DirNum, model.ComName, model.Map);
    }

    public static void SaveComponent(int dirNum, string componentName, MapTM map)
    {
      string str = componentName + ".com";
      if (str.Length > 40)
        throw new CoreException("Filename is too long. Maximum filename is 40 characters");
      string filename = Globals2.ComponentPath(dirNum) + str;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          VoxelModelManager.WriteComponentData(writer, map);
          Globals2.WriteFileWithHash(filename, (Stream) memoryStream, writer, 294);
        }
      }
    }

    public static void SaveComponentNoHash(int dirNum, string componentName, byte[] data)
    {
      string str = componentName + ".com";
      if (str.Length > 40)
        throw new CoreException("Filename is too long. Maximum filename is 40 characters");
      string filename = Globals2.ComponentPath(dirNum) + str;
      try
      {
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
            Globals2.WriteFileNoHash(filename, writer);
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(44, ex);
      }
    }

    private static void WriteComponentData(BinaryWriter writer, MapTM map)
    {
      writer.Write(294);
      writer.Write(map.MapSize.X);
      writer.Write(map.MapSize.Y);
      writer.Write(map.MapSize.Z);
      writer.Write(map.TileSize);
      map.GetChunk(GlobalPoint3D.Zero).WriteData(writer);
      VoxelModelManager.WriteComponentAuxData(writer, map);
    }

    private static void WriteComponentAuxData(BinaryWriter writer, MapTM map)
    {
      MapSaver.WriteBlockTextures(writer, map.BlockTextures);
      new MapSerializer().SerializeBinary(writer, map, false);
    }

    public void UnloadComponent(MapModel model)
    {
      lock (this.modelCache)
      {
        string key = this.GetKey(model.DirNum, model.ComName);
        if (this.modelCache.ContainsKey(key))
          this.modelCache.Remove(key);
        model.UnloadContent();
      }
    }

    public void UnloadComponents(
      List<MapModel> exclude,
      ModelFlags includeFlags,
      ModelFlags excludeFlags)
    {
      List<string> stringList = new List<string>();
      lock (this.modelCache)
      {
        foreach (KeyValuePair<string, MapModel> keyValuePair in this.modelCache)
        {
          if (!exclude.Contains(keyValuePair.Value) && (keyValuePair.Value.Flags & includeFlags) > ModelFlags.None && (keyValuePair.Value.Flags & excludeFlags) == ModelFlags.None)
          {
            stringList.Add(keyValuePair.Key);
            keyValuePair.Value.UnloadContent();
          }
        }
        foreach (string key in stringList)
          this.modelCache.Remove(key);
      }
    }

    public string GetPackName(int dirnum)
    {
      this.GetDirNum((string) null);
      lock (this.dirNumbers)
      {
        foreach (KeyValuePair<string, int> dirNumber in this.dirNumbers)
        {
          if (dirNumber.Value == dirnum)
            return dirNumber.Key;
        }
      }
      return (string) null;
    }

    public void NewCompPackDirAdded(int dirnum, string comPack)
    {
      this.GetDirNum((string) null);
      lock (this.dirNumbers)
      {
        if (this.dirNumbers.ContainsKey(comPack.ToLower()))
          return;
        this.dirNumbers.Add(comPack.ToLower(), dirnum);
      }
    }

    public int GetDirNum(string componentPack)
    {
      int num1 = -1;
      if (componentPack != null)
        componentPack = componentPack.ToLower();
      lock (this.dirNumbers)
      {
        if (this.dirNumbers.Count > 0)
        {
          if (componentPack != null && this.dirNumbers.TryGetValue(componentPack, out num1))
            return num1;
          return -1;
        }
        string[] strArray = (string[]) null;
        try
        {
          strArray = this.GetDirNames(this.RootDir + "Com\\");
        }
        catch (DirectoryNotFoundException ex)
        {
          if (!this.isSystem)
            FileSystem.CreateDir(this.RootDir + "Com");
        }
        if (strArray != null)
        {
          if (strArray.Length > 0)
          {
            foreach (string str in strArray)
            {
              if (this.FileExists(str + "\\header.dat"))
              {
                try
                {
                  int num2 = int.Parse(str.Substring(str.Length - 6, 6));
                  using (Stream fileToRead = this.GetFileToRead(str + "\\header.dat"))
                  {
                    using (BinaryReader binaryReader = new BinaryReader(fileToRead))
                    {
                      string key = this.ConvertSystemPackNames(binaryReader.ReadString().ToLower());
                      if (key != null && !this.dirNumbers.ContainsKey(key))
                        this.dirNumbers.Add(key, num2);
                      if (key == componentPack)
                        num1 = num2;
                    }
                  }
                }
                catch (Exception ex)
                {
                }
              }
            }
          }
        }
      }
      return num1;
    }

    private string ConvertSystemPackNames(string packName)
    {
      if (this.isSystem && packName == "avatars")
        return "system avatars";
      return packName;
    }

    private string[] GetDirNames(string path)
    {
      if (!this.isSystem)
        return FileSystem.GetDirs(path);
      return TitleFileSystem.GetDirs(path);
    }

    private bool FileExists(string filename)
    {
      if (!this.isSystem)
        return FileSystem.IsFileExist(filename);
      return TitleFileSystem.IsFileExist(filename);
    }
  }
}
