using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public interface IShaderLabCompiler
{
    void OnShaderCompilerPlatformChange(ShaderCompilerPlatform platform);
    
    void OnBuildTargetChange(BuildTarget buildTarget);

    void OnOutputPathChange(string path);
}

public class ShaderLabLabCompiler : IShaderLabCompiler
{
    public struct ShaderVariantCompileInfo
    {
        private static Dictionary<string, PassType> lightMode2PassType = new Dictionary<string, PassType>()
        {
            {"", PassType.Normal},
            {"VERTEX", PassType.Vertex},
            {"VERTEXLM", PassType.VertexLM},
            {"FORWARDBASE", PassType.ForwardBase},
            {"FORWARDADD", PassType.ForwardAdd},
            {"SHADOWCASTER", PassType.ShadowCaster},
            {"DEFERRED", PassType.Deferred},
            {"META", PassType.Meta},
            {"MOTIONVECTORS", PassType.MotionVectors},
        };
        
        private static string[] GetPassKeywords(ShaderData shaderData, PassIdentifier passIdentifier, string[] keywords)
        {
            if (keywords == null) return null;
            List<string> passKeywords = new List<string>();
            LocalKeyword[] localKeywords = ShaderUtil.GetPassKeywords(shaderData.SourceShader, in passIdentifier);
            for (int i = 0; i < localKeywords.Length; i++)
            {
                if (Array.IndexOf(keywords, localKeywords[i].name) >= 0)
                {
                    passKeywords.Add(localKeywords[i].name);
                }
            }

            return passKeywords.Count == 0 ? null : passKeywords.ToArray();
        }
        
        private ShaderData shaderData;
        private PassIdentifier passIdentifier;
        private string[] keywords;
        private ShaderData.VariantCompileInfo vertex;
        private ShaderData.VariantCompileInfo fragment;

        public ShaderVariantCompileInfo(ShaderData shaderData, PassIdentifier passIdentifier, string[] keywords)
        {
            this.shaderData = shaderData;
            this.passIdentifier = passIdentifier;
            this.keywords = GetPassKeywords(shaderData, passIdentifier, keywords);
            this.vertex = default;
            this.fragment = default;
        }
        
        private ShaderData.Pass GetPass()
        {
            return shaderData.GetSubshader((int)passIdentifier.SubshaderIndex).GetPass((int)passIdentifier.PassIndex);
        }
        
        public PassType GetPassType()
        {
            var pass = GetPass();
            ShaderTagId lightMode = pass.FindTagValue(new ShaderTagId("LightMode"));
            if (!lightMode2PassType.TryGetValue(lightMode.name, out PassType passType))
            {
                if (pass.IsGrabPass)
                {
                    passType = PassType.GrabPass;
                }
                else
                {
                    passType = PassType.ScriptableRenderPipeline;
                }
            }
            return passType;
        }
        
        private string BuildShaderFilePrefix()
        {
            var pass = GetPass();
            ShaderTagId lightMode = pass.FindTagValue(new ShaderTagId("LightMode"));
            StringBuilder builder = new StringBuilder();
            builder.Append(shaderData.SourceShader.name.Replace('/', '-'));
            builder.Append("_");
            builder.Append(passIdentifier.SubshaderIndex);
            builder.Append("_");
            builder.Append(lightMode.name);
            if (keywords != null)
            {
                foreach (string keyword in keywords)
                {
                    builder.Append("_");
                    builder.Append(keyword);
                }    
            }
            return builder.ToString();
        }
        private string BuildShaderFileSuffix(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Vertex:
                    return ".vert";
                case ShaderType.Fragment:
                    return ".frag";
                case ShaderType.Geometry:
                    return ".geom";
                case ShaderType.Hull:
                    return ".tesc";
                case ShaderType.Domain:
                    return ".tese";
                case ShaderType.Surface:
                case ShaderType.RayTracing:
                    return ".glsl";
                default:
                    throw new ArgumentOutOfRangeException(nameof(shaderType), shaderType, null);
            }
        }

        
        public void Compile(ShaderCompilerPlatform platform, BuildTarget buildTarget, string savePath)
        {
            Compile(platform, buildTarget);
            Save(savePath);
        }
        public void Compile(ShaderCompilerPlatform platform, BuildTarget buildTarget)
        {
            var pass = GetPass();
            if (pass.HasShaderStage(ShaderType.Vertex))
                vertex = pass.CompileVariant(ShaderType.Vertex, keywords, platform, buildTarget, true);
            if (pass.HasShaderStage(ShaderType.Fragment))
                fragment = pass.CompileVariant(ShaderType.Fragment, keywords, platform, buildTarget, true);
        }

        
        public void Save(string path)
        {
            string prefix = BuildShaderFilePrefix();

            if (vertex.Success)
            {
                string vertexFileName = $"{prefix}{BuildShaderFileSuffix(ShaderType.Vertex)}";
                File.WriteAllBytes(Path.Combine(path, vertexFileName), vertex.ShaderData);
            }

            if (fragment.Success)
            {
                string fragFileName = $"{prefix}{BuildShaderFileSuffix(ShaderType.Fragment)}";
                File.WriteAllBytes(Path.Combine(path, fragFileName), fragment.ShaderData);    
            }
        }
    }
    

    private ShaderCompilerPlatform platform;
    private BuildTarget buildTarget;
    private string savePath;
    public ShaderLabLabCompiler(ShaderCompilerPlatform platform, BuildTarget buildTarget, string savePath)
    {
        this.platform = platform;
        this.buildTarget = buildTarget;
        this.savePath = savePath;
    }

    public bool CompileVariantCollection(ShaderVariantCollection shaderVariantCollection)
    {
        ShaderVariantCollectionImpl svc = new ShaderVariantCollectionImpl(shaderVariantCollection);
        var shaderVariants = svc.ShaderVariants;
        
        foreach (var pair in shaderVariants)
        {
            Shader shader = pair.Key;
            Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>> variants = pair.Value;
            CompileShader(shader, variants);
        }
        return true;
    }
    
    private void CompileShader(Shader shader, Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>> variants)
    {
        ShaderData shaderData = ShaderUtil.GetShaderData(shader);
        
        ShaderData.Subshader subshader = shaderData.ActiveSubshader;
        for (int i = 0; i < subshader.PassCount; i++)
        {
            ShaderData.Pass pass = subshader.GetPass(i);
            PassIdentifier passIdentifier = new PassIdentifier((uint)shaderData.ActiveSubshaderIndex, (uint)i);
            ShaderVariantCompileInfo compileInfo = new ShaderVariantCompileInfo(shaderData, passIdentifier, null);
            compileInfo.Compile(platform, buildTarget, savePath);
            
            PassType passType = compileInfo.GetPassType();
            if (variants.TryGetValue(passType, out var passVariants))
            {
                CompileShaderVariants(shaderData, passIdentifier, passVariants);
            }
        }
    }
    
    private void CompileShaderVariants(ShaderData shaderData, PassIdentifier passIdentifier, List<ShaderVariantCollection.ShaderVariant> passVariants)
    {
        foreach (var variant in passVariants)
        {
            ShaderVariantCompileInfo variantCompileInfo = new ShaderVariantCompileInfo(shaderData, passIdentifier, variant.keywords);
            variantCompileInfo.Compile(platform, buildTarget, savePath);
        }
    }

    public void OnShaderCompilerPlatformChange(ShaderCompilerPlatform platform)
    {
        this.platform = platform;
    }

    public void OnBuildTargetChange(BuildTarget buildTarget)
    {
        this.buildTarget = buildTarget;
    }

    public void OnOutputPathChange(string path)
    {
        this.savePath = path;
    }
}