namespace EmailCredentialsChecker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Helpers;

    using Microsoft.Win32;

    using Models;

    public partial class MainWindow
    {
        private List<Credential> credentials;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt";
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                this.txtPath.Text = dialog.FileName;
                this.FillDataGrid(this.txtPath.Text);
            }
        }

        private async void BtnCheck_OnClick(object sender, RoutedEventArgs e)
        {
            await this.CheckCredentials(this.credentials);
            await this.CheckCredentials(this.credentials.Where(x => x.IsValid != true));
            MessageBox.Show($"{this.credentials.Count(x => x.IsValid == true)} / {this.credentials.Count}", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            var dataObject = e.Data as DataObject;
            
            if (dataObject.ContainsFileDropList())
            {
                var fileNames = dataObject.GetFileDropList();
                foreach (var fileName in fileNames)
                {
                    this.txtPath.Text = fileName;
                    break;
                }

                this.FillDataGrid(this.txtPath.Text);
            }
        }

        private void FillDataGrid(string path)
        {
            this.progressBar.Value = 0;
            this.lblProgress.Content = "0%";
            this.credentials = TextParser.GetCredentials(path);
            this.dataGrid.ItemsSource = this.credentials;
            this.lblCredCount.Content = this.credentials.Count;
            if (this.credentials.Any())
            {
                this.btnCheck.IsEnabled = true;
                this.spSaveButtons.IsEnabled = false;
            }
        }

        private void BtnSaveSameFile_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllLines(this.txtPath.Text, this.credentials.Select(x => $"{x.Email}/{x.Password}/{x.IsValid}"));
        }

        private void BtnSaveNewFile_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt";
            dialog.CheckFileExists = false;
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                File.WriteAllLines(dialog.FileName, this.credentials.Select(x => $"{x.Email}/{x.Password}/{x.IsValid}"));
            }
        }

        private void BtnSaveValidOnly_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files|*.txt";
            dialog.CheckFileExists = false;
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                File.WriteAllLines(dialog.FileName, this.credentials.Where(x => x.IsValid == true).Select(x => $"{x.Email}/{x.Password}/{x.IsValid}"));
            }
        }

        private async Task CheckCredentials(IEnumerable<Credential> credentials)
        {
            this.btnCheck.IsEnabled = false;
            this.spSaveButtons.IsEnabled = false;
            var incrementValue = 100.0 / credentials.Count();
            this.progressBar.Value = 0;
            this.lblProgress.Content = "0%";
            await Task.Run(() =>
            {
                Parallel.ForEach(credentials, credential =>
                {
                    switch (credential.GetEmailType())
                    {
                        case EmailType.Yahoo:
                            credential.IsValid = YahooChecker.Check(credential);
                            break;
                        case EmailType.Aol:
                            credential.IsValid = AolChecker.Check(credential);
                            break;
                        case EmailType.Google:
                            credential.IsValid = GoogleChecker.Check(credential);
                            break;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.progressBar.Value += incrementValue;
                        this.lblProgress.Content = $"{Math.Round(this.progressBar.Value)}%";
                    });
                });
            });

            if (this.credentials.Any())
            {
                this.spSaveButtons.IsEnabled = true;
            }

            this.progressBar.Value = 100;
            this.lblProgress.Content = "100%";
        }
    }
}
