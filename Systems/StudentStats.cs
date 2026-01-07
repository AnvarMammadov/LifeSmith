using System;

namespace LifeSmith.Systems
{
    /// <summary>
    /// Tracks Ayako's stats and progression
    /// </summary>
    public class StudentStats
    {
        // Academic performance (0-100)
        private int _grade;
        public int Grade 
        { 
            get => _grade;
            set => _grade = Math.Clamp(value, 0, 100);
        }

        // Trust and emotional connection (0-100)
        private int _affection;
        public int Affection 
        { 
            get => _affection;
            set => _affection = Math.Clamp(value, 0, 100);
        }

        // Romantic/sexual attraction (0-100)
        private int _lust;
        public int Lust 
        { 
            get => _lust;
            set => _lust = Math.Clamp(value, 0, 100);
        }

        // Current emotional state (0-100)
        private int _mood;
        public int Mood 
        { 
            get => _mood;
            set => _mood = Math.Clamp(value, 0, 100);
        }

        // Constructor - starts with poor grades and neutral stats
        public StudentStats()
        {
            Grade = 20;      // Failing
            Affection = 0;   // Scared of teacher
            Lust = 0;        // No romantic feelings
            Mood = 50;       // Neutral/nervous
        }

        // Helper methods for stat modification
        public void ModifyGrade(int delta)
        {
            Grade += delta;
        }

        public void ModifyAffection(int delta)
        {
            Affection += delta;
        }

        public void ModifyLust(int delta)
        {
            Lust += delta;
        }

        public void ModifyMood(int delta)
        {
            Mood += delta;
        }

        // Check if student is ready for specific interactions
        public bool CanAcceptAdvances => Affection >= 50 && Lust >= 30;
        public bool IsTrusting => Affection >= 60;
        public bool IsHappy => Mood >= 70;
        public bool IsPassing => Grade >= 60;
        
        // Ending checks
        public bool IsReadyForRomance => Grade >= 70 && Affection >= 80 && Lust >= 70;
        public bool IsReadyForGoodFriendEnding => Grade >= 80 && Affection >= 70 && Lust < 60;
        public bool IsReadyForSecretEnding => Grade >= 90 && Affection >= 90 && Lust >= 90;
    }
}
