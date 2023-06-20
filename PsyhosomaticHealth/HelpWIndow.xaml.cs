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
                case "Условные обозначения":
                    contentTextBlock.Text = "This is the content for Item 1.";
                    break;
                case "Расчетные формулы":
                    contentTextBlock.Text = "This is the content for Item 2.";
                    break;
                case "Шкала золотой пропорции":
                    contentTextBlock.Text = "This is the content for Item 3.";
                    break;
				case "Литература":
					contentTextBlock.Text = "This is the content for Item 2.";
					break;
                default:
                    contentTextBlock.Text = "Содержимое еще не доступно...";
                    break;
			}
        }
    }
}
