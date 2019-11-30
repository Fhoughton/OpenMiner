// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.VoxelMeshBuilder
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class VoxelMeshBuilder : ITMMeshBuilder
  {
    private static StudioForge.Engine.Core.Pool<VoxelMeshBuilder.CustomVertexArray<VertexMapBlock>> newFormatPool = new StudioForge.Engine.Core.Pool<VoxelMeshBuilder.CustomVertexArray<VertexMapBlock>>(12, 1, true);
    public static StudioForge.Engine.Core.Pool<VoxelMeshBuilder> Pool = new StudioForge.Engine.Core.Pool<VoxelMeshBuilder>();
    public static object VBSemaphore = new object();
    private int lastTCItemID = -1;
    private VoxelMeshBuilder.RleCache[] chunkCaches = new VoxelMeshBuilder.RleCache[27];
    private Point3D[] splitFrom = new Point3D[8];
    private Point3D[] splitTo = new Point3D[8];
    private Point3D[] splitCenter = new Point3D[8];
    private List<int> splitUpdateSequence = new List<int>((IEnumerable<int>) new int[8]
    {
      0,
      1,
      2,
      3,
      4,
      5,
      6,
      7
    });
    private Matrix[] torchTransform = new Matrix[5];
    private Matrix[] rotMatTexCoords = new Matrix[3]
    {
      Matrix.CreateRotationZ(-1.570796f),
      Matrix.CreateRotationZ(3.141593f),
      Matrix.CreateRotationZ(1.570796f)
    };
    private const byte torchID = 46;
    private const byte ivyID = 120;
    private const byte ladderID = 47;
    private const byte ropeID = 72;
    private const byte signID = 117;
    private const byte paneID = 75;
    private const byte fenceID = 139;
    private const byte tableID = 158;
    private const byte postID = 77;
    private const byte post2ID = 186;
    private const byte sidePostID = 187;
    private const byte sidePost2ID = 188;
    private const byte cornerBlockID = 189;
    private const byte cornerBlock2ID = 190;
    private const byte bedrockID = 29;
    private const byte bedheadID = 135;
    private const byte bedfootID = 136;
    private const byte obsidianID = 31;
    private const byte stackID = 130;
    private const byte stack2ID = 185;
    private const byte upsideDownStackID = 131;
    private const byte snowLayerID = 145;
    private const byte cropID = 124;
    private const byte coverBlockID = 126;
    private const byte pressurePlateID = 165;
    private const byte arcadeMachineID = 128;
    private const byte paintingID = 160;
    private const byte switchID = 169;
    private const byte buttonID = 170;
    private const byte rampID = 150;
    private const byte ramp2ID = 184;
    private const byte bookID = 121;
    private const byte fireID = 118;
    private const byte lavaID = 13;
    private const byte waterID = 11;
    private const byte stairsID = 113;
    private const byte stairs2ID = 182;
    private const byte cylinderID = 154;
    private const byte spikesID = 119;
    private const byte halfBlockID = 149;
    private const byte halfBlock2ID = 183;
    private const byte trapDoorID = 171;
    private const byte woodDoorTopID = 51;
    private const byte woodDoorBottomID = 115;
    private const byte steelDoorTopID = 52;
    private const byte steelDoorBottomID = 116;
    private const byte lockedDoorTopID = 140;
    private const byte lockedDoorBottomID = 204;
    private const byte invisibleBarrierID = 125;
    private const byte cloudID = 10;
    private const byte stainedGlassID = 181;
    private const byte stainedGlassPaneID = 76;
    private const byte teleportID = 53;
    private const byte poweredLightID = 166;
    private const byte oneWayGlassID = 153;
    private const byte zLastBlockID = 255;
    public const float WaterSurfacePosYOffset = 0.16f;
    private const int cylinderTessellation = 16;
    private const int leftFace = 0;
    private const int forwardFace = 1;
    private const int rightFace = 2;
    private const int backFace = 3;
    private const int upFace = 4;
    private const int downFace = 5;
    private const int proxyFace = 6;
    private const int vbExtraVertexCount = 100;
    private const float topLiquidHeight = 0.9f;
    private const float lowLiquidHeight = 0.05f;
    private const float waterStep = 0.1214286f;
    private const float lavaStep = 0.17f;
    private const float oneEightScale = 0.125f;
    private const float rotXFac = 2.461539f;
    private byte steelSpikesDefaultTextureID;
    private static int vertexPoolMaxUsed;
    public bool IncludeBlockModels;
    public byte CurrentLight;
    private MapTM map;
    private MapChunk chunk;
    private float tilesize;
    private float halftilesize;
    private Vector4 tc;
    private Vector3 chunkVectorOffset;
    private Point3D chunksize;
    private GlobalPoint3D chunkPointOffset;
    private BoxInt mapBound;
    private bool oldskoolLight;
    private bool isWaterVertex;
    private VertexMapBlock newVertex;
    private GameInstance instance;
    private MapStrategyTM strategy;
    private VoxelMeshBuilder.CustomVertexArray<VertexMapBlock> newFormatVertices;
    private VoxelMeshBuilder.CustomVertexArray<VertexMapBlock> newFormatWaterVertices;
    private VoxelMeshBuilder.RleCache baseRleCache;
    private VoxelMeshBuilder.lightHelper[] getFaceLight;
    private Point3D splitSortPoint;
    private Comparison<int> sortSplitUpdateSequence;
    private GlobalPoint3D mapIndexCenter;
    private float outerFireFaceLean;
    private float innerFireFaceLean;
    private static Vector3[] cylinderVertices;
    private static VoxelMeshBuilder.CylinderCapData[] cylinderCapVertices;
    private BlockDataXML blockData;
    private bool isWindAffected;
    private byte[] xxHashBuff;
    private int[,] rotTexFace;
    private Vector3[] tempCylPos;
    private VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1[] SetFacePos1Array;
    private VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2[] SetFacePos2Array;
    private bool skipUpFace;

    Vector2[] ITMMeshBuilder.TexCoords1
    {
      get
      {
        return MapChunkContent.TexCoords1;
      }
    }

    Vector2[] ITMMeshBuilder.TexCoords2
    {
      get
      {
        return MapChunkContent.TexCoords2;
      }
    }

    Vector2[] ITMMeshBuilder.TexCoords3
    {
      get
      {
        return MapChunkContent.TexCoords3;
      }
    }

    Vector2[] ITMMeshBuilder.TexCoords4
    {
      get
      {
        return MapChunkContent.TexCoords4;
      }
    }

    void ITMMeshBuilder.AddVertex(
      float x,
      float y,
      float z,
      int face,
      float tx,
      float ty,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p)
    {
      this.AddVertex(x, y, z, face, tx, ty, blockID, aux, ref p);
    }

    void ITMMeshBuilder.AddVertex(
      Vector3 pos,
      int face,
      float tx,
      float ty,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p)
    {
      this.AddVertex(pos, face, tx, ty, blockID, aux, ref p);
    }

    void ITMMeshBuilder.AddVertex(
      float x,
      float y,
      float z,
      int face,
      NormalizedShort2 tc,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p)
    {
      this.AddVertex(x, y, z, face, tc, blockID, aux, ref p);
    }

    void ITMMeshBuilder.AddVertex(ref AVParams data)
    {
      this.AddVertex(ref data);
    }

    bool ITMMeshBuilder.IsClear(GlobalPoint3D p, byte blockID, int aux, int face)
    {
      return this.IsClear(p, blockID, aux, face);
    }

    void ITMMeshBuilder.RotateTexCoords(
      ref GlobalPoint3D p,
      byte face,
      ref Vector2 tc1,
      ref Vector2 tc2,
      ref Vector2 tc3,
      ref Vector2 tc4)
    {
      this.RotateTexCoords(ref p, face, ref tc1, ref tc2, ref tc3, ref tc4);
    }

    public static int VertexPoolCount
    {
      get
      {
        return VoxelMeshBuilder.newFormatPool.List.Length;
      }
    }

    public static int VertexPoolUsed
    {
      get
      {
        return VoxelMeshBuilder.newFormatPool.List.Length;
      }
    }

    public static int VertexPoolMaxUsed
    {
      get
      {
        if (VoxelMeshBuilder.vertexPoolMaxUsed < VoxelMeshBuilder.newFormatPool.UsedCount)
          VoxelMeshBuilder.vertexPoolMaxUsed = VoxelMeshBuilder.newFormatPool.UsedCount;
        return VoxelMeshBuilder.vertexPoolMaxUsed;
      }
    }

    public static void ReleasePools()
    {
      VoxelMeshBuilder.Pool.ReleaseAll();
      VoxelMeshBuilder.newFormatPool.ReleaseAll();
    }

    public static int VertexPoolSize
    {
      get
      {
        int num = 0;
        foreach (VoxelMeshBuilder.CustomVertexArray<VertexMapBlock> customVertexArray in VoxelMeshBuilder.newFormatPool.List)
        {
          if (customVertexArray != null)
            num += customVertexArray.Array.Length * VertexMapBlock.vertexDeclaration.VertexStride;
        }
        return num;
      }
    }

    public VoxelMeshBuilder()
    {
      this.sortSplitUpdateSequence = new Comparison<int>(this.SortSplitUpdateSequence);
      lock (VoxelMeshBuilder.newFormatPool)
      {
        int next1 = VoxelMeshBuilder.newFormatPool.GetNext();
        this.newFormatVertices = VoxelMeshBuilder.newFormatPool.List[next1];
        int next2 = VoxelMeshBuilder.newFormatPool.GetNext();
        this.newFormatWaterVertices = VoxelMeshBuilder.newFormatPool.List[next2];
      }
      this.SetFacePos1Array = new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1[6]
      {
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1(this.SetLeftFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1(this.SetForwardFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1(this.SetRightFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1(this.SetBackwardFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1(this.SetUpFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos1(this.SetDownFacePos)
      };
      this.SetFacePos2Array = new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2[6]
      {
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2(this.SetLeftFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2(this.SetForwardFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2(this.SetRightFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2(this.SetBackwardFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2(this.SetUpFacePos),
        new VoxelMeshBuilder.CaseStatementToArrayLookupSetFacePos2(this.SetDownFacePos)
      };
      this.getFaceLight = new VoxelMeshBuilder.lightHelper[6]
      {
        new VoxelMeshBuilder.lightHelper(this.GetLeftFaceLight),
        new VoxelMeshBuilder.lightHelper(this.GetForwardFaceLight),
        new VoxelMeshBuilder.lightHelper(this.GetRightFaceLight),
        new VoxelMeshBuilder.lightHelper(this.GetBackwardFaceLight),
        new VoxelMeshBuilder.lightHelper(this.GetUpFaceLight),
        new VoxelMeshBuilder.lightHelper(this.GetDownFaceLight)
      };
    }

    public void Initialize(GameInstance instance)
    {
      this.instance = instance;
      if (VoxelMeshBuilder.cylinderVertices == null)
        this.BuildStaticCylinderVertices();
      if (this.xxHashBuff == null)
        this.xxHashBuff = new byte[3];
      this.rotTexFace = new int[7, 4];
      this.rotTexFace[0, 0] = 0;
      this.rotTexFace[2, 0] = 2;
      this.rotTexFace[1, 0] = 1;
      this.rotTexFace[3, 0] = 3;
      this.rotTexFace[4, 0] = 4;
      this.rotTexFace[5, 0] = 5;
      this.rotTexFace[6, 0] = 6;
      this.rotTexFace[0, 1] = 4;
      this.rotTexFace[2, 1] = 5;
      this.rotTexFace[1, 1] = 1;
      this.rotTexFace[3, 1] = 3;
      this.rotTexFace[4, 1] = 0;
      this.rotTexFace[5, 1] = 2;
      this.rotTexFace[6, 1] = 6;
      this.rotTexFace[0, 2] = 0;
      this.rotTexFace[2, 2] = 2;
      this.rotTexFace[1, 2] = 4;
      this.rotTexFace[3, 2] = 5;
      this.rotTexFace[4, 2] = 1;
      this.rotTexFace[5, 2] = 3;
      this.rotTexFace[6, 2] = 6;
    }

    public void UnloadContent()
    {
      lock (VoxelMeshBuilder.newFormatPool)
      {
        VoxelMeshBuilder.newFormatPool.Release(this.newFormatVertices);
        VoxelMeshBuilder.newFormatPool.Release(this.newFormatWaterVertices);
      }
    }

    private void BuildStaticCylinderVertices()
    {
      List<Vector3> vector3List = new List<Vector3>();
      float radius = 0.47f;
      for (int i = 0; i < 16; ++i)
      {
        Vector3 circleVector1 = this.GetCircleVector(i, 16);
        Vector3 circleVector2 = this.GetCircleVector(i + 1 == 16 ? 0 : i + 1, 16);
        vector3List.Add(circleVector2 * radius + Vector3.Down * 0.5f);
        vector3List.Add(circleVector2 * radius + Vector3.Up * 0.5f);
        vector3List.Add(circleVector1 * radius + Vector3.Up * 0.5f);
        vector3List.Add(circleVector1 * radius + Vector3.Down * 0.5f);
      }
      this.CreateCap(0.5f, radius, Vector3.Up);
      VoxelMeshBuilder.cylinderVertices = vector3List.ToArray();
    }

    private void CreateCap(float y, float radius, Vector3 normal)
    {
      List<VoxelMeshBuilder.CylinderCapData> cylinderCapDataList = new List<VoxelMeshBuilder.CylinderCapData>();
      for (int i = 0; i < 15; i += 2)
      {
        Vector3 vector3_1 = this.GetCircleVector(0, 16) * radius + normal;
        vector3_1.Y = y;
        cylinderCapDataList.Add(new VoxelMeshBuilder.CylinderCapData()
        {
          Position = vector3_1,
          TexCoord = new Vector2(0.0f, 0.0f)
        });
        Vector3 vector3_2 = this.GetCircleVector(i, 16) * radius + normal;
        vector3_2.Y = y;
        cylinderCapDataList.Add(new VoxelMeshBuilder.CylinderCapData()
        {
          Position = vector3_2,
          TexCoord = new Vector2(0.0f, 0.0f)
        });
        Vector3 vector3_3 = this.GetCircleVector(i + 1, 16) * radius + normal;
        vector3_3.Y = y;
        cylinderCapDataList.Add(new VoxelMeshBuilder.CylinderCapData()
        {
          Position = vector3_3,
          TexCoord = new Vector2(0.0f, 0.0f)
        });
        Vector3 vector3_4 = this.GetCircleVector(i + 2 >= 16 ? 0 : i + 2, 16) * radius + normal;
        vector3_4.Y = y;
        cylinderCapDataList.Add(new VoxelMeshBuilder.CylinderCapData()
        {
          Position = vector3_4,
          TexCoord = new Vector2(0.0f, 0.0f)
        });
      }
      VoxelMeshBuilder.cylinderCapVertices = cylinderCapDataList.ToArray();
    }

    private Vector3 GetCircleVector(int i, int tessellation)
    {
      float num = (float) i * 6.283185f / (float) tessellation;
      return Vector3.Transform(new Vector3((float) Math.Cos((double) num), 0.0f, (float) Math.Sin((double) num)), Matrix.CreateRotationY((float) (6.28318548202515 / (double) tessellation * 0.5)));
    }

    public void BuildChunkMesh_NewFormat(
      MapChunkTM chunk,
      bool oldskoolLight,
      bool addExtraVerts,
      IProgressBar progressBar)
    {
      try
      {
        this.BuildChunkMeshCore_NewFormat_Begin(chunk, oldskoolLight);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(123, ex);
        throw ex;
      }
      this.newFormatVertices.Count = 0;
      this.newFormatWaterVertices.Count = 0;
      try
      {
        this.BuildChunkMeshAroundSolidBlocks(chunk.Content, progressBar);
        VoxelMeshBuilder.MeshVertexData data = new VoxelMeshBuilder.MeshVertexData()
        {
          VertexData = (CustomArray<VertexMapBlock>) this.newFormatVertices,
          WaterVertexData = (CustomArray<VertexMapBlock>) this.newFormatWaterVertices
        };
        try
        {
          chunk.Content.ContentData = this.UpdateNewVertexContent(data, chunk.Content.ContentData, addExtraVerts);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(125, ex);
          throw ex;
        }
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(124, ex);
        throw ex;
      }
      finally
      {
        chunk.Content.RemoveBreakdown(chunk);
        this.ClearChunkRLECaches();
      }
    }

    public void BuildChunkMeshSplit_NewFormat(MapChunkTM chunk, bool oldskoolLight)
    {
      this.BuildChunkMeshCore_NewFormat_Begin(chunk, oldskoolLight);
      MapChunkContent content = chunk.Content;
      MapTM map = chunk.Region.Map as MapTM;
      long globalHashCode = chunk.GetGlobalHashCode();
      MapChunkContentData[] chunkContentDataArray;
      lock (map.MapChunkContentBreakdown)
      {
        if (!map.MapChunkContentBreakdown.TryGetValue(globalHashCode, out chunkContentDataArray))
        {
          int next = map.MapChunkContentBreakdownPool.GetNext();
          chunkContentDataArray = map.MapChunkContentBreakdownPool.List[next];
          for (int index = 0; index < chunkContentDataArray.Length; ++index)
            chunkContentDataArray[index] = new MapChunkContentData();
          map.MapChunkContentBreakdown.Add(globalHashCode, chunkContentDataArray);
        }
      }
      try
      {
        Point3D p = chunk.LastBlockEditedIndex >= 0 ? chunk.GetPoint(chunk.LastBlockEditedIndex) : Point3D.Zero;
        chunk.LastBlockEditedIndex = -1;
        this.SetSplitUpdateSequence(p, chunk.UpdateFlags);
        chunk.ClearUpdateFlag(MapChunk.segmentFlags);
        foreach (int index in this.splitUpdateSequence)
        {
          VoxelMeshBuilder.MeshVertexData data = this.BuildChunkMeshSplitCore_NewFormat(content, this.splitFrom[index], this.splitTo[index]);
          MapChunkContentData result;
          lock (chunkContentDataArray)
            result = chunkContentDataArray[index];
          result = this.UpdateNewVertexContent(data, result);
          lock (chunkContentDataArray)
            chunkContentDataArray[index] = result;
        }
        content.SetContentFlag(MapChunkContentFlags.ContentSplit);
      }
      finally
      {
        this.ClearChunkRLECaches();
      }
    }

    private void BuildChunkMeshCore_NewFormat_Begin(MapChunkTM chunk, bool oldskoolLight)
    {
      this.chunk = (MapChunk) chunk;
      this.oldskoolLight = oldskoolLight;
      this.chunkPointOffset = chunk.GlobalOffset;
      this.chunkVectorOffset = this.chunkPointOffset.ToVector3();
      this.mapBound = chunk.Region.Map.MapBound;
      this.mapIndexCenter = GlobalPoint3D.Negate(this.mapBound.Min);
      if (chunk.Region.Map != this.map)
        this.InitializeForNewMap(chunk.Region.Map);
      for (int index = 0; index < this.chunkCaches.Length; ++index)
        this.chunkCaches[index].Chunk = (MapChunk) null;
      this.baseRleCache.Chunk = (MapChunk) null;
      this.BuildCache((MapChunk) chunk, ref this.baseRleCache);
    }

    private VoxelMeshBuilder.MeshVertexData BuildChunkMeshSplitCore_NewFormat(
      MapChunkContent chunkContent,
      Point3D from,
      Point3D to)
    {
      this.newFormatVertices.Count = 0;
      this.newFormatWaterVertices.Count = 0;
      this.BuildChunkMeshAroundSolidBlocks(chunkContent, from, to);
      return new VoxelMeshBuilder.MeshVertexData()
      {
        VertexData = (CustomArray<VertexMapBlock>) this.newFormatVertices,
        WaterVertexData = (CustomArray<VertexMapBlock>) this.newFormatWaterVertices
      };
    }

    private void InitializeForNewMap(Map map)
    {
      this.map = map as MapTM;
      this.strategy = map.MapStrategy as MapStrategyTM;
      int blockTextureIndex = this.map.GetBlockTextureIndex(Block.SteelSpikes);
      this.steelSpikesDefaultTextureID = blockTextureIndex < 0 || blockTextureIndex >= this.map.BlockTextures.GetLength(0) ? (byte) 239 : (byte) this.map.BlockTextures[blockTextureIndex, 0];
      if ((double) this.tilesize != (double) map.TileSize)
      {
        this.tilesize = map.TileSize;
        this.halftilesize = this.tilesize * 0.5f;
        this.outerFireFaceLean = this.tilesize * 0.2f;
        this.innerFireFaceLean = this.tilesize * 0.3f;
        this.torchTransform[0] = Matrix.CreateRotationZ(0.4f) * Matrix.CreateTranslation(this.tilesize * 0.3f, 0.1f, 0.0f);
        this.torchTransform[1] = Matrix.CreateRotationX(-0.4f) * Matrix.CreateTranslation(0.0f, 0.1f, this.tilesize * 0.3f);
        this.torchTransform[2] = Matrix.CreateRotationZ(-0.4f) * Matrix.CreateTranslation((float) (-(double) this.tilesize * 0.300000011920929), 0.1f, 0.0f);
        this.torchTransform[3] = Matrix.CreateRotationX(0.4f) * Matrix.CreateTranslation(0.0f, 0.1f, (float) (-(double) this.tilesize * 0.300000011920929));
        this.torchTransform[4] = Matrix.Identity;
      }
      if (!(this.chunksize != map.ChunkSize))
        return;
      this.chunksize = map.ChunkSize;
      Point3D zero1 = Point3D.Zero;
      Point3D zero2 = Point3D.Zero;
      Point3D zero3 = Point3D.Zero;
      zero3.X = this.chunksize.X / 2;
      zero3.Y = this.chunksize.Y / 2;
      zero3.Z = this.chunksize.Z / 2;
      Point3D point3D = zero1 + zero3;
      this.splitFrom[0] = zero1;
      this.splitTo[0] = point3D;
      this.splitCenter[0] = (point3D - zero1) / 2 + zero1;
      zero1.X += zero3.X;
      point3D.X += zero3.X;
      this.splitFrom[1] = zero1;
      this.splitTo[1] = point3D;
      this.splitCenter[1] = (point3D - zero1) / 2 + zero1;
      zero1.Z += zero3.Z;
      point3D.Z += zero3.Z;
      this.splitFrom[2] = zero1;
      this.splitTo[2] = point3D;
      this.splitCenter[2] = (point3D - zero1) / 2 + zero1;
      zero1.X -= zero3.X;
      point3D.X -= zero3.X;
      this.splitFrom[3] = zero1;
      this.splitTo[3] = point3D;
      this.splitCenter[3] = (point3D - zero1) / 2 + zero1;
      zero1.Z -= zero3.Z;
      point3D.Z -= zero3.Z;
      zero1.Y += zero3.Z;
      point3D.Y += zero3.Z;
      this.splitFrom[4] = zero1;
      this.splitTo[4] = point3D;
      this.splitCenter[4] = (point3D - zero1) / 2 + zero1;
      zero1.X += zero3.X;
      point3D.X += zero3.X;
      this.splitFrom[5] = zero1;
      this.splitTo[5] = point3D;
      this.splitCenter[5] = (point3D - zero1) / 2 + zero1;
      zero1.Z += zero3.Z;
      point3D.Z += zero3.Z;
      this.splitFrom[6] = zero1;
      this.splitTo[6] = point3D;
      this.splitCenter[6] = (point3D - zero1) / 2 + zero1;
      zero1.X -= zero3.X;
      point3D.X -= zero3.X;
      this.splitFrom[7] = zero1;
      this.splitTo[7] = point3D;
      this.splitCenter[7] = (point3D - zero1) / 2 + zero1;
    }

    private int SortSplitUpdateSequence(int i1, int i2)
    {
      return Point3D.DistanceSquared(this.splitSortPoint, this.splitCenter[i1]).CompareTo(Point3D.DistanceSquared(this.splitSortPoint, this.splitCenter[i2]));
    }

    private MapChunkContentData UpdateNewVertexContent(
      VoxelMeshBuilder.MeshVertexData data,
      MapChunkContentData result)
    {
      return this.UpdateNewVertexContent(data, result, true);
    }

    private MapChunkContentData UpdateNewVertexContent(
      VoxelMeshBuilder.MeshVertexData data,
      MapChunkContentData result,
      bool addExtraVerts)
    {
      int num = data.VertexData.Count + data.WaterVertexData.Count;
      if (num > 0)
      {
        VertexBuffer vb;
        if (result.VertexBuffer != null && !result.VertexBuffer.IsDisposed && (result.VertexBuffer.VertexCount >= num && result.VertexBuffer.VertexCount > num - 100))
        {
          vb = result.VertexBuffer;
          ++MapChunkContent.VBRecycledCount;
        }
        else if (result.NewVertexBuffer == null || result.NewVertexBuffer.VertexCount < num || result.NewVertexBuffer.VertexCount > num + 100)
        {
          vb = new VertexBuffer(CoreGlobals.GraphicsDevice, typeof (VertexMapBlock), num + (addExtraVerts ? 100 : 0), BufferUsage.WriteOnly);
          ++MapChunkContent.VBNewCount;
        }
        else
        {
          vb = result.NewVertexBuffer;
          ++MapChunkContent.VBRecycledCount;
        }
        if (data.VertexData.Count <= 0)
        {
          if (data.WaterVertexData.Count <= 0)
            goto label_12;
        }
        try
        {
          this.SetData(vb, data);
        }
        catch (InvalidOperationException ex)
        {
          if (vb == result.VertexBuffer)
            --MapChunkContent.VBRecycledCount;
          vb = new VertexBuffer(CoreGlobals.GraphicsDevice, typeof (VertexMapBlock), num + (addExtraVerts ? 100 : 0), BufferUsage.WriteOnly);
          ++MapChunkContent.VBNewCount;
          this.SetData(vb, data);
        }
label_12:
        result.NewVertexBuffer = vb;
      }
      result.NewVertexCount = data.VertexData.Count;
      result.NewWaterVertexCount = data.WaterVertexData.Count;
      result.VertexBufferChanged = true;
      return result;
    }

    private void SetData(VertexBuffer vb, VoxelMeshBuilder.MeshVertexData data)
    {
      if (data.VertexData.Count > 0)
        vb.SetData<VertexMapBlock>(data.VertexData.Array, 0, data.VertexData.Count);
      if (data.WaterVertexData.Count <= 0)
        return;
      int vertexStride = VertexMapBlock.vertexDeclaration.VertexStride;
      int offsetInBytes = data.VertexData.Count * vertexStride;
      vb.SetData<VertexMapBlock>(offsetInBytes, data.WaterVertexData.Array, 0, data.WaterVertexData.Count, vertexStride);
    }

    private void SetSplitUpdateSequence(Point3D p, ChunkUpdateFlags updateFlags)
    {
      this.splitSortPoint = p;
      this.splitUpdateSequence.Sort(this.sortSplitUpdateSequence);
      int segment = this.splitUpdateSequence[0];
      if ((updateFlags & ChunkUpdateFlags.LeftSegmentBorder) == ChunkUpdateFlags.LeftSegmentBorder)
        this.GivePriorityToSegment(this.GetLeftSegment(segment));
      if ((updateFlags & ChunkUpdateFlags.ForwardSegmentBorder) == ChunkUpdateFlags.ForwardSegmentBorder)
        this.GivePriorityToSegment(this.GetForwardSegment(segment));
      if ((updateFlags & ChunkUpdateFlags.RightSegmentBorder) == ChunkUpdateFlags.RightChunkBorder)
        this.GivePriorityToSegment(this.GetRighttSegment(segment));
      if ((updateFlags & ChunkUpdateFlags.BackSegmentBorder) == ChunkUpdateFlags.BackSegmentBorder)
        this.GivePriorityToSegment(this.GetBackSegment(segment));
      if ((updateFlags & ChunkUpdateFlags.UpSegmentBorder) == ChunkUpdateFlags.UpSegmentBorder)
        this.GivePriorityToSegment(this.GetUpSegment(segment));
      if ((updateFlags & ChunkUpdateFlags.DownSegmentBorder) != ChunkUpdateFlags.DownSegmentBorder)
        return;
      this.GivePriorityToSegment(this.GetDownSegment(segment));
    }

    private int GetLeftSegment(int segment)
    {
      switch (segment)
      {
        case 1:
          return 0;
        case 2:
          return 3;
        case 5:
          return 4;
        case 6:
          return 7;
        default:
          return -1;
      }
    }

    private int GetForwardSegment(int segment)
    {
      switch (segment)
      {
        case 0:
          return 3;
        case 1:
          return 2;
        case 4:
          return 7;
        case 5:
          return 6;
        default:
          return -1;
      }
    }

    private int GetRighttSegment(int segment)
    {
      switch (segment)
      {
        case 0:
          return 1;
        case 3:
          return 2;
        case 4:
          return 5;
        case 7:
          return 6;
        default:
          return -1;
      }
    }

    private int GetBackSegment(int segment)
    {
      switch (segment)
      {
        case 2:
          return 1;
        case 3:
          return 0;
        case 6:
          return 5;
        case 7:
          return 4;
        default:
          return -1;
      }
    }

    private int GetUpSegment(int segment)
    {
      if (segment >= 4)
        return -1;
      return segment + 4;
    }

    private int GetDownSegment(int segment)
    {
      return segment - 4;
    }

    private void GivePriorityToSegment(int segment)
    {
      if (segment < 0)
        return;
      this.splitUpdateSequence.Remove(segment);
      this.splitUpdateSequence.Insert(0, segment);
    }

    private void ClearChunkRLECaches()
    {
      foreach (VoxelMeshBuilder.RleCache chunkCach in this.chunkCaches)
        this.RemoveCacheRefCounts(chunkCach);
      this.RemoveCacheRefCounts(this.baseRleCache);
    }

    private void RemoveCacheRefCounts(VoxelMeshBuilder.RleCache cache)
    {
      if (cache.Chunk == null)
        return;
      this.map.ChunkCacheManager.DecRefCount(cache.BlockCacheID, cache.BlockCacheIndex);
      this.map.ChunkCacheManager.DecRefCount(cache.LightCacheID, cache.LightCacheIndex);
      this.map.ChunkCacheManager.DecRefCount(cache.AuxCacheID, cache.AuxCacheIndex);
    }

    private void BuildChunkMeshAroundEmptyBlocks(MapChunkContent content)
    {
      Point3D chunkSize = this.map.ChunkSize;
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D = new Point3D();
      for (point3D.Y = 0; point3D.Y < chunkSize.Y; ++point3D.Y)
      {
        for (point3D.Z = 0; point3D.Z < chunkSize.Z; ++point3D.Z)
        {
          for (point3D.X = 0; point3D.X < chunkSize.X; ++point3D.X)
          {
            GlobalPoint3D p;
            p.X = globalOffset.X + point3D.X;
            p.Y = globalOffset.Y + point3D.Y;
            p.Z = globalOffset.Z + point3D.Z;
            byte blockId = this.GetDataBlock(ref p).BlockID;
            this.blockData = this.map.BlockData[(int) blockId];
            if (this.blockData.Buffer > (byte) 1)
              this.BuildBlockAroundEmptyNewFormat(content, ref p, blockId);
          }
        }
      }
    }

    private void BuildChunkMeshAroundSolidBlocks(MapChunkContent content, IProgressBar progressBar)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D = new Point3D();
      int chunkLength = this.map.ChunkLength;
      VoxelMeshBuilder.RleCache rleCache = this.GetRleCache(ref globalOffset);
      GlobalPoint3D p;
      p.X = globalOffset.X;
      p.Y = globalOffset.Y;
      p.Z = globalOffset.Z;
      bool flag = progressBar != null;
      float num1 = (float) (chunkLength / 100);
      for (int index = 0; index < chunkLength; ++index)
      {
        byte num2 = rleCache.BlockCache[rleCache.BlockCacheIndex + index];
        if (num2 > (byte) 0 && num2 != (byte) 125)
        {
          this.blockData = this.map.BlockData[(int) num2];
          if (this.blockData.Buffer == (byte) 0)
            this.BuildAroundSolidBlock(ref p, num2, num2);
          else
            this.BuildSpecialBlocks(p, num2);
        }
        ++p.X;
        if (++point3D.X == this.chunksize.X)
        {
          point3D.X = 0;
          p.X = globalOffset.X;
          ++p.Z;
          if (++point3D.Z == this.chunksize.Z)
          {
            point3D.Z = 0;
            p.Z = globalOffset.Z;
            ++point3D.Y;
            ++p.Y;
          }
        }
        if (flag && (double) index % (double) num1 == 0.0)
          progressBar.AddProgress(0.01f);
      }
    }

    private void BuildChunkMeshAroundSolidBlocks(MapChunkContent content, Point3D from, Point3D to)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D = new Point3D();
      for (point3D.Y = from.Y; point3D.Y < to.Y; ++point3D.Y)
      {
        GlobalPoint3D p;
        p.Y = point3D.Y + globalOffset.Y;
        for (point3D.Z = from.Z; point3D.Z < to.Z; ++point3D.Z)
        {
          p.Z = point3D.Z + globalOffset.Z;
          for (point3D.X = from.X; point3D.X < to.X; ++point3D.X)
          {
            p.X = point3D.X + globalOffset.X;
            byte dataBlockId = this.GetDataBlockID(ref p);
            if (dataBlockId > (byte) 0 && dataBlockId != (byte) 125)
            {
              this.blockData = this.map.BlockData[(int) dataBlockId];
              if (this.blockData.Buffer == (byte) 0)
                this.BuildAroundSolidBlock(ref p, dataBlockId, dataBlockId);
              else
                this.BuildSpecialBlocks(p, dataBlockId);
            }
          }
        }
      }
    }

    private void BuildChunkCenter(MapChunkContent content)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D;
      for (point3D.Y = 1; point3D.Y < this.chunksize.Y - 1; ++point3D.Y)
      {
        for (point3D.Z = 1; point3D.Z < this.chunksize.Z - 1; ++point3D.Z)
        {
          for (point3D.X = 1; point3D.X < this.chunksize.X - 1; ++point3D.X)
          {
            GlobalPoint3D p;
            p.X = globalOffset.X + point3D.X;
            p.Y = globalOffset.Y + point3D.Y;
            p.Z = globalOffset.Z + point3D.Z;
            this.BuildBlockNewFormat(content, ref p);
          }
        }
      }
    }

    private void BuildChunkStitches(MapChunkContent content)
    {
      this.BuildChunkLeftRightSide(content, Direction.Left);
      this.BuildChunkFwdBackSide(content, Direction.Forward);
      this.BuildChunkLeftRightSide(content, Direction.Right);
      this.BuildChunkFwdBackSide(content, Direction.Backward);
      this.BuildChunkUpDownSide(content, Direction.Up);
      this.BuildChunkUpDownSide(content, Direction.Down);
      this.BuildChunkVertEdge(content, Direction.Left, Direction.Forward);
      this.BuildChunkVertEdge(content, Direction.Right, Direction.Forward);
      this.BuildChunkVertEdge(content, Direction.Right, Direction.Backward);
      this.BuildChunkVertEdge(content, Direction.Left, Direction.Backward);
      this.BuildChunkHorizEdge(content, Direction.Up, Direction.Left);
      this.BuildChunkHorizEdge(content, Direction.Up, Direction.Forward);
      this.BuildChunkHorizEdge(content, Direction.Up, Direction.Right);
      this.BuildChunkHorizEdge(content, Direction.Up, Direction.Backward);
      this.BuildChunkHorizEdge(content, Direction.Down, Direction.Left);
      this.BuildChunkHorizEdge(content, Direction.Down, Direction.Forward);
      this.BuildChunkHorizEdge(content, Direction.Down, Direction.Right);
      this.BuildChunkHorizEdge(content, Direction.Down, Direction.Backward);
      this.BuildChunkCorners(content);
    }

    private void BuildChunkLeftRightSide(MapChunkContent content, Direction dir)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D;
      point3D.X = dir == Direction.Left ? 0 : this.chunksize.X - 1;
      for (point3D.Y = 1; point3D.Y < this.chunksize.Y - 1; ++point3D.Y)
      {
        for (point3D.Z = 1; point3D.Z < this.chunksize.Z - 1; ++point3D.Z)
        {
          GlobalPoint3D p;
          p.X = globalOffset.X + point3D.X;
          p.Y = globalOffset.Y + point3D.Y;
          p.Z = globalOffset.Z + point3D.Z;
          this.BuildBlockNewFormat(content, ref p);
        }
      }
    }

    private void BuildChunkFwdBackSide(MapChunkContent content, Direction dir)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D;
      point3D.Z = dir == Direction.Forward ? 0 : this.chunksize.Z - 1;
      for (point3D.Y = 1; point3D.Y < this.chunksize.Y - 1; ++point3D.Y)
      {
        for (point3D.X = 1; point3D.X < this.chunksize.X - 1; ++point3D.X)
        {
          GlobalPoint3D p;
          p.X = globalOffset.X + point3D.X;
          p.Y = globalOffset.Y + point3D.Y;
          p.Z = globalOffset.Z + point3D.Z;
          this.BuildBlockNewFormat(content, ref p);
        }
      }
    }

    private void BuildChunkUpDownSide(MapChunkContent content, Direction dir)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D;
      point3D.Y = dir == Direction.Down ? 0 : this.chunksize.Y - 1;
      for (point3D.Z = 1; point3D.Z < this.chunksize.Z - 1; ++point3D.Z)
      {
        for (point3D.X = 1; point3D.X < this.chunksize.X - 1; ++point3D.X)
        {
          GlobalPoint3D p;
          p.X = globalOffset.X + point3D.X;
          p.Y = globalOffset.Y + point3D.Y;
          p.Z = globalOffset.Z + point3D.Z;
          this.BuildBlockNewFormat(content, ref p);
        }
      }
    }

    private void BuildChunkVertEdge(MapChunkContent content, Direction dir1, Direction dir2)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D;
      point3D.X = dir1 == Direction.Left ? 0 : this.chunksize.X - 1;
      point3D.Z = dir2 == Direction.Forward ? 0 : this.chunksize.Z - 1;
      for (point3D.Y = 1; point3D.Y < this.chunksize.Y - 1; ++point3D.Y)
      {
        GlobalPoint3D p;
        p.X = globalOffset.X + point3D.X;
        p.Y = globalOffset.Y + point3D.Y;
        p.Z = globalOffset.Z + point3D.Z;
        this.BuildBlockNewFormat(content, ref p);
      }
    }

    private void BuildChunkHorizEdge(MapChunkContent content, Direction dir1, Direction dir2)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      Point3D point3D;
      point3D.Y = dir1 == Direction.Up ? 0 : this.chunksize.Y - 1;
      if (dir2 == Direction.Left || dir2 == Direction.Right)
      {
        point3D.X = dir2 == Direction.Left ? 0 : this.chunksize.X - 1;
        for (point3D.Z = 1; point3D.Z < this.chunksize.Z - 1; ++point3D.Z)
        {
          GlobalPoint3D p;
          p.X = globalOffset.X + point3D.X;
          p.Y = globalOffset.Y + point3D.Y;
          p.Z = globalOffset.Z + point3D.Z;
          this.BuildBlockNewFormat(content, ref p);
        }
      }
      else
      {
        point3D.Z = dir2 == Direction.Forward ? 0 : this.chunksize.Z - 1;
        for (point3D.X = 1; point3D.X < this.chunksize.X - 1; ++point3D.X)
        {
          GlobalPoint3D p;
          p.X = globalOffset.X + point3D.X;
          p.Y = globalOffset.Y + point3D.Y;
          p.Z = globalOffset.Z + point3D.Z;
          this.BuildBlockNewFormat(content, ref p);
        }
      }
    }

    private void BuildChunkCorners(MapChunkContent content)
    {
      GlobalPoint3D globalOffset = this.chunk.GlobalOffset;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.X += this.chunksize.X - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.Z += this.chunksize.Z - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.X -= this.chunksize.X - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.Y += this.chunksize.Y - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.Z -= this.chunksize.Z - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.X += this.chunksize.X - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
      globalOffset.Z += this.chunksize.Z - 1;
      this.BuildBlockNewFormat(content, ref globalOffset);
    }

    private void BuildBlockAroundEmptyNewFormat(
      MapChunkContent content,
      ref GlobalPoint3D p,
      byte blockID)
    {
      if (this.blockData.Buffer <= (byte) 1)
        return;
      this.BuildAroundEmptyBlock(ref p, blockID);
      if (blockID == (byte) 0 || blockID == (byte) 125)
        return;
      this.BuildSpecialBlocks(p, blockID);
    }

    private void BuildBlockNewFormat(MapChunkContent content, ref GlobalPoint3D p)
    {
      byte blockId = this.GetDataBlock(ref p).BlockID;
      switch (blockId)
      {
        case 0:
          break;
        case 125:
          break;
        default:
          this.blockData = this.map.BlockData[(int) blockId];
          if (this.blockData.Buffer == (byte) 0)
          {
            this.BuildAroundSolidBlock(ref p, blockId, blockId);
            break;
          }
          this.BuildSpecialBlocks(p, blockId);
          break;
      }
    }

    private void BuildSpecialBlocks(GlobalPoint3D p, byte blockID)
    {
      if (this.blockData.Buffer == (byte) 4)
      {
        ITMPluginBlocks pluginBlocks = ModManager.GetPluginBlocks(blockID);
        if (pluginBlocks.IsCustomMesh(blockID))
        {
          pluginBlocks.BuildCustomMesh((ITMMeshBuilder) this, (ITMMap) this.map, p, blockID);
          return;
        }
        byte meshBlockId = pluginBlocks.GetMeshBlockID(blockID);
        if ((int) meshBlockId != (int) blockID)
        {
          blockID = meshBlockId;
          this.blockData = this.map.BlockData[(int) blockID];
        }
      }
      this.isWindAffected = this.blockData.WindAffect > (byte) 0;
      if (this.blockData.IsIcon)
      {
        if (blockID != (byte) 121)
          this.BuildIconBlock_NewFormat(p, blockID);
        else
          this.BuildBookBlock_NewFormat(p, blockID);
      }
      else if (this.blockData.Buffer == (byte) 3)
      {
        this.isWaterVertex = true;
        switch (blockID)
        {
          case 10:
            byte num = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
            byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
            this.BuildTransparentBlock_NewFormat(p, blockID, textureIdForDrawing);
            break;
          case 11:
          case 13:
            this.BuildLiquidBlock_NewFormat(p, blockID);
            break;
          case 76:
            this.BuildPaneBlock_NewFormat(p, blockID);
            break;
          case 118:
            this.BuildFire_NewFormat(p, blockID);
            break;
          case 153:
            this.isWaterVertex = false;
            this.BuildOnewayGlassBlock_NewFormat(p, blockID);
            break;
          case 181:
            this.BuildStainedGlassBlock_NewFormat(p, blockID);
            break;
          default:
            if (ItemData.IsSubTypeAny(blockID, ItemSubType.Leaves))
            {
              this.isWaterVertex = false;
              this.BuildLeavesBlock_NewFormat(p, blockID, blockID);
              break;
            }
            this.BuildTransparentBlock_NewFormat(p, blockID, blockID);
            break;
        }
        this.isWaterVertex = false;
      }
      else if (this.blockData.Buffer == (byte) 1)
      {
        switch (blockID)
        {
          case 29:
            this.BuildBedrock_NewFormat(p, blockID);
            break;
          case 31:
            this.BuildObsidian_NewFormat(p, blockID);
            break;
          case 126:
            this.BuildCoverBlock_NewFormat(ref p, blockID);
            break;
          case 128:
            this.BuildArcadeMachine_NewFormat(ref p, blockID);
            break;
          default:
            this.BuildMultiTexture_NewFormat(ref p, blockID);
            break;
        }
      }
      else
      {
        switch (blockID)
        {
          case 46:
            this.BuildTorchBlock_NewFormat(p, blockID);
            break;
          case 47:
          case 120:
            this.BuildLadder_NewFormat(p, blockID);
            break;
          case 51:
          case 52:
          case 115:
          case 116:
          case 140:
          case 204:
            this.BuildDoorBlock_NewFormat(p, blockID);
            break;
          case 53:
            this.BuildTeleportBlock_NewFormat(p, blockID);
            break;
          case 72:
            this.BuildRopeBlock_NewFormat(p, blockID);
            break;
          case 75:
            this.BuildPaneBlock_NewFormat(p, blockID);
            break;
          case 77:
          case 186:
            this.BuildPostBlock_NewFormat(p, blockID);
            break;
          case 113:
          case 182:
            this.BuildStairsBlock_NewFormat(p, blockID);
            break;
          case 117:
            this.BuildSignBlock_NewFormat(p, blockID);
            break;
          case 119:
            this.BuildSpikes_NewFormat(p, blockID);
            break;
          case 124:
            this.BuildCropBlock_NewFormat(p, blockID);
            break;
          case 130:
          case 185:
            this.BuildStack_NewFormat(p, blockID);
            break;
          case 131:
            this.BuildUpsideDownStack_NewFormat(p, blockID);
            break;
          case 135:
          case 136:
            this.BuildBed_NewFormat(p, blockID);
            break;
          case 139:
            this.BuildFenceBlock_NewFormat(p, blockID);
            break;
          case 145:
            this.BuildSnowLayer_NewFormat(p, blockID);
            break;
          case 149:
          case 183:
            this.BuildHalfBlock_NewFormat(p, blockID);
            break;
          case 150:
          case 184:
            this.BuildRamp_NewFormat(p, blockID);
            break;
          case 154:
            this.BuildCylinderBlock_NewFormat(p, blockID);
            break;
          case 158:
            this.BuildTableBlock_NewFormat(p, blockID);
            break;
          case 160:
            this.BuildPaintingBlock_NewFormat(p, blockID);
            break;
          case 165:
            this.BuildPressurePlate_NewFormat(p, blockID);
            break;
          case 169:
            this.BuildSwitchBlock_NewFormat(p, blockID);
            break;
          case 170:
            this.BuildButtonBlock_NewFormat(p, blockID);
            break;
          case 171:
            this.BuildTrapDoorBlock_NewFormat(p, blockID);
            break;
          case 187:
          case 188:
            this.BuildSidePostBlock_NewFormat(p, blockID);
            break;
          case 189:
          case 190:
            this.BuildCornerBlock_NewFormat(p, blockID);
            break;
        }
      }
      this.isWindAffected = false;
    }

    private void BuildVisibleOre(GlobalPoint3D p, byte blockID)
    {
      this.AddLeftFaceNewFormat(ref p, blockID, blockID);
      this.AddForwardFaceNewFormat(ref p, blockID, blockID);
      this.AddRightFaceNewFormat(ref p, blockID, blockID);
      this.AddBackwardFaceNewFormat(ref p, blockID, blockID);
      this.AddUpFaceNewFormat(ref p, blockID, blockID);
      this.AddDownFaceNewFormat(ref p, blockID, blockID);
    }

    private void BuildAroundEmptyBlock(ref GlobalPoint3D p, byte blockID)
    {
      if (p.X > this.mapBound.Min.X)
      {
        --p.X;
        byte blockId = this.GetDataBlock(ref p).BlockID;
        if (this.map.BlockData[(int) blockId].Buffer < (byte) 2)
          this.AddRightFaceNewFormat(ref p, blockId, blockId);
        ++p.X;
      }
      if (p.Z > this.mapBound.Min.Z)
      {
        --p.Z;
        byte blockId = this.GetDataBlock(ref p).BlockID;
        if (this.map.BlockData[(int) blockId].Buffer < (byte) 2)
          this.AddBackwardFaceNewFormat(ref p, blockId, blockId);
        ++p.Z;
      }
      if (p.X < this.mapBound.Max.X - 1)
      {
        ++p.X;
        byte blockId = this.GetDataBlock(ref p).BlockID;
        if (this.map.BlockData[(int) blockId].Buffer < (byte) 2)
          this.AddLeftFaceNewFormat(ref p, blockId, blockId);
        --p.X;
      }
      if (p.Z < this.mapBound.Max.Z - 1)
      {
        ++p.Z;
        byte blockId = this.GetDataBlock(ref p).BlockID;
        if (this.map.BlockData[(int) blockId].Buffer < (byte) 2)
          this.AddForwardFaceNewFormat(ref p, blockId, blockId);
        --p.Z;
      }
      if (p.Y > this.mapBound.Min.Y)
      {
        --p.Y;
        byte blockId = this.GetDataBlock(ref p).BlockID;
        if (this.map.BlockData[(int) blockId].Buffer < (byte) 2)
          this.AddUpFaceNewFormat(ref p, blockId, blockId);
        ++p.Y;
      }
      if (p.Y >= this.mapBound.Max.Y - 1)
        return;
      ++p.Y;
      byte blockId1 = this.GetDataBlock(ref p).BlockID;
      if (this.map.BlockData[(int) blockId1].Buffer < (byte) 2)
        this.AddDownFaceNewFormat(ref p, blockId1, blockId1);
      --p.Y;
    }

    public int BuildPickup(
      MapTM map,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float rotation,
      ushort itemID,
      byte textureIndex,
      float light,
      Player player)
    {
      this.map = map;
      if (VoxelMeshBuilder.UseCube((Item) itemID))
      {
        Block blockID = (Block) itemID;
        byte blockTextureID = (byte) blockID;
        if (textureIndex > (byte) 0 || map.UsesBlockTextureTable(blockID))
          blockTextureID = (byte) map.GetBlockTextureIDForDrawing(blockID, (int) textureIndex);
        int count1 = verts.Count;
        this.BuildBlock(map, verts, pos, scale, blockTextureID, light);
        int count2 = verts.Count - count1;
        if ((double) rotation != 0.0)
          this.TransformVerticesForSpin(verts, count1, count2, pos, rotation);
        return count2;
      }
      this.BuildItemPickup(verts, pos, scale, (int) itemID, 1f);
      this.TransformVerticesToFacePlayer(verts, verts.Count - 4, 4, pos, player, light);
      return 4;
    }

    public static bool UseCube(Item itemID)
    {
      if (itemID > Item.zLastBlockID)
        return false;
      Block block = (Block) itemID;
      if (Globals1.BlockData[(int) block].IsIcon)
        return false;
      return Globals1.ItemTypeData[(int) block].Swing == ItemSwingType.Block;
    }

    public int BuildProjectile(
      MapTM map,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 vel,
      float scale,
      ushort itemID,
      float light)
    {
      this.map = map;
      this.BuildItemProjectile(verts, pos, scale, (int) itemID, 1f);
      this.TransformVertices(verts, verts.Count - 8, 8, pos, Vector3.Normalize(vel), light);
      return 8;
    }

    private void BuildAroundSolidBlock(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      if (this.IsClearLeft(ref p, blockID, 0, 0))
        this.AddLeftFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearForward(ref p, blockID, 0, 1))
        this.AddForwardFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearRight(ref p, blockID, 0, 2))
        this.AddRightFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearBackward(ref p, blockID, 0, 3))
        this.AddBackwardFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearUp(ref p, blockID, 0, 4))
        this.AddUpFaceNewFormat(ref p, blockID, blockIDTexture);
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockIDTexture);
    }

    private void BuildAroundSolidBlock2(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      if (this.IsClearLeft(ref p, blockID, 0, 0))
        this.AddLeftFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearForward(ref p, blockID, 0, 1))
        this.AddForwardFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearRight(ref p, blockID, 0, 2))
        this.AddRightFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearBackward(ref p, blockID, 0, 3))
        this.AddBackwardFaceNewFormat(ref p, blockID, blockIDTexture);
      if (this.IsClearUp(ref p, blockID, 0, 4))
        this.AddUpFaceNewFormat(ref p, blockID, blockIDTexture);
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockIDTexture);
    }

    private void BuildTransparentBlock_NewFormat(
      GlobalPoint3D p,
      byte blockID,
      byte blockIDTexture)
    {
      byte leftBlockId = this.GetLeftBlockID(p);
      if ((int) leftBlockId != (int) blockID && this.map.BlockData[(int) leftBlockId].Buffer > (byte) 1)
        this.AddLeftFaceNewFormat(ref p, blockID, blockIDTexture);
      byte forwardBlockId = this.GetForwardBlockID(p);
      if ((int) forwardBlockId != (int) blockID && this.map.BlockData[(int) forwardBlockId].Buffer > (byte) 1)
        this.AddForwardFaceNewFormat(ref p, blockID, blockIDTexture);
      byte rightBlockId = this.GetRightBlockID(p);
      if ((int) rightBlockId != (int) blockID && this.map.BlockData[(int) rightBlockId].Buffer > (byte) 1)
        this.AddRightFaceNewFormat(ref p, blockID, blockIDTexture);
      byte backwardBlockId = this.GetBackwardBlockID(p);
      if ((int) backwardBlockId != (int) blockID && this.map.BlockData[(int) backwardBlockId].Buffer > (byte) 1)
        this.AddBackwardFaceNewFormat(ref p, blockID, blockIDTexture);
      byte upBlockId = this.GetUpBlockID(p);
      if ((int) upBlockId != (int) blockID && this.map.BlockData[(int) upBlockId].Buffer > (byte) 1)
        this.AddUpFaceNewFormat(ref p, blockID, blockIDTexture);
      byte downBlockId = this.GetDownBlockID(p);
      if ((int) downBlockId == (int) blockID || this.map.BlockData[(int) downBlockId].Buffer <= (byte) 1)
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockIDTexture);
    }

    private void BuildLeavesBlock_NewFormat(GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      GameSettings gameSettings = Globals2.GameSettings;
      bool flag = (gameSettings.LeafMesh & LeafMeshType.Sides) > LeafMeshType.None;
      byte leftBlockId = this.GetLeftBlockID(p);
      if (ItemData.IsSubTypeAny(leftBlockId, ItemSubType.Leaves) ? flag : (int) leftBlockId != (int) blockID && this.map.BlockData[(int) leftBlockId].Buffer > (byte) 1)
        this.AddLeftFaceNewFormat(ref p, blockID, blockIDTexture);
      byte forwardBlockId = this.GetForwardBlockID(p);
      if (ItemData.IsSubTypeAny(forwardBlockId, ItemSubType.Leaves) ? flag : (int) forwardBlockId != (int) blockID && this.map.BlockData[(int) forwardBlockId].Buffer > (byte) 1)
        this.AddForwardFaceNewFormat(ref p, blockID, blockIDTexture);
      byte rightBlockId = this.GetRightBlockID(p);
      if (ItemData.IsSubTypeAny(rightBlockId, ItemSubType.Leaves) ? flag : (int) rightBlockId != (int) blockID && this.map.BlockData[(int) rightBlockId].Buffer > (byte) 1)
        this.AddRightFaceNewFormat(ref p, blockID, blockIDTexture);
      byte backwardBlockId = this.GetBackwardBlockID(p);
      if (ItemData.IsSubTypeAny(backwardBlockId, ItemSubType.Leaves) ? flag : (int) backwardBlockId != (int) blockID && this.map.BlockData[(int) backwardBlockId].Buffer > (byte) 1)
        this.AddBackwardFaceNewFormat(ref p, blockID, blockIDTexture);
      byte upBlockId = this.GetUpBlockID(p);
      if (ItemData.IsSubTypeAny(upBlockId, ItemSubType.Leaves) ? (gameSettings.LeafMesh & LeafMeshType.Above) > LeafMeshType.None : (int) upBlockId != (int) blockID && this.map.BlockData[(int) upBlockId].Buffer > (byte) 1)
        this.AddUpFaceNewFormat(ref p, blockID, blockIDTexture);
      byte downBlockId = this.GetDownBlockID(p);
      if (!(ItemData.IsSubTypeAny(downBlockId, ItemSubType.Leaves) ? (gameSettings.LeafMesh & LeafMeshType.Below) > LeafMeshType.None : (int) downBlockId != (int) blockID && this.map.BlockData[(int) downBlockId].Buffer > (byte) 1))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockIDTexture);
    }

    private void BuildOnewayGlassBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte num = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      this.BuildTransparentBlock_NewFormat(p, blockID, textureIdForDrawing);
    }

    private void BuildStainedGlassBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      int blockIDTexture = (int) (byte) ((uint) this.GetDataBlockAux(ref p) >> 4) + 304;
      byte leftBlockId = this.GetLeftBlockID(p);
      if ((int) leftBlockId != (int) blockID && this.map.BlockData[(int) leftBlockId].Buffer > (byte) 1)
        this.AddLeftFaceNewFormat(ref p, blockID, blockIDTexture);
      byte forwardBlockId = this.GetForwardBlockID(p);
      if ((int) forwardBlockId != (int) blockID && this.map.BlockData[(int) forwardBlockId].Buffer > (byte) 1)
        this.AddForwardFaceNewFormat(ref p, blockID, blockIDTexture);
      byte rightBlockId = this.GetRightBlockID(p);
      if ((int) rightBlockId != (int) blockID && this.map.BlockData[(int) rightBlockId].Buffer > (byte) 1)
        this.AddRightFaceNewFormat(ref p, blockID, blockIDTexture);
      byte backwardBlockId = this.GetBackwardBlockID(p);
      if ((int) backwardBlockId != (int) blockID && this.map.BlockData[(int) backwardBlockId].Buffer > (byte) 1)
        this.AddBackwardFaceNewFormat(ref p, blockID, blockIDTexture);
      byte upBlockId = this.GetUpBlockID(p);
      if ((int) upBlockId != (int) blockID && this.map.BlockData[(int) upBlockId].Buffer > (byte) 1)
        this.AddUpFaceNewFormat(ref p, blockID, blockIDTexture);
      byte downBlockId = this.GetDownBlockID(p);
      if ((int) downBlockId == (int) blockID || this.map.BlockData[(int) downBlockId].Buffer <= (byte) 1)
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockIDTexture);
    }

    private float GetLiquidTopY(GlobalPoint3D p)
    {
      MapBlock dataBlock = this.GetDataBlock(ref p);
      if (dataBlock.BlockID == (byte) 0)
        return 0.0f;
      if (dataBlock.BlockID != (byte) 11 && dataBlock.BlockID != (byte) 13)
        return 0.05f;
      ++p.Y;
      switch (p.Y < this.mapBound.Max.Y ? this.GetDataBlockID(ref p) : this.map.OutOfBoundsBlockID)
      {
        case 11:
        case 13:
          return 1f;
        default:
          if (dataBlock.BlockID == (byte) 11)
            return (float) ((double) (7 - ((int) dataBlock.AuxData & 7)) * 0.121428564190865 + 0.0500000007450581);
          return (float) ((double) (5 - ((int) dataBlock.AuxData & 7)) * 0.170000001788139 + 0.0500000007450581);
      }
    }

    public static float GetLiquidTopY(Map map, GlobalPoint3D p)
    {
      byte blockId = map.GetBlockID(p);
      switch (blockId)
      {
        case 0:
          return 0.0f;
        case 11:
        case 13:
          ++p.Y;
          switch (p.Y < map.MapBound.Max.Y ? map.GetBlockID(p) : map.OutOfBoundsBlockID)
          {
            case 11:
            case 13:
              return 1f;
            default:
              --p.Y;
              byte auxData = map.GetAuxData(p);
              if (blockId == (byte) 11)
                return (float) ((double) (7 - (int) auxData) * 0.121428564190865 + 0.0500000007450581);
              return (float) ((double) (5 - (int) auxData) * 0.170000001788139 + 0.0500000007450581);
          }
        default:
          return 0.05f;
      }
    }

    public static float GetLiquidTopY(Map map, Vector3 pos)
    {
      GlobalPoint3D point = map.GetPoint(pos);
      float liquidTopY1 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      --point.X;
      float liquidTopY2 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      --point.Z;
      float liquidTopY3 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      ++point.X;
      float liquidTopY4 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      ++point.X;
      float liquidTopY5 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      ++point.Z;
      float liquidTopY6 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      ++point.Z;
      float liquidTopY7 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      --point.X;
      float liquidTopY8 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      --point.X;
      float liquidTopY9 = VoxelMeshBuilder.GetLiquidTopY(map, point);
      float val2_1 = Math.Max(liquidTopY2, Math.Max(liquidTopY3, liquidTopY4));
      float num1 = (double) val2_1 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_1);
      float val2_2 = Math.Max(liquidTopY4, Math.Max(liquidTopY5, liquidTopY6));
      float num2 = (double) val2_2 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_2);
      float val2_3 = Math.Max(liquidTopY6, Math.Max(liquidTopY7, liquidTopY8));
      float num3 = (double) val2_3 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_3);
      float val2_4 = Math.Max(liquidTopY2, Math.Max(liquidTopY9, liquidTopY8));
      float num4 = (double) val2_4 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_4);
      float amount1 = pos.X - (float) (int) pos.X;
      float amount2 = (float) (1.0 - ((double) pos.Z - (double) (int) pos.Z));
      if ((double) amount1 + (double) amount2 < (double) map.TileSize)
        return MathHelper.Lerp(MathHelper.Lerp(num1, num3, amount1), MathHelper.Lerp(num4, num3, amount1), amount2) + (float) (int) pos.Y;
      return MathHelper.Lerp(MathHelper.Lerp(num1, num2, amount1), MathHelper.Lerp(num1, num3, amount1), amount2) + (float) (int) pos.Y;
    }

    private void BuildLiquidBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      ++p.Y;
      byte num1 = p.Y < this.mapBound.Max.Y ? this.GetDataBlockID(ref p) : this.map.OutOfBoundsBlockID;
      --p.Y;
      --p.Y;
      byte num2 = p.Y > this.mapBound.Min.Y ? this.GetDataBlockID(ref p) : this.map.OutOfBoundsBlockID;
      ++p.Y;
      bool flag1 = false;
      float num3 = 1f;
      float num4;
      float num5;
      float num6;
      float num7;
      if (num1 == (byte) 11 || num1 == (byte) 13)
      {
        double num8;
        num4 = (float) (num8 = (double) num3);
        num5 = (float) num8;
        num6 = (float) num8;
        num7 = (float) num8;
      }
      else
      {
        float liquidTopY1 = this.GetLiquidTopY(p);
        --p.X;
        float liquidTopY2 = this.GetLiquidTopY(p);
        --p.Z;
        float liquidTopY3 = this.GetLiquidTopY(p);
        ++p.X;
        float liquidTopY4 = this.GetLiquidTopY(p);
        ++p.X;
        float liquidTopY5 = this.GetLiquidTopY(p);
        ++p.Z;
        float liquidTopY6 = this.GetLiquidTopY(p);
        ++p.Z;
        float liquidTopY7 = this.GetLiquidTopY(p);
        --p.X;
        float liquidTopY8 = this.GetLiquidTopY(p);
        --p.X;
        float liquidTopY9 = this.GetLiquidTopY(p);
        ++p.X;
        --p.Z;
        float val2_1 = Math.Max(liquidTopY2, Math.Max(liquidTopY3, liquidTopY4));
        num7 = (double) val2_1 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_1);
        float val2_2 = Math.Max(liquidTopY4, Math.Max(liquidTopY5, liquidTopY6));
        num6 = (double) val2_2 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_2);
        float val2_3 = Math.Max(liquidTopY6, Math.Max(liquidTopY7, liquidTopY8));
        num5 = (double) val2_3 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_3);
        float val2_4 = Math.Max(liquidTopY2, Math.Max(liquidTopY9, liquidTopY8));
        num4 = (double) val2_4 <= 0.0 ? 0.05f : Math.Max(liquidTopY1, val2_4);
        flag1 = (double) num7 != (double) liquidTopY1 || (double) num6 != (double) liquidTopY1 || (double) num5 != (double) liquidTopY1 || (double) num4 != (double) liquidTopY1;
      }
      Vector3 position1 = this.map.GetPosition(p);
      Vector3 vector3 = new Vector3();
      vector3.X = position1.X + this.tilesize;
      vector3.Z = position1.Z + this.tilesize;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = false;
      data.Face = 4;
      data.Pos1 = position1;
      data.Pos2 = vector3;
      float num9 = position1.Y - this.tilesize;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, flag1 ? 0 : 4]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, flag1 ? 0 : 4]];
      if (num1 != (byte) 11 && num1 != (byte) 13 && ((double) num6 != 1.0 || (double) num7 != 1.0 || ((double) num5 != 1.0 || (double) num4 != 1.0) || this.map.BlockData[(int) num1].Buffer > (byte) 1))
      {
        Vector2 vector1 = new Vector2();
        Vector2 vector2 = new Vector2();
        Vector2 vector4 = new Vector2();
        Vector2 vector5 = new Vector2();
        bool flag2 = false;
        if (flag1)
        {
          float num8 = 1f;
          float radians = 0.0f;
          if ((double) num7 < (double) num6 || (double) num4 < (double) num5)
          {
            radians = -1.570796f;
            if ((double) num7 < (double) num4 || (double) num6 < (double) num5)
            {
              radians -= 0.7853982f;
              num8 = 0.71f;
            }
            else if ((double) num4 < (double) num7 || (double) num5 < (double) num6)
            {
              radians += 0.7853982f;
              num8 = 0.71f;
            }
          }
          else if ((double) num6 < (double) num7 || (double) num5 < (double) num4)
          {
            radians = 1.570796f;
            if ((double) num5 < (double) num6 || (double) num4 < (double) num7)
            {
              radians -= 0.7853982f;
              num8 = 0.71f;
            }
            else if ((double) num7 < (double) num4 || (double) num6 < (double) num5)
            {
              radians += 0.7853982f;
              num8 = 0.71f;
            }
          }
          else if ((double) num7 < (double) num4 || (double) num6 < (double) num5)
            radians = 3.141593f;
          if ((double) radians != 0.0)
          {
            Vector2 position2 = new Vector2(-num8, -num8);
            Vector2 position3 = new Vector2(num8, -num8);
            Vector2 position4 = new Vector2(num8, num8);
            Vector2 position5 = new Vector2(-num8, num8);
            flag2 = true;
            Matrix rotationZ = Matrix.CreateRotationZ(radians);
            Vector2 vector2_3 = Vector2.Transform(position2, rotationZ);
            Vector2 vector2_4 = Vector2.Transform(position3, rotationZ);
            Vector2 vector2_5 = Vector2.Transform(position4, rotationZ);
            position5 = Vector2.Transform(position5, rotationZ);
            float num10 = (float) (((double) vector2_2.X - (double) vector2_1.X) * 0.5);
            float num11 = (float) (((double) vector2_2.Y - (double) vector2_1.Y) * 0.5);
            float num12 = vector2_1.X + num10;
            float num13 = vector2_1.Y + num11;
            vector1.X = vector2_3.X * num10 + num12;
            vector1.Y = vector2_3.Y * num11 + num13;
            vector2.X = vector2_4.X * num10 + num12;
            vector2.Y = vector2_4.Y * num11 + num13;
            vector4.X = vector2_5.X * num10 + num12;
            vector4.Y = vector2_5.Y * num11 + num13;
            vector5.X = position5.X * num10 + num12;
            vector5.Y = position5.Y * num11 + num13;
          }
        }
        if (!flag2)
        {
          vector1.X = vector2_1.X;
          vector1.Y = vector2_1.Y;
          vector2.X = vector2_2.X;
          vector2.Y = vector2_1.Y;
          vector4.X = vector2_2.X;
          vector4.Y = vector2_2.Y;
          vector5.X = vector2_1.X;
          vector5.Y = vector2_2.Y;
        }
        data.X = position1.X;
        data.Y = num9 + num7;
        data.Z = position1.Z;
        data.TC = new NormalizedShort2(vector1);
        this.AddVertex(ref data);
        data.X = vector3.X;
        data.Y = num9 + num6;
        data.TC = new NormalizedShort2(vector2);
        this.AddVertex(ref data);
        data.Y = num9 + num5;
        data.Z = vector3.Z;
        data.TC = new NormalizedShort2(vector4);
        this.AddVertex(ref data);
        data.X = position1.X;
        data.Y = num9 + num4;
        data.TC = new NormalizedShort2(vector5);
        this.AddVertex(ref data);
      }
      vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 5]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 5]];
      --p.X;
      byte num14 = p.X > this.mapBound.Min.X ? this.GetDataBlockID(ref p) : (byte) 11;
      ++p.X;
      if (num14 != (byte) 11 && num14 != (byte) 13 && this.map.BlockData[(int) num14].Buffer > (byte) 1)
      {
        data.X = position1.X;
        data.Y = num9;
        data.Z = position1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = num9 + num7;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = vector3.Z;
        data.Y = num9 + num4;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = num9;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      --p.Z;
      byte num15 = p.Z > this.mapBound.Min.Z ? this.GetDataBlockID(ref p) : (byte) 11;
      ++p.Z;
      if (num15 != (byte) 11 && num15 != (byte) 13 && this.map.BlockData[(int) num15].Buffer > (byte) 1)
      {
        data.X = vector3.X;
        data.Y = num9;
        data.Z = position1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = num9 + num6;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = position1.X;
        data.Y = num9 + num7;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = num9;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      ++p.X;
      byte num16 = p.X < this.mapBound.Max.X ? this.GetDataBlockID(ref p) : (byte) 11;
      --p.X;
      if (num16 != (byte) 11 && num16 != (byte) 13 && this.map.BlockData[(int) num16].Buffer > (byte) 1)
      {
        data.X = vector3.X;
        data.Y = num9;
        data.Z = vector3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = num9 + num5;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = position1.Z;
        data.Y = num9 + num6;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = num9;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      ++p.Z;
      byte num17 = p.Z < this.mapBound.Max.Z ? this.GetDataBlockID(ref p) : (byte) 11;
      --p.Z;
      if (num17 != (byte) 11 && num17 != (byte) 13 && this.map.BlockData[(int) num17].Buffer > (byte) 1)
      {
        data.X = position1.X;
        data.Y = num9;
        data.Z = vector3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = num9 + num4;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3.X;
        data.Y = num9 + num5;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = num9;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (num2 == (byte) 11 || num2 == (byte) 13 || this.map.BlockData[(int) num2].Buffer <= (byte) 1)
        return;
      vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 3]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 3]];
      data.Face = 5;
      data.X = position1.X;
      data.Y = num9;
      data.Z = position1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3.X;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = position1.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
    }

    private bool HasAirNeighbourExceptDirectlyAbove(GlobalPoint3D p, int recurseCount)
    {
      --p.X;
      if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
      {
        --p.Z;
        if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
        {
          ++p.X;
          if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
          {
            ++p.X;
            if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
            {
              ++p.Z;
              if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
              {
                ++p.Z;
                if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
                {
                  --p.X;
                  if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
                  {
                    --p.X;
                    if (this.GetDataBlockID_TestBounds(ref p) != (byte) 0)
                      return false;
                  }
                }
              }
            }
          }
        }
      }
      if (--recurseCount <= 0)
        return true;
      return this.HasAirNeighbourExceptDirectlyAbove(p, recurseCount);
    }

    private void BuildFire_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte num1;
      byte num2;
      byte num3;
      byte num4;
      byte num5;
      byte num6;
      if (this.map.MapStrategyTM.GetDataBlock(p) is FireBlock)
      {
        --p.X;
        num1 = this.GetDataBlock(ref p).BlockID;
        if (num1 == (byte) 118 || (int) num1 == (int) this.map.OutOfBoundsBlockID || Globals1.ItemData[(int) num1].BurnTime <= (ushort) 0)
          num1 = (byte) 0;
        p.X += 2;
        num2 = this.GetDataBlock(ref p).BlockID;
        if (num2 == (byte) 118 || (int) num2 == (int) this.map.OutOfBoundsBlockID || Globals1.ItemData[(int) num2].BurnTime <= (ushort) 0)
          num2 = (byte) 0;
        --p.X;
        --p.Z;
        num3 = this.GetDataBlock(ref p).BlockID;
        if (num3 == (byte) 118 || (int) num3 == (int) this.map.OutOfBoundsBlockID || Globals1.ItemData[(int) num3].BurnTime <= (ushort) 0)
          num3 = (byte) 0;
        p.Z += 2;
        num4 = this.GetDataBlock(ref p).BlockID;
        if (num4 == (byte) 118 || (int) num4 == (int) this.map.OutOfBoundsBlockID || Globals1.ItemData[(int) num4].BurnTime <= (ushort) 0)
          num4 = (byte) 0;
        --p.Z;
        --p.Y;
        num5 = this.GetDataBlock(ref p).BlockID;
        if (num5 == (byte) 118 || (int) num5 == (int) this.map.OutOfBoundsBlockID || Globals1.ItemData[(int) num5].BurnTime <= (ushort) 0)
          num5 = (byte) 0;
        p.Y += 2;
        num6 = this.GetDataBlock(ref p).BlockID;
        if (num6 == (byte) 118 || (int) num6 == (int) this.map.OutOfBoundsBlockID || Globals1.ItemData[(int) num6].BurnTime <= (ushort) 0)
          num6 = (byte) 0;
        --p.Y;
      }
      else
      {
        --p.X;
        num1 = this.GetDataBlock(ref p).BlockID;
        if (num1 == (byte) 118 || (int) num1 == (int) this.map.OutOfBoundsBlockID)
          num1 = (byte) 0;
        p.X += 2;
        num2 = this.GetDataBlock(ref p).BlockID;
        if (num2 == (byte) 118 || (int) num2 == (int) this.map.OutOfBoundsBlockID)
          num2 = (byte) 0;
        --p.X;
        --p.Z;
        num3 = this.GetDataBlock(ref p).BlockID;
        if (num3 == (byte) 118 || (int) num3 == (int) this.map.OutOfBoundsBlockID)
          num3 = (byte) 0;
        p.Z += 2;
        num4 = this.GetDataBlock(ref p).BlockID;
        if (num4 == (byte) 118 || (int) num4 == (int) this.map.OutOfBoundsBlockID)
          num4 = (byte) 0;
        --p.Z;
        --p.Y;
        num5 = this.GetDataBlock(ref p).BlockID;
        if (num5 == (byte) 118 || (int) num5 == (int) this.map.OutOfBoundsBlockID)
          num5 = (byte) 0;
        p.Y += 2;
        num6 = this.GetDataBlock(ref p).BlockID;
        if (num6 == (byte) 118 || (int) num6 == (int) this.map.OutOfBoundsBlockID)
          num6 = (byte) 0;
        --p.Y;
      }
      if (num5 > (byte) 0 || num1 > (byte) 0)
        this.AddLeftFireFaceNewFormat(ref p, blockID);
      if (num5 > (byte) 0 || num3 > (byte) 0)
        this.AddForwardFireFaceNewFormat(ref p, blockID);
      if (num5 > (byte) 0 || num2 > (byte) 0)
        this.AddRightFireFaceNewFormat(ref p, blockID);
      if (num5 > (byte) 0 || num4 > (byte) 0)
        this.AddBackwardFireFaceNewFormat(ref p, blockID);
      if (num6 > (byte) 0 && num5 == (byte) 0)
        this.AddUpFireFaceNewFormat(ref p, blockID);
      if (num5 == (byte) 0 && num1 == (byte) 0 && (num3 == (byte) 0 && num2 == (byte) 0) && (num4 == (byte) 0 && num6 == (byte) 0))
        num5 = (byte) 118;
      if (num5 <= (byte) 0)
        return;
      this.AddMiddleLeftRightFireFaceNewFormat(ref p, blockID);
      this.AddMiddleForwardBackwardFireFaceNewFormat(ref p, blockID);
    }

    private void AddLeftFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 0;
      this.SetFacePos(ref p, ref data);
      this.AddLeftFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddLeftFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 0]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 0]];
      vector2_2.Y = vector2_1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) vector2_2.Y - (double) vector2_1.Y));
      AVParams avParams = new AVParams();
      avParams.Point = p;
      avParams.BlockID = blockID;
      avParams.IsCorner = true;
      avParams.UseOwnLight = true;
      avParams.Face = face;
      if ((avParams.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        avParams.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      TempAVParams p1 = new TempAVParams(ref avParams);
      avParams.X += this.outerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      this.AddVertex(ref avParams);
      TempAVParams p2 = new TempAVParams(ref avParams);
      avParams.X = pos2.X + this.outerFireFaceLean;
      avParams.Z = pos2.Z - this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      TempAVParams p3 = new TempAVParams(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
    }

    private void AddForwardFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 1;
      this.SetFacePos(ref p, ref data);
      this.AddForwardFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddForwardFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 1]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 1]];
      vector2_2.Y = vector2_1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) vector2_2.Y - (double) vector2_1.Y));
      AVParams avParams = new AVParams();
      avParams.Point = p;
      avParams.BlockID = blockID;
      avParams.IsCorner = true;
      avParams.UseOwnLight = true;
      avParams.Face = face;
      if ((avParams.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        avParams.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y + 2;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      TempAVParams p1 = new TempAVParams(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      this.AddVertex(ref avParams);
      TempAVParams p2 = new TempAVParams(ref avParams);
      avParams.X = pos2.X + this.outerFireFaceLean;
      avParams.Z = pos2.Z + this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      TempAVParams p3 = new TempAVParams(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z -= this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
    }

    private void AddRightFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 2;
      this.SetFacePos(ref p, ref data);
      this.AddRightFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddRightFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 2]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 2]];
      vector2_2.Y = vector2_1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) vector2_2.Y - (double) vector2_1.Y));
      AVParams avParams = new AVParams();
      avParams.Point = p;
      avParams.BlockID = blockID;
      avParams.IsCorner = true;
      avParams.UseOwnLight = true;
      avParams.Face = face;
      if ((avParams.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        avParams.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y + 4;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      TempAVParams p1 = new TempAVParams(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z -= this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      this.AddVertex(ref avParams);
      TempAVParams p2 = new TempAVParams(ref avParams);
      avParams.X = pos2.X - this.outerFireFaceLean;
      avParams.Z = pos2.Z + this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      TempAVParams p3 = new TempAVParams(ref avParams);
      avParams.X += this.outerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z -= this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
    }

    private void AddBackwardFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 3;
      this.SetFacePos(ref p, ref data);
      this.AddBackwardFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddBackwardFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 3]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 3]];
      vector2_2.Y = vector2_1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) vector2_2.Y - (double) vector2_1.Y));
      AVParams avParams = new AVParams();
      avParams.Point = p;
      avParams.BlockID = blockID;
      avParams.IsCorner = true;
      avParams.UseOwnLight = true;
      avParams.Face = face;
      if ((avParams.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        avParams.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y + 6;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      TempAVParams p1 = new TempAVParams(ref avParams);
      avParams.X += this.outerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z -= this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      this.AddVertex(ref avParams);
      TempAVParams p2 = new TempAVParams(ref avParams);
      avParams.X = pos2.X - this.outerFireFaceLean;
      avParams.Z = pos2.Z - this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      TempAVParams p3 = new TempAVParams(ref avParams);
      avParams.X += this.outerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
    }

    private void AddUpFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 5;
      this.SetUpFacePos(ref p, ref data);
      this.AddUpFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddUpFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 4]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 4]];
      float num = this.outerFireFaceLean * 0.5f;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = true;
      data.Face = face;
      if ((data.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        data.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y + 8;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Y -= num;
      data.Z = (float) (((double) pos2.Z - (double) pos1.Z) * 0.600000023841858) + pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++data.WindUniformWaveRandomnessHash;
      this.AddVertex(ref data);
      data.Y += num;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.X = pos1.X;
      data.Y = pos1.Y - num;
      data.Z = (float) (((double) pos2.Z - (double) pos1.Z) * 0.400000005960464) + pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Y += num;
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      data.WindAffected = this.isWindAffected;
      ++data.WindUniformWaveRandomnessHash;
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      ++data.WindUniformWaveRandomnessHash;
      this.AddVertex(ref data);
      data.Y -= num;
      data.Z = (float) (((double) pos2.Z - (double) pos1.Z) * 0.400000005960464) + pos1.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void AddMiddleLeftRightFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 0;
      this.SetFacePos(ref p, ref data);
      this.AddMiddleLeftRightFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddMiddleLeftRightFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 0]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 0]];
      vector2_2.Y = vector2_1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) vector2_2.Y - (double) vector2_1.Y));
      AVParams avParams = new AVParams();
      avParams.Point = p;
      avParams.BlockID = blockID;
      avParams.IsCorner = true;
      avParams.UseOwnLight = true;
      avParams.Face = face;
      if ((avParams.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        avParams.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y + 12;
      pos1.X += this.tilesize * 0.4f;
      pos2.X += this.tilesize * 0.4f;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      TempAVParams p1 = new TempAVParams(ref avParams);
      avParams.X -= this.innerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      this.AddVertex(ref avParams);
      TempAVParams p2 = new TempAVParams(ref avParams);
      avParams.X = pos2.X - this.innerFireFaceLean;
      avParams.Z = pos2.Z - this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      TempAVParams p3 = new TempAVParams(ref avParams);
      avParams.X += this.innerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
      pos1.X += this.tilesize * 0.2f;
      pos2.X += this.tilesize * 0.2f;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      p1.SetFrom(ref avParams);
      avParams.X += this.innerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      p2.SetFrom(ref avParams);
      avParams.X = pos2.X + this.innerFireFaceLean;
      avParams.Z = pos2.Z - this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      p3.SetFrom(ref avParams);
      avParams.X -= this.innerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z += this.outerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
    }

    private void AddMiddleForwardBackwardFireFaceNewFormat(ref GlobalPoint3D p, byte blockID)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 1;
      this.SetFacePos(ref p, ref data);
      this.AddMiddleForwardBackwardFireFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, ref p);
    }

    private void AddMiddleForwardBackwardFireFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      ref GlobalPoint3D p)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 1]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 1]];
      vector2_2.Y = vector2_1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) vector2_2.Y - (double) vector2_1.Y));
      AVParams avParams = new AVParams();
      avParams.Point = p;
      avParams.BlockID = blockID;
      avParams.IsCorner = true;
      avParams.UseOwnLight = true;
      avParams.Face = face;
      if ((avParams.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        avParams.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y + 16;
      pos1.Z += this.tilesize * 0.4f;
      pos2.Z += this.tilesize * 0.4f;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      TempAVParams p1 = new TempAVParams(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z -= this.innerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      this.AddVertex(ref avParams);
      TempAVParams p2 = new TempAVParams(ref avParams);
      avParams.X = pos2.X + this.outerFireFaceLean;
      avParams.Z = pos2.Z - this.innerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      TempAVParams p3 = new TempAVParams(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z += this.innerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
      pos1.Z += this.tilesize * 0.2f;
      pos2.Z += this.tilesize * 0.2f;
      avParams.X = pos1.X;
      avParams.Y = pos1.Y;
      avParams.Z = pos1.Z;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref avParams);
      p1.SetFrom(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos2.Y;
      avParams.Z += this.innerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      avParams.WindAffected = this.isWindAffected;
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      p2.SetFrom(ref avParams);
      avParams.X = pos2.X + this.outerFireFaceLean;
      avParams.Z = pos2.Z + this.innerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      ++avParams.WindUniformWaveRandomnessHash;
      this.AddVertex(ref avParams);
      p3.SetFrom(ref avParams);
      avParams.X -= this.outerFireFaceLean;
      avParams.Y = pos1.Y;
      avParams.Z -= this.innerFireFaceLean;
      avParams.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      avParams.WindAffected = false;
      this.AddVertex(ref avParams);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p3);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p2);
      this.AddVertex(ref avParams);
      avParams.SetFrom(p1);
      this.AddVertex(ref avParams);
    }

    private void BuildTeleportBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte leftBlockId = this.GetLeftBlockID(p);
      if ((int) leftBlockId != (int) blockID && this.map.BlockData[(int) leftBlockId].Buffer > (byte) 1)
        this.AddLeftFaceNewFormat(ref p, blockID, blockID);
      if (p.X > this.mapBound.Min.X && (int) leftBlockId != (int) blockID)
      {
        --p.X;
        this.AddRightFaceNewFormat(ref p, blockID, blockID);
        ++p.X;
      }
      byte forwardBlockId = this.GetForwardBlockID(p);
      if ((int) forwardBlockId != (int) blockID && this.map.BlockData[(int) forwardBlockId].Buffer > (byte) 1)
        this.AddForwardFaceNewFormat(ref p, blockID, blockID);
      if (p.Z > this.mapBound.Min.Z && (int) forwardBlockId != (int) blockID)
      {
        --p.Z;
        this.AddBackwardFaceNewFormat(ref p, blockID, blockID);
        ++p.Z;
      }
      byte rightBlockId = this.GetRightBlockID(p);
      if ((int) rightBlockId != (int) blockID && this.map.BlockData[(int) rightBlockId].Buffer > (byte) 1)
        this.AddRightFaceNewFormat(ref p, blockID, blockID);
      if (p.X < this.mapBound.Max.X - 1 && (int) rightBlockId != (int) blockID)
      {
        ++p.X;
        this.AddLeftFaceNewFormat(ref p, blockID, blockID);
        --p.X;
      }
      byte backwardBlockId = this.GetBackwardBlockID(p);
      if ((int) backwardBlockId != (int) blockID && this.map.BlockData[(int) backwardBlockId].Buffer > (byte) 1)
        this.AddBackwardFaceNewFormat(ref p, blockID, blockID);
      if (p.Z < this.mapBound.Max.Z - 1 && (int) backwardBlockId != (int) blockID)
      {
        ++p.Z;
        this.AddForwardFaceNewFormat(ref p, blockID, blockID);
        --p.Z;
      }
      byte upBlockId = this.GetUpBlockID(p);
      if ((int) upBlockId != (int) blockID)
      {
        if (this.map.BlockData[(int) upBlockId].Buffer > (byte) 1)
          this.AddUpFaceNewFormat(ref p, blockID, blockID);
        if (p.Y < this.mapBound.Max.Y - 1 && (int) upBlockId != (int) blockID)
        {
          ++p.Y;
          this.AddDownFaceNewFormat(ref p, blockID, blockID);
          --p.Y;
        }
      }
      byte downBlockId = this.GetDownBlockID(p);
      if ((int) downBlockId == (int) blockID)
        return;
      if (this.map.BlockData[(int) downBlockId].Buffer > (byte) 1)
        this.AddDownFaceNewFormat(ref p, blockID, blockID);
      if (p.Y <= this.mapBound.Min.Y || (int) downBlockId == (int) blockID)
        return;
      --p.Y;
      this.AddUpFaceNewFormat(ref p, blockID, blockID);
      ++p.Y;
    }

    private void BuildIconBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      float num1 = blockID == (byte) 112 || blockID == (byte) 223 ? 1f : 0.5f;
      if ((double) num1 == 1.0)
        blockCenter.Y += this.halftilesize;
      float num2 = num1 * this.tilesize;
      float num3 = this.halftilesize * num2;
      NormalizedShort2 normalizedShort2_1 = new NormalizedShort2(MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 1]]);
      NormalizedShort2 normalizedShort2_2 = new NormalizedShort2(MapChunkContent.TexCoords2[MapChunkContent.TexOffsets[(int) blockID, 1]]);
      NormalizedShort2 normalizedShort2_3 = new NormalizedShort2(MapChunkContent.TexCoords3[MapChunkContent.TexOffsets[(int) blockID, 1]]);
      NormalizedShort2 normalizedShort2_4 = new NormalizedShort2(MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 1]]);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = true;
      if ((data.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        data.WindUniformWaveRandomnessHash = (p.X << 16) + (p.Z << 8) + p.Y;
      data.Face = 3;
      data.X = blockCenter.X - num3;
      data.Y = blockCenter.Y - num2;
      data.Z = blockCenter.Z;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = blockCenter.X + num3;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.Face = 1;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = blockCenter.X - num3;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.Face = 0;
      data.X = blockCenter.X;
      data.Y = blockCenter.Y - num2;
      data.Z = blockCenter.Z - num3;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.Z = blockCenter.Z + num3;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.Face = 2;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.Z = blockCenter.Z - num3;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void BuildCropBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      float num1 = 1f;
      blockCenter.Y += this.halftilesize;
      float num2 = num1 * this.tilesize;
      float num3 = this.halftilesize * num2;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      int num4 = (int) dataBlockAux & 7;
      if (num4 > 5)
        num4 = 5;
      int num5 = (int) dataBlockAux >> 4;
      int index = 256 + TexturePack.BlockTexturesPerRow() * 2;
      Vector2 vector1 = MapChunkContent.TexCoords1[index];
      Vector2 vector2 = MapChunkContent.TexCoords2[index];
      Vector2 vector3 = MapChunkContent.TexCoords3[index];
      Vector2 vector4 = MapChunkContent.TexCoords4[index];
      float num6 = (float) GraphicStatics.TexturePack.BlockTextureSize() / (float) GraphicStatics.TexturePack.BlockTexture.Width * (float) (num4 + num5 * 6);
      vector1.X += num6;
      vector2.X += num6;
      vector3.X += num6;
      vector4.X += num6;
      NormalizedShort2 normalizedShort2_1 = new NormalizedShort2(vector1);
      NormalizedShort2 normalizedShort2_2 = new NormalizedShort2(vector2);
      NormalizedShort2 normalizedShort2_3 = new NormalizedShort2(vector3);
      NormalizedShort2 normalizedShort2_4 = new NormalizedShort2(vector4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = true;
      if ((data.WindUniformWaveRandomness = (int) this.blockData.WindAffect) > 0)
        data.WindUniformWaveRandomnessHash = ((int) (byte) p.X << 16) + ((int) (byte) p.Z << 8) + (int) (byte) p.Y;
      data.Face = 3;
      data.X = blockCenter.X - num3;
      data.Y = blockCenter.Y - num2;
      data.Z = blockCenter.Z;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = blockCenter.X + num3;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.Face = 1;
      data.X = blockCenter.X + num3;
      data.Y = blockCenter.Y - num2;
      data.Z = blockCenter.Z;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = blockCenter.X - num3;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.Face = 0;
      data.X = blockCenter.X;
      data.Y = blockCenter.Y - num2;
      data.Z = blockCenter.Z - num3;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.Z = blockCenter.Z + num3;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
      data.Face = 2;
      data.X = blockCenter.X;
      data.Y = blockCenter.Y - num2;
      data.Z = blockCenter.Z + num3;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.Z = blockCenter.Z - num3;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = blockCenter.Y - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void BuildBookBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = num1 * 0.4f;
      float num3 = (float) (((double) this.tilesize - (double) num1) * 0.5);
      Vector3 position = this.map.GetPosition(p);
      position.Y -= this.tilesize;
      NormalizedShort2 normalizedShort2_1 = new NormalizedShort2(MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 2]]);
      NormalizedShort2 normalizedShort2_2 = new NormalizedShort2(MapChunkContent.TexCoords2[MapChunkContent.TexOffsets[(int) blockID, 2]]);
      NormalizedShort2 normalizedShort2_3 = new NormalizedShort2(MapChunkContent.TexCoords3[MapChunkContent.TexOffsets[(int) blockID, 2]]);
      NormalizedShort2 normalizedShort2_4 = new NormalizedShort2(MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 2]]);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = true;
      data.Face = 3;
      data.X = position.X + num3;
      data.Y = position.Y;
      data.Z = position.Z + this.halftilesize;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y + num1;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      this.AddVertex(ref data);
      data.X += num1;
      data.Z = position.Z + this.halftilesize + num2;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      this.AddVertex(ref data);
      data.Face = 1;
      data.X = position.X + num3 + num1;
      data.Y = position.Y;
      data.Z = position.Z + this.halftilesize - num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y + num1;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.X -= num1;
      data.Z = position.Z + this.halftilesize;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      normalizedShort2_1 = new NormalizedShort2(MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 3]]);
      normalizedShort2_2 = new NormalizedShort2(MapChunkContent.TexCoords2[MapChunkContent.TexOffsets[(int) blockID, 3]]);
      normalizedShort2_3 = new NormalizedShort2(MapChunkContent.TexCoords3[MapChunkContent.TexOffsets[(int) blockID, 3]]);
      normalizedShort2_4 = new NormalizedShort2(MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 3]]);
      data.Face = 1;
      data.X = position.X + num3 + num1;
      data.Y = position.Y;
      data.Z = position.Z + this.halftilesize + num2;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y + num1;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.X -= num1;
      data.Z = position.Z + this.halftilesize;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Face = 3;
      data.X = position.X + num3;
      data.Y = position.Y;
      data.Z = position.Z + this.halftilesize;
      data.TC.PackedValue = normalizedShort2_3.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y + num1;
      data.TC.PackedValue = normalizedShort2_1.PackedValue;
      this.AddVertex(ref data);
      data.X += num1;
      data.Z = position.Z + this.halftilesize - num2;
      data.TC.PackedValue = normalizedShort2_2.PackedValue;
      this.AddVertex(ref data);
      data.Y = position.Y;
      data.TC.PackedValue = normalizedShort2_4.PackedValue;
      this.AddVertex(ref data);
    }

    private void BuildTorchBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = 0.0f;
      Matrix torchTransform = this.GetTorchTransform((int) this.GetDataBlockAux(ref p) & 7);
      Vector2 tc1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 1]];
      Vector2 tc2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 1]];
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      int num3 = GraphicStatics.TexturePack.BlockTextureSize();
      blockCenter.Y -= (float) (num3 - GraphicStatics.TexturePack.TorchSpriteHeight) / (float) num3 * this.tilesize;
      Vector3 zero1 = Vector3.Zero;
      Vector3 zero2 = Vector3.Zero;
      Vector3 zero3 = Vector3.Zero;
      Vector3 zero4 = Vector3.Zero;
      zero1.X = num1;
      zero1.Y = -num1;
      zero1.Z = -num2;
      zero2.X = num1;
      zero2.Y = num1;
      zero2.Z = -num2;
      zero3.X = -num1;
      zero3.Y = num1;
      zero3.Z = -num2;
      zero4.X = -num1;
      zero4.Y = -num1;
      zero4.Z = -num2;
      Vector3 vector3_1 = Vector3.Transform(zero1, torchTransform) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(zero2, torchTransform) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(zero3, torchTransform) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(zero4, torchTransform) + blockCenter;
      this.AddTorchFace(vector3_1, vector3_2, vector3_3, vector3_4, 1, tc1, tc2, ref p);
      vector3_1.X = -num1;
      vector3_1.Y = -num1;
      vector3_1.Z = num2;
      vector3_2.X = -num1;
      vector3_2.Y = num1;
      vector3_2.Z = num2;
      vector3_3.X = num1;
      vector3_3.Y = num1;
      vector3_3.Z = num2;
      vector3_4.X = num1;
      vector3_4.Y = -num1;
      vector3_4.Z = num2;
      Vector3 vector3_5 = Vector3.Transform(vector3_1, torchTransform) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(vector3_2, torchTransform) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(vector3_3, torchTransform) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(vector3_4, torchTransform) + blockCenter;
      this.AddTorchFace(vector3_5, vector3_6, vector3_7, vector3_8, 3, tc1, tc2, ref p);
      vector3_5.X = -num2;
      vector3_5.Y = -num1;
      vector3_5.Z = -num1;
      vector3_6.X = -num2;
      vector3_6.Y = num1;
      vector3_6.Z = -num1;
      vector3_7.X = -num2;
      vector3_7.Y = num1;
      vector3_7.Z = num1;
      vector3_8.X = -num2;
      vector3_8.Y = -num1;
      vector3_8.Z = num1;
      Vector3 vector3_9 = Vector3.Transform(vector3_5, torchTransform) + blockCenter;
      Vector3 vector3_10 = Vector3.Transform(vector3_6, torchTransform) + blockCenter;
      Vector3 vector3_11 = Vector3.Transform(vector3_7, torchTransform) + blockCenter;
      Vector3 vector3_12 = Vector3.Transform(vector3_8, torchTransform) + blockCenter;
      this.AddTorchFace(vector3_9, vector3_10, vector3_11, vector3_12, 0, tc1, tc2, ref p);
      vector3_9.X = num2;
      vector3_9.Y = -num1;
      vector3_9.Z = num1;
      vector3_10.X = num2;
      vector3_10.Y = num1;
      vector3_10.Z = num1;
      vector3_11.X = num2;
      vector3_11.Y = num1;
      vector3_11.Z = -num1;
      vector3_12.X = num2;
      vector3_12.Y = -num1;
      vector3_12.Z = -num1;
      this.AddTorchFace(Vector3.Transform(vector3_9, torchTransform) + blockCenter, Vector3.Transform(vector3_10, torchTransform) + blockCenter, Vector3.Transform(vector3_11, torchTransform) + blockCenter, Vector3.Transform(vector3_12, torchTransform) + blockCenter, 2, tc1, tc2, ref p);
    }

    private void AddTorchFace(
      Vector3 pos1,
      Vector3 pos2,
      Vector3 pos3,
      Vector3 pos4,
      int face,
      Vector2 tc1,
      Vector2 tc2,
      ref GlobalPoint3D p)
    {
      byte blockID = 46;
      this.AddVertex(pos1, face, tc1.X, tc2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, face, tc1.X, tc1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, face, tc2.X, tc1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos4, face, tc2.X, tc2.Y, blockID, (byte) 0, ref p);
    }

    private Matrix GetTorchTransform(int auxData)
    {
      return this.torchTransform[auxData % 5];
    }

    private void BuildRopeBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float y = this.tilesize * 0.5f;
      float num1 = this.tilesize * 0.05f;
      float num2 = num1 + num1;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Vector3 vector3 = new Vector3(blockCenter.X - y, blockCenter.Y - y, blockCenter.Z - y);
      Vector3 pos1 = blockCenter - new Vector3(num1, y, num1);
      Vector3 pos2 = pos1;
      pos2.Y += this.tilesize;
      Vector3 pos3 = pos2;
      pos3.X += num2;
      Vector3 pos4 = pos3;
      pos4.Y -= this.tilesize;
      Vector3 pos5 = pos1;
      pos5.Z += num2;
      Vector3 pos6 = pos2;
      pos6.Z += num2;
      Vector3 pos7 = pos3;
      pos7.Z += num2;
      Vector3 pos8 = pos4;
      pos8.Z += num2;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 0]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 0]];
      GlobalPoint3D p1 = p + Point3D.Down;
      bool flag = this.IsClear(p, blockID, 0, 5) && this.GetDataBlock(ref p1).BlockID != (byte) 72;
      this.AddVertex(pos1, 0, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos1.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, 0, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos2.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos6, 0, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos6.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos5, 0, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos5.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos4, 1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos4.X), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, 1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos3.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, 1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos2.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos1, 1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos1.X), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos8, 2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos8.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos7, 2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos7.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, 2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos3.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos4, 2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos4.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos5, 3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos5.X), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos6, 3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos6.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos7, 3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos7.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos8, 3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos8.X), vector2_2.Y, blockID, (byte) 0, ref p);
      if (!flag)
        return;
      this.AddVertex(pos1, 5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos1.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos1.Z), blockID, (byte) 0, ref p);
      this.AddVertex(pos5, 5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos5.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos5.Z), blockID, (byte) 0, ref p);
      this.AddVertex(pos8, 5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos8.Z), blockID, (byte) 0, ref p);
      this.AddVertex(pos4, 5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos4.Z), blockID, (byte) 0, ref p);
    }

    private void BuildSpikes_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num = (byte) ((uint) dataBlockAux >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      blockCenter.X -= 0.3f;
      blockCenter.Z -= 0.3f;
      this.BuildSpikes_NewFormatCore(p, blockCenter, blockID, textureIdForDrawing, dataBlockAux, this.tilesize * 0.5f);
      blockCenter.X += 0.6f;
      blockCenter.Z += 0.2f;
      this.BuildSpikes_NewFormatCore(p, blockCenter, blockID, textureIdForDrawing, dataBlockAux, this.tilesize * 0.65f);
      blockCenter.X -= 0.3f;
      blockCenter.Z += 0.4f;
      this.BuildSpikes_NewFormatCore(p, blockCenter, blockID, textureIdForDrawing, dataBlockAux, this.tilesize * 0.4f);
    }

    private void BuildSpikes_NewFormatCore(
      GlobalPoint3D p,
      Vector3 center,
      byte blockID,
      byte blockIDTexture,
      byte aux,
      float height)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.06f;
      float num3 = num2 + num2;
      float num4 = this.tilesize * 0.3f;
      Vector3 vector3 = new Vector3(center.X - num1, center.Y - num1, center.Z - num1);
      Vector3 position1 = new Vector3(-num2, -num1, -num2);
      Vector3 position2 = position1;
      position2.Y += height;
      Vector3 position3 = position2;
      position3.X += num3;
      Vector3 position4 = position3;
      position4.Y -= height;
      Vector3 position5 = position2;
      position5.Z += num3;
      Vector3 position6 = position1;
      position6.Z += num3;
      Vector3 position7 = position3;
      position7.Z += num3;
      Vector3 position8 = position4;
      position8.Z += num3;
      Vector3 position9 = new Vector3();
      position9.Y = position2.Y + num4;
      Vector3 position10 = new Vector3();
      position10.Y = position2.Y;
      bool flag = ((int) aux & 7) == 5;
      Vector3 pos1;
      Vector3 pos2;
      Vector3 pos3;
      Vector3 pos4;
      Vector3 pos5;
      Vector3 pos6;
      Vector3 pos7;
      Vector3 pos8;
      Vector3 pos9;
      Vector3 pos10;
      if (!flag)
      {
        pos1 = position1 + center;
        pos2 = position2 + center;
        pos3 = position3 + center;
        pos4 = position4 + center;
        pos5 = position5 + center;
        pos6 = position6 + center;
        pos7 = position7 + center;
        pos8 = position8 + center;
        pos9 = position10 + center;
        pos10 = position9 + center;
      }
      else
      {
        Matrix rotationZ = Matrix.CreateRotationZ(3.141593f);
        pos1 = Vector3.Transform(position1, rotationZ) + center;
        pos2 = Vector3.Transform(position2, rotationZ) + center;
        pos3 = Vector3.Transform(position3, rotationZ) + center;
        pos4 = Vector3.Transform(position4, rotationZ) + center;
        pos5 = Vector3.Transform(position5, rotationZ) + center;
        pos6 = Vector3.Transform(position6, rotationZ) + center;
        pos7 = Vector3.Transform(position7, rotationZ) + center;
        pos8 = Vector3.Transform(position8, rotationZ) + center;
        pos9 = Vector3.Transform(position10, rotationZ) + center;
        pos10 = Vector3.Transform(position9, rotationZ) + center;
      }
      int face1 = flag ? 2 : 0;
      int face2 = flag ? 3 : 1;
      int face3 = flag ? 0 : 2;
      int face4 = flag ? 1 : 3;
      int face5 = flag ? 4 : 5;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, 6]];
      Vector2 vector2_3 = (int) blockIDTexture != (int) this.steelSpikesDefaultTextureID ? vector2_1 : MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[237, 6]];
      Vector2 vector2_4 = (int) blockIDTexture != (int) this.steelSpikesDefaultTextureID ? vector2_2 : MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[237, 6]];
      float tx = (float) (((double) vector2_4.X - (double) vector2_3.X) * 0.5) + vector2_3.X;
      this.AddVertex(pos1, face1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos1.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, face1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos2.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos5, face1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos5.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos6, face1, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos6.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      pos9.X = pos1.X;
      pos9.Z = (float) (((double) pos6.Z - (double) pos1.Z) * 0.5) + pos1.Z;
      this.AddVertex(pos9, face1, tx, vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, face1, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.Z, pos2.Z), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos10, face1, tx, vector2_3.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos5, face1, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.Z, pos5.Z), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos4, face2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos4.X), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, face2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos3.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, face2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos2.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos1, face2, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos1.X), vector2_2.Y, blockID, (byte) 0, ref p);
      pos9.Z = pos4.Z;
      pos9.X = (float) (((double) pos4.X - (double) pos1.X) * 0.5) + pos1.X;
      this.AddVertex(pos9, face2, tx, vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, face2, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.X, pos3.X), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos10, face2, tx, vector2_3.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos2, face2, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.X, pos2.X), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos8, face3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos8.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos7, face3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos7.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, face3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos3.Z), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos4, face3, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.Z, pos4.Z), vector2_2.Y, blockID, (byte) 0, ref p);
      pos9.X = pos8.X;
      pos9.Z = (float) (((double) pos8.Z - (double) pos3.Z) * 0.5) + pos3.Z;
      this.AddVertex(pos9, face3, tx, vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos7, face3, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.Z, pos7.Z), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos10, face3, tx, vector2_3.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos3, face3, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.Z, pos3.Z), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos6, face4, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos5.X), vector2_2.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos5, face4, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos6.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos7, face4, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos7.X), vector2_1.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos8, face4, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos8.X), vector2_2.Y, blockID, (byte) 0, ref p);
      pos9.Z = pos6.Z;
      pos9.X = (float) (((double) pos7.X - (double) pos5.X) * 0.5) + pos5.X;
      this.AddVertex(pos9, face4, tx, vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos5, face4, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.X, pos5.X), vector2_4.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos10, face4, tx, vector2_3.Y, blockID, (byte) 0, ref p);
      this.AddVertex(pos7, face4, this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3.X, pos7.X), vector2_4.Y, blockID, (byte) 0, ref p);
      if (!this.IsClear(p, (byte) 119, 0, face5))
        return;
      this.AddVertex(pos1, face5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos1.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos1.Z), blockID, (byte) 0, ref p);
      this.AddVertex(pos6, face5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos6.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos6.Z), blockID, (byte) 0, ref p);
      this.AddVertex(pos8, face5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos8.Z), blockID, (byte) 0, ref p);
      this.AddVertex(pos4, face5, this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3.X, pos4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3.Z, pos4.Z), blockID, (byte) 0, ref p);
    }

    private void BuildStairsBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      if (((int) dataBlockAux & 4) == 0)
        this.BuildStairsBlock_NewFormatUnflipped(p, blockID, dataBlockAux);
      else
        this.BuildStairsBlock_NewFormatFlipped(p, blockID, dataBlockAux);
    }

    private void BuildStairsBlock_NewFormatUnflipped(GlobalPoint3D p, byte blockID, byte aux)
    {
      float x = this.tilesize * 0.5f;
      Vector3 position1 = new Vector3(x, -x, -x);
      Vector3 position2 = position1;
      position2.Y += this.tilesize;
      Vector3 position3 = position2;
      position3.X -= x;
      Vector3 position4 = position3;
      position4.Y -= x;
      Vector3 position5 = position4;
      position5.X -= x;
      Vector3 position6 = position5;
      position6.Y -= x;
      Vector3 position7 = position6;
      position7.X += x;
      Vector3 position8 = position1;
      position8.Z += this.tilesize;
      Vector3 position9 = position2;
      position9.Z += this.tilesize;
      Vector3 position10 = position3;
      position10.Z += this.tilesize;
      Vector3 position11 = position4;
      position11.Z += this.tilesize;
      Vector3 position12 = position5;
      position12.Z += this.tilesize;
      Vector3 position13 = position6;
      position13.Z += this.tilesize;
      Vector3 position14 = position7;
      position14.Z += this.tilesize;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      byte num = (byte) ((uint) aux >> 4);
      aux &= (byte) 7;
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) aux & 3];
      Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_9 = Vector3.Transform(position9, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_10 = Vector3.Transform(position10, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_11 = Vector3.Transform(position11, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_12 = Vector3.Transform(position12, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_13 = Vector3.Transform(position13, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_14 = Vector3.Transform(position14, rotatedBlockMatrix) + blockCenter;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_3 = vector2_1 + (vector2_2 - vector2_1) * 0.5f;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = aux;
      data.IsCorner = false;
      this.SetUpFacePos(ref data);
      if (this.IsClearUp(ref p, blockID, (int) aux, 4))
      {
        data.Face = 4;
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
        this.AddVertex(ref data);
      }
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_10.X;
      data.Y = vector3_10.Y;
      data.Z = vector3_10.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      if (this.IsClear(p, blockID, (int) aux, (int) aux % 4))
      {
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) aux, (1 + (int) aux) % 4))
      {
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
        this.AddVertex(ref data);
        this.AddVertex();
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) aux, (3 + (int) aux) % 4))
      {
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_14.X;
        data.Y = vector3_14.Y;
        data.Z = vector3_14.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
        this.AddVertex(ref data);
        this.AddVertex();
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      data.IsCorner = true;
      if (this.IsClear(p, blockID, (int) aux, (2 + (int) aux) % 4))
        this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
      if (!this.IsClearDown(ref p, blockID, (int) aux, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, textureIdForDrawing);
    }

    private void BuildStairsBlock_NewFormatFlipped(GlobalPoint3D p, byte blockID, byte aux)
    {
      float x = this.tilesize * 0.5f;
      Vector3 position1 = new Vector3(x, -x, -x);
      Vector3 position2 = position1;
      position2.Y += this.tilesize;
      Vector3 position3 = position2;
      position3.X -= x;
      Vector3 position4 = position3;
      position4.X -= x;
      Vector3 position5 = position4;
      position5.Y -= x;
      Vector3 position6 = position5;
      position6.X += x;
      Vector3 position7 = position6;
      position7.Y -= x;
      Vector3 position8 = position1;
      position8.Z += this.tilesize;
      Vector3 position9 = position2;
      position9.Z += this.tilesize;
      Vector3 position10 = position3;
      position10.Z += this.tilesize;
      Vector3 position11 = position4;
      position11.Z += this.tilesize;
      Vector3 position12 = position5;
      position12.Z += this.tilesize;
      Vector3 position13 = position6;
      position13.Z += this.tilesize;
      Vector3 position14 = position7;
      position14.Z += this.tilesize;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      byte num = (byte) ((uint) aux >> 4);
      aux &= (byte) 7;
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) aux & 3];
      Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_9 = Vector3.Transform(position9, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_10 = Vector3.Transform(position10, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_11 = Vector3.Transform(position11, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_12 = Vector3.Transform(position12, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_13 = Vector3.Transform(position13, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_14 = Vector3.Transform(position14, rotatedBlockMatrix) + blockCenter;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_3 = vector2_1 + (vector2_2 - vector2_1) * 0.5f;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = aux;
      data.IsCorner = false;
      this.SetUpFacePos(ref data);
      if (this.IsClearDown(ref p, blockID, (int) aux, 5))
      {
        data.Face = 5;
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_14.X;
        data.Y = vector3_14.Y;
        data.Z = vector3_14.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_14.X;
      data.Y = vector3_14.Y;
      data.Z = vector3_14.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      if (this.IsClear(p, blockID, (int) aux, (int) aux % 4))
      {
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) aux, (1 + (int) aux) % 4))
      {
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_1.Y);
        this.AddVertex(ref data);
        this.AddVertex();
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) aux, (3 + (int) aux) % 4))
      {
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_14.X;
        data.Y = vector3_14.Y;
        data.Z = vector3_14.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_1.Y);
        this.AddVertex(ref data);
        this.AddVertex();
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_3.Y);
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
      }
      data.IsCorner = true;
      if (this.IsClear(p, blockID, (int) aux, (2 + (int) aux) % 4))
        this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
      if (!this.IsClearUp(ref p, blockID, (int) aux, 4))
        return;
      this.AddUpFaceNewFormat(ref p, blockID, textureIdForDrawing);
    }

    private void BuildRamp_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num = (byte) ((uint) dataBlockAux >> 4);
      bool flag = ((int) dataBlockAux & 4) > 0;
      byte aux = (byte) ((uint) dataBlockAux & 7U);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_3 = vector2_1 + (vector2_2 - vector2_1) * 0.5f;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = (byte) 0;
      data.IsCorner = true;
      float x1 = this.tilesize * 0.5f;
      int corner = this.GetCorner(p, blockID, aux, (byte) 150, (byte) 184);
      if (corner >= 0)
      {
        int x2 = p.X;
        int y = p.Y;
        int z = p.Z;
        Vector3 position = this.map.GetPosition(p);
        if (corner < 8)
        {
          switch (corner)
          {
            case 0:
              data.Face = 4;
              --data.Point.X;
              --data.Point.Y;
              this.SetUpFacePos(ref data);
              data.X = position.X;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              ++data.Point.X;
              --data.Point.Z;
              this.SetUpFacePos(ref data);
              data.X += this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              ++data.Point.X;
              data.Point.Y += 2;
              data.Point.Z += 2;
              data.Face = 0;
              this.SetLeftFacePos(ref data);
              data.Y = position.Y;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.X -= 2;
              data.Point.Y -= 2;
              data.Face = 4;
              this.SetUpFacePos(ref data);
              data.X = position.X;
              data.Y -= this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              break;
            case 1:
              data.Face = 4;
              ++data.Point.X;
              --data.Point.Y;
              this.SetUpFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              ++data.Point.Z;
              this.SetUpFacePos(ref data);
              data.Z += this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Point.X -= 2;
              data.Point.Y += 2;
              data.Face = 2;
              this.SetRightFacePos(ref data);
              data.X = position.X;
              data.Y = position.Y;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              --data.Point.X;
              data.Point.Y -= 2;
              data.Point.Z -= 2;
              data.Face = 4;
              this.SetUpFacePos(ref data);
              data.Y -= this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              break;
            case 2:
              data.Face = 4;
              --data.Point.Y;
              ++data.Point.Z;
              this.SetUpFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              --data.Point.X;
              this.SetUpFacePos(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Point.Y += 2;
              data.Point.Z -= 2;
              data.Face = 2;
              this.SetRightFacePos(ref data);
              data.Y = position.Y;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.X += 2;
              data.Point.Y -= 2;
              data.Face = 4;
              this.SetUpFacePos(ref data);
              data.Y -= this.tilesize;
              data.X += this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              break;
            case 3:
              ++data.Point.X;
              ++data.Point.Y;
              --data.Point.Z;
              data.Face = 0;
              this.SetLeftFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Face = 4;
              data.Point.Y -= 2;
              data.Point.Z += 2;
              this.SetUpFacePos(ref data);
              data.Y -= this.tilesize;
              data.Z += this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Point.X -= 2;
              this.SetUpFacePos(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Point.Z -= 2;
              this.SetUpFacePos(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
              this.AddVertex(ref data);
              break;
            case 4:
              data.Face = 4;
              ++data.Point.X;
              ++data.Point.Z;
              --data.Point.Y;
              this.SetUpFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              --data.Point.X;
              --data.Point.Z;
              ++data.Point.Y;
              this.SetUpFacePos(ref data);
              data.X = position.X;
              data.Y = position.Y;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearLeft(ref p, (byte) 1, 0, 0))
                this.AddLeftFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearForward(ref p, (byte) 1, 0, 1))
                this.AddForwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
            case 5:
              this.SetUpFacePos(ref data);
              data.Face = 3;
              data.X = position.X;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 4;
              data.Y = position.Y;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearForward(ref p, blockID, 0, 1))
                this.AddForwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearRight(ref p, blockID, 0, 2))
                this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
            case 6:
              this.SetUpFacePos(ref data);
              data.Face = 0;
              data.X = position.X;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 4;
              data.X = position.X + this.tilesize;
              data.Y = position.Y;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearBackward(ref p, blockID, 0, 3))
                this.AddBackwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearRight(ref p, blockID, 0, 2))
                this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
            case 7:
              this.SetUpFacePos(ref data);
              data.Face = 1;
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 4;
              data.Y = position.Y;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearBackward(ref p, blockID, 0, 3))
                this.AddBackwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearLeft(ref p, blockID, 0, 0))
                this.AddLeftFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
          }
        }
        else
        {
          this.SetDownFacePos(ref data);
          switch (corner)
          {
            case 8:
              ++data.Point.X;
              --data.Point.Y;
              ++data.Point.Z;
              data.Face = 0;
              this.SetLeftFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 5;
              data.Point.Y += 2;
              data.Point.Z -= 2;
              this.SetDownFacePos(ref data);
              data.Y = position.Y;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.X -= 2;
              this.SetDownFacePos(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.Z += 2;
              this.SetDownFacePos(ref data);
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              break;
            case 9:
              --data.Point.X;
              --data.Point.Y;
              ++data.Point.Z;
              data.Face = 2;
              this.SetRightFacePos(ref data);
              data.X = position.X;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 5;
              data.Point.X += 2;
              data.Point.Y += 2;
              this.SetDownFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.Z -= 2;
              this.SetDownFacePos(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.X += 2;
              this.SetDownFacePos(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              break;
            case 10:
              --data.Point.X;
              --data.Point.Y;
              --data.Point.Z;
              data.Face = 2;
              this.SetRightFacePos(ref data);
              data.X = position.X;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 5;
              data.Point.Y += 2;
              data.Point.Z += 2;
              this.SetDownFacePos(ref data);
              data.Y = position.Y;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.X += 2;
              this.SetDownFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.Z -= 2;
              this.SetDownFacePos(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              break;
            case 11:
              ++data.Point.X;
              --data.Point.Y;
              --data.Point.Z;
              data.Face = 3;
              this.SetBackwardFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Face = 5;
              data.Point.X -= 2;
              data.Point.Y += 2;
              this.SetDownFacePos(ref data);
              data.X = position.X;
              data.Y = position.Y;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.Z += 2;
              this.SetDownFacePos(ref data);
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Point.X += 2;
              this.SetDownFacePos(ref data);
              data.X = position.X + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              break;
            case 12:
              data.Face = 2;
              data.X = position.X + this.tilesize;
              data.Y = position.Y;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearLeft(ref p, (byte) 1, 0, 0))
                this.AddLeftFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearForward(ref p, (byte) 1, 0, 1))
                this.AddForwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
            case 13:
              data.Face = 3;
              data.X = position.X;
              data.Y = position.Y;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.X = position.X + this.tilesize;
              data.Y = position.Y - this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearForward(ref p, (byte) 1, 0, 1))
                this.AddForwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearRight(ref p, blockID, 0, 2))
                this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
            case 14:
              data.Face = 0;
              data.X = position.X;
              data.Y = position.Y;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Y = position.Y - this.tilesize;
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearRight(ref p, blockID, 0, 2))
                this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearBackward(ref p, blockID, 0, 3))
                this.AddBackwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
            case 15:
              data.Face = 1;
              data.X = position.X + this.tilesize;
              data.Y = position.Y;
              data.Z = position.Z;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
              this.AddVertex(ref data);
              data.Y = position.Y - this.tilesize;
              data.X = position.X;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.Z = position.Z + this.tilesize;
              data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
              this.AddVertex(ref data);
              data.X = position.X + this.tilesize;
              data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
              this.AddVertex(ref data);
              this.blockData.IsRotated = false;
              if (this.IsClearBackward(ref p, blockID, 0, 3))
                this.AddBackwardFaceNewFormat(ref p, blockID, textureIdForDrawing);
              if (this.IsClearLeft(ref p, (byte) 1, 0, 0))
                this.AddLeftFaceNewFormat(ref p, blockID, textureIdForDrawing);
              this.blockData.IsRotated = true;
              break;
          }
        }
        p.X = x2;
        p.Y = y;
        p.Z = z;
      }
      else
      {
        data.Aux = aux;
        Vector3 position1 = new Vector3(x1, -x1, -x1);
        Vector3 position2 = position1;
        position2.Y += this.tilesize;
        Vector3 position3 = position2;
        position3.X -= this.tilesize;
        if (!flag)
          position3.Y -= this.tilesize;
        Vector3 position4 = position1;
        position4.Z += this.tilesize;
        Vector3 position5 = position2;
        position5.Z += this.tilesize;
        Vector3 position6 = position3;
        position6.Z += this.tilesize;
        Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) aux & 3];
        Vector3 blockCenter = this.map.GetBlockCenter(p);
        Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
        Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
        Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
        Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
        Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
        Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
        if (this.IsClear(p, blockID, (int) aux, (1 + (int) aux) % 4))
        {
          data.Face = 1;
          this.SetFacePos(ref data);
          data.X = vector3_3.X;
          data.Y = vector3_3.Y;
          data.Z = vector3_3.Z;
          data.TC = new NormalizedShort2(vector2_2.X, flag ? vector2_1.Y : vector2_2.Y);
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.Y += 0.01f;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          data.IsCorner = false;
          this.AddVertex(ref data);
          data.IsCorner = true;
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
        }
        if (this.IsClear(p, blockID, (int) aux, (3 + (int) aux) % 4))
        {
          data.Face = 3;
          this.SetFacePos(ref data);
          data.X = vector3_6.X;
          data.Y = vector3_6.Y;
          data.Z = vector3_6.Z;
          data.TC = new NormalizedShort2(vector2_1.X, flag ? vector2_1.Y : vector2_2.Y);
          this.AddVertex(ref data);
          data.X = vector3_5.X;
          data.Y = vector3_5.Y;
          data.Z = vector3_5.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_4.X;
          data.Y = vector3_4.Y + 0.01f;
          data.Z = vector3_4.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          data.IsCorner = false;
          this.AddVertex(ref data);
          data.IsCorner = true;
          data.Y = vector3_4.Y;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
        }
        if (flag)
        {
          int x2 = data.Point.X;
          int z1 = data.Point.Z;
          switch ((int) aux & 3)
          {
            case 0:
              --data.Point.X;
              break;
            case 1:
              --data.Point.Z;
              break;
            case 2:
              ++data.Point.X;
              break;
            case 3:
              ++data.Point.Z;
              break;
          }
          ++data.Point.Y;
          int x3 = data.Point.X;
          int z2 = data.Point.Z;
          data.Face = 5;
          this.SetDownFacePos(ref data);
          data.X = vector3_3.X;
          data.Y = vector3_3.Y;
          data.Z = vector3_3.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_6.X;
          data.Y = vector3_6.Y;
          data.Z = vector3_6.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Point.X = x2 + (x2 - x3);
          data.Point.Y -= 2;
          data.Point.Z = z1 + (z1 - z2);
          switch (this.GetDataBlockID(ref data.Point))
          {
            case 0:
            case 150:
            case 184:
              data.Point.X = x2;
              ++data.Point.Y;
              data.Point.Z = z1;
              this.SetDownFacePos(ref data);
              break;
            default:
              data.Face = 4;
              this.SetFacePos(ref data);
              break;
          }
          data.X = vector3_4.X;
          data.Y = vector3_4.Y;
          data.Z = vector3_4.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
        }
        else
        {
          int x2 = data.Point.X;
          int z1 = data.Point.Z;
          switch ((int) aux & 3)
          {
            case 0:
              --data.Point.X;
              break;
            case 1:
              --data.Point.Z;
              break;
            case 2:
              ++data.Point.X;
              break;
            case 3:
              ++data.Point.Z;
              break;
          }
          --data.Point.Y;
          int x3 = data.Point.X;
          int z2 = data.Point.Z;
          data.Face = 4;
          this.SetUpFacePos(ref data);
          data.X = vector3_6.X;
          data.Y = vector3_6.Y;
          data.Z = vector3_6.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.X = vector3_3.X;
          data.Y = vector3_3.Y;
          data.Z = vector3_3.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Point.X = x2 + (x2 - x3);
          data.Point.Y += 2;
          data.Point.Z = z1 + (z1 - z2);
          switch (this.GetDataBlockID(ref data.Point))
          {
            case 0:
            case 150:
            case 184:
              data.Point.X = x2;
              --data.Point.Y;
              data.Point.Z = z1;
              this.SetUpFacePos(ref data);
              break;
            default:
              this.SetFacePos(ref data);
              break;
          }
          data.X = vector3_2.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_5.X;
          data.Y = vector3_5.Y;
          data.Z = vector3_5.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
        }
        if (this.IsClear(p, blockID, (int) aux, (2 + (int) aux) % 4))
          this.AddRightFaceNewFormat(ref p, blockID, textureIdForDrawing);
      }
      if (flag)
      {
        if (!this.IsClearUp(ref p, blockID, (int) aux, 4))
          return;
        this.AddUpFaceNewFormat(ref p, blockID, textureIdForDrawing);
      }
      else
      {
        if (!this.IsClearDown(ref p, blockID, (int) aux, 5))
          return;
        this.AddDownFaceNewFormat(ref p, blockID, textureIdForDrawing);
      }
    }

    private int GetCorner(GlobalPoint3D p, byte blockID, byte aux, byte blockID1, byte blockID2)
    {
      if (((int) aux & 3) == 0)
      {
        ++p.Z;
        byte blockIdTestBounds1 = this.GetDataBlockID_TestBounds(ref p);
        if (((int) blockIdTestBounds1 == (int) blockID1 || (int) blockIdTestBounds1 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
        {
          p.Z -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if ((int) blockIdTestBounds2 != (int) blockID1 && (int) blockIdTestBounds2 != (int) blockID2)
          {
            ++p.X;
            ++p.Z;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds3 == (int) blockID1 || (int) blockIdTestBounds3 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 1)
              return aux <= (byte) 3 ? 0 : 8;
            p.X -= 2;
            byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 3)
              return aux <= (byte) 3 ? 5 : 13;
          }
        }
        else
        {
          p.Z -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if (((int) blockIdTestBounds2 == (int) blockID1 || (int) blockIdTestBounds2 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
          {
            p.Z += 2;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if ((int) blockIdTestBounds3 != (int) blockID1 && (int) blockIdTestBounds3 != (int) blockID2)
            {
              ++p.X;
              --p.Z;
              byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 3)
                return aux <= (byte) 3 ? 3 : 11;
              p.X -= 2;
              byte blockIdTestBounds5 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds5 == (int) blockID1 || (int) blockIdTestBounds5 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 1)
                return aux <= (byte) 3 ? 6 : 14;
            }
          }
        }
      }
      else if (((int) aux & 3) == 1)
      {
        ++p.X;
        byte blockIdTestBounds1 = this.GetDataBlockID_TestBounds(ref p);
        if (((int) blockIdTestBounds1 == (int) blockID1 || (int) blockIdTestBounds1 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
        {
          p.X -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if ((int) blockIdTestBounds2 != (int) blockID1 && (int) blockIdTestBounds2 != (int) blockID2)
          {
            ++p.X;
            ++p.Z;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds3 == (int) blockID1 || (int) blockIdTestBounds3 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 1)
              return aux <= (byte) 3 ? 0 : 8;
            p.Z -= 2;
            byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 1)
              return aux <= (byte) 3 ? 7 : 15;
          }
        }
        else
        {
          p.X -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if (((int) blockIdTestBounds2 == (int) blockID1 || (int) blockIdTestBounds2 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
          {
            p.X += 2;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if ((int) blockIdTestBounds3 != (int) blockID1 && (int) blockIdTestBounds3 != (int) blockID2)
            {
              --p.X;
              ++p.Z;
              byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 1)
                return aux <= (byte) 3 ? 1 : 9;
              p.Z -= 2;
              byte blockIdTestBounds5 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds5 == (int) blockID1 || (int) blockIdTestBounds5 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 1)
                return aux <= (byte) 3 ? 6 : 14;
            }
          }
        }
      }
      else if (((int) aux & 3) == 2)
      {
        ++p.Z;
        byte blockIdTestBounds1 = this.GetDataBlockID_TestBounds(ref p);
        if (((int) blockIdTestBounds1 == (int) blockID1 || (int) blockIdTestBounds1 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
        {
          p.Z -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if ((int) blockIdTestBounds2 != (int) blockID1 && (int) blockIdTestBounds2 != (int) blockID2)
          {
            --p.X;
            ++p.Z;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds3 == (int) blockID1 || (int) blockIdTestBounds3 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 1)
              return aux <= (byte) 3 ? 1 : 9;
            p.X += 2;
            byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 1)
              return aux <= (byte) 3 ? 4 : 12;
          }
        }
        else
        {
          p.Z -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if (((int) blockIdTestBounds2 == (int) blockID1 || (int) blockIdTestBounds2 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
          {
            p.Z += 2;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if ((int) blockIdTestBounds3 != (int) blockID1 && (int) blockIdTestBounds3 != (int) blockID2)
            {
              --p.X;
              --p.Z;
              byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux + 1)
                return aux <= (byte) 3 ? 2 : 10;
              p.X += 2;
              byte blockIdTestBounds5 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds5 == (int) blockID1 || (int) blockIdTestBounds5 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 1)
                return aux <= (byte) 3 ? 7 : 15;
            }
          }
        }
      }
      else if (((int) aux & 3) == 3)
      {
        ++p.X;
        byte blockIdTestBounds1 = this.GetDataBlockID_TestBounds(ref p);
        if (((int) blockIdTestBounds1 == (int) blockID1 || (int) blockIdTestBounds1 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
        {
          p.X -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if ((int) blockIdTestBounds2 != (int) blockID1 && (int) blockIdTestBounds2 != (int) blockID2)
          {
            ++p.X;
            --p.Z;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds3 == (int) blockID1 || (int) blockIdTestBounds3 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 3)
              return aux <= (byte) 3 ? 3 : 11;
            p.Z += 2;
            byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
            if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 1)
              return aux <= (byte) 3 ? 4 : 12;
          }
        }
        else
        {
          p.X -= 2;
          byte blockIdTestBounds2 = this.GetDataBlockID_TestBounds(ref p);
          if (((int) blockIdTestBounds2 == (int) blockID1 || (int) blockIdTestBounds2 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux)
          {
            p.X += 2;
            byte blockIdTestBounds3 = this.GetDataBlockID_TestBounds(ref p);
            if ((int) blockIdTestBounds3 != (int) blockID1 && (int) blockIdTestBounds3 != (int) blockID2)
            {
              --p.X;
              --p.Z;
              byte blockIdTestBounds4 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds4 == (int) blockID1 || (int) blockIdTestBounds4 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 1)
                return aux <= (byte) 3 ? 2 : 10;
              p.Z += 2;
              byte blockIdTestBounds5 = this.GetDataBlockID_TestBounds(ref p);
              if (((int) blockIdTestBounds5 == (int) blockID1 || (int) blockIdTestBounds5 == (int) blockID2) && ((int) this.GetDataBlockAux(ref p) & 7) == (int) aux - 3)
                return aux <= (byte) 3 ? 5 : 13;
            }
          }
        }
      }
      return -1;
    }

    private void BuildHalfBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num2 = (byte) ((uint) dataBlockAux >> 4);
      byte num3 = (byte) ((uint) dataBlockAux & 3U);
      bool flag1 = num3 == (byte) 0 || this.IsClearUp(ref p, blockID, (int) num3, 4);
      bool flag2 = this.IsClearLeft(ref p, blockID, (int) num3, 0);
      bool flag3 = this.IsClearForward(ref p, blockID, (int) num3, 1);
      bool flag4 = this.IsClearRight(ref p, blockID, (int) num3, 2);
      bool flag5 = this.IsClearBackward(ref p, blockID, (int) num3, 3);
      bool flag6 = num3 == (byte) 1 || this.IsClearDown(ref p, blockID, (int) num3, 5);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num2);
      AVParams data = new AVParams();
      data.BlockID = blockID;
      data.Point = p;
      data.IsCorner = false;
      if (flag2)
      {
        data.Face = 0;
        this.SetLeftFacePos(ref data);
        this.AddHalfBlockSideFace(ref data, (int) num3, textureIdForDrawing);
      }
      if (flag3)
      {
        data.Face = 1;
        this.SetForwardFacePos(ref data);
        this.AddHalfBlockSideFace(ref data, (int) num3, textureIdForDrawing);
      }
      if (flag4)
      {
        data.Face = 2;
        this.SetRightFacePos(ref data);
        this.AddHalfBlockSideFace(ref data, (int) num3, textureIdForDrawing);
      }
      if (flag5)
      {
        data.Face = 3;
        this.SetBackwardFacePos(ref data);
        this.AddHalfBlockSideFace(ref data, (int) num3, textureIdForDrawing);
      }
      if (flag1)
      {
        data.IsCorner = true;
        data.Face = 4;
        this.SetUpFacePos(ref data);
        Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
        Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
        data.X = data.Pos1.X;
        data.Y = data.Pos1.Y;
        data.Z = data.Pos1.Z;
        if (num3 == (byte) 0)
        {
          data.Y -= num1;
          data.IsCorner = false;
        }
        else
          data.IsCorner = true;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = data.Pos2.X;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = data.Pos2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = data.Pos1.X;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (!flag6)
        return;
      data.IsCorner = true;
      data.Face = 5;
      this.SetDownFacePos(ref data);
      Vector2 vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 5]];
      Vector2 vector2_4 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 5]];
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y;
      data.Z = data.Pos2.Z;
      if (num3 == (byte) 1)
      {
        data.Y += num1;
        data.IsCorner = false;
      }
      else
        data.IsCorner = true;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = data.Pos1.X;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_4.Y);
      this.AddVertex(ref data);
    }

    private void AddHalfBlockSideFace(ref AVParams data, int aux, byte blockIDTexture)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, 6]];
      float num = this.tilesize * 0.5f;
      float y1 = data.Pos1.Y;
      float y2 = data.Pos2.Y;
      float y3 = vector2_1.Y;
      float y4 = vector2_2.Y;
      if (aux == 1)
      {
        y1 += num;
        y4 = y3 - (float) (((double) y3 - (double) y4) * 0.5);
      }
      else
      {
        y2 -= num;
        y3 = y4 + (float) (((double) y3 - (double) y4) * 0.5);
      }
      data.X = data.Pos1.X;
      data.Y = y1;
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, y4);
      this.AddVertex(ref data);
      data.Y = y2;
      data.TC = new NormalizedShort2(vector2_1.X, y3);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, y3);
      this.AddVertex(ref data);
      data.Y = y1;
      data.TC = new NormalizedShort2(vector2_2.X, y4);
      this.AddVertex(ref data);
    }

    private void BuildStack_NewFormat(GlobalPoint3D p, byte blockID)
    {
      AVParams data = new AVParams();
      data.BlockID = blockID;
      data.Point = p;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num1 = (byte) ((uint) dataBlockAux >> 4);
      byte num2 = (byte) ((uint) dataBlockAux & 7U);
      data.IsCorner = num2 == (byte) 7;
      float scale = (float) ((int) num2 + 1) * 0.125f;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num1);
      if (this.IsClearLeft(ref p, blockID, (int) num2, 0))
      {
        data.Face = 0;
        this.SetLeftFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, textureIdForDrawing, 6);
      }
      if (this.IsClearForward(ref p, blockID, (int) num2, 1))
      {
        data.Face = 1;
        this.SetForwardFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, textureIdForDrawing, 6);
      }
      if (this.IsClearRight(ref p, blockID, (int) num2, 2))
      {
        data.Face = 2;
        this.SetRightFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, textureIdForDrawing, 6);
      }
      if (this.IsClearBackward(ref p, blockID, (int) num2, 3))
      {
        data.Face = 3;
        this.SetBackwardFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, textureIdForDrawing, 6);
      }
      if (num2 < (byte) 7 || this.IsClearUp(ref p, blockID, (int) num2, 4))
      {
        data.IsCorner = true;
        data.Face = 4;
        this.SetUpFacePos(ref data);
        Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
        Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
        data.X = data.Pos1.X;
        data.Y = (float) ((double) data.Pos1.Y - (double) this.tilesize + (double) this.tilesize * (double) scale);
        data.Z = data.Pos1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = data.Pos2.X;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = data.Pos2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = data.Pos1.X;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, textureIdForDrawing);
    }

    private void AddVariableHeightSideFace(
      ref AVParams data,
      float scale,
      byte blockIDTexture,
      int texface)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, texface]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, texface]];
      float y1 = vector2_1.Y;
      float y2 = vector2_2.Y;
      float y3 = y2 - (y2 - y1) * scale;
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y;
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, y2);
      this.AddVertex(ref data);
      data.Y = data.Pos1.Y + scale * this.tilesize;
      data.TC = new NormalizedShort2(vector2_1.X, y3);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, y3);
      this.AddVertex(ref data);
      data.Y = data.Pos1.Y;
      data.TC = new NormalizedShort2(vector2_2.X, y2);
      this.AddVertex(ref data);
    }

    private void BuildUpsideDownStack_NewFormat(GlobalPoint3D p, byte blockID)
    {
      AVParams data = new AVParams();
      data.BlockID = blockID;
      data.Point = p;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num1 = (byte) ((uint) dataBlockAux >> 4);
      byte num2 = (byte) ((uint) dataBlockAux & 7U);
      data.IsCorner = num2 == (byte) 7;
      float scale = (float) ((int) num2 + 1) * 0.125f;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num1);
      if (this.IsClearLeft(ref p, blockID, (int) num2, 0))
      {
        data.Face = 0;
        this.SetLeftFacePos(ref data);
        this.AddVariableHeightUpsideDownSideFace(ref data, scale, textureIdForDrawing);
      }
      if (this.IsClearForward(ref p, blockID, (int) num2, 1))
      {
        data.Face = 1;
        this.SetForwardFacePos(ref data);
        this.AddVariableHeightUpsideDownSideFace(ref data, scale, textureIdForDrawing);
      }
      if (this.IsClearRight(ref p, blockID, (int) num2, 2))
      {
        data.Face = 2;
        this.SetRightFacePos(ref data);
        this.AddVariableHeightUpsideDownSideFace(ref data, scale, textureIdForDrawing);
      }
      if (this.IsClearBackward(ref p, blockID, (int) num2, 3))
      {
        data.Face = 3;
        this.SetBackwardFacePos(ref data);
        this.AddVariableHeightUpsideDownSideFace(ref data, scale, textureIdForDrawing);
      }
      if (this.IsClearUp(ref p, blockID, (int) num2, 4))
        this.AddUpFaceNewFormat(ref p, blockID, textureIdForDrawing);
      if (num2 >= (byte) 7 && !this.IsClearDown(ref p, blockID, 0, 5))
        return;
      data.IsCorner = true;
      data.Face = 5;
      this.SetDownFacePos(ref data);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 5]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 5]];
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y - this.tilesize * scale + this.tilesize;
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = data.Pos1.X;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
    }

    private void AddVariableHeightUpsideDownSideFace(
      ref AVParams data,
      float scale,
      byte blockIDTexture)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, data.Face]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, data.Face]];
      float y1 = vector2_1.Y;
      float y2 = vector2_2.Y;
      float y3 = y1 + (y2 - y1) * scale;
      data.X = data.Pos1.X;
      data.Y = data.Pos2.Y - scale * this.tilesize;
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, y3);
      this.AddVertex(ref data);
      data.Y = data.Pos2.Y;
      data.TC = new NormalizedShort2(vector2_1.X, y1);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, y1);
      this.AddVertex(ref data);
      data.Y = data.Pos2.Y - scale * this.tilesize;
      data.TC = new NormalizedShort2(vector2_2.X, y3);
      this.AddVertex(ref data);
    }

    private void BuildCylinderBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = (byte) 0;
      data.IsCorner = false;
      data.UseOwnLight = false;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num1 = (byte) ((uint) dataBlockAux >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num1);
      byte aux = (byte) ((uint) dataBlockAux & 7U);
      float num2 = this.tilesize * 0.5f;
      Vector3 position = this.map.GetPosition(p);
      position.X += num2;
      position.Y -= num2;
      position.Z += num2;
      Vector3 cylinderVertex1 = VoxelMeshBuilder.cylinderVertices[0];
      data.Pos1.X = cylinderVertex1.X + position.X;
      data.Pos1.Y = cylinderVertex1.Y + position.Y;
      data.Pos1.Z = cylinderVertex1.Z + position.Z;
      Vector3 cylinderVertex2 = VoxelMeshBuilder.cylinderVertices[2];
      data.Pos2.X = cylinderVertex2.X + position.X;
      data.Pos2.Y = cylinderVertex2.Y + position.Y;
      data.Pos2.Z = cylinderVertex2.Z + position.Z;
      data.Face = this.GetBestFaceForCylinder(p);
      this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux, position, data, Vector2.Zero);
      float x = 0.591f;
      if (aux == (byte) 1 || aux == (byte) 3 || aux > (byte) 3)
      {
        --p.X;
        MapBlock dataBlock1 = this.GetDataBlock(ref p);
        dataBlock1.AuxData &= (byte) 7;
        p.X += 2;
        MapBlock dataBlock2 = this.GetDataBlock(ref p);
        dataBlock2.AuxData &= (byte) 7;
        --p.X;
        bool flag1 = dataBlock1.BlockID == (byte) 154 && (dataBlock1.AuxData == (byte) 0 || dataBlock1.AuxData == (byte) 2);
        bool flag2 = dataBlock2.BlockID == (byte) 154 && (dataBlock2.AuxData == (byte) 0 || dataBlock2.AuxData == (byte) 2);
        if (flag1 && flag2)
          this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux == (byte) 1 ? (byte) 0 : (byte) 2, position, data, Vector2.Zero);
        else if (flag1)
          this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux == (byte) 1 ? (byte) 0 : (byte) 2, position, data, aux == (byte) 1 ? new Vector2(x, 0.0f) : new Vector2(x, 0.0f));
        else if (flag2)
          this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux == (byte) 1 ? (byte) 0 : (byte) 2, position, data, aux == (byte) 1 ? new Vector2(0.0f, -x) : new Vector2(0.0f, -x));
      }
      if (aux == (byte) 0 || aux == (byte) 2 || aux > (byte) 3)
      {
        --p.Z;
        MapBlock dataBlock1 = this.GetDataBlock(ref p);
        dataBlock1.AuxData &= (byte) 7;
        p.Z += 2;
        MapBlock dataBlock2 = this.GetDataBlock(ref p);
        dataBlock2.AuxData &= (byte) 7;
        --p.Z;
        bool flag1 = dataBlock1.BlockID == (byte) 154 && (dataBlock1.AuxData == (byte) 1 || dataBlock1.AuxData == (byte) 3);
        bool flag2 = dataBlock2.BlockID == (byte) 154 && (dataBlock2.AuxData == (byte) 1 || dataBlock2.AuxData == (byte) 3);
        if (flag1 && flag2)
          this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux == (byte) 0 ? (byte) 1 : (byte) 3, position, data, Vector2.Zero);
        else if (flag1)
          this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux == (byte) 0 ? (byte) 1 : (byte) 3, position, data, aux == (byte) 0 ? new Vector2(x, 0.0f) : new Vector2(x, 0.0f));
        else if (flag2)
          this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, aux == (byte) 0 ? (byte) 1 : (byte) 3, position, data, aux == (byte) 0 ? new Vector2(0.0f, -x) : new Vector2(0.0f, -x));
      }
      if (aux >= (byte) 4)
        return;
      --p.Y;
      MapBlock dataBlock3 = this.GetDataBlock(ref p);
      dataBlock3.AuxData &= (byte) 7;
      p.Y += 2;
      MapBlock dataBlock4 = this.GetDataBlock(ref p);
      dataBlock4.AuxData &= (byte) 7;
      --p.Y;
      bool flag3 = dataBlock4.BlockID == (byte) 154 && dataBlock4.AuxData > (byte) 3;
      bool flag4 = dataBlock3.BlockID == (byte) 154 && dataBlock3.AuxData > (byte) 3;
      if (flag3 && flag4)
        this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, (byte) 4, position, data, Vector2.Zero);
      else if (flag3)
      {
        this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, (byte) 4, position, data, new Vector2(x, 0.0f));
      }
      else
      {
        if (!flag4)
          return;
        this.BuildCylinderCoreBlock_NewFormat(p, blockID, textureIdForDrawing, (byte) 4, position, data, new Vector2(0.0f, -x));
      }
    }

    private void BuildCylinderCoreBlock_NewFormat(
      GlobalPoint3D p,
      byte blockID,
      byte blockIDTexture,
      byte aux,
      Vector3 pos,
      AVParams data,
      Vector2 adjY)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, 6]];
      if ((double) adjY.X != 0.0)
        vector2_2.Y -= (float) (((double) vector2_2.Y - (double) vector2_1.Y) * (1.0 - (double) Math.Abs(adjY.X)));
      else if ((double) adjY.Y != 0.0)
        vector2_1.Y += (float) (((double) vector2_2.Y - (double) vector2_1.Y) * (1.0 - (double) Math.Abs(adjY.Y)));
      int num1 = 4;
      float num2 = (vector2_2.X - vector2_1.X) / (float) num1;
      Vector2 vector2_3 = vector2_1;
      Vector2 vector2_4 = vector2_2;
      vector2_4.X = vector2_3.X + num2;
      bool flag1 = (double) adjY.X != 0.0 || (double) adjY.Y != 0.0;
      Vector3 position = new Vector3();
      Matrix matrix1 = aux < (byte) 4 ? Matrix.CreateRotationZ(1.570796f) : Matrix.Identity;
      int num3 = (int) aux & 3;
      bool flag2 = aux < (byte) 4 && (num3 == 0 || num3 == 2);
      for (int index = 0; index < VoxelMeshBuilder.cylinderVertices.Length; ++index)
      {
        int num4 = index % 4;
        Vector3 cylinderVertex1 = VoxelMeshBuilder.cylinderVertices[index];
        if (aux < (byte) 4)
        {
          position.X = cylinderVertex1.X;
          position.Y = cylinderVertex1.Y;
          position.Z = cylinderVertex1.Z;
          if (flag1)
          {
            if (num4 == 0 || num4 == 3)
              position.Y += adjY.X;
            else
              position.Y += adjY.Y;
          }
          position = Vector3.Transform(position, matrix1 * MapTM.RotatedBlockMatrices[flag2 ? 0 : 1]);
          data.X = position.X;
          data.Y = position.Y;
          data.Z = position.Z;
        }
        else
        {
          data.X = cylinderVertex1.X;
          data.Y = cylinderVertex1.Y;
          data.Z = cylinderVertex1.Z;
          if (flag1)
          {
            if (num4 == 0 || num4 == 3)
              data.Y += adjY.X;
            else
              data.Y += adjY.Y;
          }
        }
        data.X += pos.X;
        data.Y += pos.Y;
        data.Z += pos.Z;
        data.TC = new NormalizedShort2(num4 == 0 || num4 == 1 ? vector2_4.X : vector2_3.X, num4 == 1 || num4 == 2 ? vector2_3.Y : vector2_4.Y);
        this.AddVertex(ref data);
        if (num4 == 3)
        {
          vector2_3.X = vector2_4.X;
          vector2_4.X += num2;
          if ((double) vector2_4.X > (double) vector2_2.X)
          {
            vector2_3.X = vector2_1.X;
            vector2_4.X = vector2_3.X + num2;
          }
          data.Pos1.X = data.Pos2.X;
          data.Pos1.Z = data.Pos2.Z;
          Vector3 cylinderVertex2 = VoxelMeshBuilder.cylinderVertices[index + 3 < VoxelMeshBuilder.cylinderVertices.Length ? index + 3 : index + 3 - VoxelMeshBuilder.cylinderVertices.Length];
          data.Pos2.X = cylinderVertex2.X + pos.X;
          data.Pos2.Z = cylinderVertex2.Z + pos.Z;
        }
      }
      if (flag1)
        return;
      VoxelMeshBuilder.CylinderCapData cylinderCapData = new VoxelMeshBuilder.CylinderCapData();
      bool flag3 = false;
      bool flag4 = false;
      if (aux > (byte) 3)
      {
        flag3 = this.IsClearUp(ref p, blockID, (int) aux, 4);
        flag4 = this.IsClearDown(ref p, blockID, (int) aux, 5);
      }
      else
      {
        switch (aux)
        {
          case 0:
            flag3 = this.IsClearLeft(ref p, blockID, (int) aux, 0);
            flag4 = this.IsClearRight(ref p, blockID, (int) aux, 2);
            break;
          case 1:
            flag3 = this.IsClearForward(ref p, blockID, (int) aux, 1);
            flag4 = this.IsClearBackward(ref p, blockID, (int) aux, 3);
            break;
          case 2:
            flag3 = this.IsClearRight(ref p, blockID, (int) aux, 2);
            flag4 = this.IsClearLeft(ref p, blockID, (int) aux, 0);
            break;
          case 3:
            flag3 = this.IsClearBackward(ref p, blockID, (int) aux, 3);
            flag4 = this.IsClearForward(ref p, blockID, (int) aux, 1);
            break;
        }
      }
      float num5 = (float) GraphicStatics.TexturePack.BlockTextureSize();
      float num6 = num5 / (float) GraphicStatics.TexturePack.BlockTexture.Width;
      float num7 = num5 / (float) GraphicStatics.TexturePack.BlockTexture.Height;
      Vector2 vector2_5;
      if (flag3)
      {
        Vector2 vector2_6 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, 4]];
        vector2_5 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, 4]];
        for (int index = 0; index < VoxelMeshBuilder.cylinderCapVertices.Length; ++index)
        {
          VoxelMeshBuilder.CylinderCapData cylinderCapVertex = VoxelMeshBuilder.cylinderCapVertices[index];
          if (aux < (byte) 4)
          {
            position = cylinderCapVertex.Position;
            position = Vector3.Transform(position, matrix1 * MapTM.RotatedBlockMatrices[(int) aux & 3]);
            data.X = position.X;
            data.Y = position.Y;
            data.Z = position.Z;
          }
          else
          {
            data.X = cylinderCapVertex.Position.X;
            data.Y = cylinderCapVertex.Position.Y;
            data.Z = cylinderCapVertex.Position.Z;
          }
          data.X += pos.X;
          data.Y += pos.Y;
          data.Z += pos.Z;
          data.TC = new NormalizedShort2((cylinderCapVertex.Position.X + 0.5f) * num6 + vector2_6.X, (cylinderCapVertex.Position.Z + 0.5f) * num7 + vector2_6.Y);
          this.AddVertex(ref data);
        }
      }
      if (!flag4)
        return;
      Vector2 vector2_7 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, 5]];
      vector2_5 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, 5]];
      Matrix matrix2 = Matrix.CreateRotationX(3.141593f) * matrix1;
      for (int index = 0; index < VoxelMeshBuilder.cylinderCapVertices.Length; ++index)
      {
        VoxelMeshBuilder.CylinderCapData cylinderCapVertex = VoxelMeshBuilder.cylinderCapVertices[index];
        position = cylinderCapVertex.Position;
        position = Vector3.Transform(position, matrix2 * MapTM.RotatedBlockMatrices[(int) aux & 3]);
        data.X = position.X;
        data.Y = position.Y;
        data.Z = position.Z;
        data.X += pos.X;
        data.Y += pos.Y;
        data.Z += pos.Z;
        data.TC = new NormalizedShort2((cylinderCapVertex.Position.X + 0.5f) * num6 + vector2_7.X, (cylinderCapVertex.Position.Z + 0.5f) * num7 + vector2_7.Y);
        this.AddVertex(ref data);
      }
    }

    private int GetBestFaceForCylinder(GlobalPoint3D p)
    {
      GlobalPoint3D min = this.mapBound.Min;
      GlobalPoint3D max = this.mapBound.Max;
      byte num1 = 2;
      byte num2 = 2;
      byte num3 = 2;
      if (p.X > min.X)
      {
        --p.X;
        num2 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
        if (p.Z > min.Z)
        {
          --p.Z;
          num3 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
          ++p.Z;
        }
        if (p.Z < max.Z - 1)
        {
          ++p.Z;
          num1 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
          --p.Z;
        }
        ++p.X;
        if (num1 > (byte) 0 && num2 > (byte) 0 && num3 > (byte) 0)
          return 0;
      }
      byte num4 = 2;
      byte num5 = 2;
      byte num6 = 2;
      if (p.X < max.X - 1)
      {
        ++p.X;
        num5 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
        if (p.Z > min.Z)
        {
          --p.Z;
          num6 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
          ++p.Z;
        }
        if (p.Z < max.Z - 1)
        {
          ++p.Z;
          num4 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
          --p.Z;
        }
        --p.X;
        if (num4 > (byte) 0 && num5 > (byte) 0 && num6 > (byte) 0)
          return 2;
      }
      byte num7 = 2;
      if (p.Z > min.Z)
      {
        --p.Z;
        num7 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
        if (num3 > (byte) 0 && num7 > (byte) 0 && num6 > (byte) 0)
          return 1;
      }
      byte num8 = 2;
      if (p.Z < max.Z - 1)
      {
        ++p.Z;
        num8 = this.map.BlockData[(int) this.GetDataBlockID(ref p)].Buffer;
        if (num1 > (byte) 0 && num8 > (byte) 0 && num4 > (byte) 0)
          return 3;
      }
      if (num2 > (byte) 0 && (num1 > (byte) 0 || num3 > (byte) 0))
        return 0;
      if (num5 > (byte) 0 && (num4 > (byte) 0 || num6 > (byte) 0))
        return 2;
      if (num7 > (byte) 0 && (num3 > (byte) 0 || num6 > (byte) 0))
        return 1;
      if (num8 > (byte) 0 && (num1 > (byte) 0 || num4 > (byte) 0))
        return 3;
      if (num2 > (byte) 0)
        return 0;
      if (num5 > (byte) 0)
        return 2;
      if (num7 > (byte) 0)
        return 1;
      return num8 > (byte) 0 ? 3 : 4;
    }

    private void BuildCylinderBlock_NewFormat2(GlobalPoint3D p, byte blockID)
    {
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = (byte) 0;
      data.IsCorner = false;
      data.UseOwnLight = false;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num1 = (byte) ((uint) dataBlockAux >> 4);
      byte num2 = (byte) ((uint) dataBlockAux & 7U);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num1);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      int num3 = 4;
      float num4 = (vector2_2.X - vector2_1.X) / (float) num3;
      Vector2 vector2_3 = vector2_1;
      Vector2 vector2_4 = vector2_2;
      vector2_4.X = vector2_3.X + num4;
      Vector3 position = this.map.GetPosition(p);
      position.X += this.tilesize * 0.5f;
      position.Z += this.tilesize * 0.5f;
      if (this.tempCylPos == null)
        this.tempCylPos = new Vector3[VoxelMeshBuilder.cylinderVertices.Length];
      VoxelMeshBuilder.cylinderVertices.CopyTo((Array) this.tempCylPos, 0);
      Matrix rotationY = Matrix.CreateRotationY(-0.7853982f);
      for (int index = 0; index < this.tempCylPos.Length; ++index)
        this.tempCylPos[index] = Vector3.Transform(this.tempCylPos[index], rotationY);
      Vector3 tempCylPo = this.tempCylPos[0];
      data.Pos1.X = tempCylPo.X + position.X;
      data.Pos1.Y = tempCylPo.Y + position.Y;
      data.Pos1.Z = tempCylPo.Z + position.Z;
      tempCylPo = this.tempCylPos[2];
      data.Pos2.X = tempCylPo.X + position.X;
      data.Pos2.Y = tempCylPo.Y + position.Y;
      data.Pos2.Z = tempCylPo.Z + position.Z;
      int num5 = 0;
      data.Face = 3;
      for (int index = 0; index < this.tempCylPos.Length; ++index)
      {
        tempCylPo = this.tempCylPos[index];
        data.X = tempCylPo.X + position.X;
        data.Y = tempCylPo.Y + position.Y;
        data.Z = tempCylPo.Z + position.Z;
        int num6 = index % 4;
        data.TC = new NormalizedShort2(num6 == 0 || num6 == 1 ? vector2_3.X : vector2_4.X, num6 == 1 || num6 == 2 ? vector2_3.Y : vector2_4.Y);
        this.AddVertex(ref data);
        if (num6 == 3)
        {
          vector2_3.X = vector2_4.X;
          vector2_4.X += num4;
          if ((double) vector2_4.X > (double) vector2_2.X)
          {
            vector2_3.X = vector2_1.X;
            vector2_4.X = vector2_3.X + num4;
          }
          data.Pos1.X = data.Pos2.X;
          data.Pos1.Z = data.Pos2.Z;
          tempCylPo = this.tempCylPos[index + 3 < this.tempCylPos.Length ? index + 3 : index + 3 - this.tempCylPos.Length];
          data.Pos2.X = tempCylPo.X + position.X;
          data.Pos2.Z = tempCylPo.Z + position.Z;
          if (++num5 == 4)
          {
            if (++data.Face > 3)
              data.Face = 0;
            num5 = 0;
          }
        }
      }
    }

    private void BuildPaintingBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.05f;
      float num3 = num2 + num2;
      float num4 = num1;
      float num5 = num1;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num6 = (byte) ((uint) dataBlockAux >> 4);
      byte num7 = (byte) ((uint) dataBlockAux & 7U);
      Vector3 position1 = new Vector3(num1 - num3, -num4, -num5);
      Vector3 position2 = position1;
      position2.Y += num4 * 2f;
      Vector3 position3 = position2;
      position3.Z += num5 * 2f;
      Vector3 position4 = position3;
      position4.Y -= num4 * 2f;
      Vector3 position5 = position1;
      position5.X += num3;
      Vector3 position6 = position2;
      position6.X += num3;
      Vector3 position7 = position3;
      position7.X += num3;
      Vector3 position8 = position4;
      position8.X += num3;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      bool flag = ((int) num7 & 4) > 0;
      Matrix matrix = (flag ? MapTM.RotatedBlockMatrices[4] : Matrix.Identity) * MapTM.RotatedBlockMatrices[(int) num7 & 3];
      Vector3 vector3_1 = Vector3.Transform(position1, matrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, matrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, matrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, matrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, matrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, matrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, matrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, matrix) + blockCenter;
      byte num8 = blockID;
      int index = 384 + (int) num6 / 8 * 32 - 8 + (int) num6 % 8;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num7;
      data.IsCorner = false;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[index];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[index];
      data.UseOwnLight = true;
      data.Face = flag ? 4 : 0;
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.UseOwnLight = false;
      if (this.IsClear(p, blockID, (int) num7, (1 + (int) num7) % 4))
      {
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num8, 1]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num8, 1]];
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (flag ? this.IsClearDown(ref p, blockID, (int) num7, 2) : this.IsClear(p, blockID, (int) num7, (2 + (int) num7) % 4))
      {
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num8, 2]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num8, 2]];
        data.Face = flag ? 5 : 2;
        this.SetFacePos(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) num7, (3 + (int) num7) % 4))
      {
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num8, 3]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num8, 3]];
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (flag ? this.IsClear(p, blockID, (int) num7, (2 + (int) num7) % 4) : this.IsClearUp(ref p, blockID, (int) num7, 4))
      {
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num8, 4]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num8, 4]];
        data.Face = 4;
        this.SetUpFacePos(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (!(flag ? this.IsClear(p, blockID, (int) num7, (int) num7 % 4) : this.IsClearDown(ref p, blockID, (int) num7, 5)))
        return;
      vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num8, 5]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num8, 5]];
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, ref data), vector2_2.Y);
      this.AddVertex(ref data);
    }

    private void BuildSwitchBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.05f;
      float num3 = num2 + num2;
      float num4 = this.tilesize * 0.2f;
      float num5 = this.tilesize * 0.2f;
      Vector3 position1 = new Vector3(-num2, -num4, -num5);
      byte num6 = (byte) ((uint) this.GetDataBlockAux(ref p) & 7U);
      position1.X += num1 - num2;
      Vector3 position2 = position1;
      position2.Y += num4 * 2f;
      Vector3 position3 = position2;
      position3.Z += num5 * 2f;
      Vector3 position4 = position3;
      position4.Y -= num4 * 2f;
      Vector3 position5 = position1;
      position5.X += num3;
      Vector3 position6 = position2;
      position6.X += num3;
      Vector3 position7 = position3;
      position7.X += num3;
      Vector3 position8 = position4;
      position8.X += num3;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) num6];
      Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, rotatedBlockMatrix) + blockCenter;
      byte num7 = blockID;
      int index = this.strategy.IsBlockDeliveringPower(p) ? 2 : 0;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num6;
      data.IsCorner = false;
      data.UseOwnLight = true;
      float num8 = 3f / 512f;
      float num9 = 3f / 208f;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num7, index]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_1.X += num8;
      vector2_1.Y += num9;
      vector2_2.X -= num8;
      vector2_2.Y -= num9;
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      int num10;
      switch (num6)
      {
        case 4:
          num10 = this.IsClearDown(ref p, blockID, (int) num6, 2) ? 1 : 0;
          break;
        case 5:
          num10 = this.IsClearUp(ref p, blockID, (int) num6, 2) ? 1 : 0;
          break;
        default:
          num10 = this.IsClear(p, blockID, (int) num6, (2 + (int) num6) % 4) ? 1 : 0;
          break;
      }
      if (num10 != 0)
      {
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      Vector2 vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_3.Y += num9;
      vector2_2.X = vector2_3.X + num8;
      vector2_2.Y -= num9;
      data.Face = 1;
      this.SetFacePos(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_3.Y += num9;
      vector2_3.X = vector2_2.X - num8;
      vector2_2.Y -= num9;
      data.Face = 3;
      this.SetFacePos(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_3.X += num8;
      vector2_2.X -= num8;
      vector2_2.Y = vector2_3.Y + num9;
      data.Face = 4;
      this.SetUpFacePos(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num7, index]];
      vector2_3.X += num8;
      vector2_2.X -= num8;
      vector2_3.Y = vector2_2.Y - num9;
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
    }

    private void BuildButtonBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.06f;
      float num3 = num2 + num2;
      float num4 = this.tilesize * 0.15f;
      float num5 = this.tilesize * 0.15f;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num6 = (byte) ((uint) dataBlockAux >> 4);
      byte num7 = (byte) ((uint) dataBlockAux & 7U);
      if (this.strategy.IsBlockDeliveringPower(p))
      {
        num2 = 0.02f;
        num3 = num2 + num2;
      }
      Vector3 position1 = new Vector3(-num2, -num4, -num5);
      position1.X += num1 - num2;
      Vector3 position2 = position1;
      position2.Y += num4 * 2f;
      Vector3 position3 = position2;
      position3.Z += num5 * 2f;
      Vector3 position4 = position3;
      position4.Y -= num4 * 2f;
      Vector3 position5 = position1;
      position5.X += num3;
      Vector3 position6 = position2;
      position6.X += num3;
      Vector3 position7 = position3;
      position7.X += num3;
      Vector3 position8 = position4;
      position8.X += num3;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) num7];
      Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, rotatedBlockMatrix) + blockCenter;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num6);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num7;
      data.IsCorner = false;
      data.UseOwnLight = true;
      float num8 = 3f / 512f;
      float num9 = 3f / 208f;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 0]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 0]];
      vector2_1.X += num8;
      vector2_1.Y += num9;
      vector2_2.X -= num8;
      vector2_2.Y -= num9;
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      int num10;
      switch (num7)
      {
        case 4:
          num10 = this.IsClearDown(ref p, blockID, (int) num7, 2) ? 1 : 0;
          break;
        case 5:
          num10 = this.IsClearUp(ref p, blockID, (int) num7, 2) ? 1 : 0;
          break;
        default:
          num10 = this.IsClear(p, blockID, (int) num7, (2 + (int) num7) % 4) ? 1 : 0;
          break;
      }
      if (num10 != 0)
      {
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      Vector2 vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 1]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 1]];
      vector2_3.Y += num9;
      vector2_2.X = vector2_3.X + num8;
      vector2_2.Y -= num9;
      data.Face = 1;
      this.SetFacePos(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 3]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 3]];
      vector2_3.Y += num9;
      vector2_3.X = vector2_2.X - num8;
      vector2_2.Y -= num9;
      data.Face = 3;
      this.SetFacePos(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
      vector2_3.X += num8;
      vector2_2.X -= num8;
      vector2_2.Y = vector2_3.Y + num9;
      data.Face = 4;
      this.SetUpFacePos(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
      vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 5]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 5]];
      vector2_3.X += num8;
      vector2_2.X -= num8;
      vector2_2.Y = vector2_3.Y + num9;
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_3.Y);
      this.AddVertex(ref data);
    }

    private void BuildSignBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float y = this.tilesize * 0.5f;
      float num1 = this.tilesize * 0.065f;
      float num2 = num1 + num1;
      float num3 = this.tilesize * 0.25f;
      float num4 = this.tilesize * 0.45f;
      Vector3 position1 = new Vector3(-num1, (float) (-(double) num3 * 0.5), -num4);
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num5 = (byte) ((uint) dataBlockAux >> 4);
      byte num6 = (byte) ((uint) dataBlockAux & 7U);
      if (num6 > (byte) 3)
        position1.X += y - num1;
      Vector3 position2 = position1;
      position2.Y += num3 * 2f;
      Vector3 position3 = position2;
      position3.Z += num4 * 2f;
      Vector3 position4 = position3;
      position4.Y -= num3 * 2f;
      Vector3 position5 = position1;
      position5.X += num2;
      Vector3 position6 = position2;
      position6.X += num2;
      Vector3 position7 = position3;
      position7.X += num2;
      Vector3 position8 = position4;
      position8.X += num2;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) num6 & 3];
      Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, rotatedBlockMatrix) + blockCenter;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num5);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num6;
      data.IsCorner = false;
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = true;
      bool flag4 = true;
      data.UseOwnLight = true;
      if (flag1)
      {
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (num6 > (byte) 3)
        data.UseOwnLight = false;
      if (flag3)
      {
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position2.X, position6.X), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position1.X, position5.X), vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (flag2)
      {
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (flag4)
      {
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position3.X, position7.X), vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position4.X, position8.X), vector2_2.Y);
        this.AddVertex(ref data);
      }
      data.Face = 4;
      this.SetUpFacePos(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_1.X, this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position2.X, position6.X));
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position3.X, position7.X));
      this.AddVertex(ref data);
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position1.X, position5.X));
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position4.X, position8.X));
      this.AddVertex(ref data);
      if (num6 > (byte) 3)
        return;
      Vector3 vector3_9 = new Vector3();
      Vector3 vector3_10 = new Vector3();
      vector3_9.X = blockCenter.X - y;
      vector3_9.Y = blockCenter.Y - y;
      vector3_9.Z = blockCenter.Z - y;
      vector3_10.X = blockCenter.X + y;
      vector3_10.Y = blockCenter.Y + y;
      vector3_10.Z = blockCenter.Z + y;
      float num7 = this.tilesize * 0.045f;
      float num8 = num7 + num7;
      Vector3 vector3_11 = blockCenter - new Vector3(num7, y, num7);
      vector3_2 = vector3_11;
      vector3_2.Y += this.tilesize;
      vector3_3 = vector3_2;
      vector3_3.Z += num8;
      Vector3 vector3_12 = vector3_3;
      vector3_12.Y -= this.tilesize;
      Vector3 vector3_13 = vector3_11;
      vector3_13.X += num8;
      vector3_6 = vector3_2;
      vector3_6.X += num8;
      vector3_7 = vector3_3;
      vector3_7.X += num8;
      vector3_8 = vector3_12;
      vector3_8.X += num8;
      byte num9 = 5;
      vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num9, 6]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num9, 6]];
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_11.Z, vector3_9.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_2.Z, vector3_9.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, vector3_9.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_12.Z, vector3_9.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 1;
      this.SetFacePos(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_13.X, vector3_10.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_6.X, vector3_10.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_2.X, vector3_10.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_11.X, vector3_10.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_8.Z, vector3_10.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_7.Z, vector3_10.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_6.Z, vector3_10.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_13.Z, vector3_10.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 3;
      this.SetFacePos(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_12.X, vector3_9.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, vector3_9.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_7.X, vector3_9.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_8.X, vector3_9.X), vector2_2.Y);
      this.AddVertex(ref data);
      bool flag5 = this.IsClear(p, blockID, 0, 4);
      bool flag6 = this.IsClear(p, blockID, 0, 5);
      if (flag5 || flag6)
      {
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) num9, 4]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) num9, 4]];
      }
      if (flag5)
      {
        data.Face = 4;
        this.SetUpFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (!flag6)
        return;
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
    }

    private void BuildFenceBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.08f;
      float num3 = num2 + num2;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Vector3 vector3_1 = new Vector3(blockCenter.X - num1, blockCenter.Y - num1, blockCenter.Z - num1);
      Vector3 vector3_2 = blockCenter - new Vector3(num2, num1, num2);
      Vector3 vector3_3 = vector3_2;
      vector3_3.Y += this.tilesize;
      Vector3 vector3_4 = vector3_3;
      vector3_4.X += num3;
      Vector3 vector3_5 = vector3_4;
      vector3_5.Y -= this.tilesize;
      Vector3 vector3_6 = vector3_2;
      vector3_6.Z += num3;
      Vector3 vector3_7 = vector3_3;
      vector3_7.Z += num3;
      Vector3 vector3_8 = vector3_4;
      vector3_8.Z += num3;
      Vector3 vector3_9 = vector3_5;
      vector3_9.Z += num3;
      bool flag1 = this.IsClear(p, blockID, 0, 4);
      bool flag2 = this.IsClear(p, blockID, 0, 5);
      byte num4 = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num4);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = (byte) 0;
      data.IsCorner = false;
      data.Face = 1;
      this.SetFacePos(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_5.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_4.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_3.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_2.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_2.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_3.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_7.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_6.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 3;
      this.SetFacePos(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_6.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_7.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_8.X), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_9.X), vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_9.Z), vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_8.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_4.Z), vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_5.Z), vector2_2.Y);
      this.AddVertex(ref data);
      if (flag1)
      {
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_7.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_7.Z));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_3.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_3.Z));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_4.Z));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_8.Z));
        this.AddVertex(ref data);
      }
      if (flag2)
      {
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_2.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_2.Z));
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_6.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_6.Z));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_9.Z));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_5.Z));
        this.AddVertex(ref data);
      }
      --p.X;
      byte blockId1 = this.GetDataBlock(ref p).BlockID;
      p.X += 2;
      byte num5 = this.GetDataBlock(ref p).BlockID;
      --p.X;
      --p.Z;
      byte blockId2 = this.GetDataBlock(ref p).BlockID;
      p.Z += 2;
      byte num6 = this.GetDataBlock(ref p).BlockID;
      --p.Z;
      Vector3 vector3_10;
      Vector3 vector3_11;
      Vector3 vector3_12;
      Vector3 vector3_13;
      if (blockId1 == (byte) 139 || blockId1 == (byte) 75 || blockId1 == (byte) 76 || blockId1 > (byte) 0 && this.map.BlockData[(int) blockId1].Buffer < (byte) 2)
      {
        float z = this.tilesize * 0.07f;
        float num7 = z + z;
        vector3_10 = blockCenter - new Vector3(num1, (float) (-(double) num1 + (double) num7 * 3.0), z);
        vector3_11 = vector3_10;
        vector3_11.Y += num7;
        vector3_4 = vector3_11;
        vector3_4.X += num1;
        vector3_5 = vector3_4;
        vector3_5.Y -= num7;
        vector3_12 = vector3_10;
        vector3_12.Z += num7;
        vector3_13 = vector3_11;
        vector3_13.Z += num7;
        vector3_8 = vector3_4;
        vector3_8.Z += num7;
        vector3_9 = vector3_5;
        vector3_9.Z += num7;
        if (num5 == (byte) 139 || num5 == (byte) 75 || num5 == (byte) 76 || num5 > (byte) 0 && this.map.BlockData[(int) num5].Buffer < (byte) 2)
        {
          vector3_4.X += num1;
          vector3_5.X += num1;
          vector3_8.X += num1;
          vector3_9.X += num1;
          num5 = (byte) 0;
        }
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_5.Y));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_4.Y));
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_11.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_11.Y));
        this.AddVertex(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_10.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_10.Y));
        this.AddVertex(ref data);
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_12.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_12.Y));
        this.AddVertex(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_13.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_13.Y));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_8.Y));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_9.Y));
        this.AddVertex(ref data);
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_13.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_13.Z));
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_11.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_11.Z));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_4.Z));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_8.Z));
        this.AddVertex(ref data);
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_10.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_10.Z));
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_12.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_12.Z));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_9.Z));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_5.Z));
        this.AddVertex(ref data);
      }
      if (num5 == (byte) 139 || num5 == (byte) 75 || num5 == (byte) 76 || num5 > (byte) 0 && this.map.BlockData[(int) num5].Buffer < (byte) 2)
      {
        float z = this.tilesize * 0.07f;
        float num7 = z + z;
        vector3_10 = blockCenter - new Vector3(0.0f, (float) (-(double) num1 + (double) num7 * 3.0), z);
        vector3_11 = vector3_10;
        vector3_11.Y += num7;
        vector3_4 = vector3_11;
        vector3_4.X += num1;
        vector3_5 = vector3_4;
        vector3_5.Y -= num7;
        vector3_12 = vector3_10;
        vector3_12.Z += num7;
        vector3_13 = vector3_11;
        vector3_13.Z += num7;
        vector3_8 = vector3_4;
        vector3_8.Z += num7;
        vector3_9 = vector3_5;
        vector3_9.Z += num7;
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_5.Y));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_4.Y));
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_11.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_11.Y));
        this.AddVertex(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_10.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_10.Y));
        this.AddVertex(ref data);
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_12.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_12.Y));
        this.AddVertex(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_13.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_13.Y));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_8.Y));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_9.Y));
        this.AddVertex(ref data);
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_13.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_13.Z));
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_11.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_11.Z));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_4.Z));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_8.Z));
        this.AddVertex(ref data);
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_10.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_10.Z));
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_12.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_12.Z));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_9.Z));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Z, vector3_5.Z));
        this.AddVertex(ref data);
      }
      if (blockId2 == (byte) 139 || blockId2 == (byte) 75 || blockId2 == (byte) 76 || blockId2 > (byte) 0 && this.map.BlockData[(int) blockId2].Buffer < (byte) 2)
      {
        float x = this.tilesize * 0.07f;
        float num7 = x + x;
        vector3_10 = blockCenter - new Vector3(x, (float) (-(double) num1 + (double) num7 * 3.0), num1);
        vector3_11 = vector3_10;
        vector3_11.Y += num7;
        vector3_4 = vector3_11;
        vector3_4.X += num7;
        vector3_5 = vector3_4;
        vector3_5.Y -= num7;
        vector3_12 = vector3_10;
        vector3_12.Z += num1;
        vector3_13 = vector3_11;
        vector3_13.Z += num1;
        vector3_8 = vector3_4;
        vector3_8.Z += num1;
        vector3_9 = vector3_5;
        vector3_9.Z += num1;
        if (num6 == (byte) 139 || num6 == (byte) 75 || num6 == (byte) 76 || num6 > (byte) 0 && this.map.BlockData[(int) num6].Buffer < (byte) 2)
        {
          vector3_12.Z += num1;
          vector3_13.Z += num1;
          vector3_8.Z += num1;
          vector3_9.Z += num1;
          num6 = (byte) 0;
        }
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_10.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_10.Y));
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_11.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_11.Y));
        this.AddVertex(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_13.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_13.Y));
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_12.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_12.Y));
        this.AddVertex(ref data);
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_9.Y));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_8.Y));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_4.Y));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_5.Y));
        this.AddVertex(ref data);
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_13.X;
        data.Y = vector3_13.Y;
        data.Z = vector3_13.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_13.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_13.X));
        this.AddVertex(ref data);
        data.X = vector3_11.X;
        data.Y = vector3_11.Y;
        data.Z = vector3_11.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_11.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_11.X));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_4.X));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_8.X));
        this.AddVertex(ref data);
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_10.X;
        data.Y = vector3_10.Y;
        data.Z = vector3_10.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_10.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_10.X));
        this.AddVertex(ref data);
        data.X = vector3_12.X;
        data.Y = vector3_12.Y;
        data.Z = vector3_12.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_12.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_12.X));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_9.X));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_5.X));
        this.AddVertex(ref data);
      }
      if (num6 != (byte) 139 && num6 != (byte) 75 && num6 != (byte) 76 && (num6 <= (byte) 0 || this.map.BlockData[(int) num6].Buffer >= (byte) 2))
        return;
      float x1 = this.tilesize * 0.07f;
      float num8 = x1 + x1;
      vector3_10 = blockCenter - new Vector3(x1, (float) (-(double) num1 + (double) num8 * 3.0), 0.0f);
      vector3_11 = vector3_10;
      vector3_11.Y += num8;
      vector3_4 = vector3_11;
      vector3_4.X += num8;
      vector3_5 = vector3_4;
      vector3_5.Y -= num8;
      vector3_12 = vector3_10;
      vector3_12.Z += num1;
      vector3_13 = vector3_11;
      vector3_13.Z += num1;
      vector3_8 = vector3_4;
      vector3_8.Z += num1;
      vector3_9 = vector3_5;
      vector3_9.Z += num1;
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_10.X;
      data.Y = vector3_10.Y;
      data.Z = vector3_10.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_10.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_10.Y));
      this.AddVertex(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_11.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_11.Y));
      this.AddVertex(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_13.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_13.Y));
      this.AddVertex(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_12.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_12.Y));
      this.AddVertex(ref data);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_9.Y));
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_8.Y));
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_4.Y));
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.Y, vector3_5.Y));
      this.AddVertex(ref data);
      data.Face = 4;
      this.SetFacePos(ref data);
      data.X = vector3_13.X;
      data.Y = vector3_13.Y;
      data.Z = vector3_13.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_13.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_13.X));
      this.AddVertex(ref data);
      data.X = vector3_11.X;
      data.Y = vector3_11.Y;
      data.Z = vector3_11.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_11.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_11.X));
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_4.X));
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_8.X));
      this.AddVertex(ref data);
      data.Face = 5;
      this.SetFacePos(ref data);
      data.X = vector3_10.X;
      data.Y = vector3_10.Y;
      data.Z = vector3_10.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_10.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_10.X));
      this.AddVertex(ref data);
      data.X = vector3_12.X;
      data.Y = vector3_12.Y;
      data.Z = vector3_12.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_12.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_12.X));
      this.AddVertex(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_9.X));
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, vector3_5.X));
      this.AddVertex(ref data);
    }

    private void BuildPostBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = num1 * 0.5f;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num3 = (byte) ((uint) dataBlockAux >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num3);
      byte num4 = (byte) ((uint) dataBlockAux & 7U);
      Vector3 position = this.map.GetPosition(p);
      Vector3 vector3_1 = new Vector3();
      vector3_1.X = position.X + this.tilesize;
      vector3_1.Y = position.Y - this.tilesize;
      vector3_1.Z = position.Z + this.tilesize;
      Vector3 vector3_2 = new Vector3();
      Vector3 vector3_3 = new Vector3();
      vector3_2.Y = position.Y - this.tilesize;
      vector3_3.Y = position.Y;
      switch (num4)
      {
        case 0:
          vector3_2.X = position.X + num2;
          vector3_2.Z = position.Z + num2;
          vector3_3.X = position.X + this.tilesize - num2;
          vector3_3.Z = position.Z + this.tilesize - num2;
          break;
        case 1:
          vector3_2.X = position.X;
          vector3_2.Z = position.Z;
          vector3_3.X = position.X + num1;
          vector3_3.Z = position.Z + num1;
          break;
        case 2:
          vector3_2.X = position.X + this.tilesize - num1;
          vector3_2.Z = position.Z;
          vector3_3.X = position.X + this.tilesize;
          vector3_3.Z = position.Z + num1;
          break;
        case 3:
          vector3_2.X = position.X + this.tilesize - num1;
          vector3_2.Z = position.Z + this.tilesize - num1;
          vector3_3.X = position.X + this.tilesize;
          vector3_3.Z = position.Z + this.tilesize;
          break;
        case 4:
          vector3_2.X = position.X;
          vector3_2.Z = position.Z + this.tilesize - num1;
          vector3_3.X = position.X + num1;
          vector3_3.Z = position.Z + this.tilesize;
          break;
      }
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = (byte) 0;
      data.IsCorner = false;
      if (num4 == (byte) 0 || num4 == (byte) 2 || num4 == (byte) 3 || this.IsClearLeft(ref p, blockID, (int) num4, 0))
      {
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num4 == (byte) 0 || num4 > (byte) 2 || this.IsClearForward(ref p, blockID, (int) num4, 1))
      {
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num4 < (byte) 2 || num4 == (byte) 4 || this.IsClearRight(ref p, blockID, (int) num4, 2))
      {
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num4 < (byte) 3 || this.IsClearBackward(ref p, blockID, (int) num4, 3))
      {
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (this.IsClearUp(ref p, blockID, (int) num4, 4))
      {
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
        this.AddVertex(ref data);
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
        this.AddVertex(ref data);
      }
      if (!this.IsClearDown(ref p, blockID, (int) num4, 5))
        return;
      data.Face = 5;
      this.SetFacePos(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
      this.AddVertex(ref data);
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
      this.AddVertex(ref data);
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_1.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_1.X, data.X));
      this.AddVertex(ref data);
    }

    private void BuildSidePostBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      int textureIndex = (int) dataBlockAux >> 4;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, textureIndex);
      byte num = (byte) ((uint) dataBlockAux & 7U);
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector3 position = this.map.GetPosition(p);
      Vector3 vector3_1 = new Vector3();
      Vector3 vector3_2 = new Vector3();
      Vector3 vector3_3 = new Vector3()
      {
        X = position.X + this.tilesize,
        Y = position.Y + this.tilesize,
        Z = position.Z + this.tilesize
      };
      if (num < (byte) 4)
      {
        switch (num)
        {
          case 0:
            vector3_1.X = position.X;
            vector3_1.Y = position.Y - this.tilesize;
            break;
          case 1:
            vector3_1.X = position.X + this.halftilesize;
            vector3_1.Y = position.Y - this.tilesize;
            break;
          case 2:
            vector3_1.X = position.X;
            vector3_1.Y = position.Y - this.halftilesize;
            break;
          case 3:
            vector3_1.X = position.X + this.halftilesize;
            vector3_1.Y = position.Y - this.halftilesize;
            break;
        }
        vector3_1.Z = position.Z;
        vector3_2.X = vector3_1.X + this.halftilesize;
        vector3_2.Y = vector3_1.Y + this.halftilesize;
        vector3_2.Z = vector3_1.Z + this.tilesize;
        AVParams data = new AVParams();
        data.Point = p;
        data.BlockID = blockID;
        data.Aux = (byte) 0;
        data.IsCorner = false;
        if (num == (byte) 1 || num == (byte) 3 || this.IsClearLeft(ref p, blockID, 0, 0))
        {
          data.Face = 0;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (this.IsClearForward(ref p, blockID, 0, 1))
        {
          data.Face = 1;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (num == (byte) 0 || num == (byte) 2 || this.IsClearRight(ref p, blockID, 0, 2))
        {
          data.Face = 2;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (this.IsClearBackward(ref p, blockID, 0, 3))
        {
          data.Face = 3;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (num < (byte) 2 || this.IsClearUp(ref p, blockID, 0, 4))
        {
          data.Face = 4;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
        }
        if (num != (byte) 2 && num != (byte) 3 && !this.IsClearDown(ref p, blockID, 0, 5))
          return;
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
      }
      else
      {
        switch (num)
        {
          case 4:
            vector3_1.Y = position.Y - this.tilesize;
            vector3_1.Z = position.Z + this.halftilesize;
            break;
          case 5:
            vector3_1.Y = position.Y - this.tilesize;
            vector3_1.Z = position.Z;
            break;
          case 6:
            vector3_1.Y = position.Y - this.halftilesize;
            vector3_1.Z = position.Z + this.halftilesize;
            break;
          case 7:
            vector3_1.Y = position.Y - this.halftilesize;
            vector3_1.Z = position.Z;
            break;
        }
        vector3_1.X = position.X;
        vector3_2.X = vector3_1.X + this.tilesize;
        vector3_2.Y = vector3_1.Y + this.halftilesize;
        vector3_2.Z = vector3_1.Z + this.halftilesize;
        AVParams data = new AVParams();
        data.Point = p;
        data.BlockID = blockID;
        data.Aux = (byte) 0;
        data.IsCorner = false;
        if (this.IsClearLeft(ref p, blockID, 0, 0))
        {
          data.Face = 0;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (num == (byte) 4 || num == (byte) 6 || this.IsClearForward(ref p, blockID, 0, 1))
        {
          data.Face = 1;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (this.IsClearRight(ref p, blockID, 0, 2))
        {
          data.Face = 2;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (num == (byte) 5 || num == (byte) 7 || this.IsClearBackward(ref p, blockID, 0, 3))
        {
          data.Face = 3;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
          this.AddVertex(ref data);
        }
        if (num == (byte) 4 || num == (byte) 5 || this.IsClearUp(ref p, blockID, 0, 4))
        {
          data.Face = 4;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
          this.AddVertex(ref data);
        }
        if (num <= (byte) 5 && !this.IsClearDown(ref p, blockID, 0, 5))
          return;
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
      }
    }

    private void BuildCornerBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      int textureIndex = (int) dataBlockAux >> 4;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, textureIndex);
      byte num = (byte) ((uint) dataBlockAux & 7U);
      Vector3 position = this.map.GetPosition(p);
      Vector3 vector3_1 = new Vector3();
      Vector3 vector3_2 = new Vector3();
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      switch (num)
      {
        case 0:
        case 4:
          vector3_1.X = position.X;
          vector3_1.Z = position.Z;
          break;
        case 1:
        case 5:
          vector3_1.X = position.X + this.halftilesize;
          vector3_1.Z = position.Z;
          break;
        case 2:
        case 6:
          vector3_1.X = position.X + this.halftilesize;
          vector3_1.Z = position.Z + this.halftilesize;
          break;
        case 3:
        case 7:
          vector3_1.X = position.X;
          vector3_1.Z = position.Z + this.halftilesize;
          break;
      }
      vector3_1.Y = position.Y - (num > (byte) 3 ? this.halftilesize : this.tilesize);
      vector3_2.X = vector3_1.X + this.halftilesize;
      vector3_2.Y = vector3_1.Y + this.halftilesize;
      vector3_2.Z = vector3_1.Z + this.halftilesize;
      Vector3 vector3_3 = new Vector3()
      {
        X = position.X + this.tilesize,
        Y = position.Y + this.tilesize,
        Z = position.Z + this.tilesize
      };
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = (byte) 0;
      data.IsCorner = false;
      if (num == (byte) 1 || num == (byte) 2 || (num == (byte) 5 || num == (byte) 6) || this.IsClearLeft(ref p, blockID, 0, 0))
      {
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num == (byte) 2 || num == (byte) 3 || (num == (byte) 6 || num == (byte) 7) || this.IsClearForward(ref p, blockID, 0, 1))
      {
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num == (byte) 0 || num == (byte) 3 || (num == (byte) 4 || num == (byte) 7) || this.IsClearRight(ref p, blockID, 0, 2))
      {
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num < (byte) 2 || num == (byte) 4 || num == (byte) 5 || this.IsClearBackward(ref p, blockID, 0, 3))
      {
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.X, data.X), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, position.Y, data.Y));
        this.AddVertex(ref data);
      }
      if (num < (byte) 4 || this.IsClearUp(ref p, blockID, 0, 4))
      {
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, position.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
        this.AddVertex(ref data);
      }
      if (num <= (byte) 3 && !this.IsClearDown(ref p, blockID, 0, 5))
        return;
      data.Face = 5;
      this.SetFacePos(ref data);
      data.X = vector3_1.X;
      data.Y = vector3_1.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
      this.AddVertex(ref data);
      data.Z = vector3_1.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
      this.AddVertex(ref data);
      data.X = vector3_1.X;
      data.TC = new NormalizedShort2(this.CalcTexCoord(vector2_1.X, vector2_2.X, vector3_3.Z, data.Z), this.CalcTexCoord(vector2_1.Y, vector2_2.Y, vector3_3.X, data.X));
      this.AddVertex(ref data);
    }

    private void BuildPaneBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = 0.125f;
      float num2 = num1 * 0.5f;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num3 = (byte) ((uint) dataBlockAux >> 4);
      byte num4 = (byte) ((uint) dataBlockAux & 3U);
      int index = blockID == (byte) 76 ? (int) num3 + 304 : MapChunkContent.TexOffsets[(int) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num3), 6];
      Vector2 vector2_1 = MapChunkContent.TexCoords1[index];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[index];
      Vector2 vector2_3 = vector2_1;
      Vector2 vector2_4 = vector2_2;
      Vector3 position = this.map.GetPosition(p);
      ++p.Y;
      byte blockId1 = this.GetDataBlock(ref p).BlockID;
      p.Y -= 2;
      byte blockId2 = this.GetDataBlock(ref p).BlockID;
      ++p.Y;
      --p.X;
      byte blockId3 = this.GetDataBlock(ref p).BlockID;
      p.X += 2;
      byte blockId4 = this.GetDataBlock(ref p).BlockID;
      --p.X;
      --p.Z;
      byte blockId5 = this.GetDataBlock(ref p).BlockID;
      p.Z += 2;
      byte blockId6 = this.GetDataBlock(ref p).BlockID;
      --p.Z;
      bool flag1 = this.map.BlockData[(int) blockId1].Buffer > (byte) 1;
      bool flag2 = this.map.BlockData[(int) blockId2].Buffer > (byte) 1;
      int buffer1 = (int) this.map.BlockData[(int) blockId3].Buffer;
      int buffer2 = (int) this.map.BlockData[(int) blockId5].Buffer;
      int buffer3 = (int) this.map.BlockData[(int) blockId4].Buffer;
      int buffer4 = (int) this.map.BlockData[(int) blockId6].Buffer;
      bool flag3 = buffer1 > 1 && blockId3 != (byte) 75 && blockId3 != (byte) 76;
      bool flag4 = buffer2 > 1 && blockId5 != (byte) 75 && blockId5 != (byte) 76;
      bool flag5 = buffer3 > 1 && blockId4 != (byte) 75 && blockId4 != (byte) 76;
      bool flag6 = buffer4 > 1 && blockId6 != (byte) 75 && blockId6 != (byte) 76;
      bool flag7 = BlockData.ShouldDrawPaneSection(this.map, (Block) blockId3);
      bool flag8 = BlockData.ShouldDrawPaneSection(this.map, (Block) blockId5);
      bool flag9 = BlockData.ShouldDrawPaneSection(this.map, (Block) blockId4);
      bool flag10 = BlockData.ShouldDrawPaneSection(this.map, (Block) blockId6);
      if (!flag7 && !flag8 && (!flag9 && !flag10))
      {
        int num5;
        flag10 = (num5 = 1) != 0;
        flag9 = num5 != 0;
        flag8 = num5 != 0;
        flag7 = num5 != 0;
      }
      bool flag11 = flag7 && flag9 && !flag10;
      bool flag12 = flag9 && flag7 && !flag8;
      bool flag13 = flag8 && flag10 && !flag7;
      bool flag14 = flag10 && flag8 && !flag9;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num4;
      data.IsCorner = false;
      if (flag7)
      {
        Vector3 vector3_1 = position;
        vector3_1.Z += this.halftilesize - num2;
        Vector3 vector3_2 = vector3_1;
        vector3_2.X += this.halftilesize - num2;
        vector3_2.Y -= this.tilesize;
        vector3_2.Z += num1;
        if (flag11)
          vector3_2.X = position.X + this.tilesize;
        else if (!flag9 && !flag10 && !flag8)
          vector3_2.X += num1;
        vector2_1.X = vector2_3.X;
        vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.X, vector3_2.X);
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        if (!flag11 && !flag9 && (!flag10 && !flag8))
        {
          vector3_2.X -= num1;
          vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.X, vector3_2.X);
        }
        if (flag3)
        {
          data.Face = 0;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
        }
        if (!flag12)
        {
          float x = vector3_2.X;
          if (!flag9 && !flag8)
          {
            vector3_2.X += num1;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.X, vector3_2.X);
          }
          else if (flag11)
          {
            vector3_2.X = position.X + this.halftilesize - num2;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.X, vector3_2.X);
          }
          data.Face = 1;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          if (flag11)
          {
            vector3_2.X = x;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.X, vector3_2.X);
          }
          if (!flag9 && !flag8)
          {
            data.Face = 2;
            this.SetFacePos(ref data);
            data.X = vector3_2.X;
            data.Y = vector3_2.Y;
            data.Z = vector3_2.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
            this.AddVertex(ref data);
            data.Y = vector3_1.Y;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Z = vector3_1.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Y = vector3_2.Y;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
            this.AddVertex(ref data);
          }
        }
        if (flag1 || flag2)
        {
          if (flag9 && flag8 && flag10)
          {
            vector3_2.X += num1;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.X, vector3_2.X);
          }
          if (flag1)
          {
            data.Face = 4;
            this.SetFacePos(ref data);
            data.X = vector3_1.X;
            data.Y = vector3_1.Y;
            data.Z = vector3_1.Z;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_2.X;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Z = vector3_2.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_1.X;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
          }
          if (flag2)
          {
            data.Face = 5;
            this.SetFacePos(ref data);
            data.X = vector3_1.X;
            data.Y = vector3_2.Y;
            data.Z = vector3_2.Z;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_2.X;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Z = vector3_1.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_1.X;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
          }
        }
      }
      if (flag8)
      {
        Vector3 vector3_1 = position;
        vector3_1.X += this.halftilesize - num2;
        Vector3 vector3_2 = vector3_1;
        vector3_2.X += num1;
        vector3_2.Y -= this.tilesize;
        vector3_2.Z += this.halftilesize - num2;
        if (flag13)
          vector3_2.Z = position.Z + this.tilesize;
        else if (!flag7 && !flag10 && !flag9)
          vector3_2.Z += num1;
        vector2_1.X = vector2_3.X;
        vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.Z, vector3_2.Z);
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        if (!flag13 && !flag7 && (!flag10 && !flag9))
        {
          vector3_2.Z -= num1;
          vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.Z, vector3_2.Z);
        }
        if (flag4)
        {
          data.Face = 1;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
        }
        if (!flag14)
        {
          float z = vector3_2.Z;
          if (!flag9 && !flag10)
          {
            vector3_2.Z += num1;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.Z, vector3_2.Z);
          }
          else if (flag13)
          {
            vector3_2.Z = position.Z + this.halftilesize - num2;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.Z, vector3_2.Z);
          }
          data.Face = 2;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          if (flag13)
          {
            vector3_2.Z = z;
            vector2_2.X = this.CalcTexCoord(vector2_3.X, vector2_4.X, vector3_1.Z, vector3_2.Z);
          }
          if (!flag10 && !flag9)
          {
            data.Face = 3;
            this.SetFacePos(ref data);
            data.X = vector3_1.X;
            data.Y = vector3_2.Y;
            data.Z = vector3_2.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
            this.AddVertex(ref data);
            data.Y = vector3_1.Y;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_2.X;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Y = vector3_2.Y;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
            this.AddVertex(ref data);
          }
        }
        if (flag1)
        {
          data.Face = 4;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_1.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
        }
        if (flag2)
        {
          data.Face = 5;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_1.X;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
        }
      }
      if (flag9)
      {
        Vector3 vector3_1 = position;
        vector3_1.X += this.halftilesize + num2;
        vector3_1.Z += this.halftilesize - num2;
        Vector3 vector3_2 = vector3_1;
        vector3_2.X = position.X + this.tilesize;
        vector3_2.Y -= this.tilesize;
        vector3_2.Z += num1;
        if (flag12)
          vector3_1.X = position.X;
        else if (!flag8 && !flag7 && !flag10)
          vector3_1.X -= num1;
        vector2_2.X = vector2_4.X;
        vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_2.X, vector3_1.X);
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = vector3_1.Y;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = vector3_2.Y;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        if (!flag12 && !flag8 && (!flag7 && !flag10))
        {
          vector3_1.X += num1;
          vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_2.X, vector3_1.X);
        }
        if (flag5)
        {
          data.Face = 2;
          this.SetFacePos(ref data);
          data.X = vector3_2.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
        }
        if (!flag11)
        {
          float x = vector3_1.X;
          if (!flag7 && !flag10)
          {
            vector3_1.X = position.X + this.halftilesize - num2;
            vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_2.X, vector3_1.X);
          }
          else if (flag12)
          {
            vector3_1.X = position.X + this.halftilesize + num2;
            vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_2.X, vector3_1.X);
          }
          data.Face = 3;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.X = vector3_2.X;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
          this.AddVertex(ref data);
          if (flag12)
          {
            vector3_1.X = x;
            vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_2.X, vector3_1.X);
          }
        }
        if (!flag7 && !flag10)
        {
          data.Face = 0;
          this.SetFacePos(ref data);
          data.X = vector3_1.X;
          data.Y = vector3_2.Y;
          data.Z = vector3_1.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
          data.Y = vector3_1.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Z = vector3_2.Z;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
          this.AddVertex(ref data);
          data.Y = vector3_2.Y;
          data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
          this.AddVertex(ref data);
        }
        if (!flag11)
        {
          if (flag1)
          {
            data.Face = 4;
            this.SetFacePos(ref data);
            data.X = vector3_2.X;
            data.Y = vector3_1.Y;
            data.Z = vector3_2.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_1.X;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Z = vector3_1.Z;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_2.X;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
          }
          if (flag2)
          {
            data.Face = 5;
            this.SetFacePos(ref data);
            data.X = vector3_2.X;
            data.Y = vector3_2.Y;
            data.Z = vector3_1.Z;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_1.X;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.Z = vector3_2.Z;
            data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
            this.AddVertex(ref data);
            data.X = vector3_2.X;
            data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
            this.AddVertex(ref data);
          }
        }
      }
      if (!flag10)
        return;
      Vector3 vector3_3 = position;
      vector3_3.X += this.halftilesize - num2;
      vector3_3.Z += this.halftilesize + num2;
      Vector3 vector3_4 = vector3_3;
      vector3_4.X += num1;
      vector3_4.Y -= this.tilesize;
      vector3_4.Z = position.Z + this.tilesize;
      if (flag14)
        vector3_3.Z = position.Z;
      else if (!flag9 && !flag8 && !flag7)
        vector3_3.Z -= num1;
      vector2_2.X = vector2_4.X;
      vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_4.Z, vector3_3.Z);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Y = vector3_3.Y;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Y = vector3_4.Y;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      if (!flag14 && !flag9 && (!flag8 && !flag7))
      {
        vector3_3.Z += num1;
        vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_4.Z, vector3_3.Z);
      }
      if (flag6)
      {
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = vector3_4.Y;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (!flag13)
      {
        float z = vector3_3.Z;
        if (!flag7 && !flag8)
        {
          vector3_3.Z -= num1;
          vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_4.Z, vector3_3.Z);
        }
        else if (flag14)
        {
          vector3_3.Z = position.Z + this.halftilesize + num2;
          vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_4.Z, vector3_3.Z);
        }
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = vector3_4.Y;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        if (flag14)
        {
          vector3_3.Z = z;
          vector2_1.X = this.CalcTexCoord(vector2_4.X, vector2_3.X, vector3_4.Z, vector3_3.Z);
        }
      }
      if (!flag8 && !flag7)
      {
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Y = vector3_3.Y;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Y = vector3_4.Y;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (flag13)
        return;
      if (flag1)
      {
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
      }
      if (!flag2)
        return;
      data.Face = 5;
      this.SetFacePos(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
    }

    private void BuildTableBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte num1 = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
      bool flag1 = this.IsClearUp(ref p, blockID, 0, 4);
      bool flag2 = this.IsClearLeft(ref p, blockID, 0, 0);
      bool flag3 = this.IsClearForward(ref p, blockID, 0, 1);
      bool flag4 = this.IsClearRight(ref p, blockID, 0, 2);
      bool flag5 = this.IsClearBackward(ref p, blockID, 0, 3);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num1);
      Vector2 tc1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 tc2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      AVParams data = new AVParams();
      data.BlockID = blockID;
      data.Point = p;
      data.IsCorner = true;
      if (flag2)
      {
        data.Face = 0;
        this.SetLeftFacePos(ref data);
        this.AddTableTopSideFace(ref data, tc1, tc2);
      }
      if (flag3)
      {
        data.Face = 1;
        this.SetForwardFacePos(ref data);
        this.AddTableTopSideFace(ref data, tc1, tc2);
      }
      if (flag4)
      {
        data.Face = 2;
        this.SetRightFacePos(ref data);
        this.AddTableTopSideFace(ref data, tc1, tc2);
      }
      if (flag5)
      {
        data.Face = 3;
        this.SetBackwardFacePos(ref data);
        this.AddTableTopSideFace(ref data, tc1, tc2);
      }
      if (flag1)
      {
        data.Face = 4;
        this.SetUpFacePos(ref data);
        data.X = data.Pos1.X;
        data.Y = data.Pos1.Y;
        data.Z = data.Pos1.Z;
        data.TC = new NormalizedShort2(tc1.X, tc2.Y);
        this.AddVertex(ref data);
        data.X = data.Pos2.X;
        data.TC = new NormalizedShort2(tc1.X, tc1.Y);
        this.AddVertex(ref data);
        data.Z = data.Pos2.Z;
        data.TC = new NormalizedShort2(tc2.X, tc1.Y);
        this.AddVertex(ref data);
        data.X = data.Pos1.X;
        data.TC = new NormalizedShort2(tc2.X, tc2.Y);
        this.AddVertex(ref data);
      }
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = data.Pos1.X;
      data.Y = (float) ((double) data.Pos1.Y + (double) this.tilesize - (double) this.tilesize * 0.150000005960464);
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(tc1.X, tc2.Y);
      this.AddVertex(ref data);
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.TC = new NormalizedShort2(tc2.X, tc1.Y);
      this.AddVertex(ref data);
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      this.AddVertex(ref data);
      float num2 = this.tilesize * 0.5f;
      float num3 = this.tilesize * 0.08f;
      float num4 = num3 + num3;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Vector3 vector3_1 = new Vector3(blockCenter.X - num2, blockCenter.Y - num2, blockCenter.Z - num2);
      Vector3 vector3_2 = blockCenter - new Vector3(num3, num2, num3);
      Vector3 vector3_3 = vector3_2;
      vector3_3.Y += this.tilesize - this.tilesize * 0.15f;
      Vector3 vector3_4 = vector3_3;
      vector3_4.X += num4;
      Vector3 vector3_5 = vector3_4;
      vector3_5.Y -= this.tilesize - this.tilesize * 0.15f;
      Vector3 vector3_6 = vector3_2;
      vector3_6.Z += num4;
      Vector3 vector3_7 = vector3_3;
      vector3_7.Z += num4;
      Vector3 vector3_8 = vector3_4;
      vector3_8.Z += num4;
      Vector3 vector3_9 = vector3_5;
      vector3_9.Z += num4;
      bool flag6 = this.IsClear(p, blockID, 0, 5);
      data.IsCorner = false;
      data.Face = 1;
      this.SetFacePos(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_5.X), tc2.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_4.X), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_3.X), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_2.X), tc2.Y);
      this.AddVertex(ref data);
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_2.Z), tc2.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_3.Z), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_7.Z), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_6.Z), tc2.Y);
      this.AddVertex(ref data);
      data.Face = 3;
      this.SetFacePos(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_6.X), tc2.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_7.X), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_8.X), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_9.X), tc2.Y);
      this.AddVertex(ref data);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_9.Z), tc2.Y);
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_8.Z), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_4.Z), tc1.Y);
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_5.Z), tc2.Y);
      this.AddVertex(ref data);
      if (flag6)
      {
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_2.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_2.Z));
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_6.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_6.Z));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_9.Z));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_5.Z));
        this.AddVertex(ref data);
      }
      GlobalPoint3D p1 = p + Point3D.Left;
      if (this.GetDataBlock(ref p1).BlockID == (byte) 158)
      {
        float z = this.tilesize * 0.07f;
        float num5 = z + z;
        vector3_2 = blockCenter - new Vector3(num2, num2 - num5 * 2f, z);
        vector3_3 = vector3_2;
        vector3_3.Y += num5;
        vector3_4 = vector3_3;
        vector3_4.X += num2;
        vector3_5 = vector3_4;
        vector3_5.Y -= num5;
        vector3_6 = vector3_2;
        vector3_6.Z += num5;
        vector3_7 = vector3_3;
        vector3_7.Z += num5;
        vector3_8 = vector3_4;
        vector3_8.Z += num5;
        vector3_9 = vector3_5;
        vector3_9.Z += num5;
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_5.Y));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_4.Y));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_3.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_3.Y));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_2.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_2.Y));
        this.AddVertex(ref data);
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_6.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_6.Y));
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_7.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_7.Y));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_8.Y));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_9.Y));
        this.AddVertex(ref data);
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_7.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_7.Z));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_3.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_3.Z));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_4.Z));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_8.Z));
        this.AddVertex(ref data);
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_2.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_2.Z));
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_6.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_6.Z));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_9.Z));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_5.Z));
        this.AddVertex(ref data);
      }
      GlobalPoint3D p2 = p + Point3D.Right;
      if (this.GetDataBlock(ref p2).BlockID == (byte) 158)
      {
        float z = this.tilesize * 0.07f;
        float num5 = z + z;
        vector3_2 = blockCenter - new Vector3(0.0f, num2 - num5 * 2f, z);
        vector3_3 = vector3_2;
        vector3_3.Y += num5;
        vector3_4 = vector3_3;
        vector3_4.X += num2;
        vector3_5 = vector3_4;
        vector3_5.Y -= num5;
        vector3_6 = vector3_2;
        vector3_6.Z += num5;
        vector3_7 = vector3_3;
        vector3_7.Z += num5;
        vector3_8 = vector3_4;
        vector3_8.Z += num5;
        vector3_9 = vector3_5;
        vector3_9.Z += num5;
        data.Face = 1;
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_5.Y));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_4.Y));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_3.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_3.Y));
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_2.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_2.Y));
        this.AddVertex(ref data);
        data.Face = 3;
        this.SetFacePos(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_6.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_6.Y));
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_7.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_7.Y));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_8.Y));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_9.Y));
        this.AddVertex(ref data);
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_7.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_7.Z));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_3.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_3.Z));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_4.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_4.Z));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_8.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_8.Z));
        this.AddVertex(ref data);
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_2.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_2.Z));
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_6.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_6.Z));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_9.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_9.Z));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.X, vector3_5.X), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Z, vector3_5.Z));
        this.AddVertex(ref data);
      }
      GlobalPoint3D p3 = p + Point3D.Forward;
      if (this.GetDataBlock(ref p3).BlockID == (byte) 158)
      {
        float x = this.tilesize * 0.07f;
        float num5 = x + x;
        vector3_2 = blockCenter - new Vector3(x, num2 - num5 * 2f, num2);
        vector3_3 = vector3_2;
        vector3_3.Y += num5;
        vector3_4 = vector3_3;
        vector3_4.X += num5;
        vector3_5 = vector3_4;
        vector3_5.Y -= num5;
        vector3_6 = vector3_2;
        vector3_6.Z += num2;
        vector3_7 = vector3_3;
        vector3_7.Z += num2;
        vector3_8 = vector3_4;
        vector3_8.Z += num2;
        vector3_9 = vector3_5;
        vector3_9.Z += num2;
        data.Face = 0;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_2.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_2.Y));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_3.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_3.Y));
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_7.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_7.Y));
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_6.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_6.Y));
        this.AddVertex(ref data);
        data.Face = 2;
        this.SetFacePos(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_9.Y));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_8.Y));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_4.Y));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_5.Y));
        this.AddVertex(ref data);
        data.Face = 4;
        this.SetFacePos(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_7.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_7.X));
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_3.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_3.X));
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_4.X));
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_8.X));
        this.AddVertex(ref data);
        data.Face = 5;
        this.SetFacePos(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_2.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_2.X));
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_6.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_6.X));
        this.AddVertex(ref data);
        data.X = vector3_9.X;
        data.Y = vector3_9.Y;
        data.Z = vector3_9.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_9.X));
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_5.X));
        this.AddVertex(ref data);
      }
      GlobalPoint3D p4 = p + Point3D.Backward;
      if (this.GetDataBlock(ref p4).BlockID != (byte) 158)
        return;
      float x1 = this.tilesize * 0.07f;
      float num6 = x1 + x1;
      vector3_2 = blockCenter - new Vector3(x1, num2 - num6 * 2f, 0.0f);
      vector3_3 = vector3_2;
      vector3_3.Y += num6;
      vector3_4 = vector3_3;
      vector3_4.X += num6;
      vector3_5 = vector3_4;
      vector3_5.Y -= num6;
      vector3_6 = vector3_2;
      vector3_6.Z += num2;
      vector3_7 = vector3_3;
      vector3_7.Z += num2;
      vector3_8 = vector3_4;
      vector3_8.Z += num2;
      vector3_9 = vector3_5;
      vector3_9.Z += num2;
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_2.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_2.Y));
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_3.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_3.Y));
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_7.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_7.Y));
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_6.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_6.Y));
      this.AddVertex(ref data);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_9.Y));
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_8.Y));
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_4.Y));
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.Y, vector3_5.Y));
      this.AddVertex(ref data);
      data.Face = 4;
      this.SetFacePos(ref data);
      data.X = vector3_7.X;
      data.Y = vector3_7.Y;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_7.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_7.X));
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Y = vector3_3.Y;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_3.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_3.X));
      this.AddVertex(ref data);
      data.X = vector3_4.X;
      data.Y = vector3_4.Y;
      data.Z = vector3_4.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_4.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_4.X));
      this.AddVertex(ref data);
      data.X = vector3_8.X;
      data.Y = vector3_8.Y;
      data.Z = vector3_8.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_8.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_8.X));
      this.AddVertex(ref data);
      data.Face = 5;
      this.SetFacePos(ref data);
      data.X = vector3_2.X;
      data.Y = vector3_2.Y;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_2.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_2.X));
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Y = vector3_6.Y;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_6.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_6.X));
      this.AddVertex(ref data);
      data.X = vector3_9.X;
      data.Y = vector3_9.Y;
      data.Z = vector3_9.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_9.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_9.X));
      this.AddVertex(ref data);
      data.X = vector3_5.X;
      data.Y = vector3_5.Y;
      data.Z = vector3_5.Z;
      data.TC = new NormalizedShort2(this.CalcTexCoord(tc1.X, tc2.X, vector3_1.Z, vector3_5.Z), this.CalcTexCoord(tc1.Y, tc2.Y, vector3_1.X, vector3_5.X));
      this.AddVertex(ref data);
    }

    private void AddTableTopSideFace(ref AVParams data, Vector2 tc1, Vector2 tc2)
    {
      data.X = data.Pos1.X;
      data.Y = (float) ((double) data.Pos1.Y + (double) this.tilesize - (double) this.tilesize * 0.150000005960464);
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(tc1.X, tc2.Y);
      data.IsCorner = false;
      this.AddVertex(ref data);
      data.Y = data.Pos2.Y;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      data.IsCorner = true;
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc1.Y);
      this.AddVertex(ref data);
      data.Y = (float) ((double) data.Pos1.Y + (double) this.tilesize - (double) this.tilesize * 0.150000005960464);
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      data.IsCorner = false;
      this.AddVertex(ref data);
    }

    private void BuildLadder_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = 0.06f;
      Vector3 position = this.map.GetPosition(p);
      Vector3 zero = Vector3.Zero;
      zero.X = position.X + this.tilesize;
      zero.Y = position.Y;
      position.Y -= this.tilesize;
      zero.Z = position.Z + this.tilesize;
      byte num2 = (byte) ((uint) this.GetDataBlockAux(ref p) & 7U);
      switch (num2)
      {
        case 0:
          zero.X -= num1;
          position.X = zero.X;
          this.AddLeftFaceNewFormat(position, zero, 0, blockID, (int) blockID, ref p, true);
          if (!this.IsClearRight(ref p, blockID, (int) num2, 2))
            break;
          float z1 = position.Z;
          position.Z = zero.Z;
          zero.Z = z1;
          this.AddRightFaceNewFormat(position, zero, 2, blockID, (int) blockID, ref p, true);
          break;
        case 1:
          zero.Z -= num1;
          position.Z = zero.Z;
          position.X = zero.X;
          zero.X = position.X - this.tilesize;
          this.AddForwardFaceNewFormat(position, zero, 1, blockID, (int) blockID, ref p, true);
          if (!this.IsClearBackward(ref p, blockID, (int) num2, 3))
            break;
          float x1 = position.X;
          position.X = zero.X;
          zero.X = x1;
          this.AddForwardFaceNewFormat(position, zero, 3, blockID, (int) blockID, ref p, true);
          break;
        case 2:
          position.X += num1;
          zero.X = position.X;
          position.Z = zero.Z;
          zero.Z = position.Z - this.tilesize;
          this.AddRightFaceNewFormat(position, zero, 2, blockID, (int) blockID, ref p, true);
          if (!this.IsClearLeft(ref p, blockID, (int) num2, 2))
            break;
          float z2 = position.Z;
          position.Z = zero.Z;
          zero.Z = z2;
          this.AddLeftFaceNewFormat(position, zero, 0, blockID, (int) blockID, ref p, true);
          break;
        case 3:
          position.Z += num1;
          zero.Z = position.Z;
          this.AddBackwardFaceNewFormat(position, zero, 3, blockID, (int) blockID, ref p, true);
          if (!this.IsClearForward(ref p, blockID, (int) num2, 1))
            break;
          float x2 = position.X;
          position.X = zero.X;
          zero.X = x2;
          this.AddBackwardFaceNewFormat(position, zero, 1, blockID, (int) blockID, ref p, true);
          break;
      }
    }

    private void BuildPressurePlate_NewFormat(GlobalPoint3D p, byte blockID)
    {
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      float plateHeight = this.strategy.IsBlockDeliveringPower(p) ? GraphicStatics.PlateHeight * 0.5f : GraphicStatics.PlateHeight;
      AVParams data = new AVParams();
      data.BlockID = blockID;
      data.Aux = (byte) ((uint) dataBlockAux & 7U);
      data.Point = p;
      data.IsCorner = false;
      byte num = (byte) ((uint) dataBlockAux >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      if (this.IsClearLeft(ref p, blockID, 0, 0))
      {
        data.Face = 0;
        this.SetLeftFacePos(ref data);
        this.AddPressurePlateSideFace(ref data, plateHeight, textureIdForDrawing);
      }
      if (this.IsClearForward(ref p, blockID, 0, 1))
      {
        data.Face = 1;
        this.SetForwardFacePos(ref data);
        this.AddPressurePlateSideFace(ref data, plateHeight, textureIdForDrawing);
      }
      if (this.IsClearRight(ref p, blockID, 0, 2))
      {
        data.Face = 2;
        this.SetRightFacePos(ref data);
        this.AddPressurePlateSideFace(ref data, plateHeight, textureIdForDrawing);
      }
      if (this.IsClearBackward(ref p, blockID, 0, 3))
      {
        data.Face = 3;
        this.SetBackwardFacePos(ref data);
        this.AddPressurePlateSideFace(ref data, plateHeight, textureIdForDrawing);
      }
      data.IsCorner = true;
      data.Face = 4;
      this.SetUpFacePos(ref data);
      data.Pos1.Y = (float) ((double) data.Pos1.Y - (double) this.tilesize + (double) plateHeight * (double) this.tilesize);
      data.Pos2.Y = data.Pos1.Y;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 4]];
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y;
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = data.Pos1.X;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, textureIdForDrawing);
    }

    private void AddPressurePlateSideFace(
      ref AVParams data,
      float plateHeight,
      byte blockIDTexture)
    {
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockIDTexture, data.Face]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockIDTexture, data.Face]];
      float y1 = vector2_1.Y;
      float y2 = vector2_2.Y;
      float y3 = (float) (((double) y2 - (double) y1) * (1.0 - (double) plateHeight)) + y1;
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y;
      data.Z = data.Pos1.Z;
      data.TC = new NormalizedShort2(vector2_1.X, y2);
      this.AddVertex(ref data);
      data.Y = data.Pos1.Y + plateHeight * this.tilesize;
      data.TC = new NormalizedShort2(vector2_1.X, y3);
      this.AddVertex(ref data);
      data.X = data.Pos2.X;
      data.Z = data.Pos2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, y3);
      this.AddVertex(ref data);
      data.Y = data.Pos1.Y;
      data.TC = new NormalizedShort2(vector2_2.X, y2);
      this.AddVertex(ref data);
    }

    private void BuildSnowLayer_NewFormat(GlobalPoint3D p, byte blockID)
    {
      AVParams data = new AVParams();
      data.BlockID = blockID;
      data.Point = p;
      int aux = (int) this.GetDataBlockAux(ref p) & 7;
      data.IsCorner = aux == 7;
      float scale = (float) (aux + 1) * 0.125f;
      if (this.IsClearLeft(ref p, blockID, aux, 0))
      {
        data.Face = 0;
        this.SetLeftFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, blockID, 0);
      }
      if (this.IsClearForward(ref p, blockID, aux, 1))
      {
        data.Face = 1;
        this.SetForwardFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, blockID, 1);
      }
      if (this.IsClearRight(ref p, blockID, aux, 2))
      {
        data.Face = 2;
        this.SetRightFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, blockID, 2);
      }
      if (this.IsClearBackward(ref p, blockID, aux, 3))
      {
        data.Face = 3;
        this.SetBackwardFacePos(ref data);
        this.AddVariableHeightSideFace(ref data, scale, blockID, 3);
      }
      if (aux < 7 || this.IsClearUp(ref p, blockID, aux, 4))
      {
        data.IsCorner = true;
        data.Face = 4;
        this.SetUpFacePos(ref data);
        Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 4]];
        Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 4]];
        data.X = data.Pos1.X;
        data.Y = (float) ((double) data.Pos1.Y - (double) this.tilesize + (double) this.tilesize * (double) scale);
        data.Z = data.Pos1.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = data.Pos2.X;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.Z = data.Pos2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = data.Pos1.X;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockID);
    }

    private void BuildBed_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float x = this.tilesize * 0.5f;
      Vector3 position1 = new Vector3(x, -x, -x);
      Vector3 position2 = position1;
      position2.Y += this.tilesize;
      Vector3 position3 = position2;
      position3.X -= this.tilesize;
      Vector3 position4 = position3;
      position4.Y -= this.tilesize;
      Vector3 position5 = position1;
      position5.Z += this.tilesize;
      Vector3 position6 = position2;
      position6.Z += this.tilesize;
      Vector3 position7 = position3;
      position7.Z += this.tilesize;
      Vector3 position8 = position4;
      position8.Z += this.tilesize;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      byte num1 = (byte) ((uint) this.GetDataBlockAux(ref p) & 3U);
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[(int) num1];
      Vector3 vector3_1 = Vector3.Transform(position1, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_2 = Vector3.Transform(position2, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_3 = Vector3.Transform(position3, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_4 = Vector3.Transform(position4, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_5 = Vector3.Transform(position5, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_6 = Vector3.Transform(position6, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_7 = Vector3.Transform(position7, rotatedBlockMatrix) + blockCenter;
      Vector3 vector3_8 = Vector3.Transform(position8, rotatedBlockMatrix) + blockCenter;
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num1;
      data.IsCorner = false;
      Vector2 vector2_1;
      Vector2 vector2_2;
      if (blockID != (byte) 136 && this.IsClear(p, blockID, (int) num1, (int) num1 % 4))
      {
        data.Face = 0;
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        this.SetFacePos(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Face = 2;
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) num1, (1 + (int) num1) % 4))
      {
        data.Face = 1;
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        this.SetFacePos(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_4.X;
        data.Y = vector3_4.Y;
        data.Z = vector3_4.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Face = 3;
        this.AddVertex(ref data);
        data.X = vector3_3.X;
        data.Y = vector3_3.Y;
        data.Z = vector3_3.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (blockID != (byte) 135 && this.IsClear(p, blockID, (int) num1, (2 + (int) num1) % 4))
      {
        data.Face = 2;
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        this.SetFacePos(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_1.X;
        data.Y = vector3_1.Y;
        data.Z = vector3_1.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Face = 0;
        this.AddVertex(ref data);
        data.X = vector3_2.X;
        data.Y = vector3_2.Y;
        data.Z = vector3_2.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      if (this.IsClear(p, blockID, (int) num1, (3 + (int) num1) % 4))
      {
        data.Face = 3;
        vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
        this.SetFacePos(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_5.X;
        data.Y = vector3_5.Y;
        data.Z = vector3_5.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
        this.AddVertex(ref data);
        data.Face = 1;
        this.AddVertex(ref data);
        data.X = vector3_6.X;
        data.Y = vector3_6.Y;
        data.Z = vector3_6.Z;
        data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_7.X;
        data.Y = vector3_7.Y;
        data.Z = vector3_7.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
        this.AddVertex(ref data);
        data.X = vector3_8.X;
        data.Y = vector3_8.Y;
        data.Z = vector3_8.Z;
        data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
        this.AddVertex(ref data);
      }
      data.IsCorner = false;
      data.Face = 4;
      vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
      vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, data.Face]];
      this.SetUpFacePos(ref data);
      float num2 = (float) (((double) vector3_7.Y - (double) vector3_8.Y) * 0.561999976634979) + vector3_8.Y;
      data.X = vector3_7.X;
      data.Y = num2;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_6.X;
      data.Z = vector3_6.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 5;
      this.AddVertex(ref data);
      data.X = vector3_2.X;
      data.Z = vector3_2.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_3.X;
      data.Z = vector3_3.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.X = vector3_7.X;
      data.Z = vector3_7.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
    }

    private void BuildTrapDoorBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.1f;
      byte dataBlockAux = this.GetDataBlockAux(ref p);
      byte num3 = (byte) ((uint) dataBlockAux >> 4);
      byte num4 = (byte) ((uint) dataBlockAux & 7U);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num3);
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Vector3 vector3 = blockCenter;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) textureIdForDrawing, 6]];
      Vector2 vector2_3 = vector2_1;
      Vector2 vector2_4 = vector2_2;
      Vector2 vector2_5 = vector2_1;
      Vector2 vector2_6 = vector2_2;
      float num5 = (float) (((double) vector2_2.X - (double) vector2_1.X) * 0.100000001490116);
      vector2_5.X = vector2_6.X - num5;
      blockCenter.X -= num1;
      vector3.X += num1;
      vector3.Y += num1;
      blockCenter.Y = vector3.Y - num2;
      blockCenter.Z -= num1;
      vector3.Z += num1;
      switch ((int) num4 - 4)
      {
        case 0:
          blockCenter.Z = vector3.Z - num2;
          blockCenter.Y = vector3.Y - this.tilesize;
          break;
        case 1:
          vector3.X = blockCenter.X + num2;
          blockCenter.Y = vector3.Y - this.tilesize;
          break;
        case 2:
          vector3.Z = blockCenter.Z + num2;
          blockCenter.Y = vector3.Y - this.tilesize;
          break;
        case 3:
          blockCenter.X = vector3.X - num2;
          blockCenter.Y = vector3.Y - this.tilesize;
          break;
      }
      switch (num4)
      {
        case 0:
        case 6:
          vector2_5 = vector2_1;
          vector2_6 = vector2_2;
          break;
        case 1:
        case 7:
          vector2_5 = vector2_1;
          vector2_6 = vector2_2;
          break;
        case 2:
        case 4:
          vector2_5 = vector2_1;
          vector2_6 = vector2_2;
          break;
        case 3:
        case 5:
          vector2_5 = vector2_1;
          vector2_6 = vector2_2;
          break;
      }
      this.IsClear(p, blockID, 0, 4);
      this.IsClear(p, blockID, 0, 5);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num4;
      data.IsCorner = false;
      data.UseOwnLight = true;
      data.Face = 0;
      this.SetFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = blockCenter.Y;
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 1;
      this.SetFacePos(ref data);
      data.X = vector3.X;
      data.Y = blockCenter.Y;
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = blockCenter.X;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Face = 2;
      this.SetFacePos(ref data);
      data.X = vector3.X;
      data.Y = blockCenter.Y;
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = 3;
      this.SetFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = blockCenter.Y;
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3.X;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Face = 4;
      this.SetUpFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = vector3.Y;
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_5.X, vector2_6.Y);
      this.AddVertex(ref data);
      data.X = vector3.X;
      data.TC = new NormalizedShort2(vector2_5.X, vector2_5.Y);
      this.AddVertex(ref data);
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_6.X, vector2_5.Y);
      this.AddVertex(ref data);
      data.X = blockCenter.X;
      data.TC = new NormalizedShort2(vector2_6.X, vector2_6.Y);
      this.AddVertex(ref data);
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = blockCenter.Y;
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_6.X, vector2_5.Y);
      this.AddVertex(ref data);
      data.X = vector3.X;
      data.TC = new NormalizedShort2(vector2_6.X, vector2_6.Y);
      this.AddVertex(ref data);
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_5.X, vector2_6.Y);
      this.AddVertex(ref data);
      data.X = blockCenter.X;
      data.TC = new NormalizedShort2(vector2_5.X, vector2_5.Y);
      this.AddVertex(ref data);
    }

    private void BuildDoorBlock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.1f;
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Vector3 vector3 = blockCenter;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[(int) blockID, 0]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[(int) blockID, 0]];
      Vector2 vector2_3 = vector2_1;
      Vector2 vector2_4 = vector2_2;
      Vector2 vector2_5 = vector2_1;
      Vector2 vector2_6 = vector2_2;
      float num3 = (float) (((double) vector2_2.X - (double) vector2_1.X) * 0.100000001490116);
      vector2_5.X = vector2_6.X - num3;
      byte num4 = (byte) ((uint) this.GetDataBlockAux(ref p) & 7U);
      int num5 = 0;
      int num6 = 2;
      int num7 = 1;
      int num8 = 3;
      switch (num4)
      {
        case 0:
        case 4:
          blockCenter.X -= num1;
          vector3.X = blockCenter.X + num2;
          blockCenter.Y -= num1;
          vector3.Y += num1;
          blockCenter.Z -= num1;
          vector3.Z += num1;
          num5 = 0;
          num6 = 2;
          num7 = 1;
          num8 = 3;
          break;
        case 1:
        case 5:
          blockCenter.X -= num1;
          vector3.X += num1;
          blockCenter.Y -= num1;
          vector3.Y += num1;
          blockCenter.Z -= num1;
          vector3.Z = blockCenter.Z + num2;
          num5 = 3;
          num6 = 1;
          num7 = 0;
          num8 = 2;
          break;
        case 2:
        case 6:
          blockCenter.X += num1 - num2;
          vector3.X = blockCenter.X + num2;
          blockCenter.Y -= num1;
          vector3.Y += num1;
          blockCenter.Z -= num1;
          vector3.Z += num1;
          num5 = 2;
          num6 = 0;
          num7 = 3;
          num8 = 1;
          break;
        case 3:
        case 7:
          blockCenter.X -= num1;
          vector3.X += num1;
          blockCenter.Y -= num1;
          vector3.Y += num1;
          blockCenter.Z += num1 - num2;
          vector3.Z += num1;
          num5 = 1;
          num6 = 3;
          num7 = 2;
          num8 = 0;
          break;
      }
      bool flag1 = false;
      switch (num4)
      {
        case 0:
        case 6:
          vector2_3.X = vector2_4.X - num3;
          break;
        case 1:
        case 7:
          vector2_1.X = vector2_2.X - num3;
          flag1 = true;
          break;
        case 2:
        case 4:
          float x1 = vector2_2.X;
          vector2_2.X = vector2_1.X;
          vector2_1.X = x1;
          vector2_3.X = vector2_4.X - num3;
          vector2_5 = vector2_6;
          vector2_6 = vector2_3;
          break;
        case 3:
        case 5:
          float x2 = vector2_4.X;
          vector2_4.X = vector2_3.X;
          vector2_3.X = x2;
          vector2_1.X = vector2_2.X - num3;
          vector2_5 = vector2_6;
          vector2_6 = vector2_1;
          flag1 = true;
          break;
      }
      bool flag2 = this.IsClear(p, blockID, 0, 4);
      bool flag3 = this.IsClear(p, blockID, 0, 5);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.Aux = num4;
      data.IsCorner = false;
      data.UseOwnLight = true;
      data.Face = num5;
      this.SetFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = blockCenter.Y;
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = num7;
      this.SetFacePos(ref data);
      data.X = vector3.X;
      data.Y = blockCenter.Y;
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = blockCenter.X;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Face = num6;
      this.SetFacePos(ref data);
      data.X = vector3.X;
      data.Y = blockCenter.Y;
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Z = blockCenter.Z;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      this.AddVertex(ref data);
      data.Face = num8;
      this.SetFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = blockCenter.Y;
      data.Z = vector3.Z;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_4.Y);
      this.AddVertex(ref data);
      data.Y = vector3.Y;
      data.TC = new NormalizedShort2(vector2_4.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.X = vector3.X;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      this.AddVertex(ref data);
      data.Y = blockCenter.Y;
      data.TC = new NormalizedShort2(vector2_3.X, vector2_4.Y);
      this.AddVertex(ref data);
      if (flag2)
      {
        data.Face = 4;
        this.SetUpFacePos(ref data);
        data.X = blockCenter.X;
        data.Y = vector3.Y;
        data.Z = vector3.Z;
        float x3 = !flag1 ? vector2_5.X : vector2_6.X;
        data.TC = new NormalizedShort2(x3, vector2_6.Y);
        this.AddVertex(ref data);
        data.Z = blockCenter.Z;
        float y1 = !flag1 ? vector2_5.Y : vector2_6.Y;
        data.TC = new NormalizedShort2(vector2_5.X, y1);
        this.AddVertex(ref data);
        data.X = vector3.X;
        float x4 = !flag1 ? vector2_6.X : vector2_5.X;
        data.TC = new NormalizedShort2(x4, vector2_5.Y);
        this.AddVertex(ref data);
        data.Z = vector3.Z;
        float y2 = !flag1 ? vector2_6.Y : vector2_5.Y;
        data.TC = new NormalizedShort2(vector2_6.X, y2);
        this.AddVertex(ref data);
      }
      if (!flag3)
        return;
      data.Face = 5;
      this.SetDownFacePos(ref data);
      data.X = blockCenter.X;
      data.Y = blockCenter.Y;
      data.Z = vector3.Z;
      float x5 = !flag1 ? vector2_5.X : vector2_6.X;
      data.TC = new NormalizedShort2(x5, vector2_6.Y);
      this.AddVertex(ref data);
      data.X = vector3.X;
      float y3 = !flag1 ? vector2_6.Y : vector2_5.Y;
      data.TC = new NormalizedShort2(vector2_6.X, y3);
      this.AddVertex(ref data);
      data.Z = blockCenter.Z;
      float x6 = !flag1 ? vector2_6.X : vector2_5.X;
      data.TC = new NormalizedShort2(x6, vector2_5.Y);
      this.AddVertex(ref data);
      data.X = blockCenter.X;
      float y4 = !flag1 ? vector2_5.Y : vector2_6.Y;
      data.TC = new NormalizedShort2(vector2_5.X, y4);
      this.AddVertex(ref data);
    }

    private void BuildObsidian_NewFormat(GlobalPoint3D p, byte blockID)
    {
      if (this.IsClearLeft(ref p, blockID, 0, 0))
        this.AddLeftFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearForward(ref p, blockID, 0, 1))
        this.AddForwardFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearRight(ref p, blockID, 0, 2))
        this.AddRightFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearBackward(ref p, blockID, 0, 3))
        this.AddBackwardFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearUp(ref p, blockID, 0, 4))
      {
        byte num = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
        byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
        this.AddUpFaceNewFormat(ref p, blockID, textureIdForDrawing);
      }
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockID);
    }

    private void BuildBedrock_NewFormat(GlobalPoint3D p, byte blockID)
    {
      if (this.IsClearUp(ref p, blockID, 0, 4))
      {
        byte bedRockId = GraphicStatics.TexturePack.GetBedRockID();
        if (bedRockId != (byte) 125)
          this.AddUpFaceNewFormat(ref p, blockID, bedRockId);
      }
      if (p.Y <= 0)
        return;
      if (this.IsClearLeft(ref p, blockID, 0, 0))
        this.AddLeftFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearForward(ref p, blockID, 0, 1))
        this.AddForwardFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearRight(ref p, blockID, 0, 2))
        this.AddRightFaceNewFormat(ref p, blockID, blockID);
      if (this.IsClearBackward(ref p, blockID, 0, 3))
        this.AddBackwardFaceNewFormat(ref p, blockID, blockID);
      if (!this.IsClearDown(ref p, blockID, 0, 5))
        return;
      this.AddDownFaceNewFormat(ref p, blockID, blockID);
    }

    private void BuildMultiTexture_NewFormat(ref GlobalPoint3D p, byte blockID)
    {
      byte num = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing((Block) blockID, (int) num);
      if (textureIdForDrawing == (byte) 125)
        return;
      this.BuildAroundSolidBlock(ref p, blockID, textureIdForDrawing);
    }

    private void BuildCoverBlock_NewFormat(ref GlobalPoint3D p, byte blockID)
    {
      byte num1 = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
      if (num1 > (byte) 5)
        num1 = (byte) 0;
      byte num2 = (byte) MapTM.CoverBlockTop[(int) num1];
      --p.Y;
      byte dataBlockId = this.GetDataBlockID(ref p);
      ++p.Y;
      if ((int) dataBlockId == (int) num2 || dataBlockId == (byte) 126 || this.map.BlockData[(int) dataBlockId].Buffer > (byte) 1 || (dataBlockId == (byte) 2 && num2 == (byte) 1 || dataBlockId == (byte) 1 && num2 == (byte) 2))
      {
        blockID = num2;
        this.BuildAroundSolidBlock(ref p, blockID, blockID);
      }
      else
      {
        this.skipUpFace = true;
        this.BuildAroundSolidBlock(ref p, dataBlockId, dataBlockId);
        this.skipUpFace = false;
        int blockIDTexture = num1 == (byte) 0 ? (int) blockID : (num1 < (byte) 3 ? 301 + (int) num1 : (num1 < (byte) 5 ? 347 + (int) num1 : 296 + (int) num1));
        if (this.IsClearLeft(ref p, blockID, 0, 0))
          this.AddLeftFaceNewFormat(ref p, blockID, blockIDTexture);
        if (this.IsClearForward(ref p, blockID, 0, 1))
          this.AddForwardFaceNewFormat(ref p, blockID, blockIDTexture);
        if (this.IsClearRight(ref p, blockID, 0, 2))
          this.AddRightFaceNewFormat(ref p, blockID, blockIDTexture);
        if (this.IsClearBackward(ref p, blockID, 0, 3))
          this.AddBackwardFaceNewFormat(ref p, blockID, blockIDTexture);
        if (!this.IsClearUp(ref p, blockID, 0, 4))
          return;
        this.AddUpFaceNewFormat(ref p, blockID, (int) num2);
      }
    }

    private void BuildArcadeMachine_NewFormat(ref GlobalPoint3D p, byte blockID)
    {
      byte num = (byte) ((uint) this.GetDataBlockAux(ref p) >> 4);
      int blockIDTexture = num == (byte) 0 || num > (byte) 2 ? (int) blockID : (int) num + 298;
      this.BuildAroundSolidBlock2(ref p, blockID, blockIDTexture);
    }

    private void BuildPressurePlate_NewFormat(BlockGroupData group)
    {
      float num1 = this.tilesize * 0.5f;
      float num2 = this.tilesize * 0.25f;
      Vector3 blockCenter = this.map.GetBlockCenter(group.Point);
      blockCenter.X -= num2;
      blockCenter.Z -= num2;
      blockCenter.Y -= num2;
      Vector3 zero = Vector3.Zero;
      zero.X = blockCenter.X + num1;
      zero.Y = blockCenter.Y - num2;
      zero.Z = blockCenter.Z + num1;
      bool flag = this.IsClear((GlobalPoint3D) group.Point, group.Center.BlockID, 0, 5);
      byte blockID = 27;
      GlobalPoint3D p = new GlobalPoint3D();
      this.AddLeftFaceNewFormat(blockCenter, zero, 0, blockID, (int) blockID, ref p, false);
      this.AddForwardFaceNewFormat(blockCenter, zero, 1, blockID, (int) blockID, ref p, false);
      this.AddRightFaceNewFormat(blockCenter, zero, 2, blockID, (int) blockID, ref p, false);
      this.AddBackwardFaceNewFormat(blockCenter, zero, 3, blockID, (int) blockID, ref p, false);
      this.AddUpFaceNewFormat(blockCenter, zero, 4, blockID, (int) blockID, ref p, false);
      if (!flag)
        return;
      this.AddDownFaceNewFormat(blockCenter, zero, 5, blockID, (int) blockID, ref p, false);
    }

    private void BuildItemPickup(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      int itemID,
      float light)
    {
      if (itemID != this.lastTCItemID)
      {
        this.tc = this.GetItemTextureCoords((Item) itemID);
        this.lastTCItemID = itemID;
      }
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Normal = Vector3.Right * light;
      t.Position.X = pos.X + scale;
      t.Position.Y = pos.Y - scale;
      t.Position.Z = pos.Z;
      t.TextureCoordinate.X = this.tc.X;
      t.TextureCoordinate.Y = this.tc.W;
      verts.Add(t);
      t.Position.Y = pos.Y;
      t.TextureCoordinate.Y = this.tc.Y;
      verts.Add(t);
      t.Position.X = pos.X;
      t.TextureCoordinate.X = this.tc.Z;
      verts.Add(t);
      t.Position.Y = pos.Y - scale;
      t.TextureCoordinate.Y = this.tc.W;
      verts.Add(t);
    }

    private void BuildItemProjectile(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      int itemID,
      float light)
    {
      if (itemID != this.lastTCItemID)
      {
        this.tc = this.GetItemTextureCoords((Item) itemID);
        this.lastTCItemID = itemID;
      }
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Normal = Vector3.Right * light;
      t.Position.X = pos.X - scale;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z + scale;
      t.TextureCoordinate.X = this.tc.X;
      t.TextureCoordinate.Y = this.tc.W;
      verts.Add(t);
      t.Position.Z = pos.Z - scale;
      t.TextureCoordinate.Y = this.tc.Y;
      verts.Add(t);
      t.Position.X = pos.X + scale;
      t.TextureCoordinate.X = this.tc.Z;
      verts.Add(t);
      t.Position.Z = pos.Z + scale;
      t.TextureCoordinate.Y = this.tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y + scale;
      t.Position.Z = pos.Z - scale;
      t.TextureCoordinate.X = this.tc.Z;
      t.TextureCoordinate.Y = this.tc.Y;
      verts.Add(t);
      t.Position.Z = pos.Z + scale;
      t.TextureCoordinate.Y = this.tc.W;
      verts.Add(t);
      t.Position.Y = pos.Y - scale;
      t.TextureCoordinate.X = this.tc.X;
      verts.Add(t);
      t.Position.Z = pos.Z - scale;
      t.TextureCoordinate.Y = this.tc.Y;
      verts.Add(t);
    }

    public void BuildBlock(
      MapTM map,
      CustomArray<VertexPositionNormalTexture> vertices,
      Vector3 pos,
      float scale,
      byte blockTextureID,
      float light)
    {
      this.map = map;
      Vector4 tc = new Vector4();
      int texOffset1 = MapChunkContent.TexOffsets[(int) blockTextureID, 0];
      Vector2 vector2_1 = MapChunkContent.TexCoords1[texOffset1];
      tc.X = vector2_1.X;
      tc.Y = vector2_1.Y;
      Vector2 vector2_2 = MapChunkContent.TexCoords4[texOffset1];
      tc.Z = vector2_2.X;
      tc.W = vector2_2.Y;
      this.AddLeftFace(vertices, pos, scale, light, tc);
      int texOffset2 = MapChunkContent.TexOffsets[(int) blockTextureID, 1];
      Vector2 vector2_3 = MapChunkContent.TexCoords1[texOffset2];
      tc.X = vector2_3.X;
      tc.Y = vector2_3.Y;
      Vector2 vector2_4 = MapChunkContent.TexCoords4[texOffset2];
      tc.Z = vector2_4.X;
      tc.W = vector2_4.Y;
      this.AddForwardFace(vertices, pos, scale, light, tc);
      int texOffset3 = MapChunkContent.TexOffsets[(int) blockTextureID, 2];
      Vector2 vector2_5 = MapChunkContent.TexCoords1[texOffset3];
      tc.X = vector2_5.X;
      tc.Y = vector2_5.Y;
      Vector2 vector2_6 = MapChunkContent.TexCoords4[texOffset3];
      tc.Z = vector2_6.X;
      tc.W = vector2_6.Y;
      this.AddRightFace(vertices, pos, scale, light, tc);
      int texOffset4 = MapChunkContent.TexOffsets[(int) blockTextureID, 3];
      Vector2 vector2_7 = MapChunkContent.TexCoords1[texOffset4];
      tc.X = vector2_7.X;
      tc.Y = vector2_7.Y;
      Vector2 vector2_8 = MapChunkContent.TexCoords4[texOffset4];
      tc.Z = vector2_8.X;
      tc.W = vector2_8.Y;
      this.AddBackwardFace(vertices, pos, scale, light, tc);
      int texOffset5 = MapChunkContent.TexOffsets[(int) blockTextureID, 4];
      Vector2 vector2_9 = MapChunkContent.TexCoords1[texOffset5];
      tc.X = vector2_9.X;
      tc.Y = vector2_9.Y;
      Vector2 vector2_10 = MapChunkContent.TexCoords4[texOffset5];
      tc.Z = vector2_10.X;
      tc.W = vector2_10.Y;
      this.AddUpFace(vertices, pos, scale, light, tc);
      int texOffset6 = MapChunkContent.TexOffsets[(int) blockTextureID, 5];
      if (texOffset6 <= 0)
        return;
      Vector2 vector2_11 = MapChunkContent.TexCoords1[texOffset6];
      tc.X = vector2_11.X;
      tc.Y = vector2_11.Y;
      Vector2 vector2_12 = MapChunkContent.TexCoords4[texOffset6];
      tc.Z = vector2_12.X;
      tc.W = vector2_12.Y;
      this.AddDownFace(vertices, pos, scale, light, tc);
    }

    private void AddBackwardFace(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos.Y - scale;
      t.Position.Z = pos.Z + scale;
      t.Normal = Vector3.Backward * light;
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.Y = pos.Y;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X + scale;
      t.TextureCoordinate.X = tc.Z;
      verts.Add(t);
      t.Position.Y = pos.Y - scale;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddForwardFace(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X + scale;
      t.Position.Y = pos.Y - scale;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Forward * light;
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.Y = pos.Y;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X;
      t.TextureCoordinate.X = tc.Z;
      verts.Add(t);
      t.Position.Y = pos.Y - scale;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddRightFace(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X + scale;
      t.Position.Y = pos.Y - scale;
      t.Position.Z = pos.Z + scale;
      t.Normal = Vector3.Right * light;
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.Y = pos.Y;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.Z = pos.Z;
      t.TextureCoordinate.X = tc.Z;
      verts.Add(t);
      t.Position.Y = pos.Y - scale;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddLeftFace(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos.Y - scale;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Left * light;
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.Y = pos.Y;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.Z = pos.Z + scale;
      t.TextureCoordinate.X = tc.Z;
      verts.Add(t);
      t.Position.Y = pos.Y - scale;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddUpFace(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Up * light;
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X + scale;
      t.TextureCoordinate.X = tc.Z;
      verts.Add(t);
      t.Position.Z = pos.Z + scale;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.TextureCoordinate.X = tc.X;
      verts.Add(t);
    }

    private void AddDownFace(
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      float scale,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos.Y - scale;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Down * light;
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.Z = pos.Z + scale;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X + scale;
      t.TextureCoordinate.X = tc.Z;
      verts.Add(t);
      t.Position.Z = pos.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
    }

    private void AddBackwardFace(
      GlobalPoint3D p,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 pos2,
      byte blockID,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Backward * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Down);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Backward * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Up);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Backward * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Up);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Backward * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Down);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddForwardFace(
      GlobalPoint3D p,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 pos2,
      byte blockID,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Forward * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Down);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Forward * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Up);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Forward * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Up);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Forward * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Down);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddRightFace(
      GlobalPoint3D p,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 pos2,
      byte blockID,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Right * this.GetNormalLight(light, p, GlobalPoint3D.Down, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Right * this.GetNormalLight(light, p, GlobalPoint3D.Up, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Right * this.GetNormalLight(light, p, GlobalPoint3D.Up, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Right * this.GetNormalLight(light, p, GlobalPoint3D.Down, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddLeftFace(
      GlobalPoint3D p,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 pos2,
      byte blockID,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Left * this.GetNormalLight(light, p, GlobalPoint3D.Down, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Left * this.GetNormalLight(light, p, GlobalPoint3D.Up, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Left * this.GetNormalLight(light, p, GlobalPoint3D.Up, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Left * this.GetNormalLight(light, p, GlobalPoint3D.Down, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddUpFace(
      GlobalPoint3D p,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 pos2,
      byte blockID,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Up * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Up * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Up * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Up * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddDownFace(
      GlobalPoint3D p,
      CustomArray<VertexPositionNormalTexture> verts,
      Vector3 pos,
      Vector3 pos2,
      byte blockID,
      float light,
      Vector4 tc)
    {
      VertexPositionNormalTexture t = new VertexPositionNormalTexture();
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Down * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
      t.Position.X = pos.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Down * this.GetNormalLight(light, p, GlobalPoint3D.Left, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.X;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos.Z;
      t.Normal = Vector3.Down * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Backward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.Y;
      verts.Add(t);
      t.Position.X = pos2.X;
      t.Position.Y = pos2.Y;
      t.Position.Z = pos2.Z;
      t.Normal = Vector3.Down * this.GetNormalLight(light, p, GlobalPoint3D.Right, GlobalPoint3D.Forward);
      t.TextureCoordinate.X = tc.Z;
      t.TextureCoordinate.Y = tc.W;
      verts.Add(t);
    }

    private void AddLeftFaceNewFormat(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 0;
      this.SetFacePos(ref p, ref data);
      int index = (int) blockIDTexture == (int) blockID ? 0 : 6;
      int textureIndex = 0;
      if (this.blockData.Buffer == (byte) 0)
      {
        byte dataBlockAux = this.GetDataBlockAux(ref p);
        if (this.blockData.IsRotated)
          index = this.rotTexFace[index, (int) dataBlockAux & 3];
        textureIndex = (int) dataBlockAux >> 4;
      }
      int texOffset = MapChunkContent.TexOffsets[(int) blockIDTexture, index];
      this.AddLeftFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, texOffset, ref p, false);
      if (textureIndex <= 0)
        return;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing(Block.zLastBlockID, textureIndex);
      if (textureIdForDrawing == byte.MaxValue)
        return;
      this.AddLeftFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, this.GetDecalTextureID(textureIdForDrawing), ref p, false);
    }

    private void AddLeftFaceNewFormat(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 0;
      this.SetFacePos(ref p, ref data);
      this.AddLeftFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, blockIDTexture, ref p, false);
    }

    private void AddLeftFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      int blockIDTexture,
      ref GlobalPoint3D p,
      bool useOwnLight)
    {
      Vector2 tc1 = MapChunkContent.TexCoords1[blockIDTexture];
      Vector2 tc2 = MapChunkContent.TexCoords2[blockIDTexture];
      Vector2 tc3 = MapChunkContent.TexCoords3[blockIDTexture];
      Vector2 tc4 = MapChunkContent.TexCoords4[blockIDTexture];
      tc3.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc3.Y - (double) tc1.Y));
      tc4.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc4.Y - (double) tc1.Y));
      if (this.blockData.IsRotated && this.blockData.Buffer == (byte) 0)
        this.RotateTexCoords(ref p, (byte) 0, ref tc1, ref tc2, ref tc3, ref tc4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = useOwnLight;
      data.Face = face;
      data.Pos1 = pos1;
      data.Pos2 = pos2;
      data.WindUniformWaveRandomness = (int) this.blockData.WindAffect;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(tc3.X, tc3.Y);
      this.AddVertex(ref data);
      data.Y = pos2.Y;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.Y = pos1.Y;
      data.TC = new NormalizedShort2(tc4.X, tc4.Y);
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void AddForwardFaceNewFormat(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 1;
      this.SetFacePos(ref p, ref data);
      int index = (int) blockIDTexture == (int) blockID ? 1 : 6;
      int textureIndex = 0;
      if (this.blockData.Buffer == (byte) 0)
      {
        byte dataBlockAux = this.GetDataBlockAux(ref p);
        if (this.blockData.IsRotated)
          index = this.rotTexFace[index, (int) dataBlockAux & 3];
        textureIndex = (int) dataBlockAux >> 4;
      }
      int texOffset = MapChunkContent.TexOffsets[(int) blockIDTexture, index];
      this.AddForwardFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, texOffset, ref p, false);
      if (textureIndex <= 0)
        return;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing(Block.zLastBlockID, textureIndex);
      if (textureIdForDrawing == byte.MaxValue)
        return;
      this.AddForwardFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, this.GetDecalTextureID(textureIdForDrawing), ref p, false);
    }

    private void AddForwardFaceNewFormat(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 1;
      this.SetFacePos(ref p, ref data);
      this.AddForwardFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, blockIDTexture, ref p, false);
    }

    private void AddForwardFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      int blockIDTexture,
      ref GlobalPoint3D p,
      bool useOwnLight)
    {
      Vector2 tc1 = MapChunkContent.TexCoords1[blockIDTexture];
      Vector2 tc2 = MapChunkContent.TexCoords2[blockIDTexture];
      Vector2 tc3 = MapChunkContent.TexCoords3[blockIDTexture];
      Vector2 tc4 = MapChunkContent.TexCoords4[blockIDTexture];
      tc3.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc3.Y - (double) tc1.Y));
      tc4.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc4.Y - (double) tc1.Y));
      if (this.blockData.IsRotated && this.blockData.Buffer == (byte) 0)
        this.RotateTexCoords(ref p, (byte) 1, ref tc1, ref tc2, ref tc3, ref tc4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = useOwnLight;
      data.Face = face;
      data.Pos1 = pos1;
      data.Pos2 = pos2;
      data.WindUniformWaveRandomness = (int) this.blockData.WindAffect;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(tc3.X, tc3.Y);
      this.AddVertex(ref data);
      data.Y = pos2.Y;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.Y = pos1.Y;
      data.TC = new NormalizedShort2(tc4.X, tc4.Y);
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void AddRightFaceNewFormat(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 2;
      this.SetFacePos(ref p, ref data);
      int index = (int) blockIDTexture == (int) blockID ? 2 : 6;
      int textureIndex = 0;
      if (this.blockData.Buffer == (byte) 0)
      {
        byte dataBlockAux = this.GetDataBlockAux(ref p);
        if (this.blockData.IsRotated)
          index = this.rotTexFace[index, (int) dataBlockAux & 3];
        textureIndex = (int) dataBlockAux >> 4;
      }
      int texOffset = MapChunkContent.TexOffsets[(int) blockIDTexture, index];
      this.AddRightFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, texOffset, ref p, false);
      if (textureIndex <= 0)
        return;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing(Block.zLastBlockID, textureIndex);
      if (textureIdForDrawing == byte.MaxValue)
        return;
      this.AddRightFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, this.GetDecalTextureID(textureIdForDrawing), ref p, false);
    }

    private void AddRightFaceNewFormat(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 2;
      this.SetFacePos(ref p, ref data);
      this.AddRightFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, blockIDTexture, ref p, false);
    }

    private void AddRightFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      int blockIDTexture,
      ref GlobalPoint3D p,
      bool useOwnLight)
    {
      Vector2 tc1 = MapChunkContent.TexCoords1[blockIDTexture];
      Vector2 tc2 = MapChunkContent.TexCoords2[blockIDTexture];
      Vector2 tc3 = MapChunkContent.TexCoords3[blockIDTexture];
      Vector2 tc4 = MapChunkContent.TexCoords4[blockIDTexture];
      tc3.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc3.Y - (double) tc1.Y));
      tc4.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc4.Y - (double) tc1.Y));
      if (this.blockData.IsRotated && this.blockData.Buffer == (byte) 0)
        this.RotateTexCoords(ref p, (byte) 2, ref tc1, ref tc2, ref tc3, ref tc4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = useOwnLight;
      data.Face = face;
      data.Pos1 = pos1;
      data.Pos2 = pos2;
      data.WindUniformWaveRandomness = (int) this.blockData.WindAffect;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(tc3.X, tc3.Y);
      this.AddVertex(ref data);
      data.Y = pos2.Y;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.Y = pos1.Y;
      data.TC = new NormalizedShort2(tc4.X, tc4.Y);
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void AddBackwardFaceNewFormat(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 3;
      this.SetFacePos(ref p, ref data);
      int index = (int) blockIDTexture == (int) blockID ? 3 : 6;
      int textureIndex = 0;
      if (this.blockData.Buffer == (byte) 0)
      {
        byte dataBlockAux = this.GetDataBlockAux(ref p);
        if (this.blockData.IsRotated)
          index = this.rotTexFace[index, (int) dataBlockAux & 3];
        textureIndex = (int) dataBlockAux >> 4;
      }
      int texOffset = MapChunkContent.TexOffsets[(int) blockIDTexture, index];
      this.AddBackwardFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, texOffset, ref p, false);
      if (textureIndex <= 0)
        return;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing(Block.zLastBlockID, textureIndex);
      if (textureIdForDrawing == byte.MaxValue)
        return;
      this.AddBackwardFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, this.GetDecalTextureID(textureIdForDrawing), ref p, false);
    }

    private void AddBackwardFaceNewFormat(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 3;
      this.SetFacePos(ref p, ref data);
      this.AddBackwardFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, blockIDTexture, ref p, false);
    }

    private void AddBackwardFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      int blockIDTexture,
      ref GlobalPoint3D p,
      bool useOwnLight)
    {
      Vector2 tc1 = MapChunkContent.TexCoords1[blockIDTexture];
      Vector2 tc2 = MapChunkContent.TexCoords2[blockIDTexture];
      Vector2 tc3 = MapChunkContent.TexCoords3[blockIDTexture];
      Vector2 tc4 = MapChunkContent.TexCoords4[blockIDTexture];
      tc3.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc3.Y - (double) tc1.Y));
      tc4.Y = tc1.Y + (float) (((double) pos2.Y - (double) pos1.Y) / (double) this.tilesize * ((double) tc4.Y - (double) tc1.Y));
      if (this.blockData.IsRotated && this.blockData.Buffer == (byte) 0)
        this.RotateTexCoords(ref p, (byte) 3, ref tc1, ref tc2, ref tc3, ref tc4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = useOwnLight;
      data.Face = face;
      data.Pos1 = pos1;
      data.Pos2 = pos2;
      data.WindUniformWaveRandomness = (int) this.blockData.WindAffect;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(tc3.X, tc3.Y);
      this.AddVertex(ref data);
      data.Y = pos2.Y;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      data.WindAffected = this.isWindAffected;
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.Y = pos1.Y;
      data.TC = new NormalizedShort2(tc4.X, tc4.Y);
      data.WindAffected = false;
      this.AddVertex(ref data);
    }

    private void AddUpFaceNewFormat(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 4;
      this.SetUpFacePos(ref p, ref data);
      int index = (int) blockIDTexture == (int) blockID ? 4 : 6;
      int textureIndex = 0;
      if (this.blockData.Buffer == (byte) 0)
      {
        byte dataBlockAux = this.GetDataBlockAux(ref p);
        if (this.blockData.IsRotated)
          index = this.rotTexFace[index, (int) dataBlockAux & 3];
        textureIndex = (int) dataBlockAux >> 4;
      }
      int texOffset = MapChunkContent.TexOffsets[(int) blockIDTexture, index];
      this.AddUpFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, texOffset, ref p, false);
      if (textureIndex <= 0)
        return;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing(Block.zLastBlockID, textureIndex);
      if (textureIdForDrawing == byte.MaxValue)
        return;
      this.AddUpFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, this.GetDecalTextureID(textureIdForDrawing), ref p, false);
    }

    private void AddUpFaceNewFormat(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 4;
      this.SetUpFacePos(ref p, ref data);
      this.AddUpFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, blockIDTexture, ref p, false);
    }

    private void AddUpFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      int blockIDTexture,
      ref GlobalPoint3D p,
      bool useOwnLight)
    {
      Vector2 tc1 = MapChunkContent.TexCoords1[blockIDTexture];
      Vector2 tc2 = MapChunkContent.TexCoords2[blockIDTexture];
      Vector2 tc3 = MapChunkContent.TexCoords3[blockIDTexture];
      Vector2 tc4 = MapChunkContent.TexCoords4[blockIDTexture];
      if (this.blockData.IsRotated && this.blockData.Buffer == (byte) 0)
        this.RotateTexCoords(ref p, (byte) 4, ref tc1, ref tc2, ref tc3, ref tc4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = useOwnLight;
      data.Face = face;
      data.Pos1 = pos1;
      data.Pos2 = pos2;
      data.WindAffected = this.isWindAffected;
      data.WindUniformWaveRandomness = (int) this.blockData.WindAffect;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(tc3.X, tc3.Y);
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      this.AddVertex(ref data);
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.X = pos1.X;
      data.TC = new NormalizedShort2(tc4.X, tc4.Y);
      this.AddVertex(ref data);
    }

    private void AddDownFaceNewFormat(ref GlobalPoint3D p, byte blockID, byte blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 5;
      this.SetDownFacePos(ref p, ref data);
      int index = (int) blockIDTexture == (int) blockID ? 5 : 6;
      int textureIndex = 0;
      if (this.blockData.Buffer == (byte) 0)
      {
        byte dataBlockAux = this.GetDataBlockAux(ref p);
        if (this.blockData.IsRotated)
          index = this.rotTexFace[index, (int) dataBlockAux & 3];
        textureIndex = (int) dataBlockAux >> 4;
      }
      int texOffset = MapChunkContent.TexOffsets[(int) blockIDTexture, index];
      this.AddDownFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, texOffset, ref p, false);
      if (textureIndex <= 0)
        return;
      byte textureIdForDrawing = (byte) this.map.GetBlockTextureIDForDrawing(Block.zLastBlockID, textureIndex);
      if (textureIdForDrawing == byte.MaxValue)
        return;
      this.AddDownFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, this.GetDecalTextureID(textureIdForDrawing), ref p, false);
    }

    private void AddDownFaceNewFormat(ref GlobalPoint3D p, byte blockID, int blockIDTexture)
    {
      VoxelMeshBuilder.FaceData data = new VoxelMeshBuilder.FaceData();
      data.Face = 5;
      this.SetDownFacePos(ref p, ref data);
      this.AddDownFaceNewFormat(data.Pos1, data.Pos2, data.Face, blockID, blockIDTexture, ref p, false);
    }

    private void AddDownFaceNewFormat(
      Vector3 pos1,
      Vector3 pos2,
      int face,
      byte blockID,
      int blockIDTexture,
      ref GlobalPoint3D p,
      bool useOwnLight)
    {
      Vector2 tc1 = MapChunkContent.TexCoords1[blockIDTexture];
      Vector2 tc2 = MapChunkContent.TexCoords2[blockIDTexture];
      Vector2 tc3 = MapChunkContent.TexCoords3[blockIDTexture];
      Vector2 tc4 = MapChunkContent.TexCoords4[blockIDTexture];
      if (this.blockData.IsRotated && this.blockData.Buffer == (byte) 0)
        this.RotateTexCoords(ref p, (byte) 5, ref tc1, ref tc2, ref tc3, ref tc4);
      AVParams data = new AVParams();
      data.Point = p;
      data.BlockID = blockID;
      data.IsCorner = true;
      data.UseOwnLight = useOwnLight;
      data.Face = face;
      data.Pos1 = pos1;
      data.Pos2 = pos2;
      data.X = pos1.X;
      data.Y = pos1.Y;
      data.Z = pos2.Z;
      data.TC = new NormalizedShort2(tc3.X, tc3.Y);
      this.AddVertex(ref data);
      data.X = pos2.X;
      data.TC = new NormalizedShort2(tc1.X, tc1.Y);
      this.AddVertex(ref data);
      data.Z = pos1.Z;
      data.TC = new NormalizedShort2(tc2.X, tc2.Y);
      this.AddVertex(ref data);
      data.X = pos1.X;
      data.TC = new NormalizedShort2(tc4.X, tc4.Y);
      this.AddVertex(ref data);
    }

    private void SetFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      if (this.blockData.IsRotated && this.blockData.Buffer > (byte) 1)
      {
        int dataBlockAux = (int) this.GetDataBlockAux(ref p);
        data.Face = data.Face + (dataBlockAux & 3) & 3;
      }
      this.SetFacePos2Array[data.Face](ref p, ref data);
    }

    private void SetFacePos(ref AVParams data)
    {
      if (this.blockData.IsRotated && this.blockData.Buffer > (byte) 1)
        data.Face = data.Face + ((int) data.Aux & 3) & 3;
      this.SetFacePos1Array[data.Face](ref data);
    }

    private void SetLeftFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      data.Pos1 = this.map.GetPosition(p);
      data.Pos2 = new Vector3();
      data.Pos2.X = data.Pos1.X;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos2.Z = data.Pos1.Z + this.tilesize;
    }

    private void SetLeftFacePos(ref AVParams data)
    {
      data.Pos1 = this.map.GetPosition(data.Point);
      data.Pos2 = new Vector3();
      data.Pos2.X = data.Pos1.X;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos2.Z = data.Pos1.Z + this.tilesize;
    }

    private void SetForwardFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      data.Pos1 = this.map.GetPosition(p);
      data.Pos2.X = data.Pos1.X;
      data.Pos1.X += this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos2.Z = data.Pos1.Z;
    }

    private void SetForwardFacePos(ref AVParams data)
    {
      data.Pos1 = this.map.GetPosition(data.Point);
      data.Pos2.X = data.Pos1.X;
      data.Pos1.X += this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos2.Z = data.Pos1.Z;
    }

    private void SetRightFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      data.Pos1 = this.map.GetPosition(p);
      data.Pos1.X += this.tilesize;
      data.Pos2.X = data.Pos1.X;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos2.Z = data.Pos1.Z;
      data.Pos1.Z += this.tilesize;
    }

    private void SetRightFacePos(ref AVParams data)
    {
      data.Pos1 = this.map.GetPosition(data.Point);
      data.Pos1.X += this.tilesize;
      data.Pos2.X = data.Pos1.X;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos2.Z = data.Pos1.Z;
      data.Pos1.Z += this.tilesize;
    }

    private void SetBackwardFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      data.Pos1 = this.map.GetPosition(p);
      data.Pos2.X = data.Pos1.X + this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos1.Z += this.tilesize;
      data.Pos2.Z = data.Pos1.Z;
    }

    private void SetBackwardFacePos(ref AVParams data)
    {
      data.Pos1 = this.map.GetPosition(data.Point);
      data.Pos2.X = data.Pos1.X + this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
      data.Pos1.Y -= this.tilesize;
      data.Pos1.Z += this.tilesize;
      data.Pos2.Z = data.Pos1.Z;
    }

    private void SetUpFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      data.Pos1 = this.map.GetPosition(p);
      data.Pos2.X = data.Pos1.X + this.tilesize;
      data.Pos2.Z = data.Pos1.Z + this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
    }

    private void SetUpFacePos(ref AVParams data)
    {
      data.Pos1 = this.map.GetPosition(data.Point);
      data.Pos2.X = data.Pos1.X + this.tilesize;
      data.Pos2.Z = data.Pos1.Z + this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
    }

    private void SetDownFacePos(ref GlobalPoint3D p, ref VoxelMeshBuilder.FaceData data)
    {
      data.Pos1 = this.map.GetPosition(p);
      data.Pos1.Y -= this.tilesize;
      data.Pos2.X = data.Pos1.X + this.tilesize;
      data.Pos2.Z = data.Pos1.Z + this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
    }

    private void SetDownFacePos(ref AVParams data)
    {
      data.Pos1 = this.map.GetPosition(data.Point);
      data.Pos1.Y -= this.tilesize;
      data.Pos2.X = data.Pos1.X + this.tilesize;
      data.Pos2.Z = data.Pos1.Z + this.tilesize;
      data.Pos2.Y = data.Pos1.Y;
    }

    private void AddVertex()
    {
      this.newFormatVertices.Add(this.newVertex);
    }

    private void AddVertex(
      float x,
      float y,
      float z,
      int face,
      float tx,
      float ty,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p)
    {
      this.AddVertex(x, y, z, face, new NormalizedShort2(tx, ty), blockID, aux, ref p);
    }

    private void AddVertex(
      Vector3 pos,
      int face,
      float tx,
      float ty,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p)
    {
      this.AddVertex(pos.X, pos.Y, pos.Z, face, new NormalizedShort2(tx, ty), blockID, aux, ref p);
    }

    private void AddVertex(
      float x,
      float y,
      float z,
      int face,
      NormalizedShort2 tc,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p)
    {
      AVParams data = new AVParams();
      data.Point = p;
      data.X = x;
      data.Y = y;
      data.Z = z;
      data.Face = face;
      data.TC = tc;
      data.BlockID = blockID;
      data.Aux = aux;
      this.SetFacePos(ref data);
      this.AddVertex(ref data);
    }

    private void AddVertex(ref AVParams data)
    {
      byte blockId = data.BlockID;
      if (!data.UseOwnLight)
      {
        Vector2 vector2 = this.getFaceLight[data.Face](ref data);
        byte num = blockId == (byte) 166 ? this.map.GetLuminance(ref data.Point, blockId) : this.blockData.Luminance;
        vector2.Y = Math.Max(vector2.Y, (float) num / this.map.MaxLight);
        this.newVertex.Light = new NormalizedShort2(vector2.X, vector2.Y);
      }
      else
        this.newVertex.Light = new NormalizedShort2(this.GetDataBlock(ref data.Point).Light.ToVector2((Map) this.map));
      uint num1 = 0;
      if (data.WindAffected)
      {
        num1 += 6U;
        if (data.WindUniformWaveRandomness > 1)
        {
          byte num2 = (byte) ((double) data.X + (double) (data.WindUniformWaveRandomnessHash >> 16));
          byte num3 = (byte) ((double) data.Y + (double) (byte) data.WindUniformWaveRandomnessHash);
          byte num4 = (byte) ((double) data.Z + (double) (byte) (data.WindUniformWaveRandomnessHash >> 8));
          this.xxHashBuff[0] = num2;
          this.xxHashBuff[1] = num3;
          this.xxHashBuff[2] = num4;
          uint hash = xxHash.CalculateHash(this.xxHashBuff, 3, (uint) this.map.Seed);
          num1 += hash % 1000U * 6U;
        }
      }
      this.newVertex.Position = new HalfVector4(data.X - this.chunkVectorOffset.X, data.Y - this.chunkVectorOffset.Y, data.Z - this.chunkVectorOffset.Z, (float) ((long) data.Face + (long) num1));
      this.newVertex.TexCoord.PackedValue = data.TC.PackedValue;
      if (!this.isWaterVertex)
        this.newFormatVertices.Add(this.newVertex);
      else
        this.newFormatWaterVertices.Add(this.newVertex);
    }

    private bool IsClearLeft(ref GlobalPoint3D p, byte blockID, int aux, int face)
    {
      if (p.X <= this.mapBound.Min.X)
        return false;
      GlobalPoint3D np = p;
      --np.X;
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClearForward(ref GlobalPoint3D p, byte blockID, int aux, int face)
    {
      if (p.Z <= this.mapBound.Min.Z)
        return false;
      GlobalPoint3D np = p;
      --np.Z;
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClearRight(ref GlobalPoint3D p, byte blockID, int aux, int face)
    {
      if (p.X >= this.mapBound.Max.X - 1)
        return false;
      GlobalPoint3D np = p;
      ++np.X;
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClearBackward(ref GlobalPoint3D p, byte blockID, int aux, int face)
    {
      if (p.Z >= this.mapBound.Max.Z - 1)
        return false;
      GlobalPoint3D np = p;
      ++np.Z;
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClearUp(ref GlobalPoint3D p, byte blockID, int aux, int face)
    {
      if (this.skipUpFace || p.Y >= this.mapBound.Max.Y - 1)
        return false;
      GlobalPoint3D np = p;
      ++np.Y;
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClearDown(ref GlobalPoint3D p, byte blockID, int aux, int face)
    {
      if (p.Y <= this.mapBound.Min.Y)
        return false;
      GlobalPoint3D np = p;
      --np.Y;
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClear(GlobalPoint3D p, byte blockID, int aux, int face)
    {
      GlobalPoint3D np = p;
      switch (face)
      {
        case 0:
          if (p.X <= this.mapBound.Min.X)
            return false;
          --np.X;
          break;
        case 1:
          if (p.Z <= this.mapBound.Min.Z)
            return false;
          --np.Z;
          break;
        case 2:
          if (p.X >= this.mapBound.Max.X - 1)
            return false;
          ++np.X;
          break;
        case 3:
          if (p.Z >= this.mapBound.Max.Z - 1)
            return false;
          ++np.Z;
          break;
        case 4:
          if (p.Y >= this.mapBound.Max.Y - 1)
            return false;
          ++np.Y;
          break;
        case 5:
          if (p.Y <= this.mapBound.Min.Y)
            return false;
          --np.Y;
          break;
      }
      return this.IsClearTest(ref p, ref np, blockID, aux, face);
    }

    private bool IsClearTest(
      ref GlobalPoint3D op,
      ref GlobalPoint3D np,
      byte origBlockID,
      int aux,
      int face)
    {
      byte dataBlockId = this.GetDataBlockID(ref np);
      if (dataBlockId == (byte) 0)
        return true;
      BlockDataXML blockDataXml = this.map.BlockData[(int) dataBlockId];
      byte buffer = blockDataXml.Buffer;
      if (buffer < (byte) 1)
        return false;
      if (buffer > (byte) 2)
        return true;
      MapBlock dataBlock = this.GetDataBlock(ref np);
      return this.IsClearPartial(ref op, ref np, buffer, dataBlockId, ref dataBlock, origBlockID, aux, face, (int) blockDataXml.TextureID);
    }

    private bool IsClearPartial(
      ref GlobalPoint3D op,
      ref GlobalPoint3D np,
      byte buffer,
      byte blockIDToTest,
      ref MapBlock itemToTest,
      byte origBlockID,
      int aux,
      int face,
      int textureID)
    {
      Block block1 = (Block) blockIDToTest;
      if ((uint) block1 <= 116U)
      {
        switch (block1)
        {
          case Block.WoodDoorTop:
          case Block.SteelDoorTop:
          case Block.WoodDoorBottom:
          case Block.SteelDoorBottom:
            break;
          default:
            goto label_4;
        }
      }
      else if (block1 != Block.LockedDoorTop && block1 != Block.LockedDoorBottom)
        goto label_4;
      return true;
label_4:
      if (textureID > 0)
      {
        Block block2 = (Block) blockIDToTest;
        if ((uint) block2 <= 132U)
        {
          if (block2 != Block.StainedGlassPane)
          {
            if (block2 == Block.LockedChest)
              return false;
            goto label_11;
          }
        }
        else
        {
          switch (block2)
          {
            case Block.Painting:
              goto label_13;
            case Block.StainedGlass:
              break;
            default:
              goto label_11;
          }
        }
        return true;
label_11:
        if (this.map.BlockData[(int) this.map.GetBlockTextureIDForDrawing((Block) blockIDToTest, (int) itemToTest.AuxData >> 4)].Buffer > (byte) 2)
          return true;
      }
label_13:
      int num1 = (int) itemToTest.AuxData & 7;
      int num2 = num1 & 3;
      int num3 = aux & 3;
      switch (origBlockID)
      {
        case 182:
          origBlockID = (byte) 113;
          break;
        case 183:
          origBlockID = (byte) 149;
          break;
        case 184:
          origBlockID = (byte) 150;
          break;
        case 185:
          origBlockID = (byte) 130;
          break;
      }
      switch (blockIDToTest)
      {
        case 182:
          blockIDToTest = (byte) 113;
          break;
        case 183:
          blockIDToTest = (byte) 149;
          break;
        case 184:
          blockIDToTest = (byte) 150;
          break;
        case 185:
          blockIDToTest = (byte) 130;
          break;
      }
      Block block3 = (Block) origBlockID;
      Block block4 = (Block) blockIDToTest;
      if ((uint) block4 <= 145U)
      {
        if ((uint) block4 <= 113U)
        {
          switch (block4)
          {
            case Block.Post:
              break;
            case Block.Stairs:
              bool flag1 = num1 > 3 && aux > 3 || num1 < 4 && aux < 4;
              Block block5 = block3;
              if ((uint) block5 <= 130U)
              {
                switch (block5)
                {
                  case Block.Stairs:
                    if (face > 3 && num3 == num2 && !flag1)
                      return false;
                    if (face < 4)
                    {
                      int num4 = face < 4 ? (face - num3 + 4) % 4 : face;
                      if (num4 == 0)
                        return !flag1;
                      if (aux == num1 && (num4 == 1 || num4 == 3))
                        return false;
                      if ((num3 + 4 - num2) % 4 == 2)
                      {
                        if (num4 != 1)
                          return num4 == 3;
                        return true;
                      }
                      if (num4 != 2)
                        return true;
                      goto label_115;
                    }
                    else
                      goto label_115;
                  case Block.Stack:
                    break;
                  default:
                    goto label_115;
                }
              }
              else
              {
                switch (block5)
                {
                  case Block.SnowLayer:
                    break;
                  case Block.HalfBlock:
                    if (face == 5)
                      return num1 < 4;
                    if (face == 4)
                      return num1 > 3;
                    if ((num3 == 0 ? (num1 > 3 ? 1 : 0) : (num1 < 4 ? 1 : 0)) != 0)
                      return face != num2;
                    return false;
                  case Block.Ramp:
                    if (aux < 4 && face == 5 || face == 4)
                      return flag1;
                    if ((face < 4 ? (face - num3 + 4) % 4 : face) != 2)
                    {
                      if (face < 4 && !flag1)
                        return true;
                      if (num2 == num3)
                        return false;
                      if ((num2 + 2) % 4 != num3)
                        return (num2 + 2) % 4 == face;
                      return true;
                    }
                    goto label_115;
                  default:
                    goto label_115;
                }
              }
              switch (face)
              {
                case 4:
                  if (num1 <= 3)
                    return aux < 7;
                  return true;
                case 5:
                  return num1 < 4;
                default:
                  if (num1 <= 3)
                    return aux > 3;
                  return true;
              }
label_115:
              if (face == 5)
                return num2 == num1;
              if (face == 4)
                return num2 != num1;
              return face != num2;
            default:
              goto label_234;
          }
        }
        else
        {
          switch (block4)
          {
            case Block.Stack:
              Block block6 = block3;
              if ((uint) block6 <= 131U)
              {
                switch (block6)
                {
                  case Block.Stairs:
                    if (face != 5 && face != 4)
                      return num1 < 7;
                    break;
                  case Block.UpsideDownStack:
                    return num1 < 7;
                }
              }
              else
              {
                switch (block6)
                {
                  case Block.HalfBlock:
                    if (face != 5 && face != 4)
                    {
                      if (aux != 0)
                        return num1 < 7;
                      return num1 < 3;
                    }
                    break;
                  case Block.PressurePlate:
                    return false;
                }
              }
              switch (face)
              {
                case 4:
                  if (block3 == Block.Stack)
                    return aux < 7;
                  return false;
                case 5:
                  return num1 < 7;
                default:
                  if (block3 == Block.Stack)
                    return num1 < aux;
                  return num1 < 7;
              }
            case Block.UpsideDownStack:
              Block block7 = block3;
              if ((uint) block7 <= 130U)
              {
                switch (block7)
                {
                  case Block.Stairs:
                    if (face != 5 && face != 4)
                      return num1 < 7;
                    goto label_173;
                  case Block.Stack:
                    break;
                  default:
                    goto label_173;
                }
              }
              else
              {
                switch (block7)
                {
                  case Block.HalfBlock:
                    if (face != 5 && face != 4)
                    {
                      if (aux == 0)
                        return num1 < 7;
                      return num1 < 3;
                    }
                    goto label_173;
                  case Block.PressurePlate:
                    break;
                  default:
                    goto label_173;
                }
              }
              return num1 < 7;
label_173:
              if (face == 5)
              {
                if (block3 == Block.UpsideDownStack)
                  return aux < 7;
                return false;
              }
              if (face == 4 || block3 != Block.UpsideDownStack)
                return num1 < 7;
              return num1 < aux;
            case Block.BedHead:
            case Block.BedFoot:
              return face == 4 || face == 5 || block3 != Block.BedHead && block3 != Block.BedFoot;
            case Block.Fence:
              return block3 != Block.Fence;
            case Block.SnowLayer:
              switch (face)
              {
                case 4:
                  if (block3 == Block.SnowLayer)
                    return aux < 7;
                  return true;
                case 5:
                  if (block3 == Block.SnowLayer)
                    return num1 < 7;
                  return true;
                default:
                  if (block3 == Block.SnowLayer)
                    return num1 != aux;
                  return true;
              }
            default:
              goto label_234;
          }
        }
      }
      else if ((uint) block4 <= 154U)
      {
        switch (block4)
        {
          case Block.HalfBlock:
            Block block8 = block3;
            if ((uint) block8 <= 131U)
            {
              switch (block8)
              {
                case Block.Stairs:
                  if (face == 5)
                    return num2 == 0;
                  if (face == 4)
                    return num2 != 0;
                  if ((face - num3 + 4) % 4 != 0)
                    return true;
                  return num2 == (aux > 3 ? 0 : 1);
                case Block.Stack:
                  break;
                case Block.UpsideDownStack:
                  switch (face)
                  {
                    case 4:
                      return num2 != 0;
                    case 5:
                      if (num2 != 0)
                        return aux < 7;
                      return true;
                    default:
                      if (num2 != 0)
                        return aux > 3;
                      return true;
                  }
                default:
                  goto label_69;
              }
            }
            else
            {
              switch (block8)
              {
                case Block.SnowLayer:
                  break;
                case Block.HalfBlock:
                  switch (face)
                  {
                    case 4:
                      if (num3 == 1)
                        return num2 != 0;
                      return true;
                    case 5:
                      if (num3 == 0)
                        return num2 != 1;
                      return true;
                    default:
                      return num3 != num2;
                  }
                case Block.Painting:
                  if (face == 5 || aux > 3 && face == 2)
                    return num2 == 0;
                  if (face == 4 && num2 != 1)
                    return aux > 3;
                  return true;
                default:
                  goto label_69;
              }
            }
            switch (face)
            {
              case 4:
                if (num2 == 0)
                  return aux < 7;
                return true;
              case 5:
                return num2 == 0;
              default:
                if (num2 == 0)
                  return aux > 3;
                return true;
            }
label_69:
            if (face == 4)
              return num2 == 1;
            if (face != 5)
              return true;
            return num2 == 0;
          case Block.Ramp:
            bool flag2 = num1 > 3 && aux > 3 || num1 < 4 && aux < 4;
            Block block9 = block3;
            if ((uint) block9 <= 130U)
            {
              switch (block9)
              {
                case Block.Stairs:
                  if (face == 4)
                    return num1 > 3;
                  if (face == 5)
                    return num1 < 4;
                  goto label_139;
                case Block.Stack:
                  break;
                default:
                  goto label_139;
              }
            }
            else
            {
              switch (block9)
              {
                case Block.SnowLayer:
                  break;
                case Block.Ramp:
                  if (face > 3)
                    return flag2;
                  if ((face - num3 + 4) % 4 != 2)
                    return aux != num1;
                  goto label_139;
                case Block.Painting:
                  if (face == 2 && aux > 3 && num2 != num1)
                    return false;
                  goto label_139;
                default:
                  goto label_139;
              }
            }
            switch (face)
            {
              case 4:
                if (num1 <= 3)
                  return aux < 7;
                return true;
              case 5:
                return num1 < 4;
              default:
                return true;
            }
label_139:
            if (face == 5)
              return num2 == num1;
            if (face == 4)
              return num2 != num1;
            return face != num2;
          case Block.Cylinder:
            return block3 != Block.Cylinder;
          default:
            goto label_234;
        }
      }
      else
      {
        switch (block4)
        {
          case Block.Table:
            if (face == 5)
              return false;
            if (face == 4)
              return true;
            return block3 != Block.Table;
          case Block.Painting:
            if (block3 == Block.Painting)
            {
              if ((face == 5 || face == 2 && aux > 3) && num1 > 3)
                return true;
              if (face == 4 && num1 > 3)
                return false;
              if (aux > 3)
                return num1 < 4;
              if (num2 != num1)
                return true;
              return num3 != num2;
            }
            if (face == 4)
              return num1 < 4;
            if (face == num2)
              return num1 > 3;
            return true;
          case Block.PressurePlate:
            if (face == 5)
              return true;
            if (face == 4)
              return false;
            if (block3 == Block.PressurePlate)
              return this.strategy.IsBlockDeliveringPower(op) != this.strategy.IsBlockDeliveringPower(np);
            return true;
          case Block.Post2:
            break;
          default:
            goto label_234;
        }
      }
      if (face == 4 || face == 5)
      {
        if (block3 == Block.Post || block3 == Block.Post2)
          return aux != num1;
        if (block3 == Block.Fence)
          return false;
      }
      return block3 != Block.Button && block3 != Block.Switch || (face != 2 || aux <= 3);
label_234:
      return buffer > (byte) 1;
    }

    private int GetDecalTextureID(byte decalID)
    {
      if (decalID > (byte) 28)
        return 387 + (int) decalID;
      if (decalID >= (byte) 15)
        return 369 + (int) decalID;
      return 351 + (int) decalID;
    }

    private float CalcTexCoord(float tc1, float tc2, float pos1, float pos2)
    {
      float num = tc2 - tc1;
      if ((double) pos2 <= (double) pos1)
        return tc1 + (pos1 - pos2) * num;
      return tc1 + (pos2 - pos1) * num;
    }

    private float CalcTexCoord(float tc1, float tc2, ref AVParams data)
    {
      float num = tc2 - tc1;
      switch (data.Face)
      {
        case 0:
        case 4:
          return tc1 + (data.Z - data.Pos1.Z) * num;
        case 1:
        case 5:
          return tc1 + (data.Pos1.X - data.X) * num;
        case 2:
          return tc1 + (data.Pos1.Z - data.Z) * num;
        case 3:
          return tc1 + (data.X - data.Pos1.X) * num;
        default:
          return tc1;
      }
    }

    private Point3D GetRotatedPoint(byte aux, Direction dir)
    {
      switch ((int) (dir - 1 + (int) aux) % 4 + 1)
      {
        case 1:
          return Point3D.Left;
        case 2:
          return Point3D.Forward;
        case 3:
          return Point3D.Right;
        case 4:
          return Point3D.Backward;
        default:
          throw new Exception("");
      }
    }

    private VoxelMeshBuilder.FaceData RotatePositios(
      GlobalPoint3D p,
      int aux,
      VoxelMeshBuilder.FaceData data)
    {
      Vector3 blockCenter = this.map.GetBlockCenter(p);
      Vector3 position1 = new Vector3();
      Vector3 position2 = new Vector3();
      position1.X = data.Pos1.X - blockCenter.X;
      position2.X = data.Pos2.X - blockCenter.X;
      position1.Y = data.Pos1.Y - blockCenter.Y;
      position2.Y = data.Pos2.Y - blockCenter.Y;
      Matrix rotatedBlockMatrix = MapTM.RotatedBlockMatrices[aux & 3];
      Vector3.Transform(ref position1, ref rotatedBlockMatrix, out data.Pos1);
      Vector3.Transform(ref position2, ref rotatedBlockMatrix, out data.Pos2);
      data.Pos1.X += blockCenter.X;
      data.Pos2.X += blockCenter.X;
      data.Pos1.Y += blockCenter.Y;
      data.Pos2.Y += blockCenter.Y;
      data.Face = (data.Face + aux) % 4;
      return data;
    }

    private void RotateTexCoords(
      ref GlobalPoint3D p,
      byte face,
      ref Vector2 tc1,
      ref Vector2 tc2,
      ref Vector2 tc3,
      ref Vector2 tc4)
    {
      int num = (int) this.GetDataBlockAux(ref p) & 3;
      switch (num)
      {
        case 0:
          return;
        case 2:
          if (face >= (byte) 4)
            return;
          break;
      }
      Vector2 vector2 = new Vector2();
      vector2.X = (float) (((double) tc4.X - (double) tc1.X) * 0.5) + tc1.X;
      vector2.Y = (float) (((double) tc4.Y - (double) tc1.Y) * 0.5) + tc1.Y;
      Vector2 position1 = new Vector2();
      Vector2 position2 = new Vector2();
      Vector2 position3 = new Vector2();
      Vector2 position4 = new Vector2();
      position1.X = tc1.X - vector2.X;
      position2.X = tc2.X - vector2.X;
      position3.X = tc3.X - vector2.X;
      position4.X = tc4.X - vector2.X;
      position1.Y = tc1.Y - vector2.Y;
      position2.Y = tc2.Y - vector2.Y;
      position3.Y = tc3.Y - vector2.Y;
      position4.Y = tc4.Y - vector2.Y;
      position1.X *= 2.461539f;
      position2.X *= 2.461539f;
      position3.X *= 2.461539f;
      position4.X *= 2.461539f;
      Matrix rotMatTexCoord = this.rotMatTexCoords[num - 1];
      Vector2.Transform(ref position1, ref rotMatTexCoord, out tc1);
      Vector2.Transform(ref position2, ref rotMatTexCoord, out tc2);
      Vector2.Transform(ref position3, ref rotMatTexCoord, out tc3);
      Vector2.Transform(ref position4, ref rotMatTexCoord, out tc4);
      tc1.X /= 2.461539f;
      tc2.X /= 2.461539f;
      tc3.X /= 2.461539f;
      tc4.X /= 2.461539f;
      tc1.X += vector2.X;
      tc2.X += vector2.X;
      tc3.X += vector2.X;
      tc4.X += vector2.X;
      tc1.Y += vector2.Y;
      tc2.Y += vector2.Y;
      tc3.Y += vector2.Y;
      tc4.Y += vector2.Y;
    }

    public void ShiftPositions(CustomArray<VertexPositionNormalTexture> vertices, Vector3 delta)
    {
        for (int i = 0; i < vertices.Array.Length; i++)
        {
            vertices.Array[i].Position += delta;
        }
    }

    public void NormalizeNormals(CustomArray<VertexPositionNormalTexture> vertices)
    {
      foreach (VertexPositionNormalTexture positionNormalTexture in vertices.Array)
        positionNormalTexture.Normal.Normalize();
    }

    private float GetNormalLight(float light, GlobalPoint3D p, GlobalPoint3D p1, GlobalPoint3D p2)
    {
      GlobalPoint3D p3 = p;
      p3.X += p1.X;
      p3.Y += p1.Y;
      p3.Z += p1.Z;
      GlobalPoint3D p4 = p;
      p4.X += p2.X;
      p4.Y += p2.Y;
      p4.Z += p2.Z;
      GlobalPoint3D p5 = p4;
      p5.X += p1.X;
      p5.Y += p1.Y;
      p5.Z += p1.Z;
      bool flag1 = this.map.IsValidPoint(p3);
      bool flag2 = this.map.IsValidPoint(p4);
      float num1 = flag1 ? this.map.GetLightNormalized(p3) : light;
      float num2 = flag2 ? this.map.GetLightNormalized(p4) : light;
      float num3 = flag1 && flag2 && ((double) num1 > 0.0 || (double) num2 > 0.0) ? this.map.GetLightNormalized(p5) : light;
      return (float) (((double) light + ((double) num1 + (double) num2 + (double) num3) / 3.0 * 2.0) / 3.0);
    }

    private float GetNormalisedSunLight(
      float light,
      GlobalPoint3D p,
      GlobalPoint3D p1,
      GlobalPoint3D p2)
    {
      GlobalPoint3D p3 = p;
      p3.X += p1.X;
      p3.Y += p1.Y;
      p3.Z += p1.Z;
      GlobalPoint3D p4 = p;
      p4.X += p2.X;
      p4.Y += p2.Y;
      p4.Z += p2.Z;
      GlobalPoint3D p5 = p4;
      p5.X += p1.X;
      p5.Y += p1.Y;
      p5.Z += p1.Z;
      bool flag1 = this.map.IsValidPoint(p3);
      bool flag2 = this.map.IsValidPoint(p4);
      float num1 = flag1 ? this.map.GetSunLightNormalized(p3) : light;
      float num2 = flag2 ? this.map.GetSunLightNormalized(p4) : light;
      float num3 = flag1 && flag2 && ((double) num1 > 0.0 || (double) num2 > 0.0) ? this.map.GetSunLightNormalized(p5) : light;
      return (float) (((double) light + ((double) num1 + (double) num2 + (double) num3) / 3.0 * 2.0) / 3.0);
    }

    private float GetNormalisedBlockLight(
      float light,
      GlobalPoint3D p,
      GlobalPoint3D p1,
      GlobalPoint3D p2)
    {
      GlobalPoint3D p3 = p;
      p3.X += p1.X;
      p3.Y += p1.Y;
      p3.Z += p1.Z;
      GlobalPoint3D p4 = p;
      p4.X += p2.X;
      p4.Y += p2.Y;
      p4.Z += p2.Z;
      GlobalPoint3D p5 = p4;
      p5.X += p1.X;
      p5.Y += p1.Y;
      p5.Z += p1.Z;
      bool flag1 = this.map.IsValidPoint(p3);
      bool flag2 = this.map.IsValidPoint(p4);
      float num1 = flag1 ? this.map.GetBlockLightNormalized(p3) : light;
      float num2 = flag2 ? this.map.GetBlockLightNormalized(p4) : light;
      float num3 = flag1 && flag2 && ((double) num1 > 0.0 || (double) num2 > 0.0) ? this.map.GetBlockLightNormalized(p5) : light;
      return (float) (((double) light + ((double) num1 + (double) num2 + (double) num3) / 3.0 * 2.0) / 3.0);
    }

    private float GetNormalizedSunLight(byte light, MapBlock m1, MapBlock m2, MapBlock m3)
    {
      int sunLight1 = (int) m1.Light.SunLight;
      int sunLight2 = (int) m2.Light.SunLight;
      int num = (int) m3.Light.SunLight;
      if (sunLight1 == 0 && this.map.BlockData[(int) m1.BlockID].Opacity > (byte) 16 && (sunLight2 == 0 && this.map.BlockData[(int) m2.BlockID].Opacity > (byte) 16))
        num = 0;
      return this.map.GetLightNormalized((byte) ((double) ((float) ((int) light + sunLight1 + sunLight2 + num) / 4f) + 0.990000009536743));
    }

    private float GetNormalizedBlockLight(byte light, MapBlock m1, MapBlock m2, MapBlock m3)
    {
      int blockLight1 = (int) m1.Light.BlockLight;
      int blockLight2 = (int) m2.Light.BlockLight;
      int num = (int) m3.Light.BlockLight;
      if (blockLight1 == 0 && this.map.BlockData[(int) m1.BlockID].Opacity > (byte) 16 && (blockLight2 == 0 && this.map.BlockData[(int) m2.BlockID].Opacity > (byte) 16))
        num = 0;
      return this.map.GetLightNormalized((byte) ((double) ((float) ((int) light + blockLight1 + blockLight2 + num) / 4f) + 0.990000009536743));
    }

    private float GetAverageLight(GlobalPoint3D p)
    {
      float lightNormalized = this.map.GetLightNormalized(p);
      GlobalPoint3D globalPoint3D = this.mapBound.Max - this.mapBound.Min;
      float num1;
      if (p.X > this.mapBound.Min.X)
      {
        --p.X;
        num1 = lightNormalized + this.map.GetLightNormalized(p);
        ++p.X;
      }
      else
        num1 = lightNormalized + 1f;
      float num2;
      if (p.X < globalPoint3D.X - 1)
      {
        ++p.X;
        num2 = num1 + this.map.GetLightNormalized(p);
        --p.X;
      }
      else
        num2 = num1 + 1f;
      float num3;
      if (p.Z > this.mapBound.Min.Z)
      {
        --p.Z;
        num3 = num2 + this.map.GetLightNormalized(p);
        ++p.Z;
      }
      else
        num3 = num2 + 1f;
      float num4;
      if (p.Z < globalPoint3D.Z - 1)
      {
        ++p.Z;
        num4 = num3 + this.map.GetLightNormalized(p);
        --p.Z;
      }
      else
        num4 = num3 + 1f;
      float num5;
      if (p.Y < this.mapBound.Max.Y - 1)
      {
        ++p.Y;
        num5 = num4 + this.map.GetLightNormalized(p);
        --p.Y;
      }
      else
        num5 = num4 + 1f;
      float num6;
      if (p.Y > this.mapBound.Min.Y)
      {
        --p.Y;
        num6 = num5 + this.map.GetLightNormalized(p);
        ++p.Y;
      }
      else
        num6 = num5 + 1f;
      return num6 / 7f;
    }

    private void TransformVerticesToFacePlayer(
      CustomArray<VertexPositionNormalTexture> vertices,
      int offset,
      int count,
      Vector3 pos,
      Player player,
      float light)
    {
      Matrix billboard = Matrix.CreateBillboard(pos, player.EyePosition, Vector3.Up, new Vector3?(player.ViewDirection));
      Vector3 vector3 = Vector3.Right * light * 2f;
      VertexPositionNormalTexture[] array = vertices.Array;
      for (int index = offset; index < offset + count; ++index)
      {
        array[index].Position = Vector3.Transform(array[index].Position - pos, billboard);
        array[index].Normal = vector3;
      }
    }

    private void TransformVertices(
      CustomArray<VertexPositionNormalTexture> vertices,
      int offset,
      int count,
      Vector3 pos,
      Vector3 dir,
      float light)
    {
      Matrix matrix = Matrix.Invert(Matrix.CreateLookAt(pos, pos + dir, Vector3.Up));
      Vector3 vector3 = Vector3.Right * light * 2f;
      VertexPositionNormalTexture[] array = vertices.Array;
      for (int index = offset; index < offset + count; ++index)
      {
        array[index].Position = Vector3.Transform(array[index].Position - pos, matrix);
        array[index].Normal = vector3;
      }
    }

    private void TransformVerticesForSpin(
      CustomArray<VertexPositionNormalTexture> vertices,
      int offset,
      int count,
      Vector3 pos,
      float rotation)
    {
      Matrix rotationY = Matrix.CreateRotationY(rotation);
      VertexPositionNormalTexture[] array = vertices.Array;
      for (int index = offset; index < offset + count; ++index)
        array[index].Position = Vector3.Transform(array[index].Position - pos, rotationY) + pos;
    }

    public void TransformVertices(VertexPositionNormalTexture[] verts, Matrix m)
    {
      for (int index = 0; index < verts.Length; ++index)
        verts[index].Position = Vector3.Transform(verts[index].Position, m);
    }

    private Vector4 GetItemTextureCoords(Item itemID)
    {
      if (itemID > Item.zLastBlockID)
      {
        Rectangle rectangle = GraphicStatics.TexturePack.ItemSrcRect(itemID);
        float width = (float) GraphicStatics.TexturePack.ItemTexture.Width;
        float height = (float) GraphicStatics.TexturePack.ItemTexture.Height;
        float x = (float) rectangle.X / width;
        float y = (float) rectangle.Y / height;
        float z = x + (float) rectangle.Width / width;
        float w = y + (float) rectangle.Height / height;
        return new Vector4(x, y, z, w);
      }
      Vector2 vector2_1 = MapChunkContent.TexCoords3[MapChunkContent.TexOffsets[(int) itemID, 0]];
      Vector2 vector2_2 = MapChunkContent.TexCoords2[MapChunkContent.TexOffsets[(int) itemID, 0]];
      return new Vector4()
      {
        X = vector2_1.X,
        Y = vector2_2.Y,
        Z = vector2_2.X,
        W = vector2_1.Y
      };
    }

    private byte GetLeftBlockID(GlobalPoint3D p)
    {
      switch ((int) this.GetDataBlock(ref p).AuxData & 3)
      {
        case 1:
          --p.Z;
          return this.GetDataBlock(ref p).BlockID;
        case 2:
          ++p.X;
          return this.GetDataBlock(ref p).BlockID;
        case 3:
          ++p.Z;
          return this.GetDataBlock(ref p).BlockID;
        default:
          --p.X;
          return this.GetDataBlock(ref p).BlockID;
      }
    }

    private byte GetForwardBlockID(GlobalPoint3D p)
    {
      switch ((int) this.GetDataBlockAux(ref p) & 3)
      {
        case 1:
          ++p.X;
          return this.GetDataBlock(ref p).BlockID;
        case 2:
          ++p.Z;
          return this.GetDataBlock(ref p).BlockID;
        case 3:
          --p.X;
          return this.GetDataBlock(ref p).BlockID;
        default:
          --p.Z;
          return this.GetDataBlock(ref p).BlockID;
      }
    }

    private byte GetRightBlockID(GlobalPoint3D p)
    {
      switch ((int) this.GetDataBlockAux(ref p) & 3)
      {
        case 1:
          ++p.Z;
          return this.GetDataBlock(ref p).BlockID;
        case 2:
          --p.X;
          return this.GetDataBlock(ref p).BlockID;
        case 3:
          --p.Z;
          return this.GetDataBlock(ref p).BlockID;
        default:
          ++p.X;
          return this.GetDataBlock(ref p).BlockID;
      }
    }

    private byte GetBackwardBlockID(GlobalPoint3D p)
    {
      switch ((int) this.GetDataBlockAux(ref p) & 3)
      {
        case 1:
          --p.X;
          return this.GetDataBlock(ref p).BlockID;
        case 2:
          --p.Z;
          return this.GetDataBlock(ref p).BlockID;
        case 3:
          ++p.X;
          return this.GetDataBlock(ref p).BlockID;
        default:
          ++p.Z;
          return this.GetDataBlock(ref p).BlockID;
      }
    }

    private byte GetUpBlockID(GlobalPoint3D p)
    {
      ++p.Y;
      return this.GetDataBlock(ref p).BlockID;
    }

    private byte GetDownBlockID(GlobalPoint3D p)
    {
      --p.Y;
      return this.GetDataBlock(ref p).BlockID;
    }

    private Vector2 GetLeftFaceLight(ref AVParams data)
    {
      if (this.oldskoolLight)
        return this.GetDataBlock(new GlobalPoint3D(data.Point.X - 1, data.Point.Y, data.Point.Z)).Light.ToVector2((Map) this.map);
      if (data.IsCorner)
        return this.GetLeftFaceLightCore(ref data);
      float x = data.X;
      float y = data.Y;
      float z = data.Z;
      data.Z = data.Pos1.Z;
      data.Y = data.Pos1.Y;
      Vector2 leftFaceLightCore1 = this.GetLeftFaceLightCore(ref data);
      data.Z = data.Pos2.Z;
      Vector2 leftFaceLightCore2 = this.GetLeftFaceLightCore(ref data);
      data.Z = data.Pos1.Z;
      data.Y = data.Pos2.Y;
      Vector2 leftFaceLightCore3 = this.GetLeftFaceLightCore(ref data);
      data.Z = data.Pos2.Z;
      Vector2 leftFaceLightCore4 = this.GetLeftFaceLightCore(ref data);
      data.X = x;
      data.Y = y;
      data.Z = z;
      float num1 = (z - data.Pos1.Z) / this.tilesize;
      float num2 = (y - data.Pos1.Y) / this.tilesize;
      float num3 = (leftFaceLightCore2.X - leftFaceLightCore1.X) * num1 + leftFaceLightCore1.X;
      float num4 = (leftFaceLightCore4.X - leftFaceLightCore3.X) * num1 + leftFaceLightCore3.X;
      float num5 = (leftFaceLightCore2.Y - leftFaceLightCore1.Y) * num1 + leftFaceLightCore1.Y;
      float num6 = (leftFaceLightCore4.Y - leftFaceLightCore3.Y) * num1 + leftFaceLightCore3.Y;
      return new Vector2((num4 - num3) * num2 + num3, (num6 - num5) * num2 + num5);
    }

    private Vector2 GetLeftFaceLightCore(ref AVParams data)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      GlobalPoint3D p4 = new GlobalPoint3D();
      p1.X = data.Point.X - 1;
      p1.Y = data.Point.Y;
      p1.Z = data.Point.Z;
      p2.X = p1.X;
      p3.X = p1.X;
      p4.X = p1.X;
      p2.Z = p1.Z;
      p4.Y = p1.Y;
      if ((double) data.Z == (double) data.Pos1.Z)
      {
        p3.Z = p1.Z - 1;
        p4.Z = p3.Z;
      }
      else if ((double) data.Z == (double) data.Pos2.Z)
      {
        p3.Z = p1.Z + 1;
        p4.Z = p3.Z;
      }
      else
      {
        p3.Z = p1.Z;
        p4.Z = p1.Z;
      }
      if ((double) data.Y == (double) data.Pos1.Y)
      {
        p2.Y = p1.Y - 1;
        p3.Y = p2.Y;
      }
      else if ((double) data.Y == (double) data.Pos2.Y)
      {
        p2.Y = p1.Y + 1;
        p3.Y = p2.Y;
      }
      else
      {
        p2.Y = p1.Y;
        p3.Y = p1.Y;
      }
      return this.GetLight(ref p1, ref p2, ref p3, ref p4);
    }

    private Vector2 GetForwardFaceLight(ref AVParams data)
    {
      if (this.oldskoolLight)
        return this.GetDataBlock(new GlobalPoint3D(data.Point.X, data.Point.Y, data.Point.Z - 1)).Light.ToVector2((Map) this.map);
      if (data.IsCorner)
        return this.GetForwardFaceLightCore(ref data);
      float x = data.X;
      float y = data.Y;
      float z = data.Z;
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y;
      Vector2 forwardFaceLightCore1 = this.GetForwardFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 forwardFaceLightCore2 = this.GetForwardFaceLightCore(ref data);
      data.X = data.Pos1.X;
      data.Y = data.Pos2.Y;
      Vector2 forwardFaceLightCore3 = this.GetForwardFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 forwardFaceLightCore4 = this.GetForwardFaceLightCore(ref data);
      data.X = x;
      data.Y = y;
      data.Z = z;
      float num1 = (data.Pos1.X - x) / this.tilesize;
      float num2 = (y - data.Pos1.Y) / this.tilesize;
      float num3 = (forwardFaceLightCore2.X - forwardFaceLightCore1.X) * num1 + forwardFaceLightCore1.X;
      float num4 = (forwardFaceLightCore4.X - forwardFaceLightCore3.X) * num1 + forwardFaceLightCore3.X;
      float num5 = (forwardFaceLightCore2.Y - forwardFaceLightCore1.Y) * num1 + forwardFaceLightCore1.Y;
      float num6 = (forwardFaceLightCore4.Y - forwardFaceLightCore3.Y) * num1 + forwardFaceLightCore3.Y;
      return new Vector2((num4 - num3) * num2 + num3, (num6 - num5) * num2 + num5);
    }

    private Vector2 GetForwardFaceLightCore(ref AVParams data)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      GlobalPoint3D p4 = new GlobalPoint3D();
      p1.X = data.Point.X;
      p1.Y = data.Point.Y;
      p1.Z = data.Point.Z - 1;
      p2.Z = p1.Z;
      p3.Z = p1.Z;
      p4.Z = p1.Z;
      p2.X = p1.X;
      p4.Y = p1.Y;
      if ((double) data.X == (double) data.Pos1.X)
      {
        p3.X = p1.X + 1;
        p4.X = p3.X;
      }
      else if ((double) data.X == (double) data.Pos2.X)
      {
        p3.X = p1.X - 1;
        p4.X = p3.X;
      }
      else
      {
        p3.X = p1.X;
        p4.X = p1.X;
      }
      if ((double) data.Y == (double) data.Pos1.Y)
      {
        p2.Y = p1.Y - 1;
        p3.Y = p2.Y;
      }
      else if ((double) data.Y == (double) data.Pos2.Y)
      {
        p2.Y = p1.Y + 1;
        p3.Y = p2.Y;
      }
      else
      {
        p2.Y = p1.Y;
        p3.Y = p1.Y;
      }
      return this.GetLight(ref p1, ref p2, ref p3, ref p4);
    }

    private Vector2 GetRightFaceLight(ref AVParams data)
    {
      if (this.oldskoolLight)
        return this.GetDataBlock(new GlobalPoint3D(data.Point.X + 1, data.Point.Y, data.Point.Z)).Light.ToVector2((Map) this.map);
      if (data.IsCorner)
        return this.GetRightFaceLightCore(ref data);
      float x = data.X;
      float y = data.Y;
      float z = data.Z;
      data.Z = data.Pos1.Z;
      data.Y = data.Pos1.Y;
      Vector2 rightFaceLightCore1 = this.GetRightFaceLightCore(ref data);
      data.Z = data.Pos2.Z;
      Vector2 rightFaceLightCore2 = this.GetRightFaceLightCore(ref data);
      data.Z = data.Pos1.Z;
      data.Y = data.Pos2.Y;
      Vector2 rightFaceLightCore3 = this.GetRightFaceLightCore(ref data);
      data.Z = data.Pos2.Z;
      Vector2 rightFaceLightCore4 = this.GetRightFaceLightCore(ref data);
      data.X = x;
      data.Y = y;
      data.Z = z;
      float num1 = (data.Pos1.Z - z) / this.tilesize;
      float num2 = (y - data.Pos1.Y) / this.tilesize;
      float num3 = (rightFaceLightCore2.X - rightFaceLightCore1.X) * num1 + rightFaceLightCore1.X;
      float num4 = (rightFaceLightCore4.X - rightFaceLightCore3.X) * num1 + rightFaceLightCore3.X;
      float num5 = (rightFaceLightCore2.Y - rightFaceLightCore1.Y) * num1 + rightFaceLightCore1.Y;
      float num6 = (rightFaceLightCore4.Y - rightFaceLightCore3.Y) * num1 + rightFaceLightCore3.Y;
      return new Vector2((num4 - num3) * num2 + num3, (num6 - num5) * num2 + num5);
    }

    private Vector2 GetRightFaceLightCore(ref AVParams data)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      GlobalPoint3D p4 = new GlobalPoint3D();
      p1.X = data.Point.X + 1;
      p1.Y = data.Point.Y;
      p1.Z = data.Point.Z;
      p2.X = p1.X;
      p3.X = p1.X;
      p4.X = p1.X;
      p2.Z = p1.Z;
      p4.Y = p1.Y;
      if ((double) data.Z == (double) data.Pos1.Z)
      {
        p3.Z = p1.Z + 1;
        p4.Z = p3.Z;
      }
      else if ((double) data.Z == (double) data.Pos2.Z)
      {
        p3.Z = p1.Z - 1;
        p4.Z = p3.Z;
      }
      if ((double) data.Y == (double) data.Pos1.Y)
      {
        p2.Y = p1.Y - 1;
        p3.Y = p2.Y;
      }
      else if ((double) data.Y == (double) data.Pos2.Y)
      {
        p2.Y = p1.Y + 1;
        p3.Y = p2.Y;
      }
      return this.GetLight(ref p1, ref p2, ref p3, ref p4);
    }

    private Vector2 GetBackwardFaceLight(ref AVParams data)
    {
      if (this.oldskoolLight)
        return this.GetDataBlock(new GlobalPoint3D(data.Point.X, data.Point.Y, data.Point.Z + 1)).Light.ToVector2((Map) this.map);
      if (data.IsCorner)
        return this.GetBackwardFaceLightCore(ref data);
      float x = data.X;
      float y = data.Y;
      float z = data.Z;
      data.X = data.Pos1.X;
      data.Y = data.Pos1.Y;
      Vector2 backwardFaceLightCore1 = this.GetBackwardFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 backwardFaceLightCore2 = this.GetBackwardFaceLightCore(ref data);
      data.X = data.Pos1.X;
      data.Y = data.Pos2.Y;
      Vector2 backwardFaceLightCore3 = this.GetBackwardFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 backwardFaceLightCore4 = this.GetBackwardFaceLightCore(ref data);
      data.X = x;
      data.Y = y;
      data.Z = z;
      float num1 = (x - data.Pos1.X) / this.tilesize;
      float num2 = (y - data.Pos1.Y) / this.tilesize;
      float num3 = (backwardFaceLightCore2.X - backwardFaceLightCore1.X) * num1 + backwardFaceLightCore1.X;
      float num4 = (backwardFaceLightCore4.X - backwardFaceLightCore3.X) * num1 + backwardFaceLightCore3.X;
      float num5 = (backwardFaceLightCore2.Y - backwardFaceLightCore1.Y) * num1 + backwardFaceLightCore1.Y;
      float num6 = (backwardFaceLightCore4.Y - backwardFaceLightCore3.Y) * num1 + backwardFaceLightCore3.Y;
      return new Vector2((num4 - num3) * num2 + num3, (num6 - num5) * num2 + num5);
    }

    private Vector2 GetBackwardFaceLightCore(ref AVParams data)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      GlobalPoint3D p4 = new GlobalPoint3D();
      p1.X = data.Point.X;
      p1.Y = data.Point.Y;
      p1.Z = data.Point.Z + 1;
      p2.Z = p1.Z;
      p3.Z = p1.Z;
      p4.Z = p1.Z;
      p2.X = p1.X;
      p4.Y = p1.Y;
      if ((double) data.X == (double) data.Pos1.X)
      {
        p3.X = p1.X - 1;
        p4.X = p3.X;
      }
      else if ((double) data.X == (double) data.Pos2.X)
      {
        p3.X = p1.X + 1;
        p4.X = p3.X;
      }
      else
      {
        p3.X = p1.X;
        p4.X = p1.X;
      }
      if ((double) data.Y == (double) data.Pos1.Y)
      {
        p2.Y = p1.Y - 1;
        p3.Y = p2.Y;
      }
      else if ((double) data.Y == (double) data.Pos2.Y)
      {
        p2.Y = p1.Y + 1;
        p3.Y = p2.Y;
      }
      else
      {
        p2.Y = p1.Y;
        p3.Y = p1.Y;
      }
      return this.GetLight(ref p1, ref p2, ref p3, ref p4);
    }

    private Vector2 GetUpFaceLight(ref AVParams data)
    {
      if (this.oldskoolLight)
        return this.GetDataBlock(new GlobalPoint3D(data.Point.X, data.Point.Y + 1, data.Point.Z)).Light.ToVector2((Map) this.map);
      if (data.IsCorner)
        return this.GetUpFaceLightCore(ref data);
      float x = data.X;
      float y = data.Y;
      float z = data.Z;
      data.X = data.Pos1.X;
      data.Z = data.Pos1.Z;
      Vector2 upFaceLightCore1 = this.GetUpFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 upFaceLightCore2 = this.GetUpFaceLightCore(ref data);
      data.X = data.Pos1.X;
      data.Z = data.Pos2.Z;
      Vector2 upFaceLightCore3 = this.GetUpFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 upFaceLightCore4 = this.GetUpFaceLightCore(ref data);
      data.X = x;
      data.Y = y;
      data.Z = z;
      float num1 = (x - data.Pos1.X) / this.tilesize;
      float num2 = (z - data.Pos1.Z) / this.tilesize;
      float num3 = (upFaceLightCore2.X - upFaceLightCore1.X) * num1 + upFaceLightCore1.X;
      float num4 = (upFaceLightCore4.X - upFaceLightCore3.X) * num1 + upFaceLightCore3.X;
      float num5 = (upFaceLightCore2.Y - upFaceLightCore1.Y) * num1 + upFaceLightCore1.Y;
      float num6 = (upFaceLightCore4.Y - upFaceLightCore3.Y) * num1 + upFaceLightCore3.Y;
      return new Vector2((num4 - num3) * num2 + num3, (num6 - num5) * num2 + num5);
    }

    private Vector2 GetUpFaceLightCore(ref AVParams data)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      GlobalPoint3D p4 = new GlobalPoint3D();
      p1.X = data.Point.X;
      p1.Y = data.Point.Y + 1;
      p1.Z = data.Point.Z;
      p2.Y = p1.Y;
      p3.Y = p1.Y;
      p4.Y = p1.Y;
      p2.X = p1.X;
      p4.Z = p1.Z;
      if ((double) data.X == (double) data.Pos1.X)
      {
        p3.X = p1.X - 1;
        p4.X = p3.X;
      }
      else if ((double) data.X == (double) data.Pos2.X)
      {
        p3.X = p1.X + 1;
        p4.X = p3.X;
      }
      else
      {
        p3.X = p1.X;
        p4.X = p1.X;
      }
      if ((double) data.Z == (double) data.Pos1.Z)
      {
        p2.Z = p1.Z - 1;
        p3.Z = p2.Z;
      }
      else if ((double) data.Z == (double) data.Pos2.Z)
      {
        p2.Z = p1.Z + 1;
        p3.Z = p2.Z;
      }
      else
      {
        p2.Z = p1.Z;
        p3.Z = p1.Z;
      }
      return this.GetLight(ref p1, ref p2, ref p3, ref p4);
    }

    private Vector2 GetDownFaceLight(ref AVParams data)
    {
      if (this.oldskoolLight)
        return this.GetDataBlock(new GlobalPoint3D(data.Point.X, data.Point.Y - 1, data.Point.Z)).Light.ToVector2((Map) this.map);
      if (data.IsCorner)
        return this.GetDownFaceLightCore(ref data);
      float x = data.X;
      float y = data.Y;
      float z = data.Z;
      data.X = data.Pos1.X;
      data.Z = data.Pos1.Z;
      Vector2 downFaceLightCore1 = this.GetDownFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 downFaceLightCore2 = this.GetDownFaceLightCore(ref data);
      data.X = data.Pos1.X;
      data.Z = data.Pos2.Z;
      Vector2 downFaceLightCore3 = this.GetDownFaceLightCore(ref data);
      data.X = data.Pos2.X;
      Vector2 downFaceLightCore4 = this.GetDownFaceLightCore(ref data);
      data.X = x;
      data.Y = y;
      data.Z = z;
      float num1 = (x - data.Pos1.X) / this.tilesize;
      float num2 = (z - data.Pos1.Z) / this.tilesize;
      float num3 = (downFaceLightCore2.X - downFaceLightCore1.X) * num1 + downFaceLightCore1.X;
      float num4 = (downFaceLightCore4.X - downFaceLightCore3.X) * num1 + downFaceLightCore3.X;
      float num5 = (downFaceLightCore2.Y - downFaceLightCore1.Y) * num1 + downFaceLightCore1.Y;
      float num6 = (downFaceLightCore4.Y - downFaceLightCore3.Y) * num1 + downFaceLightCore3.Y;
      return new Vector2((num4 - num3) * num2 + num3, (num6 - num5) * num2 + num5);
    }

    private Vector2 GetDownFaceLightCore(ref AVParams data)
    {
      GlobalPoint3D p1 = new GlobalPoint3D();
      GlobalPoint3D p2 = new GlobalPoint3D();
      GlobalPoint3D p3 = new GlobalPoint3D();
      GlobalPoint3D p4 = new GlobalPoint3D();
      p1.X = data.Point.X;
      p1.Y = data.Point.Y - 1;
      p1.Z = data.Point.Z;
      p2.Y = p1.Y;
      p3.Y = p1.Y;
      p4.Y = p1.Y;
      p2.X = p1.X;
      p4.Z = p1.Z;
      if ((double) data.X == (double) data.Pos1.X)
      {
        p3.X = p1.X - 1;
        p4.X = p3.X;
      }
      else if ((double) data.X == (double) data.Pos2.X)
      {
        p3.X = p1.X + 1;
        p4.X = p3.X;
      }
      else
      {
        p3.X = p1.X;
        p4.X = p1.X;
      }
      if ((double) data.Z == (double) data.Pos1.Z)
      {
        p2.Z = p1.Z - 1;
        p3.Z = p2.Z;
      }
      else if ((double) data.Z == (double) data.Pos2.Z)
      {
        p2.Z = p1.Z + 1;
        p3.Z = p2.Z;
      }
      else
      {
        p2.Z = p1.Z;
        p3.Z = p1.Z;
      }
      return this.GetLight(ref p1, ref p2, ref p3, ref p4);
    }

    private Vector2 GetLight(
      ref GlobalPoint3D p1,
      ref GlobalPoint3D p2,
      ref GlobalPoint3D p3,
      ref GlobalPoint3D p4)
    {
      byte sunLight1;
      byte blockLight1;
      this.GetDataLightCore(ref p1, out sunLight1, out blockLight1);
      byte sunLight2;
      byte blockLight2;
      this.GetDataLightCore(ref p2, out sunLight2, out blockLight2);
      byte sunLight3;
      byte blockLight3;
      this.GetDataLightCore(ref p3, out sunLight3, out blockLight3);
      byte sunLight4;
      byte blockLight4;
      this.GetDataLightCore(ref p4, out sunLight4, out blockLight4);
      Vector2 zero = Vector2.Zero;
      float maxLight = this.map.MaxLight;
      zero.X = (float) (((double) sunLight1 / (double) maxLight + (double) sunLight2 / (double) maxLight + (double) sunLight3 / (double) maxLight + (double) sunLight4 / (double) maxLight) * 0.25);
      zero.Y = (float) (((double) blockLight1 / (double) maxLight + (double) blockLight2 / (double) maxLight + (double) blockLight3 / (double) maxLight + (double) blockLight4 / (double) maxLight) * 0.25);
      return zero;
    }

    private byte GetDataBlockID_TestBounds(ref GlobalPoint3D p)
    {
      if (p.X >= this.mapBound.Min.X && p.X < this.mapBound.Max.X && (p.Z >= this.mapBound.Min.Z && p.Z < this.mapBound.Max.Z) && (p.Y >= this.mapBound.Min.Y && p.Y < this.mapBound.Max.Y))
        return this.GetDataBlockID(ref p);
      return this.map.OutOfBoundsBlockID;
    }

    private byte GetDataBlockID(ref GlobalPoint3D p)
    {
      VoxelMeshBuilder.RleCache rleCache = this.GetRleCache(ref p);
      return rleCache.BlockCache[rleCache.BlockCacheIndex + rleCache.Chunk.GetMapIndex(p)];
    }

    private byte GetDataBlockAux(ref GlobalPoint3D p)
    {
      VoxelMeshBuilder.RleCache rleCache = this.GetRleCache(ref p);
      return rleCache.AuxCache[rleCache.AuxCacheIndex + rleCache.Chunk.GetMapIndex(p)];
    }

    private VoxelMeshBuilder.RleCache GetRleCache(ref GlobalPoint3D p)
    {
      int num1 = (p.X + this.mapIndexCenter.X + this.chunksize.X) % (this.chunksize.X * 3) / this.chunksize.X;
      int num2 = (p.Y + this.mapIndexCenter.Y + this.chunksize.Y) % (this.chunksize.Y * 3) / this.chunksize.Y;
      int num3 = (p.Z + this.mapIndexCenter.Z + this.chunksize.Z) % (this.chunksize.Z * 3) / this.chunksize.Z;
      int index = num1 + num3 * 3 + num2 * 9;
      VoxelMeshBuilder.RleCache chunkCach = this.chunkCaches[index];
      if (chunkCach.Chunk == null)
      {
        this.BuildCache(this.map.GetChunk(p), ref chunkCach);
        this.chunkCaches[index] = chunkCach;
      }
      return chunkCach;
    }

    private void BuildCache(MapChunk chunk, ref VoxelMeshBuilder.RleCache cache)
    {
      cache.Chunk = chunk;
      chunk.BlockData.GetCacheAndAddRefCount(chunk, out cache.BlockCacheID, out cache.BlockCacheIndex);
      cache.BlockCache = this.map.ChunkCacheManager.Cache[(int) cache.BlockCacheID];
      chunk.LightData.GetCacheAndAddRefCount(chunk, out cache.LightCacheID, out cache.LightCacheIndex);
      cache.LightCache = this.map.ChunkCacheManager.Cache[(int) cache.LightCacheID];
      chunk.AuxData.GetCacheAndAddRefCount(chunk, out cache.AuxCacheID, out cache.AuxCacheIndex);
      cache.AuxCache = this.map.ChunkCacheManager.Cache[(int) cache.AuxCacheID];
    }

    private MapBlock GetDataBlock(ref GlobalPoint3D p)
    {
      MapBlock mapBlock = new MapBlock();
      if (p.X >= this.mapBound.Min.X && p.X < this.mapBound.Max.X && (p.Y >= this.mapBound.Min.Y && p.Y < this.mapBound.Max.Y) && (p.Z >= this.mapBound.Min.Z && p.Z < this.mapBound.Max.Z))
      {
        VoxelMeshBuilder.RleCache rleCache = this.GetRleCache(ref p);
        int mapIndex = rleCache.Chunk.GetMapIndex(p);
        mapBlock.BlockID = rleCache.BlockCache[rleCache.BlockCacheIndex + mapIndex];
        mapBlock.AuxData = rleCache.AuxCache[rleCache.AuxCacheIndex + mapIndex];
        mapBlock.Light = MapLight.FromByte(rleCache.LightCache[rleCache.LightCacheIndex + mapIndex]);
      }
      else
      {
        mapBlock.BlockID = this.map.OutOfBoundsBlockID;
        mapBlock.Light = this.map.SunLight;
      }
      return mapBlock;
    }

    private MapBlock GetDataBlock(GlobalPoint3D p)
    {
        MapBlock mapBlock = new MapBlock();
        if (p.X >= this.mapBound.Min.X && p.X < this.mapBound.Max.X && (p.Y >= this.mapBound.Min.Y && p.Y < this.mapBound.Max.Y) && (p.Z >= this.mapBound.Min.Z && p.Z < this.mapBound.Max.Z))
        {
            VoxelMeshBuilder.RleCache rleCache = this.GetRleCache(ref p);
            int mapIndex = rleCache.Chunk.GetMapIndex(p);
            mapBlock.BlockID = rleCache.BlockCache[rleCache.BlockCacheIndex + mapIndex];
            mapBlock.AuxData = rleCache.AuxCache[rleCache.AuxCacheIndex + mapIndex];
            mapBlock.Light = MapLight.FromByte(rleCache.LightCache[rleCache.LightCacheIndex + mapIndex]);
        }
        else
        {
            mapBlock.BlockID = this.map.OutOfBoundsBlockID;
            mapBlock.Light = this.map.SunLight;
        }
        return mapBlock;
    }

        private void GetDataLightCore(ref GlobalPoint3D p, out byte sunLight, out byte blockLight)
    {
      if (p.X >= this.mapBound.Min.X && p.X < this.mapBound.Max.X && (p.Y >= this.mapBound.Min.Y && p.Y < this.mapBound.Max.Y) && (p.Z >= this.mapBound.Min.Z && p.Z < this.mapBound.Max.Z))
      {
        VoxelMeshBuilder.RleCache rleCache = this.GetRleCache(ref p);
        int mapIndex = rleCache.Chunk.GetMapIndex(p);
        byte num = rleCache.LightCache[rleCache.LightCacheIndex + mapIndex];
        sunLight = (byte) ((uint) num >> 4);
        blockLight = (byte) ((uint) num & 15U);
      }
      else
      {
        sunLight = this.map.SunLight.SunLight;
        blockLight = this.map.SunLight.BlockLight;
      }
    }

    private struct MeshVertexData
    {
      public CustomArray<VertexMapBlock> VertexData;
      public CustomArray<VertexMapBlock> WaterVertexData;
    }

    private struct FaceData
    {
      public Vector3 Pos1;
      public Vector3 Pos2;
      public int Face;
    }

    private struct RleCache
    {
      public MapChunk Chunk;
      public byte[] BlockCache;
      public short BlockCacheID;
      public int BlockCacheIndex;
      public byte[] LightCache;
      public short LightCacheID;
      public int LightCacheIndex;
      public byte[] AuxCache;
      public short AuxCacheID;
      public int AuxCacheIndex;
    }

    private class CustomVertexArray<T> : CustomArray<T>
    {
      public CustomVertexArray()
        : base(80000, 1.2f)
      {
      }

      protected override void Expand()
      {
            System.Array.Resize(ref Array, (int)(Count * (double)resizeFactor));
      }
    }

    private struct CylinderCapData
    {
      public Vector3 Position;
      public Vector2 TexCoord;
    }

    private delegate Vector2 lightHelper(ref AVParams data);

    private delegate void CaseStatementToArrayLookupSetFacePos1(ref AVParams data);

    private delegate void CaseStatementToArrayLookupSetFacePos2(
      ref GlobalPoint3D p,
      ref VoxelMeshBuilder.FaceData data);
  }
}
