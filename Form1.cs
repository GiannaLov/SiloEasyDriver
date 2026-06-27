using Microsoft.Win32;

namespace SiloEasyDriver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            try
            {
                string? steamPath = GetSteamPath();
                if (steamPath == null)
                {
                    MessageBox.Show("Steam installation path not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string? gamePath = FindSiloGame(steamPath);
                if (gamePath == null)
                {
                    string debugInfo = GetDebugInfo(steamPath);
                    MessageBox.Show($"Silo game not found! Please make sure the game is installed.\n\nDebug Info:\n{debugInfo}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string targetDir = Path.Combine(gamePath, "driver", "slime_locomotion", "bin", "win64");
                if (!Directory.Exists(targetDir))
                {
                    MessageBox.Show($"Target directory not found: {targetDir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string sourceDll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SiloDriver.dll");
                if (!File.Exists(sourceDll))
                {
                    MessageBox.Show($"Source DLL file not found: {sourceDll}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string targetDll = Path.Combine(targetDir, "driver_slime_locomotion.dll");
                string backupDll = Path.Combine(targetDir, "driver_slime_locomotion.bak");

                if (File.Exists(targetDll) && !File.Exists(backupDll))
                {
                    File.Copy(targetDll, backupDll, true);
                }

                File.Copy(sourceDll, targetDll, true);

                MessageBox.Show($"EasyDriver installed successfully!\nTarget path: {targetDll}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Installation failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string? GetSteamPath()
        {
            using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
            {
                if (key?.GetValue("InstallPath") is string path && Directory.Exists(path))
                {
                    return path;
                }
            }

            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                if (key?.GetValue("SteamPath") is string path && Directory.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        private string? FindSiloGame(string steamPath)
        {
            List<string> libraryPaths = new List<string>();

            string defaultSteamApps = Path.Combine(steamPath, "steamapps");
            if (Directory.Exists(defaultSteamApps))
            {
                libraryPaths.Add(steamPath);
            }

            string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(libraryFoldersPath))
            {
                try
                {
                    string content = File.ReadAllText(libraryFoldersPath);
                    int pos = 0;
                    while (pos < content.Length)
                    {
                        int pathIdx = content.IndexOf("\"path\"", pos);
                        if (pathIdx < 0)
                            break;

                        int valueStart = content.IndexOf('"', pathIdx + 6);
                        if (valueStart < 0)
                            break;

                        int valueEnd = content.IndexOf('"', valueStart + 1);
                        if (valueEnd < 0)
                            break;

                        string libPath = content.Substring(valueStart + 1, valueEnd - valueStart - 1).Replace("\\\\", "\\");
                        if (Directory.Exists(libPath) && !libraryPaths.Contains(libPath))
                        {
                            libraryPaths.Add(libPath);
                        }

                        pos = valueEnd + 1;
                    }
                }
                catch
                {
                }
            }

            foreach (string libPath in libraryPaths)
            {
                string steamAppsPath = Path.Combine(libPath, "steamapps");
                if (!Directory.Exists(steamAppsPath))
                    continue;

                string commonPath = Path.Combine(steamAppsPath, "common");
                if (Directory.Exists(commonPath))
                {
                    string gamePath = Path.Combine(commonPath, "Slime Locomotion");
                    if (Directory.Exists(gamePath))
                    {
                        return gamePath;
                    }
                }

                if (Directory.Exists(commonPath))
                {
                    foreach (string manifestFile in Directory.GetFiles(steamAppsPath, "appmanifest_*.acf"))
                    {
                        try
                        {
                            string content = File.ReadAllText(manifestFile);
                            string? gameName = ExtractValue(content, "name");
                            if (gameName == "Slime Locomotion")
                            {
                                string? installDir = ExtractValue(content, "installdir");
                                if (!string.IsNullOrEmpty(installDir))
                                {
                                    string fullGamePath = Path.Combine(commonPath, installDir);
                                    if (Directory.Exists(fullGamePath))
                                    {
                                        return fullGamePath;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return null;
        }

        private string? ExtractValue(string content, string key)
        {
            string searchKey = $"\"{key}\"";
            int keyIdx = content.IndexOf(searchKey);
            if (keyIdx < 0)
                return null;

            int valueStart = content.IndexOf('\"', keyIdx + searchKey.Length);
            if (valueStart < 0)
                return null;

            int valueEnd = content.IndexOf('\"', valueStart + 1);
            if (valueEnd < 0)
                return null;

            return content.Substring(valueStart + 1, valueEnd - valueStart - 1);
        }

        private string GetDebugInfo(string steamPath)
        {
            List<string> debugLines = new List<string>();
            debugLines.Add($"Steam Path: {steamPath}");

            List<string> libraryPaths = new List<string>();
            string defaultSteamApps = Path.Combine(steamPath, "steamapps");
            if (Directory.Exists(defaultSteamApps))
            {
                libraryPaths.Add(steamPath);
            }

            string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(libraryFoldersPath))
            {
                try
                {
                    string content = File.ReadAllText(libraryFoldersPath);
                    int pos = 0;
                    while (pos < content.Length)
                    {
                        int pathIdx = content.IndexOf("\"path\"", pos);
                        if (pathIdx < 0)
                            break;

                        int valueStart = content.IndexOf('"', pathIdx + 6);
                        if (valueStart < 0)
                            break;

                        int valueEnd = content.IndexOf('"', valueStart + 1);
                        if (valueEnd < 0)
                            break;

                        string libPath = content.Substring(valueStart + 1, valueEnd - valueStart - 1).Replace("\\\\", "\\");
                        if (Directory.Exists(libPath) && !libraryPaths.Contains(libPath))
                        {
                            libraryPaths.Add(libPath);
                        }

                        pos = valueEnd + 1;
                    }
                }
                catch { }
            }

            debugLines.Add($"Libraries found: {libraryPaths.Count}");
            foreach (string libPath in libraryPaths)
            {
                debugLines.Add($"  - {libPath}");
                string commonPath = Path.Combine(libPath, "steamapps", "common");
                if (Directory.Exists(commonPath))
                {
                    try
                    {
                        string[] games = Directory.GetDirectories(commonPath);
                        debugLines.Add($"    Games in common folder ({games.Length}):");
                        foreach (string game in games)
                        {
                            debugLines.Add($"      - {Path.GetFileName(game)}");
                        }
                    }
                    catch { }
                }
            }

            return string.Join("\n", debugLines);
        }

        private void Button2_Click(object? sender, EventArgs e)
        {
            try
            {
                string? steamPath = GetSteamPath();
                if (steamPath == null)
                {
                    MessageBox.Show("Steam installation path not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string? gamePath = FindSiloGame(steamPath);
                if (gamePath == null)
                {
                    MessageBox.Show("Silo game not found! Please make sure the game is installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string targetDir = Path.Combine(gamePath, "driver", "slime_locomotion", "bin", "win64");
                if (!Directory.Exists(targetDir))
                {
                    MessageBox.Show($"Target directory not found: {targetDir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string targetDll = Path.Combine(targetDir, "driver_slime_locomotion.dll");
                string backupDll = Path.Combine(targetDir, "driver_slime_locomotion.bak");

                if (!File.Exists(backupDll))
                {
                    MessageBox.Show("Backup file not found! No official driver backup exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                File.Copy(backupDll, targetDll, true);
                File.Delete(backupDll);

                MessageBox.Show($"Official driver restored successfully!\nTarget path: {targetDll}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Restore failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
