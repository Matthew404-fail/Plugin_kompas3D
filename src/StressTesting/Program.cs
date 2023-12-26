namespace StressTesting
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using DoorPlugin.Model;
    using DoorPlugin.Wrapper;
    using Microsoft.VisualBasic.Devices;

    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new Builder();
            var stopWatch = new Stopwatch();
            var parameters = new Parameters();
            var streamWriter = new StreamWriter($"log.txt", true);
            Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var count = 0;

            while (true)
            {
                const double gigabyteInByte = 0.000000000931322574615478515625;
                stopWatch.Start();

                builder.BuildDetail(
                    parameters.GetParametersCurrentValues(),
                    parameters.IsHandleCylinder);

                stopWatch.Stop();

                var computerInfo = new ComputerInfo();
                var usedMemory = (computerInfo.TotalPhysicalMemory
                                  - computerInfo.AvailablePhysicalMemory)
                                 * gigabyteInByte;

                streamWriter.WriteLine(
                    $"{++count}\t{stopWatch.Elapsed:hh\\:mm\\:ss}\t{usedMemory}");
                streamWriter.Flush();

                stopWatch.Reset();
            }

            streamWriter.Close();
            streamWriter.Dispose();
            Console.Write($"End {new ComputerInfo().TotalPhysicalMemory}");
        }
    }
}