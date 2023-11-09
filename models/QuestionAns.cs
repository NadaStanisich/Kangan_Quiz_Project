public class QuestionAns
{
    public int Id { get; set; }
    public string Question { get; set; }
    public List<string> Options { get; set; }

    public string CorrectAnswer { get; set; }

    public QuestionAns() {
        // This is required for Entity Framework
    }

    public QuestionAns(int id, string question, List<string> options) {
        Id = id;
        Question = question;
        Options = options;
        CorrectAnswer = options[3];
    }

}