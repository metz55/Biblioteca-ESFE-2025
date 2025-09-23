using System.Globalization;
using Library.Client.MVC.Models.DTO;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text; 

public class EmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration configuration)
    {
        _config = configuration;
    }

    private string GetReminderEmailBody(EmailDTO emailDto)
    {
        var subtitle = emailDto.Subject.Contains("Recordatorio") || emailDto.Message.Contains("hoy")?"El préstamo de tu libro vencerá pronto!" : "El préstamo de tu libro se ha vencido";
        var color = emailDto.Subject.Contains("Recordatorio") ? "#3498db" : "#c82333";
        //Capitalizar el nombre del estudiante
        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        string formattedName = textInfo.ToTitleCase(emailDto.ReceptorName.ToLower());

        var body = $@"
            <html>
                <body style='background-color: white'>
                    <div style='text-align:center;'>
                        <img src='https://i.postimg.cc/Sx3vRhh1/Imagen4.png' alt='Header Image' width='600' height='150'/>
                    </div>
                    <h1 style='color:{color};'>Estimado/a {formattedName}</h1>
                    <h2>{subtitle}</h2>
                    <p style='font-size:16px'>{emailDto.Message}</p>
                </body>
            </html>";

        return body;
    }

    private string GetEmailBody(EmailDTO emailDto)
    {
        var subtitle = emailDto.Subtitle;
        var color = "#3498db";
        //Capitalizar el nombre
        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        string formattedName = textInfo.ToTitleCase(emailDto.ReceptorName.ToLower());
        var body =  "";
        if(emailDto.EmailBody != "")
        {
            body = emailDto.EmailBody;
        }
        else
        {
            body = $@"
                    <html>
                        <body style='background-color: white'>
                            <div style='text-align:center;'>
                                <img src='https://i.postimg.cc/Sx3vRhh1/Imagen4.png' alt='Header Image' width='600' height='150'/>
                            </div>
                            <h1 style='color:{color};'>Estimado/a {formattedName}</h1>
                            <h2>{subtitle}</h2>
                            <p style='font-size:16px'>{emailDto.Message}</p>
                        </body>
                    </html>";
        }
         

        return body;
    }

    public async Task<bool> SendEmailAsync(EmailDTO emailDto)
    {
        if (string.IsNullOrWhiteSpace(emailDto.ReceptorEmail))
        {
            Console.WriteLine("Email receptor es inválido o está vacío.");
            return false;
        }

        try
        {
            var Host = _config.GetSection("Email:Host").Value;
            int Port = Convert.ToInt32(_config.GetSection("Email:Port").Value);
            var UserName = _config.GetSection("Email:UserName").Value;
            var Account = _config.GetSection("Email:Account").Value;
            var Password = _config.GetSection("Email:Password").Value;

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(UserName, Account));
            email.To.Add(MailboxAddress.Parse(emailDto.ReceptorEmail));
            email.Subject = emailDto.Subject;

            var htmlBody = emailDto.IsLoanReminder ? GetReminderEmailBody(emailDto) : GetEmailBody(emailDto);

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlBody
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(Account, Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error trying to send email: " + e);
            return false;
        }
    }


}
