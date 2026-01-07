using System.Collections.Generic;
// using LifeSmith.Content; // Removed - House.cs deleted for TryTeach
using LifeSmith.Core;

namespace LifeSmith.Systems
{
    public class JobManager
    {
        private static JobManager _instance;
        public static JobManager Instance => _instance ??= new JobManager();

        // TODO: Refactor for TryTeach - Remove job system
        // public List<House> AvailableJobs { get; private set; } = new();
        // public House CurrentJob { get; private set; }

        private JobManager()
        {
            // TODO: Remove for TryTeach - No job system
            // CreateFirstJob();
        }

        // TODO: Remove for TryTeach
        /*
        private void CreateFirstJob()
        {
            var firstHouse = new House(
                id: "house_001",
                name: "Emily's House",
                address: "123 Oak Street",
                lockDifficulty: 1
            )
            {
                ResidentName = "Emily",
                ResidentDescription = "A young woman living alone",
                MoneyReward = 100,
                ValuableItems = new List<string> { "Watch", "Necklace" }
            };

            AvailableJobs.Add(firstHouse);
        }
        */

        // TODO: Remove for TryTeach
        /*
        public void AcceptJob(House job)
        {
            CurrentJob = job;
            GameStateManager.Instance.CurrentJobHouseId = job.Id;
        }
        */

        // TODO: Remove for TryTeach
        /*
        public void CompleteJob()
        {
            if (CurrentJob != null)
            {
                GameStateManager.Instance.CompletedJobs.Add(CurrentJob.Id);
                AvailableJobs.Remove(CurrentJob);
                CurrentJob = null;
            }
        }
        */
    }
}
