```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3593/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method                      | Mean        | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |------------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| SimpleJsonSerializeTest     | 20,165.6 ns | 83.34 ns | 77.95 ns | 97.89 |    1.26 | 0.5188 |    4457 B |        5.74 |
| NewtonsoftJsonSerializeTest |    916.9 ns | 17.21 ns | 17.68 ns |  4.44 |    0.12 | 0.2260 |    1896 B |        2.44 |
| SystemTextJsonSerializeTest |    440.7 ns |  5.44 ns |  4.83 ns |  2.14 |    0.04 | 0.0648 |     544 B |        0.70 |
| ManualSerializeTest         |    206.0 ns |  2.65 ns |  2.48 ns |  1.00 |    0.00 | 0.0927 |     776 B |        1.00 |
