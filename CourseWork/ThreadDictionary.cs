namespace CourseWork;

public static class ThreadDictionary
{
    private static readonly IDictionary<string, List<string>> _threadDictionary = new Dictionary<string, List<string>>();

    public static void AddOrUpdate(string key, List<string> value)
    {
        lock (_threadDictionary)
        {
            if (_threadDictionary.TryGetValue(key, out var paths))
            {
                if (paths.Contains(value.First()) == false)
                {
                    paths.AddRange(value);
                }
            }
            else
            {
                _threadDictionary.Add(key, value);
            }
        }
    }

    public static bool TryGetValue(string key, out List<string> paths)
    {
        lock (_threadDictionary)
        {
            if (_threadDictionary.TryGetValue(key, out var value))
            {
                paths = value;
                return true;
            }

            Console.WriteLine($"ThreadDictionary: There are no values for the key: {key}");
            paths = new List<string>();
            return false;
        }
    }

    public static IDictionary<string, List<string>> GetDictionary()
    {
        lock (_threadDictionary)
        {
            return _threadDictionary;
        }
    }

    public static void Clear()
    {
        lock (_threadDictionary)
        {
            _threadDictionary.Clear();
        }
    }
}