namespace Library.Client.MVC.Models
{
    public class Student
    {
        public long Id { get; set; }
        public int CareerId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;
        public string CellPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public virtual Career Career { get; set; }
    }
}
