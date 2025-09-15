using System.Collections.Generic;
using UnityEngine;

namespace Sigmoid.Enemies
{
    /// <summary>
    /// Allows for multiple different movement types to be switched between by index
    /// </summary>
    public class CompositeMovement : MovementBase<CompositeMovementParams>
    {
        public CompositeMovement(Enemy enemy, CompositeMovementParams parameters) : base(enemy, parameters)
        {
            movements = new List<IMovement>();
            foreach(MovementParams subMovement in parameters.Params)
                movements.Add(subMovement.CreateModule(enemy));
        }

        public override void Initialise(){}

        private readonly List<IMovement> movements;
        protected int currentIndex;
        public IMovement CurrentMovement => movements[currentIndex];

        public IMovement GetMode(int index) => movements[index];
        public void SetMode(int index)
        {
            currentIndex = index;
            CurrentMovement.Initialise();
        }

        public override void Update(Vector2 targetPosition, float deltaTime) => CurrentMovement?.Update(targetPosition, deltaTime);
    }
}
