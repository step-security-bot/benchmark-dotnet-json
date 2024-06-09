using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace BenchmarkJson.Benchmarks.NativeAot;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class NativeAotSerialize
{
    private static readonly CalibrationPoint[] TestPoints =
    [
        new CalibrationPoint(30, 30, 552, 514),
        new CalibrationPoint(290, 210, 3341, 3353),
    ];

#if IS_NATIVE_AOT
    [Benchmark(Baseline = true)]
    public string Serialize()
    {
        return JsonSerializer.Serialize(TestPoints);
    }
#endif

    [Benchmark]
    public string SerializeAot()
    {
        return JsonSerializer.Serialize(TestPoints, NativeAotSourceGenerationContext.Default.CalibrationPointArray);
    }

    [Benchmark]
    public byte[] SerializeUtf8Writer()
    {

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        writer.WriteStartArray();
        foreach (CalibrationPoint point in TestPoints)
        {
            writer.WriteStartObject();
            writer.WriteNumber("ScreenX", point.ScreenX);
            writer.WriteNumber("ScreenY", point.ScreenY);
            writer.WriteNumber("RawX", point.RawX);
            writer.WriteNumber("RawY", point.RawY);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
        writer.Flush();
        return stream.ToArray();
    }

    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default
                .WithRuntime(CoreRuntime.Core80)
            );
            AddJob(Job.Default
                .WithRuntime(NativeAotRuntime.Net80)
            );
        }
    }
}
