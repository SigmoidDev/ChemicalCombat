using System.Collections;
using Sigmoid.Chemicals;
using Sigmoid.Players;
using UnityEngine;

namespace Sigmoid.Enemies
{
    public class SpiritAttacker : AttackerBase<SpiritParams>
    {
        public SpiritAttacker(Enemy enemy, SpiritParams parameters) : base(enemy, parameters)
        {
            maxRange = parameters.MaxRange;
            forceCoefficient = parameters.ForceCoefficient;

            affliction = ChemicalManager.GetRandom();
            ScriptableChemical chemical = ChemicalManager.Get(affliction);
            me.Damageable.ChemicalReactor.PermanentlyApply(affliction);

            int numReplacements = parameters.OriginalColours.Length;
            Color[] replacedColours = new Color[numReplacements];
            for(int i = 0; i < numReplacements; i++)
                replacedColours[i] = chemical.colours[i];

            me.Swapper.UpdateLUT(parameters.OriginalColours, replacedColours);
        }

        private readonly Chemical affliction;
        private readonly float maxRange;
        private readonly float forceCoefficient;

        public override void Update(IAttackable target, float deltaTime)
        {
            if(target is not PlayerAttackable player) return;

            Vector2 myPosition = me.transform.position;
            Vector2 deltaPosition = player.Position - myPosition;

            float distance = deltaPosition.magnitude;
            if(distance > maxRange) return;

            float force = forceCoefficient / distance;
            player.AddForce(force * Time.deltaTime * deltaPosition.normalized);
        }

        public override void Destroy()
        {
            me.Swapper.UpdateLUT(new Color[0], new Color[0]);
            me.Damageable.ChemicalReactor.PermanentlyApply(Chemical.None);
        }
    }
}
