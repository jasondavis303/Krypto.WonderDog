using System;

namespace Krypto.WonderDog
{
    public class KryptoProgress
    {
        internal KryptoProgress(DateTime started, long complete, long total, bool done)
        {
            Started = started;
            RunTime = DateTime.Now - Started;
            BytesComplete = complete;
            TotalBytes = total;
            Done = done;

            Percent = TotalBytes > 0 ? BytesComplete / (double)TotalBytes : 0;
        
            if(BytesComplete > 0)
            {
                try
                {
                    double secs = RunTime.TotalSeconds;
                    if (secs > 0)
                        BytesPerSecond = BytesComplete / secs;
                }
                catch { }
            }

            if (BytesPerSecond > 0)
            {
                double val = BytesPerSecond;
                string[] exts = new string[] { "B", "Kb", "Mb", "Gb", "Tb", "Pb", "Xb", "Yb", "Zb" };
                int index = 0;
                while (val >= 1024)
                {
                    val /= 1024;
                    index++;
                    if (index >= exts.Length - 1)
                        break;
                }
                Speed = $"{val:0.00} {exts[index]}/s";
            }
            else
            {
                Speed = "0 B/s";
            }        
        }

        public DateTime Started { get; }
        public TimeSpan RunTime { get; }
        public long BytesComplete { get; }
        public long TotalBytes { get; }
        public double Percent { get; }
        public double BytesPerSecond { get; }
        public string Speed { get; }
        public bool Done { get; set; }
    }
}
