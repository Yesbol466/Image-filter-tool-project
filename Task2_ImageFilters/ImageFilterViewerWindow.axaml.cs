using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.ComponentModel;
using System;
using Task2_Filters.Common;
using System.Collections.Generic;
using Avalonia.Platform.Storage;
using Avalonia;

namespace Task2_ImageFilters;

public partial class ImageFilterViewerWindow : Window, INotifyPropertyChanged
{
    public WriteableBitmap _bitmap = null!;
    private WriteableBitmap _originalBitmap = null!; 
    private (double[,], double[,], double[,]) _hsvData;

    public ImageFilterViewerWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadDefaultImage();
    }

    public WriteableBitmap GetBitmap()
    {
        return _bitmap;
    }

    public void LoadDefaultImage()
    {
        var uri = new Uri("avares://Task2_ImageFilters/placeholder_image.jpeg");
        var bitmap = new Bitmap(AssetLoader.Open(uri));
        _originalBitmap = BitmapConverters.LoadImageAsWriteableBitmap(bitmap);
        _bitmap = _originalBitmap;
        InputImage.Source = _bitmap;
    }

    public void ApplyFilter(PixelColor[,] array)
    {
        _bitmap = BitmapConverters.ArrayToBitmap(array);
        InputImage.Source = _bitmap;
    }

    private void ResetImageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _bitmap = _originalBitmap;
        InputImage.Source = _bitmap;
        HueImage.Source = null;
        SaturationImage.Source = null;
        ValueImage.Source = null;
        ReconstructedImage.Source = null;

    }

    private async void LoadImageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var openFileDialog = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Image Files") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } }
            }
        });
        if (openFileDialog != null && openFileDialog.Count > 0)
        {
            var file = openFileDialog[0];
            var stream = await file.OpenReadAsync();
            var bitmap = new Bitmap(stream);
            _originalBitmap = BitmapConverters.LoadImageAsWriteableBitmap(bitmap);
            _bitmap = _originalBitmap;
            InputImage.Source = _bitmap;
        }
    }

    public void ApplyGaussianBlurFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyGaussianBlurFilter(inputArray));
    }

    public void ApplyLaplaceFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyLaplaceFilter(inputArray));
    }

    public void ApplyEmbossFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyEmbossFilter(inputArray));
    }

    public void ApplyInversionFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyInversionFilter(inputArray));
    }

    public void ApplyBrightnessCorrectionFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyBrightnessCorrectionFilter(inputArray, 50)); // Example brightness value
    }

    public void ApplyContrastEnhancementFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyContrastEnhancementFilter(inputArray, 1.5)); // Example contrast value
    }

    public void ApplyGammaCorrectionFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyGammaCorrectionFilter(inputArray, 2.2)); // Example gamma value
    }

    public void ApplyBlurFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyBlurFilter(inputArray));
    }

    private void ApplySharpenFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplySharpenFilter(inputArray));
    }

    private void ApplyEdgeDetectionFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ApplyEdgeDetectionFilter(inputArray));
    }

    private void EditConvolutionFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var editorWindow = new ConvolutionFilterEditorWindow();
        editorWindow.ShowDialog(this);
    }

    private void ApplyMedianFilterButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("MedianFilterSizeTextBox");
        if (textBox != null && int.TryParse(textBox.Text, out int n) && n > 0 && n % 2 != 0)
        {
            PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
            ApplyFilter(FiltersLibrary.ApplyMedianFilter(inputArray, n));
        }
        else
        {
            Console.WriteLine("Invalid input");
        }
    }

    private void ApplyAverageDitheringButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("DitheringShadesTextBox");
        if (textBox != null && int.TryParse(textBox.Text, out int shades) && shades > 1)
        {
            PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
            ApplyFilter(FiltersLibrary.ApplyAverageDithering(inputArray, shades));
        }
        else
        {
            Console.WriteLine("Invalid input");
        }
    }

    private void ApplyKMeansQuantizationButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("KMeansClustersTextBox");
        if (textBox != null && int.TryParse(textBox.Text, out int k) && k > 1)
        {
            PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
            ApplyFilter(FiltersLibrary.ApplyKMeansQuantization(inputArray, k));
        }
        else
        {
            Console.WriteLine("Invalid input");
        }
    }

    private void ConvertToGrayscaleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        ApplyFilter(FiltersLibrary.ConvertToGrayscale(inputArray));
    }

    private async void SaveImageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var saveFileDialog = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            FileTypeChoices = new List<FilePickerFileType>
            {
                new FilePickerFileType("Image Files") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } }
            }
        });
        if (saveFileDialog != null)
        {
            var stream = await saveFileDialog.OpenWriteAsync();
            _bitmap.Save(stream);
        }
    }

    private void ConvertToHSVButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PixelColor[,] inputArray = BitmapConverters.BitmapTo2DArray(_bitmap);
        _hsvData = FiltersLibrary.ConvertToHSV(inputArray);

        HueImage.Source = BitmapConverters.ArrayToBitmap(ConvertToPixelColorArray(_hsvData.Item1));
        SaturationImage.Source = BitmapConverters.ArrayToBitmap(ConvertToPixelColorArray(_hsvData.Item2));
        ValueImage.Source = BitmapConverters.ArrayToBitmap(ConvertToPixelColorArray(_hsvData.Item3));
    }

    private void ConvertToRGBButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_hsvData.Item1 != null && _hsvData.Item2 != null && _hsvData.Item3 != null)
        {
            PixelColor[,] rgbArray = FiltersLibrary.ConvertToRGB(_hsvData.Item1, _hsvData.Item2, _hsvData.Item3);
            ReconstructedImage.Source = BitmapConverters.ArrayToBitmap(rgbArray);
        }
    }
    private PixelColor[,] ConvertToPixelColorArray(double[,] array)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int value = (int)(array[x, y] * 255);
                result[x, y] = new PixelColor(value, value, value);
            }
        }

        return result;
    }

}
