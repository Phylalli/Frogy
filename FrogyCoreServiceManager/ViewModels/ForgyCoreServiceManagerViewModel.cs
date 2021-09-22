using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using FrogyCoreServiceManager.Utils;

namespace FrogyCoreServiceManager.ViewModels
{
    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameters)
        {
            return _canExecute == null || _canExecute(parameters);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameters)
        {
            _execute(parameters);
        }

        #endregion // ICommand Members
    }

    class ForgyCoreServiceManagerViewModel : INotifyPropertyChanged
    {
        string serviceFilePath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
            "FrogyCoreService.exe");
        string serviceName = "FrogyCoreService";

        private string serviceStatus;
        public string ServiceStatus
        {
            get
            {
                return serviceStatus;
            }
            set
            {
                serviceStatus = value;
                OnPropertyChanged();
            }
        }

        private string runningStatus;
        public string RunningStatus
        {
            get
            {
                return runningStatus;
            }
            set
            {
                runningStatus = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshButton_Click
        {
            get
            {
                return new RelayCommand(
                        param => this.refresh(),
                        param => true
                    );
            }
        }
        private void refresh()
        {
            ServiceStatus = ServiceHelper.IsServiceExisted(serviceName)?
                "Installed":
                "Not installed";

            RunningStatus = ServiceHelper.IsServiceExisted(serviceName)?
                ServiceHelper.ServiceStatus(serviceName).ToString():
                "--";
        }

        public ICommand Install_Click
        {
            get
            {
                return new RelayCommand(
                        param => this.install(),
                        param => true
                    );
            }
        }
        private void install()
        {
            if (ServiceHelper.IsServiceExisted(serviceName))
            {
                ServiceHelper.UninstallService(serviceName);
            }
            ServiceHelper.InstallService(serviceFilePath);
        }

        public ICommand Start_Click
        {
            get
            {
                return new RelayCommand(
                        param => this.start(),
                        param => true
                    );
            }
        }
        private void start()
        {
            if (ServiceHelper.IsServiceExisted(serviceName))
            {
                ServiceHelper.ServiceStart(serviceName);
            }
        }

        public ICommand Stop_Click
        {
            get
            {
                return new RelayCommand(
                        param => this.stop(),
                        param => true
                    );
            }
        }
        private void stop()
        {
            if (ServiceHelper.IsServiceExisted(serviceName))
            {
                ServiceHelper.ServiceStop(serviceName);
            }
        }

        public ICommand Uninstall_Click
        {
            get
            {
                return new RelayCommand(
                        param => this.uninstall(),
                        param => true
                    );
            }
        }
        private void uninstall()
        {
            if (ServiceHelper.IsServiceExisted(serviceName))
            {
                ServiceHelper.ServiceStop(serviceName);
                ServiceHelper.UninstallService(serviceFilePath);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
