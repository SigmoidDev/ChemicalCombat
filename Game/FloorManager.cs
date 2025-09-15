using System.Collections.Generic;
using System.Linq;
using Sigmoid.Generation;
using Sigmoid.Utilities;
using UnityEngine;

namespace Sigmoid.Game
{
	public class FloorManager : Singleton<FloorManager>
	{
        [SerializeField] private List<ScriptableFloor> possibleFloors;
        [SerializeField] private List<ScriptableSize> possibleSizes;

        public ScriptableFloor Floor { get; private set; }
        public int FloorNumber { get; private set; } = 1;

        private void Awake()
        {
            rng = new System.Random();
            recent = new Queue<int>();
            recent.Enqueue(0); 
        }

        private System.Random rng;
        private Queue<int> recent;

        /// <summary>
        /// Gets a new floor that has not been recently used
        /// </summary>
        /// <returns></returns>
        private int ChooseFloor()
        {
            if(FloorNumber <= 4) return Enqueue(FloorNumber - 1);

            int random = rng.Next(possibleFloors.Count);
            while(recent.Contains(random))
                random = rng.Next(possibleFloors.Count);

            return Enqueue(random);
        }

        /// <summary>
        /// Adds the new number to the queue, removing the oldest one if the queue has over 2 items
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private int Enqueue(int number)
        {
            recent.Enqueue(number);
            if(recent.Count > 2)
                recent.Dequeue();

            return number;
        }

        /// <summary>
        /// Randomly chooses a dungeon size based on a weighted random algorithm
        /// </summary>
        /// <returns></returns>
        private ScriptableSize ChooseSize()
        {
            if(FloorNumber == 1) return possibleSizes[0];

            Dictionary<ScriptableSize, float> weights = possibleSizes.ToDictionary(w => w, w => w.GetWeight(FloorNumber));
            float totalWeight = weights.Values.Sum();

            float randomValue = (float) rng.NextDouble() * totalWeight;
            foreach(KeyValuePair<ScriptableSize, float> weight in weights)
            {
                randomValue -= weight.Value;
                if(randomValue <= 0f) return weight.Key;
            }

            return null;
        }

        /// <summary>
        /// Increments the floor number and gets the next floor
        /// </summary>
        public (ScriptableFloor, ScriptableSize) NextFloor
        {
            get
            {
                Floor = possibleFloors[ChooseFloor()];
                ScriptableSize size = ChooseSize();
                FloorNumber++;
                return (Floor, size);
            }
        }

        public bool IsIndex(ScriptableFloor floor, int index) => floor == possibleFloors[index];
    }
}
