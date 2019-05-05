using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;

namespace MultiFilteredDataGridMVVM.View
{
    public partial class EditDialog : Window
    {
        int i;

        public EditDialog()
        {
            InitializeComponent();
            for (i = 2006; i < 2016; i++)
            {
                e_Year.Items.Add(i);
            }
        }

        public delegate void OnChildInputHandler(Thing things);
        public event OnChildInputHandler OnChildInputEvent;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Thing new_thing=new Thing();
            new_thing.Author=e_Author.Text;
            new_thing.Title=e_Title.Text;
            new_thing.Year = Convert.ToInt32(e_Year.SelectedValue);
            if (OnChildInputEvent != null)
                OnChildInputEvent(new_thing);
            this.Close();
        }
    }
}
