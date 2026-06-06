if (args.Length <= 0 || args[0] == "help")
{
    Console.WriteLine("Usage: set <key> <value>");
    Console.WriteLine("       get <key>");
    Console.WriteLine("       delete <key>");
}

Database.Init("test");
Database.Load();

switch (args[0])
{
    case "set":
        Database.Set(args[1], args[2]);
        return;
    case "get":
        Database.Get(args[1]);
        return;
    case "delete":
        Database.Delete(args[1]);
        return;
    case "compaction":
        Database.Compaction();
        return;
    default:
        Console.WriteLine("Unknown command '{0}'", args[0]);
        return;
}