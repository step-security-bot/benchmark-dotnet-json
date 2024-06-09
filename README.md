# Json serialization benchmarks

This repository is in response of [Serializers in IoT: Json.NET and System.Text.Json are Both Terrible!](https://youtu.be/ZkqcNQifSgI) video by @yoshimoshi-garage

I believe that his benchmarks in the video are not fair, nor valid, because he didn't actually benchmark the libraries or implementations.
For a readout of what are the pitfalls of rolling your own benchmarks, you can visit the [BenchmarkDotNet website](https://benchmarkdotnet.org/index.html#reliability).

As an embedded systems programmer, I would highly recommend him to read through the advanced documentation of System.Text.Json, because it is specifically made for high performance scenarios.

## Run benchmarks

Run all benchmarks:

``` bash
dotnet run --project BenchmarkJson -c Release -- -f *
```

### Run the original code, converted to BenchmarkDotNet

``` bash
dotnet run --project BenchmarkJson -c Release -- --filter *Original*
```

I am unfortunately not able to test this on embedded systems, however his findings were that the original difference between the approaches was relatively the same on all platforms. With the F7 being slower in general.
