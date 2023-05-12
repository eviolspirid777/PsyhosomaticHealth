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
    /// Interaction logic for chooseMenu.xaml
    /// </summary>
    public partial class chooseMenu : Window
    {
        public chooseMenu()
        {
            InitializeComponent();
            string imagePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Images/logo.png");          //Переменная, которая хранит путь к логотипу
            
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            Image image = new Image() { Height = 100, Width = 100, Margin = new Thickness(0,350,640,10) };        //создаем объект для логотипа
            image.Source = bitmapImage;
            chseMenu.Children.Add(image);
        }
        public void baseClick(object sender, RoutedEventArgs e)         //стандартная версия
        {
            PsyhHealth psyhHealth = new PsyhHealth();
            psyhHealth.Show();
            Close();
        }
        public void advancedClick(object sender, RoutedEventArgs e)             //продвинутая версия приложения
        {

            //Close();
        }
        public void helpClick(object sender, RoutedEventArgs e)                 //помощь
        {
            MessageBox.Show("БАЗОВАЯ ВЕРСИЯ - представляет собой заранее заготовленные дисциплины.\nПРОДВИНУТАЯ ВЕРСИЯ - позволяет задать свои цели и разработать программу по их достижению", "Помощь");
        }
    }
}
