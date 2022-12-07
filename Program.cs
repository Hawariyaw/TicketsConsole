// See https://aka.ms/new-console-template for more information
var code = "{[{()}]}";

var openers = code.Where(c => c == '{' || c == '[' || c == '(').ToList();
Console.WriteLine(string.Join("", openers));

var closers = code.Where(c => c == '}' || c == ']' || c == ')').ToList();
Console.WriteLine(string.Join("", closers));

Console.WriteLine(openers.Count() == closers.Count() ? "true" : "false");

foreach (var character in openers)
{
    Console.WriteLine(string.Join("", closers));
    if (character == '{')
    {
        if (closers.Last() == '}') closers.Remove(closers.Last());
    }
    if (character == '[')
    {
        if (closers.Last() == ']') closers.Remove(closers.Last());
    }
    if (character == '(')
    {
        if (closers.Last() == ')') closers.Remove(closers.Last());
    }
}
//Console.WriteLine(string.Join("", closers));
if (closers.Count <= 0) Console.WriteLine("true");
else Console.WriteLine("false");
