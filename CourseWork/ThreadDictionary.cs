namespace CourseWork;

public static class ThreadDictionary
{
    private static readonly IDictionary<string, HashSet<string>> _threadDictionary = new Dictionary<string, HashSet<string>>();

    public static void AddOrUpdate(string key, string value)
    {
        lock (_threadDictionary)
        {
            if (_threadDictionary.TryGetValue(key, out var paths))
            {
                paths.Add(value);
            }
            else
            {
                var newHashset = new HashSet<string> { value };
                _threadDictionary.Add(key, newHashset);
            }
        }
    }

    public static bool TryGetValue(string key, out HashSet<string> paths)
    {
        lock (_threadDictionary)
        {
            if (_threadDictionary.TryGetValue(key, out var value))
            {
                paths = value;
                return true;
            }

            Console.WriteLine($"ThreadDictionary: There are no values for the key: {key}");
            paths = new HashSet<string>();
            return false;
        }
    }

    public static IDictionary<string, HashSet<string>> GetDictionary()
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