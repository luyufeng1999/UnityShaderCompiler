using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;


public abstract class ProcessBase
{
    protected ProcessStartInfo StartInfo { get; }

    public void OnPathChanged(string exePath)
    {
        StartInfo.FileName = exePath;
    }
    
    public string LastOutput { get; private set; } = string.Empty;
    public string LastError { get; private set; } = string.Empty;

    protected ProcessBase(string exePath)
    {
        StartInfo = new ProcessStartInfo()
        {
            FileName = exePath,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            UseShellExecute = false,
        };
    }
    protected int Run(params string[] args)
    {
        StartInfo.Arguments = BuildArguments(args);
        using var process = new Process { StartInfo = StartInfo };

        var output = new StringBuilder();
        var error = new StringBuilder();

        // 绑定数据接收事件
        process.OutputDataReceived += (s, e) => {
            if (!string.IsNullOrEmpty(e.Data)) output.AppendLine(e.Data);
        };
        
        process.ErrorDataReceived += (s, e) => {
            if (!string.IsNullOrEmpty(e.Data)) error.AppendLine(e.Data);
        };

        try
        {
            process.Start();
            
            // 开始异步读取（仍然需要异步读取机制）
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            process.WaitForExit(); // 阻塞直到退出

            // 确保获取完整输出
            LastOutput = output.ToString();
            LastError = error.ToString();

            return process.ExitCode;
        }
        catch (Exception ex)
        {
            LastError = $"Process execution failed: {ex.Message}";
            return -1;
        }
    }
    protected abstract string BuildArguments(string[] args);
}



public interface IOfflineCompiler
{
    public bool Compile(string filePath, string outputPath);
}

public interface IOfflineCompiler<TArchitecture> : IOfflineCompiler
{
    public void OnArchitectureChanged(TArchitecture architecture);
}

public class AOC : ProcessBase, IOfflineCompiler<AOC.Architecture>
{
    public enum Architecture
    {
        None,
        a608,
        a640,
        a650,
        a660,
        a690,
        a730,
        a740,
        a741,
        a750,
        a830
    }

    public enum API
    {
        None,
        OpenGLES,
        Vulkan,
        OpenCL,
        Direct3D
    }
    
    private Architecture architecture;
    private API api;
    public AOC(string exePath, Architecture architecture, API api) : base(exePath)
    {
        this.architecture = architecture;
        this.api = api;
    }
    
    protected override string BuildArguments(string[] args)
    {
        return $"{string.Join(" ", args)}";
    }
    
    public bool Compile(string filePath, string outputPath)
    {
        int exitCode = Run($"-arch={architecture.ToString()}", $"-api={api.ToString()}", $"\"{filePath}\"");
        switch (exitCode)
        {
            case 0:
                File.WriteAllText(outputPath, LastOutput);
                return true;
            case 1:
            case -1:
                return false;
            default:
                return false;
        }
    }
    
    public void OnArchitectureChanged(Architecture architecture)
    {
        this.architecture = architecture;
    }
    
    public void OnAPIChanged(API api)
    {
        this.api = api;
    }
}
public class MaliOC : ProcessBase, IOfflineCompiler<MaliOC.Architecture>
{
    public enum Architecture
    {
        None,
        Immortalis_G925,
        Immortalis_G720,
        Mali_G725,
        Mali_G720,
        Mali_G625,
        Mali_G620,
        Immortalis_G715,
        Mali_G715,
        Mali_G710,
        Mali_G615,
        Mali_G610,
        Mali_G510,
        Mali_G310,
        Mali_G78AE,
        Mali_G78,
        Mali_G77,
        Mali_G68,
        Mali_G57,
        Mali_G76,
        Mali_G72,
        Mali_G71,
        Mali_G52,
        Mali_G51,
        Mali_G31,
        Mali_T880,
        Mali_T860,
        Mali_T830,
        Mali_T820,
        Mali_T760,
        Mali_T720,
    }
    

    private static Dictionary<Architecture, string> architectures = new Dictionary<Architecture, string>()
    {
        { Architecture.Immortalis_G925, "Immortalis-G925"},
        { Architecture.Immortalis_G720, "Immortalis-G720"},
        { Architecture.Mali_G725, "Mali-G725"},
        { Architecture.Mali_G720, "Mali-G720"},
        { Architecture.Mali_G625, "Mali-G625"},
        { Architecture.Mali_G620, "Mali-G620"},
        { Architecture.Immortalis_G715, "Immortalis-G715"},
        { Architecture.Mali_G715, "Mali-G715"},
        { Architecture.Mali_G710, "Mali-G710"},
        { Architecture.Mali_G615, "Mali-G615"},
        { Architecture.Mali_G610, "Mali-G610"},
        { Architecture.Mali_G510, "Mali-G510"},
        { Architecture.Mali_G310, "Mali-G310"},
        { Architecture.Mali_G78AE, "Mali-G78AE"},
        { Architecture.Mali_G78, "Mali-G78"},
        { Architecture.Mali_G77, "Mali-G77"},
        { Architecture.Mali_G68, "Mali-G68"},
        { Architecture.Mali_G57, "Mali-G57"},
        { Architecture.Mali_G76, "Mali-G76"},
        { Architecture.Mali_G72, "Mali-G72"},
        { Architecture.Mali_G71, "Mali-G71"},
        { Architecture.Mali_G52, "Mali-G52"},
        { Architecture.Mali_G51, "Mali-G51"},
        { Architecture.Mali_G31, "Mali-G31"},
        { Architecture.Mali_T880, "Mali-T880"},
        { Architecture.Mali_T860, "Mali-T860"},
        { Architecture.Mali_T830, "Mali-T830"},
        { Architecture.Mali_T820, "Mali-T820"},
        { Architecture.Mali_T760, "Mali-T760"},
        { Architecture.Mali_T720, "Mali-T720"},
    };


    private Architecture architecture;

    public MaliOC(string exePath, Architecture architecture) : base(exePath)
    {
        this.architecture = architecture;
    }

    public bool Compile(string filePath, string outputPath)
    {
        if (!architectures.TryGetValue(architecture, out string strArchitecture))
        {
            return false;
        }
        int exitCode = Run($"-c {strArchitecture}", $"\"{filePath}\"");
        switch (exitCode)
        {
            case 0:
                File.WriteAllText(outputPath, LastOutput);
                return true;
            case 1:
            case -1:
                return false;
            default:
                return false;
        }
    }
    public void OnArchitectureChanged(Architecture architecture)
    {
        this.architecture = architecture;
    }

    protected override string BuildArguments(string[] args)
    {
        return $"{string.Join(" ", args)}";
    }
}

public class ReportParser : ProcessBase
{
    public ReportParser(string exePath) : base(exePath) { }

    public bool Parser(string reportPath, string outputPath)
    {
        int exitCode = Run(reportPath, $"--output {outputPath}");
        switch (exitCode)
        {
            case 0:
                return true;
            case 1:
            case -1:
                return false;
            default:
                return false;
        }
    }
    protected override string BuildArguments(string[] args)
    {
        return $"{string.Join(" ", args)}";
    }
}