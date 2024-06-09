# Json serialization benchmarks

This repository is in response of [Serializers in IoT: Json.NET and System.Text.Json are Both Terrible!](https://youtu.be/ZkqcNQifSgI) video by @yoshimoshi-garage

I believe that his benchmarks in the video are not fair, nor valid, because he didn't actually benchmark the libraries or implementations.
For a readout of what are the pitfalls of rolling your own benchmarks, you can visit the [BenchmarkDotNet website](https://benchmarkdotnet.org/index.html#reliability).

I would highly recommend everyone to read through the advanced [documentation of `System.Text.Json`](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-utf8jsonwriter), because it is specifically made for high performance scenarios.

## Run benchmarks

Run all benchmarks:

``` bash
dotnet run --project BenchmarkJson -c Release -- -f *
```
Run all AOT benchmarks:

``` bash
dotnet run --project BenchmarkJsonAot -c Release -- -f *
```

### Run the original code, converted to BenchmarkDotNet

``` bash
dotnet run --project BenchmarkJson -c Release -- --filter *Original*
```

I am unfortunately not able to test this on embedded systems, however his findings were that the original difference between the approaches was relatively the same on all platforms. With the F7 being slower in general.

My results - unsurprisingly - are not even close to what OP measured on X64.
```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3593/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  Job-ESIKYW : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
Runtime=.NET 8.0  
```
| Method                        | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |-----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| SimpleJsonDeserializeTest     | 6,900.4 ns | 19.34 ns | 17.14 ns | 15.10 |    0.20 | 0.4044 |      - |    3432 B |        3.61 |
| NewtonsoftJsonDeserializeTest | 1,937.4 ns | 15.93 ns | 14.12 ns |  4.24 |    0.06 | 0.3700 | 0.0038 |    3120 B |        3.28 |
| SystemTextJsonDeserializeTest | 1,030.4 ns |  9.33 ns |  8.72 ns |  2.25 |    0.03 | 0.0744 |      - |     632 B |        0.66 |
| ManualDeserializeTest         |   457.5 ns |  6.04 ns |  5.65 ns |  1.00 |    0.00 | 0.1135 |      - |     952 B |        1.00 |

| Method                      | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| SimpleJsonSerializeTest     | 19,880.7 ns | 130.87 ns | 122.42 ns | 88.41 |    2.07 | 0.5188 |    4457 B |        5.74 |
| NewtonsoftJsonSerializeTest |    933.2 ns |  15.99 ns |  24.41 ns |  4.23 |    0.14 | 0.2260 |    1896 B |        2.44 |
| SystemTextJsonSerializeTest |    438.1 ns |   7.54 ns |   7.05 ns |  1.95 |    0.04 | 0.0648 |     544 B |        0.70 |
| ManualSerializeTest         |    220.8 ns |   3.82 ns |   5.95 ns |  1.00 |    0.00 | 0.0925 |     776 B |        1.00 |

As you can see, ironically SimpleJson is the slowest and consumes the most memory. Another observation is that the memory measurement of OP is flawed, because we can see that the `Manual` methods consumed more memory than `System.Text.Json`. This benchmark however does not utilize the full power of the `System.Text.Json` library, because it does not use NativeAot, nor `Utf8JsonWriter/Reader`.

## Run High-Performance benchmarks
``` bash
dotnet run --project BenchmarkJson -c Release -- --filter *HighPerformance*
```
``` bash
dotnet run --project BenchmarkJsonAot -c Release -- --filter *NativeAot*
```
Using the high performance options of `System.Text.Json` and `Utf8JsonWriter/Reader`, the results are:
```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22621.3593/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  Job-FTOAFU : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
Runtime=.NET 8.0  
```
| Method                | Mean       | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
|---------------------- |-----------:|---------:|---------:|------:|-------:|----------:|------------:|
| Deserialize           | 1,006.2 ns | 10.97 ns | 10.26 ns |  1.00 | 0.0744 |     632 B |        1.00 |
| DeserializeAot        |   927.1 ns |  9.44 ns |  8.83 ns |  0.92 | 0.0753 |     632 B |        1.00 |
| DeserializeUtf8Reader |   554.1 ns |  3.20 ns |  2.99 ns |  0.55 | 0.0229 |     192 B |        0.30 |

| Method              | Mean     | Error   | StdDev  | Ratio | Gen0   | Allocated | Alloc Ratio |
|-------------------- |---------:|--------:|--------:|------:|-------:|----------:|------------:|
| Serialize           | 440.6 ns | 4.12 ns | 3.85 ns |  1.00 | 0.0648 |     544 B |        1.00 |
| SerializeAot        | 252.9 ns | 2.35 ns | 2.20 ns |  0.57 | 0.0277 |     232 B |        0.43 |
| SerializeUtf8Writer | 397.2 ns | 5.19 ns | 4.85 ns |  0.90 | 0.1087 |     912 B |        1.68 |

The `SerializeUtf8Writer` is probably slower because of the `ToArray()` call on the memory stream. Ideally we would write to a file directly.
However, we can clearly see that the `SerializeAot` is comparable to `ManualSerializeTest` however it uses much less memory. The same is true for `DeserializeUtf8Reader`, however this too uses much less memory. However being very brittle, I would personally try to go for SourceGeneration deserialization, considering how little *boilerplate* code is needed on the developer side, for easy performance gains, even IF it is slower than hand rolling deserialization.
