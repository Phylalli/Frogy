
namespace FrogyCoreService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FrogyCoreServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FrogyCoreServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FrogyCoreServiceProcessInstaller
            // 
            this.FrogyCoreServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FrogyCoreServiceProcessInstaller.Password = null;
            this.FrogyCoreServiceProcessInstaller.Username = null;
            // 
            // FrogyCoreServiceInstaller
            // 
            this.FrogyCoreServiceInstaller.Description = "Frogy core service.";
            this.FrogyCoreServiceInstaller.ServiceName = "FrogyCoreService";
            this.FrogyCoreServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.FrogyCoreServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FrogyCoreServiceProcessInstaller,
            this.FrogyCoreServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FrogyCoreServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FrogyCoreServiceInstaller;
    }
}