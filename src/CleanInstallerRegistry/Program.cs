namespace Vurdalakov.CleanInstallerRegistry
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    using Microsoft.Win32;

    class Program
    {
        static void Main(String[] args)
        {
            var assembly = Assembly.GetCallingAssembly();
            Console.WriteLine($"{(assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute).Title} {assembly.GetName().Version} | {(assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute).Copyright} | https://github.com/vurdalakov/cleaninstallerregistry");

            var interactiveMode = false;
            var fixErrors = false;

            switch (args.Length)
            {
                case 0:
                    interactiveMode = true;
                    break;
                case 1:
                    interactiveMode = false;
                    switch (args[0].ToLower())
                    {
                        case "scan":
                            fixErrors = false;
                            break;
                        case "fix":
                            fixErrors = true;
                            break;
                        default:
                            Help();
                            return;
                    }
                    break;
                default:
                    Help();
                    return;
            }

            try
            {
                using (var installerUserData = new InstallerUserData())
                {
                    if (interactiveMode)
                    {
                        if (installerUserData.ProcessUserData(false))
                        {
                            Console.WriteLine("No problems were found.");
                        }
                        else if (AskYesNo("Problems were found. Do you want to fix them?"))
                        {
                            installerUserData.ProcessUserData(true);
                        }
                    }
                    else
                    {
                        installerUserData.ProcessUserData(fixErrors);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            Exit(0);
        }

        private static void Help()
        {
            Console.WriteLine("Fixes errors in Windows Installer Registry");
            Console.WriteLine("Usage:\r\n\tCleanInstallerRegistry.exe [scan|fix]");
            Console.WriteLine("Example (interactive mode):\r\n\tCleanInstallerRegistry.exe");
            Console.WriteLine("Example (scan only):\r\n\tCleanInstallerRegistry.exe scan");
            Console.WriteLine("Example (scan and fix):\r\n\tCleanInstallerRegistry.exe fix");
            Exit(1);
        }

        private static void Exit(Int32 exitCode)
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
            Environment.Exit(exitCode);
        }

        private static Boolean AskYesNo(String prompt)
        {
            Console.WriteLine(prompt);
            Console.WriteLine("Press 'y' or 'n'");
            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        return true;
                    case ConsoleKey.N:
                        return false;
                }
            }
        }
    }
}
