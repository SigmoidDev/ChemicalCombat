namespace Sigmoid.Enemies
{
    /// <summary>
    /// Same as DirectMovement, but without pathfinding
    /// </summary>
	public class PhaseMovement : DirectMovement
	{
        public PhaseMovement(Enemy enemy, PhaseParams parameters) : base(enemy, parameters){}

        //I just got these values from the Debug inspector
        public override void Initialise()
        {
            me.Agent.agentTypeID = 1479372276;
            base.Initialise();
        }

        public override void Destroy()
        {
            me.Agent.agentTypeID = 0;
            base.Destroy();
        }
    }
}
