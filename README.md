# Fractional Indexing

![Nuget](https://img.shields.io/nuget/v/FractionalIndexing)

This is based on [JS implementation by rocicorp](https://github.com/rocicorp/fractional-indexing)
which in turn is based on [Implementing Fractional Indexing
](https://observablehq.com/@dgreensp/implementing-fractional-indexing) by [David Greenspan
](https://github.com/dgreensp).

Use it to generate and validation short string keys which help maintain ordered lists. 

For example when reordering to-do items, drag and dropping between items, or as last / first one.

## Usage generate
``` csharp
var key1 = OrderKeyGenerator.GenerateKeyBetween(null, null); // To get started
var key2 = OrderKeyGenerator.GenerateKeyBetween(key1, null); // After key1
var key3 = OrderKeyGenerator.GenerateKeyBetween(key1, key2); // Between key1 and key2
var key0 = OrderKeyGenerator.GenerateKeyBetween(null, key1); // First one
```

## Usage validation
``` csharp
var key = OrderKeyGenerator.GenerateKeyBetween(null, null); // New key
var inputKey = "Hello world!"; // Input value with client

var result1 = OrderKeyValidator.IsValid(key); // Result is true
var result2 = OrderKeyValidator.IsValid(inputKey); // Result is false

var result3 = OrderKeyValidator.TryValidate(key, out var error); // Result is true and error is null
var result4 = OrderKeyValidator.TryValidate(inputKey, out var error); // Result is false and error exist
```