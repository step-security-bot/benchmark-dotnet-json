using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace BenchmarkJson.Benchmarks;

public static class Original
{
    private const string testJson =
        """[{"ScreenX":30,"ScreenY":30,"RawX":552,"RawY":514},{"ScreenX":290,"ScreenY":210,"RawX":3341,"RawY":3353}]""";

    private static CalibrationPoint[] testPoints =
        [new CalibrationPoint(30, 30, 552, 514), new CalibrationPoint(290, 210, 3341, 3353)];

    // This method of benchmarking is DEEPLY flawed
    // DON'T DO THIS
    public static void Main()
    {
        SimpleJsonSerializeTest();
        SimpleJsonDeserializeTest();
        SystemTextJsonSerializeTest();
        SystemTextJsonDeserializeTest();
        NewtonsoftJsonSerializeTest();
        NewtonsoftJsonDeserializeTest();
        ManualSerializeTest();
        ManualDeserializeTest();
    }

    // unnecessary async void + async void + L + ratio
    public static async void SimpleJsonSerializeTest()
    {
        var start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        string? json = SimpleJsonSerializer.JsonSerializer.SerializeObject(testPoints);
        sw.Stop();
        TimeSpan serializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"SimpleJsonSerializeTest: {end - start} bytes, {serializeTime.TotalMilliseconds} ms");
    }
    public static async void SimpleJsonDeserializeTest()
    {
        long start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        object? o = SimpleJsonSerializer.JsonSerializer.DeserializeString(testJson);
        var list = new List<CalibrationPoint>();
        // null dereference
        foreach (var node in o as ArrayList)
        {
            var props = node as Hashtable;

            list.Add(new CalibrationPoint
            {
                // null dereference
                ScreenX = Convert.ToInt32(props["ScreenX"]),
                ScreenY = Convert.ToInt32(props["ScreenY"]),
                RawX = Convert.ToInt32(props["RawX"]),
                RawY = Convert.ToInt32(props["RawY"])
            });
        }
        var data = list.ToArray();
        sw.Stop();
        TimeSpan deserializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"SimpleJsonDeserializeTest: {end - start} bytes, {deserializeTime.TotalMilliseconds} ms");
    }

    public static async void NewtonsoftJsonSerializeTest()
    {
        var start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        // unused variable falls into dead code elimination
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(testPoints);
        sw.Stop();
        TimeSpan serializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"NewtonsoftJsonSerializeTest: {end - start} bytes, {serializeTime.TotalMilliseconds} ms");
    }

    public static async void NewtonsoftJsonDeserializeTest()
    {
        var start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        // unused variable falls into dead code elimination
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CalibrationPoint[]>(testJson);
        sw.Stop();
        TimeSpan deserializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"NewtonsoftJsonDeserializeTest: {end - start} bytes, {deserializeTime.TotalMilliseconds} ms");
    }

    public static async void SystemTextJsonSerializeTest()
    {
        var start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        // unused variable falls into dead code elimination
        var json = JsonSerializer.Serialize(testPoints);
        sw.Stop();
        TimeSpan serializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"SystemTextJsonSerializeTest: {end - start} bytes, {serializeTime.TotalMilliseconds} ms");
    }

    public static async void SystemTextJsonDeserializeTest()
    {
        var start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        // unused variable falls into dead code elimination
        var data = JsonSerializer.Deserialize<CalibrationPoint[]>(testJson);
        sw.Stop();
        TimeSpan deserializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"SystemTextJsonDeserializeTest: {end - start} bytes, {deserializeTime.TotalMilliseconds} ms");
    }

    private static void ManualSerializeTest()
    {
        long start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        var sb = new StringBuilder("[");
        foreach (var point in testPoints)
        {
            // this creates invalid JSON, because of the trailing comma
            sb.Append(
                $"{{\"ScreenX\":{point.ScreenX},\"ScreenY\":{point.ScreenY},\"RawX\":{point.RawX},\"RawY\":{point.RawY}}},");
        }

        sb.Append(']');
        var json = sb.ToString();
        // sic, missing Stopwatch.Stop()
        //sw.Stop();
        TimeSpan serializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"ManualSerializeTest: {end - start} bytes, {serializeTime.TotalMilliseconds} ms");
    }

    public static void ManualDeserializeTest()
    {
        var start = GC.GetTotalMemory(false);
        var sw = Stopwatch.StartNew();
        var list = new List<CalibrationPoint>();

        int? GetPropertyValue(string node, string name)
        {
            // String.IndexOf(string) is culture-specific
            int nameStart = node.IndexOf(name);
            if (nameStart < 0)
            {
                return null;
            }

            int valueStart = node.IndexOf(':', nameStart) + 1;
            if (valueStart < 0)
            {
                return null;
            }

            // easy win of collection expression using Span is missed here, starting with .NET9/C#13
            int valueEnd = node.IndexOfAny(new char[] { ',', ' ', '}' }, valueStart);
            return int.Parse(node.Substring(valueStart, valueEnd - valueStart));
        }

        int index = 0;
        while (index < testJson.Length)
        {
            int nodeStart = testJson.IndexOf('{', index);
            if (nodeStart < 0)
            {
                break;
            }

            int nodeEnd = testJson.IndexOf('}', nodeStart) + 1;

            string node = testJson.Substring(nodeStart, nodeEnd - nodeStart);
            list.Add(new CalibrationPoint
            {
                // unchecked null reference exception here
                ScreenX = GetPropertyValue(node, "ScreenX").Value,
                ScreenY = GetPropertyValue(node, "ScreenY").Value,
                RawX = GetPropertyValue(node, "RawX").Value,
                RawY = GetPropertyValue(node, "RawY").Value,
            });
            index = nodeEnd;
        }

        var data = list.ToArray();
        sw.Stop();
        TimeSpan deserializeTime = sw.Elapsed;
        long end = GC.GetTotalMemory(false);
        Console.WriteLine($"ManualDeserializeTest: {end - start} bytes, {deserializeTime.TotalMilliseconds} ms");
    }
}

public class CalibrationPoint
{
    public int ScreenX { get; set; }
    public int ScreenY { get; set; }
    public int RawX { get; set; }
    public int RawY { get; set; }

    public CalibrationPoint()
    {
    }

    public CalibrationPoint(int screenX, int screenY, int rawX, int rawY)
    {
        ScreenX = screenX;
        ScreenY = screenY;
        RawX = rawX;
        RawY = rawY;
    }
}
