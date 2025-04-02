using UnityEditor;
using UnityEngine;

public partial class UnityShaderCompilerWindow
{
    public void DrawExternalToolsSettingsTab()
    {
        DrawOfflineCompilerSettings();
        EditorGUILayout.Space();
        DrawParserSettings();
        EditorGUILayout.Space();
        DrawOutputPathSettingsTab();
        EditorGUILayout.Space();
        AddDownloadLinks();
    }
    public void DrawOfflineCompilerSettings()
    {
        GUILayout.Label("编译器设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("box");
        {
            externalToolsSettings.aocPath = DrawPathField("Adreno Offline Compiler路径", externalToolsSettings.aocPath, "选择aoc.exe", "exe");
            externalToolsSettings.maliocPath = DrawPathField("Mali Offline Compiler路径", externalToolsSettings.maliocPath, "选择malioc.exe", "exe");
        }
        EditorGUILayout.EndVertical();
    }
    
    public void DrawParserSettings()
    {
        GUILayout.Label("解析器设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        {
            externalToolsSettings.reportParserPath = DrawPathField("Adreno报告解析器路径", externalToolsSettings.reportParserPath, "选择解析器", "exe");
        }
        EditorGUILayout.EndVertical();
    }
    

    public void DrawOutputPathSettingsTab()
    {
        GUILayout.Label("输出路径设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        {
            outputPaths.shdaerPath = DrawPathField("Shader文件输出路径", outputPaths.shdaerPath, "选择输出路径", null, true);
            outputPaths.shaderReportPath = DrawPathField("Shader报告输出路径", outputPaths.shaderReportPath, "选择输出路径", null, true);
        }
        EditorGUILayout.EndVertical();
    }
    private void AddDownloadLinks()
    {
        GUILayout.FlexibleSpace();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            DrawDownloadLink("Adreno Offline Compiler", "https://softwarecenter.qualcomm.com/#/catalog/item/Adreno_GPU_Offline_Compiler");
            DrawLinkSeparator();
            DrawDownloadLink("Mali Offline Compiler", "https://developer.arm.com/Tools%20and%20Software/Mali%20Offline%20Compiler");
        }
        EditorGUILayout.EndHorizontal();
    }
}