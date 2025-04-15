using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class UnityShaderCompilerWindow
{
    private ReportParser reportParser;

    #region 编译Shader
    private void CompileVariantCollections()
    {
        foreach (var collection in compileOptions.collections)
        {
            ShaderLabCompiler.CompileVariantCollection(collection, compileOptions.selectedShaderPlatform, compileOptions.selectedBuildTarget, out var compileResults);
            foreach (var compileResult in compileResults)
            {
                if (compileResult.compileInfo.Success)
                {
                    File.WriteAllBytes(Path.Combine(outputPaths.shdaerPath, compileResult.variantInfo.GetShaderFileName()), compileResult.compileInfo.ShaderData);    
                }
            }
        }
        outputPaths.LoadFileList();
        AssetDatabase.Refresh();
    }
    #endregion
    
    

    #region 生成报告
    private void GenerateReport()
    {
        switch (externalToolsSettings.selectedTool)
        {
            case ExternalToolsSettings.OfflineCompiler.Adreno:
                GenerateAOCReport();
                break;
            case ExternalToolsSettings.OfflineCompiler.Mali:
                GenerateMaliOCReport();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        AssetDatabase.Refresh();
    }

    private void GenerateAOCReport()
    {
        AdrenoOfflineCompiler aoc = new AdrenoOfflineCompiler(externalToolsSettings.aocPath);
        foreach (var shaderFile in outputPaths.shaderFiles)
        {
            string fileName = Path.GetFileName(shaderFile);
            string reportPath = Path.Combine(outputPaths.shaderReportPath, fileName);
            AdrenoOfflineCompiler.CompileReport report = aoc.Compile(shaderFile, externalToolsSettings.aocArch);
            if (report.errorMessage != null)
            {
                Debug.LogError(report.errorMessage);
                continue;
            }
            File.WriteAllText(reportPath, report.reportData);
        }
    }

    private void GenerateMaliOCReport()
    {
        MaliOfflineCompiler malioc = new MaliOfflineCompiler(externalToolsSettings.maliocPath);
        foreach (var shaderFile in outputPaths.shaderFiles)
        {
            string fileName = Path.GetFileName(shaderFile);
            string reportPath = Path.Combine(outputPaths.shaderReportPath, fileName);
            MaliOfflineCompiler.CompileReport report = malioc.Compile(shaderFile, externalToolsSettings.maliocArch);
            if (report.errorMessage != null)
            {
                Debug.LogError(report.errorMessage);
                continue;
            }
            File.WriteAllText(reportPath, report.reportData);
        }
    }
    

    #endregion

    #region 导出报告
    private void ExportReports()
    {
        string path = EditorUtility.SaveFilePanel("选择保存位置", "", "shader_report_stats", "csv");
        reportParser = new ReportParser(externalToolsSettings.reportParserPath);
        reportParser.Parser(outputPaths.shaderReportPath, path);
        AssetDatabase.Refresh();
    }
    #endregion
    
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
}
