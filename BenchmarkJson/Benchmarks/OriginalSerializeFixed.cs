using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using Newtonsoft.Json;
using JsonSerializer = SimpleJsonSerializer.JsonSerializer;

namespace BenchmarkJson.Benchmarks;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class OriginalSerializeFixed
{
    private static readonly CalibrationPoint[] TestPoints =
        [new CalibrationPoint(30, 30, 552, 514), new CalibrationPoint(290, 210, 3341, 3353)];

    [Benchmark]
    public string SimpleJsonSerializeTest()
    {
        return JsonSerializer.SerializeObject(TestPoints);
    }

    [Benchmark]
    public string NewtonsoftJsonSerializeTest()
    {
        return JsonConvert.SerializeObject(TestPoints);
    }

    [Benchmark]
    public string SystemTextJsonSerializeTest()
    {
        return System.Text.Json.JsonSerializer.Serialize(TestPoints);
    }

    [Benchmark(Baseline = true)]
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
