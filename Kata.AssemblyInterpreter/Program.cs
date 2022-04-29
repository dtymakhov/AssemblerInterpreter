using System.Text.RegularExpressions;

namespace Kata.AssemblyInterpreter;

public class AssemblerInterpreter
{
    private static readonly Dictionary<string, int> Memory = new Dictionary<string, int>();
    private static readonly Stack<int> Indexes = new Stack<int>();
    private static string _output = string.Empty;
    private static int _pointer = 0;
    private static string[] _lines;
    private const string End = "end";

    public static string? Interpret(string input)
    {
        input = RemoveComments(input);

        if (FindProgramEnd(input) == -1) return null;

        _lines = input.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        for (; _pointer < _lines.Length; _pointer++)
        {
            var (commandName, commandArgs) = ParseLine(_lines[_pointer]);

            switch (commandName)
            {
                case "mov":
                    Mov(commandArgs[0], commandArgs[1]);
                    break;
                case "jnz":
                    Jnz(commandArgs[0], commandArgs[1]);
                    break;
                case "inc":
                    Inc(commandArgs[0]);
                    break;
                case "dec":
                    Dec(commandArgs[0]);
                    break;
                case "add":
                    Add(commandArgs[0], commandArgs[1]);
                    break;
                case "sub":
                    Sub(commandArgs[0], commandArgs[1]);
                    break;
                case "mul":
                    Mul(commandArgs[0], commandArgs[1]);
                    break;
                case "div":
                    Div(commandArgs[0], commandArgs[1]);
                    break;
                case "msg":
                    Msg(commandArgs[0], commandArgs.Skip(1).ToArray());
                    break;
                case "call":
                    Call(commandArgs[0]);
                    break;
                case "ret":
                    Ret();
                    break;
                case "end":
                    return _output;
            }
        }

        return _output;
    }

    private static int GetValue(string s) => int.TryParse(s, out var tmp) ? tmp : Memory[s];

    #region Instructions

    private static void Mov(string name, string value) => Memory[name] = GetValue(value);

    private static void Jnz(string val1, string val2) => _pointer += GetValue(val1) != 0 ? GetValue(val2) - 1 : 0;

    private static void Inc(string name) => Memory[name]++;

    private static void Dec(string name) => Memory[name]--;

    private static void Add(string name, string value) => Memory[name] += GetValue(value);

    private static void Sub(string name, string value) => Memory[name] -= GetValue(value);

    private static void Mul(string name, string value) => Memory[name] *= GetValue(value);

    private static void Div(string name, string value) => Memory[name] /= GetValue(value);

    private static void Msg(string msg, params string[] parameters)
    {
        var args = new List<string> {msg};
        args.AddRange(parameters.Select(x => GetValue(x).ToString()).ToArray());

        _output += string.Join(' ', args);
    }

    private static void Call(string label)
    {
        Indexes.Push(_pointer);
        var line = _lines.FirstOrDefault(l => l.StartsWith(label));
        var funcIndex = Array.IndexOf(_lines, line);
        _pointer = funcIndex;
    }

    private static void Ret()
    {
        _pointer = Indexes.Pop();
    }

    private static string RemoveComments(string input)
    {
        var output = Regex.Replace(input, @";(.*?)\r?\n", "\n");

        return output;
    }

    private static int FindProgramEnd(string input)
    {
        return input.IndexOf(End, StringComparison.Ordinal);
    }

    private static (string, string[]) ParseLine(string line)
    {
        var trimmedLine = line.Trim();
        var index = trimmedLine.IndexOf(" ", StringComparison.Ordinal);

        if (index == -1) return (trimmedLine, Array.Empty<string>());

        var commandName = trimmedLine[..index];
        var commandArgs = trimmedLine[index..].Split(',');

        for (var i = 0; i < commandArgs.Length; i++)
        {
            if (commandArgs[i].Contains('\'', StringComparison.Ordinal))
            {
                commandArgs[i] = commandArgs[i].Trim('\'', ' ');
            }
            else
            {
                commandArgs[i] = commandArgs[i].Trim();
            }
        }

        return (commandName, commandArgs);
    }

    #endregion

    public static void Main()
    {
        var result = Interpret(
            "\n; My first program\nmov  a, 5\ninc  a\ncall function\nmsg  '(5+1)/2 = ', a    ; output message\nend\n\nfunction:\n    div  a, 2\n    ret\n");

        Console.WriteLine(result);
    }
}