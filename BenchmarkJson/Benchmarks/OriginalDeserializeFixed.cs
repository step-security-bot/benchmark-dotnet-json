using System.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using Newtonsoft.Json;
using JsonSerializer = SimpleJsonSerializer.JsonSerializer;

namespace BenchmarkJson.Benchmarks;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class OriginalDeserializeFixed
{
    private const string TestJson =
        """[{"ScreenX":30,"ScreenY":30,"RawX":552,"RawY":514},{"ScreenX":290,"ScreenY":210,"RawX":3341,"RawY":3353}]""";

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
    public CalibrationPoint[]? NewtonsoftJsonDeserializeTest()
    {
        return JsonConvert.DeserializeObject<CalibrationPoint[]>(TestJson);
    }

    [Benchmark]
    public CalibrationPoint[]? SystemTextJsonDeserializeTest()
    {
        return System.Text.Json.JsonSerializer.Deserialize<CalibrationPoint[]>(TestJson);
    }

    [Benchmark(Baseline = true)]
    public CalibrationPoint[] ManualDeserializeTest()
    {
        var list = new List<CalibrationPoint>();

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
    }

    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default
                .WithRuntime(ClrRuntime.Net481)
                .WithRuntime(CoreRuntime.Core80)
            );
        }
    }
}
