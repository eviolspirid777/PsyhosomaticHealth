using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using FileWorking;
using Disciplines;
using System.Threading;
using System.Linq.Expressions;

namespace PsyhosomaticHealth
{
    /// <summary>
    /// Interaction logic for addDiscipline.xaml
    /// </summary>
    public partial class addDiscipline : Window
    {
        public addDiscipline()
        {
            InitializeComponent();
        }
        public void enter_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            DisciplinesTypes temp = new DisciplinesTypes();
            if (double.TryParse(setMaxValueTextBox.Text, out double result))
                temp.maxValue = result;
            else
            {
                MessageBox.Show("Вы ввели не число!", "Ошибка");
                flag = false;
            }
            temp.title = setTitleTextBox.Text;
            setNum(temp);
			if (reverseProgres.IsChecked == true)
                temp.dirProp = true;
            if(straightProgres.IsChecked == true)
                temp.dirProp = false;
            if (flag)
            {
               List<DisciplinesTypes> disciplinesTypes = new List<DisciplinesTypes>();
               FileFunct.ReadData(out disciplinesTypes);
               disciplinesTypes.Add(temp);
               FileFunct.WriteData(disciplinesTypes);
               PsyhHealth psyhHealth = new PsyhHealth();
               Close();
               psyhHealth.Show();
            }
        }
        public void cancel_Click(object sender, RoutedEventArgs e)
        {
            PsyhHealth psyhHealth = new PsyhHealth();
            psyhHealth.Show();
            this.Close();
        }
        public void setNum(DisciplinesTypes temp)
        {
			if (setNumber.SelectedIndex == 0)
				temp.number = 1;
			if (setNumber.SelectedIndex == 1)
				temp.number = 2;
			if (setNumber.SelectedIndex == 2)
				temp.number = 3;
			if (setNumber.SelectedIndex == 3)
				temp.number = 4;
		}

    }
}
