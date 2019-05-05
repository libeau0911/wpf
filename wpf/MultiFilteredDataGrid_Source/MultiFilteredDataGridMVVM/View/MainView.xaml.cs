using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using MultiFilteredDataGridMVVM.Helpers;
using Common;

namespace MultiFilteredDataGridMVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            // Here we send a message which is caught by the view model.  The message contains a reference
            // to the CollectionViewSource which is instantiated when the view is instantiated (before the view model).
            Messenger.Default.Send(new ViewCollectionViewSourceMessageToken() { CVS = (CollectionViewSource)(this.Resources["X_CVS"]) });

            // Note to MVVM purists:  Not an ideal solution.  But based on the amount if time spent on this it was acceptable, especially to the client.
        }

        private void addBook(object sender, RoutedEventArgs e)
        {
            EditDialog ed = new EditDialog();
            ed.Show();
        }

        EditDialog ed = new EditDialog();

        private void editBook(object sender, RoutedEventArgs e)
        {
            ed.SaveButton.Content = "Save";
            Thing items = dataGrid.SelectedItem as Thing;
            System.Console.WriteLine(items.Author);
            ed.e_Author.Text = items.Author;
            ed.e_Title.Text = items.Title;
            ed.e_Year.SelectedValue = items.Year;
            ed.OnChildInputEvent += new EditDialog.OnChildInputHandler(OnChildInputEvent);
            ed.ShowDialog();
        }

        void OnChildInputEvent(Thing things)
        {
            Thing items=dataGrid.SelectedItem as Thing;
            items.Author = things.Author;
            items.Title = things.Title;
            items.Year = things.Year;
            
            ed.OnChildInputEvent -= new EditDialog.OnChildInputHandler(OnChildInputEvent);
            ed = null;
        }
    }
}
