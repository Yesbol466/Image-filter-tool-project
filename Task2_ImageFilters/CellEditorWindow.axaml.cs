using Avalonia.Controls;
using System.Collections.Generic;

namespace Task2_ImageFilters
{
    public partial class CellEditorWindow : Window
    {
        private int[,] _kernel;

        public CellEditorWindow(int[,] kernel)
        {
            InitializeComponent();
            _kernel = kernel;
            UpdateCellGrid();
        }

        private void UpdateCellGrid()
        {
            int rows = _kernel.GetLength(0);
            int columns = _kernel.GetLength(1);

            CellGrid.Rows = rows;
            CellGrid.Columns = columns;
            CellGrid.Children.Clear();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var textBox = new TextBox
                    {
                        Text = _kernel[i, j].ToString(),
                        Margin = new Avalonia.Thickness(5)
                    };
                    CellGrid.Children.Add(textBox);
                }
            }
        }

        private void ApplyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int rows = _kernel.GetLength(0);
            int columns = _kernel.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var textBox = (TextBox)CellGrid.Children[i * columns + j];
                    if (int.TryParse(textBox.Text, out int value))
                    {
                        _kernel[i, j] = value;
                    }
                }
            }
            this.Close();
        }

        private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
