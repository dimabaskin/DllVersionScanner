using CommandLine.Text;
using CommandLine;
using System.Diagnostics;
using System.Reflection;

namespace DllVersionScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string pathlist = "";
            string version = "";
            int daysold = 0;
            //string pathlist = @"C:\Xerox, C:\swsetup, C:\Dima, C:\Windows\SysWOW64";
            //string version = "3.0";
            //int daysold = 10;

            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options =>
            {
                if(String.IsNullOrEmpty(options.PathDLL))
                {
                    Console.WriteLine("Argument Path do not Exist");
                    return;
                }
                else
                {
                    pathlist = @options.PathDLL;
                }
                if(String.IsNullOrEmpty(options.VersionDLL))
                {
                    Console.WriteLine("Argument DLL Version do not Exist");
                    return;
                }
                else
                {
                    version = options.VersionDLL;
                }
                if (options.AgeOfDLLFile == 0)
                {
                    Console.WriteLine("Argument DLL age can not be null");
                    return;
                }
                else
                {
                    daysold = options.AgeOfDLLFile;
                }


            });

            DateTime dateNow= DateTime.Now;
            string outputfileName = $"output_{dateNow.Day}_{dateNow.Month}_{dateNow.Year}-{dateNow.Hour}-{dateNow.Minute}-{dateNow.Second}.txt";
            string outputfile = Directory.GetCurrentDirectory() + @"\" + outputfileName;
            Version versionToCompare = new Version(version);
            DateTime datetoCompare = dateNow.AddDays( - daysold);
            string[] patharray = pathlist.Split(",");
            using (StreamWriter file = new StreamWriter(outputfile))
            {
                foreach (string path in patharray)
                {
                    string pathToExecuteSearch = path.TrimStart().TrimEnd();

                    file.WriteLine($"Path for DLL Search: {pathToExecuteSearch}");
                    if (Directory.Exists(pathToExecuteSearch))
                        try
                        {
                            List<string> validFiles = new List<string>();
                            List<string> notValidFiles = new List<string>();
                            List<string> notValidFormat = new List<string>();
                            string[] fileList = Directory.GetFiles(pathToExecuteSearch, "*.dll", SearchOption.AllDirectories);
                            {
                                foreach (string filename in fileList)
                                {
                                    string fileVersionStringFormat = FileVersionInfo.GetVersionInfo(filename).FileVersion;
                                    if (String.IsNullOrEmpty(fileVersionStringFormat) || fileVersionStringFormat.Contains(","))
                                    {
                                        notValidFormat.Add($"{filename} Version: {FileVersionInfo.GetVersionInfo(filename).FileVersion} Created on: {File.GetCreationTime(filename)} | Invalid Version Format");
                                    }
                                    else
                                    {
                                        Version fileVersion = new(fileVersionStringFormat.Split(" ").FirstOrDefault());
                                        if (fileVersion > versionToCompare && File.GetCreationTime(filename).Date < datetoCompare)
                                        { 
                                            notValidFiles.Add($"{filename} Version: {FileVersionInfo.GetVersionInfo(filename).FileVersion} Created on: {File.GetCreationTime(filename)} | NOT Valid Due to DLL Version & Date"); 
                                        }
                                        else if(fileVersion > versionToCompare)
                                        {
                                            notValidFiles.Add($"{filename} Version: {FileVersionInfo.GetVersionInfo(filename).FileVersion} Created on: {File.GetCreationTime(filename)} | NOT Valid Due to DLL Version");
                                        }
                                        else if (File.GetCreationTime(filename).Date < datetoCompare)
                                        {
                                            notValidFiles.Add($"{filename} Version: {FileVersionInfo.GetVersionInfo(filename).FileVersion} Created on: {File.GetCreationTime(filename)} | NOT Valid Due to Date");
                                        }
                                        else
                                        { 
                                            validFiles.Add($"{filename} Version: {FileVersionInfo.GetVersionInfo(filename).FileVersion} Created on: {File.GetCreationTime(filename)} | Valid"); 
                                        }
                                    }
                                    
                                }
                                if(notValidFiles.Count > 0)
                                {
                                    file.WriteLine("NOT Valid DLL list:");
                                    foreach (string inValidFile in notValidFiles)
                                    {
                                        file.WriteLine(inValidFile);
                                    }
                                }
                                if(validFiles.Count > 0)
                                {
                                    file.WriteLine("Valid DLL list:");
                                    foreach (string validFile in validFiles)
                                    {
                                        file.WriteLine(validFile);
                                    }
                                }
                                if(notValidFormat.Count > 0)
                                {
                                    file.WriteLine("Invalid Version Format DLL list:");
                                    foreach (string notvalidFile in notValidFormat)
                                    {
                                        file.WriteLine(notvalidFile);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(UnauthorizedAccessException))
                            { file.WriteLine(ex.Message); }
                        }
                    else
                        file.WriteLine($"Directory {path} do not Exist.");
                }
            }
        }
    }

    class Options
    {
        [Option('v', "version", Required = true, HelpText = "Input DLL version for validation.")]
        public string VersionDLL { get; set; }

        [Option('a', "age", Required = true, HelpText = "Input how old DLL file can be in days.")]
        
        public int AgeOfDLLFile { get; set; }

        [Option('p', "path", Required = true, HelpText = "Input path or multiple pathes separeted by comma .")]

        public string PathDLL { get; set; }

    }
}