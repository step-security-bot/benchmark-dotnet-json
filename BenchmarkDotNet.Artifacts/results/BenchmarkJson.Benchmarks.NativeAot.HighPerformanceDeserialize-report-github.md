```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3593/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method                | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| Deserialize           | 996.8 ns |  7.19 ns |  6.72 ns |  1.00 |    0.00 | 0.0744 |     632 B |        1.00 |
| DeserializeAot        | 959.4 ns |  6.86 ns |  6.41 ns |  0.96 |    0.01 | 0.0744 |     632 B |        1.00 |
| DeserializeUtf8Reader | 586.2 ns | 11.33 ns | 13.91 ns |  0.59 |    0.02 | 0.0229 |     192 B |        0.30 |
