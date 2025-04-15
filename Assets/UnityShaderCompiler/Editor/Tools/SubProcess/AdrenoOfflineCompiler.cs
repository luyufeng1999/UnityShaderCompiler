using System.IO;

public class AdrenoOfflineCompiler : ProcessBase
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

    public struct CompileReport
    {
        public string reportData;
        public Architecture architecture;
        public string errorMessage;
        
        public static CompileReport Null => new CompileReport
        {
            reportData = null,
            architecture = Architecture.None,
            errorMessage = null
        };
    }
    
    public AdrenoOfflineCompiler(string exePath) : base(exePath) { }

    protected override string BuildArguments(string[] args)
    {
        return $"{string.Join(" ", args)}";
    }
    
    public CompileReport Compile(string shaderFilePath, Architecture architecture)
    {
        int exitCode = Run($"-arch={architecture.ToString()}", $"\"{shaderFilePath}\"");
        switch (exitCode)
        {
            case 0:
                return new CompileReport
                {
                    reportData = LastOutput,
                    architecture = architecture,
                    errorMessage = null
                };
            case 1:
            case -1:
                return new CompileReport
                {
                    reportData = null,
                    architecture = architecture,
                    errorMessage = LastError
                };
            default:
                return new CompileReport
                {
                    reportData = null,
                    architecture = Architecture.None,
                    errorMessage = LastError
                };
        }
    }
}