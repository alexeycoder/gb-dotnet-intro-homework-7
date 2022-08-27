// Задача 52. Задайте двумерный массив из целых чисел.
// Найдите среднее арифметическое элементов в каждом столбце.
// Например, задан массив:
// 1 4 7 2
// 5 9 2 3
// 8 4 2 4
// Среднее арифметическое каждого столбца: 4,6; 5,6; 3,6; 3.

const int MaxFractionDigits = 2;

do
{
	Console.Clear();
	PrintTitle("Рассчёт среднего арифметического элементов для каждого столбца матрицы", ConsoleColor.Cyan);

	int m = GetUserInputInt("\nВведите число строк:\t", 1);
	int n = GetUserInputInt("Введите число столбцов:\t", 1);
	int minimum = GetUserInputInt("Введите нижний предел диапазона случайных целых чисел:  ");
	int maximum = GetUserInputInt("Введите верхний предел диапазона случайных целых чисел: ", minimum);
	Console.WriteLine();

	int[,] mtx = CreateMatrixRandomInt(m, n, minimum, maximum);
	double[] avgs = GetAveragesByColumn(mtx);
	int cellSize = GetSuitableCellSize(minimum, maximum, MaxFractionDigits);

	PrintColored($"Матрица {m} \u2715 {n}:\n", ConsoleColor.DarkGray);
	PrintMatrix(mtx, MaxFractionDigits, cellSize);


	PrintColored($"Средние значения по столбцам:\n", ConsoleColor.DarkGray);
	PrintArrayAsRow(avgs, MaxFractionDigits, cellSize);

} while (AskForRepeat());

// Methods:

static double[] GetAveragesByColumn(int[,] matrix)
{
	int rows = matrix.GetLength(0);
	int cols = matrix.GetLength(1);

	double[] averages = new double[cols];

	for (int row = 0; row < rows; ++row)
	{
		for (int col = 0; col < cols; ++col)
		{
			averages[col] += matrix[row, col];
		}
	}
	for (int col = 0; col < cols; ++col)
	{
		averages[col] /= rows;
	}
	return averages;
}

static int[,] CreateMatrixRandomInt(int rows, int cols, int min, int max)
{
	int[,] matrix = new int[rows, cols];

	Random rnd = new Random();
	max = max + 1;
	for (int row = 0; row < rows; ++row)
	{
		for (int col = 0; col < cols; ++col)
		{
			matrix[row, col] = rnd.Next(min, max);
		}
	}
	return matrix;
}

#region Print Matrix Generic

static void PrintArrayAsRow<T>(T[] array, int maxFractionDigits = 2, int desirableCellSize = -1) where T : struct, IFormattable
{
	T[,] row = ToRow(array);
	PrintMatrix(row, maxFractionDigits, desirableCellSize);
}

static T[,] ToRow<T>(T[] array) where T : struct
{
	T[,] row = new T[1, array.Length];
	for (int i = 0; i < array.Length; ++i)
	{
		row[0, i] = array[i];
	}
	return row;
}

static void PrintMatrix<T>(T[,] matrix, int maxFractionDigits = 2, int desirableCellSize = -1) where T : struct, IFormattable
{
	const string itemsDelimiter = "  ";
	const string paddingLeft = " ";
	string paddingRight = desirableCellSize <= 0 ? paddingLeft : new string(' ', desirableCellSize / 2 + 1);
	string format = GetNumbersToStringFormat(maxFractionDigits);

	string[,] stringMatrix = ToStringTable(matrix, format, desirableCellSize);

	int rowsLastIndex = stringMatrix.GetLength(0) - 1;
	int colsLastIndex = stringMatrix.GetLength(1) - 1;

	int posRight = 0;
	Console.WriteLine("\u250f"); // ┏

	for (int row = 0; row <= rowsLastIndex; ++row)
	{
		Console.Write("\u2503" + paddingLeft); // ┃->
		for (int col = 0; col < colsLastIndex; ++col)
		{
			Console.Write(stringMatrix[row, col] + itemsDelimiter);
		}
		Console.Write(stringMatrix[row, colsLastIndex]);
		Console.Write(paddingRight + "\u2503"); // ->┃
		if (posRight == 0) posRight = Console.CursorLeft - 1; // just once enough (too slow in linux gui terminal, fast in tty)
		Console.WriteLine();
	}

	//--posRight;
	int bkpTopPos = Console.CursorTop;
	Console.Write("\u2517"); // ┗
	Console.CursorLeft = posRight;
	Console.Write("\u251b"); // ┛
	Console.CursorLeft = posRight;
	int rowsToTop = rowsLastIndex + 2;
	if (Console.CursorTop >= rowsToTop)
	{
		Console.CursorTop -= rowsToTop;
		Console.Write("\u2513"); // ┓
		Console.CursorTop = bkpTopPos;
	}
	Console.WriteLine();
}

static string GetNumbersToStringFormat(int maxFractionDigits)
{
	return maxFractionDigits == int.MaxValue
			?
			"G" : maxFractionDigits > 0 ? "0." + new string('#', maxFractionDigits) : "0";
}

static string[,] ToStringTable<T>(T[,] matrix, string format, int desirableCellSize = -1) where T : struct, IFormattable
{
	int rows = matrix.GetLength(0);
	int cols = matrix.GetLength(1);
	string[,] strTable = new string[rows, cols];
	int maxLength = 0;
	for (int col = 0; col < cols; ++col)
	{
		for (int row = 0; row < rows; ++row)
		{
			string strValue = matrix[row, col].ToString(format, null);
			strTable[row, col] = strValue;
			maxLength = Math.Max(maxLength, strValue.Length);
		}
	}
	// apply cells padding & alignment:
	maxLength = Math.Max(desirableCellSize, maxLength);
	for (int col = 0; col < cols; ++col)
	{
		for (int row = 0; row < rows; ++row)
		{
			strTable[row, col] = strTable[row, col].PadLeft(maxLength);
		}
	}

	return strTable;
}

#endregion Print Matrix Generic

#region Table Text Formatting

static int GetSuitableCellSize(int minValue, int maxValue, int maxFractionDigits)
{
	return 1 + maxFractionDigits + Math.Max(minValue.ToString().Length, maxValue.ToString().Length);
}

#endregion Table Text Formatting

#region User Interaction Common

static int GetUserInputInt(string inputMessage, int minAllowed = int.MinValue, int maxAllowed = int.MaxValue)
{
	const string errorMessageWrongType = "Некорректный ввод! Требуется целое число. Пожалуйста повторите\n";
	string errorMessageOutOfRange = string.Empty;
	if (minAllowed != int.MinValue && maxAllowed != int.MaxValue)
		errorMessageOutOfRange = $"Число должно быть в интервале от {minAllowed} до {maxAllowed}! Пожалуйста повторите\n";
	else if (minAllowed != int.MinValue)
		errorMessageOutOfRange = $"Число не должно быть меньше {minAllowed}! Пожалуйста повторите\n";
	else
		errorMessageOutOfRange = $"Число не должно быть больше {maxAllowed}! Пожалуйста повторите\n";

	int input;
	bool notANumber = false;
	bool outOfRange = false;
	do
	{
		if (notANumber)
		{
			PrintError(errorMessageWrongType, ConsoleColor.Magenta);
		}
		if (outOfRange)
		{
			PrintError(errorMessageOutOfRange, ConsoleColor.Magenta);
		}
		Console.Write(inputMessage);
		notANumber = !int.TryParse(Console.ReadLine(), out input);
		outOfRange = !notANumber && (input < minAllowed || input > maxAllowed);

	} while (notANumber || outOfRange);

	return input;
}

static void PrintTitle(string title, ConsoleColor foreColor)
{
	int feasibleWidth = Math.Min(title.Length, Console.BufferWidth);
	string titleDelimiter = new string('\u2550', feasibleWidth);
	PrintColored(titleDelimiter + Environment.NewLine + title + Environment.NewLine + titleDelimiter + Environment.NewLine, foreColor);
}

static void PrintError(string errorMessage, ConsoleColor foreColor)
{
	PrintColored("\u2757 Ошибка: " + errorMessage, foreColor);
}

static void PrintColored(string message, ConsoleColor foreColor)
{
	var bkpColor = Console.ForegroundColor;
	Console.ForegroundColor = foreColor;
	Console.Write(message);
	Console.ForegroundColor = bkpColor;
}

static bool AskForRepeat()
{
	Console.WriteLine();
	Console.WriteLine("Нажмите Enter, чтобы начать сначала или Esc, чтобы завершить...");
	ConsoleKeyInfo key = Console.ReadKey(true);
	return key.Key != ConsoleKey.Escape;
}

#endregion User Interaction Common
