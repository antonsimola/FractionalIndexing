#Fractional Indexing

This is based on [JS implementation by rocicorp](https://github.com/rocicorp/fractional-indexing)
which in turn is based on [Implementing Fractional Indexing
](https://observablehq.com/@dgreensp/implementing-fractional-indexing) by [David Greenspan
](https://github.com/dgreensp).

Use it to generate short string keys which help maintain ordered lists. 

For example when reordering to-do items, drag and dropping between items, or as last / first one.

## Usage
```charp
var key1 = OrderKeyGenerator.GenerateKeyBetween(null, null)); // to get started
key2 = OrderKeyGenerator.GenerateKeyBetween(key1, null)); // after key1
var key3 = OrderKeyGenerator.GenerateKeyBetween(key1, key2)); // between key1 and key2 
var key4 = OrderKeyGenerator.GenerateKeyBetween(key3, null)); // last one
var key0 = OrderKeyGenerator.GenerateKeyBetween(null, key1)); // first one
```