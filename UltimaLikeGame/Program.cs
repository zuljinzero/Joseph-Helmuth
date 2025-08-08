using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Text.Json;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.Color;

// TODO //
// FIX missile attacks
// geomod tech? like red faction...

// Halt program execution for 'double' seconds
// WaitTime(0.2f);

namespace UltimaLike
{
    class Program
    {
        enum State { PLAYER, PLAYERATTACK, PLAYERSHOOT, PLAYERCHOICE, PLAYERSPELL, PLAYERTALK, ENEMY, MENU, MAP, GAMEOVER };
        enum Location { WORLD, TOWN, DUNGEON };
        enum Menu { STATUS, ITEMS, ARMOR, WEAPONS, EQUIPABLES };

        public static int Main()
        {
            const int screenWidth = 1400;
            const int screenHeight = 820; //dt 900 // lptp 800
            
            InitWindow(screenWidth, screenHeight, "The Retired Hero and the Necromancer");
            // Disables ESC key close
            // SetExitKey(0);
            SetTargetFPS(60);

            // void Start()
            // {

            // }
            Random rand = new Random();

            const int slowSpeed = 3;
            const int mediumSpeed = 2;
            // int fastSpeed = 1;

            const int worldMapSize = 160; // 30 on each side for hills = + 60
            const int mapViewSize = 33;
            const int mapSize = 50; // dt 26 // lptp 23 // bit 32
            const int tileSize = 23; // 30
            const int tileSizeView = tileSize / 4;

            const char empty = '.';
            const char water = '~';
            const char hill = '#';
            const char forest = 'F';
            const char town = 'T';
            const char dungeon = 'D';
            const char road = 'R';
            // const char mountain = 'M';

            const char player = '@';
            const char dummy = '0';
            // const char npc = 'n';
            // const char shopKeeper = 's';
            const char questGiver = 'q';
            const char questItem1 = '1';
            const char questItem2 = '2';
            const char questItem3 = '3';
            const char enemyZ = 'z';
            const char enemyM = 'm';
            const char corpse = 'x';
            const char missile = '-';
            const char spellCursor = 's';
            // const char hPotion = 'h';
            
            const int textSize = 30;
            // const int textHeaderX = 820;
            const int textHeaderY = 50;
            const int textSTATSX = 850;
            const int textLVLY = 90;
            const int textXPY = 130;
            const int textHPY = 170;
            const int textGOLDY = 210;
            const int textTurnCountY = 250;

            const int textAction0 = 740;
            const int textAction1 = 700;
            const int textAction2 = 660;
            const int textAction3 = 620;
            const int textAction4 = 580;
            const int textAction5 = 540;
            
            // TESTING A THING //
            List<List<object>> saveObjects = new List<List<object>>();
            //////////

            ////////////////////////// ENEMY INIT ////////////////////////////
            // Creates allEnemies List to hold enemy type groups
            List<List<Enemies>> allEnemies = new List<List<Enemies>>();

            // Creates enemy type groups 
            List<Enemies> zombie;
            List<Enemies> mummy;

            // (amount-in-group, hp, attack, speed(turns between moves), color, gold, xp)
            // zombie = Enemies.AddEnemy("ZOMBIE", 15, 2, 1, slowSpeed, ORANGE, 1, 1);
            // mummy = Enemies.AddEnemy("MUMMY", 10, 4, 2, mediumSpeed, LIGHTGRAY, 2, 2);

            // Initialize zombie
            allEnemies = new List<List<Enemies>>();
            // allEnemies.Add(zombie);
            // allEnemies.Add(mummy);
            //////////////////////////////////////////////////////////////////


            ////////////////////////// WEAPON INIT ///////////////////////////
            // Create addWeapons to hold weapon type group
            List<Weapons> allWeapons = new List<Weapons>();

            Weapons woodenSword;
            Weapons ironSword;
            Weapons steelSword;

            // NOT FINAL ATRIBUTES
            // Weapons(NAME, DAMAGE, RANGE, COST, DROP_RATE)
            woodenSword = new Weapons("Wooden Sword", 1, 1, 5, 0.10f);
            ironSword = new Weapons("Iron Sword", 2, 1, 50, 0.05f);
            steelSword = new Weapons("Steel Sword", 3, 1, 150, 0.01f);

            allWeapons.Add(woodenSword);
            allWeapons.Add(ironSword);
            allWeapons.Add(steelSword);
            //////////////////////////////////////////////////////////////////


            ////////////////////////// ARMOR INIT ////////////////////////////
            List<Armor> allArmor = new List<Armor>();

            Armor woodenArmor;
            Armor ironArmor;
            Armor steelArmor;

            // Armor(NAME, AP, COST, DROP_RATE)
            woodenArmor = new Armor("Wooden Armor", 1, 15, 0.10f);
            ironArmor = new Armor("Iron Armor", 2, 50, 0.05f);
            steelArmor = new Armor("Steel Armor", 3, 150, 0.01f);

            allArmor.Add(woodenArmor);
            allArmor.Add(ironArmor);
            allArmor.Add(steelArmor);
            //////////////////////////////////////////////////////////////////


            ///////////////////////// WEARABLE INIT //////////////////////////
            List<Wearable> allWearable = new List<Wearable>();

            Wearable boneBelt;
            Wearable bloodAmulet;
            Wearable spiritBand;
            
            // Wearable(NAME, DAMAGE, AP, HP_BUFF, MP_BUFF, COST, DROP_RATE)
            boneBelt = new Wearable("Bone Belt", 0, 1, 5, 0, 75, 0.04f);
            bloodAmulet = new Wearable("Blood Amulet", 1, 0, 15, 0, 195, 0.03f);
            spiritBand = new Wearable("Spirit Band", 0, 2, 0, 30, 450, 0.0f);

            allWearable.Add(boneBelt);
            allWearable.Add(bloodAmulet);
            allWearable.Add(spiritBand);
            //////////////////////////////////////////////////////////////////


            ////////// PLAYER INVENTORY INIT //////////
            List<Weapons> playerWeapons = new List<Weapons>();
            List<Armor> playerArmor = new List<Armor>();
            List<Wearable> playerWearable = new List<Wearable>();

            List<Weapons> playerWeaponsBag = new List<Weapons>();
            List<Armor> playerArmorBag = new List<Armor>();
            List<Wearable> playerWearableBag = new List<Wearable>();
            // TEST INVENTORY //
            playerWeapons.Add(ironSword);
            playerArmor.Add(ironArmor);
            playerWearable.Add(bloodAmulet);
            //////////////////////////////////////
          
            char[,] testMap = new char[worldMapSize,worldMapSize];
            // testMap = Maps.WorldMap(char[,]);

            bool playerDEAD = false;

            // int frameCounter = 0;

            string quest1 = "";
            string quest2 = "";
            string quest3 = "";

            string spellChoice = "";
            int spellCursorX = 0;
            int spellCursorY = 0;
            bool spellChoiceWait = false;
            bool spellDrawCheck = false;

            int zombieSpeed = 3;
            int mummySpeed = 2;

            // Player creation (X, Y, HP, MP, ATTACK)
            // List<Player> plr;
            Player plr = new Player(20, 120, 20, 0, 2);
            // List<object> plr = new Player(worldMapSize/2, worldMapSize/2, 20, 0, 2);
            // plr.DisplayStats();

            int playerAdjustedAttack = 0;
            int playerAdjustedDefense = 0;
            int playerAdjustedHP = 0;
            int playerAdjustedMP = 0;

            int xMap;
            int yMap;
            int xCheck;
            int yCheck;

            int turnCount = 0;

            int ZStep = 1;
            int MStep = 1;
            int MX = 0;
            int MY = 0;

            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            bool showMap = false;

            string actionText0 = "";
            string actionText1 = "";
            string actionText2 = "";
            string actionText3 = "";
            string actionText4 = "";
            string actionText5 = "";

            State state = State.PLAYER;
            Location location = Location.WORLD;
            Menu menu = Menu.STATUS;

            char[,] mapTerrain = new char[worldMapSize, worldMapSize];
            char[,] mapExtra = new char[worldMapSize, worldMapSize];
            char[,] mapQuest = new char[worldMapSize, worldMapSize];
            char[,] mapCorpses = new char[worldMapSize, worldMapSize];
            char[,] mapCharacters = new char[worldMapSize, worldMapSize];
            char[,] mapView = new char[mapViewSize, mapViewSize];

            char[,] tempMapLoad = new char[worldMapSize, worldMapSize];
            char[,] mapLoad = new char[worldMapSize, worldMapSize];

            // Populate maps
            for (int i = 0; i < worldMapSize; i++)
            {
                for (int j = 0; j < worldMapSize; j++)
                {
                    mapTerrain[i,j] = empty;
                    mapCharacters[i,j] = empty;
                    mapCorpses[i,j] = empty;
                    mapExtra[i,j] = empty;
                    mapQuest[i,j] = empty;
                }
            }

            // mapCharacters[0,5] = dummy;
            // mapCharacters[159,7] = dummy;
            // mapCharacters[5,0] = dummy;
            // mapCharacters[7,159] = dummy;

            // Spawn player in center of map
            mapCharacters[plr.Y,plr.X] = player;

            // Test magic cursor
            // mapExtra[plr.Y,plr.X] = spellCursor;

            // // TESTING quest items //
            // mapQuest[98,98] = questItem1;
            // mapQuest[50,50] = questItem2;
            // mapQuest[40,75] = questItem3;
            // mapCharacters[66,64] = questGiver;

            void WaitTimer(int wt)
            {
                // Sleep for wt converted to 0.wt seconds
                // 50 becomes 500 (or 0.5 seconds)
                // 100 becomes 1000 (or 1 seconds)
                Thread.Sleep(wt * 10);
            }

            // void UpdateFrameCounter()
            // {
            //     if (frameCounter >= 1000)
            //     {
            //         frameCounter = 0;
            //     }
            //     frameCounter++;
            // }

            // void TestSaveList()
            // {
            //     var zombieObj = zombie.ToList<object>();
            //     saveObjects.Add(zombieObj);
            //     var mummyObj = mummy.ToList<object>();
            //     saveObjects.Add(mummyObj);
            //     // var plrObj = plr.ToList<object>();
            //     // saveObjects.Add(plr);
            //     var allWeaponsObj = allWeapons.ToList<object>();
            //     saveObjects.Add(allWeaponsObj);
            // }

            // var options = new JsonSerializerOptions { WriteIndented = true };
            // string saveFile = "SaveTest.json";
            // Create seperate file for plr stats 
            //(each paramater needs to be specified individually as a string
            // i.e. $"{plr.X}, {plr.Y} ...") // deserializing is gonna be a BITCH!

            // Saves objects to json file
            // void SaveGame(object obj)
            // {
            //     string saveString = JsonSerializer.Serialize(obj, options);
            //     File.WriteAllText(saveFile, saveString);

            //     // Console.WriteLine(File.ReadAllText(saveFile));
            // }

            // void LoadGame()
            // {
                // string loadString = File.ReadAllText(saveFile);
                // Type name = JsonSerializer.Deserialize<Type>(loadString)!;

                // Can writeline to test loaded paramaters like normal...
            // }

            // void SaveMap()
            // {
            //     // This way works, but DAMN that's a huge string!!!
            //     // string mapFile = "MapTest.json";
            //     // string mapString = string.Join(", ", mapTerrain.OfType<string>());
            //     // string saveMapString = JsonSerializer.Serialize(mapString, options);
            //     // File.WriteAllText(mapFile, saveMapString);

            //     // string[] mapArray = new string[worldMapSize];
            //     string mapString = "";

            //     for (int y = 0; y < worldMapSize; y++)
            //     {
            //         for (int x = 0; x < worldMapSize; x++)
            //         {
            //             // mapArray[x] = mapTerrain[y,x];
            //             mapString = mapString + mapTerrain[y,x];
            //         }
            //     }

            //     File.AppendAllText("MapTestText.txt", mapString);
            // }

            string loadedMap = "";

            void LoadMap()
            {
                try{
                    using (var sr = new StreamReader("GameMapWorld01.txt"))
                    {
                        loadedMap = sr.ReadToEnd();

                    }

                    for (int y = 0; y < worldMapSize; y++)
                    {
                        for (int x = 0; x < worldMapSize; x++)
                        {
                            mapTerrain[y,x] = loadedMap[((worldMapSize) * y) + x];
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read: " + e.Message);
                }

                for (int y = 0; y < worldMapSize; y++)
                {
                    for (int x = 0; x < worldMapSize; x++)
                    {
                        Console.Write(tempMapLoad[y,x]);
                    }
                    Console.WriteLine();
                }
            }

            void PlayerEquipedStatAdjust()
            {
                playerAdjustedAttack += plr.ATTACK;
                playerAdjustedHP += plr.HP;
                playerAdjustedMP += plr.MP;
                // Weapons(NAME, DAMAGE, RANGE, COST, DROP_RATE)
                foreach (var weapon in playerWeapons)
                {
                    playerAdjustedAttack += weapon.DAMAGE;
                }
                // Armor(NAME, AP, COST, DROP_RATE)
                foreach (var armor in playerArmor)
                {
                    playerAdjustedDefense += armor.AP;
                }
                // Wearable(NAME, DAMAGE, AP, HP_BUFF, MP_BUFF, COST, DROP_RATE)
                foreach (var wearable in playerWearable)
                {
                    playerAdjustedAttack += wearable.DAMAGE;
                    playerAdjustedDefense += wearable.AP;
                    playerAdjustedHP += wearable.HP_BUFF;
                    playerAdjustedMP += wearable.MP_BUFF;
                }
            }

            void PlayerAdjustedReset()
            {
                playerAdjustedAttack = 0;
                playerAdjustedDefense = 0;
                playerAdjustedHP = 0;
                playerAdjustedMP = 0;
            }

            // void TestWeapons()
            // {
            //     foreach (var weapon in allWeapons)
            //     {
            //         Console.WriteLine($"NAME: {weapon.NAME}, DAMAGE: {weapon.DAMAGE}, RANGE: {weapon.RANGE}, COST: {weapon.COST}, DROPRATE: {weapon.DROP_RATE}");
            //     }
            // }

            // void TestArmor()
            // {
            //     foreach (var armor in allArmor)
            //     {
            //         Console.WriteLine($"NAME: {armor.NAME}, AP: {armor.AP}, COST: {armor.COST}, DROPRATE: {armor.DROP_RATE}");
            //     }
            // }

            // void TestWearables()
            // {
            //     foreach (var wearable in allWearable)
            //     {
            //         Console.WriteLine($"NAME: {wearable.NAME}, DMG: {wearable.DAMAGE}, AP: {wearable.AP}, HPB: {wearable.HP_BUFF}, MPB: {wearable.MP_BUFF}, COST: {wearable.COST}, DR: {wearable.DROP_RATE}");
            //     }
            // }

            void SpawnEnemies()
            {
                for (int i = allEnemies.Count - 1; i >= 0; i--)
                {
                    for (int j = allEnemies[i].Count - 1; j >= 0; j--)
                    {
                        if (allEnemies[i][j].NAME == "ZOMBIE")
                        {
                            if ((mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] == hill || 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] == water || 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] == forest) && 
                                mapCharacters[allEnemies[i][j].Y, allEnemies[i][j].X] != empty)
                            {
                                allEnemies[i].Remove(allEnemies[i][j]);
                            }
                            if (mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] != hill && 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] != water && 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] != forest && 
                                mapCharacters[allEnemies[i][j].Y, allEnemies[i][j].X] == empty)
                            {
                                mapCharacters[allEnemies[i][j].Y, allEnemies[i][j].X] = enemyZ;
                            }
                        }
                        if (allEnemies[i][j].NAME == "MUMMY")
                        {
                            if ((mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] == hill || 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] == water || 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] == forest) && 
                                mapCharacters[allEnemies[i][j].Y, allEnemies[i][j].X] != empty)
                            {
                                allEnemies[i].Remove(allEnemies[i][j]);
                            }
                            if (mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] != hill && 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] != water && 
                                mapTerrain[allEnemies[i][j].Y, allEnemies[i][j].X] != forest &&
                                mapCharacters[allEnemies[i][j].Y, allEnemies[i][j].X] == empty)
                            {
                                mapCharacters[allEnemies[i][j].Y, allEnemies[i][j].X] = enemyM;
                            }
                        }
                    }
                }
            }

            void SpawnEnemiesWhileRunning()
            {
                if (rand.Next(1, 10) <= 8)
                {
                    if (rand.Next(1, 3) == 1)
                    {
                        zombie = Enemies.AddEnemy("ZOMBIE", 4, 2, 1, slowSpeed, ORANGE, 1, 1);
                        allEnemies.Add(zombie);
                    }
                    else
                    {
                        zombie = Enemies.AddEnemy("ZOMBIE", 2, 2, 1, slowSpeed, ORANGE, 1, 1);
                        allEnemies.Add(zombie);
                    }
                }
                else
                {
                    if (rand.Next(1, 3) == 1)
                    {
                        mummy = Enemies.AddEnemy("MUMMY", 2, 4, 2, mediumSpeed, LIGHTGRAY, 2, 2);
                        allEnemies.Add(mummy);
                    }
                    else
                    {
                        mummy = Enemies.AddEnemy("MUMMY", 1, 4, 2, mediumSpeed, LIGHTGRAY, 2, 2);
                        allEnemies.Add(mummy);
                    }
                }

                SpawnEnemies();
            }

            void SpawnCorpses(int tx, int ty)
            {
                for (int n = 1; n < 4; n++)
                {
                    if (mapCorpses[ty, tx] == empty)
                    {
                        mapCorpses[ty, tx] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty + n, tx] == empty)
                    {
                        mapCorpses[ty + n, tx] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty, tx + n] == empty)
                    {
                        mapCorpses[ty, tx + n] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty - n, tx] == empty)
                    {
                        mapCorpses[ty - n, tx] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty, tx - n] == empty)
                    {
                        mapCorpses[ty, tx - n] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty + n, tx + n] == empty)
                    {
                        mapCorpses[ty + n, tx + n] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty - n, tx + n] == empty)
                    {
                        mapCorpses[ty - n, tx + n] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty - n, tx - n] == empty)
                    {
                        mapCorpses[ty - n, tx - n] = corpse;
                        break;
                    }
                    else if (mapCorpses[ty + n, tx - n] == empty)
                    {
                        mapCorpses[ty + n, tx - n] = corpse;
                        break;
                    }
                }
            }

            void QuestManager()
            {
                // check quest1, quest2, quest3 strings for "Accepted", "Got", "Complete"
                
                // Quest complete check
                if (quest1 == "Got")
                {
                    DrawActionText("QUEST 1 COMPLETE!");
                    DrawActionText("GAINED 50 XP & GOLD!");
                    plr.XP += 50;
                    plr.GOLD += 50;
                    quest1 = "Complete";
                    return;
                }
                else if (quest2 == "Got")
                {
                    DrawActionText("QUEST 2 COMPLETE!");
                    DrawActionText("GAINED 50 XP & GOLD!");
                    plr.XP += 50;
                    plr.GOLD += 50;
                    quest2 = "Complete";
                    return;
                }
                else if (quest3 == "Got")
                {
                    DrawActionText("QUEST 3 COMPLETE!");
                    DrawActionText("GAINED 50 XP & GOLD!");
                    plr.XP += 50;
                    plr.GOLD += 50;
                    quest3 = "Complete";
                    return;
                }

                // Quest not completed check
                if (quest1 == "Accepted")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("GO GET QUEST ITEM 1 ALREADY!");
                    return;
                }
                else if (quest2 == "Accepted")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("GO GET QUEST ITEM 2 ALREADY!");
                    return;
                }
                else if (quest3 == "Accepted")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("GO GET QUEST ITEM 3 ALREADY!");
                    return;
                }

                // Quest give check
                if (quest1 == "")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("BRING ME QUEST ITEM 1");
                    quest1 = "Accepted";
                    mapQuest[rand.Next(81,129), rand.Next(81,129)] = questItem1;
                    // mapQuest[98,98] = questItem1;
                    return;
                }
                else if (quest2 == "")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("BRING ME QUEST ITEM 2");
                    quest2 = "Accepted";
                    mapQuest[rand.Next(31,79), rand.Next(31,79)] = questItem2;
                    // mapQuest[50,50] = questItem2;
                    return;
                }
                else if (quest3 == "")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("BRING ME QUEST ITEM 3");
                    quest3 = "Accepted";
                    mapQuest[rand.Next(31,79), rand.Next(81,129)] = questItem3;
                    // mapQuest[40,75] = questItem3;
                    return;
                }

                // No more quests to give
                if (quest1 == "Complete" && quest2 == "Complete" && quest3 == "Complete")
                {
                    DrawActionText("QUEST GIVER:");
                    DrawActionText("I HAVE NO MORE QUESTS...");
                }
            }

            void TurnEnd()
            {
                turnCount++;

                if (turnCount != 0 && turnCount % 10 == 0)
                {
                    SpawnEnemiesWhileRunning();
                }

                // WaitTimer(2);
                state = State.ENEMY;
            }

            void TurnState()
            {
                switch (state)
                {
                    case State.PLAYER:
                        // Check if player DEAD
                        // FIXME -- CREATE A PlayerDeadCheck that uses playerAdjustedHP to determine if dead
                        if (plr.HP <= 0)
                        {
                            mapCharacters[plr.Y,plr.X] = empty;
                            mapCorpses[plr.Y,plr.X] = corpse;
                            // DrawMap();
                            DrawMapView();
                            DrawTextDisplay();
                            playerDEAD = true;
                            state = State.GAMEOVER;
                        }
                        // If arrow key pressed, prepare player movememnt
                        if (IsKeyPressed(KEY_KP_4) || IsKeyPressed(KEY_LEFT))
                        {
                            plr.DX = -1;
                            PlayerMove();
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_6) || IsKeyPressed(KEY_RIGHT))
                        {
                            plr.DX = 1;
                            PlayerMove();
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_8) || IsKeyPressed(KEY_UP))
                        {
                            plr.DY = -1;
                            PlayerMove();
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_2) || IsKeyPressed(KEY_DOWN))
                        {
                            plr.DY = 1;
                            PlayerMove();
                            TurnEnd();
                        }
                        // Press space to skip turn
                        if (IsKeyPressed(KEY_SPACE))
                        {
                            TurnEnd();
                        }
                        // Press tab to open menu
                        if (IsKeyPressed(KEY_TAB))
                        {
                            state = State.MENU;
                        }
                        // Press M to open Map
                        if (IsKeyPressed(KEY_M))
                        {
                            state = State.MAP;
                        }
                        // Press A then a direction to attack
                        if (IsKeyPressed(KEY_A))
                        {
                            DrawActionText("PRESS DIRECTION TO ATTACK...");
                            state = State.PLAYERATTACK;
                        }
                        // Press S then a direction to shoot
                        if (IsKeyPressed(KEY_S))
                        {
                            DrawActionText("PRESS DIRECTION TO SHOOT...");
                            state = State.PLAYERSHOOT;
                        }
                        // Press C, choose a spell, then a direction
                        if (IsKeyPressed(KEY_C))
                        {
                            state = State.PLAYERSPELL;
                        }
                        // Get item on ground
                        if (IsKeyPressed(KEY_G))
                        {
                            if (mapQuest[plr.Y, plr.X] == questItem1)
                            {
                                quest1 = "Got";
                                DrawActionText("YOU PICKED UP QUEST ITEM 1");
                                mapQuest[plr.Y, plr.X] = empty;
                            }
                            if (mapQuest[plr.Y, plr.X] == questItem2)
                            {
                                quest2 = "Got";
                                DrawActionText("YOU PICKED UP QUEST ITEM 2");
                                mapQuest[plr.Y, plr.X] = empty;
                            }
                            if (mapQuest[plr.Y, plr.X] == questItem3)
                            {
                                quest3 = "Got";
                                DrawActionText("YOU PICKED UP QUEST ITEM 3");
                                mapQuest[plr.Y, plr.X] = empty;
                            }
                        }
                        if (IsKeyPressed(KEY_O))
                        {
                            location = Location.TOWN;
                        }
                        if (IsKeyPressed(KEY_P))
                        {
                            location = Location.WORLD;
                        }
                        // Press T then a direction to talk to NPC
                        if (IsKeyPressed(KEY_T))
                        {
                            state = State.PLAYERTALK;
                        }
                        // Press H to save // TESTING //
                        // if (IsKeyPressed(KEY_H))
                        // {
                        //     SaveGame(saveObjects);
                        // }
                        // Press J to store Lists into new Object List of Lists // TESTING //
                        // if (IsKeyPressed(KEY_J))
                        // {
                        //     TestSaveList();
                        // }
                        // if (IsKeyPressed(KEY_K))
                        // {
                        //     SaveMap();
                        // }
                        if (IsKeyPressed(KEY_L))
                        {
                            LoadMap();
                        }
                        break;
                    
                    case State.PLAYERATTACK:
                        if (IsKeyPressed(KEY_KP_4) || IsKeyPressed(KEY_LEFT))
                        {
                            plr.DX = -1;
                            PlayerAttack();
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_6) || IsKeyPressed(KEY_RIGHT))
                        {
                            plr.DX = 1;
                            PlayerAttack();
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_8) || IsKeyPressed(KEY_UP))
                        {
                            plr.DY = -1;
                            PlayerAttack();
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_2) || IsKeyPressed(KEY_DOWN))
                        {
                            plr.DY = 1;
                            PlayerAttack();
                            TurnEnd();
                        }
                        break;

                    case State.PLAYERSHOOT:
                        if (IsKeyPressed(KEY_KP_4) || IsKeyPressed(KEY_LEFT))
                        {
                            plr.DX = -1;
                            PlayerShoot(10);
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_6) || IsKeyPressed(KEY_RIGHT))
                        {
                            plr.DX = 1;
                            PlayerShoot(10);
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_8) || IsKeyPressed(KEY_UP))
                        {
                            plr.DY = -1;
                            PlayerShoot(10);
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_2) || IsKeyPressed(KEY_DOWN))
                        {
                            plr.DY = 1;
                            PlayerShoot(10);
                            TurnEnd();
                        }
                        break;

                    case State.PLAYERCHOICE:
                        state = State.PLAYER;
                        break;

                    case State.PLAYERSPELL:
                        if (spellChoice == "")
                        {
                            if (!spellChoiceWait)
                            {
                                DrawActionText("PRESS KEY TO CHOOSE SPELL:");
                                DrawActionText("S: SMIGHT, H: HOLY ESPLOSION");
                                DrawActionText("(C: CANCELS CASTING)");
                                spellCursorY = plr.Y;
                                spellCursorX = plr.X;
                                spellChoiceWait = true;
                            }

                            // Holy Esplosion
                            if (IsKeyPressed(KEY_H))
                            {
                                DrawActionText("CASTING HOLY ESPLOSION");
                                spellChoice = "Holy Esplosion";
                                // ClearDraw();
                                mapExtra[spellCursorY,spellCursorX] = spellCursor;
                            }
                            // Smight
                            if (IsKeyPressed(KEY_S))
                            {
                                DrawActionText("CASTING SMIGHT");
                                spellChoice = "Smight";
                                // ClearDraw();
                                mapExtra[spellCursorY,spellCursorX] = spellCursor;
                            }
                            // Cancel spell cast
                            if (IsKeyPressed(KEY_C))
                            {
                                ClearMapExtra();
                                ClearDraw();
                                state = State.PLAYER;
                                break;
                            }
                        }
                        else 
                        {
                            if (IsKeyPressed(KEY_KP_4) || IsKeyPressed(KEY_LEFT))
                            {
                                mapExtra[spellCursorY,spellCursorX] = empty;
                                spellCursorX -= 1;
                                mapExtra[spellCursorY,spellCursorX] = spellCursor;
                            }
                            if (IsKeyPressed(KEY_KP_6) || IsKeyPressed(KEY_RIGHT))
                            {
                                mapExtra[spellCursorY,spellCursorX] = empty;
                                spellCursorX += 1;
                                mapExtra[spellCursorY,spellCursorX] = spellCursor;
                            }
                            if (IsKeyPressed(KEY_KP_8) || IsKeyPressed(KEY_UP))
                            {
                                mapExtra[spellCursorY,spellCursorX] = empty;
                                spellCursorY -= 1;
                                mapExtra[spellCursorY,spellCursorX] = spellCursor;
                            }
                            if (IsKeyPressed(KEY_KP_2) || IsKeyPressed(KEY_DOWN))
                            {
                                mapExtra[spellCursorY,spellCursorX] = empty;
                                spellCursorY += 1;
                                mapExtra[spellCursorY,spellCursorX] = spellCursor;
                            }
                            // Cancel spell cast
                            if (IsKeyPressed(KEY_C))
                            {
                                ClearMapExtra();
                                ClearDraw();
                                state = State.PLAYER;
                                break;
                            }
                            if (IsKeyPressed(KEY_ENTER) || IsKeyPressed(KEY_KP_ENTER))
                            {
                                ClearDraw();
                                PlayerSpell();
                                spellChoice = "";
                                spellChoiceWait = false;
                                TurnEnd();
                            }
                        }
                        
                        break;
                    
                    case State.PLAYERTALK:
                        if (IsKeyPressed(KEY_KP_4) || IsKeyPressed(KEY_LEFT))
                        {
                            if (mapCharacters[plr.Y, plr.X - 1] == questGiver)
                            {
                                QuestManager();
                            }
                            else if (mapCharacters[plr.Y, plr.X - 1] == enemyM || 
                                     mapCharacters[plr.Y, plr.X - 1] == enemyZ)
                            {
                                DrawActionText("CAN'T TALK TO ENEMIES!");
                            }
                            else if (mapCharacters[plr.Y, plr.X - 1] == empty)
                            {
                                DrawActionText("NO ONE TO TALK TO...");
                            }
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_6) || IsKeyPressed(KEY_RIGHT))
                        {
                            if (mapCharacters[plr.Y, plr.X + 1] == questGiver)
                            {
                                QuestManager();
                            }
                            else if (mapCharacters[plr.Y, plr.X + 1] == enemyM || 
                                     mapCharacters[plr.Y, plr.X + 1] == enemyZ)
                            {
                                DrawActionText("CAN'T TALK TO ENEMIES!");
                            }
                            else if (mapCharacters[plr.Y, plr.X + 1] == empty)
                            {
                                DrawActionText("NO ONE TO TALK TO...");
                            }
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_8) || IsKeyPressed(KEY_UP))
                        {
                            if (mapCharacters[plr.Y - 1, plr.X] == questGiver)
                            {
                                QuestManager();
                            }
                            else if (mapCharacters[plr.Y - 1, plr.X] == enemyM || 
                                     mapCharacters[plr.Y - 1, plr.X] == enemyZ)
                            {
                                DrawActionText("CAN'T TALK TO ENEMIES!");
                            }
                            else if (mapCharacters[plr.Y - 1, plr.X] == empty)
                            {
                                DrawActionText("NO ONE TO TALK TO...");
                            }
                            TurnEnd();
                        }
                        if (IsKeyPressed(KEY_KP_2) || IsKeyPressed(KEY_DOWN))
                        {
                            if (mapCharacters[plr.Y + 1, plr.X] == questGiver)
                            {
                                QuestManager();
                            }
                            else if (mapCharacters[plr.Y + 1, plr.X] == enemyM || 
                                     mapCharacters[plr.Y + 1, plr.X] == enemyZ)
                            {
                                DrawActionText("CAN'T TALK TO ENEMIES!");
                            }
                            else if (mapCharacters[plr.Y + 1, plr.X] == empty)
                            {
                                DrawActionText("NO ONE TO TALK TO...");
                            }
                            TurnEnd();
                        }
                        break;
                        
                    case State.ENEMY:
                        // Not sure why this only works exactly here...
                        if (spellDrawCheck)
                        {
                            WaitTimer(30);
                            ClearMapExtra();
                            spellDrawCheck = false;
                        }
                        else
                        {
                            ClearMapExtra();
                        }
                        
                        // Each type will eventually need its own turn ticker 
                        // or this is going to get messy...
                        if (ZStep > zombieSpeed) ZStep = 1;
                        if (MStep > mummySpeed) MStep = 1;

                        foreach (var enemyList in allEnemies)
                        {
                            foreach (var enemy in enemyList)
                            {
                                if (enemy.NAME == "ZOMBIE")
                                {
                                    if (ZStep == zombieSpeed)
                                    {
                                        if (enemy.X > plr.X) enemy.DX = -1;
                                        if (enemy.X < plr.X) enemy.DX = 1;
                                        if (enemy.Y > plr.Y) enemy.DY = -1;
                                        if (enemy.Y < plr.Y) enemy.DY = 1;
                                    }
                                }

                                if (enemy.NAME == "MUMMY")
                                {
                                    if (MStep == mummySpeed)
                                    {
                                        if (enemy.X > plr.X) enemy.DX = -1;
                                        if (enemy.X < plr.X) enemy.DX = 1;
                                        if (enemy.Y > plr.Y) enemy.DY = -1;
                                        if (enemy.Y < plr.Y) enemy.DY = 1;
                                    }
                                }
                            }
                        }
                        EnemiesMove();

                        ZStep++;
                        MStep++;
                        
                        // WaitTimer();
                        state = State.PLAYER;
                        break;

                    case State.MENU:
                        state = State.PLAYER;
                        break;

                    case State.MAP:
                        showMap = true;
                        DrawMap();
                        if (IsKeyPressed(KEY_M))
                        {
                            showMap = false;
                            state = State.PLAYER;
                            break;
                        }
                        break;
                    
                    case State.GAMEOVER:
                        DrawText("Game Over\nPress R to restart...", 80, (screenHeight/2) - 60, 60, WHITE);
                        // FIXME //
                        // This is broken with new ememy List system.
                        // Create Reset() function that is also called at runtime
                        if (IsKeyPressed(KEY_R))
                        {
                            playerDEAD = false;
                            state = State.PLAYER;
                            plr.HP = 10;

                            mapCharacters[mapSize/2,mapSize/2] = player;
                        }
                        break;
                }
            }

            void PlayerMove()
            {
                yCheck = plr.Y + plr.DY;
                xCheck = plr.X + plr.DX;

                if (yCheck < 0)
                {
                    yMap = worldMapSize + yCheck;
                }
                else if (yCheck > (worldMapSize - 1))
                {
                    yMap = yCheck - worldMapSize;
                }
                else
                {
                    yMap = yCheck;
                }

                if (xCheck < 0)
                {
                    xMap = worldMapSize + xCheck;
                }
                else if (xCheck > (worldMapSize - 1))
                {
                    xMap = xCheck - worldMapSize;
                }
                else
                {
                    xMap = xCheck;
                }

                // Move player if destination is empty //
                if ((plr.DY != 0 || plr.DX != 0) && 
                    mapTerrain[yMap, xMap] != hill && 
                    mapTerrain[yMap, xMap] != water && 
                    mapTerrain[yMap, xMap] != forest && 
                    mapCharacters[yMap, xMap] == empty)
                {
                    if (plr.DY == -1) DrawActionText("N");
                    if (plr.DY == 1) DrawActionText("S");
                    if (plr.DX == -1) DrawActionText("W");
                    if (plr.DX == 1) DrawActionText("E");
                    mapCharacters[yMap, xMap] = player;
                    mapCharacters[plr.Y, plr.X] = empty;
                    plr.Y = yMap;
                    plr.X = xMap;
                }
                else
                {
                    if (plr.DY == -1) DrawActionText("CAN'T MOVE N");
                    if (plr.DY == 1) DrawActionText("CAN'T MOVE S");
                    if (plr.DX == -1) DrawActionText("CAN'T MOVE W");
                    if (plr.DX == 1) DrawActionText("CAN'T MOVE E");
                }
                plr.DY = 0;
                plr.DX = 0;
                ClearMapExtra();
            }

            void PlayerAttack()
            {
                yCheck = plr.Y + plr.DY;
                xCheck = plr.X + plr.DX;

                if (yCheck < 0)
                {
                    yMap = worldMapSize + yCheck;
                }
                else if (yCheck > (worldMapSize - 1))
                {
                    yMap = yCheck - worldMapSize;
                }
                else
                {
                    yMap = yCheck;
                }

                if (xCheck < 0)
                {
                    xMap = worldMapSize + xCheck;
                }
                else if (xCheck > (worldMapSize - 1))
                {
                    xMap = xCheck - worldMapSize;
                }
                else
                {
                    xMap = xCheck;
                }

                // Player attacks in specified attack direction
                if (((plr.DY != 0 || plr.DX != 0) && mapCharacters[yMap, xMap] == enemyZ) ||
                    ((plr.DY != 0 || plr.DX != 0) && mapCharacters[yMap, xMap] == enemyM) ||
                    ((plr.DY != 0 || plr.DX != 0) && mapCharacters[yMap, xMap] == dummy))
                {
                    DamageEnemy(xMap, yMap);
                }
                else
                {
                    DrawActionText("NOTHING TO ATTACK...");
                }
                plr.DY = 0;
                plr.DX = 0;
            }

            // FIXME //
            // Shoots all at once
            // Needs to
            void PlayerShoot(int s)
            {
                ClearMapExtra();

                // Player shoots projectile, x spaces, in specified direction
                if ((plr.DY != 0 || plr.DX != 0) && 
                    (mapCharacters[plr.Y + plr.DY, plr.X + plr.DX] == empty && 
                     mapTerrain[plr.Y + plr.DY, plr.X + plr.DX] != hill &&
                     mapTerrain[plr.Y + plr.DY, plr.X + plr.DX] != water &&
                     mapTerrain[plr.Y + plr.DY, plr.X + plr.DX] != forest))
                {
                    for (int n = 0; n < s; n++)
                    {
                        if (plr.DY == 1 && 
                            mapCharacters[plr.Y + plr.DY + n, plr.X + plr.DX] == empty && 
                            mapTerrain[plr.Y + plr.DY + n, plr.X + plr.DX] != hill &&
                            mapTerrain[plr.Y + plr.DY + n, plr.X + plr.DX] != water &&
                            mapTerrain[plr.Y + plr.DY + n, plr.X + plr.DX] != forest)
                        {
                            mapExtra[plr.Y + plr.DY + n, plr.X + plr.DX] = missile;
                        }
                        else if (plr.DY == 1 && 
                                (mapCharacters[plr.Y + plr.DY + n, plr.X + plr.DX] == enemyZ ||
                                 mapCharacters[plr.Y + plr.DY + n, plr.X + plr.DX] == enemyM))
                        {
                            DamageEnemy(plr.X + plr.DX, plr.Y + plr.DY + n);
                            break;
                        }

                        if (plr.DY == -1 && 
                            mapCharacters[plr.Y + plr.DY - n, plr.X + plr.DX] == empty && 
                            mapTerrain[plr.Y + plr.DY - n, plr.X + plr.DX] != hill &&
                            mapTerrain[plr.Y + plr.DY - n, plr.X + plr.DX] != water &&
                            mapTerrain[plr.Y + plr.DY - n, plr.X + plr.DX] != forest)
                        {
                            mapExtra[plr.Y + plr.DY - n, plr.X + plr.DX] = missile;
                        }
                        else if (plr.DY == -1 && 
                                (mapCharacters[plr.Y + plr.DY - n, plr.X + plr.DX] == enemyZ || 
                                 mapCharacters[plr.Y + plr.DY - n, plr.X + plr.DX] == enemyM))
                        {
                            DamageEnemy(plr.X + plr.DX, plr.Y + plr.DY - n);
                            break;
                        }

                        if (plr.DX == 1 && 
                            mapCharacters[plr.Y + plr.DY, plr.X + plr.DX + n] == empty && 
                            mapTerrain[plr.Y + plr.DY, plr.X + plr.DX + n] != hill &&
                            mapTerrain[plr.Y + plr.DY, plr.X + plr.DX + n] != water &&
                            mapTerrain[plr.Y + plr.DY, plr.X + plr.DX + n] != forest)
                        {
                            mapExtra[plr.Y + plr.DY, plr.X + plr.DX + n] = missile;
                        }
                        else if (plr.DX == 1 && 
                                (mapCharacters[plr.Y + plr.DY, plr.X + plr.DX + n] == enemyZ || 
                                 mapCharacters[plr.Y + plr.DY, plr.X + plr.DX + n] == enemyM))
                        {
                            DamageEnemy(plr.X + plr.DX + n, plr.Y + plr.DY);
                            break;
                        }

                        if (plr.DX == -1 && 
                            mapCharacters[plr.Y + plr.DY, plr.X + plr.DX - n] == empty && 
                            mapTerrain[plr.Y + plr.DY, plr.X + plr.DX - n] != hill &&
                            mapTerrain[plr.Y + plr.DY, plr.X + plr.DX - n] != water &&
                            mapTerrain[plr.Y + plr.DY, plr.X + plr.DX - n] != forest)
                        {
                            mapExtra[plr.Y + plr.DY, plr.X + plr.DX - n] = missile;
                        }
                        else if (plr.DX == -1 && 
                                (mapCharacters[plr.Y + plr.DY, plr.X + plr.DX - n] == enemyZ || 
                                 mapCharacters[plr.Y + plr.DY, plr.X + plr.DX - n] == enemyM))
                        {
                            DamageEnemy(plr.X + plr.DX - n, plr.Y + plr.DY);
                            break;
                        }
                        
                        ClearDraw();
                        WaitTimer(3);
                        ClearMapExtra();
                    }

                    ClearDraw();
                    WaitTimer(3);
                    ClearMapExtra();
                }
                plr.DY = 0;
                plr.DX = 0;
            }

            void PlayerSpell()
            {
                if (spellChoice == "Smight")
                {
                    spellDrawCheck = true;
                    mapExtra[spellCursorY,spellCursorX] = missile;

                    if (mapCharacters[spellCursorY,spellCursorX] == enemyZ ||
                        mapCharacters[spellCursorY,spellCursorX] == enemyM)
                    {
                        DamageEnemy(spellCursorX,spellCursorY);
                    }
                    return;
                }

                if (spellChoice == "Holy Esplosion")
                {
                    spellDrawCheck = true;
                    mapExtra[spellCursorY,spellCursorX] = missile;
                    if (mapCharacters[spellCursorY,spellCursorX] == enemyZ ||
                        mapCharacters[spellCursorY,spellCursorX] == enemyM)
                    {
                        DamageEnemy(spellCursorX,spellCursorY);
                    }

                    mapExtra[spellCursorY-2,spellCursorX] = missile;
                    if (mapCharacters[spellCursorY-2,spellCursorX] == enemyZ ||
                        mapCharacters[spellCursorY-2,spellCursorX] == enemyM)
                    {
                        DamageEnemy(spellCursorX,spellCursorY-2);
                    }

                    mapExtra[spellCursorY-1,spellCursorX] = missile;
                    if (mapCharacters[spellCursorY-1,spellCursorX] == enemyZ ||
                        mapCharacters[spellCursorY-1,spellCursorX] == enemyM)
                    {
                        DamageEnemy(spellCursorX,spellCursorY-1);
                    }

                    mapExtra[spellCursorY+1,spellCursorX] = missile;
                    if (mapCharacters[spellCursorY+1,spellCursorX] == enemyZ ||
                        mapCharacters[spellCursorY+1,spellCursorX] == enemyM)
                    {
                        DamageEnemy(spellCursorX,spellCursorY+1);
                    }
                    
                    mapExtra[spellCursorY+2,spellCursorX] = missile;
                    if (mapCharacters[spellCursorY+2,spellCursorX] == enemyZ ||
                        mapCharacters[spellCursorY+2,spellCursorX] == enemyM)
                    {
                        DamageEnemy(spellCursorX,spellCursorY+2);
                    }

                    mapExtra[spellCursorY,spellCursorX-2] = missile;
                    if (mapCharacters[spellCursorY,spellCursorX-2] == enemyZ ||
                        mapCharacters[spellCursorY,spellCursorX-2] == enemyM)
                    {
                        DamageEnemy(spellCursorX-2,spellCursorY);
                    }
                    
                    mapExtra[spellCursorY,spellCursorX-1] = missile;
                    if (mapCharacters[spellCursorY,spellCursorX-1] == enemyZ ||
                        mapCharacters[spellCursorY,spellCursorX-1] == enemyM)
                    {
                        DamageEnemy(spellCursorX-1,spellCursorY);
                    }
                    
                    mapExtra[spellCursorY,spellCursorX+1] = missile;
                    if (mapCharacters[spellCursorY,spellCursorX+1] == enemyZ ||
                        mapCharacters[spellCursorY,spellCursorX+1] == enemyM)
                    {
                        DamageEnemy(spellCursorX+1,spellCursorY);
                    }
                    
                    mapExtra[spellCursorY,spellCursorX+2] = missile;
                    if (mapCharacters[spellCursorY,spellCursorX+2] == enemyZ ||
                        mapCharacters[spellCursorY,spellCursorX+2] == enemyM)
                    {
                        DamageEnemy(spellCursorX+2,spellCursorY);
                    }
                    
                    mapExtra[spellCursorY-1,spellCursorX-1] = missile;
                    if (mapCharacters[spellCursorY-1,spellCursorX-1] == enemyZ ||
                        mapCharacters[spellCursorY-1,spellCursorX-1] == enemyM)
                    {
                        DamageEnemy(spellCursorX-1,spellCursorY-1);
                    }
                    
                    mapExtra[spellCursorY+1,spellCursorX-1] = missile;
                    if (mapCharacters[spellCursorY+1,spellCursorX-1] == enemyZ ||
                        mapCharacters[spellCursorY+1,spellCursorX-1] == enemyM)
                    {
                        DamageEnemy(spellCursorX-1,spellCursorY+1);
                    }
                    
                    mapExtra[spellCursorY+1,spellCursorX+1] = missile;
                    if (mapCharacters[spellCursorY+1,spellCursorX+1] == enemyZ ||
                        mapCharacters[spellCursorY+1,spellCursorX+1] == enemyM)
                    {
                        DamageEnemy(spellCursorX+1,spellCursorY+1);
                    }
                    
                    mapExtra[spellCursorY-1,spellCursorX+1] = missile;
                    if (mapCharacters[spellCursorY-1,spellCursorX+1] == enemyZ ||
                        mapCharacters[spellCursorY-1,spellCursorX+1] == enemyM)
                    {
                        DamageEnemy(spellCursorX+1,spellCursorY-1);
                    }

                    return;
                }
            }

            void PlayerLevelUpCheck()
            {
                // FIXME //
                // Placeholder values (lvl up gets 5xp more expensive every time)
                // Though these might work?
                if (plr.XP >= 270 && plr.LEVEL < 10) PlayerLevelUp();
                if (plr.XP >= 220 && plr.LEVEL < 9) PlayerLevelUp();
                if (plr.XP >= 175 && plr.LEVEL < 8) PlayerLevelUp();
                if (plr.XP >= 135 && plr.LEVEL < 7) PlayerLevelUp();
                if (plr.XP >= 100 && plr.LEVEL < 6) PlayerLevelUp();
                if (plr.XP >= 70 && plr.LEVEL < 5) PlayerLevelUp();
                if (plr.XP >= 45 && plr.LEVEL < 4) PlayerLevelUp();
                if (plr.XP >= 25 && plr.LEVEL < 3) PlayerLevelUp();
                if (plr.XP >= 10 && plr.LEVEL < 2) PlayerLevelUp();
            }
            void PlayerLevelUp()
            {
                // FIXME //
                // Need to set proper xp and stats for each level up
                plr.HP += (20 * plr.LEVEL);
                plr.LEVEL += 1;
                plr.ATTACK += 2;
                plr.DisplayStats();
                DrawActionText($"DING! YOU ARE NOW LEVEL {plr.LEVEL}!");
            }

            void EnemiesMove()
            {
                foreach (var enemyList in allEnemies)
                {
                    foreach (var enemy in enemyList)
                    {
                        // Hit player if destination is player
                        if ((enemy.DX != 0 || enemy.DY != 0) && (enemy.X != 0 || enemy.Y != 0) && 
                            mapCharacters[enemy.Y + enemy.DY, enemy.X + enemy.DX] == player)
                        {
                            DrawActionText($"{enemy.NAME} HITS PLAYER FOR {enemy.ATTACK}");
                            plr.HP--;
                        }
                        // Move enemyZ if destination is empty
                        else if ((enemy.DX != 0 || enemy.DY != 0) && (enemy.X != 0 || enemy.Y != 0) && 
                            mapCharacters[enemy.Y + enemy.DY, enemy.X + enemy.DX] == empty &&
                            mapTerrain[enemy.Y + enemy.DY, enemy.X + enemy.DX] != hill &&
                            mapTerrain[enemy.Y + enemy.DY, enemy.X + enemy.DX] != water &&
                            mapTerrain[enemy.Y + enemy.DY, enemy.X + enemy.DX] != forest)
                        {
                            if (enemy.NAME == "ZOMBIE")
                            {
                                mapCharacters[enemy.Y + enemy.DY, enemy.X + enemy.DX] = enemyZ;
                            }
                            if (enemy.NAME == "MUMMY")
                            {
                                mapCharacters[enemy.Y + enemy.DY, enemy.X + enemy.DX] = enemyM;
                            }
                            mapCharacters[enemy.Y, enemy.X] = empty;
                            enemy.X = enemy.X + enemy.DX;
                            enemy.Y = enemy.Y + enemy.DY;
                        }
                        enemy.DX = 0;
                        enemy.DY = 0;
                    }
                }
            }

            void DamageEnemy(int tx, int ty)
            {
                if (mapCharacters[ty, tx] == dummy)
                {
                    mapCharacters[ty, tx] = empty;
                    DrawActionText("YOU KILLED A DUMMY, YIPPIEEE!");
                }
                else
                {
                    for (int i = allEnemies.Count - 1; i >= 0; i--)
                    {
                        for (int j = allEnemies[i].Count - 1; j >= 0; j--)
                        {
                            if (allEnemies[i][j].Y == ty &&
                                allEnemies[i][j].X == tx)
                            {
                                allEnemies[i][j].HP = allEnemies[i][j].HP - playerAdjustedAttack;
                                DrawActionText($"DEALT {playerAdjustedAttack} DAMAGE TO {allEnemies[i][j].NAME}");
                                if (allEnemies[i][j].HP <= 0)
                                {
                                    DrawActionText($"KILLED {allEnemies[i][j].NAME}");
                                    DrawActionText($"GAINED {allEnemies[i][j].XP} XP & {allEnemies[i][j].GOLD} GOLD");
                                    plr.XP += allEnemies[i][j].XP;
                                    plr.GOLD += allEnemies[i][j].GOLD;
                                    PlayerLevelUpCheck();
                                    allEnemies[i].Remove(allEnemies[i][j]);
                                    mapCharacters[ty, tx] = empty;
                                    SpawnCorpses(tx, ty);
                                    ClearDraw();
                                }
                            }
                        }
                    }
                }
            }

            void DrawActionText(string lastAction)
            {
                actionText5 = actionText4;
                actionText4 = actionText3;
                actionText3 = actionText2;
                actionText2 = actionText1;
                actionText1 = actionText0;
                actionText0 = lastAction;
            }

            void DrawMapView()
            {
                MY = 0;
                for (int y = -16; y < 17; y++)
                {
                    MX = 0;
                    for (int x = -16; x < 17; x++)
                    {
                        yCheck = plr.Y + y;
                        xCheck = plr.X + x;

                        if (yCheck < 0)
                        {
                            yMap = worldMapSize + yCheck;
                        }
                        else if (yCheck > (worldMapSize - 1))
                        {
                            yMap = yCheck - worldMapSize;
                        }
                        else
                        {
                            yMap = yCheck;
                        }

                        if (xCheck < 0)
                        {
                            xMap = worldMapSize + xCheck;
                        }
                        else if (xCheck > (worldMapSize - 1))
                        {
                            xMap = xCheck - worldMapSize;
                        }
                        else
                        {
                            xMap = xCheck;
                        }

                        switch (mapTerrain[yMap,xMap])
                        {
                            case empty:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, GREEN);
                                break;

                            case water:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, BLUE);
                                break;
                                
                            case hill:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, BROWN);
                                break;

                            case forest:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, DARKGREEN);
                                break;

                            case town:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, VIOLET);
                                break;

                            case dungeon:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, BLACK);
                                break;

                            case road:
                                DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, LIGHTGRAY);
                                break;
                        }
                        if (mapCorpses[yMap,xMap] == corpse)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, RED);
                        }
                        if (mapCharacters[yMap,xMap] == enemyZ)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, ORANGE);
                        }
                        if (mapCharacters[yMap,xMap] == enemyM)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, BEIGE);
                        }
                        if (mapCharacters[yMap,xMap] == dummy)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, GRAY);
                        }
                        if (mapCharacters[yMap,xMap] == questGiver)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, WHITE);
                        }
                        if (mapQuest[yMap,xMap] == questItem1 || mapQuest[yMap,xMap] == questItem2 || mapQuest[yMap,xMap] == questItem3)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, PURPLE);
                        }
                        if (mapCharacters[yMap,xMap] == player)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, DARKPURPLE);
                        }
                        if (mapExtra[yMap,xMap] == missile)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, tileSize, YELLOW);
                        }
                        if (mapExtra[yMap,xMap] == spellCursor)
                        {
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), tileSize, 2, WHITE);
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY), 2, tileSize, WHITE);
                            DrawRectangle(50 + (tileSize * MX), 50 + (tileSize * MY) + tileSize - 2, tileSize, 2, WHITE);
                            DrawRectangle(50 + (tileSize * MX) + tileSize - 2, 50 + (tileSize * MY), 2, tileSize, WHITE);
                        }
                        MX++;
                    }
                    MY++;
                }
            }

            void DrawBorders()
            {
                // Left side of map
                // DrawLine(40, 40, 40, 797, WHITE);
                DrawLine(41, 41, 41, 819, WHITE);
                DrawLine(45, 45, 45, 815, WHITE);
                DrawLine(49, 49, 49, 811, WHITE);

                // Top side of map (offset x by -1, to fill a weird pixel gap)
                // DrawLine(39, 40, 797, 40, WHITE);
                DrawLine(40, 41, 819, 41, WHITE);
                DrawLine(44, 45, 815, 45, WHITE);
                DrawLine(48, 49, 811, 49, WHITE);

                // Right side of map
                // DrawLine(797, 40, 797, 797, WHITE);
                DrawLine(819, 41, 819, 819, WHITE);
                DrawLine(815, 45, 815, 815, WHITE);
                DrawLine(811, 49, 811, 811, WHITE);

                // Bottom side of map (offset x by -1, to fill a weird pixel gap)
                // DrawLine(39, 797, 797, 797, WHITE);
                DrawLine(40, 819, 819, 819, WHITE);
                DrawLine(44, 815, 815, 815, WHITE);
                DrawLine(48, 811, 811, 811, WHITE);

                // Left side of stats
                DrawLine(836, 40, 836, 290, WHITE);
                // Top side of stats
                DrawLine(835, 40, 1170, 40, WHITE);
                // Right side of stats
                DrawLine(1170, 40, 1170, 290, WHITE);
                // Bottom side of stats
                DrawLine(835, 290, 1170, 290, WHITE);

                // Left side of action text - 836, 530, 840, 780
                // Top side of action text
                // Right side of action text - 1370, 53, 1370, 780
                // Bottom side of action text


                // DrawRectangleLines(40, 45, 5, 700, WHITE);
            }

            void ClearMapExtra()
            {
                for (int y = 0; y < worldMapSize; y++)
                {
                    for (int x = 0; x < worldMapSize; x++)
                    {
                        mapExtra[y,x] = empty;
                    }
                }
            }

            void DrawMap()
            {
                // This first DrawRectangles is drawn first because Raylib can't draw over 80(?) rectangles of the same color
                // Draw empties
                DrawRectangle(14, 14, tileSizeView * worldMapSize, tileSizeView * worldMapSize, GREEN);

                for (int y = 0; y < worldMapSize; y++)
                {
                    for (int x = 0; x < worldMapSize; x++)
                    {
                        switch (mapTerrain[y,x])
                        {
                            case hill:
                                DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, BROWN);
                                break;

                            case forest:
                                DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, DARKGREEN);
                                break;

                            case water:
                                DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, BLUE);
                                break;

                            case town:
                                DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, VIOLET);
                                break;

                            case dungeon:
                                DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, BLACK);
                                break;

                            case road:
                                DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, LIGHTGRAY);
                                break;
                        }
                        if (mapCorpses[y,x] == corpse)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, RED);
                        }
                        if (mapCharacters[y,x] == enemyZ)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, ORANGE);
                        }
                        if (mapCharacters[y,x] == enemyM)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, LIGHTGRAY);
                        }
                        if (mapCharacters[y,x] == dummy)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, GRAY);
                        }
                        if (mapCharacters[y,x] == questGiver)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, WHITE);
                        }
                        if (mapQuest[y,x] == questItem1 || mapQuest[y,x] == questItem2 || mapQuest[y,x] == questItem3)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, PURPLE);
                        }
                        if (mapCharacters[y,x] == player)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, DARKPURPLE);
                        }
                        if (mapExtra[y,x] == missile)
                        {
                            DrawRectangle(14 + (tileSizeView * x), 14 + (tileSizeView * y), tileSizeView, tileSizeView, YELLOW);
                        }
                    }
                }
            }

            void DrawTextDisplay()
            {
                switch(menu)
                {
                    case Menu.STATUS:
                        // Text Header
                        // DrawText("--STATS-------------", textHeaderX, textHeaderY, textSize, WHITE);
                        DrawText("STATS-------------", textSTATSX, textHeaderY, textSize, WHITE);

                        // Text Player Stats
                        DrawText($"LVL: {plr.LEVEL}", textSTATSX, textLVLY, textSize, WHITE);
                        DrawText($"EXP: {plr.XP}", textSTATSX, textXPY, textSize, WHITE);
                        DrawText($"HP: {playerAdjustedHP}", textSTATSX, textHPY, textSize, WHITE);
                        DrawText($"GOLD: {plr.GOLD}", textSTATSX, textGOLDY, textSize, WHITE);
                        DrawText($"TURN {turnCount}", textSTATSX, textTurnCountY, textSize, WHITE);
                        break;
                    
                    case Menu.ITEMS:
                        break;
                    
                    case Menu.ARMOR:
                        break;
                    
                    case Menu.WEAPONS:
                        break;
                    
                    case Menu.EQUIPABLES:
                        break;
                }

                // Text Scrolling Actions
                DrawText(actionText0, textSTATSX, textAction0, textSize, WHITE);
                DrawText(actionText1, textSTATSX, textAction1, textSize, WHITE);
                DrawText(actionText2, textSTATSX, textAction2, textSize, WHITE);
                DrawText(actionText3, textSTATSX, textAction3, textSize, WHITE);
                DrawText(actionText4, textSTATSX, textAction4, textSize, WHITE);
                DrawText(actionText5, textSTATSX, textAction5, textSize, WHITE);
                
                // Draw line borders
                DrawBorders();

                // Mouse position for testing
                DrawText($"{mouseX}, {mouseY}", 10, 10, 5, WHITE);
            }

            void ClearDraw()
            {
                EndDrawing();
                BeginDrawing();
                ClearBackground(BLACK);
                DrawMapView();
                DrawTextDisplay();
            }
            
            SpawnEnemies();

            while (!WindowShouldClose())
            {
                PlayerEquipedStatAdjust();
                TurnState();

                BeginDrawing();
                ClearBackground(BLACK);

                if (!playerDEAD)
                {
                    // Turn this into a random enemy spawn every so many moves 
                    // (random between 5 and 10, or there is a CHANCE for them to spawn every 5 moves?)
                    if (IsKeyPressed(KEY_KP_0))
                    {
                        SpawnEnemiesWhileRunning();
                    }
                    
                    /////////////////

                    switch (location)
                    {
                        case Location.WORLD:
                            if (!showMap)
                            {
                                // DrawMap();
                                DrawMapView();
                                DrawTextDisplay();
                            }
                            break;
                        
                        case Location.TOWN:
                            Towns.TestTown();
                            break;
                    }
                }

                // TEST mouse pos
                if (IsMouseButtonDown(0))
                {
                    mouseX = GetMouseX();
                    mouseY = GetMouseY();
                }

                EndDrawing();
                PlayerAdjustedReset();
                
            }
            CloseWindow();

            return 0;
        }
    }
}

