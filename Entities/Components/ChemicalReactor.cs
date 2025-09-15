using System.Collections.Generic;
using System;
using Sigmoid.Chemicals;
using Sigmoid.Reactions;
using UnityEngine;

namespace Sigmoid.Enemies
{
	public class ChemicalReactor : MonoBehaviour
	{
        [field: SerializeField] public Damageable Damageable { get; private set; }
		[field: SerializeField] public StatusBelt StatusBelt { get; private set; }

        private Chemical permanent;
        public void PermanentlyApply(Chemical chemical)
        {
            permanent = chemical;
            chemicalInflictions.Add(chemical);
        }

        private void Awake() => Damageable.OnDeath += ClearBelt; //unsubscribed automatically on unload of current Scene
        public void Initialise()
        {
            ClearBelt(); //i don't THINK this is necessary but i'd rather have it just to be safe
			chemicalInflictions = new List<Chemical>();
            permanent = Chemical.None;
        }

        private List<Chemical> chemicalInflictions;
		public void InflictChemical(Chemical chemical, Action<Reaction> callback)
		{
			//Only allow a maximum of 5 elements at once
			if(chemicalInflictions.Count >= 5)
			{
                for(int i = 0; i < 5; i++)
                {
                    if(chemicalInflictions[i] == permanent) continue;

                    Chemical firstChemical = chemicalInflictions[i];
                    RemoveBelt(firstChemical);
                    chemicalInflictions.Remove(firstChemical);
                    break;
                }
			}

            (Reaction reaction, List<Chemical> involvedChemicals) = FindValidReaction(chemical);
			if(reaction == null)
			{
				chemicalInflictions.Add(chemical);
				AddBelt(chemical);
			}
            else
            {
                foreach(Chemical involved in involvedChemicals)
                {
                    //makes it so that the permanently applied chemical isn't removed
                    if(involved == permanent && NumberOf(involved) <= 1) continue;

                    chemicalInflictions.Remove(involved);
                    RemoveBelt(involved);
                }
                callback?.Invoke(reaction);
            }
        }

        /// <summary>
        /// Finds the first valid reaction using some weird bitwise wizardry that ChatGPT told me about
        /// </summary>
        /// <param name="addition"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private (Reaction, List<Chemical>) FindValidReaction(Chemical addition)
        {
            int count = chemicalInflictions.Count;
            int total = 1 << count;

            for(int length = count; length >= 1; length--)
            {
                for(int mask = 1; mask < total; mask++)
                {
                    if(CountBits(mask) != length) continue;

                    List<Chemical> subset = new List<Chemical>(length + 1);
                    for(int i = 0; i < count; i++)
                    {
                        if((mask & (1 << i)) != 0)
                            subset.Add(chemicalInflictions[i]);
                    }

                    subset.Add(addition);
                    Reaction reaction = CombinationVerifier.Try(new Combination(subset));
                    if(reaction != null) return (reaction, subset);
                }
            }

            return (null, null);
        }

        /// <summary>
        /// Counts the number of 1s in a bitmask
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int CountBits(int value)
        {
            int count = 0;
            while(value != 0)
            {
                //Checks the final bit
                count += value & 1;
                //Shifts all bits 1 to the right, thus moving onto the next one
                value >>= 1;
            }
            return count;
        }

        /// <summary>
        /// Returns how many instances of this chemical are present in the composition
        /// </summary>
        /// <param name="chemical"></param>
        /// <returns></returns>
        private int NumberOf(Chemical chemical)
        {
            int count = 0;
            foreach(Chemical infliction in chemicalInflictions)
                if(chemical == infliction) count++;
            
            return count;
        }

        private void AddBelt(Chemical chemical){ if(StatusBelt != null) StatusBelt.Add(chemical); }
        private void RemoveBelt(Chemical chemical){ if(StatusBelt != null) StatusBelt.Remove(chemical); }
        private void ClearBelt(){ if(StatusBelt != null) StatusBelt.Clear(); }
	}
}
