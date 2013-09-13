TiledSharp {#mainpage}
==========

About TiledSharp
----------------

TiledSharp is a .NET C# library for importing [TMX][] tilemaps and TSX tilesets
generated by [Tiled][], a tile map generation tool. The data is saved as a
[TmxMap][] object, whose structure closely resembles the original TMX file.

As a generic TMX and TSX parser, TiledSharp does not render the maps or depend
on any external framework beyond .NET, such as Windows, XNA/MonoGame, Unity, or
PSM. However, it can be used as an interface between TMX data and external
games.


Usage
-----

To import a TMX file into your C# application:

- Include a reference to TiledSharp. You can either incorporare it directory
  into your own project, or you can pre-compile it and include the DLL.

- Import the TiledSharp namespace (optional):

  ~~~~ {.cs}
  using TiledSharp;
  ~~~~

- Create a Map object using the constructor:

  ~~~~ {.cs}
  var map = new TmxMap("someMap.tmx");
  ~~~~

  TiledSharp supports both resource names and file paths, and should work as
  expected in most situations. For more details, please consult the wiki.

- Access the TMX data using the Map fields. Principal classes can be accessed
  by either name or index:

  ~~~~ {.cs}
     var map = new TmxMap("someMap.tmx");
     var version = map.Version;
     var myTileset = map.Tilesets["myTileset"];
     var myLayer = map.Layers[2];
     var hiddenChest = map.ObjectGroups["Chests"].Objects["hiddenChest"];
  ~~~~

Map data fields correspond closely to the TMX file structure. For a complete
listing, see the [TiledSharp Data Hierarchy][].

Although TiledSharp can manage elements with the same name, it is not
recommended. For more information, see the [TmxList][] specification.


Notes
-----

TiledSharp parses XML files produced by [Tiled][], an open-source (GPL) tile map
editor developed and maintained by Thorbjørn Lindeijer.

Zlib decompression in TiledSharp uses the Zlib implementation of [DotNetZip][]
v1.9.1.8.


Licensing
---------

[TiledSharp][] is distributed under the [Apache 2.0 License][].

Support code from [DotNetZip][] is distributed under the [Microsoft Public
License][].


Contact
-------

Marshall Ward <tiledsharp@marshallward.org>

[TMX]: https://github.com/bjorn/tiled/wiki/TMX-Map-Format
[Tiled]: http://mapeditor.org
[Tesserae]: https://github.com/marshallward/Tesserae
[TmxMap]: https://github.com/marshallward/TiledSharp/wiki/TmxMap
[TiledSharp]: https://github.com/marshallward/TiledSharp
[TiledSharp Data Hierarchy]: https://github.com/marshallward/TiledSharp/wiki/TiledSharp-Data-Hierarchy
[TmxList]: https://github.com/marshallward/TiledSharp/wiki/TmxList
[DotNetZip]: http://dotnetzip.codeplex.com
[Apache 2.0 License]: http://www.apache.org/licenses/LICENSE-2.0.txt
[Microsoft Public License]: http://www.microsoft.com/en-us/openness/licenses.aspx#MPL