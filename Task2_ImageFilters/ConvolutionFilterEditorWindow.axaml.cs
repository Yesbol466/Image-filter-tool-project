using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Task2_Filters.Common;

namespace Task2_ImageFilters
{
    public partial class ConvolutionFilterEditorWindow : Window
    {
        private int[,] _kernel;
        private int _divisor;
        private int _offset;
        private (int, int) _anchorPoint;

        public ConvolutionFilterEditorWindow()
        {
            InitializeComponent();
        }

        private void UpdateKernelDataGrid()
        {
            if (!int.TryParse(RowsTextBox.Text, out int rows) || !int.TryParse(ColumnsTextBox.Text, out int columns))
            {
                return;
            }

            _kernel = new int[rows, columns];
            KernelDataGrid.Columns.Clear();
            for (int i = 0; i < columns; i++)
            {
                KernelDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = i.ToString(),
                    Binding = new Avalonia.Data.Binding($"[{i}]"),
                    IsReadOnly = false
                });
            }

            var data = new List<int[]>();
            for (int i = 0; i < rows; i++)
            {
                var row = new int[columns];
                for (int j = 0; j < columns; j++)
                {
                    row[j] = _kernel[i, j];
                }
                data.Add(row);
            }

            KernelDataGrid.ItemsSource = data;
        }

        private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            UpdateKernelDataGrid();
        }

        private void EditCellsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var cellEditorWindow = new CellEditorWindow(_kernel);
            cellEditorWindow.ShowDialog(this);
        }

        private async void LoadFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Console.WriteLine("Starting test");
            var openFileDialog = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } }
                }
            });
            if (openFileDialog != null && openFileDialog.Count > 0)
            {
                var file = openFileDialog[0];
                var stream = await file.OpenReadAsync();
                using (var reader = new StreamReader(stream))
                {
                    if (int.TryParse(reader.ReadLine(), out int rows) && int.TryParse(reader.ReadLine(), out int columns))
                    {
                        _kernel = new int[rows, columns];
                        _divisor = int.Parse(reader.ReadLine());
                        _offset = int.Parse(reader.ReadLine());
                        var anchorPoint = reader.ReadLine().Split(',');
                        _anchorPoint = (int.Parse(anchorPoint[0]), int.Parse(anchorPoint[1]));

                        for (int i = 0; i < rows; i++)
                        {
                            var row = reader.ReadLine().Split(' ');
                            for (int j = 0; j < columns; j++)
                            {
                                _kernel[i, j] = int.Parse(row[j]);
                            }
                        }

                        Console.WriteLine("Filter loaded:");
                        Console.WriteLine($"Rows: {rows}, Columns: {columns}");
                        Console.WriteLine($"Divisor: {_divisor}, Offset: {_offset}");
                        Console.WriteLine($"Anchor Point: {_anchorPoint}");
                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = 0; j < columns; j++)
                            {
                                Console.Write($"{_kernel[i, j]} ");
                            }
                            Console.WriteLine();
                        }

                       // UpdateKernelDataGrid();
                    }
                }
            }
        }

        private async void SaveFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var saveFileDialog = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } }
                }
            });
            if (saveFileDialog != null)
            {
                var stream = await saveFileDialog.OpenWriteAsync();
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(_kernel.GetLength(0));
                    writer.WriteLine(_kernel.GetLength(1));
                    writer.WriteLine(_divisor);
                    writer.WriteLine(_offset);
                    writer.WriteLine($"{_anchorPoint.Item1},{_anchorPoint.Item2}");
                    for (int i = 0; i < _kernel.GetLength(0); i++)
                    {
                        for (int j = 0; j < _kernel.GetLength(1); j++)
                        {
                            writer.Write(_kernel[i, j] + " ");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        private void ApplyFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!int.TryParse(DivisorTextBox.Text, out _divisor))
            {
                _divisor = 1;
            }

            if (!int.TryParse(OffsetTextBox.Text, out _offset))
            {
                _offset = 0;
            }

            var selectedAnchor = AnchorPointComboBox.SelectedItem as ComboBoxItem;
            if (selectedAnchor != null)
            {
                switch (selectedAnchor.Content.ToString())
                {
                    case "Top-Left":
                        _anchorPoint = (0, 0);
                        break;
                    case "Top-Right":
                        _anchorPoint = (0, _kernel.GetLength(1) - 1);
                        break;
                    case "Bottom-Left":
                        _anchorPoint = (_kernel.GetLength(0) - 1, 0);
                        break;
                    case "Bottom-Right":
                        _anchorPoint = (_kernel.GetLength(0) - 1, _kernel.GetLength(1) - 1);
                        break;
                    default:
                        _anchorPoint = (_kernel.GetLength(0) / 2, _kernel.GetLength(1) / 2);
                        break;
                }
            }

            var mainWindow = (ImageFilterViewerWindow?)this.Owner;
            if (mainWindow != null)
            {
                var inputArray = BitmapConverters.BitmapTo2DArray(mainWindow.GetBitmap());
                var filteredArray = ApplyConvolutionFilter(inputArray, _kernel, _divisor, _offset, _anchorPoint);
                mainWindow.ApplyFilter(filteredArray);
            }
        }

        private void ApplyLoadedFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWindow = (ImageFilterViewerWindow?)this.Owner;
            if (mainWindow != null)
            {
                var inputArray = BitmapConverters.BitmapTo2DArray(mainWindow.GetBitmap());
                var filteredArray = ApplyConvolutionFilter(inputArray, _kernel, _divisor, _offset, _anchorPoint);

                Console.WriteLine("Applying loaded filter:");
                Console.WriteLine($"Divisor: {_divisor}, Offset: {_offset}");
                Console.WriteLine($"Anchor Point: {_anchorPoint}");
                for (int i = 0; i < _kernel.GetLength(0); i++)
                {
                    for (int j = 0; j < _kernel.GetLength(1); j++)
                    {
                        Console.Write($"{_kernel[i, j]} ");
                    }
                    Console.WriteLine();
                }

                mainWindow.ApplyFilter(filteredArray);
            }
        }
        

        private PixelColor[,] ApplyConvolutionFilter(PixelColor[,] array, int[,] kernel, int divisor, int offset, (int, int) anchorPoint)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            var result = new PixelColor[width, height];

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);
            int kernelOffsetX = anchorPoint.Item1;
            int kernelOffsetY = anchorPoint.Item2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int r = 0, g = 0, b = 0;

                    for (int ky = 0; ky < kernelHeight; ky++)
                    {
                        for (int kx = 0; kx < kernelWidth; kx++)
                        {
                            int pixelX = Math.Clamp(x + kx - kernelOffsetX, 0, width - 1);
                            int pixelY = Math.Clamp(y + ky - kernelOffsetY, 0, height - 1);

                            var pixel = array[pixelX, pixelY];
                            int weight = kernel[kx, ky];

                            r += pixel.R * weight;
                            g += pixel.G * weight;
                            b += pixel.B * weight;
                        }
                    }

                    r = Math.Clamp(r / divisor + offset, 0, 255);
                    g = Math.Clamp(g / divisor + offset, 0, 255);
                    b = Math.Clamp(b / divisor + offset, 0, 255);

                    result[x, y] = new PixelColor(r, g, b);
                }
            }

            return result;
        }
    }
}
