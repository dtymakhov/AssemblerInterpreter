using System.Text.RegularExpressions;

namespace Kata.AssemblyInterpreter;

public class AssemblerInterpreter
{
    private static Dictionary<string, int> _memory = new Dictionary<string, int>();
    private static Dictionary<string, string> _functions = new Dictionary<string, string>();
    private static string _output = string.Empty;
    private static int _pointer = 0;

    public static string Interpret(string input)
    {
        input = RemoveComments(input);
        var lines = input.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        for (; _pointer < lines.Length; _pointer++)
        {
            var line = lines[_pointer];
            var command = line.Split(' ');
            var commandName = command[0];
            var commandArgs = command.Skip(1).ToArray();

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
                case "Mul":
                    Mul(commandArgs[0], commandArgs[1]);
                    break;
                case "Div":
                    Div(commandArgs[0], commandArgs[1]);
                    break;
                case "Msg":
                    Msg(commandArgs[0], commandArgs.Skip(1).ToArray());
                    break;
                case "Call":
                    Call(commandArgs[0]);
                    break;
            }
        }

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

    private static void Call(string label) => Interpret(_functions[label]);

    private static string RemoveComments(string input)
    {
        var output = Regex.Replace(input, @";(.*?)\r?\n", "\n");

        return output;
    }

    #endregion

    public static void Main()
    {
        Interpret(
            "\nmov   a, 81         ; value1\nmov   b, 153        ; value2\ncall  init\ncall  proc_gcd\ncall  print\nend\n\nproc_gcd:\n    cmp   c, d\n    jne   loop\n    ret\n\nloop:\n    cmp   c, d\n    jg    a_bigger\n    jmp   b_bigger\n\na_bigger:\n    sub   c, d\n    jmp   proc_gcd\n\nb_bigger:\n    sub   d, c\n    jmp   proc_gcd\n\ninit:\n    cmp   a, 0\n    jl    a_abs\n    cmp   b, 0\n    jl    b_abs\n    mov   c, a            ; temp1\n    mov   d, b            ; temp2\n    ret\n\na_abs:\n    mul   a, -1\n    jmp   init\n\nb_abs:\n    mul   b, -1\n    jmp   init\n\nprint:\n    msg   'gcd(', a, ', ', b, ') = ', c\n    ret\n");
    }
}