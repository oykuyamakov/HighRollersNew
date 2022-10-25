namespace CharImplementations.EnemyImplementations
{
    public class Enemy : Character
    {
        public override CharType GetCharType() => CharType.Enemy;
        public override void Die()
        {
            base.Die();
            this.gameObject.SetActive(false);
        }
    }
}
