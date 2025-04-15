using System.Collections.Generic;

public class MaliOfflineCompiler : ProcessBase
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
    
    public struct CompileReport
    {
        public string reportData;
        public Architecture architecture;
        public string errorMessage;
        
        public static CompileReport Null => new CompileReport
        {
            reportData = null,
            architecture = Architecture.None,
        };
    }
    
    public MaliOfflineCompiler(string exePath) : base(exePath)
    {
        
    }

    protected override string BuildArguments(string[] args)
    {
        return $"{string.Join(" ", args)}";
    }
    
    
    public CompileReport Compile(string shaderFilePath, Architecture architecture)
    {
        if (!architectures.TryGetValue(architecture, out string strArchitecture))
        {
            return new CompileReport
            {
                reportData = null,
                architecture = Architecture.None,
                errorMessage = "Invalid Architecture"
            };
        }
        int exitCode = Run($"-c {strArchitecture}", $"\"{shaderFilePath}\"");
        switch (exitCode)
        {
            case 0:
            case 1:
                return new CompileReport
                {
                    reportData = LastOutput,
                    architecture = architecture,
                    errorMessage = null
                };
            case -1:
                return new CompileReport
                {
                    reportData = null,
                    architecture = architecture,
                    errorMessage = LastOutput
                };
            default:
                return new CompileReport
                {
                    reportData = null,
                    architecture = Architecture.None,
                    errorMessage = LastOutput
                };
        }
    }
}