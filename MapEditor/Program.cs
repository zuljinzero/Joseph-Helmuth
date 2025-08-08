using System;
using System.IO;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.Color;


namespace MapEditor
{
    class Program
    {
        static string mapType = "WORLD"; // "WORLD", "TOWN", "DUNGEON"

        static bool running = true;

        static int mouseX;
        static int mouseY;
        static int worldMapSize = 160;
        static int townMapSize = 65;
        // static int dungeonMapSize = 17;
        static int dungeonAllMapSize = 53;

        static int mapSize = 10;
        static int worldTileSize = 5;
        static int townTileSize = 12;
        // static int dungeonTileSize = 47; // for 3 x 3 display with 1 tile seperator
        static int dungeonAllTileSize = 15;

        static int saveClick = 0;

        // UNIVERSAL CHARACTERS //
        static char empty = '.';

        // WORLD CHARACTERS //
        static char water = '~';
        static char hill = '#';
        static char forest = 'F';
        static char town = 'T';
        static char dungeon = 'D';
        static char road = 'R';

        // TOWN & DUNGEON CHARACTERS //
        static char wall = '#';
        static char chest = 'T';
        static char button = 'D';

        // TOWN CHARACTERS //
        static char river = '~';
        static char tree = 'F';
        static char path = 'R';

        // DUNGEON CHARACTERS //
        static char down = '~';
        static char trap = 'F';
        static char up = 'R';

        static char choice = empty;
        static string choiceBrush = "single";

        static bool emptyBtn = true;
        static bool hillBtn = false;
        static string hillBtnString = "hill";
        static bool forestBtn = false;
        static string forestBtnString = "forest";
        static bool waterBtn = false;
        static string waterBtnString = "water";
        static bool townBtn = false;
        static string townBtnString = "town";
        static bool dungeonBtn = false;
        static string dungeonBtnString = "dungeon";
        static bool roadBtn = false;
        static string roadBtnString = "road";
        static bool saveBtn = false;

        static bool singleBtn = true;
        static bool boxBtn = false;
        static bool circleBtn = false;

        static bool worldMapBtn = true;
        static bool townMapBtn = false;
        static bool dungeonMapBtn = false;

        static bool save = false;

        static char[,] mapWorld = new char[worldMapSize, worldMapSize];
        static char[,] mapTown = new char[townMapSize, townMapSize];
        static char[,] mapDungeonAll = new char[dungeonAllMapSize, dungeonAllMapSize];

        static char[,] tempMapWorld = new char[worldMapSize, worldMapSize];
        static char[,] tempMapTown = new char[townMapSize, townMapSize];
        static char[,] tempMapDungeon = new char[dungeonAllMapSize, dungeonAllMapSize];

        static string loadedMap = "";
        static string mapFile = "GameMapWorld01.txt";

        public static void DrawScene()
        {            
            // if (test) {Console.WriteLine(worldTileSize); test = false;}

            DrawText($"{mouseX}, {mouseY}", 1, 1, 5, WHITE);

            switch (mapType)
            {
                case "WORLD":
                    DrawWorldMap();
                    break;
                
                case "TOWN":
                    DrawTownMap();
                    break;
                
                case "DUNGEON":
                    DrawDungeonMap();
                    break;
            }

            DrawButtons();
        }

        public static void InsertMap()
        {
            switch (mapType)
            {
                case "WORLD":
                    for (int y = 0; y < worldMapSize; y++)
                    {
                        for (int x = 0; x < worldMapSize; x++)
                        {
                            if (choiceBrush == "single")
                            {
                                if ((mouseX >= (mapSize + (worldTileSize * x)) && mouseX <= (mapSize + (worldTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (worldTileSize * y)) && mouseY <= (mapSize + (worldTileSize * (y + 1)))))
                                {
                                    mapWorld[y,x] = choice;
                                }
                            }

                            if (choiceBrush == "box")
                            {
                                if ((mouseX >= (mapSize + (worldTileSize * x)) && mouseX <= (mapSize + (worldTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (worldTileSize * y)) && mouseY <= (mapSize + (worldTileSize * (y + 1)))))
                                {
                                    mapWorld[y,x] = choice;
                                    if (y - 1 >= 0 && x - 1 >= 0)
                                    {
                                        mapWorld[y-1,x-1] = choice;
                                    }
                                    if (y - 1 >= 0 && x >= 0)
                                    {
                                        mapWorld[y-1,x] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y-1,x+1] = choice;
                                    }
                                    if (y >= 0 && x - 1 >= 0)
                                    {
                                        mapWorld[y,x-1] = choice;
                                    }
                                    if (y >= 0 && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y,x+1] = choice;
                                    }
                                    if (y + 1 < worldMapSize && x - 1 >= 0)
                                    {
                                        mapWorld[y+1,x-1] = choice;
                                    }
                                    if (y + 1 < worldMapSize)
                                    {
                                        mapWorld[y+1,x] = choice;
                                    }
                                    if (y + 1 < worldMapSize && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y+1,x+1] = choice;
                                    }
                                }
                            }

                            if (choiceBrush == "circle")
                            {
                                if ((mouseX >= (mapSize + (worldTileSize * x)) && mouseX <= (mapSize + (worldTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (worldTileSize * y)) && mouseY <= (mapSize + (worldTileSize * (y + 1)))))
                                {
                                    mapWorld[y,x] = choice;
                                    if (y - 1 >= 0 && x - 1 >= 0)
                                    {
                                        mapWorld[y-1,x-1] = choice;
                                    }
                                    if (y - 1 >= 0 && x >= 0)
                                    {
                                        mapWorld[y-1,x] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y-1,x+1] = choice;
                                    }
                                    if (y >= 0 && x - 1 >= 0)
                                    {
                                        mapWorld[y,x-1] = choice;
                                    }
                                    if (y >= 0 && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y,x+1] = choice;
                                    }
                                    if (y + 1 < worldMapSize && x - 1 >= 0)
                                    {
                                        mapWorld[y+1,x-1] = choice;
                                    }
                                    if (y + 1 < worldMapSize)
                                    {
                                        mapWorld[y+1,x] = choice;
                                    }
                                    if (y + 1 < worldMapSize && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y+1,x+1] = choice;
                                    }
                                    if (y - 1 >= 0 && x - 2 >= 0)
                                    {
                                        mapWorld[y-1,x-2] = choice;
                                    }
                                    if (y >= 0 && x - 2 >= 0)
                                    {
                                        mapWorld[y,x-2] = choice;
                                    }
                                    if (y + 1 < worldMapSize && x - 2 >= 0)
                                    {
                                        mapWorld[y+1,x-2] = choice;
                                    }
                                    if (y - 2 >= 0 && x - 1 >= 0)
                                    {
                                        mapWorld[y-2,x-1] = choice;
                                    }
                                    if (y - 2 >= 0 && x >= 0)
                                    {
                                        mapWorld[y-2,x] = choice;
                                    }
                                    if (y - 2 >= 0 && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y-2,x+1] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 2 < worldMapSize)
                                    {
                                        mapWorld[y-1,x+2] = choice;
                                    }
                                    if (y >= 0 && x + 2 < worldMapSize)
                                    {
                                        mapWorld[y,x+2] = choice;
                                    }
                                    if (y + 1 < worldMapSize && x + 2 < worldMapSize)
                                    {
                                        mapWorld[y+1,x+2] = choice;
                                    }
                                    if (y + 2 < worldMapSize && x - 1 >= 0)
                                    {
                                        mapWorld[y+2,x-1] = choice;
                                    }
                                    if (y + 2 < worldMapSize && x >= 0)
                                    {
                                        mapWorld[y+2,x] = choice;
                                    }
                                    if (y + 2 < worldMapSize && x + 1 < worldMapSize)
                                    {
                                        mapWorld[y+2,x+1] = choice;
                                    }
                                }
                            }
                        }
                    }
                    break;
                
                case "TOWN":
                    for (int y = 0; y < townMapSize; y++)
                    {
                        for (int x = 0; x < townMapSize; x++)
                        {
                            if (choiceBrush == "single")
                            {
                                if ((mouseX >= (mapSize + (townTileSize * x)) && mouseX <= (mapSize + (townTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (townTileSize * y)) && mouseY <= (mapSize + (townTileSize * (y + 1)))))
                                {
                                    mapTown[y,x] = choice;
                                }
                            }

                            if (choiceBrush == "box")
                            {
                                if ((mouseX >= (mapSize + (townTileSize * x)) && mouseX <= (mapSize + (townTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (townTileSize * y)) && mouseY <= (mapSize + (townTileSize * (y + 1)))))
                                {
                                    mapTown[y,x] = choice;
                                    if (y - 1 >= 0 && x - 1 >= 0)
                                    {
                                        mapTown[y-1,x-1] = choice;
                                    }
                                    if (y - 1 >= 0 && x >= 0)
                                    {
                                        mapTown[y-1,x] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 1 < townMapSize)
                                    {
                                        mapTown[y-1,x+1] = choice;
                                    }
                                    if (y >= 0 && x - 1 >= 0)
                                    {
                                        mapTown[y,x-1] = choice;
                                    }
                                    if (y >= 0 && x + 1 < townMapSize)
                                    {
                                        mapTown[y,x+1] = choice;
                                    }
                                    if (y + 1 < townMapSize && x - 1 >= 0)
                                    {
                                        mapTown[y+1,x-1] = choice;
                                    }
                                    if (y + 1 < townMapSize)
                                    {
                                        mapTown[y+1,x] = choice;
                                    }
                                    if (y + 1 < townMapSize && x + 1 < townMapSize)
                                    {
                                        mapTown[y+1,x+1] = choice;
                                    }
                                }
                            }

                            if (choiceBrush == "circle")
                            {
                                if ((mouseX >= (mapSize + (townTileSize * x)) && mouseX <= (mapSize + (townTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (townTileSize * y)) && mouseY <= (mapSize + (townTileSize * (y + 1)))))
                                {
                                    mapTown[y,x] = choice;
                                    if (y - 1 >= 0 && x - 1 >= 0)
                                    {
                                        mapTown[y-1,x-1] = choice;
                                    }
                                    if (y - 1 >= 0 && x >= 0)
                                    {
                                        mapTown[y-1,x] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 1 < townMapSize)
                                    {
                                        mapTown[y-1,x+1] = choice;
                                    }
                                    if (y >= 0 && x - 1 >= 0)
                                    {
                                        mapTown[y,x-1] = choice;
                                    }
                                    if (y >= 0 && x + 1 < townMapSize)
                                    {
                                        mapTown[y,x+1] = choice;
                                    }
                                    if (y + 1 < townMapSize && x - 1 >= 0)
                                    {
                                        mapTown[y+1,x-1] = choice;
                                    }
                                    if (y + 1 < townMapSize)
                                    {
                                        mapTown[y+1,x] = choice;
                                    }
                                    if (y + 1 < townMapSize && x + 1 < townMapSize)
                                    {
                                        mapTown[y+1,x+1] = choice;
                                    }
                                    if (y - 1 >= 0 && x - 2 >= 0)
                                    {
                                        mapTown[y-1,x-2] = choice;
                                    }
                                    if (y >= 0 && x - 2 >= 0)
                                    {
                                        mapTown[y,x-2] = choice;
                                    }
                                    if (y + 1 < townMapSize && x - 2 >= 0)
                                    {
                                        mapTown[y+1,x-2] = choice;
                                    }
                                    if (y - 2 >= 0 && x - 1 >= 0)
                                    {
                                        mapTown[y-2,x-1] = choice;
                                    }
                                    if (y - 2 >= 0 && x >= 0)
                                    {
                                        mapTown[y-2,x] = choice;
                                    }
                                    if (y - 2 >= 0 && x + 1 < townMapSize)
                                    {
                                        mapTown[y-2,x+1] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 2 < townMapSize)
                                    {
                                        mapTown[y-1,x+2] = choice;
                                    }
                                    if (y >= 0 && x + 2 < townMapSize)
                                    {
                                        mapTown[y,x+2] = choice;
                                    }
                                    if (y + 1 < townMapSize && x + 2 < townMapSize)
                                    {
                                        mapTown[y+1,x+2] = choice;
                                    }
                                    if (y + 2 < townMapSize && x - 1 >= 0)
                                    {
                                        mapTown[y+2,x-1] = choice;
                                    }
                                    if (y + 2 < townMapSize && x >= 0)
                                    {
                                        mapTown[y+2,x] = choice;
                                    }
                                    if (y + 2 < townMapSize && x + 1 < townMapSize)
                                    {
                                        mapTown[y+2,x+1] = choice;
                                    }
                                }
                            }
                        }
                    }
                    break;
                
                case "DUNGEON":
                    for (int y = 0; y < dungeonAllMapSize; y++)
                    {
                        for (int x = 0; x < dungeonAllMapSize; x++)
                        {
                            if (choiceBrush == "single")
                            {
                                if ((mouseX >= (mapSize + (dungeonAllTileSize * x)) && mouseX <= (mapSize + (dungeonAllTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (dungeonAllTileSize * y)) && mouseY <= (mapSize + (dungeonAllTileSize * (y + 1)))))
                                {
                                    mapDungeonAll[y,x] = choice;
                                }
                            }

                            if (choiceBrush == "box")
                            {
                                if ((mouseX >= (mapSize + (dungeonAllTileSize * x)) && mouseX <= (mapSize + (dungeonAllTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (dungeonAllTileSize * y)) && mouseY <= (mapSize + (dungeonAllTileSize * (y + 1)))))
                                {
                                    mapDungeonAll[y,x] = choice;
                                    if (y - 1 >= 0 && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y-1,x-1] = choice;
                                    }
                                    if (y - 1 >= 0 && x >= 0)
                                    {
                                        mapDungeonAll[y-1,x] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y-1,x+1] = choice;
                                    }
                                    if (y >= 0 && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y,x-1] = choice;
                                    }
                                    if (y >= 0 && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y,x+1] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y+1,x-1] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y+1,x] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y+1,x+1] = choice;
                                    }
                                }
                            }

                            if (choiceBrush == "circle")
                            {
                                if ((mouseX >= (mapSize + (dungeonAllTileSize * x)) && mouseX <= (mapSize + (dungeonAllTileSize * (x + 1)))) &&
                                (mouseY >= (mapSize + (dungeonAllTileSize * y)) && mouseY <= (mapSize + (dungeonAllTileSize * (y + 1)))))
                                {
                                    mapDungeonAll[y,x] = choice;
                                    if (y - 1 >= 0 && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y-1,x-1] = choice;
                                    }
                                    if (y - 1 >= 0 && x >= 0)
                                    {
                                        mapDungeonAll[y-1,x] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y-1,x+1] = choice;
                                    }
                                    if (y >= 0 && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y,x-1] = choice;
                                    }
                                    if (y >= 0 && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y,x+1] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y+1,x-1] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y+1,x] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y+1,x+1] = choice;
                                    }
                                    if (y - 1 >= 0 && x - 2 >= 0)
                                    {
                                        mapDungeonAll[y-1,x-2] = choice;
                                    }
                                    if (y >= 0 && x - 2 >= 0)
                                    {
                                        mapDungeonAll[y,x-2] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize && x - 2 >= 0)
                                    {
                                        mapDungeonAll[y+1,x-2] = choice;
                                    }
                                    if (y - 2 >= 0 && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y-2,x-1] = choice;
                                    }
                                    if (y - 2 >= 0 && x >= 0)
                                    {
                                        mapDungeonAll[y-2,x] = choice;
                                    }
                                    if (y - 2 >= 0 && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y-2,x+1] = choice;
                                    }
                                    if (y - 1 >= 0 && x + 2 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y-1,x+2] = choice;
                                    }
                                    if (y >= 0 && x + 2 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y,x+2] = choice;
                                    }
                                    if (y + 1 < dungeonAllMapSize && x + 2 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y+1,x+2] = choice;
                                    }
                                    if (y + 2 < dungeonAllMapSize && x - 1 >= 0)
                                    {
                                        mapDungeonAll[y+2,x-1] = choice;
                                    }
                                    if (y + 2 < dungeonAllMapSize && x >= 0)
                                    {
                                        mapDungeonAll[y+2,x] = choice;
                                    }
                                    if (y + 2 < dungeonAllMapSize && x + 1 < dungeonAllMapSize)
                                    {
                                        mapDungeonAll[y+2,x+1] = choice;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            
        }

        public static void DrawWorldMap()
        {
            DrawRectangle(mapSize, mapSize, worldTileSize * worldMapSize, worldTileSize  * worldMapSize, GREEN);

            for (int y = 0; y < worldMapSize; y++)
            {
                for (int x = 0; x < worldMapSize; x++)
                {
                    switch (mapWorld[y,x])
                    {
                        // hill
                        case '#':
                            DrawRectangle(mapSize + (worldTileSize * x), mapSize + (worldTileSize * y), worldTileSize, worldTileSize, BROWN);
                            break;
                        
                        // forest
                        case 'F':
                            DrawRectangle(mapSize + (worldTileSize * x), mapSize + (worldTileSize * y), worldTileSize, worldTileSize, DARKGREEN);
                            break;

                        // water
                        case '~':
                            DrawRectangle(mapSize + (worldTileSize * x), mapSize + (worldTileSize * y), worldTileSize, worldTileSize, BLUE);
                            break;
                        
                        // town
                        case 'T':
                            DrawRectangle(mapSize + (worldTileSize * x), mapSize + (worldTileSize * y), worldTileSize, worldTileSize, VIOLET);
                            break;
                        
                        // dungeon
                        case 'D':
                            DrawRectangle(mapSize + (worldTileSize * x), mapSize + (worldTileSize * y), worldTileSize, worldTileSize, BLACK);
                            break;

                        // road
                        case 'R':
                            DrawRectangle(mapSize + (worldTileSize * x), mapSize + (worldTileSize * y), worldTileSize, worldTileSize, LIGHTGRAY);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public static void DrawTownMap()
        {
            DrawRectangle(mapSize, mapSize, townTileSize * townMapSize, townTileSize  * townMapSize, GREEN);

            for (int y = 0; y < townMapSize; y++)
            {
                for (int x = 0; x < townMapSize; x++)
                {
                    switch (mapTown[y,x])
                    {
                        // wall
                        case '#':
                            DrawRectangle(mapSize + (townTileSize * x), mapSize + (townTileSize * y), townTileSize, townTileSize, BROWN);
                            break;
                        
                        // tree
                        case 'F':
                            DrawRectangle(mapSize + (townTileSize * x), mapSize + (townTileSize * y), townTileSize, townTileSize, DARKGREEN);
                            break;

                        // water
                        case '~':
                            DrawRectangle(mapSize + (townTileSize * x), mapSize + (townTileSize * y), townTileSize, townTileSize, BLUE);
                            break;
                        
                        // chest
                        case 'T':
                            DrawRectangle(mapSize + (townTileSize * x), mapSize + (townTileSize * y), townTileSize, townTileSize, VIOLET);
                            break;
                        
                        // button
                        case 'D':
                            DrawRectangle(mapSize + (townTileSize * x), mapSize + (townTileSize * y), townTileSize, townTileSize, BLACK);
                            break;

                        // path
                        case 'R':
                            DrawRectangle(mapSize + (townTileSize * x), mapSize + (townTileSize * y), townTileSize, townTileSize, LIGHTGRAY);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public static void DrawDungeonMap()
        {
            // DrawRectangle(mapSize, mapSize, dungeonAllTileSize * dungeonAllMapSize, dungeonAllTileSize  * dungeonAllMapSize, GREEN);

            for (int y = 0; y < dungeonAllMapSize; y++)
            {
                for (int x = 0; x < dungeonAllMapSize; x++)
                {
                    switch (mapDungeonAll[y,x])
                    {
                        // wall
                        case '#':
                            DrawRectangle(mapSize + (dungeonAllTileSize * x), mapSize + (dungeonAllTileSize * y), dungeonAllTileSize, dungeonAllTileSize, BROWN);
                            break;
                        
                        // trap
                        case 'F':
                            DrawRectangle(mapSize + (dungeonAllTileSize * x), mapSize + (dungeonAllTileSize * y), dungeonAllTileSize, dungeonAllTileSize, DARKGREEN);
                            break;

                        // inout
                        case '~':
                            DrawRectangle(mapSize + (dungeonAllTileSize * x), mapSize + (dungeonAllTileSize * y), dungeonAllTileSize, dungeonAllTileSize, BLUE);
                            break;
                        
                        // chest
                        case 'T':
                            DrawRectangle(mapSize + (dungeonAllTileSize * x), mapSize + (dungeonAllTileSize * y), dungeonAllTileSize, dungeonAllTileSize, VIOLET);
                            break;
                        
                        // button
                        case 'D':
                            DrawRectangle(mapSize + (dungeonAllTileSize * x), mapSize + (dungeonAllTileSize * y), dungeonAllTileSize, dungeonAllTileSize, WHITE);
                            break;

                        // path
                        case 'R':
                            DrawRectangle(mapSize + (dungeonAllTileSize * x), mapSize + (dungeonAllTileSize * y), dungeonAllTileSize, dungeonAllTileSize, LIGHTGRAY);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public static void Input()
        {
            if (IsMouseButtonDown(0))
            {
                mouseX = GetMouseX();
                mouseY = GetMouseY();
                
                // Test mouse pos
                // Console.WriteLine($"{mouseX}, {mouseY}");

                CheckButton();
                InsertMap();
            }
        }

        public static void CheckButton()
        {
            // Empty button coords
            if ((mouseX >= 855 && mouseX <= 950) && (mouseY >= 50 && mouseY <= 85))
            {
                ResetButton();
                emptyBtn = true;
                choice = empty;
            }

            // Hill button coords
            if ((mouseX >= 855 && mouseX <= 950) && (mouseY >= 100 && mouseY <= 135))
            {
                ResetButton();
                hillBtn = true;
                choice = hill;
            }

            // Forest button coords
            if ((mouseX >= 855 && mouseX <= 965) && (mouseY >= 150 && mouseY <= 185))
            {
                ResetButton();
                forestBtn = true;
                choice = forest;
            }

            // Water button coords
            if ((mouseX >= 855 && mouseX <= 950) && (mouseY >= 200 && mouseY <= 235))
            {
                ResetButton();
                waterBtn = true;
                choice = water;
            }

            // Town button coords
            if ((mouseX >= 855 && mouseX <= 950) && (mouseY >= 250 && mouseY <= 285))
            {
                ResetButton();
                townBtn = true;
                choice = town;
            }
            
            // Dungeon button coords
            if ((mouseX >= 855 && mouseX <= 990) && (mouseY >= 300 && mouseY <= 335))
            {
                ResetButton();
                dungeonBtn = true;
                choice = dungeon;
            }

            // Road button coords
            if ((mouseX >= 855 && mouseX <= 960) && (mouseY >= 350 && mouseY <= 385))
            {
                ResetButton();
                roadBtn = true;
                choice = road;
            }

            // WOLRD map button coords
            if ((mouseX >= 1050 && mouseX <= 1161) && (mouseY >= 50 && mouseY <= 85))
            {
                ResetMapButton();
                worldMapBtn = true;
                mapType = "WORLD";
                ChangeButtonNames();
            }

            // TOWN map button coords
            if ((mouseX >= 1050 && mouseX <= 1146) && (mouseY >= 100 && mouseY <= 135))
            {
                ResetMapButton();
                townMapBtn = true;
                mapType = "TOWN";
                ChangeButtonNames();
            }

            // DUNGEON map button coords
            if ((mouseX >= 1050 && mouseX <= 1203) && (mouseY > 150 && mouseY <= 185))
            {
                ResetMapButton();
                dungeonMapBtn = true;
                mapType = "DUNGEON";
                ChangeButtonNames();
            }

            // Single brush button coords
            if ((mouseX >= 855 && mouseX <= 950) && (mouseY >= 550 && mouseY <= 585))
            {
                ResetBrushButton();
                singleBtn = true;
                choiceBrush = "single";
            }

            // Box brush button coords
            if ((mouseX >= 975 && mouseX <= 1035) && (mouseY >= 550 && mouseY <= 585))
            {
                ResetBrushButton();
                boxBtn = true;
                choiceBrush = "box";
            }

            // Circle brush button coords
            if ((mouseX >= 1060 && mouseX <= 1155) && (mouseY >= 550 && mouseY <= 585))
            {
                ResetBrushButton();
                circleBtn = true;
                choiceBrush = "circle";
            }

            // Save button coords
            if ((mouseX >= 855 && mouseX <= 950) && (mouseY >= (815 - 40) && mouseY <= (850 - 40)))
            {
                saveClick++;
                ResetButton();
                saveBtn = true;
                if (saveClick == 1) save = true;
            }
        }

        public static void DrawButtons()
        {
            // "WORLD" button //
            if (worldMapBtn)
            {
                DrawRectangle(1050, 50, 111, 35, DARKBLUE);
            }
            DrawCustomButton(1050, 1161, 50, 85, 1055, 50, 30, "WORLD");
            // -- //

            // "TOWN" button //
            if (townMapBtn)
            {
                DrawRectangle(1050, 100, 96, 35, DARKBLUE);
            }
            DrawCustomButton(1050, 1146, 100, 135, 1055, 100, 30, "TOWN");
            // -- //

            // "DUNGEON button //
            if (dungeonMapBtn)
            {
                DrawRectangle(1050, 150, 153, 35, DARKBLUE);
            }
            DrawCustomButton(1050, 1203, 150, 185, 1055, 150, 30, "DUNGEON");
            // -- //

            // "empty" button //
            if (emptyBtn)
            {
                DrawRectangle(855, 50, 95, 35, ORANGE);
            }
            DrawCustomButton(855, 950, 50, 85, 860, 50, 30, "empty");
            // -- //

            // "hill" button //
            if (hillBtn)
            {
                DrawRectangle(855, 100, 95, 35, ORANGE);
            }
            DrawCustomButton(855, 950, 100, 135, 860, 100, 30, hillBtnString);
            // -- //

            // "forest" button //
            if (forestBtn)
            {
                DrawRectangle(855, 150, 110, 35, ORANGE);
            }
            DrawCustomButton(855, 965, 150, 185, 860, 150, 30, forestBtnString);
            // -- //

            // "water" button //
            if (waterBtn)
            {
                DrawRectangle(855, 200, 95, 35, ORANGE);
            }
            DrawCustomButton(855, 950, 200, 235, 860, 200, 30, waterBtnString);
            // -- //

            // "town" button //
            if (townBtn)
            {
                DrawRectangle(855, 250, 95, 35, ORANGE);
            }
            DrawCustomButton(855, 950, 250, 285, 860, 250, 30, townBtnString);
            // -- //

            // "dungeon" button //
            if (dungeonBtn)
            {
                DrawRectangle(855, 300, 135, 35, ORANGE);
            }
            DrawCustomButton(855, 990, 300, 335, 860, 300, 30, dungeonBtnString);
            // -- //

            // "road" button //
            if (roadBtn)
            {
                DrawRectangle(855, 350, 95, 35, ORANGE);
            }
            DrawCustomButton(855, 950, 350, 385, 860, 350, 30, roadBtnString);
            // -- //

            // BRUSHES label //
            DrawCustomButton(855, 1155, 500, 535, 915, 500, 38, "BRUSHES");
            // -- //

            // "single" brush button //
            if (singleBtn)
            {
                DrawRectangle(855, 550, 95, 35, GRAY);
            }
            DrawCustomButton(855, 950, 550, 585, 860, 550, 30, "single");
            // -- //

            // "box" brush button //
            if (boxBtn)
            {
                DrawRectangle(975, 550, 60, 35, GRAY);
            }
            DrawCustomButton(975, 1035, 550, 585, 980, 550, 30, "box");
            // -- //

            // "circle" brush button //
            if (circleBtn)
            {
                DrawRectangle(1060, 550, 95, 35, GRAY);
            }
            DrawCustomButton(1060, 1155, 550, 585, 1065, 550, 30, "circle");
            // -- //

            // SAVE button //
            if (saveBtn)
            {
                DrawRectangle(855, 775, 105, 35, ORANGE);
            }
            DrawCustomButton(855, 960, 775, 810, 860, 777, 35, "SAVE");
            // -- //
        }

        public static void DrawCustomButton(int a, int b, int c, int d, int e, int f, int g, string label)
        {
            DrawLine(a, c, b, c, WHITE);
            DrawLine(a + 1, c, a, d, WHITE);
            DrawLine(a, d, b, d, WHITE);
            DrawLine(b, d, b, c, WHITE);
            DrawText(label, e, f, g, WHITE);
        }

        public static void ResetButton()
        {
            emptyBtn = false;
            hillBtn = false;
            forestBtn = false;
            waterBtn = false;
            townBtn = false;
            dungeonBtn = false;
            roadBtn = false;
            saveBtn = false;
        }

        public static void ResetMapButton()
        {
            worldMapBtn = false;
            townMapBtn = false;
            dungeonMapBtn = false;
        }

        public static void ResetBrushButton()
        {
            singleBtn = false;
            boxBtn = false;
            circleBtn = false;
        }

        public static void ChangeButtonNames()
        {
            switch (mapType)
            {
                case "WORLD":
                    waterBtnString = "water";
                    hillBtnString = "hill";
                    forestBtnString = "forest";
                    townBtnString = "town";
                    dungeonBtnString = "dungeon";
                    roadBtnString = "road";
                    break;
                
                case "TOWN":
                    waterBtnString = "river";
                    hillBtnString = "wall";
                    forestBtnString = "tree";
                    townBtnString = "chest";
                    dungeonBtnString = "button";
                    roadBtnString = "path";
                    break;
                
                case "DUNGEON":
                    waterBtnString = "down";
                    hillBtnString = "wall";
                    forestBtnString = "trap";
                    townBtnString = "chest";
                    dungeonBtnString = "button";
                    roadBtnString = "up";
                    break;
            }
        }

        public static void SaveMap()
        {
            save = false;
            string mapString = "";

            if (mapType == "WORLD")
            {
                for (int y = 0; y < worldMapSize; y++)
                {
                    for (int x = 0; x < worldMapSize; x++)
                    {
                        mapString = mapString + mapWorld[y,x];
                    }
                }

                File.AppendAllText("MapWorld.txt", mapString);
            }

            if (mapType == "TOWN")
            {
                for (int y = 0; y < townMapSize; y++)
                {
                    for (int x = 0; x < townMapSize; x++)
                    {
                        mapString = mapString + mapTown[y,x];
                    }
                }

                File.AppendAllText("MapTown.txt", mapString);
            }

            if (mapType == "DUNGEON")
            {
                for (int y = 0; y < dungeonAllMapSize; y++)
                {
                    for (int x = 0; x < dungeonAllMapSize; x++)
                    {
                        mapString = mapString + mapDungeonAll[y,x];
                    }
                }

                File.AppendAllText("MapDungeon.txt", mapString);
            }
        }

        public static void LoadMap()
        {
            try{
                using (var sr = new StreamReader(mapFile))
                {
                    loadedMap = sr.ReadToEnd();

                }

                for (int y = 0; y < worldMapSize; y++)
                {
                    for (int x = 0; x < worldMapSize; x++)
                    {
                        mapWorld[y,x] = loadedMap[((worldMapSize) * y) + x];
                        // if (x == 0 && y != 0)
                        // {
                        //     tempMapLoad[y,x] = loadedMap[y];
                        // }
                        // if (y != 0 && x != 0)
                        // {
                        //     tempMapLoad[y,x] = loadedMap[y * x];
                        // }
                        // else
                        // {
                        //     tempMapLoad[y,x] = loadedMap[x];
                        // }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read: " + e.Message);
            }

            // for (int y = 0; y < worldMapSize; y++)
            // {
            //     for (int x = 0; x < worldMapSize; x++)
            //     {
            //         Console.Write(tempMapWorld[y,x]);
            //     }
            //     Console.WriteLine();
            // }
        }

        public static void Update()
        {
            running = !WindowShouldClose();

            if (save)
            {
                SaveMap();
            }

            // TESTING MAP LOAD
            if (IsKeyPressed(KEY_L))
            {
                LoadMap();
            }
        }

        public static void Render()
        {
            BeginDrawing();

            ClearBackground(BLACK);
            DrawScene();

            EndDrawing();
        }

        public static void Init()
        {
            const int screenWidth = 1400;
            const int screenHeight = 820;

            InitWindow(screenWidth, screenHeight, "Map Editor");
            SetTargetFPS(60);

            for (int y = 0; y < worldMapSize; y++)
            {
                for (int x = 0; x < worldMapSize; x++)
                {
                    mapWorld[y,x] = empty;
                }
            }

            for (int y = 0; y < townMapSize; y++)
            {
                for (int x = 0; x < townMapSize; x++)
                {
                    mapTown[y,x] = empty;
                    if ((x == 0) || (x == (townMapSize - 1)) || (y == 0) || (y == (townMapSize - 1)))
                    {
                        mapTown[y,x] = water;
                    }
                }
            }

            for (int y = 0; y < dungeonAllMapSize; y++)
            {
                for (int x = 0; x < dungeonAllMapSize; x++)
                {
                    if ((((x >= 0 && x <= 16) || (x >= 18 && x <= 34) || (x >= 36 && x <= 52)) && 
                          (y == 0 || y == 16 || y == 18 || y == 34 || y == 36 || y == 52)) || 
                        (((y >= 1 && y <= 15) || (y >= 19 && y <= 33) || (y >= 37 && y <= 51)) && 
                          (x == 0 || x == 16 || x == 18 || x == 34 || x == 36 || x == 52)))
                    {
                        mapDungeonAll[y,x] = wall;
                    } else {
                        mapDungeonAll[y,x] = empty;
                    }
                }
            }
        }

        public static int Main()
        {
            Init();

            while (running)
            {
                Input();
                Update();
                Render();
            }

            CloseWindow();

            return 0;
        }
    }
}
