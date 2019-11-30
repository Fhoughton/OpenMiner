// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.ItemModel
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Graphics
{
  internal class ItemModel
  {
    public Item ItemID;
    private Actor owner;
    private GameInstance instance;

    public int VertexCount
    {
      get
      {
        return ItemModelManager.Cache[(int) this.ItemID].VertexCount;
      }
    }

    public int PrimitiveCount
    {
      get
      {
        return ItemModelManager.Cache[(int) this.ItemID].VertexCount / 2;
      }
    }

    public float Scale
    {
      get
      {
        return ItemModelManager.Cache[(int) this.ItemID].Scale;
      }
    }

    public Vector3 Center
    {
      get
      {
        return ItemModelManager.Cache[(int) this.ItemID].Center;
      }
    }

    public VertexBuffer VertexBuffer
    {
      get
      {
        return ItemModelManager.Cache[(int) this.ItemID].VertexBuffer;
      }
    }

    public ItemModel(GameInstance instance, Actor owner)
    {
      this.instance = instance;
      this.owner = owner;
    }

    public void ReloadModel()
    {
      this.BuildMesh();
    }

    public void Initialize(Item itemID)
    {
      if (this.ItemID == itemID)
        return;
      this.ItemID = itemID;
      this.BuildMesh();
    }

    private void BuildMesh()
    {
      if (this.ItemID == Item.None || GraphicStatics.TexturePack is TestTexturePack)
        return;
      ItemModelManager.BuildModel(this.ItemID);
    }

    public void CrumbleLeftHand()
    {
      this.Crumble(this.owner.LeftHand.ItemSwing.AnimData.CurrPosition, Vector3.Zero);
    }

    public void CrumbleRightHand()
    {
      this.Crumble(this.owner.RightHand.ItemSwing.AnimData.CurrPosition, Vector3.Zero);
    }

    private void Crumble(Vector3 itemPosition, Vector3 ypr)
    {
      if (this.ItemID == Item.None)
        return;
      ItemModelCache itemModelCache = ItemModelManager.Cache[(int) this.ItemID];
      if (itemModelCache.VertexCount <= 0)
        return;
      Vector3 viewDirection = this.owner.ViewDirection;
      Vector3 zero = Vector3.Zero;
      Vector3 vector3_1 = this.owner.EyePosition + viewDirection * 2f;
      Vector3 vector3_2 = new Vector3();
      Color[] itemColorData = GraphicStatics.TexturePack.GetItemColorData(this.ItemID);
      int num1 = GraphicStatics.TexturePack.ItemTextureSize();
      int num2 = 16;
      int num3 = num1 / num2;
      float radius = (float) ((double) itemModelCache.ItemBlockSize * 2.0 * ((double) num1 / 16.0));
      for (int index1 = 0; index1 < num1; index1 += num3)
      {
        for (int index2 = 0; index2 < num1; index2 += num3)
        {
          int index3 = index2 + index1 * num1;
          Color color = itemColorData[index3];
          if (color != Color.Transparent)
          {
            vector3_2.X = (float) (this.instance.Random.NextDouble() - 0.5) * 0.5f;
            vector3_2.Y = (float) (this.instance.Random.NextDouble() - 0.5) * 0.5f;
            vector3_2.Z = (float) (this.instance.Random.NextDouble() - 0.5) * 0.5f;
            zero.X = (float) (this.instance.Random.NextDouble() - 0.5);
            zero.Y = (float) (this.instance.Random.NextDouble() * 0.5);
            zero.Z = (float) (this.instance.Random.NextDouble() - 0.5);
            zero += viewDirection;
            this.instance.ParticleManager.AddBlockNew(3f, vector3_2 + vector3_1, zero, ypr, radius, color, this.instance.ParticleModifiers.ItemCrumbleParticleModifier);
          }
        }
      }
    }
  }
}
