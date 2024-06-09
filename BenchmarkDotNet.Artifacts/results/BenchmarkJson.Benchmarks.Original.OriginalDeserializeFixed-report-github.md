```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3593/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method                        | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |-----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| SimpleJsonDeserializeTest     | 6,925.6 ns | 34.46 ns | 30.54 ns | 15.24 |    0.22 | 0.4044 |      - |    3432 B |        3.61 |
| NewtonsoftJsonDeserializeTest | 1,943.4 ns | 20.91 ns | 18.54 ns |  4.28 |    0.07 | 0.3700 | 0.0038 |    3120 B |        3.28 |
| SystemTextJsonDeserializeTest | 1,010.0 ns |  6.13 ns |  5.73 ns |  2.22 |    0.04 | 0.0744 |      - |     632 B |        0.66 |
| ManualDeserializeTest         |   454.5 ns |  4.56 ns |  5.07 ns |  1.00 |    0.00 | 0.1135 |      - |     952 B |        1.00 |
