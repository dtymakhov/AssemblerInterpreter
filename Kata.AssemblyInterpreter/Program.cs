namespace Kata.AssemblyInterpreter;

public class AssemblerInterpreter
{
    private static Dictionary<string, int> _memory = new Dictionary<string, int>();
    private static int _pointer = 0;

    public static string Interpret(string input)
    {
        // Your code here!
        return null;
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

    #endregion

    public static void Main()
    {
    }
}