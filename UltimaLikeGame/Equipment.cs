

namespace UltimaLike
{
    class Weapons
    {
        public string NAME { get; set; }
        public int DAMAGE { get; set; }
        public int RANGE { get; set; }
        public int COST { get; set; }
        public float DROP_RATE { get; set; }

        public Weapons(string name, int dmg, int range, int cost, float dropRate)
        {
            NAME = name;
            DAMAGE = dmg;
            RANGE = range;
            COST = cost;
            DROP_RATE = dropRate;
        }

        // public static List<Weapons> AddWeapon(string name, int dmg, int range, int cost, float dropRate)
        // {
        //     List<Weapons> newWeapon = new List<Weapons>();
        //     newWeapon.Add()

        //     return 0;
        // }
    }

    class Armor
    {
        public string NAME { get; set; }
        public int AP { get; set; }
        public int COST { get; set; }
        public float DROP_RATE { get; set; }

        public Armor(string name, int ap, int cost, float dropRate)
        {
            NAME = name;
            AP = ap;
            COST = cost;
            DROP_RATE = dropRate;
        }
    }

    class Wearable
    {
        public string NAME { get; set; }
        public int DAMAGE { get; set; }
        public int AP { get; set; }
        public int HP_BUFF { get; set; }
        public int MP_BUFF { get; set; }
        public int COST { get; set; }
        public float DROP_RATE { get; set; }

        public Wearable(string name, int dmg, int ap, int hpBuff, int mpBuff, int cost, float dropRate)
        {
            NAME = name;
            DAMAGE = dmg;
            AP = ap;
            HP_BUFF = hpBuff;
            MP_BUFF = mpBuff;
            COST = cost;
            DROP_RATE = dropRate;
        }
    }
}

