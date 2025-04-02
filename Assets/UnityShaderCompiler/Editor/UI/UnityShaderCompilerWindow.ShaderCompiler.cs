using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Rendering;

public partial class UnityShaderCompilerWindow
{
    private Vector2 scrollPos;
    private string scanPath = "Assets";
    
    void DrawShaderVariantConfigTab()
    {
        DrawPlatformSelectors();
        EditorGUILayout.BeginVertical("box");
        {
            DrawToolbar();
            DrawCollectionList();
            DrawScanSection();    
        }
        EditorGUILayout.EndVertical();
    }
    
    void DrawPlatformSelectors()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                // Shader编译平台下拉菜单
                compileOptions.selectedShaderPlatform = (ShaderCompilerPlatform)EditorGUILayout.EnumPopup(
                    "编译平台：",
                    compileOptions.selectedShaderPlatform, GUILayout.Width(300));
                
                // 构建目标下拉菜单
                compileOptions.selectedBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(
                    "构建目标：",
                    compileOptions.selectedBuildTarget,
                    GUILayout.Width(300));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("开始编译"))
        {
            CompileVariantCollections();
        }
    }

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("添加", EditorStyles.toolbarButton))
            {
                AddNewCollection();
            }
            
            if (GUILayout.Button("保存配置", EditorStyles.toolbarButton))
            {
                SaveCollections();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void DrawCollectionList()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        {
            for (int i = 0; i < compileOptions.collections.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    // 显示资源字段
                    compileOptions.collections[i] = (ShaderVariantCollection)EditorGUILayout.ObjectField(
                        compileOptions.collections[i], 
                        typeof(ShaderVariantCollection), 
                        false);

                    // 删除按钮
                    if (GUILayout.Button("×", GUILayout.Width(20)))
                    {
                        compileOptions.collections.RemoveAt(i);
                        SaveCollections();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }
    void DrawScanSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("Box");
        {
            EditorGUILayout.LabelField("批量扫描", EditorStyles.boldLabel);
            
            // 路径显示和选择
            EditorGUILayout.BeginHorizontal();
            {
                scanPath = EditorGUILayout.TextField("扫描路径：", scanPath);
                if (GUILayout.Button("选择", GUILayout.Width(60)))
                {
                    string newPath = EditorUtility.OpenFolderPanel("选择扫描目录", 
                        Application.dataPath, "");
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        scanPath = GetRelativePath(newPath);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // 扫描按钮
            if (GUILayout.Button("开始扫描"))
            {
                ScanCollections();
            }
        }
        EditorGUILayout.EndVertical();
    }
    void AddNewCollection()
    {
        ShaderVariantCollection newCollection = null;
        string path = EditorUtility.OpenFilePanel("选择Shader变体文件", 
            "Assets", "shadervariants");
        
        if (!string.IsNullOrEmpty(path))
        {
            string relativePath = GetRelativePath(path);
            newCollection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(relativePath);
        }

        if (newCollection != null && !compileOptions.collections.Contains(newCollection))
        {
            compileOptions.collections.Add(newCollection);
            SaveCollections();
        }
    }
    void ScanCollections()
    {
        if (string.IsNullOrEmpty(scanPath)) return;

        string[] guids = AssetDatabase.FindAssets("t:ShaderVariantCollection", 
            new[] { scanPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ShaderVariantCollection svc = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
            
            if (svc != null && !compileOptions.collections.Contains(svc))
            {
                compileOptions.collections.Add(svc);
            }
        }

        SaveCollections();
    }
    void SaveCollections()
    {
        List<string> guids = new List<string>();
        foreach (var collection in compileOptions.collections)
        {
            string path = AssetDatabase.GetAssetPath(collection);
            string guid = AssetDatabase.AssetPathToGUID(path);
            guids.Add(guid);
        }
        EditorPrefs.SetString("ShaderVariantConfig", string.Join(",", guids));
    }
}