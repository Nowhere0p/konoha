namespace Konoha.Models;

public class EmailModel
{
    public List<string> ToEmails { get; set; } // List of recipients
    public string? Subject { get; set; }
    public string Body { get; set; }
}
