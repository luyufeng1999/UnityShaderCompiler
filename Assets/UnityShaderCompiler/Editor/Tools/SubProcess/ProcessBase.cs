using System;
using System.Diagnostics;
using System.Text;
public abstract class ProcessBase
{
    protected ProcessStartInfo StartInfo { get; }
    
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

