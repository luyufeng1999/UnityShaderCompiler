using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class OutputPathSettings : IUnityShaderCompilerSettings
{
    public const string SHADER_PATH_KEYS = "OutputPaths_Shader";
    public const string SHADER_REPORT_PATH_KEYS = "OutputPaths_ShaderReport";
    
    
    public Action<string> OnShaderPathChange;
    public Action<string> OnShaderReportPathChange;
    
    private string _shdaerPath;
    public string shdaerPath
    {
        get
        {
            return _shdaerPath;
        }
        set
        {
            if (_shdaerPath != value)
            {
                _shdaerPath = value;
                EditorPrefs.SetString(SHADER_PATH_KEYS, _shdaerPath);
                OnShaderPathChange(_shdaerPath);
            }
        }
    }

    private string _shaderReportPath;
    public string shaderReportPath
    {
        get
        {
            return _shaderReportPath;
        }
        set
        {
            if (_shaderReportPath != value)
            {
                _shaderReportPath = value;
                EditorPrefs.SetString(SHADER_REPORT_PATH_KEYS, _shaderReportPath);
                OnShaderReportPathChange(_shaderReportPath);
            }
        }
    }

    public List<string> shaderFiles { get; private set; }

    public void LoadData()
    {
        _shdaerPath = EditorPrefs.GetString(SHADER_PATH_KEYS, "");
        _shaderReportPath = EditorPrefs.GetString(SHADER_REPORT_PATH_KEYS, "");
        LoadFileList();
    }

    public void LoadFileList()
    {
        shaderFiles = new List<string>();
        string path = _shdaerPath;

        if (Directory.Exists(path))
        {
            // 获取所有文件（排除.meta文件）
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if (!file.EndsWith(".meta"))
                {
                    shaderFiles.Add(file);
                }
            }
        }
        else
        {
            Debug.LogWarning($"路径不存在：{path}");
        }
    }

    public void ClearFileList()
    {
        foreach (string file in shaderFiles)
        {
            File.Delete(file);  // 删除文件
        }
        shaderFiles.Clear();
        AssetDatabase.Refresh();
    }
}