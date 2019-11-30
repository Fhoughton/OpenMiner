// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.SkillDataXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;
using System;

namespace StudioForge.TotalMiner
{
  public struct SkillDataXML
  {
    public Item ItemID;
    public int MineReq;
    public int UseReq;
    public SkillType UseSkill;
    public int CraftReq;
    public SkillType CraftSkill;

    public float UseExp
    {
      get
      {
        return (float) (1.0 + (double) this.UseReq / 20.0);
      }
    }

    public float MineExp
    {
      get
      {
        Item obj = this.ItemID;
        if (obj > Item.zLastBlockID)
        {
          if (this.MineReq == 0)
            return 0.0f;
          obj = ItemData.ConvertItemIDToBlockID(this.ItemID);
          if (obj > Item.zLastBlockID)
            return 0.0f;
        }
        BlockDataXML blockDataXml = Globals1.BlockData[(int) obj];
        BlockMaterialDataXML blockMaterialDataXml = Globals1.BlockMaterialData[(int) blockDataXml.Material];
        if (blockMaterialDataXml.Resistance == (ushort) 0)
          return 0.0f;
        float resistance = (float) blockMaterialDataXml.Resistance;
        float num1 = (float) (1.0 + (double) this.MineReq / 2000.0 + (double) Math.Min(10000, Globals1.ItemData[(int) this.ItemID].MinCSPrice) / 100000.0);
        float num2 = resistance * ((float) (1.0 + (double) resistance / 20000.0) * num1) / 1000f;
        if ((double) blockMaterialDataXml.XPAdjust != 0.0)
          num2 *= blockMaterialDataXml.XPAdjust;
        return num2;
      }
    }

    public float CraftExp
    {
      get
      {
        int stackSize = ItemData.GetStackSize(this.ItemID);
        if (stackSize >= 5)
          return (float) (1.0 + (double) this.CraftReq / 20.0);
        return (float) (((double) (11 - stackSize) + (double) this.CraftReq * 1.39999997615814) * 0.200000002980232);
      }
    }
  }
}
