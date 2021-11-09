# PngChunkUtil

[<img alt="Build and Test" src="https://github.com/koyashiro/PngChunkUtil/workflows/Build%20and%20Test/badge.svg">](https://github.com/koyashiro/PngChunkUtil/actions?query=workflow%3Abuild-and-test)

[PngChunkUtil](https://www.nuget.org/packages/PngChunkUtil/)

## Convert PNG(`byte[]`) into chunks(`Chunk[]`)

```cs
using System;
using System.IO;
using Koyashiro.PngChunkUtil;

byte[] png = File.ReadAllBytes("example.png");
Chunk[] chunks = PngReader.ReadBytes(png);
```

## Convert chunks(`Chunk[]`) into PNG(`byte[]`)

```cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

List<Chunk> chunks = PngReader.ReadBytes(File.ReadAllBytes("example.png")).ToList();
byte[] png = PngWriter.WriteBytes(CollectionsMarshal.AsSpan(chunks));
```

## Example

[ChunkReader](https://github.com/koyashiro/PngChunkUtil/tree/main/examples/ChunkReader)
