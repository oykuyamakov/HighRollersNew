namespace CharImplementations.EnemyImplementations
{
    public class Enemy : Character
    {
        public override CharType GetCharType() => CharType.Enemy;

        public virtual void Setup(){}
        
        public override void Die()
        {
            base.Die();
            this.gameObject.SetActive(false);
        }
    }
}
