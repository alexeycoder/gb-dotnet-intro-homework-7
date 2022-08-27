// Задача 47. Задайте двумерный массив размером m×n, заполненный случайными вещественными числами.
// m = 3, n = 4.
// 0,5 7 -2 -0,2
// 1 -3,3 8 -9,9
// 8 7,8 -7,1 9

const int MaxFractionDigits = 2;

do
{
	Console.Clear();
	PrintTitle("Формирование матрицы случайных вещественных чисел", ConsoleColor.Cyan);
	int m = GetUserInputInt("Введите число строк:\t", 1);
	int n = GetUserInputInt("Введите число столбцов:\t", 1);
	double minimum = GetUserInputDbl("Введите нижний предел диапазона случайных чисел:  ");
	double maximum = GetUserInputDbl("Введите верхний предел диапазона случайных чисел: ", minimum);

	double[,] mtx = CreateMatrixRandomDbl(m, n, minimum, maximum);
	PrintColored($"\nМатрица {m} \u2715 {n}:\n", ConsoleColor.DarkGray);
	PrintMatrix(mtx, MaxFractionDigits);

} while (AskForRepeat());

// Methods:

static double[,] CreateMatrixRandomDbl(int rows, int cols, double min, double max)
{
	double[,] matrix = new double[rows, cols];

	Random rnd = new Random();
	for (int row = 0; row < rows; ++row)
	{
		for (int col = 0; col < cols; ++col)
		{
			matrix[row, col] = min + rnd.NextDouble() * (max - min);
		}
	}
	return matrix;
}

static void PrintMatrix<T>(T[,] matrix, int maxFractionDigits = 2) where T : struct, IFormattable
{
	const string padding = " ";
	const string itemsDelimiter = "  ";
	string format = GetNumbersToStringFormat(maxFractionDigits);

	string[,] stringMatrix = ToStringTable(matrix, format);

	int rowsLastIndex = stringMatrix.GetLength(0) - 1;
	int colsLastIndex = stringMatrix.GetLength(1) - 1;

	int posRight = 0;
	Console.WriteLine("\u250f"); // ┏

	for (int row = 0; row <= rowsLastIndex; ++row)
	{
		Console.Write("\u2503" + padding); // ┃->
		for (int col = 0; col < colsLastIndex; ++col)
		{
			Console.Write(stringMatrix[row, col] + itemsDelimiter);
		}
		Console.Write(stringMatrix[row, colsLastIndex]);
		Console.Write(padding + "\u2503"); // ->┃
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

static string[,] ToStringTable<T>(T[,] matrix, string format) where T : struct, IFormattable
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
	for (int col = 0; col < cols; ++col)
	{
		for (int row = 0; row < rows; ++row)
		{
			strTable[row, col] = strTable[row, col].PadLeft(maxLength);
		}
	}

	return strTable;
}

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

static double GetUserInputDbl(string inputMessage, double minAllowed = double.MinValue, double maxAllowed = double.MaxValue)
{
	const string errorMessageWrongType = "Некорректный ввод! Пожалуйста повторите\n";
	string errorMessageOutOfRange = string.Empty;
	if (minAllowed != double.MinValue && maxAllowed != double.MaxValue)
		errorMessageOutOfRange = $"Число должно быть в интервале от {minAllowed} до {maxAllowed}! Пожалуйста повторите\n";
	else if (minAllowed != double.MinValue)
		errorMessageOutOfRange = $"Число не должно быть меньше {minAllowed}! Пожалуйста повторите\n";
	else
		errorMessageOutOfRange = $"Число не должно быть больше {maxAllowed}! Пожалуйста повторите\n";

	double input = 0;
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

		string? inputStr = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(inputStr))
		{
			notANumber = true;
			continue;
		}
		notANumber = !double.TryParse(MakeInvariantToSeparator(inputStr), out input);
		outOfRange = !notANumber && (input < minAllowed || input > maxAllowed);

	} while (notANumber || outOfRange);

	return input;
}

static string MakeInvariantToSeparator(string input)
{
	char decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
	char wrongSeparator = decimalSeparator.Equals('.') ? ',' : '.';
	return input.Trim().Replace(wrongSeparator, decimalSeparator);
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
	Console.WriteLine("Нажмите Enter, чтобы повторить или Esc, чтобы завершить...");
	ConsoleKeyInfo key = Console.ReadKey(true);
	return key.Key != ConsoleKey.Escape;
}
