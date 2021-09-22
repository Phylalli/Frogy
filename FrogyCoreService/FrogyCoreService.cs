using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace FrogyCoreService
{
    public partial class FrogyCoreService : ServiceBase
    {
        string coreFilePath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
            "FrogyCore.exe");
        Process core;


        public FrogyCoreService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread.Sleep(10000);

            core = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = coreFilePath,
                    Verb = "runas"
                }
            };

            core.Start();
        }

        protected override void OnStop()
        {
            //core.Kill();
        }
    }
}
