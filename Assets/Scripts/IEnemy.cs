/// <summary>
/// An IGridEntity that also takes a turn each round.
/// </summary>
public interface IEnemy : IGridEntity
{
    /// <summary>Called once per enemy turn.</summary>
    void TakeTurn();
}
