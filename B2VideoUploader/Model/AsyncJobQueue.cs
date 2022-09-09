using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public class AsyncJobQueue
    {
        Queue<Action> jobQueue;

        public AsyncJobQueue()
        {
            jobQueue = new Queue<Action>();
        }

        public Task Enqueue(IEnumerable<Action> jobs)
        {
            foreach (Action job in jobs)
            {
                jobQueue.Enqueue(job);
            }

            return Task.Run(() =>
            {
                lock (jobQueue)
                {
                    ProcessQueue();
                }
            });
        }
        public Task Enqueue(Action job)
        {
            return Enqueue(new Action[1] { job });
        }

        private void ProcessQueue()
        {
            while (jobQueue.Any()) {
                var job = jobQueue.Dequeue();
                job();
            }
        }

        public void WaitForJobs()
        {
            Task t = Task.Run(() =>
            {
                lock (jobQueue) { };
                while (jobQueue.Any()) { };
            });
        }

    }
}
