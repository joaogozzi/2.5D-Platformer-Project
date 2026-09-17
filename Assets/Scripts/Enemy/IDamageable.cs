/// <summary>
/// Implementada por qualquer objeto que possa receber dano (player, inimigos, destrutíveis).
/// Assim o EnemyController pode causar dano sem precisar conhecer a classe concreta do player.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount);
}
