// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Renderers.MapRenderer
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Renderers
{
  internal class MapRenderer : DrawableGameObjectBase
  {
    public object ReloadChunksInViewSemaphore = new object();
    private BoundingFrustum frustumForDraw = new BoundingFrustum(Matrix.Identity);
    private PcgRandom rand = new PcgRandom(new Random().Next());
    private GameTime gameTime = new GameTime();
    private BoundingFrustum playerFrustum = new BoundingFrustum(Matrix.Identity);
    private BoundingFrustum debugFrustum = new BoundingFrustum(Matrix.Identity);
    private VertexBufferBinding[] bindings = new VertexBufferBinding[2];
    private string techniqueMapShader = "MapShader";
    private string techniqueMapShaderMipmaps = "MapShaderMipMaps";
    private string techniqueMapShaderTexMove = "MapShaderTexMovement";
    private string techniqueMapShaderMipmapsTexMove = "MapShaderMipMapsTexMovement";
    private float lastMapAlpha = -1f;
    private float lastItemAlpha = -1f;
    private Matrix[] signRotationOffsets = new Matrix[8]
    {
      Matrix.CreateRotationY(-1.570796f),
      Matrix.CreateRotationY(3.141593f),
      Matrix.CreateRotationY(1.570796f),
      Matrix.Identity,
      Matrix.CreateRotationY(-1.570796f),
      Matrix.CreateRotationY(3.141593f),
      Matrix.CreateRotationY(1.570796f),
      Matrix.Identity
    };
    private BoundingFrustum shadowFrustum = new BoundingFrustum(Matrix.Identity);
    private const float predatorAmuletAlpha = 0.2f;
    private const int rtMaxSizeX = 1920;
    private const int rtMaxSizeY = 1920;
    private MapTM map;
    private GameInstance instance;
    private RasterizerState rasterStateCull;
    private RasterizerState rasterStateNoCull;
    private RasterizerState rasterStateNoCullBiasPlus;
    private RasterizerState rasterStateNoCullBiasMinus;
    private RasterizerState rasterStateCullBiasPlus;
    private RasterizerState rasterStateCullBiasMinus;
    private RasterizerState rasterStateWireFrame;
    private DepthStencilState depthState;
    private DepthStencilState depthStateSunMoon;
    private DepthStencilState depthStateSkyCurtain;
    private DepthStencilState depthNoWriteState;
    private DepthStencilState itemDepthState;
    private BlendState blendState;
    private SamplerState pointClamp;
    private VertexBuffer signTextVertexBuffer;
    private GraphicsDevice graphicsDevice;
    private Vector3 windDirection;
    private Vec2Interpolator waterTextureMovement;
    private InitState initState;
    private short[] splinterIndices;
    private short[] arcadeMachineIndices;
    private int splinterVertexCount;
    private int signTextVertexCount;
    private VertexPositionNormalTexture[] splinterVertices;
    private VertexPositionNormalTexture[] arcadeMachineVertices;
    private CustomArray<VertexSignText> signTextVertices;
    private Pulsator globalPulsator;
    private Matrix itemProjection;
    private Matrix blueprintFinderProjection;
    private IProgressBar loadProgressBar;
    private bool rebuildSignData;
    private bool rebuildHealthBlockData;
    private RenderTarget2D signTextRenderTarget;
    private List<MapChunkTM> waterToDraw;
    private CubePrimitive zonePrimitive;
    private BasicEffect basicEffect;
    private SpriteBatchSafe nameplateSpriteBatch;
    private SpriteBatchSafe nameplateSpriteBatchFar;
    private SpriteBatchSafe nameplateSpriteBatchPoint;
    private SpriteBatchSafe nameplateSpriteBatchFarPoint;
    private MapModel blueprintFinderModel;
    private ChunksInPlayerViewLoader playerChunkLoader;
    private OctreeLeavesInPlayerViewLoader playerChunkLoader2;
    private QuadtreeLeavesInPlayerViewLoader playerChunkLoader3;
    private bool someHitboxesToDraw;
    private bool materialSet;
    private float horizY;
    private float rayDistance;
    private bool chunksAreSortedFrontToBack;
    private bool waterChunkAdded;
    private bool mapChunkDrawStuffSet;
    private bool alphaAdjusted;
    private Vector4 oasisFogColor;
    private Vector3 lastSetTintColor;
    private Vector2 lavaTextureOffset;
    private Vector2 waterTextureOffset;
    private int fireAnimationFrame;
    private int fireAnimationFrameTimer;

    public long BufferSize
    {
      get
      {
        return this.blueprintFinderModel != null ? this.blueprintFinderModel.BufferSize : 0L;
      }
    }

    public void SignsChanged(bool textCacheChanged)
    {
      this.rebuildSignData = true;
      if (!textCacheChanged)
        return;
      this.map.SignTextCacheChanged = true;
    }

    public void HealthBlockChanged()
    {
      this.rebuildHealthBlockData = true;
    }

    public MapRenderer(GameInstance instance, IProgressBar loadProgressBar)
    {
      this.instance = instance;
      this.map = instance.Map;
      this.loadProgressBar = loadProgressBar;
      this.waterToDraw = new List<MapChunkTM>(20);
      this.signTextVertices = new CustomArray<VertexSignText>(48, 2f);
    }

    protected override void LoadContentCore(InitState state)
    {
      this.initState = state;
      this.graphicsDevice = CoreGlobals.GraphicsDevice;
      GraphicStatics.CustomSkyColor.Reset(Vector4.Zero);
      GraphicStatics.CustomTintColor.Reset(Vector3.One);
      this.basicEffect = new BasicEffect(this.graphicsDevice);
      this.basicEffect.VertexColorEnabled = true;
      this.basicEffect.FogEnabled = true;
      this.basicEffect.PreferPerPixelLighting = false;
      this.basicEffect.TextureEnabled = false;
      PcgRandom pcgRandom = new PcgRandom(this.map.Seed);
      this.oasisFogColor = pcgRandom.Next(3) != 0 ? new Vector4((float) ((91.0 - pcgRandom.NextDouble() * 3.0) / (double) byte.MaxValue), (float) ((130.0 - pcgRandom.NextDouble() * 3.0) / (double) byte.MaxValue), (float) ((75.0 - pcgRandom.NextDouble() * 3.0) / (double) byte.MaxValue), 1f) : new Vector4((float) ((224.0 - pcgRandom.NextDouble() * 3.0) / (double) byte.MaxValue), (float) ((222.0 - pcgRandom.NextDouble() * 3.0) / (double) byte.MaxValue), (float) ((103.0 - pcgRandom.NextDouble() * 3.0) / (double) byte.MaxValue), 1f);
      this.nameplateSpriteBatch = new SpriteBatchSafe(this.graphicsDevice);
      this.nameplateSpriteBatchFar = new SpriteBatchSafe(this.graphicsDevice);
      this.nameplateSpriteBatchPoint = new SpriteBatchSafe(this.graphicsDevice);
      this.nameplateSpriteBatchFarPoint = new SpriteBatchSafe(this.graphicsDevice);
      this.windDirection = Vector3.Right + Vector3.Backward;
      this.waterTextureMovement = new Vec2Interpolator();
      this.InitDeviceStates();
      this.InitSplintering();
      this.globalPulsator = new Pulsator();
      this.globalPulsator.Start(0.4f, 1f, 1f, true);
      this.blueprintFinderModel = this.instance.SystemVoxelModelManager.LoadComponent("System", "Objects_BPFinder", true);
      this.itemProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), CoreGlobals.GraphicsDevice.Viewport.AspectRatio, 0.1f, 310f);
      this.blueprintFinderProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), CoreGlobals.GraphicsDevice.Viewport.AspectRatio, 0.01f, 20f);
      this.zonePrimitive = new CubePrimitive();
      this.playerChunkLoader = new ChunksInPlayerViewLoader();
      this.playerChunkLoader.Initialize(this.instance, (Map) this.map);
      this.playerChunkLoader2 = new OctreeLeavesInPlayerViewLoader();
      this.playerChunkLoader2.Initialize(this.instance, (Map) this.map);
      this.playerChunkLoader3 = new QuadtreeLeavesInPlayerViewLoader();
      this.playerChunkLoader3.Initialize(this.instance, (Map) this.map);
      this.InitArcadeMachinePrimitive();
      this.SignsChanged(true);
      int num = Globals2.GameSettings.ShadowMaps ? 1 : 0;
    }

    protected override void UnloadContentCore()
    {
      if (this.rasterStateNoCull != null)
      {
        this.rasterStateNoCull.Dispose();
        this.rasterStateCull.Dispose();
        this.depthState.Dispose();
        this.depthNoWriteState.Dispose();
        this.depthStateSkyCurtain.Dispose();
        this.itemDepthState.Dispose();
        this.rasterStateCullBiasPlus.Dispose();
        this.pointClamp.Dispose();
        if (this.signTextRenderTarget != null)
          this.signTextRenderTarget.Dispose();
      }
      base.UnloadContentCore();
    }

    public void GraphicsDeviceSettingsChanged()
    {
      this.InitDeviceStates();
    }

    public void GameSettingsChanged(GameSettings oldSettings)
    {
    }

    private void InitDeviceStates()
    {
      this.pointClamp = new SamplerState()
      {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = TextureFilter.Point,
        MaxAnisotropy = 0,
        MaxMipLevel = 0
      };
      this.graphicsDevice.SamplerStates[1] = this.pointClamp;
      this.graphicsDevice.SamplerStates[2] = this.pointClamp;
      this.graphicsDevice.SamplerStates[3] = this.pointClamp;
      this.graphicsDevice.SamplerStates[4] = this.pointClamp;
      this.rasterStateCull = new RasterizerState()
      {
        CullMode = CullMode.CullCounterClockwiseFace,
        DepthBias = 0.0f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = Globals2.MultiSampling,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.rasterStateCullBiasPlus = new RasterizerState()
      {
        CullMode = CullMode.CullCounterClockwiseFace,
        DepthBias = -2E-05f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = Globals2.MultiSampling,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.rasterStateCullBiasMinus = new RasterizerState()
      {
        CullMode = CullMode.CullCounterClockwiseFace,
        DepthBias = 0.0002f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = Globals2.MultiSampling,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.rasterStateNoCull = new RasterizerState()
      {
        CullMode = CullMode.None,
        DepthBias = 0.0f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = Globals2.MultiSampling,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.rasterStateWireFrame = new RasterizerState()
      {
        CullMode = CullMode.None,
        DepthBias = 0.0f,
        FillMode = FillMode.WireFrame,
        MultiSampleAntiAlias = false,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.rasterStateNoCullBiasPlus = new RasterizerState()
      {
        CullMode = CullMode.None,
        DepthBias = -2E-05f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = Globals2.MultiSampling,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.rasterStateNoCullBiasMinus = new RasterizerState()
      {
        CullMode = CullMode.None,
        DepthBias = 0.0002f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = Globals2.MultiSampling,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
      this.depthState = new DepthStencilState()
      {
        DepthBufferEnable = true,
        DepthBufferWriteEnable = true,
        StencilEnable = true,
        StencilFunction = CompareFunction.NotEqual,
        StencilPass = StencilOperation.Keep,
        ReferenceStencil = 1
      };
      this.depthNoWriteState = new DepthStencilState()
      {
        DepthBufferEnable = false,
        DepthBufferWriteEnable = false,
        StencilEnable = true,
        StencilFunction = CompareFunction.NotEqual,
        StencilPass = StencilOperation.Keep,
        ReferenceStencil = 1
      };
      this.depthStateSunMoon = this.depthStateSkyCurtain = new DepthStencilState()
      {
        DepthBufferEnable = false,
        DepthBufferWriteEnable = false,
        StencilEnable = true,
        StencilFunction = CompareFunction.NotEqual,
        StencilPass = StencilOperation.Keep,
        ReferenceStencil = 1
      };
      this.itemDepthState = new DepthStencilState()
      {
        DepthBufferEnable = true,
        DepthBufferWriteEnable = true,
        StencilEnable = true,
        StencilFunction = CompareFunction.Always,
        StencilPass = StencilOperation.Replace,
        ReferenceStencil = 1
      };
      this.blendState = BlendState.AlphaBlend;
    }

    private void InitIndices(int[] indices)
    {
      int num = 0;
      for (int index = 0; index < indices.Length; index += 6)
      {
        indices[index] = num;
        indices[index + 1] = num + 1;
        indices[index + 2] = num + 2;
        indices[index + 3] = num;
        indices[index + 4] = num + 2;
        indices[index + 5] = num + 3;
        num += 4;
      }
    }

    private void InitSplintering()
    {
      this.splinterVertices = new VertexPositionNormalTexture[576];
      this.splinterIndices = new short[864];
      int num1 = 0;
      for (int index1 = 0; index1 < 24; ++index1)
      {
        for (int index2 = 0; index2 < 6; ++index2)
        {
          short[] splinterIndices1 = this.splinterIndices;
          int index3 = num1;
          int num2 = index3 + 1;
          int num3 = (int) (short) (index2 * 4 + index1 * 36);
          splinterIndices1[index3] = (short) num3;
          short[] splinterIndices2 = this.splinterIndices;
          int index4 = num2;
          int num4 = index4 + 1;
          int num5 = (int) (short) (index2 * 4 + 1 + index1 * 36);
          splinterIndices2[index4] = (short) num5;
          short[] splinterIndices3 = this.splinterIndices;
          int index5 = num4;
          int num6 = index5 + 1;
          int num7 = (int) (short) (index2 * 4 + 2 + index1 * 36);
          splinterIndices3[index5] = (short) num7;
          short[] splinterIndices4 = this.splinterIndices;
          int index6 = num6;
          int num8 = index6 + 1;
          int num9 = (int) (short) (index2 * 4 + index1 * 36);
          splinterIndices4[index6] = (short) num9;
          short[] splinterIndices5 = this.splinterIndices;
          int index7 = num8;
          int num10 = index7 + 1;
          int num11 = (int) (short) (index2 * 4 + 2 + index1 * 36);
          splinterIndices5[index7] = (short) num11;
          short[] splinterIndices6 = this.splinterIndices;
          int index8 = num10;
          num1 = index8 + 1;
          int num12 = (int) (short) (index2 * 4 + 3 + index1 * 36);
          splinterIndices6[index8] = (short) num12;
        }
      }
    }

    private void InitArcadeMachinePrimitive()
    {
      this.arcadeMachineIndices = new short[6]
      {
        (short) 0,
        (short) 1,
        (short) 2,
        (short) 0,
        (short) 2,
        (short) 3
      };
      this.arcadeMachineVertices = new VertexPositionNormalTexture[4];
    }

    private void CopyAndOffsetCloudVertices(
      CloudMap map,
      CustomArray<VertexPositionNormalTexture> vertices)
    {
        float num = map.TileSize / 2f;
        for (int i = 0; i < vertices.Array.Length; i++)
        {
            vertices.Array[i].Position.Y /= num;
        }
    }

    private void BuildSignData()
    {
      if (!this.rebuildSignData)
        return;
      List<SignBlock> signBlocks = this.instance.MapStrategyTM.SignBlocks;
      int signTextCacheCount = this.map.SignTextCacheCount;
      if (this.map.SignTextCacheChanged)
      {
        this.BuildSignTextRenderTarget(signBlocks, signTextCacheCount);
        this.map.SignTextCacheChanged = false;
      }
      if (signTextCacheCount > 0)
      {
        this.BuildSignTextVertices(signBlocks);
        this.BuildSignTextVertexBuffer();
      }
      this.rebuildSignData = false;
    }

    private void BuildSignTextRenderTarget(List<SignBlock> signBlocks, int textCacheCount)
    {
      if (this.signTextRenderTarget != null)
      {
        this.signTextRenderTarget.Dispose();
        this.signTextRenderTarget = (RenderTarget2D) null;
      }
      if (textCacheCount > 0)
      {
        this.BuildSignTextRenderTargetCore(signBlocks, textCacheCount);
      }
      else
      {
        if (this.signTextVertexBuffer != null)
        {
          this.signTextVertexBuffer.Dispose();
          this.signTextVertexBuffer = (VertexBuffer) null;
        }
        this.signTextVertices.Clear();
        this.signTextVertexCount = 0;
      }
    }

    private void BuildSignTextRenderTargetCore(List<SignBlock> signs, int textCacheCount)
    {
      Vector2 one = Vector2.One;
      int num1 = 160;
      int num2 = 15;
      int num3 = 1920 / num1;
      int num4 = 1920 / num2;
      int num5 = num3 * num4;
      if (textCacheCount > num5)
        textCacheCount = num5;
      int num6 = textCacheCount > num3 ? num3 : textCacheCount;
      int num7 = textCacheCount > num3 ? textCacheCount / num3 + 1 : 1;
      int width = num6 * num1;
      int height = num7 * num2;
      if (this.signTextRenderTarget == null)
        this.signTextRenderTarget = new RenderTarget2D(this.graphicsDevice, width, height, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
      RenderTargetBinding[] renderTargets = this.graphicsDevice.GetRenderTargets();
      this.graphicsDevice.SetRenderTarget(this.signTextRenderTarget);
      this.graphicsDevice.Clear(Color.Transparent);
      CoreGlobals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
      Vector2 position = new Vector2(0.0f, -3f);
      int num8 = 0;
      for (int index = 0; index < this.map.SignTextCache.Count; ++index)
      {
        string text = this.map.SignTextCache[index];
        if (text != null && text.Length > 0)
        {
          float num9;
          while (true)
          {
            num9 = GraphicStatics.SignTextFont.MeasureString(text).X * one.X;
            if ((double) num9 > (double) num1)
              text = text.Substring(0, text.Length - 1);
            else
              break;
          }
          float x = position.X;
          position.X += (float) (((double) num1 - (double) num9 * (double) one.X) * 0.5 - 5.0);
          CoreGlobals.SpriteBatch.DrawString(GraphicStatics.SignTextFont, text, position, Color.Black, 0.0f, Vector2.Zero, one, SpriteEffects.None, 0.0f);
          while (this.map.SignTextCacheRTIndex.Count <= index)
            this.map.SignTextCacheRTIndex.Add((ushort) 0);
          this.map.SignTextCacheRTIndex[index] = (ushort) num8;
          ++num8;
          position.X = x;
          position.X += (float) num1;
          if ((double) position.X >= 1920.0)
          {
            position.Y += (float) num2;
            position.X = 0.0f;
          }
        }
      }
      CoreGlobals.SpriteBatch.End();
      this.graphicsDevice.SetRenderTargets(renderTargets);
      GraphicStatics.SignTextShader.Texture.SetValue((Texture) this.signTextRenderTarget);
    }

    private void BuildSignTextVertices(List<SignBlock> signBlocks)
    {
      float scale = 0.125f;
      float num1 = scale * 0.5f;
      float num2 = this.map.TileSize * 0.5f;
      Vector3[] vector3Array = new Vector3[8]
      {
        Vector3.Left * (num1 + 0.01f) + new Vector3(0.0f, 0.0f, 0.03f),
        Vector3.Forward * (num1 + 0.01f) + new Vector3(-0.03f, 0.0f, 0.0f),
        Vector3.Right * (num1 + 0.01f) + new Vector3(0.0f, 0.0f, -0.03f),
        Vector3.Backward * (num1 + 0.01f) + new Vector3(0.03f, 0.0f, 0.0f),
        new Vector3((float) ((double) num2 - (double) scale - 0.00999999977648258), 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, (float) ((double) num2 - (double) scale - 0.00999999977648258)),
        new Vector3((float) (-(double) num2 + (double) scale + 0.00999999977648258), 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, (float) (-(double) num2 + (double) scale + 0.00999999977648258))
      };
      double tileSize = (double) this.map.TileSize;
      for (int index = 0; index < signBlocks.Count; ++index)
      {
        SignBlock signBlock = signBlocks[index];
        if (this.map.GetChunk(signBlock.Point) != null)
        {
          byte auxData = this.map.GetAuxData(signBlock.Point);
          Vector3 pos1 = this.map.GetBlockCenter(signBlock.Point) + vector3Array[(int) auxData];
          pos1.Y += (float) ((double) scale - 0.0299999993294477 + (double) scale * 2.0);
          Matrix signRotationOffset = this.signRotationOffsets[(int) auxData];
          int firstSlotId = this.GetFirstSlotID(signBlock);
          if (signBlock.Text1 >= (short) 0 && (int) signBlock.Text1 < this.map.SignTextCacheRTIndex.Count)
            this.AddSignTextFace(scale, pos1, (float) firstSlotId++, this.map.SignTextCacheRTIndex[(int) signBlock.Text1], ref signRotationOffset);
          if (signBlock.Text2 >= (short) 0 && (int) signBlock.Text2 < this.map.SignTextCacheRTIndex.Count)
            this.AddSignTextFace(scale, pos1, (float) firstSlotId++, this.map.SignTextCacheRTIndex[(int) signBlock.Text2], ref signRotationOffset);
          if (signBlock.Text3 >= (short) 0 && (int) signBlock.Text3 < this.map.SignTextCacheRTIndex.Count)
            this.AddSignTextFace(scale, pos1, (float) firstSlotId++, this.map.SignTextCacheRTIndex[(int) signBlock.Text3], ref signRotationOffset);
          if (signBlock.Text4 >= (short) 0 && (int) signBlock.Text4 < this.map.SignTextCacheRTIndex.Count)
          {
            double num3 = (double) scale;
            Vector3 pos2 = pos1;
            int num4 = firstSlotId;
            int num5 = num4 + 1;
            double num6 = (double) num4;
            int num7 = (int) this.map.SignTextCacheRTIndex[(int) signBlock.Text4];
            ref Matrix local = ref signRotationOffset;
            this.AddSignTextFace((float) num3, pos2, (float) num6, (ushort) num7, ref local);
          }
        }
      }
    }

    private int GetFirstSlotID(SignBlock sign)
    {
      int num = 0;
      if (sign.Text1 >= (short) 0)
        ++num;
      if (sign.Text2 >= (short) 0)
        ++num;
      if (sign.Text3 >= (short) 0)
        ++num;
      if (sign.Text4 >= (short) 0)
        ++num;
      return num >= 3 ? 0 : 1;
    }

    private void BuildSignTextVertexBuffer()
    {
      this.signTextVertexCount = this.signTextVertices.Count;
      if (this.signTextVertexBuffer != null && this.signTextVertexBuffer.VertexCount != this.signTextVertexCount)
      {
        this.signTextVertexBuffer.Dispose();
        this.signTextVertexBuffer = (VertexBuffer) null;
      }
      if (this.signTextVertexCount <= 0)
        return;
      if (this.signTextVertexBuffer == null)
        this.signTextVertexBuffer = new VertexBuffer(this.graphicsDevice, this.signTextVertices.Array[0].VertexDeclaration, this.signTextVertexCount, BufferUsage.WriteOnly);
      this.signTextVertexBuffer.SetData<VertexSignText>(this.signTextVertices.Array, 0, this.signTextVertexCount);
      this.signTextVertices.Clear();
    }

    private void AddSignTextFace(
      float scale,
      Vector3 pos,
      float slotID,
      ushort textID,
      ref Matrix transform)
    {
      float x = scale * 3f;
      float num1 = (float) ((double) scale * 1.79999995231628 * 2.0 / 4.0);
      float num2 = 160f / (float) this.signTextRenderTarget.Width;
      float num3 = 15f / (float) this.signTextRenderTarget.Height;
      Vector2 zero = Vector2.Zero;
      float num4 = 0.0f;
      float num5;
      if (this.signTextRenderTarget.Width < 1920)
      {
        num5 = num2 * (float) textID;
      }
      else
      {
        num5 = num2 * (float) ((int) textID % 12);
        num4 = (float) ((int) textID / 12) * num3;
      }
      VertexSignText t = new VertexSignText();
      pos.Y -= num1 * slotID;
      float num6 = num1 - 0.01f;
      Vector3 position = new Vector3(-x, -num6, 0.0f);
      t.Position = pos + Vector3.Transform(position, transform);
      zero.X = num5;
      zero.Y = num4 + num3;
      t.TexCoord = new NormalizedShort2(zero.X, zero.Y);
      this.signTextVertices.Add(t);
      position = new Vector3(-x, 0.0f, 0.0f);
      t.Position = pos + Vector3.Transform(position, transform);
      zero.Y = num4;
      t.TexCoord = new NormalizedShort2(zero.X, zero.Y);
      this.signTextVertices.Add(t);
      position = new Vector3(x, 0.0f, 0.0f);
      t.Position = pos + Vector3.Transform(position, transform);
      zero.X = num5 + num2;
      t.TexCoord = new NormalizedShort2(zero.X, zero.Y);
      this.signTextVertices.Add(t);
      position = new Vector3(x, -num6, 0.0f);
      t.Position = pos + Vector3.Transform(position, transform);
      zero.Y = num4 + num3;
      t.TexCoord = new NormalizedShort2(zero.X, zero.Y);
      this.signTextVertices.Add(t);
    }

    private void BuildArcadeMachinePrimitive(GlobalPoint3D p, BlockFace face)
    {
      BoundingBox blockBox = this.instance.GetBlockBox(p);
      VertexPositionNormalTexture positionNormalTexture = new VertexPositionNormalTexture();
      positionNormalTexture.Normal = Vector3.Left;
      float num1 = 3f / 64f;
      float num2 = 3f / 500f;
      switch (face)
      {
        case BlockFace.Left:
          blockBox.Min.Y += num1;
          blockBox.Min.Z += num1;
          blockBox.Max.Y -= num1;
          blockBox.Max.Z -= num1;
          blockBox.Min.X -= num2;
          blockBox.Max.X -= num2;
          positionNormalTexture.Position.X = blockBox.Min.X;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.Position.Z = blockBox.Min.Z;
          positionNormalTexture.TextureCoordinate.X = 0.0f;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[0] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Max.Y;
          positionNormalTexture.TextureCoordinate.Y = 0.0f;
          this.arcadeMachineVertices[1] = positionNormalTexture;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.X = 1f;
          this.arcadeMachineVertices[2] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[3] = positionNormalTexture;
          break;
        case BlockFace.Forward:
          blockBox.Min.X += num1;
          blockBox.Min.Y += num1;
          blockBox.Max.X -= num1;
          blockBox.Max.Y -= num1;
          blockBox.Min.Z -= num2;
          blockBox.Max.Z -= num2;
          positionNormalTexture.Position.X = blockBox.Max.X;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.Position.Z = blockBox.Min.Z;
          positionNormalTexture.TextureCoordinate.X = 0.0f;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[0] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Max.Y;
          positionNormalTexture.TextureCoordinate.Y = 0.0f;
          this.arcadeMachineVertices[1] = positionNormalTexture;
          positionNormalTexture.Position.X = blockBox.Min.X;
          positionNormalTexture.TextureCoordinate.X = 1f;
          this.arcadeMachineVertices[2] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[3] = positionNormalTexture;
          break;
        case BlockFace.Right:
          blockBox.Min.Y += num1;
          blockBox.Min.Z += num1;
          blockBox.Max.Y -= num1;
          blockBox.Max.Z -= num1;
          blockBox.Min.X += num2;
          blockBox.Max.X += num2;
          positionNormalTexture.Position.X = blockBox.Max.X;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.X = 0.0f;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[0] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Max.Y;
          positionNormalTexture.TextureCoordinate.Y = 0.0f;
          this.arcadeMachineVertices[1] = positionNormalTexture;
          positionNormalTexture.Position.Z = blockBox.Min.Z;
          positionNormalTexture.TextureCoordinate.X = 1f;
          this.arcadeMachineVertices[2] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[3] = positionNormalTexture;
          break;
        case BlockFace.Backward:
          blockBox.Min.X += num1;
          blockBox.Min.Y += num1;
          blockBox.Max.X -= num1;
          blockBox.Max.Y -= num1;
          blockBox.Min.Z += num2;
          blockBox.Max.Z += num2;
          positionNormalTexture.Position.X = blockBox.Min.X;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.X = 0.0f;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[0] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Max.Y;
          positionNormalTexture.TextureCoordinate.Y = 0.0f;
          this.arcadeMachineVertices[1] = positionNormalTexture;
          positionNormalTexture.Position.X = blockBox.Max.X;
          positionNormalTexture.TextureCoordinate.X = 1f;
          this.arcadeMachineVertices[2] = positionNormalTexture;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[3] = positionNormalTexture;
          break;
        case BlockFace.Up:
          blockBox.Min.X += num1;
          blockBox.Min.Z += num1;
          blockBox.Max.X -= num1;
          blockBox.Max.Z -= num1;
          blockBox.Min.Y += num2;
          blockBox.Max.Y += num2;
          positionNormalTexture.Position.X = blockBox.Min.X;
          positionNormalTexture.Position.Y = blockBox.Max.Y;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.X = 0.0f;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[0] = positionNormalTexture;
          positionNormalTexture.Position.Z = blockBox.Min.Z;
          positionNormalTexture.TextureCoordinate.Y = 0.0f;
          this.arcadeMachineVertices[1] = positionNormalTexture;
          positionNormalTexture.Position.X = blockBox.Max.X;
          positionNormalTexture.TextureCoordinate.X = 1f;
          this.arcadeMachineVertices[2] = positionNormalTexture;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[3] = positionNormalTexture;
          break;
        default:
          blockBox.Min.X += num1;
          blockBox.Min.Z += num1;
          blockBox.Max.X -= num1;
          blockBox.Max.Z -= num1;
          blockBox.Min.Y -= num2;
          blockBox.Max.Y -= num2;
          positionNormalTexture.Position.X = blockBox.Max.X;
          positionNormalTexture.Position.Y = blockBox.Min.Y;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.X = 0.0f;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[0] = positionNormalTexture;
          positionNormalTexture.Position.Z = blockBox.Min.Z;
          positionNormalTexture.TextureCoordinate.Y = 0.0f;
          this.arcadeMachineVertices[1] = positionNormalTexture;
          positionNormalTexture.Position.X = blockBox.Min.X;
          positionNormalTexture.TextureCoordinate.X = 1f;
          this.arcadeMachineVertices[2] = positionNormalTexture;
          positionNormalTexture.Position.Z = blockBox.Max.Z;
          positionNormalTexture.TextureCoordinate.Y = 1f;
          this.arcadeMachineVertices[3] = positionNormalTexture;
          break;
      }
    }

    private void BuildHealthBlockData()
    {
      if (!this.rebuildHealthBlockData)
        return;
      this.rebuildHealthBlockData = false;
    }

    private void AddHealthBlockVertices(HealthBlock block)
    {
    }

    protected override void UpdateCore(UpdateState state)
    {
      if (this.instance.IsAvatarDesigner)
        return;
      this.map.LightCycle = this.instance.SunMoon.GlobalLight;
      if (Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock == Item.SpaceWorld)
        this.map.LightCycle = 1f;
      this.globalPulsator.Update();
      if (++this.fireAnimationFrameTimer > 3)
      {
        this.fireAnimationFrameTimer = 0;
        if (++this.fireAnimationFrame > 7)
          this.fireAnimationFrame = 0;
      }
      float num = (float) ((double) GraphicStatics.TexturePack.BlockTextureSize() / (double) GraphicStatics.TexturePack.BlockTexture.Height * 0.5);
      this.lavaTextureOffset.X -= this.map.LavaFlowSpeedX * 0.1f;
      if ((double) this.lavaTextureOffset.X < -(double) num)
        this.lavaTextureOffset.X = num;
      this.lavaTextureOffset.Y -= this.map.LavaFlowSpeedY * 0.1f;
      if ((double) this.lavaTextureOffset.Y < -(double) num)
        this.lavaTextureOffset.Y = num;
      this.waterTextureOffset.X -= this.map.WaterFlowSpeedX * 0.1f;
      if ((double) this.waterTextureOffset.X < -(double) num)
        this.waterTextureOffset.X = num;
      this.waterTextureOffset.Y -= this.map.WaterFlowSpeedY * 0.1f;
      if ((double) this.waterTextureOffset.Y < -(double) num)
        this.waterTextureOffset.Y = num;
      if (GraphicStatics.CustomSkyColor.IsActive)
        GraphicStatics.CustomSkyColor.Update();
      if (!GraphicStatics.CustomTintColor.IsActive)
        return;
      GraphicStatics.CustomTintColor.Update();
    }

    protected override void DrawCore(DrawState state)
    {
    }

    public void Draw(Player player, Player virtualPlayer)
    {
      this.BuildSignData();
      this.BuildHealthBlockData();
      this.playerFrustum.Matrix = virtualPlayer.ViewMatrix * player.ProjectionMatrix;
      this.SetShaderParams(player, virtualPlayer);
      this.lastMapAlpha = -1f;
      this.lastItemAlpha = -1f;
      bool isAvatarDesigner = this.instance.IsAvatarDesigner;
      this.graphicsDevice.BlendState = this.blendState;
      this.DrawPlayerItemsInHand(player, virtualPlayer, true);
      this.graphicsDevice.DepthStencilState = this.depthStateSkyCurtain;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateNoCullBiasMinus;
      this.DrawSkyCurtain(player, virtualPlayer);
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateNoCull;
      if (!isAvatarDesigner)
        this.DrawStarfield(player, virtualPlayer);
      this.graphicsDevice.DepthStencilState = this.depthStateSunMoon;
      if (!isAvatarDesigner)
        this.DrawSunAndMoon(player, virtualPlayer);
      this.graphicsDevice.DepthStencilState = this.depthState;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
      this.DrawMapChunks(player, virtualPlayer);
      this.graphicsDevice.BlendState = BlendState.Opaque;
      this.graphicsDevice.RasterizerState = this.rasterStateCullBiasPlus;
      if (!isAvatarDesigner)
        this.DrawSignText(player, virtualPlayer);
      if (!isAvatarDesigner)
        this.DrawArcadeGames(player, virtualPlayer);
      this.graphicsDevice.BlendState = this.blendState;
      this.DrawSplintering(player, virtualPlayer);
      this.DrawParticles(player, virtualPlayer);
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
      this.DrawAvatars(player, virtualPlayer);
      this.DrawEntities(player, virtualPlayer);
      this.DrawPlayersItemsInHand(player, virtualPlayer);
      this.DrawPlayerClipboard(player, virtualPlayer);
      this.DrawSwingTargetFrame(player, virtualPlayer);
      this.DrawNamePlates(player, virtualPlayer);
      if (!isAvatarDesigner)
      {
        this.DrawCloudsAndWater(player, virtualPlayer);
        this.DrawZones(player, virtualPlayer);
        this.DrawSounds(player, virtualPlayer);
      }
      this.DrawPlayerItemsInHand(player, virtualPlayer, false);
      this.DrawScriptTools(player);
      if (!this.someHitboxesToDraw)
        return;
      this.DrawNpcHitBoxes(player, virtualPlayer);
    }

    private void DrawAvatars(Player player, Player virtualPlayer)
    {
      this.DrawNpcs(player, virtualPlayer);
      this.DrawPlayerAvatars(player, virtualPlayer);
    }

    private void DrawPlayerItemsInHand(Player player, Player virtualPlayer, bool first)
    {
      if (!virtualPlayer.ShowItemsInHand || virtualPlayer.IsAssemblingPhoto)
        return;
      bool flag = virtualPlayer.IsItemEquippedAndUsable(Item.PredatorAmulet);
      if ((!first || flag) && (first || !flag))
        return;
      this.graphicsDevice.DepthStencilState = this.itemDepthState;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
      GraphicStatics.ItemInHandShader.CameraPosition.SetValue(virtualPlayer.EyeOffset);
      GraphicStatics.ItemInHandShader.View.SetValue(virtualPlayer.ViewMatrixLocal);
      GraphicStatics.ItemInHandShader.Projection.SetValue(this.itemProjection);
      float alpha = flag ? 0.4f : 1f;
      this.DrawPlayerItemsInHandCore(player, virtualPlayer, (Actor) virtualPlayer, alpha);
    }

    private void DrawPlayersItemsInHand(Player player, Player virtualPlayer)
    {
      GraphicStatics.ItemInHandShader.CameraPosition.SetValue(Vector3.Zero);
      GraphicStatics.ItemInHandShader.View.SetValue(virtualPlayer.ViewMatrixLocal);
      GraphicStatics.ItemInHandShader.Projection.SetValue(player.ProjectionMatrix);
      foreach (Gamer allGamer in this.instance.NetworkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null && tag != virtualPlayer && !tag.IsDeadOrInactiveOrDisabled)
        {
          float alpha = tag.IsItemEquippedAndUsable(Item.PredatorAmulet) ? 0.2f : 1f;
          this.DrawPlayerItemsInHandCore(player, virtualPlayer, (Actor) tag, alpha);
        }
      }
      List<NpcBase> npcList = this.instance.NpcManager.GetNpcList();
      for (int index = npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npcBase = npcList[index];
        if (npcBase != null && !npcBase.IsDeadOrInactiveOrDisabled && (!npcBase.Properties.DrawEquipedItems.HasValue || npcBase.Properties.DrawEquipedItems.Value))
          this.DrawPlayerItemsInHandCore(player, virtualPlayer, (Actor) npcBase, 1f);
      }
    }

    private void DrawPlayerItemsInHandCore(
      Player player,
      Player virtualPlayer,
      Actor avatar,
      float alpha)
    {
      bool flag1 = avatar.LeftHand != null && avatar.LeftHand.CanDraw;
      bool flag2 = avatar.RightHand != null && avatar.RightHand.CanDraw;
      if (!flag1 && !flag2)
        return;
      GlobalPoint3D point = this.map.GetPoint(avatar.EyePosition);
      Vector2 blockLightNormalized = this.map.GetSunAndBlockLightNormalized(point);
      GraphicStatics.ItemInHandShader.Sunlight.SetValue(blockLightNormalized.X * this.map.LightCycle);
      if ((double) alpha != (double) this.lastItemAlpha)
      {
        GraphicStatics.ItemInHandShader.Alpha.SetValue(alpha);
        this.lastItemAlpha = alpha;
      }
      if (flag1)
        this.DrawItemModel(player, virtualPlayer, avatar, avatar.LeftHand, blockLightNormalized.Y, ref point);
      if (!flag2)
        return;
      this.DrawItemModel(player, virtualPlayer, avatar, avatar.RightHand, blockLightNormalized.Y, ref point);
    }

    private void DrawItemModel(
      Player player,
      Player virtualPlayer,
      Actor avatar,
      Hand hand,
      float light,
      ref GlobalPoint3D p)
    {
      ItemModel itemModel = hand.ItemModel;
      if (itemModel.PrimitiveCount < 1)
        return;
      Item itemId = itemModel.ItemID;
      float scale1 = itemModel.Scale;
      float num1 = 0.0f;
      bool flag1 = avatar == virtualPlayer;
      if (!flag1 && itemId == Item.Hand)
        return;
      Hand hand1 = hand;
      Vector3 vector3 = flag1 ? hand1.ItemSwing.AnimDataFPV.CurrYawPitchRoll : hand1.ItemSwing.AnimData.CurrYawPitchRoll;
      Vector3 position1 = flag1 ? hand1.ItemSwing.AnimDataFPV.CurrPosition : hand1.ItemSwing.AnimData.CurrPosition;
      bool flag2 = ItemModelManager.UseCube(itemId);
      ItemModelDataXML itemModelDataXml = Globals1.ItemModelData[(int) Globals1.ItemTypeData[(int) itemId].Model];
      float scale2;
      if (flag1)
      {
        scale2 = scale1 * itemModelDataXml.HUDScale;
        position1.X += itemModelDataXml.HUDOffset.X;
        position1.Y += itemModelDataXml.HUDOffset.Y;
        position1.Z += itemModelDataXml.HUDOffset.Z;
      }
      else
      {
        scale2 = scale1 * itemModelDataXml.Scale;
        ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) avatar.ActorType];
        num1 = actorTypeDataXml.ModelHeight / avatar.Size.Y * actorTypeDataXml.ModelHeight;
        position1.X += actorTypeDataXml.ItemModelOffset.X + itemModelDataXml.HandOffset.X;
        position1.Y += actorTypeDataXml.ItemModelOffset.Y + itemModelDataXml.HandOffset.Y;
        position1.Z += actorTypeDataXml.ItemModelOffset.Z + itemModelDataXml.HandOffset.Z;
        vector3.X += itemModelDataXml.HandYPR.X;
        vector3.Y += itemModelDataXml.HandYPR.Y;
        vector3.Z += itemModelDataXml.HandYPR.Z;
      }
      if (hand.HandType == InventoryHand.Left)
      {
        position1.X = -position1.X;
        vector3.X = -vector3.X;
        vector3.Z = -vector3.Z;
      }
      int num2 = (int) itemId;
      float num3 = Math.Max(light, itemId < Item.zLastBlockID ? (float) this.map.GetLuminance(ref p, (byte) num2) / this.map.MaxLight : (float) ItemData.GetParticleLight(itemId) / this.map.MaxLight);
      GraphicStatics.ItemInHandShader.Blocklight.SetValue(num3);
      Matrix matrix1 = Matrix.CreateTranslation(-itemModel.Center.X, 0.0f, -itemModel.Center.Z) * Matrix.CreateRotationY(-1.570796f) * Matrix.CreateScale(scale2) * Matrix.CreateFromYawPitchRoll(vector3.X, vector3.Y, vector3.Z) * Matrix.CreateTranslation(position1);
      if (virtualPlayer.IsWorldShaking)
      {
        Matrix worldShake = virtualPlayer.WorldShake;
        worldShake.M41 *= 0.2f;
        worldShake.M42 *= 0.2f;
        worldShake.M43 *= 0.2f;
        matrix1 = worldShake * matrix1;
      }
      Matrix matrix2;
      if (avatar != virtualPlayer)
      {
        if (avatar.IsCrouching)
          return;
        Vector3 position2 = avatar.Position;
        position2.Y += num1 * 0.5f;
        Vector3 cameraPosition = position2 - virtualPlayer.Position;
        Matrix lookAt = Matrix.CreateLookAt(cameraPosition, cameraPosition + avatar.ViewDirNoYNormalized, Vector3.Up);
        matrix2 = matrix1 * Matrix.Invert(lookAt);
      }
      else
        matrix2 = matrix1 * Matrix.Invert(avatar.ViewMatrixLocal);
      this.SetIndices(itemModel.PrimitiveCount);
      this.graphicsDevice.SetVertexBuffer(itemModel.VertexBuffer);
      GraphicStatics.ItemInHandShader.World.SetValue(matrix2);
      Effect effect = GraphicStatics.ItemInHandShader.Effect;
      effect.CurrentTechnique = effect.Techniques[flag2 ? "TextureShader" : "ColorShader"];
      effect.CurrentTechnique.Passes[0].Apply();
      try
      {
        this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, itemModel.VertexCount, 0, itemModel.PrimitiveCount);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(28, (Exception) ex);
      }
    }

    public void DrawBlueprintFinder(Player player, Player virtualPlayer)
    {
      MapChunkContentData chunkContentData = this.blueprintFinderModel.MapChunkContentData;
      if (chunkContentData.VertexBuffer == null || chunkContentData.VertexCount <= 0 || virtualPlayer.BlueprintFinderTarget == null)
        return;
      this.graphicsDevice.DepthStencilState = this.depthState;
      this.graphicsDevice.BlendState = BlendState.Opaque;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
      float num = -0.5f;
      Matrix translation = Matrix.CreateTranslation((float) this.blueprintFinderModel.ModelSize.X * num, (float) this.blueprintFinderModel.ModelSize.Y * num, (float) this.blueprintFinderModel.ModelSize.Z * num);
      GraphicStatics.MapShader.World.SetValue(translation * virtualPlayer.BlueprintFinderWorld);
      GraphicStatics.MapShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      GraphicStatics.MapShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * this.itemProjection);
      GraphicStatics.MapShader.Alpha.SetValue(1f);
      GraphicStatics.MapShader.LightCycle.SetValue(1f);
      GraphicStatics.MapShader.RayDistance.SetValue(0.0f);
      GraphicStatics.MapShader.FogColor.SetValue(Vector4.Zero);
      GraphicStatics.MapShader.TintColor.SetValue(Vector4.One);
      GraphicStatics.MapShader.FarClip.SetValue(50);
      GraphicStatics.MapShader.FadeStart.SetValue(49);
      this.graphicsDevice.SetVertexBuffer(chunkContentData.VertexBuffer);
      this.SetIndices(chunkContentData.VertexCount / 2);
      Effect effect = GraphicStatics.MapShader.Effect;
      effect.CurrentTechnique = effect.Techniques[this.techniqueMapShader];
      effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2);
    }

    private void DrawMapChunks(Player player, Player virtualPlayer)
    {
      this.waterToDraw.Clear();
      this.DrawMapChunksByOctreeLeaves(player, virtualPlayer, false);
    }

    private void DrawMapChunksByChunks(Player player, Player virtualPlayer, bool sortChunks)
    {
      this.playerChunkLoader.LoadChunksInView(player, virtualPlayer);
      List<MapChunk> latest = virtualPlayer.ChunksToDraw.GetLatest();
      this.chunksAreSortedFrontToBack = false;
      if (latest != null)
      {
        if (!sortChunks)
        {
          if (!virtualPlayer.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
            goto label_4;
        }
        latest.Sort(new Comparison<MapChunk>(this.playerChunkLoader.SortChunksFrontToBack));
        this.chunksAreSortedFrontToBack = true;
        this.instance.ChunkSortCount += latest.Count;
      }
label_4:
      try
      {
        this.DrawMapChunks(player, virtualPlayer, latest);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(29, ex);
      }
      finally
      {
        virtualPlayer.ChunksToDraw.Release(latest);
      }
    }

    private void DrawMapChunksByOctreeLeaves(Player player, Player virtualPlayer, bool sortLeaves)
    {
      this.playerChunkLoader2.LoadChunksInView(player, virtualPlayer, ref player.ProjectionMatrix);
      List<OctreeLeaf<MapChunk>> latest = virtualPlayer.OctreeLeavesToDraw.GetLatest();
      this.chunksAreSortedFrontToBack = false;
      if (latest != null)
      {
        if (!sortLeaves)
        {
          if (!virtualPlayer.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
            goto label_4;
        }
        latest.Sort(new Comparison<OctreeLeaf<MapChunk>>(this.playerChunkLoader2.SortLeavesFrontToBack));
        this.chunksAreSortedFrontToBack = true;
        this.instance.ChunkSortCount += latest.Count;
      }
label_4:
      try
      {
        this.DrawMapChunks(player, virtualPlayer, latest);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(29, ex);
      }
      finally
      {
        virtualPlayer.OctreeLeavesToDraw.Release(latest);
      }
    }

    private void DrawMapChunksByQuadtreeLeaves(
      Player player,
      Player virtualPlayer,
      bool sortLeaves)
    {
      this.playerChunkLoader3.LoadChunksInView(player, virtualPlayer);
      List<QuadtreeLeaf<MapChunk>> latest = virtualPlayer.QuadtreeLeavesToDraw.GetLatest();
      this.chunksAreSortedFrontToBack = false;
      if (latest != null)
      {
        if (sortLeaves)
        {
          latest.Sort(new Comparison<QuadtreeLeaf<MapChunk>>(this.playerChunkLoader3.SortLeavesFrontToBack));
          this.chunksAreSortedFrontToBack = true;
          this.instance.ChunkSortCount += latest.Count;
        }
      }
      try
      {
        this.DrawMapChunks(player, virtualPlayer, latest);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(29, ex);
      }
      finally
      {
        virtualPlayer.QuadtreeLeavesToDraw.Release(latest);
      }
    }

    private void DrawMapChunks(Player player, Player virtualPlayer, List<MapChunk> chunksToDraw)
    {
      if (chunksToDraw == null || chunksToDraw.Count <= 0)
        return;
      Effect effect = this.InitStatesForChunkDrawing(player, virtualPlayer);
      if (!virtualPlayer.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
      {
        for (int index = 0; index < chunksToDraw.Count; ++index)
        {
          MapChunk mapChunk = chunksToDraw[index];
          if (mapChunk != null)
            this.DrawMapChunk(virtualPlayer, mapChunk as MapChunkTM, effect);
        }
      }
      else
      {
        for (int index = chunksToDraw.Count - 1; index >= 0; --index)
        {
          MapChunk mapChunk = chunksToDraw[index];
          if (mapChunk != null)
            this.DrawMapChunk(virtualPlayer, mapChunk as MapChunkTM, effect);
        }
      }
    }

    private void DrawMapChunks(
      Player player,
      Player virtualPlayer,
      List<OctreeLeaf<MapChunk>> leavesToDraw)
    {
      if (leavesToDraw == null || leavesToDraw.Count <= 0)
        return;
      Effect effect = this.InitStatesForChunkDrawing(player, virtualPlayer);
      if (!virtualPlayer.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
      {
        for (int index = 0; index < leavesToDraw.Count; ++index)
          this.DrawMapChunks(player, virtualPlayer, leavesToDraw[index], effect);
      }
      else
      {
        for (int index = leavesToDraw.Count - 1; index >= 0; --index)
          this.DrawMapChunks(player, virtualPlayer, leavesToDraw[index], effect);
      }
    }

    private void DrawMapChunks(
      Player player,
      Player virtualPlayer,
      OctreeLeaf<MapChunk> leaf,
      Effect effect)
    {
      for (int index = 0; index < leaf.BoundedObjectCount; ++index)
      {
        MapChunk boundedObject = leaf.BoundedObjects[index];
        if (boundedObject != null)
          this.DrawMapChunk(virtualPlayer, boundedObject as MapChunkTM, effect);
      }
    }

    private void DrawMapChunks(
      Player player,
      Player virtualPlayer,
      List<QuadtreeLeaf<MapChunk>> leavesToDraw)
    {
      if (leavesToDraw == null || leavesToDraw.Count <= 0)
        return;
      Effect effect = this.InitStatesForChunkDrawing(player, virtualPlayer);
      for (int index1 = 0; index1 < leavesToDraw.Count; ++index1)
      {
        QuadtreeLeaf<MapChunk> quadtreeLeaf = leavesToDraw[index1];
        for (int index2 = 0; index2 < quadtreeLeaf.BoundedObjectCount; ++index2)
        {
          MapChunk boundedObject = quadtreeLeaf.BoundedObjects[index2];
          if (boundedObject != null)
            this.DrawMapChunk(virtualPlayer, boundedObject as MapChunkTM, effect);
        }
      }
    }

    private void DrawMapChunk(Player virtualPlayer, MapChunkTM chunk, Effect effect)
    {
      MapChunkContent content = chunk.Content;
      this.alphaAdjusted = false;
      this.waterChunkAdded = false;
      MapTM map = this.map;
      if (content.IsContentSplit)
      {
        long globalHashCode = chunk.GetGlobalHashCode();
        bool flag = false;
        MapChunkContentData[] dataList;
        lock (map.MapChunkContentBreakdown)
          flag = map.MapChunkContentBreakdown.TryGetValue(globalHashCode, out dataList);
        if (flag)
        {
          this.mapChunkDrawStuffSet = false;
          this.SetChunkWorldMatrixForDrawing(virtualPlayer, (MapChunk) chunk);
          for (int index = 0; index < dataList.Length; ++index)
          {
            MapChunkContentData vertexData = content.GetVertexData((MapChunk) chunk, dataList, index);
            if (vertexData.VertexBuffer != null)
            {
              this.SetMapChunkDrawStuff(virtualPlayer, chunk, ref vertexData);
              if (vertexData.VertexCount > 0)
                this.DrawIndexedPrimitives(effect, vertexData.VertexBuffer, 0, vertexData.VertexCount);
            }
          }
          return;
        }
      }
      MapChunkContentData vertexData1 = content.GetVertexData();
      if (vertexData1.VertexBuffer == null || vertexData1.VertexCount <= 0 && vertexData1.WaterVertexCount <= 0)
        return;
      this.mapChunkDrawStuffSet = false;
      this.SetMapChunkDrawStuff(virtualPlayer, chunk, ref vertexData1);
      if (vertexData1.VertexCount <= 0)
        return;
      this.SetChunkWorldMatrixForDrawing(virtualPlayer, (MapChunk) chunk);
      this.DrawIndexedPrimitives(effect, vertexData1.VertexBuffer, 0, vertexData1.VertexCount);
    }

    private void SetChunkWorldMatrixForDrawing(Player virtualPlayer, MapChunk chunk)
    {
      Vector3 vector3 = chunk.GlobalOffset.ToVector3();
      float tileSize = chunk.Region.Map.TileSize;
      if ((double) tileSize != 1.0)
      {
        vector3.X *= tileSize;
        vector3.Y *= tileSize;
        vector3.Z *= tileSize;
      }
      vector3.X -= virtualPlayer.Position.X;
      vector3.Y -= virtualPlayer.Position.Y;
      vector3.Z -= virtualPlayer.Position.Z;
      Matrix translation = Matrix.CreateTranslation(vector3);
      if (virtualPlayer.IsWorldShaking)
        translation *= virtualPlayer.WorldShake;
      GraphicStatics.MapShader.World.SetValue(translation);
    }

    private void DrawWaterBlocks(Player player, Player virtualPlayer)
    {
      if (this.waterToDraw.Count <= 0)
        return;
      if (!this.chunksAreSortedFrontToBack)
        this.waterToDraw.Sort(new Comparison<MapChunkTM>(this.playerChunkLoader.SortChunksFrontToBack));
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
      Effect effect = GraphicStatics.MapShader.Effect;
      string index1 = this.instance.IsMapActive ? (!Globals2.GameSettings.UseMipMaps || GraphicStatics.TexturePack.BlockTextureSize() <= 16 ? this.techniqueMapShaderTexMove : this.techniqueMapShaderMipmapsTexMove) : (!Globals2.GameSettings.UseMipMaps || GraphicStatics.TexturePack.BlockTextureSize() <= 16 ? this.techniqueMapShader : this.techniqueMapShaderMipmaps);
      effect.CurrentTechnique = effect.Techniques[index1];
      for (int index2 = this.waterToDraw.Count - 1; index2 >= 0; --index2)
      {
        MapChunkTM chunk = this.waterToDraw[index2];
        MapChunkContent content = chunk.Content;
        MapTM map = this.map;
        MapChunkContentData vertexData;
        if (content.IsContentSplit)
        {
          long globalHashCode = chunk.GetGlobalHashCode();
          bool flag = false;
          MapChunkContentData[] dataList;
          lock (map.MapChunkContentBreakdown)
            flag = map.MapChunkContentBreakdown.TryGetValue(globalHashCode, out dataList);
          if (flag)
          {
            this.mapChunkDrawStuffSet = false;
            this.SetChunkWorldMatrixForDrawing(virtualPlayer, (MapChunk) chunk);
            for (int index3 = 0; index3 < dataList.Length; ++index3)
            {
              vertexData = content.GetVertexData((MapChunk) chunk, dataList, index3);
              if (vertexData.VertexBuffer != null && vertexData.WaterVertexCount > 0)
              {
                this.SetMapChunkDrawStuffForWater(virtualPlayer, chunk, ref vertexData);
                this.DrawIndexedPrimitives(effect, vertexData.VertexBuffer, vertexData.VertexCount, vertexData.WaterVertexCount);
              }
            }
            continue;
          }
        }
        vertexData = content.GetVertexData();
        if (vertexData.VertexBuffer != null && vertexData.WaterVertexCount > 0)
        {
          this.mapChunkDrawStuffSet = false;
          this.SetMapChunkDrawStuffForWater(virtualPlayer, chunk, ref vertexData);
          this.SetChunkWorldMatrixForDrawing(virtualPlayer, (MapChunk) chunk);
          this.DrawIndexedPrimitives(effect, vertexData.VertexBuffer, vertexData.VertexCount, vertexData.WaterVertexCount);
        }
      }
    }

    private void DrawPlayerClipboard(Player player, Player virtualPlayer)
    {
      if (!virtualPlayer.IsClipboardEquipped)
        return;
      MapModel clipboardModel = virtualPlayer.ClipboardModel;
      Matrix modelWorldMatrix = virtualPlayer.ClipboardModelWorldMatrix;
      MapChunkContentData chunkContentData = clipboardModel.MapChunkContentData;
      GraphicStatics.MapShader.Alpha.SetValue(1);
      GraphicStatics.MapShader.World.SetValue(modelWorldMatrix);
      this.graphicsDevice.SetVertexBuffer(chunkContentData.VertexBuffer);
      Effect effect = GraphicStatics.MapShader.Effect;
      if (chunkContentData.VertexBuffer != null)
      {
        if (chunkContentData.VertexCount > 0)
        {
          this.SetIndices(chunkContentData.VertexCount / 2);
          effect.CurrentTechnique = effect.Techniques[this.techniqueMapShader];
          effect.CurrentTechnique.Passes[0].Apply();
          this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2);
        }
        if (chunkContentData.WaterVertexCount > 0)
        {
          this.SetIndices(chunkContentData.WaterVertexCount / 2);
          this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
          effect.CurrentTechnique = effect.Techniques[this.techniqueMapShader];
          effect.CurrentTechnique.Passes[0].Apply();
          this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, chunkContentData.VertexCount, 0, chunkContentData.WaterVertexCount, 0, chunkContentData.WaterVertexCount / 2);
        }
      }
      BoundingBox box = virtualPlayer.ClipboardModel.Map.Regions[0].Box;
      Vector3 vector3 = new Vector3(this.map.TileSize);
      box.Min += vector3;
      box.Max -= vector3;
      BoundingBoxRenderer.Render(box, this.graphicsDevice, modelWorldMatrix, virtualPlayer.ViewMatrixLocal, player.ProjectionMatrix, Color.Yellow, true);
    }

    private void DrawPlayerAvatars(Player player, Player virtualPlayer)
    {
      if (!this.instance.IsSplitScreen && !this.instance.IsMultiplayer && !(player.GamerID != virtualPlayer.GamerID))
        return;
      this.materialSet = false;
      foreach (Gamer allGamer in this.instance.NetworkManager.AllGamers)
      {
        Player tag = allGamer.Tag as Player;
        if (tag != null && tag != virtualPlayer)
          this.DrawPlayerAvatar(player, virtualPlayer, tag);
      }
    }

    private void DrawPlayerAvatar(Player player, Player virtualPlayer, Player avatar)
    {
      if (!avatar.IsEnabledField)
        return;
      MapModel avatarModel = avatar.AvatarModel;
      if (avatarModel == null)
        return;
      MapChunkContentData chunkContentData = avatarModel.MapChunkContentData;
      if (chunkContentData.VertexBuffer == null || chunkContentData.VertexCount <= 0)
        return;
      if (!this.materialSet)
      {
        this.materialSet = true;
        GraphicStatics.AvatarShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
        GraphicStatics.AvatarShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
        Effect effect = GraphicStatics.AvatarShader.Effect;
        effect.CurrentTechnique = effect.Techniques["AvatarShader"];
      }
      this.SetIndices(chunkContentData.VertexCount / 2);
      this.graphicsDevice.SetVertexBuffer(chunkContentData.VertexBuffer);
      Matrix world = virtualPlayer.WorldShake * avatar.AvatarWorld;
      Vector3 position = avatar.Position;
      position.Y += 0.1f;
      Vector3 eyePos = position + avatar.EyeOffset;
      NpcManager.SetLightInWorldMatrix((Map) this.map, position, eyePos, ref world, byte.MaxValue, true);
      GraphicStatics.AvatarShader.Alpha.SetValue(avatar.IsItemEquippedAndUsable(Item.PredatorAmulet) ? 0.2f : 1f);
      GraphicStatics.AvatarShader.World.SetValue(world);
      GraphicStatics.AvatarShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2);
    }

    private void DrawNamePlates(Player player, Player virtualPlayer)
    {
      if (player.Settings.Nameplates == NamePlateSetting.None && !player.Settings.MobNameplates)
        return;
      this.nameplateSpriteBatchFarPoint.Begin(SpriteSortMode.Texture, (BlendState) null, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null);
      this.nameplateSpriteBatchPoint.Begin(SpriteSortMode.Texture, (BlendState) null, SamplerState.PointClamp, this.depthState, (RasterizerState) null, (Effect) null);
      this.nameplateSpriteBatchFar.Begin(SpriteSortMode.Texture, (BlendState) null, (SamplerState) null, (DepthStencilState) null, (RasterizerState) null, (Effect) null);
      this.nameplateSpriteBatch.Begin(SpriteSortMode.Texture, (BlendState) null, (SamplerState) null, this.depthState, (RasterizerState) null, (Effect) null);
      if (player.Settings.Nameplates != NamePlateSetting.None)
      {
        List<NetworkGamer> allGamers = this.instance.NetworkManager.AllGamers;
        int playerCombatLevel = this.instance.IsSkillsEnabled ? virtualPlayer.CombatLevel : 0;
        foreach (Gamer gamer in allGamers)
        {
          Player tag = gamer.Tag as Player;
          if (tag != null && tag != virtualPlayer)
            this.DrawPlayerNameplate(player, virtualPlayer, tag, playerCombatLevel);
        }
      }
      if (this.instance.IsCombatEnabled && player.Settings.MobNameplates)
      {
        int combatLevel = virtualPlayer.CombatLevel;
        NpcManager npcManager = this.instance.NpcManager;
        if (npcManager != null)
        {
          List<NpcBase> npcList = npcManager.GetNpcList();
          for (int index = npcList.Count - 1; index >= 0; --index)
          {
            NpcBase avatar = npcList[index];
            if (avatar != null && avatar.Properties.ShowNamePlate.Value && (Globals1.NpcTypeData[(int) avatar.ActorType].HasNameplate && !avatar.IsDeadOrInactiveOrDisabled))
              this.DrawNpcNameplate(player, virtualPlayer, avatar, combatLevel);
          }
        }
      }
      this.nameplateSpriteBatchFar.End();
      this.nameplateSpriteBatchPoint.End();
      this.nameplateSpriteBatchFarPoint.End();
      this.nameplateSpriteBatch.End();
      this.graphicsDevice.SamplerStates[0] = this.pointClamp;
    }

    private void DrawPlayerNameplate(
      Player player,
      Player virtualPlayer,
      Player avatar,
      int playerCombatLevel)
    {
      if (!avatar.IsEnabledField)
        return;
      NamePlateSetting namePlateSetting = (NamePlateSetting) Math.Min((int) player.Settings.Nameplates, (int) avatar.Settings.Nameplates);
      if (namePlateSetting == NamePlateSetting.None)
        return;
      bool flag = namePlateSetting == NamePlateSetting.Far;
      float num1 = flag ? 1000000f : 1600f;
      Vector2 zero = Vector2.Zero;
      Matrix world = avatar.NameplateWorld * Matrix.CreateTranslation(0.0f, avatar.Size.Y + 0.01f, 0.0f);
      Vector3 pos = this.graphicsDevice.Viewport.Project(Vector3.Zero, player.ProjectionMatrix, virtualPlayer.ViewMatrix, world);
      SpriteBatchSafe spriteBatch = flag ? this.nameplateSpriteBatchFar : this.nameplateSpriteBatch;
      SpriteBatchSafe spriteBatchSafe = flag ? this.nameplateSpriteBatchFarPoint : this.nameplateSpriteBatchPoint;
      if ((double) pos.Z >= 1.0)
        return;
      float num2 = Vector3.DistanceSquared(virtualPlayer.Position, avatar.Position);
      if ((double) num2 >= (double) num1)
        return;
      float scale = MathHelper.Lerp(0.5f, 0.2f, num2 / num1);
      pos.Y -= 70f * scale;
      zero.X = pos.X - (float) (int) ((double) avatar.GamertagMeasure.X * 0.5 * (double) scale);
      zero.Y = pos.Y;
      Color color = avatar.IsGod || avatar.ActorType == ActorType.Zeus ? Color.LightGoldenrodYellow : (avatar.ActorType == ActorType.HermesWraith ? Color.Purple : (avatar.Gamer.IsHost ? Color.Blue : (avatar.IsAdmin ? Color.Cyan : Color.Yellow)));
      float alpha = 1f;
      if (avatar.IsItemEquippedAndUsable(Item.PredatorAmulet))
      {
        alpha = 0.2f;
        color *= alpha;
      }
      int num3 = 0;
      if (avatar.ClanBannerID > 0)
      {
        float num4 = scale * 2f;
        int num5 = (int) (16.0 * (double) num4);
        int x = (int) ((double) zero.X - (double) num5 - 4.0 * (double) num4);
        int y = (int) ((double) zero.Y + 3.0 * (double) num4);
        if (this.instance.IsSkillsEnabled)
        {
          x = (int) zero.X;
          y = (int) ((double) pos.Y - 37.0 * (double) scale + 2.0);
          num3 = (int) ((double) num5 + 4.0 * (double) num4);
        }
        Rectangle destinationRectangle = new Rectangle(x, y, num5, num5);
        Rectangle clanBannerRect = GraphicStatics.GetClanBannerRect((byte) avatar.ClanBannerID);
        spriteBatchSafe.Draw(GraphicStatics.ClanBanners, destinationRectangle, new Rectangle?(clanBannerRect), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, pos.Z);
      }
      spriteBatch.DrawString(CoreGlobals.GameFont, avatar.DisplayGamertag, zero, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, pos.Z);
      if (!this.instance.IsCombatEnabled)
        return;
      if (this.instance.IsSkillsEnabled)
        this.DrawNamePlateSkillData(spriteBatch, (Actor) avatar, new Rectangle((int) zero.X + num3, (int) ((double) pos.Y - 37.0 * (double) scale), 32, 32), pos.Z, SkillType.None, avatar.CombatLevelString, scale, this.GetCombatLevelColor(playerCombatLevel, avatar.CombatLevel), alpha);
      pos.X -= 50f * scale;
      pos.Y += avatar.GamertagMeasure.Y * scale;
      this.DrawCharacterHealthBar(spriteBatch, (Actor) avatar, pos, scale, alpha);
    }

    private void DrawNpcNameplate(
      Player player,
      Player virtualPlayer,
      NpcBase avatar,
      int playerCombatLevel)
    {
      Vector3 pos = this.graphicsDevice.Viewport.Project(new Vector3(avatar.Position.X, (float) ((double) avatar.Position.Y + (double) avatar.Size.Y + 0.100000001490116), avatar.Position.Z), player.ProjectionMatrix, virtualPlayer.ViewMatrix, Matrix.Identity);
      float num1 = 1600f;
      if ((double) pos.Z >= 1.0)
        return;
      float num2 = Vector3.DistanceSquared(virtualPlayer.Position, avatar.Position);
      if ((double) num2 >= (double) num1)
        return;
      float scale = MathHelper.Lerp(0.5f, 0.1f, num2 / num1);
      pos.X -= 50f * scale;
      this.DrawCharacterHealthBar(this.nameplateSpriteBatch, (Actor) avatar, pos, scale, avatar.Alpha);
      this.DrawNamePlateSkillData(this.nameplateSpriteBatch, (Actor) avatar, new Rectangle((int) pos.X, (int) ((double) pos.Y - 40.0 * (double) scale), 32, 32), pos.Z, SkillType.None, avatar.CombatLevelString, scale, this.GetCombatLevelColor(playerCombatLevel, avatar.CombatLevel), avatar.Alpha);
    }

    private void DrawNamePlateSkillData(
      SpriteBatchSafe spriteBatch,
      Actor avatar,
      Rectangle rect,
      float z,
      SkillType skill,
      string level,
      float scale,
      Color levelColor,
      float alpha)
    {
      rect.Width = rect.Height = (int) (40.0 * (double) scale);
      ++rect.X;
      rect.Y += 2;
      rect.Width -= 2;
      rect.Height -= 2;
      spriteBatch.Draw(GraphicStatics.TexturePack.ItemTexture, rect, new Rectangle?(GraphicStatics.TexturePack.ItemSrcRect(Item.SkillCombat)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, z);
      spriteBatch.DrawString(CoreGlobals.GameFont, level, new Vector2((float) (rect.X + rect.Width) + 8f * scale, (float) rect.Y - 4f * scale), levelColor * alpha, 0.0f, Vector2.Zero, scale, SpriteEffects.None, z);
    }

    private void DrawCharacterHealthBar(
      SpriteBatchSafe spriteBatch,
      Actor avatar,
      Vector3 pos,
      float scale,
      float alpha)
    {
      bool iceEffectActive = avatar.IceEffectActive;
      Color color1 = iceEffectActive ? Color.Blue : Color.Green;
      Color color2 = iceEffectActive ? Color.Cyan : Color.Red;
      Rectangle destinationRectangle = new Rectangle((int) pos.X, (int) pos.Y, (int) (120.0 * (double) scale), (int) (20.0 * (double) scale));
      ++destinationRectangle.X;
      ++destinationRectangle.Y;
      destinationRectangle.Height -= 2;
      destinationRectangle.Width -= 2;
      int width = destinationRectangle.Width;
      destinationRectangle.Width = (int) ((double) destinationRectangle.Width * (double) Math.Min(1f, avatar.Health / avatar.MaxHealth));
      if (destinationRectangle.Width > 0)
        spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, new Rectangle?(), color1 * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, pos.Z);
      destinationRectangle.X += destinationRectangle.Width;
      destinationRectangle.Width = width - destinationRectangle.Width;
      if (destinationRectangle.Width <= 0)
        return;
      spriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, new Rectangle?(), color2 * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, pos.Z);
    }

    private void DrawNpcs(Player player, Player virtualPlayer)
    {
      NpcManager npcManager = this.instance.NpcManager;
      if (npcManager == null)
        return;
      GraphicStatics.AvatarShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      GraphicStatics.AvatarShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      GraphicStatics.AvatarShader.World.SetValue(virtualPlayer.WorldShake);
      GraphicStatics.AvatarShader.Alpha.SetValue(1f);
      Effect effect = GraphicStatics.AvatarShader.Effect;
      effect.CurrentTechnique = effect.Techniques["AvatarShaderInstancing"];
      effect.CurrentTechnique.Passes[0].Apply();
      Dictionary<int, List<Actor>> activeContent = npcManager.ActiveContent;
      lock (activeContent)
      {
        foreach (KeyValuePair<int, List<Actor>> keyValuePair in activeContent)
        {
          NpcContentFrame frameContent = npcManager.GetFrameContent(keyValuePair.Key);
          frameContent.PrepareForDraw(this.instance, keyValuePair.Value);
          this.DrawNpc(frameContent);
          if (Globals1.NpcTypeData[(int) keyValuePair.Value[0].ActorType].ShowHitBoxes)
            this.someHitboxesToDraw = true;
        }
      }
    }

    private void DrawNpc(NpcContentFrame npcContent)
    {
      if (npcContent.Model != null && npcContent.InstanceBuffer != null && npcContent.InstancesToDrawCount > 0)
      {
        MapChunkContentData chunkContentData = npcContent.Model.MapChunkContentData;
        if (chunkContentData.VertexBuffer == null || chunkContentData.VertexCount <= 0)
          return;
        this.SetIndices(chunkContentData.VertexCount / 2);
        this.bindings[0] = new VertexBufferBinding(chunkContentData.VertexBuffer);
        this.bindings[1] = new VertexBufferBinding((VertexBuffer) npcContent.InstanceBuffer, 0, 1);
        this.graphicsDevice.SetVertexBuffers(this.bindings);
        this.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2, npcContent.InstancesToDrawCount);
      }
      else
        Globals1.Breakpoint();
    }

    private void DrawNpcHitBoxes(Player player, Player virtualPlayer)
    {
      Dictionary<int, List<Actor>> activeContent = this.instance.NpcManager.ActiveContent;
      lock (activeContent)
      {
        foreach (KeyValuePair<int, List<Actor>> keyValuePair in activeContent)
        {
          if (keyValuePair.Value.Count > 0 && Globals1.NpcTypeData[(int) keyValuePair.Value[0].ActorType].ShowHitBoxes)
          {
            for (int index = 0; index < keyValuePair.Value.Count; ++index)
            {
              Actor actor = keyValuePair.Value[index];
              BoundingBoxRenderer.Render(actor.Box, this.graphicsDevice, Matrix.Identity, virtualPlayer.ViewMatrix, player.ProjectionMatrix, Color.Cyan, false);
              BoundingBoxRenderer.Render(actor.BodyBox, this.graphicsDevice, Matrix.Identity, virtualPlayer.ViewMatrix, player.ProjectionMatrix, Color.Yellow, false);
              BoundingBoxRenderer.Render(actor.CriticalHitBox, this.graphicsDevice, Matrix.Identity, virtualPlayer.ViewMatrix, player.ProjectionMatrix, Color.HotPink, false);
            }
          }
        }
      }
      this.someHitboxesToDraw = false;
    }

    private void DrawCuboids(Player player, Player virtualPlayer)
    {
      NpcManager npcManager = this.instance.NpcManager;
      if (npcManager.CubeAvatarModelInstanceCount <= 0)
        return;
      GraphicStatics.CubeAvatarShader.World.SetValue(player.WorldShake);
      GraphicStatics.CubeAvatarShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      int vertexCount = npcManager.CubeAvatarModel.VertexCount;
      this.graphicsDevice.SetVertexBuffers(npcManager.CubaAvatarBindings);
      this.SetIndices(vertexCount / 2);
      Effect effect = GraphicStatics.CubeAvatarShader.Effect;
      effect.CurrentTechnique = effect.Techniques["CubeAvatarShaderInstancing"];
      effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, vertexCount / 2, npcManager.CubeAvatarModelInstanceCount);
    }

    private void DrawEntities(Player player, Player virtualPlayer)
    {
      EntityManager entityManager = this.instance.EntityManager;
      if (entityManager == null)
        return;
      GraphicStatics.EntityShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      GraphicStatics.EntityShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      GraphicStatics.EntityShader.World.SetValue(virtualPlayer.WorldShake);
      GraphicStatics.EntityShader.Alpha.SetValue(1f);
      Effect effect = GraphicStatics.EntityShader.Effect;
      effect.CurrentTechnique = effect.Techniques["EntityShaderInstancing"];
      effect.CurrentTechnique.Passes[0].Apply();
      List<EntityData> activeEntities = entityManager.ActiveEntities;
      entityManager.ActiveDrawCount = 0;
      lock (activeEntities)
      {
        foreach (EntityData entityData in activeEntities)
        {
          entityData.Content.PrepareForDraw(this.instance, entityData.Entities, virtualPlayer.Frustum);
          entityManager.ActiveDrawCount += entityData.Content.InstancesToDrawCount;
          this.DrawEntity(entityData.Content);
        }
      }
    }

    private void DrawEntity(EntityContentFrame content)
    {
      if (content.Model == null || content.InstanceBuffer == null || content.InstancesToDrawCount <= 0)
        return;
      MapChunkContentData chunkContentData = content.Model.MapChunkContentData;
      if (chunkContentData.VertexBuffer == null || chunkContentData.VertexCount <= 0)
        return;
      this.SetIndices(chunkContentData.VertexCount / 2);
      this.bindings[0] = new VertexBufferBinding(chunkContentData.VertexBuffer);
      this.bindings[1] = new VertexBufferBinding((VertexBuffer) content.InstanceBuffer, 0, 1);
      this.graphicsDevice.SetVertexBuffers(this.bindings);
      this.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2, content.InstancesToDrawCount);
    }

    private void DrawSkyCurtain(Player player, Player virtualPlayer)
    {
      SkyCurtain skyCurtain = this.instance.SkyCurtain;
      if (skyCurtain == null)
      {
        this.horizY = float.MinValue;
      }
      else
      {
        float y1;
        Matrix matrix;
        if (Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock == Item.SpaceWorld)
        {
          y1 = skyCurtain.CenterY;
          this.horizY = float.MinValue;
          Vector3 zero = Vector3.Zero;
          matrix = Matrix.CreateTranslation(new Vector3(0.0f, (float) (-(double) y1 * 2.0), 0.0f)) * Matrix.CreateRotationZ(-this.instance.SunMoon.Rotation);
        }
        else
        {
          float y2 = virtualPlayer.Position.Y;
          y1 = Math.Min((float) (((double) y2 - (double) this.map.SeaLevel * (double) this.map.TileSize) * 0.5 + (double) this.map.SeaLevel * (double) this.map.TileSize), (float) ((double) y2 + (double) virtualPlayer.EyeOffset.Y - 0.5)) - virtualPlayer.Position.Y;
          matrix = Matrix.CreateTranslation(new Vector3(0.0f, y1, 0.0f));
          this.horizY = y1 + 15f;
        }
        this.graphicsDevice.Indices = skyCurtain.IndexBuffer;
        this.graphicsDevice.SetVertexBuffer(skyCurtain.VertexBuffer);
        GraphicStatics.SkyCurtainShader.World.SetValue(matrix);
        GraphicStatics.SkyCurtainShader.ViewProjection.SetValue(virtualPlayer.ViewMatrixLocal * player.ProjectionFarMatrix);
        Vector3 vector3 = new Vector3(0.0f, y1, 0.0f);
        GraphicStatics.SkyCurtainShader.LightCycle.SetValue(Math.Max(0.01f, this.map.LightCycle));
        GraphicStatics.SkyCurtainShader.SunPosition.SetValue(this.instance.SunMoon == null ? new Vector3(0.0f, 600f, 0.0f) : this.instance.SunMoon.SunPosition + vector3);
        GraphicStatics.SkyCurtainShader.MoonPosition.SetValue(this.instance.SunMoon == null ? new Vector3(0.0f, -600f, 0.0f) : this.instance.SunMoon.MoonPosition + vector3);
        GraphicStatics.SkyCurtainShader.Effect.CurrentTechnique.Passes[0].Apply();
        this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, skyCurtain.VertexBuffer.VertexCount, 0, skyCurtain.IndexBuffer.IndexCount / 3);
      }
    }

    private void DrawSunAndMoon(Player player, Player virtualPlayer)
    {
      this.graphicsDevice.SetVertexBuffer(this.instance.SunMoon.VertexBuffer);
      this.SetIndices(8);
      float y = (float) (((double) virtualPlayer.Position.Y - (double) this.map.SeaLevel * (double) this.map.TileSize) * 0.5 + (double) this.map.SeaLevel * (double) this.map.TileSize) - virtualPlayer.Position.Y;
      Matrix rotationZ = Matrix.CreateRotationZ(-this.instance.SunMoon.Rotation);
      GraphicStatics.SunMoonShader.World.SetValue(rotationZ * Matrix.CreateTranslation(new Vector3(0.0f, y, 0.0f)));
      GraphicStatics.SunMoonShader.ViewProjection.SetValue(virtualPlayer.ViewMatrixLocal * player.ProjectionFarMatrix);
      GraphicStatics.SunMoonShader.HorizY.SetValue(Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock == Item.SpaceWorld ? float.MinValue : y);
      GraphicStatics.SunMoonShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 16, 0, 8);
    }

    private void DrawStarfield(Player player, Player virtualPlayer)
    {
      bool flag = Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock == Item.SpaceWorld;
      if (!flag && (double) this.instance.SunMoon.Rotation > -1.37444686889648 && (double) this.instance.SunMoon.Rotation < 1.37444686889648)
        return;
      this.graphicsDevice.DepthStencilState = this.depthNoWriteState;
      VertexBuffer vertexBuffer = this.instance.StarMap.VertexBuffer;
      this.graphicsDevice.SetVertexBuffer(vertexBuffer);
      this.SetIndices(vertexBuffer.VertexCount / 2);
      Matrix rotationZ = Matrix.CreateRotationZ(-this.instance.SunMoon.Rotation);
      GraphicStatics.StarfieldShader.World.SetValue(rotationZ);
      GraphicStatics.StarfieldShader.ViewProjection.SetValue(virtualPlayer.ViewMatrixLocal * player.ProjectionFarMatrix);
      GraphicStatics.StarfieldShader.HorizY.SetValue(this.horizY);
      float num = 1f;
      if (!flag && (double) this.instance.SunMoon.Rotation > -1.76714587211609 && (double) this.instance.SunMoon.Rotation < 1.76714587211609)
        num = Math.Abs(1.374447f - Math.Abs(this.instance.SunMoon.Rotation)) / 0.3926991f;
      GraphicStatics.StarfieldShader.Alpha.SetValue(num);
      GraphicStatics.StarfieldShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, vertexBuffer.VertexCount / 2);
    }

    private void DrawCloudsAndWater(Player player, Player virtualPlayer)
    {
      if (virtualPlayer.IsAboveCloudLevel)
      {
        this.DrawAlphaParticles(player, virtualPlayer);
        this.DrawWaterBlocks(player, virtualPlayer);
        this.DrawClouds(player, virtualPlayer);
      }
      else
      {
        this.DrawAlphaParticles(player, virtualPlayer);
        this.DrawClouds(player, virtualPlayer);
        this.DrawWaterBlocks(player, virtualPlayer);
      }
    }

    private void DrawClouds(Player player, Player virtualPlayer)
    {
      if (!Globals2.GameSettings.ViewClouds || !this.instance.CloudMapManager.IsCloudsVisible || virtualPlayer.IsInCloud)
        return;
      this.materialSet = false;
      this.DrawCloupMap(this.instance.CloudMapManager.CurrentCloudMap, player, virtualPlayer, this.playerFrustum);
      if (!this.instance.CloudMapManager.IsTransitioning)
        return;
      this.DrawCloupMap(this.instance.CloudMapManager.OldCloudMap, player, virtualPlayer, this.playerFrustum);
    }

    private void DrawCloupMap(
      CloudMap map,
      Player player,
      Player virtualPlayer,
      BoundingFrustum playerFrustum)
    {
      if (map == null || (double) map.Alpha <= 0.0 || playerFrustum.Contains(map.Box) == ContainmentType.Disjoint)
        return;
      MapChunkContentData vertexData = (map.Regions[0].Chunks[0] as MapChunkTM).Content.GetVertexData();
      if (vertexData.VertexBuffer == null || vertexData.WaterVertexCount <= 0)
        return;
      if (!this.materialSet)
      {
        this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
        GraphicStatics.CloudShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
        GraphicStatics.CloudShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
        this.materialSet = true;
      }
      int waterVertexCount = vertexData.WaterVertexCount;
      GraphicStatics.CloudShader.Alpha.SetValue(map.Alpha);
      GraphicStatics.CloudShader.World.SetValue(map.World);
      this.SetIndices(waterVertexCount / 2);
      this.graphicsDevice.SetVertexBuffer(vertexData.VertexBuffer);
      GraphicStatics.CloudShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexData.VertexCount, 0, waterVertexCount, 0, waterVertexCount / 2);
    }

    private void DrawSignText(Player player, Player virtualPlayer)
    {
      if (this.signTextVertexCount <= 0 || this.signTextVertexBuffer == null)
        return;
      this.graphicsDevice.SetVertexBuffer(this.signTextVertexBuffer);
      this.graphicsDevice.Indices = MapChunkContent.IndexBuffer;
      GraphicStatics.SignTextShader.World.SetValue(virtualPlayer.WorldShake);
      GraphicStatics.SignTextShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      GraphicStatics.SignTextShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      GraphicStatics.SignTextShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.signTextVertexCount, 0, this.signTextVertexCount / 2);
    }

    private void DrawArcadeGames(Player player, Player virtualPlayer)
    {
      if (this.instance.ArcadeMachines.Count <= 0)
        return;
      GraphicStatics.GlobalShader.World.SetValue(virtualPlayer.WorldShake);
      GraphicStatics.GlobalShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      GraphicStatics.GlobalShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      foreach (ArcadeMachine arcadeMachine in this.instance.ArcadeMachines)
      {
        this.BuildArcadeMachinePrimitive(arcadeMachine.Point, arcadeMachine.Face);
        GraphicStatics.GlobalShader.Texture.SetValue((Texture) arcadeMachine.RenderTarget);
        GraphicStatics.GlobalShader.Effect.CurrentTechnique.Passes[0].Apply();
        this.graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, this.arcadeMachineVertices, 0, 4, this.arcadeMachineIndices, 0, 2);
      }
      GraphicStatics.GlobalShader.Texture.SetValue((Texture) GraphicStatics.TexturePack.BlockTexture);
    }

    private void DrawParticles(Player player, Player virtualPlayer)
    {
      if (!this.instance.ParticleManager.HasParticlesToRender)
        return;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateNoCull;
      GraphicStatics.GlobalShader.World.SetValue(this.instance.WorldNoShake);
      GraphicStatics.GlobalShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      GraphicStatics.GlobalShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      try
      {
        this.instance.ParticleManager.BuildVertices(virtualPlayer);
        GraphicStatics.GlobalShader.Texture.SetValue((Texture) GraphicStatics.TexturePack.ItemTexture);
        GraphicStatics.GlobalShader.Effect.CurrentTechnique.Passes[0].Apply();
        this.DrawParticles(this.instance.ParticleManager.ItemVerticesCritical);
        this.DrawParticles(this.instance.ParticleManager.ItemVertices);
        GraphicStatics.GlobalShader.Texture.SetValue((Texture) GraphicStatics.TexturePack.BlockTexture);
        GraphicStatics.GlobalShader.Effect.CurrentTechnique.Passes[0].Apply();
        this.DrawParticles(this.instance.ParticleManager.ItemVerticesBlocksCritical);
        this.DrawParticles(this.instance.ParticleManager.ItemVerticesBlocks);
        CustomArray<VertexItemBlock2> blockVertices = this.instance.ParticleManager.BlockVertices;
        if (blockVertices.Count <= 0)
          return;
        this.graphicsDevice.RasterizerState = this.rasterStateCull;
        GraphicStatics.ItemInHandShader.World.SetValue(this.instance.WorldNoShake);
        GraphicStatics.ItemInHandShader.View.SetValue(virtualPlayer.ViewMatrix);
        GraphicStatics.ItemInHandShader.Projection.SetValue(player.ProjectionMatrix);
        GraphicStatics.ItemInHandShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
        GraphicStatics.ItemInHandShader.Sunlight.SetValue(1f);
        GraphicStatics.ItemInHandShader.Blocklight.SetValue(1f);
        GraphicStatics.ItemInHandShader.LightCycle.SetValue(1f);
        int primitiveCount = blockVertices.Count / 2;
        this.SetIndices(primitiveCount);
        Effect effect = GraphicStatics.ItemInHandShader.Effect;
        effect.CurrentTechnique = effect.Techniques["ColorShader"];
        effect.CurrentTechnique.Passes[0].Apply();
        this.graphicsDevice.DrawUserIndexedPrimitives<VertexItemBlock2>(PrimitiveType.TriangleList, blockVertices.Array, 0, blockVertices.Count, MapChunkContent.Indices, 0, primitiveCount, VertexItemBlock2.vertexDeclaration);
      }
      catch (OutOfMemoryException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(31, (Exception) ex);
      }
    }

    private void DrawParticles(CustomArray<VertexPositionNormalTexture> vertices)
    {
      if (vertices.Count <= 0)
        return;
      int primitiveCount = vertices.Count / 2;
      this.SetIndices(primitiveCount);
      this.graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices.Array, 0, vertices.Count, MapChunkContent.Indices, 0, primitiveCount, VertexPositionNormalTexture.VertexDeclaration);
    }

    private void DrawAlphaParticles(Player player, Player virtualPlayer)
    {
      if (player.RainParticleSystem.HasParticlesToDraw)
      {
        this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateNoCull;
        GraphicStatics.RainShader.World.SetValue(this.instance.WorldNoShake);
        GraphicStatics.RainShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
        GraphicStatics.RainShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
        player.RainParticleSystem.Draw(player, virtualPlayer);
      }
      if (player.HailParticleSystem.HasParticlesToDraw)
      {
        this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
        GraphicStatics.HailShader.World.SetValue(this.instance.WorldNoShake);
        GraphicStatics.HailShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
        GraphicStatics.HailShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
        player.HailParticleSystem.Draw(player, virtualPlayer, this);
      }
      if (!this.instance.EmitterParticleSystem.HasParticlesToDraw)
        return;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateCull;
      GraphicStatics.ParticleShader.World.SetValue(this.instance.WorldNoShake);
      GraphicStatics.ParticleShader.ViewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
      GraphicStatics.ParticleShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      this.instance.EmitterParticleSystem.Draw(player, virtualPlayer, this);
    }

    private void DrawSplintering(Player player, Player virtualPlayer)
    {
      this.splinterVertexCount = 0;
      foreach (Gamer allEnabledGamer in this.instance.NetworkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        int splinter = tag.Splinter;
        if (splinter >= 0)
          this.BuildSplinterVertices(virtualPlayer, tag.SwingTarget, splinter);
      }
      if (this.splinterVertexCount <= 0)
        return;
      GraphicStatics.GlobalShader.World.SetValue(virtualPlayer.WorldShake);
      GraphicStatics.GlobalShader.ViewProjection.SetValue(virtualPlayer.ViewMatrixLocal * player.ProjectionMatrix);
      GraphicStatics.GlobalShader.CameraPosition.SetValue(virtualPlayer.EyePosition);
      GraphicStatics.GlobalShader.Texture.SetValue((Texture) GraphicStatics.TexturePack.ItemTexture);
      GraphicStatics.GlobalShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, this.splinterVertices, 0, this.splinterVertexCount, this.splinterIndices, 0, this.splinterVertexCount / 2);
      GraphicStatics.GlobalShader.Texture.SetValue((Texture) GraphicStatics.TexturePack.BlockTexture);
    }

    private void BuildSplinterVertices(Player virtualPlayer, GlobalPoint3D p, int splint)
    {
      BoundingBox blockBox = this.instance.GetBlockBox(p);
      blockBox.Min -= virtualPlayer.Position;
      blockBox.Max -= virtualPlayer.Position;
      VertexPositionNormalTexture positionNormalTexture = new VertexPositionNormalTexture();
      Rectangle texRect = GraphicStatics.TexturePack.ItemSrcRect((Item) (476 + splint));
      Vector4 vector4 = GraphicStatics.TexturePack.ConvertTexPackRectToVector4(GraphicStatics.TexturePack.ItemTexture, texRect);
      Vector2 vector2_1 = new Vector2(vector4.X, vector4.Z);
      Vector2 vector2_2 = new Vector2(vector4.X, vector4.Y);
      Vector2 vector2_3 = new Vector2(vector4.W, vector4.Y);
      Vector2 vector2_4 = new Vector2(vector4.W, vector4.Z);
      positionNormalTexture.Normal = Vector3.Backward;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_1.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_1.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.TextureCoordinate.X = vector2_2.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_2.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.TextureCoordinate.X = vector2_3.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_3.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.TextureCoordinate.X = vector2_4.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_4.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Normal = Vector3.Left;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_1.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_1.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.TextureCoordinate.X = vector2_2.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_2.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_3.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_3.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.TextureCoordinate.X = vector2_4.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_4.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Normal = Vector3.Forward;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_1.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_1.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_2.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_2.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_3.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_3.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_4.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_4.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Normal = Vector3.Right;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_1.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_1.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_2.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_2.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_3.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_3.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_4.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_4.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      if (ItemData.IsSubType((Item) this.map.GetBlockID(p), ItemSubType.Door))
        return;
      positionNormalTexture.Normal = Vector3.Up;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_1.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_1.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_2.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_2.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_3.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_3.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Max.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_4.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_4.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Normal = Vector3.Down;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_1.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_1.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Max.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_2.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_2.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Min.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_3.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_3.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
      positionNormalTexture.Position.X = blockBox.Min.X;
      positionNormalTexture.Position.Y = blockBox.Min.Y;
      positionNormalTexture.Position.Z = blockBox.Max.Z;
      positionNormalTexture.TextureCoordinate.X = vector2_4.X;
      positionNormalTexture.TextureCoordinate.Y = vector2_4.Y;
      this.splinterVertices[this.splinterVertexCount++] = positionNormalTexture;
    }

    private void DrawZones(Player player, Player virtualPlayer)
    {
      if (!Globals2.GameSettings.ViewZones)
        return;
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy == null || mapStrategy.Zones.Count <= 0)
        return;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateNoCullBiasPlus;
      this.basicEffect.World = Matrix.Identity;
      this.basicEffect.Techniques[0].Passes[0].Apply();
      List<Zone> zones = mapStrategy.Zones;
      for (int index = 0; index < zones.Count; ++index)
      {
        Zone zone = zones[index];
        if (zone != null)
        {
          Color color = zone.HasZoneType(ZoneType.Spawn) ? Color.White : (zone.HasZoneType(ZoneType.NoCombat) ? Color.Red : (zone.HasZoneType(ZoneType.NoEdit) ? Color.Yellow : Color.LightBlue));
          Vector3 position1 = this.map.GetPosition(zone.Min);
          Vector3 position2 = this.map.GetPosition(zone.Max);
          position1.Y -= this.map.TileSize;
          position2.X += this.map.TileSize;
          position2.Z += this.map.TileSize;
          this.zonePrimitive.Build(Vector3.Zero, position1 - virtualPlayer.Position, position2 - virtualPlayer.Position, color * 0.4f);
          this.graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.zonePrimitive.Vertices, 0, 24, this.zonePrimitive.Indices, 0, 12, VertexPositionColor.VertexDeclaration);
        }
      }
    }

    private void DrawSounds(Player player, Player virtualPlayer)
    {
      if (!Globals2.GameSettings.ViewSounds)
        return;
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      List<AmbientSoundWorker.SoundInstance> sounds = this.instance.AmbientSoundManager.Sounds;
      if (mapStrategy == null || sounds.Count <= 0)
        return;
      this.graphicsDevice.RasterizerState = GraphicStatics.IsWireFrame ? this.rasterStateWireFrame : this.rasterStateNoCullBiasPlus;
      BoundingSphere sphere = new BoundingSphere();
      for (int index = 0; index < sounds.Count; ++index)
      {
        sphere.Center = sounds[index].Emitter.Position - virtualPlayer.Position;
        sphere.Radius = sounds[index].Range;
        BoundingSphereRenderer.Render(sphere, this.graphicsDevice, virtualPlayer.ViewMatrixLocal, player.ProjectionMatrix, Color.Yellow);
      }
    }

    private void DrawSwingTargetFrame(Player player, Player virtualPlayer)
    {
      if (virtualPlayer.SwingTargetIsValid && virtualPlayer.Settings.HudVisible && (!virtualPlayer.IsClipboardEquipped && !player.IsAssemblingPhoto))
      {
        BoundingBox swingTargetBox = virtualPlayer.SwingTargetBox;
        swingTargetBox.Min -= new Vector3(0.01f, 0.01f, 0.01f);
        swingTargetBox.Max += new Vector3(0.01f, 0.01f, 0.01f);
        swingTargetBox.Min -= virtualPlayer.Position;
        swingTargetBox.Max -= virtualPlayer.Position;
        BoundingBoxRenderer.Render(swingTargetBox, this.graphicsDevice, Matrix.Identity, virtualPlayer.ViewMatrixLocal, player.ProjectionMatrix, virtualPlayer.PlaceTargetColor, true);
      }
      NpcManager npcManager = this.instance.NpcManager;
      if (npcManager == null)
        return;
      List<NpcBase> npcList = npcManager.GetNpcList();
      for (int index = npcList.Count - 1; index >= 0; --index)
      {
        NpcBase npcBase = npcList[index];
        if (npcBase != null && !npcBase.IsDeadOrInactiveOrDisabled && (npcBase.SwingTargetIsValid && npcBase.Properties.ShowSwingTarget.Value))
        {
          BoundingBox boxFace = this.instance.GetBoxFace(this.instance.GetBlockBox(npcBase.SwingTarget), npcBase.SwingFace);
          boxFace.Min -= new Vector3(0.01f, 0.01f, 0.01f);
          boxFace.Max += new Vector3(0.01f, 0.01f, 0.01f);
          boxFace.Min -= virtualPlayer.Position;
          boxFace.Max -= virtualPlayer.Position;
          BoundingBoxRenderer.Render(boxFace, this.graphicsDevice, Matrix.Identity, virtualPlayer.ViewMatrixLocal, player.ProjectionMatrix, Color.White, true);
        }
      }
    }

    private void DrawScriptTools(Player player)
    {
      List<ScriptIntersectDisplay> intersectDisplays = this.instance.ScriptIntersectDisplays;
      lock (intersectDisplays)
      {
        for (int index = intersectDisplays.Count - 1; index >= 0; --index)
        {
          ScriptIntersectDisplay intersectDisplay = intersectDisplays[index];
          switch (intersectDisplay.Shape)
          {
            case ScriptShape.Ray:
              RayRenderer.Render(new Ray(intersectDisplay.Box.Min, intersectDisplay.Box.Max), intersectDisplay.Length, this.graphicsDevice, player.ViewMatrix, player.ProjectionMatrix, Color.White);
              break;
            case ScriptShape.Box:
              BoundingBoxRenderer.Render(intersectDisplay.Box, this.graphicsDevice, Matrix.Identity, player.ViewMatrix, player.ProjectionMatrix, Color.White, false);
              break;
            case ScriptShape.Sphere:
              BoundingSphereRenderer.Render(new BoundingSphere(intersectDisplay.Box.Min, intersectDisplay.Box.Max.X), this.graphicsDevice, player.ViewMatrix, player.ProjectionMatrix, Color.White);
              break;
            case ScriptShape.Frustum:
              this.frustumForDraw.Matrix = intersectDisplay.Frustum;
              BoundingFrustumRenderer.Render(this.frustumForDraw, this.graphicsDevice, player.ViewMatrix, player.ProjectionMatrix, Color.White);
              break;
          }
          if (++intersectDisplay.Timer > 200)
            intersectDisplays.RemoveAt(index);
        }
      }
    }

    private Color GetCombatLevelColor(int virtualPlayerCombatLevel, int avatarCombatLevel)
    {
      if (avatarCombatLevel >= virtualPlayerCombatLevel + 10)
        return Color.Red;
      if (avatarCombatLevel <= virtualPlayerCombatLevel - 10)
        return Color.Green;
      if (avatarCombatLevel <= virtualPlayerCombatLevel + 3 && avatarCombatLevel >= virtualPlayerCombatLevel - 3)
        return Color.Yellow;
      if (avatarCombatLevel < virtualPlayerCombatLevel)
        return Color.YellowGreen;
      return Color.Orange;
    }

    public void SetIndices(int primitiveCount)
    {
      if (primitiveCount * 3 > MapChunkContent.IndexBuffer.IndexCount)
        MapChunkContent.BuildChunkIndices(primitiveCount, true);
      if (this.graphicsDevice.Indices == MapChunkContent.IndexBuffer)
        return;
      this.graphicsDevice.Indices = MapChunkContent.IndexBuffer;
    }

    private void DrawIndexedPrimitives(
      Effect effect,
      VertexBuffer vb,
      int baseVertex,
      int vertexCount)
    {
      if (vb.IsDisposed)
        return;
      this.SetIndices(vertexCount / 2);
      lock (VoxelMeshBuilder.VBSemaphore)
      {
        this.graphicsDevice.SetVertexBuffer(vb);
        effect.CurrentTechnique.Passes[0].Apply();
        this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, baseVertex, 0, vertexCount, 0, vertexCount / 2);
      }
    }

    private void SetShaderParams(Player player, Player virtualPlayer)
    {
      bool isAvatarDesigner = this.instance.IsAvatarDesigner;
      bool flag1 = this.instance.SunMoon == null;
      bool isPlayerUnderWater = !isAvatarDesigner && virtualPlayer.IsUnderWater;
      bool isPlayerUnderLava = !isAvatarDesigner && !isPlayerUnderWater && virtualPlayer.IsUnderLava;
      bool isPlayerInCloud = !isAvatarDesigner && !isPlayerUnderWater && !isPlayerUnderLava && virtualPlayer.IsInCloud;
      int num1 = isPlayerUnderLava ? 8 : 1;
      if (isPlayerUnderWater)
      {
        for (GlobalPoint3D point = this.map.GetPoint(virtualPlayer.EyePosition); this.map.GetBlockID(point + GlobalPoint3D.Up) == (byte) 11 && num1 < 6; ++num1)
          point += GlobalPoint3D.Up;
      }
      float lightCycle = this.map.LightCycle;
      Vector3 vector3_1 = Vector3.Normalize(-(flag1 ? new Vector3(0.0f, 1f, 0.0f) : ((double) lightCycle > 0.5 ? this.instance.SunMoon.SunPosition : ((double) lightCycle == 0.0 ? this.instance.SunMoon.MoonPosition : Vector3.Transform(this.instance.SunMoon.MoonPosition, Matrix.CreateRotationZ((float) ((double) lightCycle * 2.0 * -3.14159274101257)))))));
      bool flag2 = (double) virtualPlayer.FogIntensity > 0.0;
      int lavaLevel = isPlayerUnderWater || isPlayerUnderLava || isPlayerInCloud ? -1 : virtualPlayer.ViewingLavaLevel;
      bool flag3 = isPlayerUnderWater || isPlayerUnderLava || (isPlayerInCloud || flag2) || lavaLevel >= 0;
      Vector4 vector4 = flag3 ? this.GetFogColor((Map) this.map, virtualPlayer, isPlayerUnderWater, isPlayerUnderLava, isPlayerInCloud, lavaLevel, false) : Vector4.Zero;
      float farClip = player.FarClip;
      float num2 = farClip;
      float num3 = farClip;
      float num4 = farClip * 0.85f;
      if (flag3)
      {
        num2 = isPlayerUnderWater || isPlayerUnderLava ? (float) (-90 - num1) : -20f;
        num3 = isPlayerUnderWater || isPlayerUnderLava ? (float) (45 - num1) : (isPlayerInCloud ? farClip * 0.5f : farClip - (farClip - (float) virtualPlayer.FogVisibility) * virtualPlayer.FogIntensity);
      }
      this.basicEffect.View = virtualPlayer.ViewMatrixLocal;
      this.basicEffect.Projection = player.ProjectionMatrix;
      this.basicEffect.FogColor = new Vector3(vector4.X, vector4.Y, vector4.Z);
      this.basicEffect.FogEnabled = true;
      this.basicEffect.FogStart = flag2 || isPlayerUnderWater || (isPlayerUnderLava || isPlayerInCloud) ? num2 : num4;
      this.basicEffect.FogEnd = flag2 || isPlayerUnderWater || (isPlayerUnderLava || isPlayerInCloud) ? num3 : player.FarClip;
      float num5 = 0.0f;
      Vector3 vector3_2 = Vector3.Zero;
      InventoryItem inventoryItem = virtualPlayer.Inventory[virtualPlayer.Inventory.NeckIndex];
      if (inventoryItem.ItemID == Item.AmuletOfStarlight && virtualPlayer.CanUseItem(Item.AmuletOfStarlight))
      {
        float num6 = 1.5f;
        float num7 = 0.05f;
        vector3_2 = new Vector3(num6 + (float) this.map.Random.NextDouble() * num7, num6 + (float) this.map.Random.NextDouble() * num7, num6 + (float) this.map.Random.NextDouble() * num7);
        num5 = (float) (11.5 + this.map.Random.NextDouble() * 0.5) * (float) ((double) inventoryItem.Durability / (double) ItemData.GetItemDurability(Item.AmuletOfStarlight) * 0.5 + 0.5);
        if (player.IsGod)
        {
          num5 *= 10f;
          vector3_2 = Vector3.One;
        }
      }
      else if (!isPlayerUnderWater && (virtualPlayer.LeftHand.ItemID == Item.Torch || virtualPlayer.RightHand.ItemID == Item.Torch))
      {
        float num6 = 0.03f;
        Vector3 lanturnColor = GraphicStatics.TexturePack.LanturnColor;
        vector3_2 = new Vector3(lanturnColor.X + (float) this.map.Random.NextDouble() * num6, lanturnColor.Y + (float) this.map.Random.NextDouble() * num6, lanturnColor.Z + (float) this.map.Random.NextDouble() * num6);
        num5 = (float) (4.0 + this.map.Random.NextDouble() * 0.25);
      }
      Vector2 vector2_1 = flag1 ? Vector2.Zero : this.instance.SunMoon.SunEffectUV;
      Vector2 vector2_2 = flag1 ? Vector2.Zero : this.instance.SunMoon.MoonEffectUV;
      int num8 = flag1 ? 2 : (this.instance.SunMoon.CurrentEffect == SunMoon.SunEffect.Sunrise ? 0 : 2);
      GraphicStatics.GlobalShader.LanturnColor.SetValue(vector3_2);
      GraphicStatics.GlobalShader.LanturnRange.SetValue(num5);
      GraphicStatics.ItemInHandShader.FogStart.SetValue(num2);
      GraphicStatics.ItemInHandShader.FogEnd.SetValue(num3);
      GraphicStatics.ItemInHandShader.FogColor.SetValue(vector4);
      GraphicStatics.ItemInHandShader.LanturnColor.SetValue(vector3_2);
      GraphicStatics.ItemInHandShader.LanturnRange.SetValue(num5);
      GraphicStatics.MapShader.FarClip.SetValue(farClip);
      GraphicStatics.MapShader.FadeStart.SetValue(num4);
      GraphicStatics.MapShader.FogStart.SetValue(num2);
      GraphicStatics.MapShader.FogEnd.SetValue(num3);
      GraphicStatics.MapShader.FogColor.SetValue(vector4);
      GraphicStatics.MapShader.SunDirection.SetValue(vector3_1);
      GraphicStatics.MapShader.SunSideNormal.SetValue(num8);
      GraphicStatics.MapShader.SunEffectColorTextCoords.SetValue(vector2_1);
      GraphicStatics.MapShader.MoonEffectColorTextCoords.SetValue(vector2_2);
      GraphicStatics.MapShader.TextureCoordMovement.SetValue(this.GetSurfaceWaterTextureMovement());
      GraphicStatics.MapShader.TextureCoordFireOffset.SetValue(this.GetFireTextureOffset());
      GraphicStatics.MapShader.TextureCoordLavaOffset.SetValue(this.lavaTextureOffset);
      GraphicStatics.MapShader.TextureCoordWaterOffset.SetValue(this.waterTextureOffset);
      GraphicStatics.MapShader.LanturnColor.SetValue(vector3_2);
      GraphicStatics.MapShader.LanturnRange.SetValue(num5);
      if (Globals2.GameSettings.FloraAnimation)
      {
        GraphicStatics.MapShader.WindTime.SetValue(this.instance.Wind.ShaderWindTime);
        GraphicStatics.MapShader.WindAmount.SetValue(this.instance.Wind.ShaderWindAmount);
        GraphicStatics.MapShader.WindDirection.SetValue(this.instance.Wind.ShaderWindDirection);
      }
      else
      {
        GraphicStatics.MapShader.WindTime.SetValue(0);
        GraphicStatics.MapShader.WindAmount.SetValue(0);
        GraphicStatics.MapShader.WindDirection.SetValue(Vector3.Zero);
      }
      int num9 = (int) ((double) Globals2.GameSettings.TextureSmoothing * 40.0 + 18.0);
      if (virtualPlayer != player || virtualPlayer.IsBinocularView)
        num9 += (int) ((1.0 - (double) virtualPlayer.FOVNormalized) * (double) virtualPlayer.FarClip);
      GraphicStatics.MapShader.FullLODDistance.SetValue(num9);
      GraphicStatics.ParticleShader.FarClip.SetValue(farClip);
      GraphicStatics.ParticleShader.FadeStart.SetValue(num4);
      GraphicStatics.ParticleShader.FogStart.SetValue(num2);
      GraphicStatics.ParticleShader.FogEnd.SetValue(num3);
      GraphicStatics.ParticleShader.FogColor.SetValue(vector4);
      GraphicStatics.EntityShader.FarClip.SetValue(farClip);
      GraphicStatics.EntityShader.FadeStart.SetValue(num4);
      GraphicStatics.EntityShader.FogStart.SetValue(num2);
      GraphicStatics.EntityShader.FogEnd.SetValue(num3);
      GraphicStatics.EntityShader.FogColor.SetValue(vector4);
      GraphicStatics.EntityShader.SunDirection.SetValue(vector3_1);
      GraphicStatics.EntityShader.SunSideNormal.SetValue(num8);
      GraphicStatics.EntityShader.SunEffectColorTextCoords.SetValue(vector2_1);
      GraphicStatics.EntityShader.MoonEffectColorTextCoords.SetValue(vector2_2);
      GraphicStatics.EntityShader.MaxLight.SetValue(this.map.MaxLight + 1f);
      GraphicStatics.EntityShader.LanturnColor.SetValue(vector3_2);
      GraphicStatics.EntityShader.LanturnRange.SetValue(num5);
      GraphicStatics.AvatarShader.FarClip.SetValue(farClip);
      GraphicStatics.AvatarShader.FadeStart.SetValue(num4);
      GraphicStatics.AvatarShader.FogStart.SetValue(num2);
      GraphicStatics.AvatarShader.FogEnd.SetValue(num3);
      GraphicStatics.AvatarShader.FogColor.SetValue(vector4);
      GraphicStatics.AvatarShader.SunDirection.SetValue(vector3_1);
      GraphicStatics.AvatarShader.SunSideNormal.SetValue(num8);
      GraphicStatics.AvatarShader.SunEffectColorTextCoords.SetValue(vector2_1);
      GraphicStatics.AvatarShader.MoonEffectColorTextCoords.SetValue(vector2_2);
      GraphicStatics.AvatarShader.MaxLight.SetValue(this.map.MaxLight + 1f);
      GraphicStatics.AvatarShader.LanturnColor.SetValue(vector3_2);
      GraphicStatics.AvatarShader.LanturnRange.SetValue(num5);
      GraphicStatics.SignTextShader.FogColor.SetValue(vector4);
      GraphicStatics.SignTextShader.FogStart.SetValue(num2);
      GraphicStatics.SignTextShader.FogEnd.SetValue(num3);
      GraphicStatics.CloudShader.FarClip.SetValue(farClip);
      GraphicStatics.CloudShader.FadeStart.SetValue(num4);
      GraphicStatics.CloudShader.FogStart.SetValue(num2);
      GraphicStatics.CloudShader.FogEnd.SetValue(num3);
      GraphicStatics.CloudShader.FogColor.SetValue(vector4);
      GraphicStatics.CloudShader.SunDirection.SetValue(vector3_1);
      GraphicStatics.CloudShader.SunSideNormal.SetValue(num8);
      GraphicStatics.CloudShader.SunEffectColorTextCoords.SetValue(vector2_1);
      GraphicStatics.CloudShader.MoonEffectColorTextCoords.SetValue(vector2_2);
      GraphicStatics.StarfieldShader.FogColor.SetValue(vector4);
      GraphicStatics.SkyCurtainShader.FogColor.SetValue(vector4);
      GraphicStatics.SkyCurtainShader.SunEffectColorTextCoords.SetValue(vector2_1);
      GraphicStatics.SkyCurtainShader.MoonEffectColorTextCoords.SetValue(vector2_2);
      GraphicStatics.SunMoonShader.SunEffectColorTextCoords.SetValue(vector2_1);
      GraphicStatics.SunMoonShader.MoonEffectColorTextCoords.SetValue(vector2_2);
      GraphicStatics.SunMoonShader.Intensity.SetValue(1f - vector4.W);
      Vector3 vector3_3 = new Vector3(0.0f, vector3_3.Y = (float) (((double) virtualPlayer.Position.Y - (double) this.map.SeaLevel * (double) this.map.TileSize) * 0.5 + (double) this.map.SeaLevel * (double) this.map.TileSize), 0.0f);
      Vector3 vector3_4 = flag1 ? new Vector3(0.0f, 600f, 0.0f) : this.instance.SunMoon.SunPosition + vector3_3;
      Vector3 vector3_5 = flag1 ? new Vector3(0.0f, -600f, 0.0f) : this.instance.SunMoon.MoonPosition + vector3_3;
      GraphicStatics.CloudShader.SunPosition.SetValue(vector3_4);
      GraphicStatics.CloudShader.MoonPosition.SetValue(vector3_5);
      GraphicStatics.AvatarShader.SunPosition.SetValue(vector3_4);
      GraphicStatics.AvatarShader.MoonPosition.SetValue(vector3_5);
      GraphicStatics.EntityShader.SunPosition.SetValue(vector3_4);
      GraphicStatics.EntityShader.MoonPosition.SetValue(vector3_5);
      vector3_4.Y -= virtualPlayer.Position.Y;
      vector3_5.Y -= virtualPlayer.Position.Y;
      GraphicStatics.MapShader.SunPosition.SetValue(vector3_4);
      GraphicStatics.MapShader.MoonPosition.SetValue(vector3_5);
      float num10 = (float) (0.5 + (double) this.map.LightCycle * 0.5);
      GraphicStatics.ItemInHandShader.LightCycle.SetValue(this.map.LightCycle);
      GraphicStatics.MapShader.LightCycle.SetValue(this.map.LightCycle);
      GraphicStatics.RainShader.LightValue.SetValue(num10);
      GraphicStatics.HailShader.LightValue.SetValue(num10);
      GraphicStatics.ParticleShader.LightValue.SetValue(num10);
      GraphicStatics.AvatarShader.LightCycle.SetValue(this.map.LightCycle);
      GraphicStatics.EntityShader.LightCycle.SetValue(this.map.LightCycle);
      GraphicStatics.CloudShader.LightCycle.SetValue(Math.Max(0.0f, this.map.LightCycle));
      bool flag4 = Globals2.GameSettings.ShaderDetail == ShaderDetail.Low;
      float num11 = flag1 ? 1f : this.instance.SunMoon.GlobalLight;
      if ((double) num11 == 1.0)
        num11 = 0.0f;
      else if ((double) num11 > 0.5)
        num11 = 1f - num11;
      this.rayDistance = (float) ((double) (num11 * 2f) * (((double) this.instance.MaxFarClip + 100.0) * 2.0) * 1.5);
      GraphicStatics.MapShader.RayDistance.SetValue(flag4 ? 0.0f : this.rayDistance);
      GraphicStatics.CloudShader.RayDistance.SetValue(flag4 ? 0.0f : this.rayDistance);
      GraphicStatics.AvatarShader.RayDistance.SetValue(flag4 ? 0.0f : this.rayDistance);
      GraphicStatics.EntityShader.RayDistance.SetValue(flag4 ? 0.0f : this.rayDistance);
      GraphicStatics.SkyCurtainShader.RayDistance.SetValue(this.rayDistance);
      Vector4 currentValue = virtualPlayer.CustomSkyColor.CurrentValue;
      if ((double) currentValue.X == 0.0 && (double) currentValue.Y == 0.0 && (double) currentValue.Z == 0.0)
        currentValue = GraphicStatics.CustomSkyColor.CurrentValue;
      Vector3 vector3_6 = player.SkyClamp.Update(new Vector3(currentValue.X, currentValue.Y, currentValue.Z));
      currentValue.X = vector3_6.X;
      currentValue.Y = vector3_6.Y;
      currentValue.Z = vector3_6.Z;
      GraphicStatics.SkyCurtainShader.CustomColor.SetValue(currentValue);
      vector3_6 = player.TintClamp.Update(virtualPlayer.CustomTintColor.CurrentValue * GraphicStatics.CustomTintColor.CurrentValue);
      currentValue.X = vector3_6.X;
      currentValue.Y = vector3_6.Y;
      currentValue.Z = vector3_6.Z;
      currentValue.W = 1f;
      if ((double) currentValue.X != (double) this.lastSetTintColor.X || (double) currentValue.Y != (double) this.lastSetTintColor.Y || (double) currentValue.Z != (double) this.lastSetTintColor.Z)
      {
        GraphicStatics.GlobalShader.TintColor.SetValue(currentValue);
        GraphicStatics.MapShader.TintColor.SetValue(currentValue);
        GraphicStatics.AvatarShader.TintColor.SetValue(currentValue);
        GraphicStatics.EntityShader.TintColor.SetValue(currentValue);
        GraphicStatics.ParticleShader.TintColor.SetValue(currentValue);
        GraphicStatics.ItemInHandShader.TintColor.SetValue(currentValue);
        GraphicStatics.CloudShader.TintColor.SetValue(currentValue);
        GraphicStatics.SkyCurtainShader.TintColor.SetValue(currentValue);
        this.lastSetTintColor.X = currentValue.X;
        this.lastSetTintColor.Y = currentValue.Y;
        this.lastSetTintColor.Z = currentValue.Z;
      }
      this.graphicsDevice.SamplerStates[0] = this.pointClamp;
      int num12 = Globals2.GameSettings.ShadowMaps ? 1 : 0;
    }

    private float GetLight(Vector3 pos)
    {
      float num = (float) this.map.SunLight.SunLight;
      GlobalPoint3D point = this.map.GetPoint(pos);
      if (this.map.IsValidPoint(point))
        num = this.map.GetLightNormalized(point);
      return num;
    }

    private MapChunkContent GetContent(MapChunk chunk)
    {
      return ((MapChunkTM) chunk).Content;
    }

    private Vector4 GetFogColor(
      Map map,
      Player virtualPlayer,
      bool isPlayerUnderWater,
      bool isPlayerUnderLava,
      bool isPlayerInCloud,
      int lavaLevel,
      bool drawingClouds)
    {
      Vector4 vector4 = new Vector4();
      if (map.SunLight.SunLight != (byte) 0)
      {
        int sunLight = (int) map.SunLight.SunLight;
      }
      if (isPlayerUnderWater)
        vector4 = new Vector4(GraphicStatics.TexturePack.WaterColor.ToVector3(), 1f);
      else if (isPlayerUnderLava)
        vector4 = new Vector4(GraphicStatics.TexturePack.LavaColor.ToVector3(), 1f);
      else if (isPlayerInCloud)
      {
        vector4 = new Vector4(GraphicStatics.TexturePack.CloudColor.ToVector3(), 1f);
      }
      else
      {
        switch (lavaLevel)
        {
          case 0:
          case 2:
            vector4 = new Vector4(GraphicStatics.TexturePack.LavaColor.ToVector3(), 1f);
            break;
          case 1:
            vector4 = this.oasisFogColor;
            break;
        }
        if ((double) virtualPlayer.FogIntensity > 0.0)
        {
          vector4 += new Vector4(virtualPlayer.FogColor, 0.0f);
          vector4.W = MathHelper.Clamp(vector4.W + virtualPlayer.FogIntensity, 0.0f, 1f);
        }
      }
      if (lavaLevel < 0)
      {
        float num1 = (float) (0.200000002980232 + (double) map.LightCycle * 0.800000011920929);
        vector4.X *= num1;
        vector4.Y *= num1;
        vector4.Z *= num1;
        if (isPlayerUnderWater)
        {
          float num2 = (float) (((double) map.GetLightNormalized(map.GetPoint(virtualPlayer.Position + new Vector3(0.0f, 0.05f, 0.0f))) + (double) map.GetLightNormalized(map.GetPoint(virtualPlayer.EyePosition))) * 0.5);
          vector4.X *= num2;
          vector4.Y *= num2;
          vector4.Z *= num2;
        }
      }
      return vector4;
    }

    private Vector2 GetFireTextureOffset()
    {
      int num1 = GraphicStatics.TexturePack.BlockTextureSize();
      float num2 = (float) num1 / (float) GraphicStatics.TexturePack.BlockTexture.Width;
      float num3 = (float) num1 / (float) GraphicStatics.TexturePack.BlockTexture.Height;
      if (this.fireAnimationFrame != 0)
        return new Vector2((float) (this.fireAnimationFrame - 1) * num2, 5f * num3);
      return Vector2.Zero;
    }

    private Vector2 GetSurfaceWaterTextureMovement()
    {
      if (this.waterTextureMovement.IsActive)
        this.waterTextureMovement.Update();
      else
        this.waterTextureMovement.Start(this.waterTextureMovement.CurrentValue, new Vector2((float) this.rand.NextDouble() * MapChunkContent.TexCoords1[1].X, (float) this.rand.NextDouble() * MapChunkContent.TexCoords1[TexturePack.BlockTexturesPerRow()].Y), this.rand.NextDouble() * 2.0 + 5.0, true);
      return this.waterTextureMovement.CurrentValue;
    }

    private Effect InitStatesForChunkDrawing(Player player, Player virtualPlayer)
    {
      GraphicStatics.MapShader.ViewProjection.SetValue(virtualPlayer.ViewMatrixLocal * player.ProjectionMatrix);
      GraphicStatics.MapShader.CameraPosition.SetValue(virtualPlayer.EyeOffset);
      string index = !Globals2.GameSettings.UseMipMaps || GraphicStatics.TexturePack.BlockTextureSize() <= 16 ? this.techniqueMapShader : this.techniqueMapShaderMipmaps;
      Effect effect = GraphicStatics.MapShader.Effect;
      effect.CurrentTechnique = effect.Techniques[index];
      return effect;
    }

    private void SetMapChunkDrawStuff(
      Player virtualPlayer,
      MapChunkTM chunk,
      ref MapChunkContentData contentData)
    {
      MapChunkContent content = chunk.Content;
      if (!this.waterChunkAdded && contentData.WaterVertexCount > 0)
      {
        this.waterChunkAdded = true;
        this.waterToDraw.Add(chunk);
        if (!this.alphaAdjusted)
        {
          if (content.Alpha < (byte) 251)
            content.Alpha += (byte) 5;
          else
            content.Alpha = byte.MaxValue;
          this.alphaAdjusted = true;
        }
      }
      if (this.mapChunkDrawStuffSet || contentData.VertexCount <= 0)
        return;
      this.mapChunkDrawStuffSet = true;
      if (!this.alphaAdjusted)
      {
        if (content.Alpha < (byte) 251)
          content.Alpha += (byte) 5;
        else
          content.Alpha = byte.MaxValue;
        this.alphaAdjusted = true;
      }
      float val1 = content.Alpha == byte.MaxValue ? 1f : MathHelper.SmoothStep(0.0f, 1f, (float) content.Alpha / (float) byte.MaxValue);
      if (virtualPlayer.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
        val1 = Math.Min(val1, 0.85f);
      if ((double) val1 == (double) this.lastMapAlpha)
        return;
      this.lastMapAlpha = val1;
      GraphicStatics.MapShader.Alpha.SetValue(val1);
    }

    private void SetMapChunkDrawStuffForWater(
      Player virtualPlayer,
      MapChunkTM chunk,
      ref MapChunkContentData data)
    {
      if (this.mapChunkDrawStuffSet || data.WaterVertexCount <= 0)
        return;
      this.mapChunkDrawStuffSet = true;
      MapChunkContent content = chunk.Content;
      float val1 = content.Alpha == byte.MaxValue ? 1f : MathHelper.SmoothStep(0.0f, 1f, (float) content.Alpha / (float) byte.MaxValue);
      if (virtualPlayer.IsItemEquippedAndUsable(Item.NecklaceOfFarsight))
        val1 = Math.Min(val1, 0.85f);
      if ((double) val1 == (double) this.lastMapAlpha)
        return;
      this.lastMapAlpha = val1;
      GraphicStatics.MapShader.Alpha.SetValue(val1);
    }
  }
}
