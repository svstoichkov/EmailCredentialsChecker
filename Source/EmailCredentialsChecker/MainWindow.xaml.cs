namespace EmailCredentialsChecker
{
    using System.Collections.Generic;
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
            }
        }

        private void BtnCheck_OnClick(object sender, RoutedEventArgs e)
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
                                                   }
                                               });
        }
    }
}
