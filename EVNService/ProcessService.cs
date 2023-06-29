using log4net;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;

namespace EVNService
{
    public partial class ProcessService : ServiceBase
    {
        private Timer scheduleTimer = null;
        ILog log = LogManager.GetLogger(typeof(ProcessService));
        public ProcessService()
        {
            InitializeComponent();
            scheduleTimer = new Timer();
            scheduleTimer.Elapsed += new System.Timers.ElapsedEventHandler(scheduleTimer_Elapsed);
        }

        protected override void OnStart(string[] args)
        {
            Init();
        }

        public void Init()
        {
            scheduleTimer.Stop();
            double waitTime;
            if (double.TryParse(ConfigurationManager.AppSettings["watingTime"], out waitTime))
            {
                scheduleTimer.Interval = waitTime;
            }
            else
            {
                scheduleTimer.Interval = 60000;
            }
            scheduleTimer.Start();
        }


        protected void scheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            scheduleTimer.Stop();
            startup();
            scheduleTimer.Start();
        }

        public void startup()
        {
            try
            {
                Utils.SendMail();
            }
            catch (Exception message)
            {
                log.Error(message);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
