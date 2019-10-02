using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AbeServices.Common.Helpers
{
    public static class LocalHost
    {
        public static string GetRandomFilename()
        {
            return Path.GetTempFileName();
        }

        public static async Task<string> WriteFileAsync(byte[] fileData, string filePath = "")
        {
            if (String.IsNullOrEmpty(filePath))
                filePath = GetRandomFilename();

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await fs.WriteAsync(fileData, 0, fileData.Length);
                return filePath;
            }
        }

        public static async Task<byte[]> ReadFileAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] result = new byte[fs.Length];
                await fs.ReadAsync(result, 0, (int)fs.Length);
                return result;
            }
        }

        public static Task RunProcessAsync(string command, string arguments = "")
        {
            var tcs = new TaskCompletionSource<object>();
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(command)
                {
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = arguments
                }
            };
            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    var errorMessage = process.StandardError.ReadToEnd();
                    tcs.SetException(new InvalidOperationException(errorMessage));
                }
                else
                {
                    tcs.SetResult(null);
                }
                process.Dispose();
            };
            process.Start();
            return tcs.Task;
        }
    }
}