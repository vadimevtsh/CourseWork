namespace CourseWork;

public static class ThreadDictionary
{
    private static readonly IDictionary<string, HashSet<string>> _threadDictionary = new Dictionary<string, HashSet<string>>();

    private static readonly object _dictionaryLock = new();

    public static void AddOrUpdate(string key, string value)
    {
        lock (_dictionaryLock)
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
        lock (_dictionaryLock)
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
        lock (_dictionaryLock)
        {
            return _threadDictionary;
        }
    }

    public static void Clear()
    {
        lock (_dictionaryLock)
        {
            _threadDictionary.Clear();
        }
    }
}