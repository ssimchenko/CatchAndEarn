using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CatchAndEarn.Model;

public static class AudioService
{
    private static readonly string _audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audio");
    private static Process? _musicProcess;

    public static async Task PlayAsync(string fileName)
    {
        try
        {
            var fullPath = Path.Combine(_audioPath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Audio file not found: {fullPath}");
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = GetPlayerCommand(),
                        Arguments = GetPlayerArguments(fullPath),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using var process = Process.Start(psi);
                    process?.WaitForExit(5000); // Ждём максимум 5 секунд
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Audio error: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Audio error: {ex.Message}");
        }
    }

    public static async Task PlayBackgroundMusicAsync(string fileName)
    {
        try
        {
            StopBackgroundMusic();

            var fullPath = Path.Combine(_audioPath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Music file not found: {fullPath}");
                return;
            }

            await Task.Run(() =>
            {
                while (_musicProcess == null || _musicProcess.HasExited)
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = GetPlayerCommand(),
                            Arguments = GetPlayerArguments(fullPath),
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        _musicProcess = Process.Start(psi);
                        _musicProcess?.WaitForExit();
                        
                        Task.Delay(100).Wait();
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Music error: {ex.Message}");
        }
    }

    public static void StopBackgroundMusic()
    {
        try
        {
            if (_musicProcess != null && !_musicProcess.HasExited)
            {
                _musicProcess.Kill();
                _musicProcess.Dispose();
            }
        }
        catch
        {
            // Игнорируем ошибки
        }
        finally
        {
            _musicProcess = null;
        }
    }

    private static string GetPlayerCommand()
    {
        if (OperatingSystem.IsMacOS())
            return "afplay";
        else if (OperatingSystem.IsWindows())
            return "powershell";
        else if (OperatingSystem.IsLinux())
            return "aplay"; // или "paplay"
        
        return "afplay"; // по умолчанию macOS
    }

    private static string GetPlayerArguments(string filePath)
    {
        if (OperatingSystem.IsMacOS())
            return $"\"{filePath}\"";
        else if (OperatingSystem.IsWindows())
            return $"-c \"(New-Object Media.SoundPlayer '{filePath.Replace("\\", "\\\\")}').PlaySync();\"";
        else if (OperatingSystem.IsLinux())
            return $"\"{filePath}\"";
        
        return $"\"{filePath}\"";

        
    }
    public static void StopAll()
    {
        StopBackgroundMusic();

        try
        {
            var killCommand = OperatingSystem.IsMacOS() ? "killall afplay" : 
                              OperatingSystem.IsLinux() ? "killall aplay" : "";
            
            if (!string.IsNullOrEmpty(killCommand))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "sh",
                    Arguments = $"-c \"{killCommand}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(psi)?.WaitForExit(500);
            }
        }
        catch
        {
        }
    }
}