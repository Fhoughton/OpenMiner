// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMMap
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner.API
{
  /// <summary>
  /// Total Miner maps are made up of regions. Each region is made up of chunks.
  /// Typically you don't have to deal directly with regions or chunks.
  /// Chunks are compressed in RAM using Run Length Encoded compression to minimize the games RAM footprint.
  /// If the data in a chunk is being edited (block id, light data, aux data), the chunk is first uncompressed into a plain x,y,z array.
  /// The array of chunk data is cached so multiple edits do not require subsequent uncompressions. The game releases the cache after some time of no edits.
  /// The game automatically handles the compression/uncompression/caching of chunk data.
  /// Typical Get methods such as GetBlockID, GetLight, GetAux will first check if an uncompressed cache of the chunk data exists, and if not, will uncompress the data and cache it, before returning the data you request.
  /// For the first uncompress this obviously has both time and RAM penalties. Subsequent calls will be fast, until the game releases the cache.
  /// For this reason it is not a good idea to access (uncompress) too many chunks at one time. RAM usage will sky rocket, potentially causing out of memory errors.
  /// For this reason there are Get...NoCache alternative methods for the Get methods. These NoCache methods will not uncompress/cache the chunk data while performing the query.
  /// If the chunk data is already uncompressed it will read from that, otherwise it will read directly from the RLE stream. Depending on the complexity of the stream, reads can be
  /// slower than reading from an uncompressed cache, but the RAM savings usually make up for that.
  /// Rule of thumb, if you are just doing the occasional queries for a few blocks, or you are querying blocks dispersed all over the map then then use the NoCache methods.
  /// If you are querying many blocks within a close proximity, use the Get methods.
  /// </summary>
  public interface ITMMap
  {
    /// <summary>The size of a map tile (in meters).</summary>
    float TileSize { get; }

    /// <summary>Force mesh rebuilds after map block changes.</summary>
    void Commit();

    /// <summary>
    /// Check if a voxel location is within the loaded bounds of a map.
    /// </summary>
    /// <param name="p">Voxel location</param>
    /// <returns>True if the location is within the map bounds, otherwise false.</returns>
    bool IsValidPoint(GlobalPoint3D p);

    /// <summary>
    /// Get an integral voxel location from a map position vector.
    /// </summary>
    GlobalPoint3D GetPoint(Vector3 pos);

    /// <summary>
    /// Get a map position vector from an integral voxel location.
    /// </summary>
    /// <returns> The top north west corner of the voxel location.</returns>
    Vector3 GetPosition(GlobalPoint3D p);

    /// <summary>
    /// Get a map position vector from an integral voxel location.
    /// </summary>
    /// <returns> The center of the voxel location.</returns>
    Vector3 GetBlockCenter(GlobalPoint3D p);

    /// <summary>
    /// Get the Block ID at a map position.
    /// If there is no uncompressed cache for the chunk that contains the block, the chunk is uncompressed and cached before the Block ID is read.
    /// This will be a slow operation if the chunk is not already uncompressed and will consume 32 x 32 x 32 bytes of RAM, but subsequent calls will be faster, until the cache is discarded by the game.
    /// </summary>
    Block GetBlockID(Vector3 pos);

    /// <summary>
    /// Get the Block ID at an integral voxel location.
    /// If there is no uncompressed cache for the chunk that contains the block, the chunk is uncompressed and cached before the Block ID is read.
    /// This will be a slow operation if the chunk is not already uncompressed and will consume 32 x 32 x 32 bytes of RAM, but subsequent calls will be faster, until the cache is discarded by the game.
    /// </summary>
    Block GetBlockID(GlobalPoint3D p);

    /// <summary>
    /// Get the Block ID at an integral voxel location.
    /// If there is no uncompressed cache for the chunk that contains the block, Block ID is read from the compressed stream.
    /// This call will not consume RAM to store a cache, and will not cause a chunk uncompression, so there is no first up penalty if the chunk not uncompressed. But generally, reading Block ID's from an uncompressed stream is slower than reading Block ID's from a compressed cache. It depends how complex the chunk is. If you plan to only make a few reads from the chunk, this method should be fine. If you plan to make hundreds or thousands of reads, it might be better to use the method that uncompresses and caches the chunk.
    /// </summary>
    Block GetBlockIDNoCache(GlobalPoint3D p);

    /// <summary>
    /// Get the Block ID, Light and Aux data at an integral voxel location.
    /// If there is no uncompressed cache for the chunk that contains the block, the chunk is uncompressed and cached before the Block data is read.
    /// This will be a slow operation if the chunk is not already uncompressed and will consume 32 x 32 x 32 x 3 bytes of RAM, but subsequent calls will be faster, until the cache is discarded by the game.
    /// </summary>
    MapBlock GetBlockData(GlobalPoint3D p);

    /// <summary>
    /// Get the Block ID and Aux data at an integral voxel location.
    /// This method is slightly faster than calling GetBlockID and GetAuxData separately, and is faster than calling GetBlockData.
    /// If there is no uncompressed cache for the chunk that contains the block, the chunk is uncompressed and cached before the Block data is read.
    /// </summary>
    MapBlock GetBlockIDAndAux(GlobalPoint3D p);

    /// <summary>
    /// Get the Block ID and Aux data at an integral voxel location.
    /// This method is slightly faster than calling GetBlockIDNoCache and GetAuxDataNoCache separately.
    /// If there is no uncompressed cache for the chunk that contains the block, Block ID and Aux is read from the compressed streams.
    /// </summary>
    MapBlock GetBlockIDAndAuxNoCache(GlobalPoint3D p);

    /// <summary>
    /// Get the Block ID and Light data at an integral voxel location.
    /// This method is slightly faster than calling GetBlockID and GetBlockLight separately, and is faster than calling GetBlockData.
    /// If there is no uncompressed cache for the chunk that contains the block, the chunk is uncompressed and cached before the Block data is read.
    /// </summary>
    MapBlock GetBlockAndLight(GlobalPoint3D p);

    /// <summary>
    /// Compares if the block data at a map position matches inputs.
    /// </summary>
    /// <param name="pos">The map position of the block to compare.</param>
    /// <param name="blockID">The Block ID to compare.</param>
    /// <param name="aux">The Aux data to compare.</param>
    /// <returns>True of the Block ID and Aux data at the map position matches the blockID and aux parameters above.</returns>
    bool IsBlockDataEqual(Vector3 pos, Block blockID, byte aux);

    void SetBlockData(
      GlobalPoint3D p,
      Block blockID,
      byte auxData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit);

    void SetBlockData(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock newBlockData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit);

    ClearBlockResult ClearBlock(
      GlobalPoint3D p,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit);

    byte GetAuxData(GlobalPoint3D p);

    byte GetAuxDataNoCache(GlobalPoint3D p);

    byte GetAuxHighData(GlobalPoint3D p);

    byte GetAuxHighDataNoCache(GlobalPoint3D p);

    byte GetAuxFullData(GlobalPoint3D p);

    byte GetAuxFullDataNoCache(GlobalPoint3D p);

    /// <summary>
    /// Tests if the auxData indicates the source block has been edited.
    /// </summary>
    /// <param name="auxData">Source block auxData.</param>
    /// <returns>Returns true of the auxData indicates the source block has been edited, i.e. the block is not the originally generated block.</returns>
    bool HasChanged(byte auxData);

    /// <summary>
    /// Tests if the blockData.AuxData indicates the source block has been edited.
    /// </summary>
    /// <param name="blockData">Source block data.</param>
    /// <returns>Returns true of the data indicates the source block has been edited, i.e. the block is not the originally generated block.</returns>
    bool HasChanged(MapBlock blockData);

    /// <summary>
    /// Tests if the block at the voxel location has been edited.
    /// </summary>
    /// <param name="p">Voxel location.</param>
    /// <returns>Returns true of the block at the voxel location has been edited, i.e. the block is not the originally generated block.</returns>
    bool HasChanged(GlobalPoint3D p);

    void SetAuxData(
      GlobalPoint3D p,
      byte auxData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit);

    void SetAuxData(
      GlobalPoint3D p,
      byte oldAuxData,
      byte auxData,
      UpdateBlockMethod method,
      GamerID gamerID,
      bool transmit);

    MapLight GetLight(GlobalPoint3D p);

    MapLight GetLightNoCache(GlobalPoint3D p);

    /// <summary>
    /// Get the amount of sunlight at a voxel location. Sunlight can have a value from 0 to 15.
    /// This value of sun light is affected by the current time of day.
    /// </summary>
    byte GetSunLight(GlobalPoint3D p);

    /// <summary>
    /// Get the amount of blocklight at a voxel location. Blocklight can have a value from 0 to 15.
    /// Block light is light emitted from light emitting blocks, such as torches, sun blocks, etc.
    /// Block light is not affected by the current time of day.
    /// </summary>
    byte GetBlockLight(GlobalPoint3D p);

    /// <summary>
    /// Get the highest light data from a voxels adjacent locations. Excludes p and diagonally adjacent voxels.
    /// </summary>
    MapLight GetMaxNeighbourLight(GlobalPoint3D p);

    /// <summary>
    /// Get the highest light data from a voxels adjacent locations. Excludes p, op and diagonally adjacent voxels.
    /// </summary>
    MapLight GetMaxNeighbourLight(GlobalPoint3D p, GlobalPoint3D op);

    /// <summary>
    /// Get the highest sunlight value from a voxels adjacent locations. Excludes p, op and diagonally adjacent voxels.
    /// </summary>
    byte GetMaxNeighbourSunLight(GlobalPoint3D p, GlobalPoint3D op);

    /// <summary>
    /// Get the highest block light value from a voxels adjacent locations. Excludes p, op and diagonally adjacent voxels.
    /// </summary>
    byte GetMaxNeighbourBlockLight(GlobalPoint3D p, GlobalPoint3D op);

    /// <summary>
    /// Return true of the voxel location is either ground level or above.
    /// </summary>
    bool CanBlockSeeTheSky(GlobalPoint3D p);

    /// <summary>Get the normalized value of a raw light value.</summary>
    float GetLightNormalized(byte light);

    /// <summary>
    /// Get the normalized value of light at a voxel location.
    /// The value returned could be either sunlight or block light, whichever is higher.
    /// </summary>
    float GetLightNormalized(GlobalPoint3D p);

    /// <summary>
    /// Get the normalized value of light from block data.
    /// The value returned could be either sunlight or block light, whichever is higher.
    /// </summary>
    float GetLightNormalized(MapBlock data);

    /// <summary>
    /// Get the normalized value of light from light data.
    /// The value returned could be either sun light or block light, whichever is higher.
    /// </summary>
    float GetLightNormalized(MapLight data);

    /// <summary>
    /// Get the normalized value of sunlight from a voxel location.
    /// This value of sunlight is affected by the current time of day.
    /// </summary>
    float GetSunLightNormalized(GlobalPoint3D p);

    /// <summary>
    /// Get the normalized value of block light from a voxel location.
    /// Block light is light emitted from light emitting blocks, such as torches, sun blocks, etc.
    /// Block light is not affected by the current time of day.
    /// </summary>
    float GetBlockLightNormalized(GlobalPoint3D p);

    /// <summary>
    /// Get the normalized values of both block light and sun light from a voxel location.
    /// </summary>
    /// <returns>X = normalized sunlight, Y = normalized block light.</returns>
    Vector2 GetSunAndBlockLightNormalized(GlobalPoint3D p);

    /// <summary>
    /// Get a blocks ITMInventory object. Certain types of blocks in the game have inventory, such as chests, safes, etc.
    /// </summary>
    /// <param name="p">Voxel location</param>
    /// <param name="gamerID">ID of gamer who owns the block inventory. Only valid for locked chests. If the block at the location is not a locked chest, pass in GamerID.Sys1</param>
    /// <param name="createIfNotExist">By default, if the blocks inventory is empty, this method will return null. If you require the inventory object even if it is empty, pass TRUE, otherwise pass FALSE.</param>
    /// <returns></returns>
    ITMInventory GetBlockInventory(
      GlobalPoint3D p,
      GamerID gamerID,
      bool createIfNotExist);

    /// <summary>
    /// Get the data block associated with a block id at a voxel location.
    /// If the data block doesn't exist, create it.
    /// Some blocks require extra data, such as chests for inventory.
    /// Data blocks are used to store this extra data.
    /// </summary>
    /// <param name="p">Voxel location.</param>
    /// <returns></returns>
    DataBlock GetOrAddDataBlock(GlobalPoint3D p);
  }
}
