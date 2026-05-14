using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Console.LightweightTelephony;
using ZoiperAPI;

namespace ZoiperCustomDialer
{
    public class ZoiperCustomDialer : ICustomDialer
    {
        private ZoiperAPI.ZoiperAPI zoiper;
        public event EventHandler<CallStatusChangedEventArgs> CallStatusChanged;
        private static bool isRaised = false;
        private readonly object loggingLock = new object();

        public ZoiperCustomDialer()
        {
            LogToFile("Initialize zoiper");
            zoiper = new ZoiperAPI.ZoiperAPI();
            zoiper.OnZoiperCallAccept += OnZoiperCallAccept;
            zoiper.OnZoiperCallReject += OnZoiperCallReject;
            zoiper.OnZoiperCallHang += OnZoiperCallHang;
            zoiper.OnZoiperCallFail += OnZoiperCallFail;
            zoiper.OnZoiperCallRing += OnZoiperCallRing;
        }

        public async void Dial(string phoneNumber)
        {
            LogToFile($"Start dialing phoneNumber {phoneNumber}");

            await Task.Run(() =>
            {
                isRaised = false;
                zoiper.Dial(phoneNumber, 0);
            });

            LogToFile("End dialing");
        }

        public async void HangUp()
        {
            LogToFile("Start hang up");

            await Task.Run(() =>
            {
                zoiper.Hang();
            });

            LogToFile("End hang up");
        }

        private void RaiseCallStatusChangedEvent(CustomCallOutcome callOutcome)
        {
            if (isRaised) return;

            CallStatusChanged?.Invoke(this, new CallStatusChangedEventArgs { CustomCallStatus = callOutcome });
            LogToFile($"RaiseCallStatusChangedEvent callOutcome: {callOutcome}");

            isRaised = true;
        }

        public void OnZoiperCallAccept(IZoiperCall call)
        {
            LogToFile("OnZoiperCallAccept");

            RaiseCallStatusChangedEvent(CustomCallOutcome.Connected);
        }

        public void OnZoiperCallReject(IZoiperCall call, int causeCode)
        {
            LogToFile($"OnZoiperCallReject causeCode: {causeCode}");

            switch (causeCode)
            {
                case 17:
                    RaiseCallStatusChangedEvent(CustomCallOutcome.Busy);
                    break;
                case 18:
                    RaiseCallStatusChangedEvent(CustomCallOutcome.Unobtainable);
                    break;
                case 41:
                    RaiseCallStatusChangedEvent(CustomCallOutcome.Unobtainable);
                    break;
                case 102:
                    RaiseCallStatusChangedEvent(CustomCallOutcome.NoReply);
                    break;
                default:
                    RaiseCallStatusChangedEvent(CustomCallOutcome.Unobtainable);
                    break;
            }
        }

        public void OnZoiperCallHang(IZoiperCall call)
        {
            LogToFile("OnZoiperCallHang");

            RaiseCallStatusChangedEvent(CustomCallOutcome.Stopped);
        }

        public void OnZoiperCallFail(IZoiperCall call, int causeCode)
        {
            LogToFile($"OnZoiperCallFail causeCode {causeCode}");

            RaiseCallStatusChangedEvent(CustomCallOutcome.TelephonyFailure);
        }

        public void OnZoiperCallRing(IZoiperCall call)
        {
            LogToFile("OnZoiperCallRing");
        }

        public void LogToFile(string data )
        {
            lock (loggingLock)
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Confirmit\\CATI");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var logFilePath = Path.Combine(folder, "CATI_ZoiperIntegrationLog.txt");
                var oldLogFilePath = Path.Combine(folder, "CATI_ZoiperIntegrationLogOld.txt");
                if (File.Exists(logFilePath))
                {
                    var logFile = new FileInfo(logFilePath);
                    if (logFile.Length > 2097152)
                    {
                        logFile.CopyTo(oldLogFilePath, true);
                        logFile.Delete();
                    }
                }

                using (var sw = new StreamWriter(logFilePath, true, Encoding.Default))
                {
                    sw.WriteLine($"{DateTime.Now:O}: {data}");
                }
            }
        }
    }
}
