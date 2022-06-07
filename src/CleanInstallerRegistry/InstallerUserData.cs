namespace Vurdalakov.CleanInstallerRegistry
{
    using System;
    using System.Linq;

    using Microsoft.Win32;

    internal class InstallerUserData : IDisposable
    {
        private const String UserDataKeyName = @"Software\Microsoft\Windows\CurrentVersion\Installer\UserData";

        private readonly RegistryKey _root;

        public InstallerUserData() => this._root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

        public void Dispose() => this._root.Dispose();

        public Boolean ProcessUserData(Boolean fixErrors)
        {
            var noProblemsFound = true;

            using (var userData = this._root.OpenSubKey(UserDataKeyName))
            {
                foreach (var sid in userData.GetSubKeyNames())
                {
                    noProblemsFound &= this.ProcessSid(userData, sid, fixErrors);
                }
            }

            return noProblemsFound;
        }

        private Boolean ProcessSid(RegistryKey userData, String sid, Boolean fixErrors)
        {
            Console.WriteLine($"--- '{sid}': {(fixErrors ? "cleaning" : "scanning")} Windows Installer components");

            var noProblemsFound = true;
            var componentsCleaned = 0;
            var componentsDeleted = 0;

            var productNames = this.GetProductNames(userData, sid);

            using (var components = userData.OpenSubKey($@"{sid}\Components", fixErrors))
            {
                var componentNames = components.GetSubKeyNames();

                foreach (var componentName in componentNames)
                {
                    using (var component = components.OpenSubKey(componentName, fixErrors))
                    {
                        var valueNames = component.GetValueNames();

                        foreach (var valueName in valueNames)
                        {
                            if (valueName.Equals("00000000000000000000000000000000"))
                            {
                                continue;
                            }

                            if (!productNames.Contains(valueName, StringComparer.InvariantCultureIgnoreCase))
                            {
                                noProblemsFound = false;
                                Console.WriteLine($"Deleting '{valueName}' from '{componentName}' ('{component.GetValue(valueName)}')");

                                if (fixErrors)
                                {
                                    component.DeleteValue(valueName);
                                    componentsCleaned++;
                                }
                            }
                        }
                    }

                    var deleteComponent = false;

                    using (var component = components.OpenSubKey(componentName))
                    {
                        var valueNames = component.GetValueNames();
                        deleteComponent = 0 == productNames.Length;
                    }

                    if (deleteComponent)
                    {
                        noProblemsFound = false;
                        Console.WriteLine($"Deleting '{componentName}'");

                        if (fixErrors)
                        {
                            components.DeleteSubKey(componentName);
                            componentsDeleted++;
                        }
                    }
                }

                Console.Write($"--- '{sid}': ");
                if (fixErrors)
                {
                    Console.WriteLine($"{componentsCleaned} components cleaned and {componentsDeleted} components deleted out of {componentNames.Length} components total");
                }
                else if (noProblemsFound)
                {
                    Console.WriteLine("No problems were found");
                }
                else
                {
                    Console.WriteLine($"Problems were found");
                }

                return noProblemsFound;
            }
        }

        private String[] GetProductNames(RegistryKey userData, String sid)
        {
            using (var products = userData.OpenSubKey($@"{sid}\Products"))
            {
                return products.GetSubKeyNames();
            }
        }
    }
}
