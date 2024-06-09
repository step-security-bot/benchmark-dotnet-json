using System.Text.Json.Serialization;

namespace BenchmarkJson.Benchmarks.NativeAot;

[JsonSerializable(typeof(CalibrationPoint))]
internal partial class NativeAotSourceGenerationContext : JsonSerializerContext;
