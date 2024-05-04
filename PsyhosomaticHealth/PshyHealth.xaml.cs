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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PsyhosomaticHealth
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class PsyhHealth : Window
	{
		public List<DisciplinesTypes> disciplineList;
		public double minValue = 0;         //переменная, определяющая минимальное значение
		public double maxValue = 0;     //переменная, определяющая максимальное значение
		public int counter = 3;         //переменная, определяющая счетчик

		private bool isDragging = false;
		private Point lastPosition;

		public PsyhHealth()
		{
			InitializeComponent();
			disciplineType.Items.Add("Состояние относительного покоя");
			disciplineType.Items.Add("Биоэкономичная психомотрная гимнастика");
			disciplineType.Items.Add("Циклические виды");
			disciplineType.Items.Add("Ациклические виды");
			disciplineType.Items.Add("Смешанные виды");
			SizeChanged += Window_SizeChanged;
			DiagramWindow diagramWindow = new DiagramWindow();
			diagramWindow.Show();
			//AddDisciplines();
		}

		#region FontSizeChanged
		double previousScaleFactor;
		bool flag = false;

		/// <summary>
		/// Функция для расчета размера окна
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Grid grid = QWe;
			if(e.NewSize.Width > e.PreviousSize.Width)
			{
				double scaleFactorIncrease = CalculateScaleFactor(e.PreviousSize, e.NewSize);
				foreach (var textBlock in FindVisualChildren<TextBlock>(ratioTable))
				{
					double currentFontSize = textBlock.FontSize;
					textBlock.FontSize = currentFontSize * scaleFactorIncrease;
				}

				if(flag)
				{
					previousScaleFactor = scaleFactorIncrease;
					grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
					grid.ColumnDefinitions[1].Width = new GridLength(6, GridUnitType.Star);
				}
				flag = true;
			}

			else
			{
				double scaleFactorDecrease = CalculateScaleFactorDecrease(e.PreviousSize, e.NewSize);
				foreach (var textBlock in FindVisualChildren<TextBlock>(ratioTable))
				{
					double currentFontSize = textBlock.FontSize;
					textBlock.FontSize = currentFontSize * scaleFactorDecrease;
				}
				previousScaleFactor = scaleFactorDecrease;
				grid.ColumnDefinitions[1].Width = new GridLength(3, GridUnitType.Star);
			}
		}
		/// <summary>
		/// Высчитывает коэфициент увеличения/уменьшения
		/// </summary>
		/// <param name="previousSize">предыдущий размер</param>
		/// <param name="newSize">новый размер</param>
		/// <returns>коэфициент</returns>
		private double CalculateScaleFactor(Size previousSize, Size newSize)
		{
			if(previousSize.Width == 0 && previousSize.Height == 0)
			{
				previousSize.Width = newSize.Width;
				previousSize.Height = newSize.Height;
			}

			var coef = 0.99;

			double scaleFactorWidth = newSize.Width / previousSize.Width * coef;
			double scaleFactorHeight = newSize.Height / previousSize.Height * coef;
			return Math.Min(scaleFactorWidth, scaleFactorHeight);
		}
		private double CalculateScaleFactorDecrease(Size previousSize, Size newSize)
		{
			if (previousSize.Width == 0 && previousSize.Height == 0)
			{
				previousSize.Width = newSize.Width;
				previousSize.Height = newSize.Height;
			}

			var coef = 1.01;

			double scaleFactorWidth = newSize.Width / previousSize.Width * coef;
			double scaleFactorHeight = newSize.Height / previousSize.Height * coef;
			return Math.Min(scaleFactorWidth, scaleFactorHeight);
		}
		/// <summary>
		/// выборка всех дочерних компонентов
		/// </summary>
		/// <typeparam name="T">Тип тега</typeparam>
		/// <param name="depObj">Объект в котором содержатся теги</param>
		/// <returns></returns>
		private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}
		#endregion FontSizeChanged
		#region Validators
		/// <summary>
		/// проверка границ ПУЛЬСА
		/// </summary>
		public bool PulseValidate(TextBox pulseTextBox)
		{
			if(pulseTextBox.Text.Length > 0)
				return (Convert.ToInt32(pulseTextBox.Text) >= 10 && Convert.ToInt32(pulseTextBox.Text) <= 36 && minSwitcher.IsChecked == false) || (Convert.ToInt32(pulseTextBox.Text) >= 60 && Convert.ToInt32(pulseTextBox.Text) <= 216 && minSwitcher.IsChecked == true);
			else return false;
		}
		/// <summary>
		/// проверка результата на минимальную границу(прямая прогрессия)
		/// </summary>
		/// <returns></returns>
		public bool MinRateStraight(TextBox resultTextBox) 
		{
			return Convert.ToDouble(resultTextBox.Text) >= minValue && counter > 0 && Convert.ToDouble(resultTextBox.Text) <= maxValue;
		}
		/// <summary>
		/// проверка результата на минимальную границу(обратная прогрессия)
		/// </summary>
		/// <param name="resultTextBox"></param>
		/// <returns></returns>
		public bool MinRateReverse(TextBox resultTextBox)
		{
			return Convert.ToDouble(resultTextBox.Text) <= minValue && counter > 0 && Convert.ToDouble(resultTextBox.Text) >= maxValue;
		}
		/// <summary>
		/// Переход на следующую границу (прямая прогрессия)
		/// </summary>
		/// <param name="resultTextBox"></param>
		/// <returns></returns>
		public bool NextRateStraight(TextBox resultTextBox)
		{
			return Convert.ToDouble(resultTextBox.Text) <= minValue && counter > 0;
		}
		/// <summary>
		/// Переход на следующую границу (обратная прогрессия)
		/// </summary>
		/// <param name="resultTextBox"></param>
		/// <returns></returns>
		public bool NextRateReverse(TextBox resultTextBox)
		{
			return Convert.ToDouble(resultTextBox.Text) >= minValue && counter > 0;
		}
		#endregion Validators
		#region EnumDictionaries
		public enum ratioStates
		{
			over_gold = 0,
			gold,
			big_reserve,
			low_reserve,
			low_defecit,
			big_deficit,
			below_low
		}
        enum HumanState                                 //ПЕРЕЧИСЛЕНИЕ СОСТОЯНИЙ
        {
            lying = 100,
            sitting = 103,
            staying = 106
        }

        public readonly Dictionary<ratioStates, string> data = new Dictionary<ratioStates, string>
		{
			[ratioStates.over_gold] = "Выше ЗОЛОТОЙ ПРОПОРЦИИ - предельная негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается стабильным целостным благоприятным состоянием организма, чувством удовлетворения, психологического и телесного комфорта. Характеризует оптимальный максимум гармоничности и экономичности жизнедеятельности организма. Объём выполняемой психомоторной нагрузки характеризуется возможностью её оптимального увеличения в соответствии с индивидуальными адаптационными возможностями организма в диапазоне негэнтропийных энергетических затрат.",
			[ratioStates.gold] = "ЗОЛОТАЯ ПРОПОРЦИЯ - –наивысшая негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается стабильным целостным благоприятным состоянием организма, чувством удовлетворения, эйфории, психологического и телесного комфорта. Характеризует оптимальный максимум гармоничности и экономичности жизнедеятельности организма. Выполняемый объём психомоторной нагрузки полностью соответствует оптимальным индивидуальным адаптационным возможностям организма.",
			[ratioStates.big_reserve] = "БОЛЬШОЙ РЕЗЕРВ - устойчиво высокая негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается стабильным благоприятным состоянием функциональных систем организма, устойчивым режимом экономичного дыхания в покое и активной деятельности с постепенным прогрессирующим снижением секундных объемов легочной вентиляции, сопровождается чувством психологического и телесного комфорта. Выполняемый объём психомоторной нагрузки полностью соответствует оптимальным индивидуальным адаптационным возможностям организма.",
			[ratioStates.low_reserve] = "МАЛЫЙ РЕЗЕРВ - начальная и умеренно- средняя негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается оптимальным состоянием функциональных систем организма, отсутствием признаков нехватки воздуха и потребности в увеличении объемов легочной вентиляции, устойчивостью психических реакций, отсутствием напряжения, дискомфортных и болезненных ощущений. Выполняемый объём психомоторной нагрузки соответствует индивидуальным адаптационным возможностям организма.",
			[ratioStates.low_defecit] = "МАЛЫЙ ДЕФИЦИТ - начальная и умеренно-средняя энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается напряжением в работе кардио-респираторной и нервной систем, ощущением нехватки воздуха, снижением психических реакций, чувством скованности и тяжести в мышечно-связочном аппарате, в ряде случаев показана смена вида деятельности. Кратковременное нахождение организма в диапазоне малого дефицита не вызывает выраженных нарушений.",
			[ratioStates.big_deficit] = "БОЛЬШОЙ ДЕФИЦИТ - высокая энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции: - сопровождается значительным напряжением в работе кардио-респираторной, нервной и вегетативной систем, нехваткой воздуха и отдышкой, выраженным сердцебиением, угнетением психических реакций, тяжестью в голове и головокружением, болезненными ощущениями в мышечно-связочном и костном аппаратах вплоть до возникновения объективной потребности в отказе от осуществления деятельности.",
			[ratioStates.below_low] = "ЗА НИЖНЕЙ ГРАНИЦЕЙ ШКАЛЫ ЗОЛОТОЙ ПРОПОРЦИИ: - сопровождается чрезмерным напряжением функционирования кардио- респираторной, нервной и вегетативной систем, приступами удушья и нехватки воздуха, подавлением психических реакций, головокружением и тяжестью в голове, тошнотой, рвотой, болями и страданиями вплоть до полного отказа от осуществляемой деятельности."
		};

		public readonly Dictionary<ratioStates, string> ratios = new Dictionary<ratioStates, string>
		{
			[ratioStates.over_gold] = "Выше золотой пропорции",
			[ratioStates.gold] = "Золотая пропорция",
			[ratioStates.big_reserve] = "Большой резерв",
			[ratioStates.low_reserve] = "Малый резерв",
			[ratioStates.low_defecit] = "Малый дефицит",
			[ratioStates.big_deficit] = "Большой дефицит",
			[ratioStates.below_low] = "Ниже шкалы золотой пропорции"
		};

		public readonly Dictionary<ratioStates, string> state1 = new Dictionary<ratioStates, string>
		{
			[ratioStates.over_gold] = "Выше золотой пропорции (1,619 - выше)",
			[ratioStates.gold] = "Золотая пропорция (1,600 - 1,618)",
			[ratioStates.big_reserve] = "Большой резерв (1,237 - 1,599)",
			[ratioStates.low_reserve] = "Малый резерв (1,000 - 1,236)",
			[ratioStates.low_defecit] = "Малый дефицит (0,999 - 0,854",
			[ratioStates.big_deficit] = "Большой дефицит (0,853 - 0,618)",
			[ratioStates.below_low] = "Ниже шкалы золотой пропорции (0,617 - ниже)"
		};

		public readonly Dictionary<ratioStates, string> state2 = new Dictionary<ratioStates, string>
		{
			[ratioStates.over_gold] = " - переходная негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции",
			[ratioStates.gold] = " - наивысшая негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции",
			[ratioStates.big_reserve] = " - резистентная и высокая негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции",
			[ratioStates.low_reserve] = " - легкая и умеренная негэнтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции",
			[ratioStates.low_defecit] = " - легкая и умеренная энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции",
			[ratioStates.big_deficit] = " - средняя и высокая энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции",
			[ratioStates.below_low] = " - предельная энтропийная энергостоимость выполняемой психомоторной деятельности в шкале золотой пропорции"
		};

		public readonly Dictionary<string, Color> colours = new Dictionary<string, Color>()
		{
			["gold"] = Colors.Gold,
			["green"] = Colors.Green,
			["green_yellow"] = Colors.GreenYellow,
			["yellow_green"] = Colors.YellowGreen,
			["yellow"] = Colors.Yellow,
			["dark_red"] = Colors.DarkRed,
			["red"] = Colors.Red
		};
        #endregion EnumDictionaries
        #region constants
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
        #endregion constants
        #region resultPulseBoxes
        const int countOfFieds = 3;
		/*GroupBox resultGroupBox = new GroupBox();*/
		ComboBox comboBox = new ComboBox();
		GroupBox [] resultGroupBoxes = new GroupBox[countOfFieds];
		GroupBox [] pulseGroupBoxes = new GroupBox[countOfFieds];
		TextBox [] pulseTextBoxes = new TextBox[countOfFieds];
		TextBox [] resultTextBoxes = new TextBox[countOfFieds];
		/*GroupBox pulseGroupBox = new GroupBox();*/
		/*		TextBox pulseTextBox = new TextBox();           //Значение текстового поля pusleBox
				TextBox resultTextBox = new TextBox();          //Значение текстового поля resultTextBox*/
		#endregion resultPusleBoxes
		#region FuncSwitchers
		public void ShowFunctions()
		{
			setDiscipline.Visibility = Visibility.Visible;          //показывает поле для выбора дисциплины
			getResult.Visibility = Visibility.Visible;              //показывает клавишу получения результата

		}

		/// <summary>
		/// Включает все функции в выпадающем списке Меню
		/// </summary>
		public void FuncSetEnable()
		{
			printFile.IsEnabled = true;
			/*printFile.Cursor = Cursors.;  Вопрос с курсорами нужно еще решить, когда файл неактивен*/
			saveFile.IsEnabled = true;
			saveFileAs.IsEnabled = true;
		}

		/// <summary>
		/// Очистить все поля
		/// </summary>
		public void ClearAll()
		{
			disciplineTypeContent.Items.Clear();
			for (int i = 0; i < countOfFieds; i++)
			{
				pulseTextBoxes[i].Text = string.Empty;
				resultTextBoxes[i].Text = string.Empty;
			}
			comboBox.Items.Clear();
			stackPanel.Children.Clear();
		}

		public void addFunction(int number)
		{
			List<DisciplinesTypes> temp = new List<DisciplinesTypes>();
			FileFunct.ReadData(out temp);
			foreach (DisciplinesTypes ex in temp)
				if (ex.number == number)
					disciplineTypeContent.Items.Add(ex.title);
		}
		#endregion FuncSwitchers
		#region ColorSetFunctions
		public void ColorSet(double result, int i)                                                            //ОКРАШИВАЕТ В ЦВЕТ
		{
			switch (i)
			{
				case 0:
                    if (result > 1.618)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["gold"]);
                        buttonStart.Background = new SolidColorBrush(colours["gold"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["gold"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["gold"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["gold"]);
					}
                    else if (result >= 1.600 && result <= 1.618)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["green"]);
                        buttonStart.Background = new SolidColorBrush(colours["green"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["green"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["green"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["green"]);
					}
					else if (result >= 1.250 && result <= 1.599)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["green_yellow"]);
                        buttonStart.Background = new SolidColorBrush(colours["green_yellow"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
					}
					else if (result >= 1 && result <= 1.249)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["yellow_green"]);
                        buttonStart.Background = new SolidColorBrush(colours["yellow_green"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
					}
					else if (result >= 0.850 && result <= 0.999)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["yellow"]);
                        buttonStart.Background = new SolidColorBrush(colours["yellow"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["yellow"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["yellow"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["yellow"]);
					}
                    else if (result >= 0.618 && result <= 0.849)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["dark_red"]);
                        buttonStart.Background = new SolidColorBrush(colours["dark_red"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["dark_red"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["dark_red"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["dark_red"]);
					}
                    else if (result <= 0.617)
                    {
                        startTraining.Foreground = new SolidColorBrush(colours["red"]);
                        buttonStart.Background = new SolidColorBrush(colours["red"]);
                        qualValueStart.Foreground = new SolidColorBrush(colours["red"]);
                        quanValueStart.Foreground = new SolidColorBrush(colours["red"]);
						resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["red"]);
					}
                    break;
				case 1:
                    if (result > 1.618)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["gold"]);
                        buttonCenter.Background = new SolidColorBrush(colours["gold"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["gold"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["gold"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["gold"]);
					}
                    else if (result >= 1.600 && result <= 1.618)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["green"]);
                        buttonCenter.Background = new SolidColorBrush(colours["green"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["green"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["green"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["green"]);
					}
					else if (result >= 1.250 && result <= 1.599)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["green_yellow"]);
                        buttonCenter.Background = new SolidColorBrush(colours["green_yellow"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["green_yellow"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["green_yellow"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["green_yellow"]);
					}
					else if (result >= 1 && result <= 1.249)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["yellow_green"]);
                        buttonCenter.Background = new SolidColorBrush(colours["yellow_green"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["yellow_green"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["yellow_green"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["yellow_green"]);
					}
					else if (result >= 0.850 && result <= 0.999)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["yellow"]);
                        buttonCenter.Background = new SolidColorBrush(colours["yellow"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["yellow"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["yellow"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["yellow"]);
					}
					else if (result >= 0.618 && result <= 0.849)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["dark_red"]);
                        buttonCenter.Background = new SolidColorBrush(colours["dark_red"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["dark_red"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["dark_red"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["dark_red"]);
					}
					else if (result <= 0.617)
                    {
                        centerTraining.Foreground = new SolidColorBrush(colours["red"]);
                        buttonCenter.Background = new SolidColorBrush(colours["red"]);
                        qualValueCenter.Foreground = new SolidColorBrush(colours["red"]);
                        quanValueCenter.Foreground = new SolidColorBrush(colours["red"]);
						resultRecomendationHeaderCenter.Foreground = new SolidColorBrush(colours["red"]);
					}
					break;
				case 2:
                    if (result > 1.618)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["gold"]);
                        buttonEnd.Background = new SolidColorBrush(colours["gold"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["gold"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["gold"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["gold"]);
					}
					else if (result >= 1.600 && result <= 1.618)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["green"]);
                        buttonEnd.Background = new SolidColorBrush(colours["green"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["green"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["green"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["green"]);
					}
					else if (result >= 1.250 && result <= 1.599)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["green_yellow"]);
                        buttonEnd.Background = new SolidColorBrush(colours["green_yellow"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["green_yellow"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["green_yellow"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["green_yellow"]);
					}
					else if (result >= 1 && result <= 1.249)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["yellow_green"]);
                        buttonEnd.Background = new SolidColorBrush(colours["yellow_green"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["yellow_green"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["yellow_green"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["yellow_green"]);
					}
					else if (result >= 0.850 && result <= 0.999)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["yellow"]);
                        buttonEnd.Background = new SolidColorBrush(colours["yellow"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["yellow"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["yellow"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["yellow"]);
					}
					else if (result >= 0.618 && result <= 0.849)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["dark_red"]);
                        buttonEnd.Background = new SolidColorBrush(colours["dark_red"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["dark_red"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["dark_red"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["dark_red"]);
					}
					else if (result <= 0.617)
                    {
                        endTraining.Foreground = new SolidColorBrush(colours["red"]);
                        buttonEnd.Background = new SolidColorBrush(colours["red"]);
                        qualValueEnd.Foreground = new SolidColorBrush(colours["red"]);
                        quanValueEnd.Foreground = new SolidColorBrush(colours["red"]);
						resultRecomendationHeaderEnd.Foreground = new SolidColorBrush(colours["red"]);
					}
					break;
			}
		}
		public void ColorSet(double result)                                                            //ОКРАШИВАЕТ В ЦВЕТ
		{
			if (result > 1.618)
			{
				startTraining.Foreground = new SolidColorBrush(colours["gold"]);
				buttonStart.Background = new SolidColorBrush(colours["gold"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["gold"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["gold"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["gold"]);
			}
			else if (result >= 1.600 && result <= 1.618)
			{
				startTraining.Foreground = new SolidColorBrush(colours["green"]);
				buttonStart.Background = new SolidColorBrush(colours["green"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["green"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["green"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["green"]);
			}
			else if (result >= 1.250 && result <= 1.599)
			{
				startTraining.Foreground = new SolidColorBrush(colours["green_yellow"]);
				buttonStart.Background = new SolidColorBrush(colours["green_yellow"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
			}
			else if (result >= 1 && result <= 1.249)
			{
				startTraining.Foreground = new SolidColorBrush(colours["yellow_green"]);
				buttonStart.Background = new SolidColorBrush(colours["yellow_green"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
			}
			else if (result >= 0.850 && result <= 0.999)
			{
				startTraining.Foreground = new SolidColorBrush(colours["yellow"]);
				buttonStart.Background = new SolidColorBrush(colours["yellow"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["yellow"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["yellow"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["yellow"]);
			}
			else if (result >= 0.618 && result <= 0.849)
			{
				startTraining.Foreground = new SolidColorBrush(colours["dark_red"]);
				buttonStart.Background = new SolidColorBrush(colours["dark_red"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["dark_red"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["dark_red"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["dark_red"]);
			}
			else if (result <= 0.617)
			{
				startTraining.Foreground = new SolidColorBrush(colours["red"]);
				buttonStart.Background = new SolidColorBrush(colours["red"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["red"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["red"]);
				resultRecomendationHeaderStart.Foreground = new SolidColorBrush(colours["red"]);
			}
		}
        #endregion ColorSetFunctions
        #region TextAddFunctions
        /// <summary>
        /// Оценивает характеристику по результату
        /// </summary>
        /// <param name="result">результат</param>
        /// <param name="i">порядковый номер</param>
        public void TextAdd(double result, int i)
		{
			switch (i)
			{
				case 0:
                    if (result > 1.618)
					{
						qualValueStart.Text = ratios[ratioStates.over_gold];
						resultRecomendationHeaderStart.Text = state1[ratioStates.over_gold];
						valueRecomendationStart.Text = state2[ratioStates.over_gold];
					}
                    else if (result >= 1.600 && result <= 1.618)
					{
						qualValueStart.Text = ratios[ratioStates.gold];
						resultRecomendationHeaderStart.Text = state1[ratioStates.gold];
						valueRecomendationStart.Text = state2[ratioStates.gold];
					}
					else if (result >= 1.250 && result <= 1.599)
					{
						qualValueStart.Text = ratios[ratioStates.big_reserve];
						resultRecomendationHeaderStart.Text = state1[ratioStates.big_reserve];
						valueRecomendationStart.Text = state2[ratioStates.big_reserve];
					}
					else if (result >= 1 && result <= 1.249)
					{
						qualValueStart.Text = ratios[ratioStates.low_reserve];
						resultRecomendationHeaderStart.Text = state1[ratioStates.low_reserve];
						valueRecomendationStart.Text = state2[ratioStates.low_reserve];
					}
					else if (result >= 0.850 && result <= 0.999)
					{
						qualValueStart.Text = ratios[ratioStates.low_defecit];
						resultRecomendationHeaderStart.Text = state1[ratioStates.low_defecit];
						valueRecomendationStart.Text = state2[ratioStates.low_defecit];
					}
					else if (result >= 0.618 && result <= 0.849)
					{
						qualValueStart.Text = ratios[ratioStates.big_deficit];
						resultRecomendationHeaderStart.Text = state1[ratioStates.big_deficit];
						valueRecomendationStart.Text = state2[ratioStates.big_deficit];
					}
					else if (result <= 0.617)
					{
						qualValueStart.Text = ratios[ratioStates.below_low];
						resultRecomendationHeaderStart.Text = state1[ratioStates.below_low];
						valueRecomendationStart.Text = state2[ratioStates.below_low];
					}
					break;
				case 1:
                    if (result > 1.618)
					{
						qualValueCenter.Text = ratios[ratioStates.over_gold];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.over_gold];
						valueRecomendationCenter.Text = state2[ratioStates.over_gold];
					}
					else if (result >= 1.600 && result <= 1.618)
					{
						qualValueCenter.Text = ratios[ratioStates.gold];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.gold];
						valueRecomendationCenter.Text = state2[ratioStates.gold];
					}
					else if (result >= 1.250 && result <= 1.599)
					{
						qualValueCenter.Text = ratios[ratioStates.big_reserve];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.big_reserve];
						valueRecomendationCenter.Text = state2[ratioStates.big_reserve];
					}
					else if (result >= 1 && result <= 1.249)
					{
						qualValueCenter.Text = ratios[ratioStates.low_reserve];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.low_reserve];
						valueRecomendationCenter.Text = state2[ratioStates.low_reserve];
					}
					else if (result >= 0.850 && result <= 0.999)
					{
						qualValueCenter.Text = ratios[ratioStates.low_defecit];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.low_defecit];
						valueRecomendationCenter.Text = state2[ratioStates.low_defecit];
					}
					else if (result >= 0.618 && result <= 0.849)
					{
						qualValueCenter.Text = ratios[ratioStates.big_deficit];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.big_deficit];
						valueRecomendationCenter.Text = state2[ratioStates.big_deficit];
					}
					else if (result <= 0.617)
					{
						qualValueCenter.Text = ratios[ratioStates.below_low];
						resultRecomendationHeaderCenter.Text = state1[ratioStates.below_low];
						valueRecomendationCenter.Text = state2[ratioStates.below_low];
					}
					break;
				case 2:
                    if (result > 1.618)
					{
						qualValueEnd.Text = ratios[ratioStates.over_gold];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.over_gold];
						valueRecomendationEnd.Text = state2[ratioStates.over_gold];
					}
					else if (result >= 1.600 && result <= 1.618)
					{
						qualValueEnd.Text = ratios[ratioStates.gold];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.gold];
						valueRecomendationEnd.Text = state2[ratioStates.gold];
					}
					else if (result >= 1.250 && result <= 1.599)
					{
						qualValueEnd.Text = ratios[ratioStates.big_reserve];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.big_reserve];
						valueRecomendationEnd.Text = state2[ratioStates.big_reserve];
					}
                    else if (result >= 1 && result <= 1.249)
					{
						qualValueEnd.Text = ratios[ratioStates.low_reserve];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.low_reserve];
						valueRecomendationEnd.Text = state2[ratioStates.low_reserve];
					}
					else if (result >= 0.850 && result <= 0.999)
					{
						qualValueEnd.Text = ratios[ratioStates.low_defecit];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.low_defecit];
						valueRecomendationEnd.Text = state2[ratioStates.low_defecit];
					}
					else if (result >= 0.618 && result <= 0.849)
					{
						qualValueEnd.Text = ratios[ratioStates.big_deficit];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.big_deficit];
						valueRecomendationEnd.Text = state2[ratioStates.big_deficit];
					}
					else if (result <= 0.617)
					{
						qualValueEnd.Text = ratios[ratioStates.below_low];
						resultRecomendationHeaderEnd.Text = state1[ratioStates.below_low];
						valueRecomendationEnd.Text = state2[ratioStates.below_low];
					}
					break;
			}
		}
		/// <summary>
		/// Функция для добавления текста на экран в зависимости от результата
		/// </summary>
		/// <param name="result">коэф, который отвечает за результат исследования</param>
        public void TextAdd(double result)
        {
			if (result > 1.618)
			{
				qualValueStart.Text = ratios[ratioStates.over_gold];
				resultRecomendationHeaderStart.Text = state1[ratioStates.over_gold];
				valueRecomendationStart.Text = state2[ratioStates.over_gold];
			}
			else if (result >= 1.600 && result <= 1.618)
			{
				qualValueStart.Text = ratios[ratioStates.gold];
				resultRecomendationHeaderStart.Text = state1[ratioStates.gold];
				valueRecomendationStart.Text = state2[ratioStates.gold];
			}
			else if (result >= 1.250 && result <= 1.599)
			{
				qualValueStart.Text = ratios[ratioStates.big_reserve];
				resultRecomendationHeaderStart.Text = state1[ratioStates.big_reserve];
				valueRecomendationStart.Text = state2[ratioStates.big_reserve];
			}
			else if (result >= 1 && result <= 1.249)
			{
				qualValueStart.Text = ratios[ratioStates.low_reserve];
				resultRecomendationHeaderStart.Text = state1[ratioStates.low_reserve];
				valueRecomendationStart.Text = state2[ratioStates.low_reserve];
			}
			else if (result >= 0.850 && result <= 0.999)
			{
				qualValueStart.Text = ratios[ratioStates.low_defecit];
				resultRecomendationHeaderStart.Text = state1[ratioStates.low_defecit];
				valueRecomendationStart.Text = state2[ratioStates.low_defecit];
			}
			else if (result >= 0.618 && result <= 0.849)
			{
				qualValueStart.Text = ratios[ratioStates.big_deficit];
				resultRecomendationHeaderStart.Text = state1[ratioStates.big_deficit];
				valueRecomendationStart.Text = state2[ratioStates.big_deficit];
			}
			else if (result <= 0.617)
			{
				qualValueStart.Text = ratios[ratioStates.below_low];
				resultRecomendationHeaderStart.Text = state1[ratioStates.below_low];
				valueRecomendationStart.Text = state2[ratioStates.below_low];
			}
		}

		public string setTextSubject(double value)
		{
			if (value <= 0.617)
				return "\nCопровождается чрезмерным напряжением функционирования кардио-респираторной, нервной и вегетативной систем, приступами удушья и нехватки воздуха, подавлением психических реакций, головокружением и тяжестью в голове, тошнотой, рвотой, болями и страданиями вплоть до полного отказа от осуществляемой деятельности. Выполняемая психомоторная нагрузка организму противопоказана, поскольку сопровождается максимальным возрастанием энергетического дисбаланса со значительным преобладанием энергозатрат над энергопоставкой [Х. Моно, Ж. Шеррер, 1973]. В нервные центры поступает поток сверхсильных раздражителей, вызывающих формирование стрессовой реакции. Её развитие приводит к нарушению и ослаблению компенсаторных механизмов, как следствие к компенсированному психофизическому перенапряжению, которое способствует развитию многочисленных патологических и превалированию физиологических нарушений над психосоматической нормой [А.Г. Дембо, Э.В. Земцовский, 1989];\r\n- в состоянии относительного покоя отражает наличие патологических нарушений психосоматической нормы в острой форме. Требует пребывание человека в полупостельном или постельном режиме, в том числе в условиях стационарного лечения. Рекомендовано, наряду с обязательным использованием клинических форм лечения, регулярное выполнение идеомоторных упражнений релаксационного характера малой интенсивности в положении «лёжа» и «сидя» с комплексным использованием здравоукрепительных технологий: экономичного дыхания и идео-респираторного потенцирования, пребывание в состоянии самогипнотического сна [В.В. Гневушев, 1996-2008; С.Ю. Забельский, 2001-2023]; \r\n- в активной деятельности прогрессирующее снижение количественных показателей в диапазоне от 0,617 приводит к наличию состояний переутомления и перенапряжения содействующих развитию патологических нарушений. Рекомендовано экстренное снижение объемов психомоторной нагрузки выполняемой активной деятельности и кардинальная смена вида деятельности. Обязательное включение в лечебный и реабилитационный процессы занятий биоэкономичной психомоторной гимнастикой с комплексным использованием здравоукрепительных технологий при выполнении изометрических и релаксационных психомоторных упражнений малой интенсивности в положении «лежа» и «сидя», направленных на снижение и устранение превалирующих показателей энтропийной энергостоимости. Эффективность лечения подтверждается данными клинической диагностики и регулярно регистрируемой динамикой повышения количественной цифровой меры процесса здравоукрепления организма от 0,618. Последняя цифра свидетельствует о завершении острой фазы регрессирующего здравоукрепления организма. Однако занятия традиционными формами физической культуры и различными видами спорта противопоказаны [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов, 1993-2008; Л.Е. Деньгова, 2004; С.Ю. Забельский, 2001-2023].\n\n";
			else if (value >= 0.618 && value <= 0.853)
				return "\nCопровождается значительным напряжением в работе кардио-респираторной, нервной и вегетативной систем, нехваткой воздуха и отдышкой, выраженным сердцебиением, угнетением психических реакций, тяжестью в голове и головокружением, болезненными ощущениями в мышечно-связочном и костном аппаратах вплоть до возникновения объективной потребности в отказе от осуществления деятельности. Текущий объём психомоторной нагрузки организму противопоказан, поскольку с самого начала ее выполнения в организме возникает нарушение энергетического баланса с превалированием энергозатрат над энергопоставкой [Х. Моно, Ж. Шеррер, 1973]. От работающей мускулатуры в нервные центры поступают интенсивные раздражения, что в свою очередь, усиливает возбуждение, увеличивая нарушения и вегетативной регуляции [П.К. Анохин, 1968; С.Х. Азов, В.В. Гневушев, 1996]. Отрицательное воздействие перевозбуждения приводит к ослаблению компенсаторных механизмов, как следствие к компенсированному психофизическому перенапряжению, которое способствует развитию многочисленных патологических и превалированию физиологических нарушений над психосоматической нормой [А.Г. Дембо, Э.В. Земцовский, 1989]. В физиологии труда предельная интенсивность психомоторной деятельности должна ограничиваться этим диапазоном [Х. Моно, Ж. Шеррер, 1973];\r\n- в состоянии относительного покоя отражает возможное наличие патологических нарушений психосоматической нормы в острой форме с меньшей или большей устойчивостью. Может потребовать пребывание человека в полупостельном или постельном режиме, в том числе в условиях стационара. Рекомендовано, наряду с использованием медикаментозных и физиотерапевтических форм лечения и профилактики, регулярное выполнение идеомоторных упражнений изометрического и релаксационного характера малой интенсивности в положении «лёжа» и «сидя» с комплексным использованием здравоукрепительных технологий: экономичного дыхания и идео-респираторного потенцирования, а также пребывание в состоянии самогипнотического сна [В.В. Гневушев, 1996-2008; С.Ю. Забельский, 2001-2023];  \r\n- в активной деятельности прогрессирующее снижение количественных показателей в диапазоне от 0,853 до 0,618 провоцирует состояния переутомления и перенапряжения, граничащие и содействующие развитию патологических нарушений, тем самым формирует регресс процесса здравоукрепления организма. Рекомендовано экстренное снижение объемов психомоторной нагрузки выполняемой активной деятельности с обязательным включением в тренировочный или реабилитационный процессы занятий биоэкономичной психомоторной гимнастикой с комплексным использованием здравоукрепительных технологий при выполнении изометрических и релаксационных психомоторных упражнений малой интенсивности в положении «лежа» или «сидя», а также биоэкономичной ходьбы на первой и второй ступенях продуктивности, направленных на снижение и устранение преобладающих показателей энтропийной энергостоимости. Эффективность лечения подтверждается не только данными клинической диагностики, но и регулярно прогрессирующей динамикой повышения количественной цифровой меры процесса здравоукрепления организма от 0,618 до 0,853. Последняя цифра свидетельствует о завершении большого здравоукрепительного дефицита, однако выполнение физических упражнений активного динамического характера, а также занятия традиционными формами физической культуры и спортом – не рекомендуются [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов, 1993-2008; Л.Е. Деньгова, 2004; С.Ю. Забельский, 2001-2023].\n\n";
			else if (value >= 0.854 && value <= 0.999)
				return "\nCопровождается напряжением в работе кардио-респираторной и нервной систем, ощущением нехватки воздуха, снижением психических реакций, чувством скованности и тяжести в мышечно-связочном аппарате, в ряде случаев показана смена вида деятельности. Кратковременное нахождение организма в диапазоне малого дефицита не вызывает выраженных нарушений. Однако, выполнение психомоторной нагрузки в текущем объёме продолжительное время – не рекомендуется, поскольку требует использование резервных энергетических возможностей организма и сопровождается постепенно прогрессирующим нарушением энергетического баланса с превалированием энергозатрат над энергопоставкой, в результате чего возникает и развивается кислородный долг, приводящий к возникновению и быстрому росту утомления [С.Х. Азов, В.В. Гневушев, 1996; Х. Моно, Ж. Шеррер, 1973]. В физиологии труда средняя интенсивность психомоторной деятельности должна ограничиваться этим диапазоном [Х. Моно, Ж. Шеррер, 1973]:\r\n- в состоянии относительного покоя отражает наличие скрытой стадии патологических нарушений с меньшей или большей степенью проявления физиологических нарушений психосоматической нормы. Рекомендовано, наряду с возможным применением медикаментозных и физиотерапевтических форм лечебно-профилактического воздействия, регулярное выполнение идеомоторных упражнений динамического, изометрического и релаксационного характера малой и средней интенсивности в положении «лёжа», «сидя» и «стоя» с комплексным использованием здравоукрепительных технологий: экономичного дыхания и идео-респираторного потенцирования, а также пребывание в состоянии самогипнотического сна [В.В. Гневушев, 1996-2008; С.Ю. Забельский, 2001-2023];  \r\n- в активной деятельности прогрессирующее снижение количественных показателей в диапазоне от 0,999 до 0,854 характеризует наличие состояния утомления в границах физиологических нарушений, способствующих регрессу процесса здравоукрепления организма. Рекомендовано снижение объемов психомоторной нагрузки выполняемой активной деятельности с обязательным включением в тренировочный или реабилитационный процессы занятий биоэкономичной психомоторной гимнастикой с выполнением психомоторных упражнений малой и средней интенсивности в положениях «лёжа» и «сидя», биоэкономичной ходьбы, бега и плавания на первой и второй ступенях продуктивности с комплексным использованием здравоукрепительных технологий, направленных на снижение и устранение показателей энтропийной энергостоимости. Эффективность лечебно-профилактического воздействия подтверждается не только данными клинической диагностики, но и регулярно прогрессирующей динамикой количественной цифровой меры процесса здравоукрепления организма от 0,854 до 0,999. Последняя цифра свидетельствует о завершении малого здравоукрепительного дефицита, характеризует готовность и предрасположенность организма к переходу в негэнтропийный благоприятный режим функционирования [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов; 1993-2008, Л.Е. Деньгова; 2004, С.Ю. Забельский, 2001-2023].\n\n";
			else if (value >= 1.000 && value <= 1.236)
				return "\nCопровождается оптимальным состоянием функциональных систем организма, отсутствием признаков нехватки воздуха и потребности в увеличении объемов легочной вентиляции, устойчивостью психических реакций, отсутствием напряжения, дискомфортных и болезненных ощущений. Выполняемый объём психомоторной нагрузки соответствует индивидуальным адаптационным возможностям организма. Отмечается достижение энергетического баланса, энергопоставка компенсирует затраты энергии в процессе самого выполнения работы [Х. Моно, Ж. Шеррер, 1973]. В центральную нервную систему поступает поток слабых проприоцептивных раздражений, оказывающий стимулирующее воздействие на нервные центры головного мозга [С.Х. Азов, В.В. Гневушев, 1996]. В физиологии труда умеренная интенсивность психомоторной деятельности характеризуется этим диапазоном [Х. Моно, Ж. Шеррер, 1973];\r\n- в состоянии относительного покоя отражает преобладание психосоматической нормы над всё менее выраженным проявлением патологических и физиологических нарушений. Рекомендована минимизация применения медикаментозных форм оздоровительного и профилактического воздействия с регулярным выполнением идеомоторных упражнений динамического, изометрического и релаксационного характера средней интенсивности в положении «лёжа» и «сидя»» с комплексным использованием здравоукрепительных технологий: экономичного дыхания и идео-респираторного потенцирования, а также пребывание в состоянии самогипнотического сна [В.В. Гневушев, 1996-2008; С.Ю. Забельский, 2001-2023];  \r\n- в активной деятельности прогрессирующее снижение количественных показателей в диапазоне от 1,236 до 1,000 характеризует наличие состояния усталости, вызванного длительной активной физической или умственной деятельностью, факторами окружающей природной и социальной среды, несбалансированного и нерегулярного питания, недостаточным временем ночного сна, способствующих регрессу процесса здравоукрепления организма. Рекомендовано сохранение объемов психомоторной нагрузки выполняемой активной деятельности с обязательным включением в тренировочный или реабилитационный процессы занятий биоэкономичной психомоторной гимнастикой с выполнением психомоторных упражнений в различных исходных положениях (в полном объеме), биоэкономичной ходьбы, бега, плавания, подвижных и спортивных игр, вело и лыжный спорт и т.д.  с комплексным использованием здравоукрепительных технологий, направленных на сохранение и преобладание показателей негэнтропийной энергостоимости. Эффективность оздоровительного и профилактического воздействия подтверждается данными клинической диагностики и регулярно регистрируемой прогрессирующей динамикой количественной цифровой мерой процесса здравоукрепления организма от 1,000 до 1,236. Последняя цифра свидетельствует о завершении стадии реабилитации и переходе к гармоничному здравоукреплению организма [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов, 1993-2008; Л.Е. Деньгова, 2004; С.Ю. Забельский, 2001-2023].\n\n";
			else if (value >= 1.237 && value <= 1.599)
				return "\nCопровождается стабильным благоприятным состоянием функциональных систем организма, устойчивым режимом экономичного дыхания в покое и активной деятельности с постепенным прогрессирующим снижением секундных объемов легочной вентиляции, сопровождается чувством психологического и телесного комфорта. Выполняемый объём психомоторной нагрузки полностью соответствует оптимальным индивидуальным адаптационным возможностям организма. Отмечается сохранение энергетического баланса, энергопоставка полностью компенсирует затраты энергии в процессе самого выполнения работы [Х. Моно, Ж. Шеррер, 1973]. В центральную нервную систему поступает поток проприоцептивных раздражений, оказывающий стимулирующее воздействие на нервные центры головного мозга. Развивается общая неспецифическая реакция экономизации психофизической деятельности [Л.Х. Гаркави, Е.Б. Квакина, М.А. Уколова, 1977]. Постепенное снижение энергостоимости психомоторики, подтверждающееся уменьшением ЧСС и сочетается с увеличением её продуктивности. Таким образом, большие объемы выполняемой психомоторной нагрузки сопровождаются уменьшением энергетической стоимости. Перспективы развития такой динамики выражаются в способности организма выполнять до 60-70% предельного объёма работы без нарушения режима негэнтропийной энергостоимости [В.В. Гневушев, 1996-2008; С.Ю. Забельский, А.А. Гриднев, В.Е. Милов, А.В. Овчаренко, 2020-2022]. В физиологии труда и спортивной физиологии оптимальная интенсивность психомоторной деятельности характеризуется этим диапазоном [Х. Моно, Ж. Шеррер, 1973];\r\n- в состоянии относительного покоя в результате отсутствия проявлений патологических нарушений и снижения физиологических, психосоматическая норма приближается к своему биологическому оптимуму. Рекомендовано исключение применения медикаментозных форм оздоровительного и профилактического воздействия с регулярным выполнением идеомоторных упражнений динамического, изометрического и релаксационного характера средней и высокой интенсивности в различных исходных положениях с комплексным использованием здравоукрепительных технологий: экономичного дыхания и идео-респираторного потенцирования, а также пребывание в состоянии самогипнотического сна [В.В. Гневушев, 2008; С.Ю. Забельский, 2023];  \r\n- в активной деятельности по мере достижения и повышения количественных показателей в диапазоне от 1,237 до 1,599 происходит активное формирование естественной внутренней защиты организма – имунно-биологического барьера, оказывающего многостороннее противодействие негативным воздействиям природной и социальной среды. Большой резерв позволяет выполнять все виды двигательной и умственной активности в оптимальных масштабах. Одной из основных задач систематических занятий различными формами биоэкономичного здравоукрепительного тренинга является неукоснительное соблюдение режима негэнтропийных энергетических затрат. Формирующаяся при этом следовая реакция экономизации создаёт фундаментальную основу возможности выполнения организмом всей ежедневной психомоторной деятельности в виде бытового и производственного труда, занятий физической культурой и различными видами спорта, активного отдыха и т.д. без их нарушения [В.В. Гневушев, 1993-2008; С.Ю. Забельский, 2001-2023]. Развиваемый в этих условиях «акцептор действия» оказывает всестороннее благоприятное воздействие на процесс здравоукрепления организма по пути его совершенствования и достижения активного долголетия [И.М. Сеченов, 1952]. Эффективность оздоровительного и профилактического воздействия подтверждается данными клинической диагностики и регулярно регистрируемой устойчивой динамикой количественной цифровой меры процесса здравоукрепления организма граничащей с показателями золотой пропорции до 1,599. Последняя цифра свидетельствует о гармоничном здравоукреплении организма [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов 1993-2008; Л.Е. Деньгова, 2004; С.Ю. Забельский, 2001-2023].\n\n";
			else if (value >= 1.600 && value <= 1.618)
				return "\nCопровождается стабильным целостным благоприятным состоянием организма, чувством удовлетворения, эйфории, психологического и телесного комфорта. Характеризует оптимальный максимум гармоничности и экономичности жизнедеятельности организма. Выполняемый объём психомоторной нагрузки полностью соответствует оптимальным индивидуальным адаптационным возможностям организма;\r\n- в состоянии относительного покоя в результате отсутствия проявлений патологических и физиологических нарушений психосоматическая норма достигает своего биологического оптимума. Рекомендовано исключение медикаментозных форм оздоровительного и профилактического воздействия с регулярным выполнением идеомоторных упражнений динамического, изометрического и релаксационного характера средней и высокой интенсивности в различных исходных положениях с комплексным использованием здравоукрепительных технологий: экономичного дыхания и идео-респираторного потенцирования, а также пребывание в состоянии самогипнотического сна [В.В. Гневушев, 2008; С.Ю. Забельский, 2023];  \r\n- в активной деятельности по мере достижения и повышения количественных показателей в диапазоне от 1,600 до 1,618 происходит активное развитие иммунной защиты, оказывающей многостороннее противодействие негативным воздействиям природной и социальной среды и сдерживая естественные процессы старения клеток. В диапазоне золотой пропорции показаны все виды двигательной и умственной активности в наивысших масштабах. Одной из основных задач систематических занятий всеми формами биоэкономичного здравоукрепительного тренинга является неукоснительное соблюдение режима негэнтропийных энергетических затрат с реализацией возможности выполнения организмом ежедневной психомоторной деятельности без их нарушения. Формируемая в этих условиях следовая реакция экономизации выполняемой деятельности оказывает всестороннее благоприятное воздействие на процесс здравоукрепления организма по пути его совершенствования и достижения активного долголетия. Эффективность оздоровительного и профилактического воздействия подтверждается данными клинической диагностики и регистрируемой количественной цифровой мерой процесса здравоукрепления организма – показателями золотой пропорции до 1,618. Последняя цифра свидетельствует об оптимальном максимуме гармоничного здравоукрепления организма [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов, 1993-2008; Л.Е. Деньгова, 2004; С.Ю. Забельский, 2001-2023].\n\n";
			else
				return "\nCопровождается стабильным целостным благоприятным состоянием организма, чувством удовлетворения, психологического и телесного комфорта. Характеризует оптимальный максимум гармоничности и экономичности жизнедеятельности организма. Объём выполняемой психомоторной нагрузки характеризуется возможностью её оптимального увеличения в соответствии с индивидуальными адаптационными возможностями организма в диапазоне негэнтропийных энергетических затрат;\r\n- в состоянии относительного покоя отсутствует (показатели энергостоимости за верхним пределом шкалы золотой пропорции свидетельствует о возможном наличии брадикардии) [С.Х. Азов, В.В. Гневушев,1993-2008;  С.Ю. Забельский, 2001-2023]; \r\n- в активной деятельности обозначает переход на более высокую ступень продуктивности выполняемой деятельности. Эффективность оздоровительного и профилактического воздействия подтверждается данными клинической диагностики и количественной цифровой мерой процесса здравоукрепления организма с устойчивым сохранением и развитием негэнтропийных энергетических затрат в диапазонах от 1,000 до 1,618 характеризующими прогресс процесса гармоничного здравоукрепления организма [С.Х. Азов, В.В. Гневушев, Ю.П. Краснов 1993-2008; Л.Е. Деньгова, 2004; С.Ю. Забельский, 2001-2023].";
		}

		public void subjectStart(object sender, RoutedEventArgs e)
		{
			if (fullInformationStart.Text.Length == 0)
			{
				var quanValue = Convert.ToDouble(quanValueStart.Text);
				var currentText = setTextSubject(quanValue);
				fullInformationStart.Text = currentText;
			}
			else
			{
				fullInformationStart.Text = "";
			}
		}
		public void subjectCenter(object sender, RoutedEventArgs e)
		{
			if (fullInformationCenter.Text.Length == 0)
			{
				var quanValue = Convert.ToDouble(quanValueCenter.Text);
				var currentText = setTextSubject(quanValue);
				fullInformationCenter.Text = currentText;
			}
			else
			{
				fullInformationCenter.Text = "";
			}
		}
		public void subjectEnd(object sender, RoutedEventArgs e)
		{
			if (fullInformationEnd.Text.Length == 0)
			{
				var quanValue = Convert.ToDouble(quanValueEnd.Text);
				var currentText = setTextSubject(quanValue);
				fullInformationEnd.Text = currentText;
			}
			else
			{
				fullInformationEnd.Text = "";
			}
		}

		#endregion TextAddFunctions
		#region DragNDrop
		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				isDragging = true;
				lastPosition = e.GetPosition(this);
				Mouse.Capture((IInputElement)sender);
			}
		}

		private void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (isDragging)
			{
				Point currentPosition = e.GetPosition(this);

				double diffX = currentPosition.X - lastPosition.X;
				double diffY = currentPosition.Y - lastPosition.Y;

				double newLeft = this.Left + diffX;
				double newTop = this.Top + diffY;

				DoubleAnimation anim1 = new DoubleAnimation(newLeft, TimeSpan.FromMilliseconds(0));
				this.BeginAnimation(Window.LeftProperty, anim1);

				DoubleAnimation anim2 = new DoubleAnimation(newTop, TimeSpan.FromMilliseconds(0));
				this.BeginAnimation(Window.TopProperty, anim2);

				lastPosition = currentPosition;
			}
		}
		private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				isDragging = false;
				Mouse.Capture(null);
			}
		}
        #endregion DragNDrop
		#region WindowOptions
		public void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close();
			}
		}
		public void closeWindow(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		public void fullscreenWindow(object sender, RoutedEventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;
				return;
			}
			this.WindowState = WindowState.Maximized;
		}
		public void hideWindow(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}
		#endregion WindowOptions
		#region ExcelFunctions
		public static string GetExcelColumnName(int columnNumber)
		{
			int dividend = columnNumber;
			string columnName = String.Empty;
			int modulo;

			while (dividend > 0)
			{
				modulo = (dividend - 1) % 26;
				columnName = Convert.ToChar(65 + modulo) + columnName;
				dividend = (int)((dividend - modulo) / 26);
			}

			return columnName;
		}
		public void exportFile(object sender, RoutedEventArgs e)
		{
			ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial; // Установка контекста лицензии

			using (ExcelPackage package = new ExcelPackage())
			{
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PsyhosomaticTable");
				switch (disciplineType.SelectedIndex)
				{
					case 0:
						{
							worksheet.Cells["A1"].Value = "Энергостоимость";
							worksheet.Cells["A2"].Value = "Ф";
							worksheet.Cells["A5"].Value = "ЧСС";
							worksheet.Cells["C5"].Value = "У(%)";
							worksheet.Cells["D1"].Value = "Продуктивность";

							worksheet.Cells["B2"].Value = "Положение относительного покоя";

							worksheet.Cells["D3"].Value = "Лежа";
							worksheet.Cells["E3"].Value = "Стоя";
							worksheet.Cells["F3"].Value = "Сидя";

							worksheet.Cells["D4"].Value = "A(0)";
							worksheet.Cells["E4"].Value = "A1(+)";
							worksheet.Cells["F4"].Value = "An(+)";

							worksheet.Cells["A1:A23"].Style.Font.Bold = true;
							worksheet.Cells["D1:F3"].Style.Font.Bold = true;
							worksheet.Cells["A5"].Style.Font.Bold = true;

							worksheet.Cells["D1:F5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells["D1:F5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
							worksheet.Cells["A1:C19"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells["A1:C19"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

							worksheet.Cells["C5:C19"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells["C5:C19"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSlateGray);
							worksheet.Cells["D5:F5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells["D5:F5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSlateGray);

							worksheet.Cells["D6:F19"].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells["D6:F19"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);

							//Проставляем уровни для ЛЕЖА, СИДЯ, СТОЯ
							for (int i = 0, percent = 100; i < 3; i++, percent += 3)
							{
								worksheet.Cells[$"{(char)('D' + i)}5"].Value = percent;
							}

							for (int i = 8; i <= 21; i++)
							{
								worksheet.Cells[$"A{i - 2}"].Value = i; //Пульс
								worksheet.Cells[$"C{i - 2}"].Value = Math.Round(100.0 * i / 12, 0, MidpointRounding.AwayFromZero); //Проценты
								if (i == 12)
								{
									worksheet.Cells[$"B{i - 2}"].Value = $"B (0)"; //В-уровень
									continue;
								}
								worksheet.Cells[$"B{i - 2}"].Value = $"B ({(i < 12 ? '-' : '+')})"; //В-уровень
							}

							for (int i = 6; i <= 19; i++)
							{
								worksheet.Cells[$"D{i}"].Formula = $"=D5/C{i}";
								worksheet.Cells[$"E{i}"].Formula = $"=E5/C{i}";
								worksheet.Cells[$"F{i}"].Formula = $"=F5/C{i}";
							}

							var file = new FileInfo($"tableResults-{Guid.NewGuid()}.xlsx");
							package.SaveAs(file);
							break;
						}
					case 1:
					case 2:
					case 3:
					case 4:
						{
							worksheet.Cells["A1"].Value = "Энергостоимость";
							worksheet.Cells["B2"].Value = "Ф";
							worksheet.Cells["A3"].Value = "ЧСС";
							worksheet.Cells["C3"].Value = "У(%)";
							worksheet.Cells["C2"].Value = "Продуктивность";

							int productivityResult = Convert.ToInt32(disciplineList.Find(p => p.title == disciplineTypeContent.Text)!.maxValue);

							worksheet.Cells[1, 1, 39, productivityResult + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells["A1:B39"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
							worksheet.Cells[1, 1, 2, productivityResult + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
							worksheet.Cells["C4:C39"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateGray);
							worksheet.Cells[3, 3, 3, productivityResult + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateGray);
							worksheet.Cells[4, 4, 39, productivityResult + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);

							var changeLevelValue = productivityResult * 100 / 162;
							int endColumn = productivityResult + 3;
							int interMaxValue = productivityResult;

							//Ошибка! Нужно итерировать исходя из кол-ва раз, а не процентов(проценты должны высчитываться)
							while (productivityResult != 0)
							{
								worksheet.Cells[2, productivityResult + 3].Value = productivityResult;
								productivityResult--; // Переходим к следующему столбцу
							}

							while (Convert.ToInt32(worksheet.Cells[2, endColumn].Value) != changeLevelValue)
							{
								if (Convert.ToInt32(worksheet.Cells[2, endColumn].Value) == interMaxValue)
								{
									worksheet.Cells[3, endColumn].Value = 162;
									for (int i = 4; i <= 39; i++)
									{
										worksheet.Cells[i, endColumn].Formula = $"={GetExcelColumnName(endColumn)}3 / C{i}";
									}
									endColumn--;
									continue;
								}
								for (int i = 4; i <= 39; i++)
								{
									worksheet.Cells[i, endColumn].Formula = $"={GetExcelColumnName(endColumn)}3 / C{i}";
								}
								worksheet.Cells[3, endColumn].Value = 162 * Convert.ToInt32(worksheet.Cells[2, endColumn].Value) / interMaxValue;
								endColumn--;
							}

							for (int i = 10; i <= 45; i++)
							{
								worksheet.Cells[$"A{i - 6}"].Value = i;
							}

							for (int i = 4; i <= 39; i++)
							{
								worksheet.Cells[$"C{i}"].Formula = $"=100*A{i}/14";
								if (i == 8)
								{
									worksheet.Cells[$"B{i}"].Value = $"B (0)"; //В-уровень
									continue;
								}
								worksheet.Cells[$"B{i}"].Value = $"B ({(i < 8 ? '-' : '+')})"; //В-уровень
							}


							var file = new FileInfo($"tableResults-{Guid.NewGuid()}.xlsx");
							package.SaveAs(file);
							break;
						}
					default:
						{
							break;
						}
				}
			}
		}
		#endregion ExcelFunctions
		#region ClicksAndComputing
		public void buttonClick(object sender, RoutedEventArgs e)                       //вывод результата исчисления
		{
			excelExport.IsEnabled = true;
			FileFunct.ReadData(out disciplineList);
			double[] result = new double[3] { 0, 0, 0 };

			if (disciplineType.SelectedIndex == 0)
			{
				if ((Convert.ToInt32(pulseTextBoxes[0].Text) >= 8 && Convert.ToInt32(pulseTextBoxes[0].Text) <= 21 && minSwitcher.IsChecked == false) || (Convert.ToInt32(pulseTextBoxes[0].Text) >= 48 && Convert.ToInt32(pulseTextBoxes[0].Text) <= 12626 && minSwitcher.IsChecked == true))
				{
					switch ((string)comboBox.SelectedItem)
					{
						case "Сидя":
							if (minSwitcher.IsChecked == false)
							{
								result[0] = SetResultSec((int)HumanState.sitting);
								break;
							}
							else
							{
								result[0] = SetResultMin((int)HumanState.sitting);
								break;
							}
						case "Стоя":
							if (minSwitcher.IsChecked == false)
							{
								result[0] = SetResultSec((int)HumanState.staying);
								break;
							}
							else
							{
								result[0] = SetResultMin((int)HumanState.staying);
								break;
							}
						case "Лежа":
							if (minSwitcher.IsChecked == false)
							{
								result[0] = SetResultSec((int)HumanState.lying);
								break;
							}
							else
							{
								result[0] = SetResultMin((int)HumanState.lying);
								break;
							}
					}
					result[0] = Ceiling(result[0]);
					if (result[0] != 0)
					{
						ratioTable.Visibility = Visibility.Visible;
						quanValueStart.Text = Convert.ToString(result[0]);
						secondNode.Visibility = Visibility.Collapsed;
						thirdNode.Visibility = Visibility.Collapsed;
						ColorSet(result[0]);
						TextAdd(result[0]);
						return;
					}
				}
				else
				{
					MessageBox.Show("Ошибка с ЧСС!", "Неверно задана ЧСС");
				}
			}

			if (disciplineTypeContent.SelectedItem is not null)
			{
				for (int i = 0; i < countOfFieds; i++)
				{
					counter = 3;
					if (disciplineList.Any(temp => temp.title.ToString() == disciplineTypeContent.SelectedItem.ToString()))               //Проверка на совпадения элемента в выпадающем списке  с элементом в векторе
					{
						if (PulseValidate(pulseTextBoxes[i]))       //проверка границ ПУЛЬСА
						{
							foreach (DisciplinesTypes temp in disciplineList)
							{
								if (temp.title.ToString() == disciplineTypeContent.SelectedItem.ToString())
								{
									if (temp.dirProp == false)                                      //прямая прогрессия
									{
										minValue = temp.maxValue * 100 / 161.8;         //находим минимальное значение
										maxValue = temp.maxValue;
										while (result[i] == 0)
										{
											if (MinRateStraight(resultTextBoxes[i]))        //проверка результата на минимальную границу(прямая прогрессия)
											{
												if (minSwitcher.IsChecked == false)             //для секунд
												{
													double percentValueProduct = 161.8 * double.Parse(resultTextBoxes[i].Text) / maxValue;
													double percentValueEnergy = double.Parse(pulseTextBoxes[i].Text) * 100 / 14;
													result[i] = percentValueProduct / percentValueEnergy;
													result[i] = Ceiling(result[i]);
													break;
												}
												else                                                            //для минут
												{
													double percentValueProduct = 161.8 * double.Parse(resultTextBoxes[i].Text) / maxValue;
													double percentValueEnergy = double.Parse(pulseTextBoxes[i].Text) * 100 / 84;
													result[i] = percentValueProduct / percentValueEnergy;
													result[i] = Ceiling(result[i]);
													break;
												}
											}
											else if (NextRateStraight(resultTextBoxes[i]))                                                      //переход на следующий уровень
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
										resultTextBoxes[i].Text = resultTextBoxes[i].Text.Replace(".", ",");
										minValue = temp.maxValue * 161.8 / 100;
										maxValue = temp.maxValue;
										while (result[i] == 0)
										{
											if (MinRateReverse(resultTextBoxes[i]))
											{
												if (minSwitcher.IsChecked == false)
												{
													double percentValueProduct = 161.8 * maxValue / double.Parse(resultTextBoxes[i].Text);
													double percentValueEnergy = double.Parse(pulseTextBoxes[i].Text) * 100 / 14;
													result[i] = percentValueProduct / percentValueEnergy;
													result[i] = Ceiling(result[i]);
													break;
												}
												else
												{
													double percentValueProduct = 161.8 * maxValue / double.Parse(resultTextBoxes[i].Text);
													double percentValueEnergy = double.Parse(pulseTextBoxes[i].Text) * 100 / 84;
													result[i] = percentValueProduct / percentValueEnergy;
													result[i] = Ceiling(result[i]);
													break;
												}
											}
											else if (NextRateReverse(resultTextBoxes[i]))               //переход на следующий уровень
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
						if (result[i] != 0)
						{
							quanValueStart.Text = Convert.ToString(result[0]);
							quanValueCenter.Text = Convert.ToString(result[1]);
							quanValueEnd.Text = Convert.ToString(result[2]);
							ColorSet(result[i], i);
							TextAdd(result[i], i);
							ratioTable.Visibility = Visibility.Visible;
						}
					}
				}
			}
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
		}
		public void newFile_Click(object sender, RoutedEventArgs e)                                                 //СОЗДАНИЕ НОВОГО ФАЙЛА
		{
			ShowFunctions();
			FuncSetEnable();
		}
		public void HelpWindowClick(object sender, EventArgs e)                 //ОБРАБОТКА НАЖАТИЯ НА ОКНО ПОМОЩИ
		{
			HelpWIndow helpWIndow = new HelpWIndow();
			helpWIndow.Show();                                                                  //Реализовать отключение
		}
		public void SelectionFunction(object sender, SelectionChangedEventArgs e)                     //обработка события, когда мы выводим курсор из выпадающего списка
		{
			for(int i = 0; i < countOfFieds; i++)
			{
                resultGroupBoxes[i] = new GroupBox();
                pulseGroupBoxes[i] = new GroupBox();
                pulseTextBoxes[i] = new TextBox();
                resultTextBoxes[i] = new TextBox();
            }
			ClearAll();

			ratioTable.Visibility = Visibility.Collapsed;

			setDisciplineContent.Visibility = Visibility.Visible;       //показывает второй комбобокс
			minSwitcher.Visibility = Visibility.Visible;

            for (int i = 0; i < countOfFieds; i++)
            {
                resultGroupBoxes[i].Name = $"typeHead{i}";
                resultGroupBoxes[i].HorizontalAlignment = HorizontalAlignment.Left;
                resultGroupBoxes[i].Width = 250;
                resultGroupBoxes[i].FontSize = 13;
                resultGroupBoxes[i].ToolTip = "Введите результат";
                resultGroupBoxes[i].Margin = new Thickness(0, 0, 0, 10);
            }

            for (int i = 0; i < countOfFieds; i++)
			{
                pulseGroupBoxes[i].Name = $"typeHead{i}";
                pulseGroupBoxes[i].HorizontalAlignment = HorizontalAlignment.Left;
                pulseGroupBoxes[i].Width = 250;
                pulseGroupBoxes[i].FontSize = 13;
                pulseGroupBoxes[i].ToolTip = "Введите результат";
                pulseGroupBoxes[i].Margin = new Thickness(0, 0, 0, 10);
            }

			//TextBox pulseTextBox = new TextBox();
			for(int i = 0; i < countOfFieds; i++)
			{
				pulseTextBoxes[i].Name = $"pulseTextBox{i}";
				pulseTextBoxes[i].Width = 240;
				pulseTextBoxes[i].Height = 30;
            }

			switch (disciplineType.SelectedIndex)
			{
				case 0:
					{
						setDisciplineContent.Visibility = Visibility.Hidden;        //скрывает второй комбобокс
						minSwitcher.Visibility = Visibility.Visible;

                        comboBox.Name = "comboBox";
						comboBox.Items.Add("Сидя");
						comboBox.Items.Add("Стоя");
						comboBox.Items.Add("Лежа");
						comboBox.Width = 240;
						comboBox.Height = 30;

						resultGroupBoxes[0].Header = "Исходное положение";
						resultGroupBoxes[0].Content = comboBox;
						pulseGroupBoxes[0].Header = "ЧСС";
						pulseGroupBoxes[0].Content = pulseTextBoxes[0];

						stackPanel.Children.Add(resultGroupBoxes[0]);
						stackPanel.Children.Add(pulseGroupBoxes[0]);

						stackPanelScroll.Visibility = Visibility.Visible;
						break;
					}
				case 1:
					{
						addFunction(1);
						for(int i = 0; i < countOfFieds; i++)
						{
							resultGroupBoxes[i].Header = $"Продуктивность {i + 1}";

							// Создание TextBox в первом GroupBox
							resultTextBoxes[i].Name = $"resultTextBox{i}";
							resultTextBoxes[i].Width = 240;
							resultTextBoxes[i].Height = 30;
							resultGroupBoxes[i].Content = resultTextBoxes[i];

							pulseGroupBoxes[i].Header = $"ЧСС {i + 1}";
							pulseGroupBoxes[i].Content = pulseTextBoxes[i];

							// Добавление обоих GroupBox в StackPanel
							stackPanel.Children.Add(resultGroupBoxes[i]);
							stackPanel.Children.Add(pulseGroupBoxes[i]);
                        }
                        stackPanelScroll.Visibility = Visibility.Visible;
                        break;
					}
				case 2:
					{
						addFunction(2);
                        for (int i = 0; i < countOfFieds; i++)
                        {
                            resultGroupBoxes[i].Header = $"Продуктивность {i+1}";

                            // Создание TextBox в первом GroupBox
                            resultTextBoxes[i].Name = $"resultTextBox{i}";
                            resultTextBoxes[i].Width = 240;
                            resultTextBoxes[i].Height = 30;
                            resultGroupBoxes[i].Content = resultTextBoxes[i];

							pulseGroupBoxes[i].Header = $"ЧСС {i+1}";
							pulseGroupBoxes[i].Content = pulseTextBoxes[i];

                            // Добавление обоих GroupBox в StackPanel
                            stackPanel.Children.Add(resultGroupBoxes[i]);
                            stackPanel.Children.Add(pulseGroupBoxes[i]);
                        }
                        stackPanelScroll.Visibility = Visibility.Visible;
                        break;
					}
				case 3:
					{
						addFunction(3);
                        for (int i = 0; i < countOfFieds; i++)
                        {
                            resultGroupBoxes[i].Header = $"Продуктивность {i+1}";

                            // Создание TextBox в первом GroupBox
                            resultTextBoxes[i].Name = $"resultTextBox{i}";
                            resultTextBoxes[i].Width = 240;
                            resultTextBoxes[i].Height = 30;
                            resultGroupBoxes[i].Content = resultTextBoxes[i];

							pulseGroupBoxes[i].Header = $"ЧСС {i + 1}";
							pulseGroupBoxes[i].Content = pulseTextBoxes[i];

                            // Добавление обоих GroupBox в StackPanel
                            stackPanel.Children.Add(resultGroupBoxes[i]);
                            stackPanel.Children.Add(pulseGroupBoxes[i]);
                        }
                        stackPanelScroll.Visibility = Visibility.Visible;
                        break;
					}
				case 4:
					{
						addFunction(4);
                        for (int i = 0; i < countOfFieds; i++)
                        {
                            resultGroupBoxes[i].Header = $"Продуктивность {i+1}";

                            // Создание TextBox в первом GroupBox
                            resultTextBoxes[i].Name = $"resultTextBox{i}";
                            resultTextBoxes[i].Width = 240;
                            resultTextBoxes[i].Height = 30;
                            resultGroupBoxes[i].Content = resultTextBoxes[i];

							pulseGroupBoxes[i].Header = $"ЧСС {i+1}";
							pulseGroupBoxes[i].Content = pulseTextBoxes[i];

                            // Добавление обоих GroupBox в StackPanel
                            stackPanel.Children.Add(resultGroupBoxes[i]);
                            stackPanel.Children.Add(pulseGroupBoxes[i]);
                        }
                        stackPanelScroll.Visibility = Visibility.Visible;
                        break;
					}
			}
		}
		#endregion ClicksAndComputing
		#region NumberFunctionsAndResults
		public double Ceiling(double result)
		{
			return Math.Round(result, 3, MidpointRounding.AwayFromZero);
		}
		/// <summary>
		/// Возвращает результат вычисления результата в секундах
		/// </summary>
		/// <param name="coef">Коэф</param>
		/// <returns>результат вычисления результата в секундах</returns>
		public double SetResultSec(int coef = 1)                                                    //ФУНКЦИЯ ВЫЧИСЛЕНИЯ В СЕКУНДАХ
		{
			if (double.TryParse(pulseTextBoxes[0].Text, out double temp))
				return coef / (100 * temp / 12);
			else
				return 0;
		}
        /// <summary>
        /// Метод для вычисления результата в секундах, работающий для списков
        /// </summary>
        /// <param name="numberOfField">Порядковый номер записи</param>
        /// <param name="coef">Процентное соотношение соотвествующему результату</param>
        /// <returns>Результат в секундах</returns>
        public double SetResultSec(int numberOfField, int coef = 1)                                                    //ФУНКЦИЯ ВЫЧИСЛЕНИЯ В СЕКУНДАХ
        {
            if (double.TryParse(pulseTextBoxes[numberOfField].Text, out double temp))
                return coef / (100 * temp / 12);
            else
                return 0;
        }
        /// <summary>
        /// Метод для расчета минут единичных записей(сидя, стоя, лежа)
        /// </summary>
        /// <param name="coef">Процентное соотношение соотвествующему результату</param>
        /// <returns>Возвращает результат в секундах</returns>
        public double SetResultMin(int coef = 1)                                            //ФУНКЦИЯ ВЫЧИСЛЕНИИ В СЕКУНДАХ
		{
			if (double.TryParse(pulseTextBoxes[0].Text, out double temp))
				return coef / (100 * temp / 72);
			else
				return 0;
		}
        public double SetResultMin(int numberOfField, int coef = 1)                                            //ФУНКЦИЯ ВЫЧИСЛЕНИИ В СЕКУНДАХ
        {
            if (double.TryParse(pulseTextBoxes[numberOfField].Text, out double temp))
                return coef / (100 * temp / 72);
            else
                return 0;
        }
		#endregion NumberFunctionsAndResults
	}
}
