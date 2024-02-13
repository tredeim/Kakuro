using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Kakuro
{
    public class Field
    {
        private static Random rand = new Random();

        private const int Min = 2;

        /// <summary>
        /// Метод проверяет возможность сделать клетку на поле черной
        /// </summary>
        /// <param name="field">Поле</param>
        /// <param name="i0">Координата y</param>
        /// <param name="j0">Координата x</param>
        /// <param name="Min">Минимальная длина ряда белых клеток</param>
        /// <returns>Результат проверки</returns>
        public static bool Check(Cell[,] field, int i0, int j0, int Min)
        {
            if (!field[i0, j0].Check) // Если клетка изначально не черная
                return false;
            bool vertCheck = false, horCheck = false; // Проверка по горизонтали и вертикали
            int vertBot = field.GetLength(0), vertTop = 0; // Значение ближайших черных клеток по вертикали 
            int horLeft = 0, horRight = field.GetLength(1); // Значение ближайших черных клеток по горизонтали
            for (int i = 1; i < i0; i++) // Ищем ближайшую черную клетку по вертикали сверху от полученной
            {
                if (field[i, j0].Check == false)
                    vertTop = i;
            }
            for (int i = i0 + 1; i < field.GetLength(0); i++) // Ищем ближайшую черную клетку по вертикали снизу от полученной
            {
                if (field[i, j0].Check == false)
                {
                    vertBot = i;
                    break;
                }
            }
            for (int j = 1; j < j0; j++) // Ищем ближайшую черную клетку по горизонтали слева от полученной
            {
                if (field[i0, j].Check == false)
                    horLeft = j;
            }
            for (int j = j0 + 1; j < field.GetLength(1); j++) // Ищем ближайшую черную клетку по горизонтали справа от полученной
            {
                if (field[i0, j].Check == false)
                {
                    horRight = j;
                    break;
                }
            } // Проверка на возможность сделать эту самую клетку черной
            if ((Math.Abs(i0 - vertTop) == 1 && Math.Abs(i0 - vertBot) == 1) || (Math.Abs(i0 - vertTop) == 1 && Math.Abs(i0 - vertBot) > Min) ||
                (Math.Abs(i0 - vertTop) > Min && Math.Abs(i0 - vertBot) == 1) || (Math.Abs(i0 - vertTop) > Min && Math.Abs(i0 - vertBot) > Min))
                vertCheck = true;
            if ((Math.Abs(j0 - horLeft) == 1 && Math.Abs(j0 - horRight) == 1) || (Math.Abs(j0 - horLeft) == 1 && Math.Abs(j0 - horRight) > Min) ||
                (Math.Abs(j0 - horLeft) > Min && Math.Abs(j0 - horRight) == 1) || (Math.Abs(j0 - horLeft) > Min && Math.Abs(j0 - horRight) > Min))
                horCheck = true;
            return vertCheck && horCheck;
        }

        public static Cell[,] CreateField(string difficulty, out Cell[,] fullField)
        {
            Cell[,] field = null;
            fullField = null;
            NewCell[,] grid = null;
            Field f = new Field();
            int nVar = 2;
            int i = 0, j = 0;
            while (nVar > 1)
            {
                field = CreateFullField(difficulty); // Создаем поле для нужной сложности
                fullField = new Cell[field.GetLength(0), field.GetLength(0)];
                for (i = 0; i < field.GetLength(0); i++) // Записываем созданное поле в другое, чтобы при необходимости брать оттуда подсказку
                    for (j = 0; j < field.GetLength(0); j++)
                    {
                        fullField[i, j] = new Cell();
                        fullField[i, j].Value = field[i, j].Value;
                    }
                i = rand.Next(1, field.GetLength(0));
                j = rand.Next(1, field.GetLength(0));
                if (field[i, j].Check && field[i, j].Value != 0)
                    field[i, j].Value = 0;
                CreateFile(field);
                grid = f.LoadPuzzle();
                nVar = f.Solve();
            }
            while (nVar <= 1)
            {
                i = rand.Next(1, field.GetLength(0));
                j = rand.Next(1, field.GetLength(0));
                if (field[i, j].Check && field[i, j].Value != 0)
                    field[i, j].Value = 0;
                CreateFile(field);
                grid = f.LoadPuzzle();
                nVar = f.Solve();
            }
            field[i, j].Value = fullField[i, j].Value;
            return field;
        }


        public static Cell[,] CreateFullField(string difficulty)
        {
            Cell[,] field;
            int size = 0;
            if (difficulty == "easy") // Значения по умолчанию для легкой сложности
                size = rand.Next(5, 6 + 1);
            if (difficulty == "normal") // Значения по умолчанию для средней сложности
                size = rand.Next(7, 8 + 1);
            if (difficulty == "hard") // Значения по умолчанию для тяжелой сложности
                size = rand.Next(9, 10 + 1);
            field = new Cell[size, size]; // Создаем поле
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    field[i, j] = new Cell();

            // Делаем все блоки по левой вертикали и верхней горизонтали черными
            for (int i = 0; i < size; i++)
            {
                field[0, i].Check = false;
                field[i, 0].Check = false;
            }

            // Все остальные блоки делаем белыми
            for (int i = 1; i < size; i++)
                for (int j = 1; j < size; j++)
                    field[i, j].Check = true;

            for (int i = 1; i < size; i++)
            {
                int cellCount = 0;
                if (difficulty == "easy")
                    cellCount = rand.Next(0, 2 + 1); // Генерируем кол-во блоков для каждой строки поля
                if (difficulty == "normal")
                    cellCount = rand.Next(0, 4 + 1); // Генерируем кол-во блоков для каждой строки поля
                if (difficulty == "hard")
                    cellCount = rand.Next(0, 6 + 1); // Генерируем кол-во блоков для каждой строки поля
                int[] jCheck = new int[size - 1]; // Массив, который состоит из клеток в строке, которые уже черные или не могут стать черными из-за своего расположения
                int count = 0; // Счетчик для заполнения массива
                while (cellCount != 0) // Пока кол-во черных блоков для строки не равно 0
                {
                    if (!jCheck.Contains(0)) // Т.к. массив изначально заполнен 0, то если 0 не осталось, значит, все клетки в строке исчерпали свои возможности
                        cellCount = 0;
                    int j = rand.Next(1, size); // Генерируем клетку, которая может стать черной
                    if (!jCheck.Contains(j)) // Если в массиве проверки не было данной клетки, то добавляем
                        jCheck[count++] = j;
                    if (Check(field, i, j, Min)) // Проверяем клетку на возможность стать черной
                    {
                        cellCount--; // Уменьшаем кол-во черных клеток для этой строки
                        field[i, j].Check = false; // Делаем клетку черной
                    }
                }
            }

            FillNumbersInCells(field); // Заполняем в поле белые клетки рандомными цифрами
            FillNumbersInBlackCells(field); // Заполняем в поле черные клетки суммой белых

            return field;
        }

        /// <summary>
        /// Метод записывает созданное поле в файл
        /// </summary>
        /// <param name="field"></param>
        public static void CreateFile(Cell[,] field)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, "task.txt");

            using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Default))
            {
                streamWriter.WriteLine($"{field.GetLength(0)} {field.GetLength(0)}");
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        if (field[i, j].Check)
                        {
                            if (field[i, j].Value == 0)
                                streamWriter.Write(".");
                            else
                            {
                                streamWriter.Write(field[i, j].Value);
                            }
                            if (j != field.GetLength(1) - 1)
                                streamWriter.Write(" ");
                        }
                        else
                        {
                            if (field[i, j].VertValue != 0)
                                streamWriter.Write($"{field[i, j].VertValue}");
                            streamWriter.Write("\\");
                            if (field[i, j].HorValue != 0)
                                streamWriter.Write($"{field[i, j].HorValue}");
                            if (j != field.GetLength(1) - 1)
                                streamWriter.Write(" ");
                        }
                    }
                    streamWriter.WriteLine();
                }
            }
        }

        //ЗАГРУЖАЕМ ЗАДАЧУ С ДИСКА
        public NewCell[,] LoadPuzzle()
        {
            List<string> puzzle = new List<string>();
            // очищаем cписок строк:
            puzzle.Clear();

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, "task.txt");
            //загружаем задачу с диска:
            StreamReader sr = new StreamReader(filePath);
            //считываем слова:
            string s = null;
            while ((s = sr.ReadLine()) != null)
            {
                // заменяем точки нулями:
                s = s.Replace('.', '0');
                puzzle.Add(s);
            }
            sr.Close();
            sr = null;

            int CellCol = 0, CellRow = 0;
            // в первой строке - размеры сетки:
            string[] str2 = puzzle[0].Split(new char[] { ' ', '-', 'x' });
            int n = 0;
            if (int.TryParse(str2[0], out n))
            {
                CellCol = n;
            }
            if (int.TryParse(str2[1], out n))
            {
                CellRow = n;
            }

            // очищаем список блоков:
            lstBlocks.Clear();
            // создаем пустую сетку:
            Grid = new NewCell[CellCol, CellRow];
            nSolved = 0;

            // сетка - строки 1 ..CellRow:
            for (int i = 1; i < CellRow + 1; ++i)
            {
                s = puzzle[i];
                int r = i - 1;

                // разбиваем строку:
                string[] asc = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int ns = 0; ns < asc.Length; ++ns)
                {
                    string sc = asc[ns];
                    // определяем цвет клетки:
                    int pos = sc.IndexOf('\\');
                    // черная клетка:
                    if (pos != -1)
                    {
                        // Создаем новую клетку:
                        Grid[ns, r] = new NewCell(false);
                        // ее решать не нужно:
                        ++nSolved;
                        Point nums = extractNums(sc, pos);
                        // горизонтальный блок:
                        if (nums.X > 0)
                        {
                            Block bl = new Block(ns, r, BlockDirection.BD_HORIZ, (int)nums.X);
                            lstBlocks.Add(bl);
                        }
                        //вертикальный блок:
                        if (nums.Y > 0)
                        {
                            Block bl = new Block(ns, r, BlockDirection.BD_VERT, (int)nums.Y);
                            lstBlocks.Add(bl);
                        }
                    }
                    // белая клетка:
                    else
                    {
                        // Создаем новую клетку:
                        Grid[ns, r] = new NewCell(true);
                        // какое в ней число:
                        int num = 0;
                        if (int.TryParse(sc, out num))
                        {
                            if (num != 0)
                            {
                                Grid[ns, r].num = num;
                                Grid[ns, r].fix = true;
                                ++nSolved;
                            }
                        }
                        else
                            Console.WriteLine("Неверный символ в строке!");
                    }
                }

            }

            // блоки
            foreach (Block bl in lstBlocks)
            {
                // коорд. черной клетки:
                int x = bl.blackX;
                int y = bl.blackY;
                // направление:
                BlockDirection bd = bl.dir;
                // число белых клеток:
                int nWhite = 0;
                if (bd == BlockDirection.BD_HORIZ)
                {
                    while (x + 1 < Grid.GetLength(0) && Grid[x + 1, y].color)
                    {
                        // добавляем блок в клетку:
                        Grid[x + 1, y].lstBlocks.Add(bl);
                        // добавляем число в множество:
                        if (Grid[x + 1, y].num != 0)
                            bl.setNums.Add(Grid[x + 1, y].num);
                        ++nWhite;
                        ++x;
                    }
                }
                else
                {
                    while (y + 1 < Grid.GetLength(1) && Grid[x, y + 1].color)
                    {
                        // добавляем блок в клетку:
                        Grid[x, y + 1].lstBlocks.Add(bl);
                        // добавляем число в множество:
                        if (Grid[x, y + 1].num != 0)
                            bl.setNums.Add(Grid[x, y + 1].num);
                        ++nWhite;
                        ++y;
                    }
                }
                bl.nWhite = nWhite;
            }
            return Grid;
        }


        //ВЫДЕЛЯЕМ ЧИСЛА ИЗ СТРОКИ
        public Point extractNums(string s, int pos)
        {
            int num = 0;
            //первое число:
            int n1 = 0;
            if (pos > 0)
            {
                //подстрока с числом:
                string s1 = s.Substring(0, pos);
                if (int.TryParse(s1, out num))
                {
                    n1 = num;
                }
                else
                {
                    return new Point(-1, -1);
                }
            }
            //второе число:
            int n2 = 0;
            //подстрока с числом:
            string s2 = s.Substring(pos + 1);
            if (s2 != String.Empty)
            {
                if (int.TryParse(s2, out num))
                {
                    n2 = num;
                }
                else
                {
                    return new Point(-1, -1);
                }
            }
            return new Point(n2, n1);
        }

        public int Solve()
        {
            nVar = 0;
            solve(1);
            return nVar;
        }

        public void solve(int n)
        {
            //ищем свободную клетку:
            Point newCell = FindNextCell();
            int c = (int)newCell.X;
            int r = (int)newCell.Y;

            for (int num = 1; num <= 9; ++num)
            {
                //ищем oчередную цифру:
                if (!test(c, r, num)) continue;

                //ставим в сетку:
                placeNum(c, r, num);

                if (nSolved < Grid.GetLength(0) * Grid.GetLength(1)) solve(n + 1);
                else
                {
                    ++nVar;
                    //Запоминаем последнее решение:
                    Solution = new NewCell[Grid.GetLength(0), Grid.GetLength(1)];
                    for (int j = 0; j < Grid.GetLength(1); ++j)
                        for (int i = 0; i < Grid.GetLength(0); ++i)
                        {
                            bool fix = Grid[i, j].fix;
                            int nm = Grid[i, j].num;
                            bool color = Grid[i, j].color;
                            Solution[i, j] = new NewCell(nm, fix, color);
                        }
                }
                //возвращаемся назад:
                deleteNum(c, r);
            }

        }
        List<Block> lstBlocks = new List<Block>();

        //ИЩЕМ СВОБОДНУЮ КЛЕТКУ
        public Point FindNextCell()
        {
            int dx = 0;
            int dy = 0;
            int xf = 0;
            int yf = 0;
            //наибольш. кол-во чисел в блоке:
            int maxSolved = 0;
            //при этом осталось пустых клеток:
            int minEmpty = 1000;
            //номер блока:
            int nb0 = 0;
            Block bl = null;
            //по всем блокам:
            for (int i = 0; i < lstBlocks.Count; ++i)
            {
                bl = lstBlocks[i];
                //число пустых клеток:
                int n0 = bl.nWhite - bl.setNums.Count;
                //уже решен:
                if (n0 == 0) continue;
                //осталась одна неразгаданная клетка:
                if (n0 == 1)
                {
                    dx = bl.getDx();
                    dy = bl.getDy();
                    xf = bl.blackX + dx;
                    yf = bl.blackY + dy;
                    while (Grid[xf, yf].num != 0)
                    {
                        xf += dx;
                        yf += dy;
                    }
                    return new Point(xf, yf);
                }
                //осталось 2 и больше клеток:
                else
                {
                    if (bl.setNums.Count > maxSolved)
                    {
                        //запоминаем номер блока:
                        nb0 = i;
                        maxSolved = bl.setNums.Count;
                        minEmpty = n0;
                    }
                    else if (bl.setNums.Count == maxSolved && n0 < minEmpty)
                    {
                        //запоминаем номер блока:
                        nb0 = i;
                        maxSolved = bl.setNums.Count;
                        minEmpty = n0;
                    }
                }
            }

            bl = lstBlocks[nb0];
            dx = bl.getDx();
            dy = bl.getDy();
            xf = bl.blackX + dx;
            yf = bl.blackY + dy;
            while (xf < Grid.GetLength(0) && yf < Grid.GetLength(1) && Grid[xf, yf].num != 0)
            {
                xf += dx;
                yf += dy;
            }
            return new Point(xf, yf);
        }

        public bool test(int c, int r, int num)
        {
            //проверяем число на повтор-->
            //по всем блокам клетки:
            foreach (Block bl in Grid[c, r].lstBlocks)
            {
                if (bl.setNums.Contains(num)) return false;
            }

            //проверяем сумму
            foreach (Block bl in Grid[c, r].lstBlocks)
            {
                //должна быть:
                int sum = bl.sum;
                //текущая сумма:
                int act = bl.setNums.Sum() + num;
                int n0 = bl.nWhite - bl.setNums.Count;

                //if (n0 == 1 && act == sum) return true;
                if (n0 == 1 && act != sum) return false;
                if (act > sum) return false;
            }

            return true;
        }

        public void deleteNum(int c, int r)
        {
            int num = Grid[c, r].num;
            Grid[c, r].num = 0;

            //по всем блокам клетки:
            foreach (Block bl in Grid[c, r].lstBlocks)
            {
                bl.setNums.Remove(num);
            }

            --nSolved;
            return;
        }

        //ставим число в сетку
        public void placeNum(int c, int r, int num)
        {
            Grid[c, r].num = num;

            //по всем блокам клетки:
            foreach (Block bl in Grid[c, r].lstBlocks)
            {
                bl.setNums.Add(num);
            }

            ++nSolved;
            return;
        }

        // сетка с числами:
        NewCell[,] Grid;
        NewCell[,] Solution;

        //число вариантов решения задачи:
        int nVar;

        //выставлено чисел:
        int nSolved;

        /// <summary>
        /// Метод приводит поле в изначальное состояние, когда нет цифр и чисел в клетках
        /// </summary>
        /// <param name="field"></param>
        public static void Refresh(Cell[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j].Check)
                        field[i, j].Value = 0;
                    else
                        field[i, j].Value = -1;
                }
        }

        /// <summary>
        /// Метод зануляет все белые клетки
        /// </summary>
        /// <param name="field"></param>
        public static void RefreshWhiteCells(Cell[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
                for (int j = 0; j < field.GetLength(1); j++)
                    if (field[i, j].Check)
                        field[i, j].Value = 0;
        }

        /// <summary>
        /// Метод для заполнения черных клеток суммой белых клеток
        /// </summary>
        /// <param name="field"></param>
        public static void FillNumbersInBlackCells(Cell[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j].Check)
                        continue;
                    int k = j + 1;
                    while (k < field.GetLength(1) && field[i, k].Check)
                    {
                        field[i, j].HorValue += field[i, k].Value;
                        k++;
                    }
                    int l = i + 1;
                    while (l < field.GetLength(0) && field[l, j].Check)
                    {
                        field[i, j].VertValue += field[l, j].Value;
                        l++;
                    }
                }
            }
        }

        /// <summary>
        /// Метод для заполнения белых клеток рандомными числами от 1 до 9
        /// </summary>
        /// <param name="field"></param>
        public static void FillNumbersInCells(Cell[,] field)
        {
            for (var i = 1; i < field.GetLength(0); i++)
            {
                for (var j = 1; j < field.GetLength(1); j++)
                {
                    if (!field[i, j].Check)
                        continue;
                    bool check = false;
                    var curVal = 0;
                    var count = 0;
                    while (!check && count < 100)
                    {
                        curVal = rand.Next(1, 9 + 1);
                        bool check1 = true,
                             check2 = true,
                             check3 = true,
                             check4 = true;
                        for (var k = j + 1; k < field.GetLength(1); k++)
                        {
                            if (!field[i, k].Check)
                                break;
                            else if (field[i, k].Value == curVal)
                            {
                                check1 = false;
                                break;
                            }
                        }
                        for (var k = j - 1; k > 0; k--)
                        {
                            if (!field[i, k].Check)
                                break;
                            else if (field[i, k].Value == curVal)
                            {
                                check2 = false;
                                break;
                            }
                        }
                        for (var k = i + 1; k < field.GetLength(0); k++)
                        {
                            if (!field[k, j].Check)
                                break;
                            else if (field[k, j].Value == curVal)
                            {
                                check3 = false;
                                break;
                            }
                        }
                        for (var k = i - 1; k > 0; k--)
                        {
                            if (!field[k, j].Check)
                                break;
                            else if (field[k, j].Value == curVal)
                            {
                                check4 = false;
                                break;
                            }
                        }
                        count++;
                        check = check1 && check2 && check3 && check4;
                    }
                    if (count == 100)
                    {
                        Refresh(field);
                        i = 1;
                        j = 0;
                    }
                    else
                        field[i, j].Value = curVal;
                }
            }
        }
    }
}


