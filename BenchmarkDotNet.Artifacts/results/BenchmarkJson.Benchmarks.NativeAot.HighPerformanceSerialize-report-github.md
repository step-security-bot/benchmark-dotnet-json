```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3593/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method              | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
| Serialize           | 450.7 ns | 8.62 ns | 8.46 ns |  1.00 |    0.00 | 0.0648 |     544 B |        1.00 |
| SerializeAot        | 251.1 ns | 2.66 ns | 2.49 ns |  0.56 |    0.01 | 0.0277 |     232 B |        0.43 |
| SerializeUtf8Writer | 398.9 ns | 3.29 ns | 3.08 ns |  0.88 |    0.02 | 0.1087 |     912 B |        1.68 |
