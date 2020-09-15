return {
  version = "1.4",
  luaversion = "5.1",
  tiledversion = "1.4.2",
  orientation = "orthogonal",
  renderorder = "right-down",
  width = 32,
  height = 32,
  tilewidth = 16,
  tileheight = 16,
  nextlayerid = 3,
  nextobjectid = 211,
  properties = {
    ["IsOutdoors"] = false,
    ["Name"] = "",
    ["TimeScale"] = ""
  },
  tilesets = {
    {
      name = "Terrain",
      firstgid = 1,
      filename = "GameMaps/Tilesets/Terrain.tsx",
      tilewidth = 16,
      tileheight = 16,
      spacing = 0,
      margin = 0,
      columns = 16,
      image = "GameMaps/Tilesets/Terrain.png",
      imagewidth = 256,
      imageheight = 256,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 16,
        height = 16
      },
      properties = {},
      terrains = {
        {
          name = "Dirt",
          tile = 13,
          properties = {}
        },
        {
          name = "Grass",
          tile = 0,
          properties = {}
        },
        {
          name = "Mid Grass",
          tile = 4,
          properties = {}
        },
        {
          name = "Dark Grass",
          tile = 8,
          properties = {}
        }
      },
      tilecount = 256,
      tiles = {
        {
          id = 0,
          terrain = { 1, 1, 1, 1 }
        },
        {
          id = 4,
          terrain = { 2, 2, 2, 2 }
        },
        {
          id = 8,
          terrain = { 3, 3, 3, 3 }
        },
        {
          id = 13,
          terrain = { 0, 0, 0, 0 }
        },
        {
          id = 16,
          terrain = { -1, -1, -1, 1 }
        },
        {
          id = 17,
          terrain = { -1, -1, 1, 1 }
        },
        {
          id = 18,
          terrain = { -1, -1, 1, 1 }
        },
        {
          id = 19,
          terrain = { -1, -1, 1, -1 }
        },
        {
          id = 20,
          terrain = { -1, -1, -1, 2 }
        },
        {
          id = 21,
          terrain = { -1, -1, 2, 2 }
        },
        {
          id = 22,
          terrain = { -1, -1, 2, 2 }
        },
        {
          id = 23,
          terrain = { -1, -1, 2, -1 }
        },
        {
          id = 24,
          terrain = { -1, -1, -1, 3 }
        },
        {
          id = 25,
          terrain = { -1, -1, 3, 3 }
        },
        {
          id = 26,
          terrain = { -1, -1, 3, 3 }
        },
        {
          id = 27,
          terrain = { -1, -1, 3, -1 }
        },
        {
          id = 28,
          terrain = { -1, -1, -1, 0 }
        },
        {
          id = 29,
          terrain = { -1, -1, 0, 0 }
        },
        {
          id = 30,
          terrain = { -1, -1, 0, 0 }
        },
        {
          id = 31,
          terrain = { -1, -1, 0, -1 }
        },
        {
          id = 32,
          terrain = { -1, 1, -1, 1 }
        },
        {
          id = 33,
          terrain = { -1, 1, 1, 1 }
        },
        {
          id = 34,
          terrain = { 1, -1, 1, 1 }
        },
        {
          id = 35,
          terrain = { 1, -1, 1, -1 }
        },
        {
          id = 36,
          terrain = { -1, 2, -1, 2 }
        },
        {
          id = 37,
          terrain = { -1, 2, 2, 2 }
        },
        {
          id = 38,
          terrain = { 2, -1, 2, 2 }
        },
        {
          id = 39,
          terrain = { 2, -1, 2, -1 }
        },
        {
          id = 40,
          terrain = { -1, 3, -1, 3 }
        },
        {
          id = 41,
          terrain = { -1, 3, 3, 3 }
        },
        {
          id = 42,
          terrain = { 3, -1, 3, 3 }
        },
        {
          id = 43,
          terrain = { 3, -1, 3, -1 }
        },
        {
          id = 44,
          terrain = { -1, 0, -1, 0 }
        },
        {
          id = 45,
          terrain = { -1, 0, 0, 0 }
        },
        {
          id = 46,
          terrain = { 0, -1, 0, 0 }
        },
        {
          id = 47,
          terrain = { 0, -1, 0, -1 }
        },
        {
          id = 48,
          terrain = { -1, 1, -1, 1 }
        },
        {
          id = 49,
          terrain = { 1, 1, -1, 1 }
        },
        {
          id = 50,
          terrain = { 1, 1, 1, -1 }
        },
        {
          id = 51,
          terrain = { 1, -1, 1, -1 }
        },
        {
          id = 52,
          terrain = { -1, 2, -1, 2 }
        },
        {
          id = 53,
          terrain = { 2, 2, -1, 2 }
        },
        {
          id = 54,
          terrain = { 2, 2, 2, -1 }
        },
        {
          id = 55,
          terrain = { 2, -1, 2, -1 }
        },
        {
          id = 56,
          terrain = { -1, 3, -1, 3 }
        },
        {
          id = 57,
          terrain = { 3, 3, -1, 3 }
        },
        {
          id = 58,
          terrain = { 3, 3, 3, -1 }
        },
        {
          id = 59,
          terrain = { 3, -1, 3, -1 }
        },
        {
          id = 60,
          terrain = { -1, 0, -1, 0 }
        },
        {
          id = 61,
          terrain = { 0, 0, -1, 0 }
        },
        {
          id = 62,
          terrain = { 0, 0, 0, -1 }
        },
        {
          id = 63,
          terrain = { 0, -1, 0, -1 }
        },
        {
          id = 64,
          terrain = { -1, 1, -1, -1 }
        },
        {
          id = 65,
          terrain = { 1, 1, -1, -1 }
        },
        {
          id = 66,
          terrain = { 1, 1, -1, -1 }
        },
        {
          id = 67,
          terrain = { 1, -1, -1, -1 }
        },
        {
          id = 68,
          terrain = { -1, 2, -1, -1 }
        },
        {
          id = 69,
          terrain = { 2, 2, -1, -1 }
        },
        {
          id = 70,
          terrain = { 2, 2, -1, -1 }
        },
        {
          id = 71,
          terrain = { 2, -1, -1, -1 }
        },
        {
          id = 72,
          terrain = { -1, 3, -1, -1 }
        },
        {
          id = 73,
          terrain = { 3, 3, -1, -1 }
        },
        {
          id = 74,
          terrain = { 3, 3, -1, -1 }
        },
        {
          id = 75,
          terrain = { 3, -1, -1, -1 }
        },
        {
          id = 76,
          terrain = { -1, 0, -1, -1 }
        },
        {
          id = 77,
          terrain = { 0, 0, -1, -1 }
        },
        {
          id = 78,
          terrain = { 0, 0, -1, -1 }
        },
        {
          id = 79,
          terrain = { 0, -1, -1, -1 }
        }
      }
    },
    {
      name = "Grassland-16x",
      firstgid = 257,
      filename = "GameMaps/Tilesets/Grassland-16x.tsx",
      tilewidth = 16,
      tileheight = 16,
      spacing = 0,
      margin = 0,
      columns = 16,
      image = "GameMaps/Tilesets/Grassland-16x.png",
      imagewidth = 256,
      imageheight = 256,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 16,
        height = 16
      },
      properties = {},
      terrains = {},
      tilecount = 256,
      tiles = {}
    },
    {
      name = "Grassland-32x",
      firstgid = 513,
      filename = "GameMaps/Tilesets/Grassland-32x.tsx",
      tilewidth = 32,
      tileheight = 32,
      spacing = 0,
      margin = 0,
      columns = 8,
      image = "GameMaps/Tilesets/Grassland-32x.png",
      imagewidth = 256,
      imageheight = 256,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 32,
        height = 32
      },
      properties = {},
      terrains = {},
      tilecount = 64,
      tiles = {}
    },
    {
      name = "Grassland-64x",
      firstgid = 577,
      filename = "GameMaps/Tilesets/Grassland-64x.tsx",
      tilewidth = 64,
      tileheight = 64,
      spacing = 0,
      margin = 0,
      columns = 4,
      image = "GameMaps/Tilesets/Grassland-64x.png",
      imagewidth = 256,
      imageheight = 256,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 64,
        height = 64
      },
      properties = {},
      terrains = {},
      tilecount = 16,
      tiles = {}
    },
    {
      name = "Trees-128x",
      firstgid = 593,
      filename = "GameMaps/Tilesets/Trees-128x.tsx",
      tilewidth = 128,
      tileheight = 128,
      spacing = 0,
      margin = 0,
      columns = 4,
      image = "GameMaps/Tilesets/Trees-128x.png",
      imagewidth = 512,
      imageheight = 512,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 128,
        height = 128
      },
      properties = {},
      terrains = {},
      tilecount = 16,
      tiles = {}
    },
    {
      name = "Cliffs-64x",
      firstgid = 609,
      filename = "GameMaps/Tilesets/Cliffs-64x.tsx",
      tilewidth = 64,
      tileheight = 64,
      spacing = 0,
      margin = 0,
      columns = 4,
      image = "GameMaps/Tilesets/Cliffs-64x.png",
      imagewidth = 256,
      imageheight = 256,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 64,
        height = 64
      },
      properties = {},
      terrains = {},
      tilecount = 16,
      tiles = {}
    },
    {
      name = "WindowFrames-32x",
      firstgid = 625,
      tilewidth = 32,
      tileheight = 32,
      spacing = 0,
      margin = 0,
      columns = 4,
      image = "GameMaps/Tilesets/WindowFrames-32x.png",
      imagewidth = 128,
      imageheight = 128,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 32,
        height = 32
      },
      properties = {},
      terrains = {},
      tilecount = 16,
      tiles = {}
    },
    {
      name = "Towers-128_512x",
      firstgid = 641,
      filename = "GameMaps/Tilesets/Towers-128_512x.tsx",
      tilewidth = 128,
      tileheight = 512,
      spacing = 0,
      margin = 0,
      columns = 4,
      image = "GameMaps/Tilesets/Towers-128_512x.png",
      imagewidth = 512,
      imageheight = 512,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 128,
        height = 512
      },
      properties = {},
      terrains = {},
      tilecount = 4,
      tiles = {}
    },
    {
      name = "Towers-128x",
      firstgid = 645,
      filename = "GameMaps/Tilesets/Towers-128x.tsx",
      tilewidth = 128,
      tileheight = 128,
      spacing = 0,
      margin = 0,
      columns = 4,
      image = "GameMaps/Tilesets/Towers-128x.png",
      imagewidth = 512,
      imageheight = 512,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 128,
        height = 128
      },
      properties = {},
      terrains = {},
      tilecount = 16,
      tiles = {}
    },
    {
      name = "Flags-32_96x",
      firstgid = 661,
      filename = "GameMaps/Tilesets/Flags-32_96x.tsx",
      tilewidth = 32,
      tileheight = 96,
      spacing = 0,
      margin = 0,
      columns = 16,
      image = "GameMaps/Tilesets/Flags-32_96x.png",
      imagewidth = 512,
      imageheight = 512,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 32,
        height = 96
      },
      properties = {},
      terrains = {},
      tilecount = 80,
      tiles = {
        {
          id = 0,
          animation = {
            {
              tileid = 0,
              duration = 100
            },
            {
              tileid = 1,
              duration = 100
            },
            {
              tileid = 2,
              duration = 100
            },
            {
              tileid = 3,
              duration = 100
            },
            {
              tileid = 4,
              duration = 100
            },
            {
              tileid = 5,
              duration = 100
            },
            {
              tileid = 6,
              duration = 100
            },
            {
              tileid = 7,
              duration = 100
            },
            {
              tileid = 8,
              duration = 100
            },
            {
              tileid = 9,
              duration = 100
            }
          }
        },
        {
          id = 16,
          animation = {
            {
              tileid = 16,
              duration = 100
            },
            {
              tileid = 17,
              duration = 100
            },
            {
              tileid = 18,
              duration = 100
            },
            {
              tileid = 19,
              duration = 100
            },
            {
              tileid = 20,
              duration = 100
            },
            {
              tileid = 21,
              duration = 100
            },
            {
              tileid = 22,
              duration = 100
            },
            {
              tileid = 23,
              duration = 100
            },
            {
              tileid = 24,
              duration = 100
            },
            {
              tileid = 25,
              duration = 100
            }
          }
        },
        {
          id = 32,
          animation = {
            {
              tileid = 32,
              duration = 100
            },
            {
              tileid = 33,
              duration = 100
            },
            {
              tileid = 34,
              duration = 100
            },
            {
              tileid = 35,
              duration = 100
            },
            {
              tileid = 36,
              duration = 100
            },
            {
              tileid = 37,
              duration = 100
            },
            {
              tileid = 38,
              duration = 100
            },
            {
              tileid = 39,
              duration = 100
            },
            {
              tileid = 40,
              duration = 100
            },
            {
              tileid = 41,
              duration = 100
            }
          }
        }
      }
    },
    {
      name = "Lanterns-32_96x",
      firstgid = 741,
      filename = "GameMaps/Tilesets/Lanterns-32_96x.tsx",
      tilewidth = 32,
      tileheight = 96,
      spacing = 0,
      margin = 0,
      columns = 16,
      image = "GameMaps/Tilesets/Lanterns-32_96x.png",
      imagewidth = 512,
      imageheight = 512,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 32,
        height = 96
      },
      properties = {},
      terrains = {},
      tilecount = 80,
      tiles = {
        {
          id = 0,
          animation = {
            {
              tileid = 0,
              duration = 150
            },
            {
              tileid = 1,
              duration = 150
            },
            {
              tileid = 2,
              duration = 150
            },
            {
              tileid = 3,
              duration = 150
            },
            {
              tileid = 4,
              duration = 150
            },
            {
              tileid = 5,
              duration = 150
            },
            {
              tileid = 6,
              duration = 150
            },
            {
              tileid = 7,
              duration = 150
            }
          }
        },
        {
          id = 8,
          animation = {
            {
              tileid = 8,
              duration = 150
            },
            {
              tileid = 9,
              duration = 150
            },
            {
              tileid = 10,
              duration = 150
            },
            {
              tileid = 11,
              duration = 150
            },
            {
              tileid = 12,
              duration = 150
            },
            {
              tileid = 13,
              duration = 150
            },
            {
              tileid = 14,
              duration = 150
            },
            {
              tileid = 15,
              duration = 150
            }
          }
        },
        {
          id = 16,
          animation = {
            {
              tileid = 16,
              duration = 150
            },
            {
              tileid = 17,
              duration = 150
            },
            {
              tileid = 18,
              duration = 150
            },
            {
              tileid = 19,
              duration = 150
            },
            {
              tileid = 20,
              duration = 150
            },
            {
              tileid = 21,
              duration = 150
            },
            {
              tileid = 22,
              duration = 150
            },
            {
              tileid = 23,
              duration = 150
            }
          }
        },
        {
          id = 24,
          animation = {
            {
              tileid = 24,
              duration = 150
            },
            {
              tileid = 25,
              duration = 150
            },
            {
              tileid = 26,
              duration = 150
            },
            {
              tileid = 27,
              duration = 150
            },
            {
              tileid = 28,
              duration = 150
            },
            {
              tileid = 29,
              duration = 150
            },
            {
              tileid = 30,
              duration = 150
            },
            {
              tileid = 31,
              duration = 150
            }
          }
        },
        {
          id = 32,
          animation = {
            {
              tileid = 32,
              duration = 150
            },
            {
              tileid = 33,
              duration = 150
            },
            {
              tileid = 34,
              duration = 150
            },
            {
              tileid = 35,
              duration = 150
            },
            {
              tileid = 36,
              duration = 150
            },
            {
              tileid = 37,
              duration = 150
            },
            {
              tileid = 38,
              duration = 150
            },
            {
              tileid = 39,
              duration = 150
            }
          }
        },
        {
          id = 40,
          animation = {
            {
              tileid = 40,
              duration = 150
            },
            {
              tileid = 41,
              duration = 150
            },
            {
              tileid = 42,
              duration = 150
            },
            {
              tileid = 43,
              duration = 150
            },
            {
              tileid = 44,
              duration = 150
            },
            {
              tileid = 45,
              duration = 150
            },
            {
              tileid = 46,
              duration = 150
            },
            {
              tileid = 47,
              duration = 150
            }
          }
        }
      }
    },
    {
      name = "Vegetation-32x",
      firstgid = 821,
      tilewidth = 32,
      tileheight = 32,
      spacing = 0,
      margin = 0,
      columns = 32,
      image = "GameMaps/Tilesets/Vegetation-32x.png",
      imagewidth = 1024,
      imageheight = 512,
      objectalignment = "unspecified",
      tileoffset = {
        x = 0,
        y = 0
      },
      grid = {
        orientation = "orthogonal",
        width = 32,
        height = 32
      },
      properties = {},
      terrains = {},
      tilecount = 512,
      tiles = {
        {
          id = 0,
          animation = {
            {
              tileid = 0,
              duration = 150
            },
            {
              tileid = 1,
              duration = 150
            },
            {
              tileid = 2,
              duration = 150
            },
            {
              tileid = 3,
              duration = 150
            },
            {
              tileid = 32,
              duration = 150
            },
            {
              tileid = 33,
              duration = 150
            },
            {
              tileid = 34,
              duration = 150
            },
            {
              tileid = 35,
              duration = 150
            },
            {
              tileid = 64,
              duration = 150
            },
            {
              tileid = 65,
              duration = 150
            }
          }
        },
        {
          id = 4,
          animation = {
            {
              tileid = 4,
              duration = 150
            },
            {
              tileid = 5,
              duration = 150
            },
            {
              tileid = 6,
              duration = 150
            },
            {
              tileid = 7,
              duration = 150
            },
            {
              tileid = 36,
              duration = 150
            },
            {
              tileid = 37,
              duration = 150
            },
            {
              tileid = 38,
              duration = 150
            },
            {
              tileid = 39,
              duration = 150
            },
            {
              tileid = 68,
              duration = 150
            },
            {
              tileid = 69,
              duration = 150
            }
          }
        },
        {
          id = 8,
          animation = {
            {
              tileid = 8,
              duration = 150
            },
            {
              tileid = 9,
              duration = 150
            },
            {
              tileid = 10,
              duration = 150
            },
            {
              tileid = 11,
              duration = 150
            },
            {
              tileid = 40,
              duration = 150
            },
            {
              tileid = 41,
              duration = 150
            },
            {
              tileid = 42,
              duration = 150
            },
            {
              tileid = 43,
              duration = 150
            },
            {
              tileid = 72,
              duration = 150
            },
            {
              tileid = 73,
              duration = 150
            }
          }
        },
        {
          id = 12,
          animation = {
            {
              tileid = 12,
              duration = 150
            },
            {
              tileid = 13,
              duration = 150
            },
            {
              tileid = 14,
              duration = 150
            },
            {
              tileid = 15,
              duration = 150
            },
            {
              tileid = 44,
              duration = 150
            },
            {
              tileid = 45,
              duration = 150
            },
            {
              tileid = 46,
              duration = 150
            },
            {
              tileid = 47,
              duration = 150
            },
            {
              tileid = 76,
              duration = 150
            },
            {
              tileid = 77,
              duration = 150
            },
            {
              tileid = 78,
              duration = 150
            },
            {
              tileid = 79,
              duration = 150
            }
          }
        },
        {
          id = 16,
          animation = {
            {
              tileid = 16,
              duration = 150
            },
            {
              tileid = 17,
              duration = 150
            },
            {
              tileid = 18,
              duration = 150
            },
            {
              tileid = 19,
              duration = 150
            },
            {
              tileid = 48,
              duration = 150
            },
            {
              tileid = 49,
              duration = 150
            },
            {
              tileid = 50,
              duration = 150
            },
            {
              tileid = 51,
              duration = 150
            },
            {
              tileid = 80,
              duration = 150
            },
            {
              tileid = 81,
              duration = 150
            }
          }
        },
        {
          id = 20,
          animation = {
            {
              tileid = 20,
              duration = 150
            },
            {
              tileid = 21,
              duration = 150
            },
            {
              tileid = 22,
              duration = 150
            },
            {
              tileid = 23,
              duration = 150
            },
            {
              tileid = 52,
              duration = 150
            },
            {
              tileid = 53,
              duration = 150
            },
            {
              tileid = 54,
              duration = 150
            },
            {
              tileid = 55,
              duration = 150
            },
            {
              tileid = 84,
              duration = 150
            },
            {
              tileid = 85,
              duration = 150
            }
          }
        },
        {
          id = 24,
          animation = {
            {
              tileid = 24,
              duration = 150
            },
            {
              tileid = 25,
              duration = 150
            },
            {
              tileid = 26,
              duration = 150
            },
            {
              tileid = 27,
              duration = 150
            },
            {
              tileid = 56,
              duration = 150
            },
            {
              tileid = 57,
              duration = 150
            },
            {
              tileid = 58,
              duration = 150
            },
            {
              tileid = 59,
              duration = 150
            }
          }
        },
        {
          id = 96,
          animation = {
            {
              tileid = 96,
              duration = 150
            },
            {
              tileid = 97,
              duration = 150
            },
            {
              tileid = 98,
              duration = 150
            },
            {
              tileid = 99,
              duration = 150
            },
            {
              tileid = 128,
              duration = 150
            },
            {
              tileid = 129,
              duration = 150
            },
            {
              tileid = 130,
              duration = 150
            },
            {
              tileid = 131,
              duration = 150
            },
            {
              tileid = 160,
              duration = 150
            },
            {
              tileid = 161,
              duration = 150
            }
          }
        },
        {
          id = 100,
          animation = {
            {
              tileid = 100,
              duration = 150
            },
            {
              tileid = 101,
              duration = 150
            },
            {
              tileid = 102,
              duration = 150
            },
            {
              tileid = 103,
              duration = 150
            },
            {
              tileid = 132,
              duration = 150
            },
            {
              tileid = 133,
              duration = 150
            },
            {
              tileid = 134,
              duration = 150
            },
            {
              tileid = 135,
              duration = 150
            },
            {
              tileid = 164,
              duration = 150
            },
            {
              tileid = 165,
              duration = 150
            }
          }
        },
        {
          id = 104,
          animation = {
            {
              tileid = 104,
              duration = 150
            },
            {
              tileid = 105,
              duration = 150
            },
            {
              tileid = 106,
              duration = 150
            },
            {
              tileid = 107,
              duration = 150
            },
            {
              tileid = 136,
              duration = 150
            },
            {
              tileid = 137,
              duration = 150
            },
            {
              tileid = 138,
              duration = 150
            },
            {
              tileid = 139,
              duration = 150
            },
            {
              tileid = 168,
              duration = 150
            },
            {
              tileid = 169,
              duration = 150
            }
          }
        },
        {
          id = 108,
          animation = {
            {
              tileid = 108,
              duration = 150
            },
            {
              tileid = 109,
              duration = 150
            },
            {
              tileid = 110,
              duration = 150
            },
            {
              tileid = 111,
              duration = 150
            },
            {
              tileid = 140,
              duration = 150
            },
            {
              tileid = 141,
              duration = 150
            },
            {
              tileid = 142,
              duration = 150
            },
            {
              tileid = 143,
              duration = 150
            },
            {
              tileid = 172,
              duration = 150
            },
            {
              tileid = 173,
              duration = 150
            },
            {
              tileid = 174,
              duration = 150
            },
            {
              tileid = 175,
              duration = 150
            }
          }
        },
        {
          id = 112,
          animation = {
            {
              tileid = 112,
              duration = 150
            },
            {
              tileid = 113,
              duration = 150
            },
            {
              tileid = 114,
              duration = 150
            },
            {
              tileid = 115,
              duration = 150
            },
            {
              tileid = 144,
              duration = 150
            },
            {
              tileid = 145,
              duration = 150
            },
            {
              tileid = 146,
              duration = 150
            },
            {
              tileid = 147,
              duration = 150
            },
            {
              tileid = 176,
              duration = 150
            },
            {
              tileid = 177,
              duration = 150
            }
          }
        },
        {
          id = 116,
          animation = {
            {
              tileid = 116,
              duration = 150
            },
            {
              tileid = 117,
              duration = 150
            },
            {
              tileid = 118,
              duration = 150
            },
            {
              tileid = 119,
              duration = 150
            },
            {
              tileid = 148,
              duration = 150
            },
            {
              tileid = 149,
              duration = 150
            },
            {
              tileid = 150,
              duration = 150
            },
            {
              tileid = 151,
              duration = 150
            },
            {
              tileid = 180,
              duration = 150
            },
            {
              tileid = 181,
              duration = 150
            }
          }
        },
        {
          id = 120,
          animation = {
            {
              tileid = 120,
              duration = 150
            },
            {
              tileid = 121,
              duration = 150
            },
            {
              tileid = 122,
              duration = 150
            },
            {
              tileid = 123,
              duration = 150
            },
            {
              tileid = 152,
              duration = 150
            },
            {
              tileid = 153,
              duration = 150
            },
            {
              tileid = 154,
              duration = 150
            },
            {
              tileid = 155,
              duration = 150
            }
          }
        },
        {
          id = 192,
          animation = {
            {
              tileid = 192,
              duration = 150
            },
            {
              tileid = 193,
              duration = 150
            },
            {
              tileid = 194,
              duration = 150
            },
            {
              tileid = 195,
              duration = 150
            },
            {
              tileid = 224,
              duration = 150
            },
            {
              tileid = 225,
              duration = 150
            },
            {
              tileid = 226,
              duration = 150
            },
            {
              tileid = 227,
              duration = 150
            },
            {
              tileid = 256,
              duration = 150
            },
            {
              tileid = 257,
              duration = 150
            }
          }
        },
        {
          id = 196,
          animation = {
            {
              tileid = 196,
              duration = 150
            },
            {
              tileid = 197,
              duration = 150
            },
            {
              tileid = 198,
              duration = 150
            },
            {
              tileid = 199,
              duration = 150
            },
            {
              tileid = 228,
              duration = 150
            },
            {
              tileid = 229,
              duration = 150
            },
            {
              tileid = 230,
              duration = 150
            },
            {
              tileid = 231,
              duration = 150
            },
            {
              tileid = 260,
              duration = 150
            },
            {
              tileid = 261,
              duration = 150
            }
          }
        },
        {
          id = 200,
          animation = {
            {
              tileid = 200,
              duration = 150
            },
            {
              tileid = 201,
              duration = 150
            },
            {
              tileid = 202,
              duration = 150
            },
            {
              tileid = 203,
              duration = 150
            },
            {
              tileid = 232,
              duration = 150
            },
            {
              tileid = 233,
              duration = 150
            },
            {
              tileid = 234,
              duration = 150
            },
            {
              tileid = 235,
              duration = 150
            },
            {
              tileid = 264,
              duration = 150
            }
          }
        },
        {
          id = 204,
          animation = {
            {
              tileid = 204,
              duration = 150
            },
            {
              tileid = 205,
              duration = 150
            },
            {
              tileid = 206,
              duration = 150
            },
            {
              tileid = 207,
              duration = 150
            },
            {
              tileid = 236,
              duration = 150
            },
            {
              tileid = 237,
              duration = 150
            },
            {
              tileid = 238,
              duration = 150
            },
            {
              tileid = 239,
              duration = 150
            },
            {
              tileid = 268,
              duration = 150
            },
            {
              tileid = 269,
              duration = 150
            },
            {
              tileid = 270,
              duration = 150
            },
            {
              tileid = 271,
              duration = 150
            }
          }
        },
        {
          id = 208,
          animation = {
            {
              tileid = 208,
              duration = 150
            },
            {
              tileid = 209,
              duration = 150
            },
            {
              tileid = 210,
              duration = 150
            },
            {
              tileid = 211,
              duration = 150
            },
            {
              tileid = 240,
              duration = 150
            },
            {
              tileid = 241,
              duration = 150
            },
            {
              tileid = 242,
              duration = 150
            },
            {
              tileid = 243,
              duration = 150
            },
            {
              tileid = 272,
              duration = 150
            },
            {
              tileid = 273,
              duration = 150
            }
          }
        },
        {
          id = 212,
          animation = {
            {
              tileid = 212,
              duration = 150
            },
            {
              tileid = 213,
              duration = 150
            },
            {
              tileid = 214,
              duration = 150
            },
            {
              tileid = 215,
              duration = 150
            },
            {
              tileid = 244,
              duration = 150
            },
            {
              tileid = 245,
              duration = 150
            },
            {
              tileid = 246,
              duration = 150
            },
            {
              tileid = 247,
              duration = 150
            },
            {
              tileid = 276,
              duration = 150
            },
            {
              tileid = 277,
              duration = 150
            }
          }
        },
        {
          id = 216,
          animation = {
            {
              tileid = 216,
              duration = 150
            },
            {
              tileid = 217,
              duration = 150
            },
            {
              tileid = 218,
              duration = 150
            },
            {
              tileid = 219,
              duration = 150
            },
            {
              tileid = 248,
              duration = 150
            },
            {
              tileid = 249,
              duration = 150
            },
            {
              tileid = 250,
              duration = 150
            },
            {
              tileid = 251,
              duration = 150
            }
          }
        },
        {
          id = 288,
          animation = {
            {
              tileid = 288,
              duration = 150
            },
            {
              tileid = 289,
              duration = 150
            },
            {
              tileid = 290,
              duration = 150
            },
            {
              tileid = 291,
              duration = 150
            },
            {
              tileid = 320,
              duration = 150
            },
            {
              tileid = 321,
              duration = 150
            },
            {
              tileid = 322,
              duration = 150
            },
            {
              tileid = 323,
              duration = 150
            },
            {
              tileid = 352,
              duration = 150
            },
            {
              tileid = 353,
              duration = 150
            }
          }
        },
        {
          id = 292,
          animation = {
            {
              tileid = 292,
              duration = 150
            },
            {
              tileid = 293,
              duration = 150
            },
            {
              tileid = 294,
              duration = 150
            },
            {
              tileid = 295,
              duration = 150
            },
            {
              tileid = 324,
              duration = 150
            },
            {
              tileid = 325,
              duration = 150
            },
            {
              tileid = 326,
              duration = 150
            },
            {
              tileid = 327,
              duration = 150
            },
            {
              tileid = 356,
              duration = 150
            },
            {
              tileid = 357,
              duration = 150
            }
          }
        },
        {
          id = 296,
          animation = {
            {
              tileid = 296,
              duration = 150
            },
            {
              tileid = 297,
              duration = 150
            },
            {
              tileid = 298,
              duration = 150
            },
            {
              tileid = 299,
              duration = 150
            },
            {
              tileid = 328,
              duration = 150
            },
            {
              tileid = 329,
              duration = 150
            },
            {
              tileid = 330,
              duration = 150
            },
            {
              tileid = 331,
              duration = 150
            },
            {
              tileid = 360,
              duration = 150
            }
          }
        },
        {
          id = 300,
          animation = {
            {
              tileid = 300,
              duration = 150
            },
            {
              tileid = 301,
              duration = 150
            },
            {
              tileid = 302,
              duration = 150
            },
            {
              tileid = 303,
              duration = 150
            },
            {
              tileid = 332,
              duration = 150
            },
            {
              tileid = 333,
              duration = 150
            },
            {
              tileid = 334,
              duration = 150
            },
            {
              tileid = 335,
              duration = 150
            },
            {
              tileid = 364,
              duration = 150
            },
            {
              tileid = 365,
              duration = 150
            }
          }
        },
        {
          id = 304,
          animation = {
            {
              tileid = 304,
              duration = 150
            },
            {
              tileid = 305,
              duration = 150
            },
            {
              tileid = 306,
              duration = 150
            },
            {
              tileid = 307,
              duration = 150
            },
            {
              tileid = 336,
              duration = 150
            },
            {
              tileid = 337,
              duration = 150
            },
            {
              tileid = 338,
              duration = 150
            },
            {
              tileid = 339,
              duration = 150
            },
            {
              tileid = 368,
              duration = 150
            },
            {
              tileid = 369,
              duration = 150
            }
          }
        },
        {
          id = 308,
          animation = {
            {
              tileid = 308,
              duration = 150
            },
            {
              tileid = 309,
              duration = 150
            },
            {
              tileid = 310,
              duration = 150
            },
            {
              tileid = 311,
              duration = 150
            },
            {
              tileid = 340,
              duration = 150
            },
            {
              tileid = 341,
              duration = 150
            },
            {
              tileid = 342,
              duration = 150
            },
            {
              tileid = 343,
              duration = 150
            },
            {
              tileid = 372,
              duration = 150
            },
            {
              tileid = 373,
              duration = 150
            }
          }
        },
        {
          id = 384,
          animation = {
            {
              tileid = 384,
              duration = 150
            },
            {
              tileid = 385,
              duration = 150
            },
            {
              tileid = 386,
              duration = 150
            },
            {
              tileid = 387,
              duration = 150
            },
            {
              tileid = 416,
              duration = 150
            },
            {
              tileid = 417,
              duration = 150
            },
            {
              tileid = 418,
              duration = 150
            },
            {
              tileid = 419,
              duration = 150
            },
            {
              tileid = 448,
              duration = 150
            },
            {
              tileid = 449,
              duration = 150
            }
          }
        },
        {
          id = 388,
          animation = {
            {
              tileid = 388,
              duration = 150
            },
            {
              tileid = 389,
              duration = 150
            },
            {
              tileid = 390,
              duration = 150
            },
            {
              tileid = 391,
              duration = 150
            },
            {
              tileid = 420,
              duration = 150
            },
            {
              tileid = 421,
              duration = 150
            },
            {
              tileid = 422,
              duration = 150
            },
            {
              tileid = 423,
              duration = 150
            },
            {
              tileid = 452,
              duration = 150
            },
            {
              tileid = 453,
              duration = 150
            }
          }
        },
        {
          id = 392,
          animation = {
            {
              tileid = 392,
              duration = 150
            },
            {
              tileid = 393,
              duration = 150
            },
            {
              tileid = 394,
              duration = 150
            },
            {
              tileid = 395,
              duration = 150
            },
            {
              tileid = 424,
              duration = 150
            },
            {
              tileid = 425,
              duration = 150
            },
            {
              tileid = 426,
              duration = 150
            },
            {
              tileid = 427,
              duration = 150
            },
            {
              tileid = 456,
              duration = 150
            }
          }
        },
        {
          id = 396,
          animation = {
            {
              tileid = 396,
              duration = 150
            },
            {
              tileid = 397,
              duration = 150
            },
            {
              tileid = 398,
              duration = 150
            },
            {
              tileid = 399,
              duration = 150
            },
            {
              tileid = 428,
              duration = 150
            },
            {
              tileid = 429,
              duration = 150
            },
            {
              tileid = 430,
              duration = 150
            },
            {
              tileid = 431,
              duration = 150
            },
            {
              tileid = 460,
              duration = 150
            },
            {
              tileid = 461,
              duration = 150
            }
          }
        },
        {
          id = 400,
          animation = {
            {
              tileid = 400,
              duration = 150
            },
            {
              tileid = 401,
              duration = 150
            },
            {
              tileid = 402,
              duration = 150
            },
            {
              tileid = 403,
              duration = 150
            },
            {
              tileid = 432,
              duration = 150
            },
            {
              tileid = 433,
              duration = 150
            },
            {
              tileid = 434,
              duration = 150
            },
            {
              tileid = 435,
              duration = 150
            },
            {
              tileid = 464,
              duration = 150
            },
            {
              tileid = 465,
              duration = 150
            }
          }
        },
        {
          id = 404,
          animation = {
            {
              tileid = 404,
              duration = 150
            },
            {
              tileid = 405,
              duration = 150
            },
            {
              tileid = 406,
              duration = 150
            },
            {
              tileid = 407,
              duration = 150
            },
            {
              tileid = 436,
              duration = 150
            },
            {
              tileid = 437,
              duration = 150
            },
            {
              tileid = 438,
              duration = 150
            },
            {
              tileid = 439,
              duration = 150
            },
            {
              tileid = 468,
              duration = 150
            },
            {
              tileid = 469,
              duration = 150
            }
          }
        }
      }
    }
  },
  layers = {
    {
      type = "tilelayer",
      x = 0,
      y = 0,
      width = 32,
      height = 32,
      id = 1,
      name = "Tile Layer 1",
      visible = true,
      opacity = 1,
      offsetx = 0,
      offsety = 0,
      properties = {},
      encoding = "lua",
      data = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
      }
    },
    {
      type = "objectgroup",
      draworder = "topdown",
      id = 2,
      name = "Blocks",
      visible = true,
      opacity = 1,
      offsetx = 0,
      offsety = 0,
      properties = {},
      objects = {
        {
          id = 46,
          name = "",
          type = "",
          shape = "rectangle",
          x = 118.667,
          y = 206.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 329,
          visible = true,
          properties = {}
        },
        {
          id = 47,
          name = "",
          type = "",
          shape = "rectangle",
          x = 148,
          y = 205.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 330,
          visible = true,
          properties = {}
        },
        {
          id = 48,
          name = "",
          type = "",
          shape = "rectangle",
          x = 177.667,
          y = 208.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 331,
          visible = true,
          properties = {}
        },
        {
          id = 49,
          name = "",
          type = "",
          shape = "rectangle",
          x = 208.333,
          y = 210.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 332,
          visible = true,
          properties = {}
        },
        {
          id = 50,
          name = "",
          type = "",
          shape = "rectangle",
          x = 243.833,
          y = 62.167,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 333,
          visible = true,
          properties = {}
        },
        {
          id = 51,
          name = "",
          type = "",
          shape = "rectangle",
          x = 273.167,
          y = 62.833,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 334,
          visible = true,
          properties = {}
        },
        {
          id = 52,
          name = "",
          type = "",
          shape = "rectangle",
          x = 304.167,
          y = 64.167,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 335,
          visible = true,
          properties = {}
        },
        {
          id = 53,
          name = "",
          type = "",
          shape = "rectangle",
          x = 340.167,
          y = 64.5,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 336,
          visible = true,
          properties = {}
        },
        {
          id = 54,
          name = "",
          type = "",
          shape = "rectangle",
          x = 110.333,
          y = 238.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 345,
          visible = true,
          properties = {}
        },
        {
          id = 55,
          name = "",
          type = "",
          shape = "rectangle",
          x = 144.667,
          y = 236.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 346,
          visible = true,
          properties = {}
        },
        {
          id = 56,
          name = "",
          type = "",
          shape = "rectangle",
          x = 178.667,
          y = 244.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 347,
          visible = true,
          properties = {}
        },
        {
          id = 57,
          name = "",
          type = "",
          shape = "rectangle",
          x = 216,
          y = 247.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 348,
          visible = true,
          properties = {}
        },
        {
          id = 58,
          name = "",
          type = "",
          shape = "rectangle",
          x = 249.833,
          y = 98.5,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 349,
          visible = true,
          properties = {}
        },
        {
          id = 59,
          name = "",
          type = "",
          shape = "rectangle",
          x = 278.167,
          y = 95.833,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 350,
          visible = true,
          properties = {}
        },
        {
          id = 60,
          name = "",
          type = "",
          shape = "rectangle",
          x = 308.167,
          y = 96.833,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 351,
          visible = true,
          properties = {}
        },
        {
          id = 61,
          name = "",
          type = "",
          shape = "rectangle",
          x = 341.5,
          y = 98.167,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 352,
          visible = true,
          properties = {}
        },
        {
          id = 62,
          name = "",
          type = "",
          shape = "rectangle",
          x = 109.667,
          y = 269.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 361,
          visible = true,
          properties = {}
        },
        {
          id = 63,
          name = "",
          type = "",
          shape = "rectangle",
          x = 150,
          y = 272.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 362,
          visible = true,
          properties = {}
        },
        {
          id = 64,
          name = "",
          type = "",
          shape = "rectangle",
          x = 186.667,
          y = 275,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 363,
          visible = true,
          properties = {}
        },
        {
          id = 65,
          name = "",
          type = "",
          shape = "rectangle",
          x = 215.667,
          y = 274.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 364,
          visible = true,
          properties = {}
        },
        {
          id = 66,
          name = "",
          type = "",
          shape = "rectangle",
          x = 243.5,
          y = 127.833,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 365,
          visible = true,
          properties = {}
        },
        {
          id = 67,
          name = "",
          type = "",
          shape = "rectangle",
          x = 270.833,
          y = 125.167,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 366,
          visible = true,
          properties = {}
        },
        {
          id = 68,
          name = "",
          type = "",
          shape = "rectangle",
          x = 305.833,
          y = 125.5,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 367,
          visible = true,
          properties = {}
        },
        {
          id = 69,
          name = "",
          type = "",
          shape = "rectangle",
          x = 335.5,
          y = 124.5,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 368,
          visible = true,
          properties = {}
        },
        {
          id = 70,
          name = "",
          type = "",
          shape = "rectangle",
          x = 243.25,
          y = 145.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 377,
          visible = true,
          properties = {}
        },
        {
          id = 71,
          name = "",
          type = "",
          shape = "rectangle",
          x = 274.583,
          y = 161.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 378,
          visible = true,
          properties = {}
        },
        {
          id = 72,
          name = "",
          type = "",
          shape = "rectangle",
          x = 308.917,
          y = 153,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 379,
          visible = true,
          properties = {}
        },
        {
          id = 73,
          name = "",
          type = "",
          shape = "rectangle",
          x = 341.25,
          y = 157.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 380,
          visible = true,
          properties = {}
        },
        {
          id = 74,
          name = "",
          type = "",
          shape = "rectangle",
          x = 248.167,
          y = 178.583,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 381,
          visible = true,
          properties = {}
        },
        {
          id = 75,
          name = "",
          type = "",
          shape = "rectangle",
          x = 281.833,
          y = 180.583,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 382,
          visible = true,
          properties = {}
        },
        {
          id = 76,
          name = "",
          type = "",
          shape = "rectangle",
          x = 314.167,
          y = 175.583,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 383,
          visible = true,
          properties = {}
        },
        {
          id = 77,
          name = "",
          type = "",
          shape = "rectangle",
          x = 336.5,
          y = 180.25,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 384,
          visible = true,
          properties = {}
        },
        {
          id = 78,
          name = "",
          type = "",
          shape = "rectangle",
          x = 386.667,
          y = 214.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 385,
          visible = true,
          properties = {}
        },
        {
          id = 79,
          name = "",
          type = "",
          shape = "rectangle",
          x = 410.667,
          y = 210,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 386,
          visible = true,
          properties = {}
        },
        {
          id = 80,
          name = "",
          type = "",
          shape = "rectangle",
          x = 440,
          y = 212,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 387,
          visible = true,
          properties = {}
        },
        {
          id = 81,
          name = "",
          type = "",
          shape = "rectangle",
          x = 388,
          y = 237.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 401,
          visible = true,
          properties = {}
        },
        {
          id = 82,
          name = "",
          type = "",
          shape = "rectangle",
          x = 413.333,
          y = 239.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 402,
          visible = true,
          properties = {}
        },
        {
          id = 83,
          name = "",
          type = "",
          shape = "rectangle",
          x = 443.333,
          y = 237,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 403,
          visible = true,
          properties = {}
        },
        {
          id = 84,
          name = "",
          type = "",
          shape = "rectangle",
          x = 390.333,
          y = 269.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 449,
          visible = true,
          properties = {}
        },
        {
          id = 85,
          name = "",
          type = "",
          shape = "rectangle",
          x = 409.667,
          y = 267.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 450,
          visible = true,
          properties = {}
        },
        {
          id = 86,
          name = "",
          type = "",
          shape = "rectangle",
          x = 430,
          y = 267.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 465,
          visible = true,
          properties = {}
        },
        {
          id = 87,
          name = "",
          type = "",
          shape = "rectangle",
          x = 449.667,
          y = 266,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 466,
          visible = true,
          properties = {}
        },
        {
          id = 88,
          name = "",
          type = "",
          shape = "rectangle",
          x = 378.333,
          y = 294.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 481,
          visible = true,
          properties = {}
        },
        {
          id = 89,
          name = "",
          type = "",
          shape = "rectangle",
          x = 398,
          y = 293.667,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 482,
          visible = true,
          properties = {}
        },
        {
          id = 90,
          name = "",
          type = "",
          shape = "rectangle",
          x = 421.333,
          y = 294.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 497,
          visible = true,
          properties = {}
        },
        {
          id = 91,
          name = "",
          type = "",
          shape = "rectangle",
          x = 439,
          y = 293,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 498,
          visible = true,
          properties = {}
        },
        {
          id = 92,
          name = "",
          type = "",
          shape = "rectangle",
          x = 458.667,
          y = 295,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 483,
          visible = true,
          properties = {}
        },
        {
          id = 93,
          name = "",
          type = "",
          shape = "rectangle",
          x = 478.333,
          y = 291.333,
          width = 16,
          height = 16,
          rotation = 0,
          gid = 499,
          visible = true,
          properties = {}
        },
        {
          id = 100,
          name = "",
          type = "",
          shape = "rectangle",
          x = 149,
          y = 354,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 533,
          visible = true,
          properties = {}
        },
        {
          id = 101,
          name = "",
          type = "",
          shape = "rectangle",
          x = 179.667,
          y = 355.333,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 534,
          visible = true,
          properties = {}
        },
        {
          id = 102,
          name = "",
          type = "",
          shape = "rectangle",
          x = 121.667,
          y = 392,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 541,
          visible = true,
          properties = {}
        },
        {
          id = 103,
          name = "",
          type = "",
          shape = "rectangle",
          x = 165.667,
          y = 392.333,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 542,
          visible = true,
          properties = {}
        },
        {
          id = 104,
          name = "",
          type = "",
          shape = "rectangle",
          x = 244.667,
          y = 349.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 535,
          visible = true,
          properties = {}
        },
        {
          id = 105,
          name = "",
          type = "",
          shape = "rectangle",
          x = 278.667,
          y = 350.333,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 536,
          visible = true,
          properties = {}
        },
        {
          id = 106,
          name = "",
          type = "",
          shape = "rectangle",
          x = 241.333,
          y = 389,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 543,
          visible = true,
          properties = {}
        },
        {
          id = 107,
          name = "",
          type = "",
          shape = "rectangle",
          x = 272.667,
          y = 385,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 544,
          visible = true,
          properties = {}
        },
        {
          id = 108,
          name = "",
          type = "",
          shape = "rectangle",
          x = 366.333,
          y = 352.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 545,
          visible = true,
          properties = {}
        },
        {
          id = 109,
          name = "",
          type = "",
          shape = "rectangle",
          x = 440.333,
          y = 355.333,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 546,
          visible = true,
          properties = {}
        },
        {
          id = 110,
          name = "",
          type = "",
          shape = "rectangle",
          x = 388.333,
          y = 387.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 553,
          visible = true,
          properties = {}
        },
        {
          id = 111,
          name = "",
          type = "",
          shape = "rectangle",
          x = 445.667,
          y = 385.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 554,
          visible = true,
          properties = {}
        },
        {
          id = 112,
          name = "",
          type = "",
          shape = "rectangle",
          x = 403.333,
          y = 326.333,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 561,
          visible = true,
          properties = {}
        },
        {
          id = 113,
          name = "",
          type = "",
          shape = "rectangle",
          x = 436.333,
          y = 322.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 569,
          visible = true,
          properties = {}
        },
        {
          id = 114,
          name = "",
          type = "",
          shape = "rectangle",
          x = 464,
          y = 325,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 570,
          visible = true,
          properties = {}
        },
        {
          id = 115,
          name = "",
          type = "",
          shape = "rectangle",
          x = 490,
          y = 340,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 571,
          visible = true,
          properties = {}
        },
        {
          id = 116,
          name = "",
          type = "",
          shape = "rectangle",
          x = 302.333,
          y = 451.667,
          width = 64,
          height = 64,
          rotation = 0,
          gid = 585,
          visible = true,
          properties = {}
        },
        {
          id = 117,
          name = "",
          type = "",
          shape = "rectangle",
          x = 362.333,
          y = 454,
          width = 64,
          height = 64,
          rotation = 0,
          gid = 586,
          visible = true,
          properties = {}
        },
        {
          id = 118,
          name = "",
          type = "",
          shape = "rectangle",
          x = 421,
          y = 455.333,
          width = 64,
          height = 64,
          rotation = 0,
          gid = 589,
          visible = true,
          properties = {}
        },
        {
          id = 138,
          name = "",
          type = "",
          shape = "rectangle",
          x = 556.4,
          y = 74.5556,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 625,
          visible = true,
          properties = {}
        },
        {
          id = 139,
          name = "",
          type = "",
          shape = "rectangle",
          x = 597.333,
          y = 70.6667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 626,
          visible = true,
          properties = {}
        },
        {
          id = 140,
          name = "",
          type = "",
          shape = "rectangle",
          x = 640,
          y = 70,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 627,
          visible = true,
          properties = {}
        },
        {
          id = 142,
          name = "",
          type = "",
          shape = "rectangle",
          x = 542.656,
          y = 127.1,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 629,
          visible = true,
          properties = {}
        },
        {
          id = 143,
          name = "",
          type = "",
          shape = "rectangle",
          x = 596.667,
          y = 128.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 630,
          visible = true,
          properties = {}
        },
        {
          id = 144,
          name = "",
          type = "",
          shape = "rectangle",
          x = 639.333,
          y = 131.333,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 631,
          visible = true,
          properties = {}
        },
        {
          id = 145,
          name = "",
          type = "",
          shape = "rectangle",
          x = 678.667,
          y = 128.667,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 632,
          visible = true,
          properties = {}
        },
        {
          id = 146,
          name = "",
          type = "",
          shape = "rectangle",
          x = 557.333,
          y = 172,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 633,
          visible = true,
          properties = {}
        },
        {
          id = 148,
          name = "",
          type = "",
          shape = "rectangle",
          x = 738.667,
          y = 417.333,
          width = 128,
          height = 512,
          rotation = 0,
          gid = 641,
          visible = true,
          properties = {}
        },
        {
          id = 149,
          name = "",
          type = "",
          shape = "rectangle",
          x = 866,
          y = 403.333,
          width = 128,
          height = 512,
          rotation = 0,
          gid = 642,
          visible = true,
          properties = {}
        },
        {
          id = 150,
          name = "",
          type = "",
          shape = "rectangle",
          x = 996,
          y = 436.667,
          width = 128,
          height = 512,
          rotation = 0,
          gid = 643,
          visible = true,
          properties = {}
        },
        {
          id = 151,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1115.33,
          y = 454.667,
          width = 128,
          height = 512,
          rotation = 0,
          gid = 644,
          visible = true,
          properties = {}
        },
        {
          id = 152,
          name = "",
          type = "",
          shape = "rectangle",
          x = 502,
          y = 497,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 645,
          visible = true,
          properties = {}
        },
        {
          id = 153,
          name = "",
          type = "",
          shape = "rectangle",
          x = 609,
          y = 524,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 646,
          visible = true,
          properties = {}
        },
        {
          id = 154,
          name = "",
          type = "",
          shape = "rectangle",
          x = 740,
          y = 487,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 647,
          visible = true,
          properties = {}
        },
        {
          id = 155,
          name = "",
          type = "",
          shape = "rectangle",
          x = 889,
          y = 480,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 648,
          visible = true,
          properties = {}
        },
        {
          id = 156,
          name = "",
          type = "",
          shape = "rectangle",
          x = 526,
          y = 651,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 649,
          visible = true,
          properties = {}
        },
        {
          id = 157,
          name = "",
          type = "",
          shape = "rectangle",
          x = 640,
          y = 651,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 650,
          visible = true,
          properties = {}
        },
        {
          id = 158,
          name = "",
          type = "",
          shape = "rectangle",
          x = 776,
          y = 633,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 651,
          visible = true,
          properties = {}
        },
        {
          id = 159,
          name = "",
          type = "",
          shape = "rectangle",
          x = 948,
          y = 620,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 652,
          visible = true,
          properties = {}
        },
        {
          id = 160,
          name = "",
          type = "",
          shape = "rectangle",
          x = 526,
          y = 797,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 653,
          visible = true,
          properties = {}
        },
        {
          id = 161,
          name = "",
          type = "",
          shape = "rectangle",
          x = 663,
          y = 793,
          width = 128,
          height = 128,
          rotation = 0,
          gid = 654,
          visible = true,
          properties = {}
        },
        {
          id = 162,
          name = "",
          type = "",
          shape = "rectangle",
          x = 604.25,
          y = 165.25,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 628,
          visible = true,
          properties = {}
        },
        {
          id = 165,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1154,
          y = 561,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 661,
          visible = true,
          properties = {}
        },
        {
          id = 166,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1199,
          y = 556,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 677,
          visible = true,
          properties = {}
        },
        {
          id = 167,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1237,
          y = 563,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 693,
          visible = true,
          properties = {}
        },
        {
          id = 168,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1293,
          y = 560,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 741,
          visible = true,
          properties = {}
        },
        {
          id = 169,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1343,
          y = 560.5,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 757,
          visible = true,
          properties = {}
        },
        {
          id = 170,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1387.5,
          y = 557.5,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 773,
          visible = true,
          properties = {}
        },
        {
          id = 171,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1438.5,
          y = 555,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 749,
          visible = true,
          properties = {}
        },
        {
          id = 172,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1490.5,
          y = 556,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 765,
          visible = true,
          properties = {}
        },
        {
          id = 173,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1540.5,
          y = 552,
          width = 32,
          height = 96,
          rotation = 0,
          gid = 781,
          visible = true,
          properties = {}
        },
        {
          id = 174,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1198.5,
          y = 626.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 821,
          visible = true,
          properties = {}
        },
        {
          id = 175,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1232,
          y = 629,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 917,
          visible = true,
          properties = {}
        },
        {
          id = 176,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1268,
          y = 626.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1013,
          visible = true,
          properties = {}
        },
        {
          id = 177,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1305,
          y = 626,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1109,
          visible = true,
          properties = {}
        },
        {
          id = 178,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1344.5,
          y = 627,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1205,
          visible = true,
          properties = {}
        },
        {
          id = 179,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1376.5,
          y = 624,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 825,
          visible = true,
          properties = {}
        },
        {
          id = 180,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1412,
          y = 627.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 921,
          visible = true,
          properties = {}
        },
        {
          id = 181,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1445.5,
          y = 632.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1017,
          visible = true,
          properties = {}
        },
        {
          id = 182,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1498.5,
          y = 643,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1113,
          visible = true,
          properties = {}
        },
        {
          id = 183,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1204.5,
          y = 670.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1209,
          visible = true,
          properties = {}
        },
        {
          id = 185,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1234,
          y = 670,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 829,
          visible = true,
          properties = {}
        },
        {
          id = 186,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1264,
          y = 671,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 925,
          visible = true,
          properties = {}
        },
        {
          id = 187,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1300.5,
          y = 670.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1021,
          visible = true,
          properties = {}
        },
        {
          id = 188,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1334,
          y = 677.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1117,
          visible = true,
          properties = {}
        },
        {
          id = 189,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1378,
          y = 671,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1213,
          visible = true,
          properties = {}
        },
        {
          id = 190,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1419.5,
          y = 676.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 833,
          visible = true,
          properties = {}
        },
        {
          id = 191,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1464,
          y = 680.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 929,
          visible = true,
          properties = {}
        },
        {
          id = 192,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1510.5,
          y = 680.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1025,
          visible = true,
          properties = {}
        },
        {
          id = 193,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1189,
          y = 707.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1121,
          visible = true,
          properties = {}
        },
        {
          id = 194,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1237.5,
          y = 708.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1217,
          visible = true,
          properties = {}
        },
        {
          id = 195,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1278.5,
          y = 714.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 837,
          visible = true,
          properties = {}
        },
        {
          id = 196,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1316,
          y = 714,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 933,
          visible = true,
          properties = {}
        },
        {
          id = 197,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1360,
          y = 714,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1029,
          visible = true,
          properties = {}
        },
        {
          id = 198,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1399.5,
          y = 716.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1125,
          visible = true,
          properties = {}
        },
        {
          id = 199,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1436.5,
          y = 716.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1221,
          visible = true,
          properties = {}
        },
        {
          id = 200,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1463,
          y = 716.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 841,
          visible = true,
          properties = {}
        },
        {
          id = 201,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1487.5,
          y = 723.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 937,
          visible = true,
          properties = {}
        },
        {
          id = 202,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1210,
          y = 746,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1033,
          visible = true,
          properties = {}
        },
        {
          id = 203,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1246,
          y = 744.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1129,
          visible = true,
          properties = {}
        },
        {
          id = 204,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1284,
          y = 754,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1225,
          visible = true,
          properties = {}
        },
        {
          id = 205,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1311.5,
          y = 755,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 845,
          visible = true,
          properties = {}
        },
        {
          id = 206,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1347.5,
          y = 752.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 941,
          visible = true,
          properties = {}
        },
        {
          id = 207,
          name = "",
          type = "",
          shape = "rectangle",
          x = 1382.5,
          y = 750.5,
          width = 32,
          height = 32,
          rotation = 0,
          gid = 1037,
          visible = true,
          properties = {}
        }
      }
    }
  }
}
