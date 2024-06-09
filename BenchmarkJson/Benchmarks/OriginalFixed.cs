using System.Collections;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Newtonsoft.Json;
using JsonSerializer = SimpleJsonSerializer.JsonSerializer;

namespace BenchmarkJson.Benchmarks;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class OriginalFixed
{
    private const string TestJson =
        """[{"ScreenX":30,"ScreenY":30,"RawX":552,"RawY":514},{"ScreenX":290,"ScreenY":210,"RawX":3341,"RawY":3353}]""";

    private static readonly CalibrationPoint[] TestPoints =
        [new CalibrationPoint(30, 30, 552, 514), new CalibrationPoint(290, 210, 3341, 3353)];

    [Benchmark]
    public string SimpleJsonSerializeTest()
    {
        return JsonSerializer.SerializeObject(TestPoints);
    }

    [Benchmark]
    public CalibrationPoint[] SimpleJsonDeserializeTest()
    {
        object? o = JsonSerializer.DeserializeString(TestJson);
        var list = new List<CalibrationPoint>();
        // null dereference
        foreach (object? node in o as ArrayList)
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

        return list.ToArray();
    }

    [Benchmark]
    public string NewtonsoftJsonSerializeTest()
    {
        return JsonConvert.SerializeObject(TestPoints);
    }

    [Benchmark]
    public CalibrationPoint[]? NewtonsoftJsonDeserializeTest()
    {
        return JsonConvert.DeserializeObject<CalibrationPoint[]>(TestJson);
    }

    [Benchmark]
    public string SystemTextJsonSerializeTest()
    {
        return System.Text.Json.JsonSerializer.Serialize(TestPoints);
    }

    [Benchmark]
    public CalibrationPoint[]? SystemTextJsonDeserializeTest()
    {
        return System.Text.Json.JsonSerializer.Deserialize<CalibrationPoint[]>(TestJson);
    }

    [Benchmark]
    public string ManualSerializeTest()
    {
        var sb = new StringBuilder("[");
        foreach (CalibrationPoint point in TestPoints)
        {
            // this creates invalid JSON, because of the trailing comma
            sb.Append(
                $"{{\"ScreenX\":{point.ScreenX},\"ScreenY\":{point.ScreenY},\"RawX\":{point.RawX},\"RawY\":{point.RawY}}},");
        }

        sb.Remove(sb.Length - 1, 1);

        sb.Append(']');
        return sb.ToString();
    }

    [Benchmark]
    public CalibrationPoint[] ManualDeserializeTest()
    {
        var list = new List<CalibrationPoint>();

        int? GetPropertyValue(string node, string name)
        {
            int nameStart = node.IndexOf(name, StringComparison.Ordinal);
            if (nameStart < 0)
            {
                return null;
            }

            int valueStart = node.IndexOf(':', nameStart) + 1;
            if (valueStart < 0)
            {
                return null;
            }

            int valueEnd = node.IndexOfAny([',', ' ', '}'], valueStart);
            return int.Parse(node.Substring(valueStart, valueEnd - valueStart));
        }

        int index = 0;
        while (index < TestJson.Length)
        {
            int nodeStart = TestJson.IndexOf('{', index);
            if (nodeStart < 0)
            {
                break;
            }

            int nodeEnd = TestJson.IndexOf('}', nodeStart) + 1;

            string node = TestJson.Substring(nodeStart, nodeEnd - nodeStart);
            list.Add(new CalibrationPoint
            {
                // unchecked null reference exception here
                ScreenX = GetPropertyValue(node, "ScreenX").GetValueOrDefault(),
                ScreenY = GetPropertyValue(node, "ScreenY").GetValueOrDefault(),
                RawX = GetPropertyValue(node, "RawX").GetValueOrDefault(),
                RawY = GetPropertyValue(node, "RawY").GetValueOrDefault()
            });
            index = nodeEnd;
        }

        return list.ToArray();
    }

    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default);
        }
    }
}
