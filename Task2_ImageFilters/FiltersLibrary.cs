using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Task2_Filters.Common;

namespace Task2_ImageFilters;
public static class FiltersLibrary
{
    private static IImageFilter LoadFilter(string dllPath)
    {
        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException($"DLL was not found: {dllPath}");
        }

        var assembly = Assembly.LoadFrom(dllPath);
        var filterType = assembly.GetTypes().FirstOrDefault(t => typeof(IImageFilter).IsAssignableFrom(t));
        if (filterType == null)
        {
            throw new InvalidOperationException("IImageFilter implementation not found in the DLL.");
        }

        var constructor = filterType.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
        {
            throw new InvalidOperationException("parameterless not found");
        }

        return (IImageFilter)constructor.Invoke(null);
    }

    public static PixelColor[,] ApplyGaussianBlurFilter(PixelColor[,] array)
    {
        // Part 3 - Invoke GaussianBlur Filter from the Task2_Filters.GaussianBlur.dll
        string dllPath = Path.Combine(AppContext.BaseDirectory, "Task2_Filters.GaussianBlur.dll");
        var filter = LoadFilter(dllPath);
        return filter.ApplyFilter(array);
    }

    public static PixelColor[,] ApplyLaplaceFilter(PixelColor[,] array)
    {
        // Part 4 - Invoke Laplace Filter from the Task2_Filters.Laplace.dll
        string dllPath = Path.Combine(AppContext.BaseDirectory, "Task2_Filters.Laplace.dll");
        var filter = LoadFilter(dllPath);
        return filter.ApplyFilter(array);
    }

    public static PixelColor[,] ApplyEmbossFilter(PixelColor[,] array)
    {
        // Part 5 - Invoke Emboss Filter from the Task2_Filters.Emboss.dll
        string dllPath = Path.Combine(AppContext.BaseDirectory, "Task2_Filters.Emboss.dll");
        var filter = LoadFilter(dllPath);
        return filter.ApplyFilter(array);
    }

    public static PixelColor[,] ApplyInversionFilter(PixelColor[,] array)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = array[x, y];
                result[x, y] = new PixelColor(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
            }
        }

        return result;
    }

    public static PixelColor[,] ApplyBrightnessCorrectionFilter(PixelColor[,] array, int brightness)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = array[x, y];
                result[x, y] = new PixelColor(
                    Math.Clamp(pixel.R + brightness, 0, 255),
                    Math.Clamp(pixel.G + brightness, 0, 255),
                    Math.Clamp(pixel.B + brightness, 0, 255));
            }
        }

        return result;
    }

    public static PixelColor[,] ApplyContrastEnhancementFilter(PixelColor[,] array, double contrast)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = array[x, y];
                result[x, y] = new PixelColor(
                    Math.Clamp((int)((pixel.R - 128) * contrast + 128), 0, 255),
                    Math.Clamp((int)((pixel.G - 128) * contrast + 128), 0, 255),
                    Math.Clamp((int)((pixel.B - 128) * contrast + 128), 0, 255));
            }
        }

        return result;
    }

    public static PixelColor[,] ApplyGammaCorrectionFilter(PixelColor[,] array, double gamma)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = array[x, y];
                result[x, y] = new PixelColor(
                    (int)(255 * Math.Pow(pixel.R / 255.0, gamma)),
                    (int)(255 * Math.Pow(pixel.G / 255.0, gamma)),
                    (int)(255 * Math.Pow(pixel.B / 255.0, gamma)));
            }
        }

        return result;
    }

    public static PixelColor[,] ApplyBlurFilter(PixelColor[,] array)
    {
        double[,] kernel = new double[,]
        {
        { 1.0 / 9, 1.0 / 9, 1.0 / 9 },
        { 1.0 / 9, 1.0 / 9, 1.0 / 9 },
        { 1.0 / 9, 1.0 / 9, 1.0 / 9 }
        };
        return ApplyConvolutionFilter(array, kernel);
    }

    public static PixelColor[,] ApplySharpenFilter(PixelColor[,] array)
    {
        double[,] kernel = new double[,]
        {
        { 0, -1, 0 },
        { -1, 5, -1 },
        { 0, -1, 0 }
        };
        return ApplyConvolutionFilter(array, kernel);
    }

    public static PixelColor[,] ApplyEdgeDetectionFilter(PixelColor[,] array)
    {
        double[,] kernel = new double[,]
        {
        { -1, -1, -1 },
        { -1, 8, -1 },
        { -1, -1, -1 }
        };
        return ApplyConvolutionFilter(array, kernel);
    }
    public static PixelColor[,] ApplyMedianFilter(PixelColor[,] array, int n)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];
        int offset = n / 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                List<int> rValues = new List<int>();
                List<int> gValues = new List<int>();
                List<int> bValues = new List<int>();
                for (int i = -offset; i <= offset; i++)
                {
                    for (int j = -offset; j <= offset; j++)
                    {
                        int pixelX = Math.Clamp(x + i, 0, width - 1);
                        int pixelY = Math.Clamp(y + j, 0, height - 1);
                        var pixel = array[pixelX, pixelY];
                        rValues.Add(pixel.R);
                        gValues.Add(pixel.G);
                        bValues.Add(pixel.B);
                    }
                }
                rValues.Sort();
                gValues.Sort();
                bValues.Sort();
                int medianIndex = rValues.Count / 2;
                result[x, y] = new PixelColor(rValues[medianIndex], gValues[medianIndex], bValues[medianIndex]);
            }
        }
        return result;
    }



    private static PixelColor[,] ApplyConvolutionFilter(PixelColor[,] array, double[,] kernel)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        int kernelWidth = kernel.GetLength(0);
        int kernelHeight = kernel.GetLength(1);
        int kernelOffsetX = kernelWidth / 2;
        int kernelOffsetY = kernelHeight / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double r = 0, g = 0, b = 0;

                for (int ky = 0; ky < kernelHeight; ky++)
                {
                    for (int kx = 0; kx < kernelWidth; kx++)
                    {
                        int pixelX = Math.Clamp(x + kx - kernelOffsetX, 0, width - 1);
                        int pixelY = Math.Clamp(y + ky - kernelOffsetY, 0, height - 1);

                        var pixel = array[pixelX, pixelY];
                        double weight = kernel[kx, ky];

                        r += pixel.R * weight;
                        g += pixel.G * weight;
                        b += pixel.B * weight;
                    }
                }

                result[x, y] = new PixelColor(
                    Math.Clamp((int)r, 0, 255),
                    Math.Clamp((int)g, 0, 255),
                    Math.Clamp((int)b, 0, 255));
            }
        }

        return result;
    }
    public static PixelColor[,] ApplyAverageDithering(PixelColor[,] array, int shades)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        int step = 255 / (shades - 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = array[x, y];
                int gray = (pixel.R + pixel.G + pixel.B) / 3;
                int newGray = (gray / step) * step;
                result[x, y] = new PixelColor(newGray, newGray, newGray);
            }
        }

        return result;
    }
    public static PixelColor[,] ApplyKMeansQuantization(PixelColor[,] array, int k)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        var result = new PixelColor[width, height];

        var pixels = new List<PixelColor>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pixels.Add(array[x, y]);
            }
        }

        var random = new Random();
        var centroids = pixels.OrderBy(p => random.Next()).Take(k).ToList();

        bool changed;
        var clusters = new List<List<PixelColor>>();

        do
        {
            clusters.Clear();
            for (int i = 0; i < k; i++)
            {
                clusters.Add(new List<PixelColor>());
            }
            foreach (var pixel in pixels)
            {
                int nearest = 0;
                double minDistance = double.MaxValue;
                for (int i = 0; i < k; i++)
                {
                    double distance = GetDistance(pixel, centroids[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = i;
                    }
                }
                clusters[nearest].Add(pixel);
            }
            changed = false;
            for (int i = 0; i < k; i++)
            {
                if (clusters[i].Count == 0) continue;

                int r = (int)clusters[i].Average(p => p.R);
                int g = (int)clusters[i].Average(p => p.G);
                int b = (int)clusters[i].Average(p => p.B);
                var newCentroid = new PixelColor(r, g, b);

                if (!newCentroid.Equals(centroids[i]))
                {
                    centroids[i] = newCentroid;
                    changed = true;
                }
            }
        } while (changed);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = array[x, y];
                int nearest = 0;
                double minDistance = double.MaxValue;
                for (int i = 0; i < k; i++)
                {
                    double distance = GetDistance(pixel, centroids[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = i;
                    }
                }
                result[x, y] = centroids[nearest];
            }
        }

        return result;
    }

    private static double GetDistance(PixelColor a, PixelColor b)
    {
        return Math.Sqrt(Math.Pow(a.R - b.R, 2) + Math.Pow(a.G - b.G, 2) + Math.Pow(a.B - b.B, 2));
    }


}
