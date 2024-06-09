using System.Text.Json.Serialization;

namespace BenchmarkJsonAot.Benchmarks.NativeAot;

[JsonSerializable(typeof(CalibrationPoint))]
[JsonSerializable(typeof(CalibrationPoint[]))]
internal partial class NativeAotSourceGenerationContext : JsonSerializerContext;
