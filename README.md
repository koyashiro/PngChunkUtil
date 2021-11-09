# PngChunkUtil

[<img alt="Build and Test" src="https://github.com/koyashiro/PngChunkUtil/workflows/Build%20and%20Test/badge.svg">](https://github.com/koyashiro/PngChunkUtil/actions?query=workflow%3Abuild-and-test)

[PngChunkUtil](https://www.nuget.org/packages/PngChunkUtil/)

## Convert PNG(`byte[]`) into chunks(`Chunk[]`)

```cs
using System;
using System.IO;
using Koyashiro.PngChunkUtil;

var png = File.ReadAllBytes("example.png");
var chunks = PngReader.ReadBytes(png);
var chunk = chunks[0];

Console.WriteLine(chunk.Length); // 13
Console.WriteLine(chunk.ChunkType); // IHDR
Console.WriteLine(chunk.ChunkDataBytes[0]); // 0
Console.WriteLine(chunk.ChunkDataBytes[1]); // 0
Console.WriteLine(chunk.ChunkDataBytes[2]); // 4
Console.WriteLine(chunk.ChunkDataBytes[3]); // 0
```

## Convert chunks(`Chunk[]`) into PNG(`byte[]`)

```cs
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using Koyashiro.PngChunkUtil;

var chunks = PngReader.ReadBytes(File.ReadAllBytes("example.png")).ToList();
chunks.Add(Chunk.Create("ABCD", new byte[4] { 0x00, 0x01, 0x02, 0x03 }));
var output = PngWriter.WriteBytes(CollectionsMarshal.AsSpan(chunks));
```

## Example

[ChunkReader](https://github.com/koyashiro/PngChunkUtil/tree/main/examples/ChunkReader)
