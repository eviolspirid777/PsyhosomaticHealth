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
using System.Threading;

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

        public double minValue = 0;         //переменная, определяющая минимальное значение
        public double maxValue = 0;     //переменная, определяющая максимальное значение
        public int counter = 3;         //переменная, определяющая счетчик

        public PsyhHealth()
        {
            InitializeComponent();
            disciplineType.Items.Add("Состояние относительного покоя");
			disciplineType.Items.Add("Биоэкономичная психомотрная гимнастика");
			disciplineType.Items.Add("Циклические виды");
			disciplineType.Items.Add("Ациклические виды");
			disciplineType.Items.Add("Смешанные виды");
			//AddDisciplines();

		}
		public void ShowFunctions()
        {
            setDiscipline.Visibility = Visibility.Visible;          //показывает поле для выбора дисциплины
            getResult.Visibility = Visibility.Visible;              //показывает клавишу получения результата
        }
        public void ColorSet( double result)                                                            //ОКРАШИВАЕТ В ЦВЕТ
        {
            if (result > 1.618)
                colorBlock.Background = new SolidColorBrush(Colors.Gold);
            else if (result >= 1.600 && result <= 1.618)
                colorBlock.Background = new SolidColorBrush(Colors.Green);                        //Золотая пропорция
            else if (result >= 1.250 && result <= 1.599)
                colorBlock.Background = new SolidColorBrush(Colors.GreenYellow);            //Большой резерв
            else if (result >= 1 && result <= 1.249)
                colorBlock.Background = new SolidColorBrush(Colors.YellowGreen);          //Малый резерв
            else if (result >= 0.850 && result <= 0.999)
                colorBlock.Background = new SolidColorBrush(Colors.Yellow);                  //Малый дефицит
            else if (result >= 0.618 && result <= 0.849)
                colorBlock.Background = new SolidColorBrush(Colors.DarkRed);            //Большой дефицит
            else if (result <= 0.617)
                colorBlock.Background = new SolidColorBrush(Colors.Red);                  //Предельная энергостоимость

		}
        public void TextAdd(double result)                                                  //ОЦЕНИВАЕТ ХАРАКТЕРИСТИКУ ПО РЕЗУЛЬТАТУ
        {
            textBlock.Text += "\nКачественная характеристика: ";        //Начало любой КАЧЕСТВЕННОЙ характеристики

            if (result > 1.618)
                textBlock.Text += "За пределом ЗОЛОТОЙ ПРОПОРЦИИ - предельная негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается стабильным целостным благоприятным состоянием организма, чувством удовлетворения, психологического и телесного комфорта. Характеризует оптимальный максимум гармоничности и экономичности жизнедеятельности организма. Объём выполняемой психомоторной нагрузки характеризуется возможностью её оптимального увеличения в соответствии с индивидуальными адаптационными возможностями организма в диапазоне негэнтропийных энергетических затрат.";
            if (result >= 1.600 && result <= 1.618)
                textBlock.Text += "ЗОЛОТАЯ ПРОПОРЦИЯ - –наивысшая негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается стабильным целостным благоприятным состоянием организма, чувством удовлетворения, эйфории, психологического и телесного комфорта. Характеризует оптимальный максимум гармоничности и экономичности жизнедеятельности организма. Выполняемый объём психомоторной нагрузки полностью соответствует оптимальным индивидуальным адаптационным возможностям организма.";
            if (result >= 1.250 && result <= 1.599)
                textBlock.Text += "БОЛЬШОЙ РЕЗЕРВ - устойчиво высокая негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается стабильным благоприятным состоянием функциональных систем организма, устойчивым режимом экономичного дыхания в покое и активной деятельности с постепенным прогрессирующим снижением секундных объемов легочной вентиляции, сопровождается чувством психологического и телесного комфорта. Выполняемый объём психомоторной нагрузки полностью соответствует оптимальным индивидуальным адаптационным возможностям организма.";
            if (result >= 1 && result <= 1.249)
                textBlock.Text += "МАЛЫЙ РЕЗЕРВ - начальная и умеренно- средняя негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается оптимальным состоянием функциональных систем организма, отсутствием признаков нехватки воздуха и потребности в увеличении объемов легочной вентиляции, устойчивостью психических реакций, отсутствием напряжения, дискомфортных и болезненных ощущений. Выполняемый объём психомоторной нагрузки соответствует индивидуальным адаптационным возможностям организма.";
            if (result >= 0.850 && result <= 0.999)
                textBlock.Text += "МАЛЫЙ ДЕФИЦИТ - начальная и умеренно-средняя энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается напряжением в работе кардио-респираторной и нервной систем, ощущением нехватки воздуха, снижением психических реакций, чувством скованности и тяжести в мышечно-связочном аппарате, в ряде случаев показана смена вида деятельности. Кратковременное нахождение организма в диапазоне малого дефицита не вызывает выраженных нарушений.";
            if (result >= 0.618 && result <= 0.849)
                textBlock.Text += "БОЛЬШОЙ ДЕФИЦИТ - высокая энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается значительным напряжением в работе кардио-респираторной, нервной и вегетативной систем, нехваткой воздуха и отдышкой, выраженным сердцебиением, угнетением психических реакций, тяжестью в голове и головокружением, болезненными ощущениями в мышечно-связочном и костном аппаратах вплоть до возникновения объективной потребности в отказе от осуществления деятельности.";
            if (result <= 0.617)
                textBlock.Text += "предельная энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается чрезмерным напряжением функционирования кардио- респираторной, нервной и вегетативной систем, приступами удушья и нехватки воздуха, подавлением психических реакций, головокружением и тяжестью в голове, тошнотой, рвотой, болями и страданиями вплоть до полного отказа от осуществляемой деятельности.";

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
        public void addDiscipline_Click(object sender, RoutedEventArgs e)                               //ОБРАБОТКА ДОБАВЛЕНИЯ
        {
            addDiscipline discipline = new addDiscipline();
            discipline.Show();
            Close();
        }
        public void newFile_Click(object sender, RoutedEventArgs e)                                                 //СОЗДАНИЕ НОВОГО ФАЙЛА
        {
            ShowFunctions();
            FuncSetEnable();
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
                        textBlock.Text = $"Колличественный показатель {Convert.ToString(result)}.";
                        ColorSet(result);
                        TextAdd(result);
                    }
                    else
                        textBlock.Text = "Ошибка при вводе данных!";
                }
				else
				{
					MessageBox.Show("Ошибка с ЧСС!", "Неверно задана ЧСС");
				}
			}

			else if (disciplinesTemp.Any(temp => temp.title.ToString() == disciplineTypeContent.SelectedItem.ToString()))               //Проверка на совпадения элемента в выпадающем списке  с элементом в векторе
            {
                if ((Convert.ToInt32(pulseBox.Text) >= 10 && Convert.ToInt32(pulseBox.Text) <= 36 && minSwitcher.IsChecked == false) || (Convert.ToInt32(pulseBox.Text) >= 60 && Convert.ToInt32(pulseBox.Text) <= 216 && minSwitcher.IsChecked == true))       //проверка границ ПУЛЬСА
                {
                    foreach (DisciplinesTypes temp in disciplinesTemp)
                    {
                        if (temp.title.ToString() == disciplineTypeContent.SelectedItem.ToString())
                        {
                            if (temp.dirProp == false)                                      //прямая прогрессия
                            {
                                minValue = temp.maxValue * 100 / 161.8;         //находим минимальное значение
                                maxValue = temp.maxValue;
                                while (result == 0)
                                {
                                    if (Convert.ToDouble(resultBox.Text) >= minValue && counter > 0 && Convert.ToDouble(resultBox.Text) <= maxValue)        //проверка результата на минимальную границу(прямая прогрессия)
                                    {
                                        if (minSwitcher.IsChecked == false)             //для секунд
                                        {
                                            double percentValueProduct = 161.8 * double.Parse(resultBox.Text) / maxValue;
                                            double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 14;
                                            result = percentValueProduct / percentValueEnergy;
                                            result = Ceiling(result);
											break;
                                        }
                                        else                                                            //для минут
                                        {
                                            double percentValueProduct = 161.8 * double.Parse(resultBox.Text) / maxValue;
                                            double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 84;
                                            result = percentValueProduct / percentValueEnergy;
                                            result = Ceiling(result);
											break;
                                        }
                                    }
                                    else if (Convert.ToDouble(resultBox.Text) <= minValue && counter > 0)                                                      //переход на следующий уровень
                                    {
                                        counter--;      //увеличиваем счетчик на единицу(переходим на следующий уровень)
                                        maxValue = minValue;       //Присваиваем максимальному значению - минимальное
                                        minValue = maxValue * 100 / 161.8;             //расчитываем новое минимальное
                                    }
                                    else                                                                                                                                                                            //обработка, если мы вышли за нормы показателя
                                    {
                                        MessageBox.Show("Ошибка с колличественным показателем", "Ошибка при вычислении колличественного показателя!");
                                        break;
                                    }
                                }
                            }
                            else                                                    //обратная прогрессия  ДОПИСАТЬ!! ХОД МЫСЛЕЙ ТОТ ЖЕ!!!
                            {
								resultBox.Text = resultBox.Text.Replace(".", ",");
								minValue = temp.maxValue * 161.8 / 100;
								maxValue = temp.maxValue;
                                while (result == 0)
                                {
                                    if (Convert.ToDouble(resultBox.Text) <= minValue && counter > 0 && Convert.ToDouble(resultBox.Text) >= maxValue)            //проверка результата на минимальную границу(обратная прогрессия)
                                    {
                                        if (minSwitcher.IsChecked == false)
                                        {
                                            double percentValueProduct = 161.8 * maxValue / double.Parse(resultBox.Text);
                                            double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 14;
                                            result = percentValueProduct / percentValueEnergy;
                                            result = Ceiling(result);
											break;
                                        }
                                        else
                                        {
                                            double percentValueProduct = 161.8 * maxValue / double.Parse(resultBox.Text);
                                            double percentValueEnergy = double.Parse(pulseBox.Text) * 100 / 84;
                                            result = percentValueProduct / percentValueEnergy;
                                            result = Ceiling(result);
                                            break;
                                        }
                                    }
                                    else if(Convert.ToDouble(resultBox.Text) >= minValue && counter > 0)               //переход на следующий уровень
                                    {
										counter--;      //увеличиваем счетчик на единицу(переходим на следующий уровень)
										maxValue = minValue;       //Присваиваем максимальному значению - минимальное
										minValue = maxValue * 161.8 / 100;             //расчитываем новое минимальное
									}
                                    else
                                    {
                                        MessageBox.Show("Ошибка с колличественным показателем", "Ошибка при вычислении колличественного показателя!");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
					MessageBox.Show("Ошибка с ЧСС!", "Неверно задана ЧСС");
				}
                if (result != 0)
                {
                    colorBlock.Visibility = Visibility.Visible;
                    textBlock.Text = $"Колличественный показатель: {Convert.ToString(result)}. ";
					ColorSet(result);
                    TextAdd(result);
                    sportBlock.Text = $"Ваш уровень продуктивности: C{counter}";
					counter = 3;                                //Возвращаем счетчик
				}
            }
        }
        public void HelpWindowClick(object sender, EventArgs e)                 //ОБРАБОТКА НАЖАТИЯ НА ОКНО ПОМОЩИ
        {
            HelpWIndow helpWIndow = new HelpWIndow();
            helpWIndow.Show();                                                                  //Реализовать отключение
        }
		public void SelectionFunction(object sender, SelectionChangedEventArgs e)                     //обработка события, когда мы выводим курсор из выпадающего списка
        {
            ClearAll();
            if (disciplineType.SelectedIndex == 0)                                                                                      //Состояние относительного покоя
            {
				setDisciplineContent.Visibility = Visibility.Hidden;        //скрывает второй комбобокс
				minSwitcher.Visibility = Visibility.Visible;
                //GroupBox resultHead = new GroupBox();
                resultHead = new GroupBox();
                resultHead.Name = "typeHead";
                resultHead.Header = "Исходное положение";
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
                pulseHead.Header = "ЧСС";
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
            else if( disciplineType.SelectedIndex == 1)                                                         //Биоэкономическая психомотрная гимнастика
            {
                addFunction(1);
				setDisciplineContent.Visibility = Visibility.Visible;       //показывает второй комбобокс
				minSwitcher.Visibility = Visibility.Visible;
                // Создание первого GroupBox
                GroupBox resultHead = new GroupBox();
                resultHead.Name = "resultHead";
                resultHead.Header = "Продуктивность";
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
                pulseHead.Header = "ЧСС";
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
			else if (disciplineType.SelectedIndex == 2)                                                                                 //Циклические виды
			{
                addFunction(2);
				setDisciplineContent.Visibility = Visibility.Visible;       //показывает второй комбобокс
				minSwitcher.Visibility = Visibility.Visible;
				// Создание первого GroupBox
				GroupBox resultHead = new GroupBox();
				resultHead.Name = "resultHead";
				resultHead.Header = "Продуктивность";
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
				pulseHead.Header = "ЧСС";
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
			else if (disciplineType.SelectedIndex == 3)                                                                                             //Ациклические виды
			{
                addFunction(3);
				setDisciplineContent.Visibility = Visibility.Visible;       //показывает второй комбобокс
				minSwitcher.Visibility = Visibility.Visible;
				// Создание первого GroupBox
				GroupBox resultHead = new GroupBox();
				resultHead.Name = "resultHead";
				resultHead.Header = "Продуктивность";
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
				pulseHead.Header = "ЧСС";
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
			else if (disciplineType.SelectedIndex == 4)                                                                                 //Смешанные виды
			{
                addFunction(4);
				setDisciplineContent.Visibility = Visibility.Visible;       //показывает второй комбобокс
				minSwitcher.Visibility = Visibility.Visible;
				// Создание первого GroupBox
				GroupBox resultHead = new GroupBox();
				resultHead.Name = "resultHead";
				resultHead.Header = "Продуктивность";
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
				pulseHead.Header = "ЧСС";
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
        public double SetResultSec(int coef = 1)                                                    //ФУНКЦИЯ ВЫЧИСЛЕНИЯ В СЕКУНДАХ
        {
            if (double.TryParse(pulseBox.Text, out double temp))
                return coef / (100 * temp / 12);
            else
                return 0;
        }
        public double SetResultMin(int coef = 1)                                            //ФУНКЦИЯ ВЫЧИСЛЕНИИ В СЕКУНДАХ
        {
			if (double.TryParse(pulseBox.Text, out double temp))
				return coef / (100 * temp / 72);
			else
				return 0;
		}
        public void ClearAll()                                                          //ОЧИСТИТЬ ВСЕ ПОЛЯ
        {
			disciplineTypeContent.Items.Clear();
			colorBlock.Visibility = Visibility.Hidden;
            pulseBox.Text = string.Empty;
            resultBox.Text = string.Empty;
            textBlock.Text = string.Empty;
            comboBox.Items.Clear();
            stackPanel.Children.Clear();
            sportBlock.Text =string.Empty;
        }
        enum HumanState                                 //ПЕРЕЧИСЛЕНИЕ СОСТОЯНИЙ
        {
            lying = 100,
            sitting = 103,
            staying = 106
        }
        public double Ceiling(double result)
        {
            return Math.Ceiling(result * 1000) / 1000;
        }
        public void addFunction(int number)
        {
			List<DisciplinesTypes> temp = new List<DisciplinesTypes>();
			FileFunct.ReadData(out temp);
				foreach (DisciplinesTypes ex in temp)
					if (ex.number == number)
						disciplineTypeContent.Items.Add(ex.title);
		}
    }
}
