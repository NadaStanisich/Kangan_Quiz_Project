public class Questions
{
    public int Id { get; set; }
    public string Question { get; set; }
    public List<string> Options { get; set; }

    public Questions() {
        // This is required for Entity Framework
    }

    public Questions(int id, string question, List<string> options) {
        Id = id;
        Question = question;
        Options = options;
    }
}