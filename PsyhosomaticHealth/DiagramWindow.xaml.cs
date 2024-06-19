using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot;
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
	/// Interaction logic for DiagramWindow.xaml
	/// </summary>
	public partial class DiagramWindow : Window
	{
		public DiagramWindow()
		{
			InitializeComponent();
			var plotModel = new PlotModel { Title = "График зависимости", Background = OxyColor.FromRgb(240, 240, 240) };

			// Меняем названием осей X и Y
			plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Время" });
			plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Пульс" });

			var random = new Random();

			// Создаем график с точками, распределенными хаотичным образом с дисперсией 0.2
			var series = new LineSeries();
			for (double x = 0; x < 10; x += 0.1)
			{
				double y = 80 + random.NextDouble() * 30; // Значение y в диапазоне от 80 до 110
				series.Points.Add(new DataPoint(x, y));
			}
			plotModel.Series.Add(series);

			diagram.Model = plotModel;
		}
	}
}
