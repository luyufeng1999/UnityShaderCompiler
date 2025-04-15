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