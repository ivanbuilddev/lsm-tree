public static class CommandHandler
{
    public static void ExecuteCommand(string command, string? key = null, string? value = null)
    {
        switch (command)
        {
            case "set":
                if(key == null || value == null)
                {
                    Console.WriteLine("Usage of 'set': set <key> <value>");
                    return;
                }
                Database.Set(key, value);
                return;
            case "get":
                if(key == null)
                {
                    Console.WriteLine("Usage of 'get': get <key>");
                    return;
                }
                Database.Get(key);
                return;
            case "delete":
                if(key == null)
                {
                    Console.WriteLine("Usage of 'delete': delete <key>");
                    return;
                }
                Database.Delete(key);
                return;
            case "compaction":
                Database.Compaction();
                return;
            case "help":
                Console.WriteLine("Usage: set <key> <value>  : Set a key-value pair");
                Console.WriteLine("       get <key>          : Get a value given the pair");
                Console.WriteLine("       delete <key>       : Delete a key-value pair");
                Console.WriteLine("       terminal           : Execute as continous terminal");
                return;
            default:
                Console.WriteLine("Unknown command '{0}'", command);
                return;
        }
    }
}