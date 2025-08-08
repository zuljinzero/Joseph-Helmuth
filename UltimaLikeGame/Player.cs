

namespace UltimaLike
{
    class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int DX { get; set; }
        public int DY { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int XP { get; set; }
        public int LEVEL { get; set; }
        public int ATTACK { get; set; }
        public int GOLD { get; set; }
        // Should I store weapons and armor here?
        // OR just compare incoming and outgoing attacks against a List in the Items class?

        // To call, use                         Player player = new Player(x, y, hp, mp, atk);
        // To change or call specific stats     player.X = 0; player.HP = 100; etc.
        public Player(int x, int y, int hp, int mp, int atk)
        {
            X = x;
            Y = y;
            DX = 0;
            DY = 0;
            HP = hp;
            MP = mp;
            XP = 0;
            LEVEL = 1;
            ATTACK = atk;
            GOLD = 0;
        }

        // Add TakeDamage(int dmg)
        // Add XPGain(int xp)
        // Add LevelUp()

        public void DisplayStats()
        {
            Console.WriteLine($"PLAYER X:{X}, Y:{Y}, DX:{DX}, DY:{DY}, HP:{HP}, MP:{MP}, XP:{XP}, LEVEL:{LEVEL}, ATTACK:{ATTACK}, GOLD:{GOLD}");
        }
    }
}
