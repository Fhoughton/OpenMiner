// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMTexturePack
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.API
{
  public interface ITMTexturePack
  {
    /// <summary>Get the name of the texture pack.</summary>
    string Name { get; }

    /// <summary>Get the Blocks Texture2D object.</summary>
    Texture2D BlockTexture { get; }

    /// <summary>
    /// Get the Blocks Level of Detail Texture2D object.
    /// BlockTextureLOD = BlockTexture for SD texture packs.
    /// </summary>
    Texture2D BlockTextureLOD { get; }

    /// <summary>Get the Items Texture2D object.</summary>
    Texture2D ItemTexture { get; }

    /// <summary>Get the Light Map Texture2D object.</summary>
    Texture2D LightMapTexture { get; }

    /// <summary>Get the Night time Light Map Texture2D object.</summary>
    Texture2D NightLightMapTexture { get; }

    /// <summary>
    /// Get the size in pixels of a block on the current texture pack.
    /// </summary>
    int BlockTextureSize();

    /// <summary>
    /// Get the size in pixels of an item on the current texture pack.
    /// </summary>
    int ItemTextureSize();

    /// <summary>
    /// Get the correct texture for an ItemID. If ItemID is &gt;= Item.Hand then ItemTexture is returned, otherwise BlockTexture is returned.
    /// </summary>
    Texture2D GetTexureForItem(Item itemID);

    /// <summary>
    /// Returns the source rectangle for the item's texture in either BlockTexture or ItemTexture.
    /// </summary>
    Rectangle ItemSrcRect(Item itemID);

    /// <summary>
    /// Get Color data for a block. The array returned is a cached array and reused for each call, so you must use the data or copy it before calling this method again.
    /// </summary>
    Color[] GetBlockColorData(Block blockID);

    /// <summary>
    /// Get Color data for an item. The array returned is a cached array and reused for each call, so you must use the data or copy it before calling this method again.
    /// </summary>
    Color[] GetItemColorData(Item itemID);
  }
}
