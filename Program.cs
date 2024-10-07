using Microsoft.Win32;

namespace CheckSEP;

internal class Program
{
    static class ExitCode
    {
        public const int OK = 0;
        public const int Warning = 1;
        public const int Critical = 2;
        public const int Unknown = 3;
    }

    static void Main(string[] args)
    {
        int warnLevel = 2;
        int critLevel = 4;

        // Parse Arguments for warning and critical levels
        foreach (string arg in args)
        {
            if (arg.StartsWith("-w:") && int.TryParse(arg[3..], out int parsedWarnLevel))
            {
                warnLevel = parsedWarnLevel;
            }
            else if (arg.StartsWith("-c:") && int.TryParse(arg[3..], out int parsedCritLevel))
            {
                critLevel = parsedCritLevel;
            }
        }

        // Registry path to DefVersion key
        // Previous x64 versions of SEP use:
        // SOFTWARE\Wow6432Node\Symantec\SharedDefs\DefWatch
        string keyPath = @"SOFTWARE\Symantec\SharedDefs\DefWatch";

        // Query Registry for the definition value
        byte[] defVersion;
        try
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyPath);
            if (key == null)
            {
                Console.WriteLine("UNKNOWN - Unable to read Definitions from the Registry");
                Environment.Exit(ExitCode.Unknown);
                return;
            }

            defVersion = key.GetValue("DefVersion") as byte[] ?? [];
            if (defVersion.Length < 18)
            {
                Console.WriteLine("UNKNOWN - DefVersion not found or invalid in the Registry");
                Environment.Exit(ExitCode.Unknown);
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UNKNOWN - Error accessing the Registry: {ex.Message}");
            Environment.Exit(ExitCode.Unknown);
            return;
        }

        // Generate output from the registry value
        int year = BitConverter.ToUInt16(defVersion, 0);
        int month = BitConverter.ToUInt16(defVersion, 2);
        int day = BitConverter.ToUInt16(defVersion, 6);
        int majorVersion = BitConverter.ToUInt16(defVersion, 16);

        string sepVersion = $"{year}-{month:D2}-{day:D2} rev. {majorVersion}";
        DateTime defDate = new(year, month, day);
        int dateDifference = (DateTime.Now - defDate).Days;

        // Output current version and definition age as Performance data
        Console.WriteLine($"Current Definitions: {sepVersion} which are {dateDifference} days old |age={dateDifference}");

        // Exit with appropriate status
        if (dateDifference > critLevel)
        {
            Environment.Exit(ExitCode.Critical);
        }
        else if (dateDifference > warnLevel)
        {
            Environment.Exit(ExitCode.Warning);
        }
        else
        {
            Environment.Exit(ExitCode.OK);
        }
    }
}
