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
			PlotModel plotModel = new PlotModel { Title = "График зависимости", Background = OxyColor.FromRgb(240,240,240) };

			// Создаем график функции y = sin(x)
			var series = new LineSeries();
			for (double x = 0; x < 10; x += 0.1)
			{
				series.Points.Add(new DataPoint(x, Math.Sin(x)));
			}
			plotModel.Series.Add(series);

			diagram.Model = plotModel;
		}
	}
}
