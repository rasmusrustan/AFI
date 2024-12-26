namespace BattleShits.Models
{
    public class BattleField2
    {
        public int[,] P1Board { get; set; } // Spelbräde (0 = tomt, 1 = skepp, 2 = träff, 3 = miss)
        public int[,] P2Board { get; set; }
    }
}
