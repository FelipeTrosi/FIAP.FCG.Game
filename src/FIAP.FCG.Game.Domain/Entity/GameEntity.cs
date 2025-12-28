namespace FIAP.FCG.Game.Domain.Entity
{
    public class GameEntity : EntityBase
    {
        public long Code { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public int PurchaseCount { get; set; }
        public float AverageRating { get; set; }
        public string Genre { get; set; } = null!;

    }
}
