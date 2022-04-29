using System.Text.RegularExpressions;

namespace Kata.AssemblyInterpreter;

public class AssemblerInterpreter
{
    private static Dictionary<string, int> _memory;
    private static Stack<int> _indexes;
    private static string _output;
    private static int _pointer;
    private static string[] _lines;
    private const string End = "end";

    private static (int, int) _conditions;

    public static string Interpret(string input)
    {
        _output = string.Empty;
        _pointer = 0;
        _memory = new Dictionary<string, int>();
        _indexes = new Stack<int>();
        _lines = null;

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
                    Msg(commandArgs);
                    break;
                case "call":
                    Call(commandArgs[0]);
                    break;
                case "cmp":
                    Cmp(commandArgs[0], commandArgs[1]);
                    break;
                case "jmp":
                    Jmp(commandArgs[0]);
                    break;
                case "jne":
                    Jne(commandArgs[0]);
                    break;
                case "je":
                    Je(commandArgs[0]);
                    break;
                case "jge":
                    Jge(commandArgs[0]);
                    break;
                case "jg":
                    Jg(commandArgs[0]);
                    break;
                case "jle":
                    Jle(commandArgs[0]);
                    break;
                case "jl":
                    Jl(commandArgs[0]);
                    break;
                case "ret":
                    Ret();
                    break;
                case "end":
                    return _output;
            }
        }

        return null;
    }

    private static int GetValue(string s) => int.TryParse(s, out var tmp) ? tmp : _memory[s];

    private static string GetStringValue(string s) => _memory.ContainsKey(s) ? _memory[s].ToString() : s;

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
        var commandArgs = Regex.Split(trimmedLine[index..], ",(?=(?:[^']*'[^']*')*[^']*$)");

        for (var i = 0; i < commandArgs.Length; i++)
        {
            commandArgs[i] = commandArgs[i].Trim(' ').Trim('\'');
        }

        return (commandName, commandArgs);
    }


    #region Instructions

    private static void Mov(string name, string value) => _memory[name] = GetValue(value);

    private static void Jnz(string val1, string val2) => _pointer += GetValue(val1) != 0 ? GetValue(val2) - 1 : 0;

    private static void Inc(string name) => _memory[name]++;

    private static void Dec(string name) => _memory[name]--;

    private static void Add(string name, string value) => _memory[name] += GetValue(value);

    private static void Sub(string name, string value) => _memory[name] -= GetValue(value);

    private static void Mul(string name, string value) => _memory[name] *= GetValue(value);

    private static void Div(string name, string value) => _memory[name] /= GetValue(value);

    private static void Msg(params string[] parameters) => _output += string.Join("", parameters.Select(x => GetStringValue(x).ToString()).ToArray());

    private static void Cmp(string val1, string val2) => _conditions = (GetValue(val1), GetValue(val2));

    private static void Jmp(string label)
    {
        var line = _lines.FirstOrDefault(l => l.StartsWith(label));
        var funcIndex = Array.IndexOf(_lines, line);
        _pointer = funcIndex;
    }

    private static void Jne(string label)
    {
        if (_conditions.Item1 != _conditions.Item2) Jmp(label);
    }

    private static void Je(string label)
    {
        if (_conditions.Item1 == _conditions.Item2) Jmp(label);
    }

    private static void Jge(string label)
    {
        if (_conditions.Item1 >= _conditions.Item2) Jmp(label);
    }

    private static void Jg(string label)
    {
        if (_conditions.Item1 > _conditions.Item2) Jmp(label);
    }

    private static void Jle(string label)
    {
        if (_conditions.Item1 <= _conditions.Item2) Jmp(label);
    }

    private static void Jl(string label)
    {
        if (_conditions.Item1 < _conditions.Item2) Jmp(label);
    }

    private static void Call(string label)
    {
        _indexes.Push(_pointer);
        var line = _lines.FirstOrDefault(l => l.StartsWith(label));
        var funcIndex = Array.IndexOf(_lines, line);
        _pointer = funcIndex;
    }

    private static void Ret()
    {
        _pointer = _indexes.Pop();
    }

    #endregion

    public static void Main()
    {
        var result = Interpret(
            "\nmov   a, 5\nmov   b, a\nmov   c, a\ncall  proc_fact\ncall  print\nend\n\nproc_fact:\n    dec   b\n    mul   c, b\n    cmp   b, 1\n    jne   proc_fact\n    ret\n\nprint:\n    msg   a, '! = ', c ; output text\n    ret\n");

        Console.WriteLine(result);
    }
}