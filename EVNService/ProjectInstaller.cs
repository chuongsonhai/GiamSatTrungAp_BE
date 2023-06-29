using System.ComponentModel;
using System.ServiceProcess;

namespace EVNService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void SendMailService_AfterInstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(this.DeliverService.ServiceName))
            {
                sc.Start();
            }
        }

        private void SendMailService_BeforeUninstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            //using (ServiceController sc = new ServiceController("ReduceMail"))
            //{
            //    sc.Stop();
            //}
        }
    }
}
