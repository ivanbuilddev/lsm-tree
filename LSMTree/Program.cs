if (args.Length <= 0)
{
    return;
}

Database.Init("test");
Database.Load();


if(args[0] == "terminal")
{
    while(true)
    {
        Console.Write("> ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input)) continue;
        if(input.Trim().ToLower() == "exit") break;

        string[] parts = input.Split(' ', 3);
        string command = parts[0];
        string? key = parts.Length > 1 ? parts[1] : null;
        string? value = parts.Length > 2 ? parts[2] : null;

        CommandHandler.ExecuteCommand(command, key, value);
    }
}
else if(args[0] == "test")
{
    if(args.Length < 2) return;
    int number = Int32.Parse(args[1]);
    for(int i = 0; i < number; i++)
    {
        CommandHandler.ExecuteCommand("set", i.ToString(), (i + 1).ToString());
    }
}
else
{
    string command = args[0];
    string? key = args.Length > 1 ? args[1] : null;
    string? value = args.Length > 2 ? args[2] : null;
    CommandHandler.ExecuteCommand(command, key, value);
}