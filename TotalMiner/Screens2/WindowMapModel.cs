// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens2.WindowMapModel
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner.Screens2
{
  internal class WindowMapModel : Window
  {
    private SamplerState pointClamp;
    private RasterizerState rasterStateCull;
    private MapModel model;
    private float modelScale;
    private string asset;
    private bool isDirty;
    private Vector3 ypr;
    private RenderTarget2D renderTarget;
    private VoxelModelManager voxelModelManager;

    public WindowMapModel(string name, int x, int y, int width, int height, float modelScale)
      : base(name, x, y, width, height)
    {
      this.modelScale = modelScale;
    }

    public override bool IsKeyNavigable
    {
      get
      {
        return false;
      }
    }

    public void SetAsset(string asset)
    {
      if (this.voxelModelManager == null && asset.IsNotEmpty())
        this.voxelModelManager = new VoxelModelManager(GameInstance.Instance, "Content\\Map", true);
      if (this.model != null)
        this.voxelModelManager.UnloadComponent(this.model);
      this.asset = asset;
      this.isDirty = true;
      if (!asset.IsNotEmpty())
        return;
      try
      {
        this.model = this.voxelModelManager.LoadComponent("System Avatars", asset, true);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(94, ex);
      }
    }

    public void SetYPR(Vector3 ypr)
    {
      this.ypr = ypr;
      this.isDirty = true;
    }

    public override void Draw(
      SpriteBatchSafe spriteBatch,
      Rectangle bound,
      float scale,
      float alpha,
      bool isEnabled)
    {
      base.Draw(spriteBatch, bound, scale, alpha, isEnabled);
      if (this.pointClamp == null)
        this.LoadContent();
      if (!this.isDirty || this.model == null)
        return;
      this.DrawModel(bound);
    }

    private void DrawModel(Rectangle bound)
    {
      MapChunkContentData chunkContentData = this.model.MapChunkContentData;
      if (chunkContentData.VertexBuffer == null || chunkContentData.VertexCount <= 0)
        return;
      GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
      if (this.renderTarget == null)
        this.renderTarget = new RenderTarget2D(graphicsDevice, bound.Width, bound.Height, false, SurfaceFormat.Color, DepthFormat.Depth16, 2, RenderTargetUsage.DiscardContents);
      float farPlaneDistance = 100f;
      Vector3 cameraPosition = new Vector3(0.0f, 0.0f, (float) ((double) this.model.ModelSize.Y / (double) this.Size.Y * 100.0));
      Matrix lookAt = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
      Matrix perspectiveFieldOfView = Matrix.CreatePerspectiveFieldOfView(0.7853982f, (float) bound.Width / (float) bound.Height, 0.1f, farPlaneDistance);
      graphicsDevice.RasterizerState = this.rasterStateCull;
      graphicsDevice.BlendState = BlendState.AlphaBlend;
      graphicsDevice.DepthStencilState = DepthStencilState.Default;
      graphicsDevice.SamplerStates[0] = this.pointClamp;
      graphicsDevice.SamplerStates[1] = this.pointClamp;
      graphicsDevice.SamplerStates[2] = this.pointClamp;
      graphicsDevice.SamplerStates[3] = this.pointClamp;
      graphicsDevice.SamplerStates[4] = this.pointClamp;
      //graphicsDevice.ReferenceStencil = 0;
      GraphicStatics.AvatarShader.FarClip.SetValue(farPlaneDistance);
      GraphicStatics.AvatarShader.FadeStart.SetValue(farPlaneDistance);
      GraphicStatics.AvatarShader.LanturnColor.SetValue(0);
      GraphicStatics.AvatarShader.LanturnRange.SetValue(0);
      GraphicStatics.AvatarShader.FogStart.SetValue(farPlaneDistance);
      GraphicStatics.AvatarShader.FogEnd.SetValue(farPlaneDistance);
      GraphicStatics.AvatarShader.FogColor.SetValue(Vector4.Zero);
      GraphicStatics.AvatarShader.LightCycle.SetValue(1f);
      GraphicStatics.AvatarShader.MaxLight.SetValue(GameInstance.Instance.Map.MaxLight + 1f);
      GraphicStatics.AvatarShader.SunDirection.SetValue(new Vector3(0.0f, 0.0f, -8f));
      GraphicStatics.AvatarShader.SunPosition.SetValue(new Vector3(0.0f, 100f, 0.0f));
      GraphicStatics.AvatarShader.RayDistance.SetValue(0);
      GraphicStatics.AvatarShader.Alpha.SetValue(1);
      GraphicStatics.AvatarShader.TintColor.SetValue(Vector4.One);
      Matrix matrix = Matrix.CreateTranslation(-((float) this.model.ModelSize.X * 0.5f), -((float) this.model.ModelSize.Y * 0.5f), -((float) this.model.ModelSize.Z * 0.5f)) * Matrix.CreateRotationY(this.ypr.X) * Matrix.CreateScale(this.modelScale);
      matrix.M44 = 49407f;
      GraphicStatics.AvatarShader.World.SetValue(matrix);
      GraphicStatics.AvatarShader.ViewProjection.SetValue(lookAt * perspectiveFieldOfView);
      GraphicStatics.AvatarShader.CameraPosition.SetValue(cameraPosition);
      GraphicStatics.AvatarShader.LightMapTexture.SetValue((Texture) GraphicStatics.TexturePack.LightMapTexture);
      GraphicStatics.AvatarShader.NightLightMapTexture.SetValue((Texture) GraphicStatics.TexturePack.NightLightMapTexture);
      graphicsDevice.SetRenderTarget(this.renderTarget);
      graphicsDevice.Clear(Color.Transparent);
      this.SetIndices(graphicsDevice, chunkContentData.VertexCount / 2);
      graphicsDevice.SetVertexBuffer(chunkContentData.VertexBuffer);
      Effect effect = GraphicStatics.AvatarShader.Effect;
      effect.CurrentTechnique = effect.Techniques["AvatarShader"];
      effect.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunkContentData.VertexCount, 0, chunkContentData.VertexCount / 2);
      graphicsDevice.SetRenderTarget((RenderTarget2D) null);
      this.LoadTexture((Texture2D) this.renderTarget, true, true, 1f);
      this.isDirty = false;
    }

    private void LoadContent()
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
      this.rasterStateCull = new RasterizerState()
      {
        CullMode = CullMode.CullCounterClockwiseFace,
        DepthBias = 0.0f,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = false,
        ScissorTestEnable = false,
        SlopeScaleDepthBias = 0.0f
      };
    }

    public void SetIndices(GraphicsDevice graphicsDevice, int primitiveCount)
    {
      if (primitiveCount * 3 > MapChunkContent.IndexBuffer.IndexCount)
        MapChunkContent.BuildChunkIndices(primitiveCount, true);
      if (graphicsDevice.Indices == MapChunkContent.IndexBuffer)
        return;
      graphicsDevice.Indices = MapChunkContent.IndexBuffer;
    }
  }
}
