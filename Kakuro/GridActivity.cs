using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using SQLite;

using Android.App;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Kakuro
{
    [Activity(Label = "Kakuro")]
    public class GridActivity : Activity
    {
        Cell[,] fullField;
        Cell[,] field;

        private Dictionary<View, ValueTuple<int, int>> cells = new Dictionary<View, ValueTuple<int, int>>();

        private bool isInputWaiting = false;

        private Button awaitingInputCell, checkButton;

        private int hintsCount = 1;

        private bool isWon = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.GridPage);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            string difficulty = Intent.GetStringExtra("difficulty"); // В эту строку передается сложность
            RecordsHelper.GameDifficulty dif = RecordsHelper.GameDifficulty.hard;

            if (difficulty == "easy")
            {
                hintsCount = 5;
                dif = RecordsHelper.GameDifficulty.easy;
            }
            else if (difficulty == "medium")
            {
                hintsCount = 3;
                dif = RecordsHelper.GameDifficulty.medium;
            }

            try
            {
                field = Field.CreateField(difficulty, out fullField); // Готовое поле, на основке которого пишем интерфейс
            }
            catch (Exception)
            {
                field = Field.CreateField(difficulty, out fullField);
            }
            InitializeGameField(field);

            CreateEditorPanel();
            System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
            AlertDialog.Builder winDialog = new AlertDialog.Builder(this);
            AlertDialog.Builder loseDialog = new AlertDialog.Builder(this);
            swatch.Start();

            checkButton = FindViewById<Button>(2131230769);


            checkButton.Click += delegate (object sender, EventArgs e)
            {
                if (CheckField())
                {
                    if (isWon)
                        winDialog.Show();
                    else
                    {
                        swatch.Stop();
                        isWon = true;
                        winDialog.SetTitle("Победа!");
                        winDialog.SetMessage($"Вы смогли решить головоломку всего за {swatch.Elapsed.Minutes} минут и {swatch.Elapsed.Seconds} секунд!");
                        winDialog.SetNeutralButton("Ок", delegate
                        {

                        });
                        winDialog.Show();
                        RecordsHelper.GameRecord gameRecord = new RecordsHelper.GameRecord(dif, swatch.Elapsed);
                        RecordsHelper.UpdateRecordTable(gameRecord);
                        RecordsHelper.SaveRecordTable(RecordsHelper.GetRecordTable());
                    }
                }
                else
                {
                    loseDialog.SetTitle("Неправильно!");
                    loseDialog.SetMessage($"К сожалению, ваше решение неверное:(");
                    loseDialog.SetNeutralButton("Ок", delegate
                    {

                    });
                    loseDialog.Show();
                }
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void CreateEditorPanel()
        {
            var table = FindViewById<TableLayout>(Resource.Id.gameFieldEditorLayout);
            table.SetBackgroundColor(Color.LightGray);

            var row1 = new TableRow(this);
            var row2 = new TableRow(this);

            for (int i = 1; i < 13; i++)
            {
                var cellView = new Button(this);
                cellView.SetText($"{i}", TextView.BufferType.Normal);
                cellView.SetBackgroundColor(Color.White);
                cellView.SetTextColor(Color.Peru);
                cellView.SetTextSize(Android.Util.ComplexUnitType.Dip, 15);
                var layoutParams = new TableRow.LayoutParams(120, 120);
                layoutParams.SetMargins(1, 1, 1, 1);
                cellView.LayoutParameters = layoutParams;
                cellView.Click += EditorCell_Click;

                if (i < 7)
                {
                    if (i == 6)
                    {
                        cellView.SetTextColor(Color.Red);
                        cellView.SetText("X", TextView.BufferType.Normal);
                    }
                    row1.AddView(cellView);
                }
                else if (i > 6 && i < 13)
                {
                    if (i == 11)
                    {
                        cellView.SetText("", TextView.BufferType.Normal);
                    }
                    else if (i == 12)
                    {
                        cellView.SetTextColor(Color.Blue);
                        cellView.SetText("?", TextView.BufferType.Normal);
                    }
                    else
                    {
                        cellView.SetText($"{i - 1}", TextView.BufferType.Normal);
                    }
                    row2.AddView(cellView);
                }
            }


            table.AddView(row1, 0);
            table.AddView(row2, 1);
        }

        private void InitializeGameField(Cell[,] field)
        {
            var table = FindViewById<TableLayout>(Resource.Id.gameFieldLayout);
            table.SetBackgroundColor(Color.Gray);

            var width = field.GetLength(0);
            var height = field.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                var row = new TableRow(this);

                for (int j = 0; j < width; j++)
                {
                    View view = null;
                    var cell = field[i, j];
                    var cellView = new Button(this);
                    var layoutParams = new TableRow.LayoutParams(100, 100);
                    layoutParams.SetMargins(1, 1, 1, 1);
                    cellView.LayoutParameters = layoutParams;
                    if (cell.Check)
                    {
                        cellView.SetBackgroundColor(Color.White);
                        cellView.SetTextSize(Android.Util.ComplexUnitType.Dip, 12);
                        if (cell.Value != 0)
                        {
                            cellView.SetTextColor(Color.DarkGreen);
                            cellView.SetText($"{cell.Value}", TextView.BufferType.Normal);
                        }
                        else
                        {
                            cellView.SetTextColor(Color.Black);
                            cellView.Click += CellView_Click;
                        }
                        view = cellView;
                    }
                    else
                    {
                        view = new BlackCellView(this, cell.VertValue, cell.HorValue);
                        view.LayoutParameters = layoutParams;
                    }
                    row.AddView(view);

                    cells.Add(view, (i, j));
                }

                table.AddView(row, i);
            }
        }

        private void CellView_Click(object sender, EventArgs e)
        {
            var cellView = sender as Button;

            if (isInputWaiting == false)
            {
                cellView.SetBackgroundColor(Color.LightGray);
                awaitingInputCell = cellView;
                isInputWaiting = true;
            }
            else
            {
                awaitingInputCell.SetBackgroundColor(Color.White);
                cellView.SetBackgroundColor(Color.LightGray);
                awaitingInputCell = cellView;
            }
        }

        private void EditorCell_Click(object sender, EventArgs e)
        {
            if (isWon)
            {
                Toast.MakeText(ApplicationContext, $"Игра окончена! Вы смогли решить головоломку!", ToastLength.Short).Show();
                return;
            }

            if (isInputWaiting == false)
            {
                return;
            }

            var editorCell = sender as Button;
            var cellText = editorCell.Text;
            var cellIndex = cells[awaitingInputCell];
            var cell = field[cellIndex.Item1, cellIndex.Item2];

            if (cellText == "X")
            {
                cell.Value = 0;
                awaitingInputCell.SetText(string.Empty, TextView.BufferType.Normal);
            }
            else if (cellText == "?")
            {
                if (hintsCount == 0)
                {
                    Toast.MakeText(ApplicationContext, $"Подсказок больше нет!", ToastLength.Short).Show();
                }
                else if (isInputWaiting)
                {
                    var fullCell = fullField[cellIndex.Item1, cellIndex.Item2];
                    cell.Value = fullCell.Value;
                    awaitingInputCell.SetTextColor(Color.Blue);
                    awaitingInputCell.SetText($"{fullCell.Value}", TextView.BufferType.Normal);
                    awaitingInputCell.Click -= CellView_Click;
                    awaitingInputCell.SetBackgroundColor(Color.White);
                    awaitingInputCell = null;
                    isInputWaiting = false;
                    hintsCount--;
                    if (hintsCount != 1)
                        Toast.MakeText(ApplicationContext, $"Осталось {hintsCount} подсказки!", ToastLength.Short).Show();
                    else
                        Toast.MakeText(ApplicationContext, $"Осталось {hintsCount} подсказка!", ToastLength.Short).Show();
                }
            }
            else if (int.TryParse(editorCell.Text, out var value))
            {
                cell.Value = value;
                awaitingInputCell.SetText(editorCell.Text, TextView.BufferType.Normal);
            }

            var isAllSetted = true;
            var w = field.GetLength(0);
            var h = field.GetLength(1);

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if ((isAllSetted = field[i, j].Value != 0) == false)
                    {
                        break;
                    }
                }

                if (isAllSetted == false)
                {
                    break;
                }
            }
        }

        private bool CheckField()
        {
            var w = field.GetLength(0);
            var h = field.GetLength(1);

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (field[i, j].Value != fullField[i, j].Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}