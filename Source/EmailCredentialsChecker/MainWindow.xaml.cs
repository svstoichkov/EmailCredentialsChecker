namespace EmailCredentialsChecker
{
    using System;
    using System.Collections.Generic;
    using System.Media;
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
                this.credentials = TextParser.GetCredentials(dialog.FileName);
                this.dataGrid.ItemsSource = this.credentials;
                this.btnCheck.IsEnabled = true;
            }
        }

        private async void BtnCheck_OnClick(object sender, RoutedEventArgs e)
        {
            this.btnCheck.IsEnabled = false;
            var incrementValue = 100.0 / this.credentials.Count;
            await Task.Run(() =>
                                {
                                    Parallel.ForEach(this.credentials, credential =>
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

            SystemSounds.Beep.Play();
            this.btnCheck.IsEnabled = true;
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            e.
        }
    }
}
