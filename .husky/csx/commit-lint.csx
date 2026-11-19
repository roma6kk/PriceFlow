using System.Text.RegularExpressions;

private var pattern = @"^(?=.{1,90}$)(?:build|feat|ci|chore|docs|fix|perf|refactor|revert|style|test)(?:\(.+\))?!?: .{4,}$";
private var msg = File.ReadAllLines(Args[0])[0];

if (Regex.IsMatch(msg, pattern))
    return 0;

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine("Invalid commit message: " + msg);
Console.ResetColor();
Console.WriteLine("Expected Conventional Commits, e.g. feat(api): add price history endpoint");
return 1;
