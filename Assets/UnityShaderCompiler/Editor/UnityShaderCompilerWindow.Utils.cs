using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class UnityShaderCompilerWindow
{
    #region 通用方法

    void OpenFile(string filePath)
    {
        string relativePath = GetRelativePath(filePath);
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
        
        if (obj != null)
        {
            // 使用Unity内置方式打开文件
            AssetDatabase.OpenAsset(obj);
        }
        else
        {
            Debug.LogError($"无法打开文件：{filePath}");
        }
    }
    
    string GetRelativePath(string fullPath)
    {
        if (fullPath.StartsWith(Application.dataPath))
        {
            return "Assets" + fullPath.Substring(Application.dataPath.Length);
        }
        return fullPath;
    }

    #endregion

    
    #region 通用控件
    private void DrawDownloadLink(string label, string url)
    {
        // 创建超链接样式
        GUIStyle linkStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            normal = { textColor = new Color(0.1f, 0.3f, 0.8f) },
            hover = { textColor = new Color(0.2f, 0.5f, 1f) },
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 5, 0, 0)
        };

        // 绘制可点击标签
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent(label)).x + 5));
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
    
        bool clicked = GUI.Button(rect, $"{label}", linkStyle);
    
        if (clicked)
        {
            Application.OpenURL(url);
        }
    }
    private void DrawLinkSeparator()
    {
        EditorGUILayout.LabelField("|", GUILayout.Width(10));
    }
    string DrawPathField(string label, string path, string dialogTitle, string extension, bool isFolder = false)
    {
        // 显示标签
        EditorGUILayout.LabelField(label, GUILayout.Width(200));
        EditorGUILayout.BeginHorizontal();
        {
            // 显示路径字段
            string newPath = EditorGUILayout.TextField(path);
            if (newPath != path)
            {
                path = newPath;
            }
            // 路径选择按钮
            if (GUILayout.Button("浏览...", GUILayout.Width(60)))
            {
                string selectedPath = isFolder ? 
                    EditorUtility.OpenFolderPanel(dialogTitle, "", "") :
                    EditorUtility.OpenFilePanel(dialogTitle, "", extension);

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    path = selectedPath;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        // 显示路径存在状态
        bool isValid = isFolder ? Directory.Exists(path) : File.Exists(path);
        EditorGUILayout.LabelField(isValid ? "✓ 路径有效" : "⚠ 路径不存在", 
            isValid ? EditorStyles.helpBox : 
                EditorStyles.wordWrappedLabel);
        return path;
    }
    
    #endregion
    
    

}