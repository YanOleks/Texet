using Antlr4.Runtime;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Texet.Utils;


namespace Texet
{
    public partial class MainPage : ContentPage
    {
        private Models.Table _table = new();
        public MainPage()
        {
            InitializeComponent();
            LoadTable();            
        }
        //private void LoadTable()
        //{
        //    Models.Table tableModel = new Models.Table();

        //    if (File.Exists(tableModel.FileName))
        //    {
        //        tableModel = Models.Table.Load();
        //    }

        //    _table = tableModel;
        //    if (_table.Data.Columns.Count == 0) {
        //        _table.AddColumn();
        //        _table.AddRow();
        //    }
        //}
        private void OnAddColumnClicked(object sender, EventArgs e)
        {
            _table.AddColumn();
            Grid grid = (Grid)((Button)sender).Parent.FindByName("tableGrid");
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int newColumnIndex = grid.ColumnDefinitions.Count - 1;
            var label = new Label { Text = $"{(char)(newColumnIndex + 64)}" };
            grid.Add(label, newColumnIndex);
            for (int row = 1; row < grid.RowDefinitions.Count; row++)
            {
                var entry = new Entry();
                entry.TextChanged += (s, e) => _table.UpdateCell(row, newColumnIndex, entry.Text);
                //entry.Completed += (s, e) => _table.UpdateCell(row, newColumnIndex, entry.Text);
                entry.Focused += (s, e) => OnFocus(s, e);
                entry.Unfocused += (s, e) => OnUnfocus(s, e);
                grid.Add(entry, newColumnIndex, row);
            }
        }

        private void OnAddRowClicked(object sender, EventArgs e)
        {
            _table.AddRow();
            Grid grid = (Grid)((Button)sender).Parent.FindByName("tableGrid");
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            int newRowIndex = grid.RowDefinitions.Count - 1;
            var label = new Label { Text = $"{newRowIndex}" };
            grid.Add(label, 0,newRowIndex);
            for (int column = 1; column < grid.ColumnDefinitions.Count; column++)
            {
                var entry = new Entry();
                entry.TextChanged += (s, e) => _table.UpdateCell(newRowIndex, column, entry.Text);
                //entry.Completed += (s, e) => _table.UpdateCell(newRowIndex, column, entry.Text);
                entry.Focused += (s, e) => OnFocus(s, e);
                entry.Unfocused += (s, e) => OnUnfocus(s, e);
                grid.Add(entry, column, newRowIndex);
            }
        }

        private void OnRemoveColumnClicked(object sender, EventArgs e)
        {
            _table.RemoveColumn();
            Grid tableGrid = (Grid)((Button)sender).Parent.FindByName("tableGrid");
            if (tableGrid.ColumnDefinitions.Count > 0)
            {
                int lastColumnIndex = tableGrid.ColumnDefinitions.Count - 1;

                for (int row = 0; row < tableGrid.RowDefinitions.Count; row++)
                {
                    var elementsInLastColumn = tableGrid.Children
                        .Where(view => Grid.GetRow((BindableObject)view) == row && Grid.GetColumn((BindableObject)view) == lastColumnIndex)
                        .ToList();
                    foreach (var element in elementsInLastColumn)
                    {
                        tableGrid.Children.Remove(element);
                    }
                }

                tableGrid.ColumnDefinitions.RemoveAt(lastColumnIndex);
            }
        }
        private void OnRemoveRowClicked(object sender, EventArgs e)
        {
            _table.RemoveRow();
            Grid tableGrid = (Grid)((Button)sender).Parent.FindByName("tableGrid");
            if (tableGrid.RowDefinitions.Count > 0)
            {
                int lastRowIndex = tableGrid.RowDefinitions.Count - 1;

                for (int column = 0; column < tableGrid.ColumnDefinitions.Count; column++)
                {
                    var elementsInLastRow = tableGrid.Children
                        .Where(view => Grid.GetColumn((BindableObject)view) == column && Grid.GetRow((BindableObject)view) == lastRowIndex)
                        .ToList();
                    foreach (var element in elementsInLastRow)
                    {
                        tableGrid.Children.Remove(element);
                    }
                }

                tableGrid.RowDefinitions.RemoveAt(lastRowIndex);
            }
        }
        private void OnSave(object sender, EventArgs e)
        {
            _table.Save();
        }
        private void OnTextChanged(object? sender, EventArgs e)
        {
            if (sender is Entry entry)
            {
                UpdateCell(entry);
            }
        }
        private void OnFocus(object? sender, EventArgs e)
        {
            if (sender is not Entry entry) return;

            int row = Grid.GetRow(entry) - 1;
            int column = Grid.GetColumn(entry) - 1;

            if (row >= 0 && column >= 0 && row < _table.Data.Rows.Count && column < _table.Data.Columns.Count)
            {
                string cellValue = _table.Data.Rows[row][column]?.ToString() ?? string.Empty;
                entry.Text = cellValue;
            }
        }

        private void OnUnfocus(object? sender, EventArgs e)
        {
            if (sender is null) return;
            if (sender is not Entry entry) return;
            UpdateCell(entry);
            entry.Text = ResolveValue(entry.Text);
        }
        
        private void UpdateCell(Entry entry)
        {
            int row = Grid.GetRow(entry);
            int column = Grid.GetColumn(entry);
            _table.UpdateCell(row - 1, column - 1, entry.Text);
        }
        private void LoadTable()
        {
            Models.Table tableModel = new Models.Table();
            
            if (File.Exists(System.IO.Path.Combine(FileSystem.AppDataDirectory, tableModel.FileName)))
            {
                tableModel = Models.Table.Load();
            }

            _table = tableModel;
            if (_table.Data.Columns.Count == 0)
            {
                _table.AddColumn();
                _table.AddRow();
            }

            RenderTable();
        }

        private void RenderTable()
        {
            Grid grid = (Grid)this.FindByName("tableGrid");

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();

            for (int col = 0; col < _table.Data.Columns.Count; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var label = new Label { Text = $"{(char)(col + 65)}" };
                grid.Add(label, col + 1, 0);
            }

            for (int row = 0; row < _table.Data.Rows.Count; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                var label = new Label { Text = $"{row + 1}" };
                grid.Add(label, 0, row + 1);

                for (int col = 0; col < _table.Data.Columns.Count; col++)
                {
                    var entry = new Entry { Text = _table.Data.Rows[row][col]?.ToString() ?? string.Empty };
                    entry.Completed += (s, e) => OnTextChanged(s, e);
                    entry.Focused += (s, e) => OnFocus(s, e);
                    entry.Unfocused += (s, e) => OnUnfocus(s, e);
                    grid.Add(entry, col + 1, row + 1);
                }
            }
        }
        
        private bool IsFormula(string entry)
        {
            if (string.IsNullOrEmpty(entry)) return false;
            return entry[0] == '=';
        }

        private string ResolveValue(string entry)
        {
            return IsFormula(entry) ? EvaluateFormula(entry) : entry;
        }
        private string EvaluateFormula(string entry)
        {
            try
            {
                string formula = entry.Substring(1);
                formula = Regex.Replace(formula, @"[A-Z][0-9]+", match => GetCellReference(match.Value));
                //string? result = Utils.Calculator.EvaluateFormula(formula);
                //return result is null ? "" : result;
                var lexer = new ExpressionLexer(new AntlrInputStream(formula));
                var tokens = new CommonTokenStream(lexer);
                var parser = new ExpressionParser(tokens);

                var tree = parser.expression();
                var calculator = new Expression();
                var result = calculator.Visit(tree);
                return Convert.ToString(result);
            }
            catch {
                return "Error: invalid expression";
            }
        }
        private string GetCellReference(string cellReference)
        {
            int column = cellReference[0] - 'A';
            if (int.TryParse(cellReference[1..], out int row))
            {
                row -= 1;

                if (row >= 0 && column >= 0 && row < _table.Data.Rows.Count && column < _table.Data.Columns.Count)
                {
                    string cellContent = _table.Data.Rows[row][column]?.ToString() ?? "0";

                    return ResolveValue(cellContent);
                }
            }
            return "0";
        }

        
    }

}
