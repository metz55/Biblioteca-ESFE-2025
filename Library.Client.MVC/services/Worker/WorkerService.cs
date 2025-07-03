using Library.Client.MVC.services;
using System.Threading.Tasks;
using System.Timers;

public class WorkerService : BackgroundService
{
    private readonly LoanService _loanService;
    private readonly TaskManager _taskManager;
    private readonly System.Timers.Timer _dailyTimer;

    public WorkerService(LoanService loanService, TaskManager taskManager)
    {
        _loanService = loanService;
        _taskManager = taskManager;

        _dailyTimer = new System.Timers.Timer
        {
            AutoReset = true,
            Interval = GetNextExecutionInterval()
        };
        _dailyTimer.Elapsed += async (sender, args) =>
        {
            await ExecuteDailyEmailTasks();
            _dailyTimer.Interval = GetNextExecutionInterval(); // Actualiza para la siguiente ejecución
        };
        _dailyTimer.Start();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Worker Service is working...");
        while (!stoppingToken.IsCancellationRequested)
        {
            _taskManager.RunPendingTasks(stoppingToken);  // Ejecuta las tareas generales de TaskManager
            await Task.Delay(1000, stoppingToken);
        }
    }

    // Método para calcular el intervalo hasta las 7 AM
    private double GetNextExecutionInterval()
    {
        var now = DateTime.Now;
        var next7Am = now.Date.AddHours(7); // 7:00 AM del día actual
        if (now > next7Am)
        {
            next7Am = next7Am.AddDays(1); // Si ya pasaron las 7 AM, pasa al siguiente día
        }
        return (next7Am - now).TotalMilliseconds;
    }

    private async Task ExecuteDailyEmailTasks()
    {
        Console.WriteLine("Ejecutando tareas diarias de envío de correos a las 7 AM..." + DateTime.Now);

        await _loanService.CheckAndNotifyExpiredSoonLoans();  // Recordatorios de préstamos próximos a vencer
        await _loanService.CheckAndNotifyExpiredLoans();  // Notificaciones de préstamos vencidos
    }
}
