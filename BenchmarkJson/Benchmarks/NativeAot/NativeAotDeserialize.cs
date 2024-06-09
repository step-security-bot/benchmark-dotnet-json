using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace BenchmarkJson.Benchmarks.NativeAot;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class NativeAotDeserialize
{
    private const string TestJson =
        """[{"ScreenX":30,"ScreenY":30,"RawX":552,"RawY":514},{"ScreenX":290,"ScreenY":210,"RawX":3341,"RawY":3353}]""";

    private readonly byte[] _testJsonUtf8 = Encoding.UTF8.GetBytes(TestJson);

    private static readonly byte[] ScreenXUtf8 = Encoding.UTF8.GetBytes("ScreenX");
    private static readonly byte[] ScreenYUtf8 = Encoding.UTF8.GetBytes("ScreenY");
    private static readonly byte[] RawXUtf8 = Encoding.UTF8.GetBytes("RawX");
    private static readonly byte[] RawYUtf8 = Encoding.UTF8.GetBytes("RawY");


#if IS_NATIVE_AOT
    [Benchmark(Baseline = true)]
    public CalibrationPoint[]? Deserialize()
    {
        return JsonSerializer.Deserialize<CalibrationPoint[]>(TestJson);
    }
#endif

    [Benchmark]
    public CalibrationPoint[]? DeserializeAot()
    {
        return JsonSerializer.Deserialize(TestJson, NativeAotSourceGenerationContext.Default.CalibrationPointArray);
    }

    [Benchmark]
    public CalibrationPoint[] DeserializeUtf8Reader()
    {
        var reader = new Utf8JsonReader(_testJsonUtf8);
        List<CalibrationPoint> list = [];
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                continue;
            }

            var point = new CalibrationPoint();
            list.Add(point);
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                if (reader.ValueTextEquals(ScreenXUtf8))
                {
                    reader.Read();
                    point.ScreenX = reader.GetInt32();
                }
                else if (reader.ValueTextEquals(ScreenYUtf8))
                {
                    reader.Read();
                    point.ScreenY = reader.GetInt32();
                }
                else if (reader.ValueTextEquals(RawXUtf8))
                {
                    reader.Read();
                    point.RawX = reader.GetInt32();
                }
                else if (reader.ValueTextEquals(RawYUtf8))
                {
                    reader.Read();
                    point.RawY = reader.GetInt32();
                }
            }
        }

        return list.ToArray();
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
