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

        // Suspicion level - how suspicious Ayako is of teacher's intentions (0-100)
        private int _suspicion;
        public int Suspicion 
        { 
            get => _suspicion;
            set => _suspicion = Math.Clamp(value, 0, 100);
        }

        // Money for buying gifts/items
        public int Money { get; set; }

        // Constructor - starts with poor grades and neutral stats
        public StudentStats()
        {
            Grade = 20;      // Failing
            Affection = 0;   // Scared of teacher
            Lust = 0;        // No romantic feelings
            Suspicion = 30;  // Somewhat suspicious initially
            Money = 1000;    // Starting money (¥1000)
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

        public void ModifySuspicion(int delta)
        {
            Suspicion += delta;
        }

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        public bool SpendMoney(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                return true;
            }
            return false;
        }

        // Check if student is ready for specific interactions
        public bool CanAcceptAdvances => Affection >= 50 && Lust >= 30 && Suspicion < 70;
        public bool IsTrusting => Affection >= 60 && Suspicion < 50;
        public bool IsSuspicious => Suspicion >= 70;
        public bool IsPassing => Grade >= 60;
        
        // Ending checks
        public bool IsReadyForRomance => Grade >= 70 && Affection >= 80 && Lust >= 70 && Suspicion < 40;
        public bool IsReadyForGoodFriendEnding => Grade >= 80 && Affection >= 70 && Lust < 60 && Suspicion < 50;
        public bool IsReadyForSecretEnding => Grade >= 90 && Affection >= 90 && Lust >= 90 && Suspicion < 30;
        public bool IsReadyForBadEnding => Suspicion >= 80 || Grade < 30; // Got caught or failed completely
    }
}
