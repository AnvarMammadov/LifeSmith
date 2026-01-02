using Microsoft.Xna.Framework;
using System;

namespace LifeSmith.Minigames
{
    /// <summary>
    /// Represents a single pin in the lock picking minigame
    /// </summary>
    public class LockPin
    {
        public float CurrentPosition { get; set; } // 0.0 to 1.0 (left to right)
        public float TargetPosition { get; private set; } // The "sweet spot"
        public float Tolerance { get; private set; } // How close you need to be
        public bool IsLocked { get; set; } // Has this pin been successfully locked?
        
        private Random _random;

        public LockPin(Random random, float tolerance = 0.05f)
        {
            _random = random;
            Tolerance = tolerance;
            
            // Random target position between 0.1 and 0.9
            TargetPosition = 0.1f + (float)_random.NextDouble() * 0.8f;
            
            // Start at random position
            CurrentPosition = (float)_random.NextDouble();
            
            IsLocked = false;
        }

        public void UpdatePosition(float delta)
        {
            CurrentPosition += delta;
            
            // Clamp between 0 and 1
            if (CurrentPosition < 0) CurrentPosition = 0;
            if (CurrentPosition > 1) CurrentPosition = 1;
        }

        public bool IsInSweetSpot()
        {
            return Math.Abs(CurrentPosition - TargetPosition) <= Tolerance;
        }

        public float GetDistanceFromTarget()
        {
            return Math.Abs(CurrentPosition - TargetPosition);
        }

        // Visual feedback - how much to shake based on proximity
        public float GetShakeIntensity()
        {
            float distance = GetDistanceFromTarget();
            
            if (distance <= Tolerance)
                return 0f; // No shake when in sweet spot
            
            if (distance <= Tolerance * 3)
                return 0.3f; // Slight shake when very close
            
            if (distance <= Tolerance * 6)
                return 0.6f; // Medium shake when close
            
            return 1.0f; // Strong shake when far
        }
    }
}
