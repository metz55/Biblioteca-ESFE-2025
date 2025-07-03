namespace Library.Client.MVC.services
{
    public class TaskManager
    {
        private readonly Queue<Func<CancellationToken, Task>> _taskQueue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void EnqueueTask(Func<CancellationToken, Task> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            lock (_taskQueue)
            {
                _taskQueue.Enqueue(task);
                _signal.Release();  // Libera el semáforo cuando se encola una tarea
            }
        }

        public async Task<Func<CancellationToken, Task>> DequeueTaskAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);  // Espera hasta que haya una tarea disponible
            lock (_taskQueue)
            {
                return _taskQueue.Dequeue();  // Devuelve la tarea desde la cola
            }
        }

        public async Task RunPendingTasks(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var task = await DequeueTaskAsync(cancellationToken);
                    await task(cancellationToken); 
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error ejecutando la tarea: {ex.Message}");
                }
            }
        }
    }
}

