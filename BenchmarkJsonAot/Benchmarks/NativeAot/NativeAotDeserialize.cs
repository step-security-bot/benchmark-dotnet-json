using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchmarkJsonAot.Benchmarks.NativeAot;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.NativeAot80)]
public class NativeAotDeserialize
{
    private readonly byte[] _testJsonUtf8 = Encoding.UTF8.GetBytes(TestJson);

    private const string TestJson =
        """[{"ScreenX":30,"ScreenY":30,"RawX":552,"RawY":514},{"ScreenX":290,"ScreenY":210,"RawX":3341,"RawY":3353}]""";

    private static readonly byte[] ScreenXUtf8 = "ScreenX"u8.ToArray();
    private static readonly byte[] ScreenYUtf8 = "ScreenY"u8.ToArray();
    private static readonly byte[] RawXUtf8 = "RawX"u8.ToArray();
    private static readonly byte[] RawYUtf8 = "RawY"u8.ToArray();


    [Benchmark(Baseline = true)]
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
}
