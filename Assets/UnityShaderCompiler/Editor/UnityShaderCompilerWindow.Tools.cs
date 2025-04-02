using System;
using System.IO;
using UnityEditor;

public partial class UnityShaderCompilerWindow
{
    private AOC aoc;
    private MaliOC malioc;
    private ShaderLabLabCompiler compiler;
    private ReportParser reportParser;
    private void InitializeTool()
    {
        aoc = new AOC(externalToolsSettings.aocPath, AOC.Architecture.a608, AOC.API.Vulkan);
        malioc = new MaliOC(externalToolsSettings.maliocPath, MaliOC.Architecture.Mali_G76);
        compiler = new ShaderLabLabCompiler(compileOptions.selectedShaderPlatform, compileOptions.selectedBuildTarget, outputPaths.shdaerPath);
        reportParser = new ReportParser(externalToolsSettings.reportParserPath);
        externalToolsSettings.OnAOCPathChanged += aoc.OnPathChanged;
        externalToolsSettings.OnMaliOCPathChanged += malioc.OnPathChanged;
        externalToolsSettings.OnReportParserPathChanged += reportParser.OnPathChanged;
        compileOptions.OnShaderCompilerPlatformChange += compiler.OnShaderCompilerPlatformChange;
        compileOptions.OnBuildTargetChange += compiler.OnBuildTargetChange;
        outputPaths.OnShaderPathChange += compiler.OnOutputPathChange;
        
    }
    private void GenerateReport()
    {
        IOfflineCompiler offlineCompiler; 
        switch (externalToolsSettings.selectedTool)
        {
            case ExternalToolsSettings.OfflineCompiler.Adreno:
                offlineCompiler = aoc;
                break;
            case ExternalToolsSettings.OfflineCompiler.Mali:
                offlineCompiler = malioc;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var shaderFile in outputPaths.shaderFiles)
        {
            string fileName = Path.GetFileName(shaderFile);
            string reportPath = Path.Combine(outputPaths.shaderReportPath, fileName);
            offlineCompiler.Compile(shaderFile, reportPath);
        }
        AssetDatabase.Refresh();
    }

    private void ExportReports()
    {
        string path = EditorUtility.SaveFilePanel("选择保存位置", "", "shader_report_stats", "csv");
        reportParser.Parser(outputPaths.shaderReportPath, path);
        AssetDatabase.Refresh();
    }

    private void ClearReports()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(outputPaths.shaderReportPath);
        var files = directoryInfo.GetFiles();
        foreach (var file in files)
        {
            File.Delete(file.FullName);
        }
        AssetDatabase.Refresh();
    }

    private void CompileVariantCollections()
    {
        foreach (var collection in compileOptions.collections)
        {
            compiler.CompileVariantCollection(collection);    
        }
        outputPaths.LoadFileList();
        AssetDatabase.Refresh();
    }
}
