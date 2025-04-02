using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class CompileOptions : IUnityShaderCompilerSettings
{
    public const string SHADER_PLATFORM_KEY = "SelectedShaderPlatform";
    public const string BUILD_TARGET_KEY = "SelectedBuildTarget";
    private const string SHADER_VARIANTS_KEY = "ShaderVariantConfig";

    public Action<ShaderCompilerPlatform> OnShaderCompilerPlatformChange;
    public Action<BuildTarget> OnBuildTargetChange;
    
    private ShaderCompilerPlatform _selectedShaderPlatform;
    public ShaderCompilerPlatform selectedShaderPlatform
    {
        get
        {
            return _selectedShaderPlatform;
        }
        
        set
        {
            if (_selectedShaderPlatform != value)
            {
                _selectedShaderPlatform = value;
                EditorPrefs.SetInt(SHADER_PLATFORM_KEY, (int)selectedShaderPlatform);
                OnShaderCompilerPlatformChange(_selectedShaderPlatform);
            }
        }
    }

    private BuildTarget _selectedBuildTarget;
    public BuildTarget selectedBuildTarget
    {
        get
        {
            return _selectedBuildTarget;
        }
        
        set
        {
            if (_selectedBuildTarget != value)
            {
                _selectedBuildTarget = value;
                EditorPrefs.SetInt(BUILD_TARGET_KEY, (int)_selectedBuildTarget);
                OnBuildTargetChange(_selectedBuildTarget);
            }
        }
    }
    
    public List<ShaderVariantCollection> _collections;
    public List<ShaderVariantCollection> collections
    {
        get
        {
            return _collections;
        }

        set
        {
            _collections = value;
            List<string> guids = new List<string>();
            foreach (var collection in _collections)
            {
                string path = AssetDatabase.GetAssetPath(collection);
                string guid = AssetDatabase.AssetPathToGUID(path);
                guids.Add(guid);
            }
            EditorPrefs.SetString(SHADER_VARIANTS_KEY, string.Join(",", guids));
        }
    }
    public void LoadData()
    {
        _selectedShaderPlatform = (ShaderCompilerPlatform)EditorPrefs.GetInt(SHADER_PLATFORM_KEY, (int)ShaderCompilerPlatform.None);
        _selectedBuildTarget = (BuildTarget)EditorPrefs.GetInt(BUILD_TARGET_KEY, (int)BuildTarget.NoTarget);
        _collections = new List<ShaderVariantCollection>();
        string saved = EditorPrefs.GetString(SHADER_VARIANTS_KEY);
        string[] guids = saved.Split(',');
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
            if (svc != null)
            {
                _collections.Add(svc);
            }
        }
    }
}