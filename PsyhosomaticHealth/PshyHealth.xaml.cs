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

namespace PsyhosomaticHealth
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class PsyhHealth : Window
	{
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
			//AddDisciplines();
		}
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
        public void ShowFunctions()
		{
			setDiscipline.Visibility = Visibility.Visible;          //показывает поле для выбора дисциплины
			getResult.Visibility = Visibility.Visible;              //показывает клавишу получения результата
		}
		public void ColorSet(double result)                                                            //ОКРАШИВАЕТ В ЦВЕТ
		{
			if (result > 1.618)
			{
				startTraining.Foreground = new SolidColorBrush(colours["gold"]);
				buttonStart.Background = new SolidColorBrush(colours["gold"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["gold"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["gold"]);
			}
			else if (result >= 1.600 && result <= 1.618)
			{
				startTraining.Foreground = new SolidColorBrush(colours["green"]);
				buttonStart.Background = new SolidColorBrush(colours["green"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["green"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["green"]);
			}
			else if (result >= 1.250 && result <= 1.599)
			{
				startTraining.Foreground = new SolidColorBrush(colours["green_yellow"]);
				buttonStart.Background = new SolidColorBrush(colours["green_yellow"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["green_yellow"]);
			}
			else if (result >= 1 && result <= 1.249)
			{
				startTraining.Foreground = new SolidColorBrush(colours["yellow_green"]);
				buttonStart.Background = new SolidColorBrush(colours["yellow_green"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["yellow_green"]);
			}
			else if (result >= 0.850 && result <= 0.999)
			{
				startTraining.Foreground = new SolidColorBrush(colours["yellow"]);
				buttonStart.Background = new SolidColorBrush(colours["yellow"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["yellow"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["yellow"]);
			}
			else if (result >= 0.618 && result <= 0.849)
			{
				startTraining.Foreground = new SolidColorBrush(colours["dark_red"]);
				buttonStart.Background = new SolidColorBrush(colours["dark_red"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["dark_red"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["dark_red"]);
			}
			else if (result <= 0.617)
			{
				startTraining.Foreground = new SolidColorBrush(colours["red"]);
				buttonStart.Background = new SolidColorBrush(colours["red"]);
				qualValueStart.Foreground = new SolidColorBrush(colours["red"]);
				quanValueStart.Foreground = new SolidColorBrush(colours["red"]);
			}
		}
		public void TextAdd(double result)                                                  //ОЦЕНИВАЕТ ХАРАКТЕРИСТИКУ ПО РЕЗУЛЬТАТУ
		{
			textBlock.Text += "\nКачественная характеристика: ";        //Начало любой КАЧЕСТВЕННОЙ характеристики

			if (result > 1.618)
			{
				textBlock.Text += data[ratioStates.over_gold];
				qualValueStart.Text = ratios[ratioStates.over_gold];
			}
			if (result >= 1.600 && result <= 1.618)
			{
				textBlock.Text += data[ratioStates.gold];
				qualValueStart.Text = ratios[ratioStates.gold];
			}
			if (result >= 1.250 && result <= 1.599)
			{
				textBlock.Text += data[ratioStates.big_reserve];
				qualValueStart.Text = ratios[ratioStates.big_reserve];
			}
			if (result >= 1 && result <= 1.249)
			{
				textBlock.Text += data[ratioStates.low_reserve];
				qualValueStart.Text = ratios[ratioStates.low_reserve];
			}
			if (result >= 0.850 && result <= 0.999)
			{
				textBlock.Text += data[ratioStates.low_defecit];
				qualValueStart.Text = ratios[ratioStates.low_defecit];
			}
			if (result >= 0.618 && result <= 0.849)
			{
				textBlock.Text += data[ratioStates.big_deficit];
				qualValueStart.Text = ratios[ratioStates.big_deficit];
			}
			if (result <= 0.617)
			{
				textBlock.Text += data[ratioStates.below_low];
				qualValueStart.Text = ratios[ratioStates.below_low];
			}
		}
		public void FuncSetEnable()                                     //Включить функции
		{
			printFile.IsEnabled = true;
			/*printFile.Cursor = Cursors.;  Вопрос с курсорами нужно еще решить, когда файл неактивен*/
			saveFile.IsEnabled = true;
			saveFileAs.IsEnabled = true;
		}
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

							/*                        worksheet.Cells[1,1, 39, productivityResult + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
													worksheet.Cells[1, 1, 39, productivityResult + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSlateGray);

													worksheet.Cells["C4:C39"].Style.Fill.PatternType = ExcelFillStyle.Solid;
													worksheet.Cells["C4:C39"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

													worksheet.Cells[3,productivityResult].Style.Fill.PatternType = ExcelFillStyle.Solid;
													worksheet.Cells[3, productivityResult].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

													worksheet.Cells[4,4,39,productivityResult].Style.Fill.PatternType = ExcelFillStyle.Solid;
													worksheet.Cells[4, 4, 39, productivityResult].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);*/

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
            excelExport.IsEnabled = true;
			FileFunct.ReadData(out disciplineList);
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

			else if (disciplineList.Any(temp => temp.title.ToString() == disciplineTypeContent.SelectedItem.ToString()))               //Проверка на совпадения элемента в выпадающем списке  с элементом в векторе
			{
				if ((Convert.ToInt32(pulseBox.Text) >= 10 && Convert.ToInt32(pulseBox.Text) <= 36 && minSwitcher.IsChecked == false) || (Convert.ToInt32(pulseBox.Text) >= 60 && Convert.ToInt32(pulseBox.Text) <= 216 && minSwitcher.IsChecked == true))       //проверка границ ПУЛЬСА
				{
					foreach (DisciplinesTypes temp in disciplineList)
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
									else if (Convert.ToDouble(resultBox.Text) >= minValue && counter > 0)               //переход на следующий уровень
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
					/*                    colorBlock.Visibility = Visibility.Visible;
										textBlock.Text = $"Колличественный показатель: {Convert.ToString(result)}. ";*/
					quanValueStart.Text = Convert.ToString(result);
					ColorSet(result);
					TextAdd(result);
					ratioTable.Visibility = Visibility.Visible;
					/*                    sportBlock.Text = $"Ваш уровень продуктивности: C{counter}";
										counter = 3;                                //Возвращаем счетчик*/
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

			setDisciplineContent.Visibility = Visibility.Visible;       //показывает второй комбобокс
			minSwitcher.Visibility = Visibility.Visible;
			resultHead = new GroupBox();                //GroupBox для выбора результатов
			resultHead.Name = "typeHead";
			resultHead.HorizontalAlignment = HorizontalAlignment.Left;
			resultHead.Width = 250;
			resultHead.FontSize = 13;
			resultHead.ToolTip = "Введите результат";
			resultHead.Margin = new Thickness(0, 0, 0, 10);

			pulseHead = new GroupBox();     //GroupBox для измерения пульса
			pulseHead.Name = "pulseHead";
			pulseHead.Header = "ЧСС";
			pulseHead.HorizontalAlignment = HorizontalAlignment.Left;
			pulseHead.Width = 250;
			pulseHead.FontSize = 13;
			pulseHead.ToolTip = "Измерьте пульc за 10 сек";

			ComboBox comboBox = new ComboBox();

			//TextBox pulseBox = new TextBox();
			pulseBox.Name = "pulseBox";
			pulseBox.Width = 240;
			pulseBox.Height = 30;

			switch (disciplineType.SelectedIndex)
			{
				case 0:
					{
						setDisciplineContent.Visibility = Visibility.Hidden;        //скрывает второй комбобокс
						minSwitcher.Visibility = Visibility.Visible;
						resultHead.Header = "Исходное положение";

						//ComboBox comboBox = new ComboBox();
						comboBox.Name = "comboBox";
						comboBox.Items.Add("Сидя");
						comboBox.Items.Add("Стоя");
						comboBox.Items.Add("Лежа");
						comboBox.Width = 240;
						comboBox.Height = 30;
						resultHead.Content = comboBox;

						pulseHead.Content = pulseBox;

						stackPanel.Children.Add(resultHead);
						stackPanel.Children.Add(pulseHead);
						break;
					}
				case 1:
					{
						addFunction(1);
						resultHead.Header = "Продуктивность";

						// Создание TextBox в первом GroupBox
						resultBox.Name = "resultBox";
						resultBox.Width = 240;
						resultBox.Height = 30;
						resultHead.Content = resultBox;

						pulseHead.Content = pulseBox;

						// Добавление обоих GroupBox в StackPanel
						stackPanel.Children.Add(resultHead);
						stackPanel.Children.Add(pulseHead);
						break;
					}
				case 2:
					{
						addFunction(2);
						resultHead.Header = "Продуктивность";

						resultBox.Name = "resultBox";
						resultBox.Width = 240;
						resultBox.Height = 30;
						resultHead.Content = resultBox;

						pulseHead.Content = pulseBox;

						// Добавление обоих GroupBox в StackPanel
						stackPanel.Children.Add(resultHead);
						stackPanel.Children.Add(pulseHead);
						break;
					}
				case 3:
					{
						addFunction(3);
						resultHead.Header = "Продуктивность";

						resultBox.Name = "resultBox";
						resultBox.Width = 240;
						resultBox.Height = 30;
						resultHead.Content = resultBox;

						pulseHead.Content = pulseBox;

						// Добавление обоих GroupBox в StackPanel
						stackPanel.Children.Add(resultHead);
						stackPanel.Children.Add(pulseHead);
						break;
					}
				case 4:
					{
						addFunction(4);
						resultHead.Header = "Продуктивность";

						resultBox.Name = "resultBox";
						resultBox.Width = 240;
						resultBox.Height = 30;

						pulseHead.Content = pulseBox;

						// Добавление обоих GroupBox в StackPanel
						stackPanel.Children.Add(resultHead);
						stackPanel.Children.Add(pulseHead);
						break;
					}
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
			pulseBox.Text = string.Empty;
			resultBox.Text = string.Empty;
			textBlock.Text = string.Empty;
			comboBox.Items.Clear();
			stackPanel.Children.Clear();
		}
		enum HumanState                                 //ПЕРЕЧИСЛЕНИЕ СОСТОЯНИЙ
		{
			lying = 100,
			sitting = 103,
			staying = 106
		}
		public double Ceiling(double result)
		{
			return Math.Round(result, 3, MidpointRounding.AwayFromZero);
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
