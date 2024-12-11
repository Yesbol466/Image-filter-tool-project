using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Task2_Filters.Common;

namespace Task2_ImageFilters;
public  static class FiltersLibrary
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
}