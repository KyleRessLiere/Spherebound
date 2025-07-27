/// <summary>
/// An IGridEntity that also takes a turn each round.
/// </summary>
public interface IEnemy : IGridEntity
{
    void TakeTurn();
    void TakeDamage(int dmg); 
}
