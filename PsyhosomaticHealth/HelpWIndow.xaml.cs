using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PsyhosomaticHealth
{
    /// <summary>
    /// Interaction logic for HelpWIndow.xaml
    /// </summary>
    public partial class HelpWIndow : Window
    {
        public HelpWIndow()
        {
            InitializeComponent();
        }
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selectedItem = (ListBoxItem)menuListBox.SelectedItem;
            string selectedContent = selectedItem.Content.ToString();

            switch (selectedContent)
            {
                case "Item 1":
                    contentTextBlock.Text = "This is the content for Item 1.";
                    break;
                case "Item 2":
                    contentTextBlock.Text = "This is the content for Item 2.";
                    break;
                case "Item 3":
                    contentTextBlock.Text = "This is the content for Item 3.";
                    break;
            }
        }
    }
}
