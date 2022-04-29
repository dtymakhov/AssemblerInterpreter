using System.Text.RegularExpressions;

namespace Kata.AssemblyInterpreter;

public class AssemblerInterpreter
{
    private static Dictionary<string, int> _memory = new Dictionary<string, int>();
    private static string _output = string.Empty;
    private static int _pointer = 0;

    public static string Interpret(string input)
    {
        input = RemoveCommentsAndEmptyLines(input);
        // Your code here!
        return _output;
    }

    private static int GetValue(string s) => int.TryParse(s, out var tmp) ? tmp : _memory[s];

    #region Instructions

    private static void Mov(string name, string value) => _memory[name] = GetValue(value);

    private static void Jnz(string val1, string val2) => _pointer += GetValue(val1) != 0 ? GetValue(val2) - 1 : 0;

    private static void Inc(string name) => _memory[name]++;

    private static void Dec(string name) => _memory[name]--;

    private static void Add(string name, string value) => _memory[name] += GetValue(value);

    private static void Sub(string name, string value) => _memory[name] -= GetValue(value);

    private static void Mul(string name, string value) => _memory[name] *= GetValue(value);

    private static void Div(string name, string value) => _memory[name] /= GetValue(value);

    private static void Msg(string msg, params string[] parameters) => _output += string.Format(msg, parameters.Select(GetValue).ToArray());

    private static string RemoveCommentsAndEmptyLines(string input)
    {
        var output = Regex.Replace(input, @";(.*?)\r?\n", "\n");
        output = Regex.Replace(output, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        output = output.Replace("\n\n", "\n");

        return output;
    }

    #endregion

    public static void Main()
    {
        Interpret(
            "\n ; My first program\nmov  a, 5\ninc  a\ncall function\nmsg  '(5+1)/2 = ', a    ; output message\nend\n\nfunction:\n    div  a, 2\n    ret\n");
    }
}