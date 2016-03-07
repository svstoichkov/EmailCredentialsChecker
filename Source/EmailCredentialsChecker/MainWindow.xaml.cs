namespace EmailCredentialsChecker
{
    using System.Windows;

    using ViewModels;

    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            var dataObject = e.Data as DataObject;

            if (dataObject.ContainsFileDropList())
            {
                var fileNames = dataObject.GetFileDropList();
                foreach (var fileName in fileNames)
                {
                    (this.DataContext as MainViewModel).AddFileWithDragAndDrop(fileName);
                    break;
                }
            }
        }
    }
}
