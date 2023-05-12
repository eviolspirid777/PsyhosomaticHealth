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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PsyhHealth : Window
    {
        ComboBox comboBox = new ComboBox();
        TextBox textBox = new TextBox();
        GroupBox resultHead = new GroupBox();
        GroupBox pulseHead = new GroupBox();
        TextBox pulseBox = new TextBox();
        public PsyhHealth()
        {
            InitializeComponent();
            disciplineType.Items.Add("Состояние покоя");
            disciplineType.Items.Add("Бег 100м");
            disciplineType.Items.Add("Приседания");
        }
        public void buttonClick(object sender, RoutedEventArgs e)                       //вывод результата исчисления
        {
            double result = 0;
            if (disciplineType.SelectedIndex == 0)
                switch (comboBox.Text)
                {
                    case "Сидя":
                        result = SetResult(((int)HumanState.sitting));
                        break;
                    case "Стоя":
                        result = SetResult((int)HumanState.staying);
                        break;
                    case "Лежа":
                        result = SetResult((int)HumanState.lying);
                        break;
                }
            result = Math.Ceiling(result * 100) / 100;
            if (result != 0)
                textBlock.Text = $"Ваш коэффициент {Convert.ToString(result)}.";
            else
                textBlock.Text = "Ошибка при вводе данных!";
        }
        public void MouseLeave(object sender, MouseEventArgs e)                     //обработка события, когда мы выводим курсор из выпадающего списка
        {
            ClearAll();
            if (disciplineType.SelectedIndex == 0)
            {
                //GroupBox resultHead = new GroupBox();
                resultHead.Name = "typeHead";
                resultHead.Header = "Вид";
                resultHead.FontSize = 13;
                resultHead.ToolTip = "Введите результат";
                resultHead.Margin = new Thickness(0, 0, 0, 10);

                //ComboBox comboBox = new ComboBox();
                comboBox.Name = "comboBox";
                comboBox.Items.Add("Сидя");
                comboBox.Items.Add("Стоя");
                comboBox.Items.Add("Лежа");
                comboBox.Width = 185;
                comboBox.Height = 30;
                resultHead.Content = comboBox;

                //GroupBox pulseHead = new GroupBox();
                pulseHead.Name = "pulseHead";
                pulseHead.Header = "Пульс";
                pulseHead.FontSize = 13;
                pulseHead.ToolTip = "Измерьте пульc за 10 сек";

                //TextBox pulseBox = new TextBox();
                pulseBox.Name = "pulseBox";
                pulseBox.Width = 185;
                pulseBox.Height = 30;
                pulseHead.Content = pulseBox;

                stackPanel.Children.Add(resultHead);
                stackPanel.Children.Add(pulseHead);
            }
            else if (disciplineType.SelectedIndex == 1 || disciplineType.SelectedIndex == 2)
            {

                // Создание первого GroupBox
                GroupBox resultHead = new GroupBox();
                resultHead.Name = "resultHead";
                resultHead.Header = "Результат";
                resultHead.FontSize = 13;
                resultHead.ToolTip = "Введите результат";
                resultHead.Margin = new Thickness(0, 0, 0, 10);

                // Создание TextBox в первом GroupBox
                TextBox resultBox = new TextBox();
                resultBox.Name = "resultBox";
                resultBox.Width = 185;
                resultBox.Height = 30;

                // Добавление TextBox в первый GroupBox
                resultHead.Content = resultBox;

                // Создание второго GroupBox
                GroupBox pulseHead = new GroupBox();
                pulseHead.Name = "pulseHead";
                pulseHead.Header = "Пульс";
                pulseHead.FontSize = 13;
                pulseHead.ToolTip = "Измерьте пульc за 10 сек";

                // Создание TextBox во втором GroupBox
                TextBox pulseBox = new TextBox();
                pulseBox.Name = "pulseBox";
                pulseBox.Width = 185;
                pulseBox.Height = 30;

                // Добавление TextBox во второй GroupBox
                pulseHead.Content = pulseBox;

                // Добавление обоих GroupBox в StackPanel
                stackPanel.Children.Add(resultHead);
                stackPanel.Children.Add(pulseHead);
            }
        }
        public double SetResult(int coef = 1)
        {
            if (double.TryParse(pulseBox.Text, out double temp))
                return coef / (100 * temp / 12);
            else
                return 0;
        }
        public void ClearAll()
        {
            textBlock.Text = "";
            comboBox.Items.Clear();
            stackPanel.Children.Clear();
        }
        enum HumanState
        {
            lying = 100,
            sitting = 103,
            staying = 106
        }
    }
}
