using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;



public static class ShaderLabCompiler
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
    
    public static PassType GetPassType(ShaderData.Pass pass)
    {
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
    
    private static string BuildShaderFileSuffix(ShaderType shaderType)
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
    
    public struct ShaderVariantInfo : IEquatable<ShaderVariantInfo>
    {
        public ShaderData shaderData;
        public PassIdentifier passIdentifier;
        public ShaderType shaderType;
        public string[] keywords;
        
        private string BuildShaderFilePrefix()
        {
            ShaderData.Subshader subshader = shaderData.GetSubshader((int)passIdentifier.SubshaderIndex);
            ShaderData.Pass pass = subshader.GetPass((int)passIdentifier.PassIndex);
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

        public string GetShaderFileName()
        {
            string prefix = BuildShaderFilePrefix();
            return $"{prefix}{BuildShaderFileSuffix(shaderType)}";
        }

        public bool Equals(ShaderVariantInfo other)
        {
            if (shaderData.SourceShader.name == other.shaderData.SourceShader.name && passIdentifier == other.passIdentifier && shaderType == other.shaderType)
            {
                if (keywords?.Length != other.keywords?.Length) return false;
                for (int i = 0; i < keywords?.Length; i++)
                {
                    if (keywords[i] != other.keywords[i]) return false;
                }
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderVariantInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(shaderData, passIdentifier, (int)shaderType, keywords);
        }
    }

    public struct ShaderVariantCompileResult
    {
        public ShaderVariantInfo variantInfo;
        public ShaderData.VariantCompileInfo compileInfo;
    }
    
    public static void CompileVariantCollection(ShaderVariantCollection shaderVariantCollection, ShaderCompilerPlatform platform, BuildTarget buildTarget, out List<ShaderVariantCompileResult> compileResults)
    {
        compileResults = new List<ShaderVariantCompileResult>();
        ShaderVariantCollectionImpl svc = new ShaderVariantCollectionImpl(shaderVariantCollection);
        var shaderVariants = svc.ShaderVariants;

        HashSet<ShaderVariantInfo> infos = new HashSet<ShaderVariantInfo>();
        foreach (var pair in shaderVariants)
        {
            Shader shader = pair.Key;
            Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>> variants = pair.Value;
            CollectShader(shader, variants, ref infos);
        }

        foreach (var variantInfo in infos)
        {
            ShaderData.Subshader subshader = variantInfo.shaderData.GetSubshader((int)variantInfo.passIdentifier.SubshaderIndex);
            ShaderData.Pass pass = subshader.GetPass((int)variantInfo.passIdentifier.PassIndex);
            ShaderData.VariantCompileInfo compileInfo = pass.CompileVariant(variantInfo.shaderType, variantInfo.keywords, platform, buildTarget, true);
            ShaderVariantCompileResult compileResult = new ShaderVariantCompileResult
            {
                variantInfo = variantInfo,
                compileInfo = compileInfo
            };
            compileResults.Add(compileResult);
        }
    }
    
    private static void CollectShader(Shader shader, Dictionary<PassType, List<ShaderVariantCollection.ShaderVariant>> variants, ref HashSet<ShaderVariantInfo> results)
    {
        ShaderData shaderData = ShaderUtil.GetShaderData(shader);
        
        ShaderData.Subshader subshader = shaderData.ActiveSubshader;
        for (int i = 0; i < subshader.PassCount; i++)
        {
            ShaderData.Pass pass = subshader.GetPass(i);
            PassIdentifier passIdentifier = new PassIdentifier((uint)shaderData.ActiveSubshaderIndex, (uint)i);
            CollectVariantInfos(shaderData, passIdentifier, null, ref results);
            
            PassType passType = GetPassType(pass);
            
            if (variants.TryGetValue(passType, out var passVariants))
            {
                CollectShaderVariants(shaderData, passIdentifier, passVariants, ref results);
            }
        }
    }
    
    private static void CollectShaderVariants(ShaderData shaderData, PassIdentifier passIdentifier, List<ShaderVariantCollection.ShaderVariant> passVariants, ref HashSet<ShaderVariantInfo> results)
    {
        foreach (var variant in passVariants)
        {
            CollectVariantInfos(shaderData, passIdentifier, variant.keywords, ref results);
        }
    }
    
    private static void CollectVariantInfos(ShaderData shaderData, PassIdentifier passIdentifier, string[] keywords, ref HashSet<ShaderVariantInfo> results)
    {
        ShaderData.Subshader subshader = shaderData.GetSubshader((int)passIdentifier.SubshaderIndex);
        ShaderData.Pass pass = subshader.GetPass((int)passIdentifier.PassIndex);
        string[] passkeywords = GetPassKeywords(shaderData, passIdentifier, keywords);
        if (pass.HasShaderStage(ShaderType.Vertex))
        {
            ShaderVariantInfo info = new ShaderVariantInfo
            {
                shaderData = shaderData,
                passIdentifier = passIdentifier,
                keywords = passkeywords,
                shaderType = ShaderType.Vertex
            };
            results.Add(info);
            
        }
        if (pass.HasShaderStage(ShaderType.Fragment))
        {
            ShaderVariantInfo info = new ShaderVariantInfo
            {
                shaderData = shaderData,
                passIdentifier = passIdentifier,
                keywords = passkeywords,
                shaderType = ShaderType.Fragment
            };
            results.Add(info);
        }
    }
}