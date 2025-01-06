namespace BattleShits.Models
{
    public class Board
    {
        public Titanic Titanic { get; set; }
        public LongShip LongShip1 { get; set; }
        public LongShip LongShip2 { get; set; }
        public TrippleShip TrippleShip1 { get; set; }
        public TrippleShip TrippleShip2 { get; set; }
        public TrippleShip TrippleShip3 { get; set; }
        public DoubleShip DoubleShip1 { get; set; }
        public DoubleShip DoubleShip2 { get; set; }
        public DoubleShip DoubleShip3 { get; set; }
        public DoubleShip DoubleShip4 { get; set; }
        public List<Shot> Shots { get; set; }
    }
}
