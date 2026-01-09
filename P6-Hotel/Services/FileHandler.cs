using System.Text.Json;

namespace P6_Hotel;

public static class FileHandler
{
    private static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    private static string StorageFolder = Path.Combine(BasePath, @"..\..\..\DataStorage");

    public static void SaveFile<T>(string fileName, List<T> data)
    {
        string fullPath = Path.Combine(StorageFolder, fileName);

        try
        {
            if (!Directory.Exists(StorageFolder))
            {
                Directory.CreateDirectory(StorageFolder);
            }

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fullPath, json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static List<T> LoadFile<T>(string fileName)
    {
        string fullPath = Path.Combine(StorageFolder, fileName);
        if (!File.Exists(fullPath))
        {
            return new List<T>();
        }
        
        string json = File.ReadAllText(fullPath);
        if (string.IsNullOrEmpty(json))
        {
            return new List<T>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new List<T>();
        }
    }

public static void EnsureFileExists(string fileName)
    {
        string filePath = Path.Combine(StorageFolder, fileName);
        if (!Directory.Exists(StorageFolder))
        {
            Directory.CreateDirectory(StorageFolder);
        }

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }
}