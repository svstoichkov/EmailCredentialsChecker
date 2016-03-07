namespace EmailCredentialsChecker.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    using Helpers;

    using Microsoft.Win32;

    using Models;

    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Credential> credentials;
        private string path;
        private double progress;
        private bool isChecked;

        public MainViewModel()
        {
            this.SelectFile = new RelayCommand(this.HandleSelectFile);
            this.Check = new RelayCommand(this.HandleCheck, this.CanCheck);
            this.Recheck = new RelayCommand(this.HandleRecheck, this.CanRecheck);
            this.SaveValidOnly = new RelayCommand(this.HandleSaveValidOnly, this.CanSave);
            this.SaveNewFile = new RelayCommand(this.HandleSaveNewFile, this.CanSave);
            this.SaveSameFile = new RelayCommand(this.HandleSaveSameFile, this.CanSave);
        }

        public ICommand SelectFile { get; }

        public ICommand Check { get; }

        public ICommand Recheck { get; }

        public ICommand SaveValidOnly { get; }

        public ICommand SaveNewFile { get; }

        public ICommand SaveSameFile { get; }

        public ICollection<Credential> Credentials
        {
            get
            {
                return this.credentials ?? (this.credentials = new ObservableCollection<Credential>());
            }

            set
            {
                this.credentials = this.credentials ?? new ObservableCollection<Credential>();
                this.credentials.Clear();
                foreach (var credential in value)
                {
                    this.credentials.Add(credential);
                }
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.Set(() => this.Path, ref this.path, value);
            }
        }

        public double Progress
        {
            get
            {
                return this.progress;
            }

            set
            {
                this.Set(() => this.Progress, ref this.progress, value);
            }
        }

        public void AddFileWithDragAndDrop(string filePath)
        {
            if (filePath.EndsWith(".txt"))
            {
                this.Path = filePath;
                this.Credentials = TextParser.GetCredentials(filePath);
                this.isChecked = false;
            }
            else
            {
                MessageBox.Show("Program only accepts *.txt files", "File extension error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HandleSelectFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt";
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                this.Path = dialog.FileName;
                this.Credentials = TextParser.GetCredentials(this.Path);
                this.isChecked = false;
            }
        }

        private bool CanCheck()
        {
            return this.Credentials.Any() && this.isChecked == false;
        }

        private async void HandleCheck()
        {
            await this.CheckCredentials(this.Credentials);
            await this.CheckCredentials(this.Credentials.Where(x => x.IsValid != true));

            this.isChecked = true;
            this.ShowFinalMessage();
        }

        private bool CanRecheck()
        {
            return this.Credentials.Any(x => x.IsValid != true) && this.isChecked;
        }

        private async void HandleRecheck()
        {
            await this.CheckCredentials(this.Credentials.Where(x => x.IsValid != true));
            this.ShowFinalMessage();
        }

        private bool CanSave()
        {
            return this.isChecked;
        }

        private void HandleSaveValidOnly()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt";
            dialog.CheckFileExists = false;
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                File.WriteAllLines(dialog.FileName, this.Credentials.Where(x => x.IsValid == true).Select(x => $"{x.Email}/{x.Password}/{x.IsValid}"));
            }
        }

        private void HandleSaveNewFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt";
            dialog.CheckFileExists = false;
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                File.WriteAllLines(dialog.FileName, this.Credentials.Select(x => $"{x.Email}/{x.Password}/{x.IsValid}"));
            }
        }

        private void HandleSaveSameFile()
        {
            File.WriteAllLines(this.Path, this.Credentials.Select(x => $"{x.Email}/{x.Password}/{x.IsValid}"));
        }

        private async Task CheckCredentials(IEnumerable<Credential> credentials)
        {
            var incrementValue = 100.0 / credentials.Count();
            this.Progress = 0;

            await Task.Run(() =>
            {
                Parallel.ForEach(credentials, (credential, state) =>
                {
                    credential.Check();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.Progress += incrementValue;
                    });
                });
            });

            this.Progress = 100;
        }

        private void ShowFinalMessage()
        {
            MessageBox.Show($"{this.Credentials.Count(x => x.IsValid == true)} valid credentials out of {this.Credentials.Count(x => true)} checked.\n{this.Credentials.Count(x => x.IsValid == null)} credentials were left unchecked.", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
