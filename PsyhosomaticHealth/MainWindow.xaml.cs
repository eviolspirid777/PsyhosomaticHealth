using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsyhosomaticHealth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string imagePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Images/logo.png");          //Переменная, которая хранит путь к логотипу

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            Image image = new Image() { Height = 200, Width = 200, Margin = new Thickness(60, 180, 70, 5) };        //создаем объект для логотипа
            image.Source = bitmapImage;
            greetWindow.Children.Add(image);

            TextBlock textBlock = new TextBlock()                           //Текстовое поле под название
            {
                Text = "Биоэкономичная Диагностика Здравоукрепления",
                FontSize = 18,
                FontFamily = new FontFamily("Comic Sans MS"),
                Foreground = Brushes.Green,
            };

            myTextBlock.Children.Add (textBlock);

            this.Show();
            Thread.Sleep(2200);
            chooseMenu chmen = new chooseMenu();            //создаем окно выбора
            chmen.Show();
            this.Close();
        }
    }
}
