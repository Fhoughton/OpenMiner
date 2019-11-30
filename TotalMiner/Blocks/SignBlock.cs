// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.SignBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class SignBlock : DataBlock
  {
    private static char[] delims = new char[1]{ '_' };
    public short Text1 = -1;
    public short Text2 = -1;
    public short Text3 = -1;
    public short Text4 = -1;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.Sign;
      }
    }

    public bool HasText
    {
      get
      {
        if (this.Text1 < (short) 0 && this.Text2 < (short) 0 && this.Text3 < (short) 0)
          return this.Text4 >= (short) 0;
        return true;
      }
    }

    public SignBlock()
    {
    }

    public SignBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public SignBlock(MapTM map, GlobalPoint3D p, string text)
      : this(p)
    {
      this.SetText(map, text);
    }

    public void SetText(MapTM map, string text)
    {
      this.Text1 = this.Text2 = this.Text3 = this.Text4 = (short) -1;
      string[] strArray = text.Split(SignBlock.delims);
      if (strArray == null)
        return;
      if (strArray.Length > 0)
        this.Text1 = SignBlock.AddText(map, strArray[0]);
      if (strArray.Length > 1)
        this.Text2 = SignBlock.AddText(map, strArray[1]);
      if (strArray.Length > 2)
        this.Text3 = SignBlock.AddText(map, strArray[2]);
      if (strArray.Length <= 3)
        return;
      this.Text4 = SignBlock.AddText(map, strArray[3]);
    }

    public static short AddText(MapTM map, string text)
    {
      if (map == null || text == null || text.Length <= 0)
        return -1;
      lock (map.SignTextCache)
      {
        for (int index = 0; index < map.SignTextCache.Count; ++index)
        {
          if (map.SignTextCache[index] != null && map.SignTextCache[index] == text)
            return (short) index;
        }
        for (int index = 0; index < map.SignTextCache.Count; ++index)
        {
          if (map.SignTextCache[index] == null)
          {
            map.SignTextCache[index] = text;
            map.SignTextCacheChanged = true;
            return (short) index;
          }
        }
        map.SignTextCache.Add(text);
        map.SignTextCacheChanged = true;
        return (short) (map.SignTextCache.Count - 1);
      }
    }

    public static bool RemoveText(MapTM map, List<SignBlock> signs, SignBlock sign)
    {
      if (sign == null)
        return false;
      if (sign.Text1 >= (short) 0)
        SignBlock.RemoveText(map, signs, (int) sign.Text1);
      if (sign.Text2 >= (short) 0)
        SignBlock.RemoveText(map, signs, (int) sign.Text2);
      if (sign.Text3 >= (short) 0)
        SignBlock.RemoveText(map, signs, (int) sign.Text3);
      if (sign.Text4 >= (short) 0)
        SignBlock.RemoveText(map, signs, (int) sign.Text4);
      return true;
    }

    private static void RemoveText(MapTM map, List<SignBlock> signs, int index)
    {
      if (map == null || SignBlock.IsTextUsed(signs, index))
        return;
      lock (map.SignTextCache)
      {
        if (index < map.SignTextCache.Count)
          map.SignTextCache[index] = (string) null;
      }
      map.SignTextCacheChanged = true;
    }

    public static bool IsTextUsed(List<SignBlock> signs, int index)
    {
      for (int index1 = 0; index1 < signs.Count; ++index1)
      {
        SignBlock sign = signs[index1];
        if ((int) sign.Text1 == index || (int) sign.Text2 == index || ((int) sign.Text3 == index || (int) sign.Text4 == index))
          return true;
      }
      return false;
    }

    public static bool IsTextUsed(Dictionary<long, DataBlock> blocks, int index)
    {
      if (blocks != null)
      {
        foreach (KeyValuePair<long, DataBlock> block in blocks)
        {
          if (block.Value != null && block.Value.ClassType == DataBlockType.Sign)
          {
            SignBlock signBlock = block.Value as SignBlock;
            if ((int) signBlock.Text1 == index || (int) signBlock.Text2 == index || ((int) signBlock.Text3 == index || (int) signBlock.Text4 == index))
              return true;
          }
        }
      }
      return false;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      SignBlock signBlock = from as SignBlock;
      this.Text1 = signBlock.Text1;
      this.Text2 = signBlock.Text2;
      this.Text3 = signBlock.Text3;
      this.Text4 = signBlock.Text4;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      if (version <= 115)
        return;
      this.Text1 = reader.ReadInt16();
      this.Text2 = reader.ReadInt16();
      this.Text3 = reader.ReadInt16();
      this.Text4 = reader.ReadInt16();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Text1);
      writer.Write(this.Text2);
      writer.Write(this.Text3);
      writer.Write(this.Text4);
    }

    public static SignBlock LoadFromSaveData(SaveSignState state)
    {
      return new SignBlock(state.Point)
      {
        Text1 = state.Text1,
        Text2 = state.Text2,
        Text3 = state.Text3,
        Text4 = state.Text4
      };
    }
  }
}
