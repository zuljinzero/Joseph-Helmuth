using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;


namespace UltimaLike
{
    class Enemies
    {
        public string NAME { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        // PX & PY hold the previous X and Y positions to avoid early movement in mapNow array
        public int DX { get; set; }
        public int DY { get; set; }
        public int HP { get; set; }
        public int ATTACK { get; set; }
        // SPEED is how many ticks before the enemy moves again
        public int SPEED { get; set; }
        public Color COLOR { get; set; }
        public int GOLD { get; set; }
        public int XP { get; set; }

        public Enemies(string name, int x, int y, int hp, int atk, int spd, Color color, int gold, int xp)
        {
            NAME = name;
            X = x;
            Y = y;
            DX = 0;
            DY = 0;
            HP = hp;
            ATTACK = atk;
            SPEED = spd;
            COLOR = color;
            GOLD = gold;
            XP = xp;
        }

        public static List<Enemies> AddEnemy(string name, int amount, int hp, int atk, int spd, Color color, int gold, int xp)
        {
            List<Enemies> newEnemy = new List<Enemies>();
            Random rand = new Random();     // change to map size once determined
            for (int i = 0; i < amount; i++)
            {
                // Just in case i need to assign px and py to match the X and Y, otherwise is using random?
                // int px = rand.Next(1, 25);
                // int py = rand.Next(1, 25);
                newEnemy.Add(new Enemies(name, rand.Next(5, 154), rand.Next(5, 154), hp, atk, spd, color, gold, xp));
            }

            return newEnemy;
        }
    }
}

