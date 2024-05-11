using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class SumoRunner
{
    public static Process sumoProcess;

    public static Thread thread;
    public static void RunSumo()
    {
        thread = null;
        Task.Run(() => RunSumoExecutable()).ContinueWith(t => thread = null);
    }

    static void RunSumoExecutable()
    {
        thread = Thread.CurrentThread;
        try
        {
            var startInfo = new ProcessStartInfo(Menu.executablePath)
            {
                Arguments = "-c \"" + Menu.configPath + "\" --remote-port 4001 --start --step-length 0.02",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using (sumoProcess = new Process { StartInfo = startInfo })
            {
                sumoProcess.OutputDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
                sumoProcess.ErrorDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
                sumoProcess.Start();
                sumoProcess.BeginOutputReadLine();
                sumoProcess.BeginErrorReadLine();
                sumoProcess.WaitForExit();
                sumoProcess.Close();
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log($"Failed to run SUMO executable: {ex.Message}");
        }
        finally
        {
            if (sumoProcess != null)
            {
                sumoProcess.Dispose();
            }
        }
    }
}