using System.Collections;

namespace Utils;

public class ConsolePrompt
{
    public string PromptText { get; set; }
    public Action MethodToRun { get; set; }

    public ConsolePrompt()
    {

    }

    public ConsolePrompt(string promptText, Action methodToRun)
    {
        PromptText = promptText;
        MethodToRun = methodToRun;
    }

}

public class ConsolePromptList : IEnumerable
{
    private readonly List<ConsolePrompt> list = [];

    public int Length => list.Count;

    public void Add(string promptText, Action methodToRun)
    {
        list.Add(new ConsolePrompt(promptText, methodToRun));
    }

    public IEnumerator GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public ConsolePrompt this[int index] => list[index];

    #region Helpers
    public void RunConsolePromptInfinitely()
    {
        new ConsolePromptListRunner(this).RunConsolePromptInfinitely();
    }

    public void RunConsolePromptOnce()
    {
        new ConsolePromptListRunner(this).RunConsolePromptOnce();
    }

    #endregion

}

public class ConsolePromptListRunner(ConsolePromptList promptList)
{
    private readonly ConsolePromptList promptList = promptList;

    public void RunConsolePromptInfinitely()
    {
        while (RunConsolePrompt())
        {
        }

        Aftermath();
    }

    public void RunConsolePromptOnce()
    {
        RunConsolePrompt();
        Aftermath();
    }

    private void Aftermath()
    {
        Console.WriteLine("Done. The next keypress will close this window.");
        Console.ReadKey();
    }

    private readonly bool greaterThanNine = promptList.Length > 8; //8 +1

    private string GreaterThanNineMessage => greaterThanNine ? " and press enter" : "";

    private string Message => $"Select one of the following actions{GreaterThanNineMessage}:";

    private bool RunConsolePrompt()
    {
        Console.WriteLine(Message);

        int i;

        for (i = 1; i <= promptList.Length; i++)
        {
            ConsolePrompt prompt = promptList[i - 1];
            Console.WriteLine($"\t{i} - " + prompt.PromptText);
        }

        Console.WriteLine($"\t{i} or any other key - exit");

        Console.WriteLine();
        Console.Write("Your choice: ");

        string choice = greaterThanNine ? Console.ReadLine() : Console.ReadKey().KeyChar.ToString();

        Console.WriteLine();


        if (!Microsoft.VisualBasic.Information.IsNumeric(choice))
        {
            return false;
        }

        var choiceNum = Convert.ToInt32(choice);

        if (choiceNum == promptList.Length + 1)
        {
            return false;
        }

        if (choiceNum < 1 || choiceNum > promptList.Length + 1)
        {
            return false;
        }

        var action = promptList[choiceNum - 1].MethodToRun;
        action();

        return true;

    }

}

