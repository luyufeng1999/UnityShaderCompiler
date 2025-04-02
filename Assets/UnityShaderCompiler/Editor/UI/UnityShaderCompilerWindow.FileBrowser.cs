using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public partial class UnityShaderCompilerWindow
{
    private Vector2 fileBrowserCcrollPos;
    
    void DrawFileBrowserTab()
    {
        DrawPathHeader();
        DrawReportTool();
        DrawFileList();
    }

    enum Tools
    {
        aoc,
        malioc
    }
    Tools selectedTool = Tools.aoc;
    void DrawReportTool()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            string currentPath = outputPaths.shaderReportPath;
            EditorGUILayout.LabelField($"报告生成路径：{currentPath}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("工具:", GUILayout.Width(35));
            externalToolsSettings.selectedTool = (ExternalToolsSettings.OfflineCompiler)EditorGUILayout.EnumPopup(
                externalToolsSettings.selectedTool, GUILayout.Width(100));
            EditorGUILayout.LabelField("目标架构:", GUILayout.Width(55));
            switch (externalToolsSettings.selectedTool)
            {
                case ExternalToolsSettings.OfflineCompiler.Adreno:
                    externalToolsSettings.aocArch = (AOC.Architecture)EditorGUILayout.EnumPopup(
                        externalToolsSettings.aocArch, GUILayout.Width(60));
                    break;
                case ExternalToolsSettings.OfflineCompiler.Mali:
                    externalToolsSettings.maliocArch = (MaliOC.Architecture)EditorGUILayout.EnumPopup(
                        externalToolsSettings.maliocArch, GUILayout.Width(120));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (GUILayout.Button("生成报告", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                GenerateReport();
            }
            if (externalToolsSettings.selectedTool == ExternalToolsSettings.OfflineCompiler.Adreno && GUILayout.Button("导出csv报告", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                ExportReports();
            }
            if (GUILayout.Button("清空报告", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                ClearReports();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void DrawPathHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            string currentPath = outputPaths.shdaerPath;
            EditorGUILayout.LabelField($"当前路径：{currentPath}", EditorStyles.miniLabel);
            
            if (GUILayout.Button("刷新", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                outputPaths.LoadFileList();
            }
            if (GUILayout.Button("清空", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                outputPaths.ClearFileList();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void DrawFileList()
    {
        fileBrowserCcrollPos = EditorGUILayout.BeginScrollView(fileBrowserCcrollPos);
        {
            foreach (var filePath in outputPaths.shaderFiles)
            {
                DrawFileItem(filePath);
            }
        }
        EditorGUILayout.EndScrollView();
    }
    
    void DrawFileItem(string filePath)
    {
        // 创建自定义边框样式
        GUIStyle borderStyle = new GUIStyle(EditorStyles.helpBox)
        {
            margin = new RectOffset(2, 2, 2, 2),
            padding = new RectOffset(5, 5, 3, 3)
        };

        EditorGUILayout.BeginVertical(borderStyle);
        {
            EditorGUILayout.BeginHorizontal();
            {
                // 文件名显示（带图标）
                DrawFileNameSection(filePath);
                DrawActionButtons(filePath);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
    
    void DrawFileNameSection(string path)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        {
            EditorGUILayout.LabelField(Path.GetFileName(path), GUILayout.ExpandWidth(true));
        }
        EditorGUILayout.EndHorizontal();
    }
    
    void DrawActionButtons(string path)
    {
        string fileName = Path.GetFileName(path);
        string reportPath = Path.Combine(outputPaths.shaderReportPath, fileName);
        bool needDrawReport = File.Exists(reportPath);
        
        EditorGUILayout.BeginHorizontal(GUILayout.Width(needDrawReport ? 110 : 60));
        {
            if (GUILayout.Button("查看", EditorStyles.miniButtonLeft, GUILayout.Width(50)))
            {
                OpenFile(path);
            }
            
            if (needDrawReport && GUILayout.Button("报告", EditorStyles.miniButtonRight, GUILayout.Width(50)))
            {
                OpenFile(reportPath);
            }
            
        }
        EditorGUILayout.EndHorizontal();
    }
}