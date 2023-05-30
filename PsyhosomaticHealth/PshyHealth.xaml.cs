using Microsoft.Win32;
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
using System.Text.Json;
using System.IO;

namespace PsyhosomaticHealth
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PsyhHealth : Window
    {
        ComboBox comboBox = new ComboBox();
        GroupBox resultHead = new GroupBox();
        GroupBox pulseHead = new GroupBox();
        TextBox pulseBox = new TextBox();           //Значение текстового поля pusleBox
        TextBox resultBox = new TextBox();          //Значение текстового поля resultbox
        public PsyhHealth()
        {
            InitializeComponent();
            disciplineType.Items.Add("Состояние относительного покоя");
            disciplineType.Items.Add("Бег 100м");
            disciplineType.Items.Add("Приседания");
        }
        public void ShowFunctions()
        {
            setDiscipline.Visibility = Visibility.Visible;          //показывает поле для выбора дисциплины
            getResult.Visibility = Visibility.Visible;              //показывает клавишу получения результата
        }
        public void ColorSet( double result)
        {
            if (result >= 1.60 && result <= 1.62)
                colorBlock.Background = new SolidColorBrush(Colors.Green);
            else if (result >= 1.01 && result <= 1.24)
                colorBlock.Background = new SolidColorBrush(Colors.GreenYellow);
            else if (result >= 1.25 && result <= 1.59)
                colorBlock.Background = new SolidColorBrush(Colors.YellowGreen);
            else if (result == 1)
                colorBlock.Background = new SolidColorBrush(Colors.Yellow);
            else if (result < 0.99)
                colorBlock.Background = new SolidColorBrush(Colors.Red);
        }
        public void TextAdd(double result)
        {
            if (result >= 1.60 && result <= 1.62)
                textBlock.Text += "\nЗОЛОТАЯ ПРОПОРЦИЯ - оптимальный максимум гармоничного и экономичности жизнидеятельности и жизнеспособности организма.";
            if (result >= 1.01 && result <= 1.59)
                textBlock.Text += "\nРЕЗЕРВ - преобладание психосоматической нормы над физиологическими и возможными патологическими нарушениями, прогрессирующее развитие экономичности жизнидеятельности организма.";
            if (result == 1)
                textBlock.Text += "\nБАЛАНС - промежуточный критерий постоянного взаимодействия на протяжении всей жизни психосоматической нормы с физиологическими или патологическими нарушениями";
            if (result <= 0.99)
                textBlock.Text += "\nДЕФИЦИТ - преобладание физиологических и патологических нарушений над психосоматической нормой, что в соответствующей степени нарушает гармоничность и экономичность жизнидеятельности организма";
        }
        public void HideFunctions()
        {
            setDiscipline.Visibility = Visibility.Visible;          //скрывает поле для выбора дисциплины
            getResult.Visibility = Visibility.Visible;              //скрывает клавишу получения результата
        }
        public void FuncSetEnable()                                     //Включить функции
        {
            printFile.IsEnabled = true;
            saveFile.IsEnabled = true;
            saveFileAs.IsEnabled = true;
        }
        public void FuncSetDisable()                               //Выключить функции
        {
            printFile.IsEnabled = true;
            saveFile.IsEnabled = true;
            saveFileAs.IsEnabled = true;
        }
        public void openFile_Click(object sender, RoutedEventArgs e)                                                    //ОТКРЫТЬ ФАЙЛ
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFilePath = openFileDialog.FileName;
                // Выполните необходимые действия с выбранным файлом
                ShowFunctions();
                FuncSetEnable();
            }
            else
            {
                MessageBox.Show("Не смог открыть файл!", "Предупреждение");
            }
        }
        public void addDiscipline_Click(object sender, RoutedEventArgs e)
        {
            addDiscipline discipline = new addDiscipline();
            discipline.Show();
        }
        public void newFile_Click(object sender, RoutedEventArgs e)                                                 //РЕАЛИЗОВАТЬ ФУНКЦИЮ ДЛЯ НОВОГО ФАЙЛА
        {
            ShowFunctions();
            FuncSetEnable();
        }
        public static void SaveToJsonFile<T>(T data, string filePath)                                       //РЕАЛИЗОВАТЬ ФУНКЦИЮ ДО ЛОГИЧЕСКОГО КОНЦА
        {
            string jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, jsonString);
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
            {
                textBlock.Text = $"Ваш коэффициент {Convert.ToString(result)}.";
                ColorSet(result);
                TextAdd(result);
            }
            else
                textBlock.Text = "Ошибка при вводе данных!";
        }
        public void SelectionFunction(object sender, SelectionChangedEventArgs e)                     //обработка события, когда мы выводим курсор из выпадающего списка
        {
            ClearAll();
            if (disciplineType.SelectedIndex == 0)
            {
                //GroupBox resultHead = new GroupBox();
                resultHead = new GroupBox();
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
                pulseHead = new GroupBox();
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
