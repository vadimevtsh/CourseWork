using System.Text.RegularExpressions;

namespace CourseWork;

public static partial class InvertedIndex
{
    private const string FolderPath = "Data";
    
    public static event Action? NeedToStopServer;
    public static event Action? NeedToStartServer;
    
    private static readonly Regex _regex = Regex();
    
    [GeneratedRegex("[^a-zA-Z0-9 -]")]
    private static partial Regex Regex();

    private static int _threadsCount;

    public static void Initialize(int threadsCount)
    {
        _threadsCount = threadsCount;

        var watcher = new FileSystemWatcher(@"D:\CourseWork\CourseWork\CourseWork\bin\Debug\net7.0\Data");
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.EnableRaisingEvents = true;
        watcher.Changed += (_, _) => Reinitialize(); 
        
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        InitializeProcess();

        watch.Stop();
        
        Console.WriteLine($"Finished parsing all files for: {watch.ElapsedMilliseconds} ms");
    }

    public static IEnumerable<string> GetPaths(string word)
    {
        ThreadDictionary.TryGetValue(word, out var paths);
        return paths;
    }

    private static void InitializeProcess()
    {
        var tasks = new Task[_threadsCount];
        for (var i = 0; i < _threadsCount; i++)
        {
            var threadNumber = i;
            tasks[i] = Task.Factory.StartNew(() => { Process(threadNumber); });
        }
        
        Task.WaitAll(tasks);
    }

    private static void Process(int threadNumber)
    {
        var filePaths = Directory.GetFiles(FolderPath);
        var filePathsPerThread = (int)Math.Ceiling((float)filePaths.Length / _threadsCount);

        for (var i = threadNumber * filePathsPerThread; i < (threadNumber + 1) * filePathsPerThread && i < filePaths.Length; i++)
        {
            var filePath = filePaths[i];
            var words = File.ReadAllText(filePath).Split(" ");

            for (var j = 0; j < words.Length; j++)
            {
                var word = words[j];
                
                word = word.ToLower();
                word = _regex.Replace(word, "");
                
                if (word == string.Empty)
                    continue;
                
                ThreadDictionary.AddOrUpdate(word, new List<string> { filePath });
            }
        }
    }

    private static void Reinitialize()
    {
        NeedToStopServer?.Invoke();
        
        ThreadDictionary.Clear();
        InitializeProcess();
        
        NeedToStartServer?.Invoke();
    }
}