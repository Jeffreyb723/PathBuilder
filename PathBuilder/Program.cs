using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace PathBuilder
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string[]> applicationCommands = args.Select(s => s.Split('='))
                .ToDictionary(s => s[0], s => s[1].Length > 0 ? s[1].Split(',') : null);

            RegistryKey registryKey =
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");

            string path = applicationCommands.ContainsKey("PATH")
                ? applicationCommands["PATH"][0]
                : registryKey?.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames).ToString() ?? "";

            if (applicationCommands.ContainsKey("REMOVE"))
            {
                path = applicationCommands["REMOVE"]
                    .Aggregate(path, (current, remove) => current?.Replace($"{remove.Trim()};", ""));
                path = applicationCommands["REMOVE"]
                    .Aggregate(path, (current, remove) => current?.Replace($";{remove.Trim()}", ""));
            }

            if (applicationCommands.ContainsKey("ADD"))
            {
                path = applicationCommands["ADD"].Aggregate(path, (current, add) => $"{current};{add}");
            }

            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Machine);
        }
    }
}
