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
using Disciplines;
using FileWorking;

namespace PsyhosomaticHealth
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PsyhHealth : Window
    {
        public const double c_deficitLMin = 0.618;
        public const double c_deficitLMax = 0.849;
        public const double c_deficitBMin = 0.850;
        public const double c_deficitBMax = 0.999;
        public const double c_balance = 1;
        public const double c_reserveLMin = 1.001;
        public const double c_reserveLMax = 1.249;
        public const double c_reserveBMin = 1.250;
        public const double c_reserveBMax = 1.599;
        public const double c_goldProportionMin = 1.600;
        public const double c_goldProportionMax = 1.618;
        ComboBox comboBox = new ComboBox();
        GroupBox resultHead = new GroupBox();
        GroupBox pulseHead = new GroupBox();
        TextBox pulseBox = new TextBox();           //Значение текстового поля pusleBox
        TextBox resultBox = new TextBox();          //Значение текстового поля resultbox

		public PsyhHealth()
        {
            InitializeComponent();
            disciplineType.Items.Add("Состояние относительного покоя");
            AddDisciplines();
            
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
            Close();
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
            List<DisciplinesTypes> disciplinesTemp = new List<DisciplinesTypes>();
            FileFunct.ReadData(out  disciplinesTemp);
            double result = 0;
            if (disciplineType.SelectedIndex == 0)
            {
				if ((Convert.ToInt32(pulseBox.Text) >= 8 && Convert.ToInt32(pulseBox.Text) <= 21 && minSwitcher.IsChecked == false) || (Convert.ToInt32(pulseBox.Text) >= 48 && Convert.ToInt32(pulseBox.Text) <= 126 && minSwitcher.IsChecked == true))
				{
					switch (comboBox.Text)
                    {
                        case "Сидя":
                            if (minSwitcher.IsChecked == false)
                            {
                                result = SetResultSec(((int)HumanState.sitting));
                                break;
                            }
                            else
                            {
								result = SetResultMin(((int)HumanState.sitting));
								break;
							}
                        case "Стоя":
							if (minSwitcher.IsChecked == false)
							{
								result = SetResultSec(((int)HumanState.staying));
								break;
							}
							else
							{
								result = SetResultMin(((int)HumanState.staying));
								break;
							}
						case "Лежа":
							if (minSwitcher.IsChecked == false)
							{
								result = SetResultSec(((int)HumanState.lying));
								break;
							}
							else
							{
								result = SetResultMin(((int)HumanState.lying));
								break;
							}
					}
                    result = Ceiling(result);
                    if (result != 0)
                    {
                        colorBlock.Visibility = Visibility.Visible;
                        textBlock.Text = $"Ваш коэффициент {Convert.ToString(result)}.";
                        ColorSet(result);
                        TextAdd(result);
                    }
                    else
                        textBlock.Text = "Ошибка при вводе данных!";
                }
				else
				{
					MessageBox.Show("Ошибка с пульсом!", "Неверно задан пульс");
				}
			}

			if (disciplinesTemp.Any(temp => temp.title.ToString() == disciplineType.SelectedItem.ToString()))               //Проверка на совпадения элемента в выпадающем списке  с элементом в векторе
            {
                if ((Convert.ToInt32(pulseBox.Text) >= 10 && Convert.ToInt32(pulseBox.Text) <= 36 && minSwitcher.IsChecked == false) || (Convert.ToInt32(pulseBox.Text) >= 60 && Convert.ToInt32(pulseBox.Text) <= 216 && minSwitcher.IsChecked == true))
                {
                    foreach (DisciplinesTypes temp in disciplinesTemp)
                    {
                        if (temp.title.ToString() == disciplineType.SelectedItem.ToString())
                        {
                            if (temp.dirProp == false)
                            {
                                if (minSwitcher.IsChecked == false)
                                {
                                    double percentValueProduct = 161.8 * double.Parse(resultBox.Text) / temp.maxValue;
                                    double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 14;
                                    result = percentValueProduct / percentValueEnergy;
                                    result = Ceiling(result);
                                    break;
                                }
                                else
                                {
									double percentValueProduct = 161.8 * double.Parse(resultBox.Text) / temp.maxValue;
									double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 84;
									result = percentValueProduct / percentValueEnergy;
									result = Ceiling(result);
									break;
								}
                            }
                            else
                            {
                                if (minSwitcher.IsChecked == false)
                                {
                                    double percentValueProduct = 161.8 * temp.maxValue / double.Parse(resultBox.Text);
                                    double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 14;
                                    result = percentValueProduct / percentValueEnergy;
                                    result = Ceiling(result);
                                    break;
                                }
                                else
                                {
									double percentValueProduct = 161.8 * temp.maxValue / double.Parse(resultBox.Text);
									double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 84;
									result = percentValueProduct / percentValueEnergy;
									result = Ceiling(result);
									break;
								}
                            }
                        }
                    }
                }
                else
                {
					MessageBox.Show("Ошибка с пульсом!", "Неверно задан пульс");
				}
                if (result != 0)
                {
                    colorBlock.Visibility = Visibility.Visible;
                    textBlock.Text = $"Ваш коэффициент {Convert.ToString(result)}.";
                    ColorSet(result);
                    TextAdd(result);
                }
            }
        }
        public void HelpWindowClick(object sender, EventArgs e)
        {
            HelpWIndow helpWIndow = new HelpWIndow();
            helpWIndow.Show();
        }
        public void SelectionFunction(object sender, SelectionChangedEventArgs e)                     //обработка события, когда мы выводим курсор из выпадающего списка
        {
            ClearAll();
            if (disciplineType.SelectedIndex == 0)
            {
                minSwitcher.Visibility = Visibility.Visible;
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
            else
            {
                minSwitcher.Visibility = Visibility.Visible;
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
        public double SetResultSec(int coef = 1)
        {
            if (double.TryParse(pulseBox.Text, out double temp))
                return coef / (100 * temp / 12);
            else
                return 0;
        }
        public double SetResultMin(int coef = 1)
        {
			if (double.TryParse(pulseBox.Text, out double temp))
				return coef / (100 * temp / 72);
			else
				return 0;
		}
        public void ClearAll()
        {
            colorBlock.Visibility = Visibility.Hidden;
            pulseBox.Text = string.Empty;
            resultBox.Text = string.Empty;
            textBlock.Text = string.Empty;
            comboBox.Items.Clear();
            stackPanel.Children.Clear();
        }
        enum HumanState
        {
            lying = 100,
            sitting = 103,
            staying = 106
        }
        public void AddDisciplines()
        {
            List <DisciplinesTypes> temp = new List <DisciplinesTypes>();
            FileFunct.ReadData(out  temp);
            foreach(DisciplinesTypes type in temp)
                disciplineType.Items.Add(type.title);
        }
        public double Ceiling(double result)
        {
            return Math.Ceiling(result * 1000) / 1000;
        }
    }
}
