namespace Library.Client.MVC.services;

using System.Text.Json;
using Library.BusinessRules;
using Library.Client.MVC.Models;
using Library.Client.MVC.Models.DTO;
using Library.DataAccess.Domain;

public class LoanService
{
    private readonly BLLoans _loansBL;
    private readonly BLLoanDates _loansDatesBL;
    private readonly BLCategories _categoriesBL;
    private readonly EmailService _emailService;

    public LoanService(BLLoans loansBL, BLLoanDates loansDatesBL, BLCategories bLCategories, EmailService emailService)
    {
        _loansBL = loansBL;
        _loansDatesBL = loansDatesBL;
        _categoriesBL = bLCategories;
        _emailService = emailService;
    }

    /// <summary>
    /// Devuelve un estudiante mediante el Codigo de ESFE
    /// </summary>
    /// <param name="code"></param>
    /// <returns>Student</returns>
    public async Task<Student> GetStudentByCode(string code)
    {
        Student student = new Student();
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/code/{code}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
            }
        }
        return student;
    }

    public async Task<Student> GetStudentById(long id)
    {
        Student student = new Student();
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync($"http://190.242.151.49/esfeapi/ra/student/{id}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                student = JsonSerializer.Deserialize<Student>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
        return student ?? new Student();
    }

    public async Task<(List<Loans>, List<LoanDates2>)> GetStudentLoans(string codigo)
    {
        List<Loans> loans = new List<Loans>();
        List<LoanDates2> loansDates = new List<LoanDates2>();

        Student student = await GetStudentByCode(codigo);
        if(student == null)
        {
            return (new List<Loans>(), new List<LoanDates2>());
        }
        loans = await _loansBL.GetLoanByIdLender(student.Id);
        
        foreach (var l in loans)
        {
            Categories categories = await _categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = l.Books.ID_CATEGORY });
            loansDates.AddRange(await _loansDatesBL.GetLoanDatesByIdLoanAsync(new LoanDates { ID_LOAN = l.LOAN_ID }));
            loans[loans.IndexOf(l)].Books.Categories = categories;
        }

        return (loans, loansDates);
    }

    public async Task<(List<Loans>, List<LoanDates2>)> GetStudentExpiredLoans(Student student)
    {
        if(student == null || student.StudentCode == "")
        {
            Console.WriteLine("No student");
            return (new List<Loans>(), new List<LoanDates2>());
        }
        List<Loans> ExpiredLoans = await _loansBL.GetExpiredLoansByIdLenderAsync(new Loans(){ID_LENDER = student.Id});
        List<LoanDates2> ExpiredLoansDates = new List<LoanDates2>();

        if(ExpiredLoans.Count() == 0)
        {
            Console.WriteLine("No expired loans");
            return (new List<Loans>(), new List<LoanDates2>());
        }

        foreach(var loan in ExpiredLoans)
        {
            var date = await _loansDatesBL.GetExpiredDatesByIdLoanAsync(new Loans(){LOAN_ID = loan.LOAN_ID});
            Categories categories = await _categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = loan.Books.ID_CATEGORY });
            ExpiredLoansDates.AddRange(date);
            ExpiredLoans[ExpiredLoans.IndexOf(loan)].Books.Categories = categories;
            
        }
    
        return (ExpiredLoans, ExpiredLoansDates);
    }

    public async Task CheckAndNotifyExpiredLoans()
    {
        List<LoanDates2> loansDates = loansDates = await _loansDatesBL.GetExpiredDatesAsync();
        List<Loans> loans = await _loansBL.GetExpiredLoansAsync(loansDates);

        if(loans == null || loans.Count() == 0) 
        {
            Console.WriteLine("No hay avisos de prestamos vencidos");
            return;
        }
        try{
            foreach (var loan in loans)
            {
                var date = loansDates.Find(l=>l.ID_LOAN == loan.LOAN_ID).END_DATE;
                var dias = (date - DateTime.Now).Days;
                var horas = (date - DateTime.Now).Hours;
                Student student = await GetStudentById(loan.ID_LENDER);
                var emailMessage = "";
                var mora = Math.Abs(dias) * 0.5;
                string email = student.StudentCode.ToLower() +  "@esfe.agape.edu.sv";
                if(student == null)
                {
                    Console.WriteLine("No se encontró ningun estudiante!");
                    return;
                }
                if(dias <= 0)
                {
                    if(dias == 0 && horas < 0)
                    {
                        emailMessage = $"{student.StudentName}, la devolución del prestamo del libro: \"{loan.Books.TITLE}\" se vence hoy, recuerda que si no deseas cancelar mora, debes entregar el libro a tiempo.";
                    }
                    else if(dias == -1)
                    {
                        emailMessage = $"{student.StudentName}, el plazo de devolución de su libro \"{loan.Books.TITLE}\" venció ayer. La mora ha aumentado $0.50 centavos. Mora a pagar: ${mora}";
                    }
                    else
                    {
                        var d = Math.Abs(dias) != 0 ? "hace "+Math.Abs(dias) + " días." : "Ayer.";
                        emailMessage = $"{student.StudentName}, el plazo de devolución de su libro \"{loan.Books.TITLE}\" venció {d} La mora ha aumentado $0.50 centavos. Mora a pagar: ${mora}";
                    }
                }
                 Console.WriteLine(emailMessage);
                EmailDTO emailDto = new EmailDTO
                {
                    Message = emailMessage,
                    Subject = "Aviso de expiración del préstamo de libro",
                    ReceptorName = student.StudentName,
                    ReceptorEmail = email,
                    IsLoanReminder = true
                };
                await _emailService.SendEmailAsync(emailDto);
            }
        }
        catch(Exception e)
        {
             Console.WriteLine("Error al enviar la advertencia de los correos " + e.Message);
        }
    }

    public async Task CheckAndNotifyExpiredSoonLoans()
    {
        List<LoanDates2> loansDates = loansDates = await _loansDatesBL.GetDatesToExpireSoonAsync();
        List<Loans> loans = await _loansBL.GetExpiredLoansAsync(loansDates);

        if(loans == null || loans.Count() == 0) 
        {
            Console.WriteLine("No hay avisos de prestamos por vencer");
            return;
        }
        try{
            foreach (var loan in loans)
            {
                var date = loansDates.Find(l=>l.ID_LOAN == loan.LOAN_ID).END_DATE;
                var dias = (date - DateTime.Now).Days;
                var diastotal = (date - DateTime.Now).TotalDays;
                var horas = (date - DateTime.Now).Hours;
                Student student = await GetStudentById(loan.ID_LENDER);
                var emailMessage = "";
                String email = student.StudentCode.ToLower() +  "@esfe.agape.edu.sv";
                if(student == null)
                {
                    Console.WriteLine("No se encontró ningun estudiante!");
                    return;
                }
                if(diastotal == 1)
                {
                    emailMessage = $"{student.StudentName}, la devolución de tu libro prestado, \"{loan.Books.TITLE}\" debes realizarla mañana.";
                }
                else
                {
                    emailMessage = $"{student.StudentName}, la devolución de tu libro prestado, \"{loan.Books.TITLE}\" debes realizarla dentro de {dias + 1} días.";
                }
                Console.WriteLine(emailMessage);
                EmailDTO emailDto = new EmailDTO
                {
                    Message = emailMessage,
                    Subject = "Recordatorio de devolución de libro",
                    ReceptorName = student.StudentName,
                    ReceptorEmail = email,
                    IsLoanReminder = true
                };
                await _emailService.SendEmailAsync(emailDto);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error al envia los recordatorios de los correos " + e.Message);
        }

    }

    public async Task SendEmailLoanCreated(Loans pLoan, LoanDates loanDates)
    {
        try{
            Student student = await GetStudentById(pLoan.ID_LENDER);
            Categories categories = await _categoriesBL.GetCategoriesByIdAsync(new Categories { CATEGORY_ID = pLoan.Books.ID_CATEGORY });
            var BookTitle = pLoan.Books.TITLE;
            var subject = "Préstamo de Libro";
            var message = $"Hola {student.StudentName}, tu préstamo del libro \"{BookTitle}\", se ha efectuado de manera exitosa. Esta es la información de tu préstamo:";
            var body = $@"
                <html>
                    <head>
                        <style>
                            body {{
                                background-color: #f4f4f9;
                                font-family: Arial, sans-serif;
                                color: #333;
                            }}
                            .email-container {{
                                background-color: #fff;
                                padding: 20px;
                                margin: 0 auto;
                                width: 80%;
                                max-width: 600px;
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                border-radius: 10px;
                            }}
                            .header-image {{
                                text-align: center;
                                margin-bottom: 20px;
                            }}
                            .header-image img {{
                                width: 100%;
                                height: auto;
                                border-radius: 10px 10px 0 0;
                            }}
                            h1 {{
                                color: #1b6ec2;
                                font-size: 24px;
                                margin-bottom: 10px;
                            }}
                            h3 {{
                                color: #555;
                                font-size: 16px;
                                margin-bottom: 10px;
                            }}
                            p {{
                                font-size: 16px;
                                line-height: 1.6;
                            }}
                            hr {{
                                border: 0;
                                height: 1px;
                                background-color: #ddd;
                                margin: 20px 0;
                            }}
                            .warning {{
                                color: #c82333;
                                font-size: 18px;
                                font-weight: bold;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='header-image'>
                                <img src='https://i.postimg.cc/Sx3vRhh1/Imagen4.png' alt='Header Image'/>
                            </div>
                            <h1>Estimado/a {student.StudentName} {student.StudentLastName}</h1>
                            <h2>Información de Préstamo de Libro</h2>
                            <p>{message}</p>
                            <br/>
                            <h3>Nombre del prestatario: {student.StudentName} {student.StudentLastName}</h3>
                            <h3>Código: {student.StudentCode}</h3>
                            <hr/>
                            <h3>Título del Libro: {BookTitle}</h3>
                            <h3>Categoría: {categories.CATEGORY_NAME}</h3>
                            <hr/>
                            <h3>Prestado el: {pLoan.REGISTRATION_DATE.ToShortDateString()}</h3>
                            <h3>Debes devolverlo el: {loanDates.END_DATE.ToShortDateString()} | {((loanDates.END_DATE - DateTime.Now).Days != 0 ? "en "+(loanDates.END_DATE - DateTime.Now).Days + " días" : "Mañana")}</h3>
                            <hr/>
                            <p class='warning'>
                                Recuerda que si no entregas el libro a tiempo, la mora aumentará $0.50 centavos cada día de retraso.
                            </p>
                        </div>
                    </body>
                </html>";

                EmailDTO emailDTO = new EmailDTO
                {
                    Subject = subject,
                    ReceptorName = student.StudentName,
                    ReceptorEmail = student.StudentCode.ToLower() +  "@esfe.agape.edu.sv",
                    EmailBody = body,
                    IsLoanReminder = false
                };

                await _emailService.SendEmailAsync(emailDTO);
        }
        catch(Exception e)
        {
            Console.WriteLine("Error al construir el email de creación de préstamo " + e.Message);
            return;
        }


    

    }


}


