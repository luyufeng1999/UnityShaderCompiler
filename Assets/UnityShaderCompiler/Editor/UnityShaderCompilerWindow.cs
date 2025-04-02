using UnityEditor;
using UnityEngine;

public partial class UnityShaderCompilerWindow : EditorWindow
{
    private int selectedTab = 0;
    private readonly string[] tabNames = new string[] { "配置", "文件", "设置", "关于" };

    public static UnityShaderCompilerWindow _instance;
    
    [MenuItem("Tools/Unity Shader Compiler")]
    public static void ShowWindow()
    {
        if (_instance != null)
        {
            _instance.Focus();
            return;
        }
        _instance = GetWindow<UnityShaderCompilerWindow>("ShaderLab Compiler");
    }

    public UnityShaderCompilerWindow()
    {
        outputPaths = new OutputPathSettings();
        externalToolsSettings = new ExternalToolsSettings();
        compileOptions = new CompileOptions();
    }
    void OnEnable()
    {
        outputPaths.LoadData();
        externalToolsSettings.LoadData();
        compileOptions.LoadData();
        InitializeTool();
    }
    void OnGUI()
    {
        // 绘制选项卡工具栏
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);

        // 根据选择的选项卡显示不同内容
        switch (selectedTab)
        {
            case 0:
                DrawShaderVariantConfigTab();
                break;
            case 1:
                DrawFileBrowserTab();
                break;
            case 2:
                DrawExternalToolsSettingsTab();
                break;
            case 3:
                DrawAboutTab();
                break;
        }
    }
    
    void DrawAboutTab()
    {
        GUILayout.Label("关于本工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("版本: 1.0.0");
            GUILayout.Label("作者: 芦雨锋");
            GUILayout.Label("最后更新: 2025-04-02");
            EditorGUILayout.Space();
            
            if (GUILayout.Button("访问官网"))
            {
                Application.OpenURL("https://github.com/luyufeng1999/UnityShaderCompiler");
            }
        }
        EditorGUILayout.EndVertical();
    }
}
