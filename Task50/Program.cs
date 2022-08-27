// Задача 50. Напишите программу, которая на вход принимает позиции элемента
// в двумерном массиве, и возвращает значение этого элемента
// или же указание, что такого элемента нет.
// Например, задан массив:
// 1 4 7 2
// 5 9 2 3
// 8 4 2 4
// 1, 7 -> такого элемента в массиве нет

const int AutoMatrixMinSize = 4;
const int AutoMatrixMaxSize = 15;
const int AutoMatrixMinValue = -200;
const int AutoMatrixMaxValue = 200;

do
{
	Console.Clear();
	PrintTitle("Вывод значения элемента матрицы по его позиции", ConsoleColor.Cyan);
	Console.WriteLine("\nЖелаете задать параметры матрицы?:");
	Console.WriteLine("\u2023 Enter или Y \u2014 задать основные параметры матрицы,");
	Console.WriteLine("\u2023 Esc или N \u2014 создать случайную матрицу в полностью автоматическом режиме.");

	CreationMode? creationMode;
	while (!(creationMode = AskHowToCreateMatrix()).HasValue) { }
	Console.WriteLine();

	int m, n, minimum, maximum;
	if (creationMode.Value == CreationMode.Auto)
	{
		Random rnd = new Random();
		m = rnd.Next(AutoMatrixMinSize, AutoMatrixMaxSize + 1);
		n = rnd.Next(AutoMatrixMinSize, AutoMatrixMaxSize + 1);
		minimum = AutoMatrixMinValue;
		maximum = AutoMatrixMaxValue;
	}
	else
	{
		m = GetUserInputInt("Введите число строк:\t", 1);
		n = GetUserInputInt("Введите число столбцов:\t", 1);
		minimum = GetUserInputInt("Введите нижний предел диапазона случайных целых чисел:  ");
		maximum = GetUserInputInt("Введите верхний предел диапазона случайных целых чисел: ", minimum);
		Console.WriteLine();
	}

	int[,] mtx = CreateMatrixRandomInt(m, n, minimum, maximum);
	PrintColored($"Матрица {m} \u2715 {n}:\n", ConsoleColor.DarkGray);
	PrintMatrix(mtx);

	Console.WriteLine("\nЗадайте позицию элемента для поиска...");
	int posRow = GetUserInputInt("Введите номер строки (нумерация с 1):\t", 1);
	int posCol = GetUserInputInt("Введите номер столбца (нумерация с 1):\t", 1);
	Console.WriteLine();

	int? valueAtPos = GetMatrixItemValue(mtx, posRow - 1, posCol - 1);
	if (valueAtPos.HasValue)
		PrintColored($"Значение элемента в заданной позиции = {valueAtPos.Value}", ConsoleColor.Yellow);
	else
		PrintColored($"В матрице нет элемента по заданной позиции!", ConsoleColor.Red);

	Console.WriteLine();

} while (AskForRepeat());

// Methods:

static T? GetMatrixItemValue<T>(T[,] matrix, int rowIndex, int colIndex) where T : struct
{
	if (rowIndex < 0 || rowIndex >= matrix.GetLength(0))
		return null;
	if (colIndex < 0 || colIndex >= matrix.GetLength(1))
		return null;

	return matrix[rowIndex, colIndex];
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

#endregion Print Matrix Generic

#region User Interaction Spec

static CreationMode? AskHowToCreateMatrix()
{
	ConsoleKeyInfo key = Console.ReadKey(true);
	if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.N)
		return CreationMode.Auto;
	else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Y)
		return CreationMode.Manual;

	return null;
}

#endregion User Interaction Spec

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

#region Types

enum CreationMode
{
	Auto = 0,
	Manual
}

#endregion Types
