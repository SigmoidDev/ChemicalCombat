namespace Sigmoid.Enemies
{
    /// <summary>
    /// Anything that can deal damage and should be recorded for stat or achievement purposes
    /// </summary>
	public interface IDamageSource
	{
		public string DisplayName { get; }
	}

    /// <summary>
    /// An instance of damage that is to be dealt, including any information about it
    /// </summary>
    public readonly struct DamageContext
    {
        public readonly float damage;
        public readonly DamageType type;
        public readonly DamageCategory category;
        public readonly IDamageSource source;

        public DamageContext(float damage, DamageType type, DamageCategory category, IDamageSource source)
        {
            this.damage = damage;
            this.type = type;
            this.category = category;
            this.source = source;
        }
    }

    public enum DamageType
    {
        Physical,
        Glacial,
        Breeze,
        Fire,
        Electric,
        Acidic,
        Toxic,
        Light
    }

    public enum DamageCategory
    {
        Blunt,
        Explosion,
        DoT,
        Summon,
        Debuff
    }
}
