using System.Collections.Generic;
using LifeSmith.Content;
using LifeSmith.Core;

namespace LifeSmith.Systems
{
    public class JobManager
    {
        private static JobManager _instance;
        public static JobManager Instance => _instance ??= new JobManager();

        public List<House> AvailableJobs { get; private set; } = new();
        public House CurrentJob { get; private set; }

        private JobManager()
        {
            // Initialize with first job
            CreateFirstJob();
        }

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

        public void AcceptJob(House job)
        {
            CurrentJob = job;
            GameStateManager.Instance.CurrentJobHouseId = job.Id;
        }

        public void CompleteJob()
        {
            if (CurrentJob != null)
            {
                GameStateManager.Instance.CompletedJobs.Add(CurrentJob.Id);
                AvailableJobs.Remove(CurrentJob);
                CurrentJob = null;
            }
        }
    }
}
