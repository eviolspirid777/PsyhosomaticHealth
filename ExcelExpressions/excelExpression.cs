namespace ExcelExpressions
{
	public class excelExpression
	{
		static public string GetNextColumn(string column)
		{
			// Преобразование строки столбца в число (A = 0, B = 1, ..., Z = 25)
			int columnIndex = column[0] - 'A';

			// Если достигнут конец алфавита, то увеличиваем первую букву
			if (columnIndex == 25)
				return ((char)('A')).ToString() + 'A';

			// Увеличиваем столбец на один и возвращаем его
			return ((char)('A' + columnIndex + 1)).ToString();
		}
	}
}