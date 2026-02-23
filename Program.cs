using BangbooEncryptor;

internal static class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            DisplayHelp();
            return;
        }

        string command = string.Empty;
        string textArg = string.Empty;
        string password = string.Empty;
        int seed = 42;
        bool trimNewline = false;
        bool useTextFromFile = false;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (arg == "--help" || arg == "-h")
            {
                DisplayHelp();
                return;
            }
            if (arg == "-V" || arg == "--version")
            {
                Console.WriteLine("BangbooEncryptor v1.0.0");
                return;
            }
            if (arg is "-t" or "--trim")
            {
                trimNewline = true;
            }
            else if (arg is "-i" or "--file")
            {
                useTextFromFile = true;
            }
            else if (arg is "-p" or "--password")
            {
                if (i + 1 >= args.Length)
                {
                    Console.WriteLine("Error: --password / -p needs to follow a password");
                    return;
                }
                password = args[++i];
            }
            else if (arg is "-s" or "--seed")
            {
                if (i + 1 >= args.Length)
                {
                    Console.WriteLine("Error: --seed / -s needs to follow an integer value");
                    return;
                }
                if (!int.TryParse(args[++i], out seed))
                {
                    Console.WriteLine($"Error: Invalid seed value: {args[i]}");
                    return;
                }
            }
            else if (string.IsNullOrEmpty(command) && (arg == "encrypt" || arg == "decrypt" || arg == "interactive"))
            {
                command = arg;
            }

            else if (string.IsNullOrEmpty(textArg))
            {
                textArg = arg;
            }
            else
            {
                Console.WriteLine($"Error: Unexpected argument '{arg}'");
                return;
            }
        }

        if (string.IsNullOrEmpty(command))
        {
            Console.WriteLine("Error: Please specify a valid command (encrypt, decrypt, interactive).");
            return;
        }

        string text;

        if (command == "interactive")
        {
            RunInteractiveMode();
            return;
        }

        if (string.IsNullOrEmpty(textArg))
        {
            if (!Console.IsInputRedirected && !Console.KeyAvailable)
            {
                Console.WriteLine("Error: Standard input not found and no input text/file specified.");
                return;
            }
            using var reader = new StreamReader(Console.OpenStandardInput());
            text = reader.ReadToEnd().TrimEnd('\r', '\n');
        }
        else
        {
            if (useTextFromFile)
            {
                if (!File.Exists(textArg))
                {
                    Console.WriteLine($"Error: File not found: {textArg}");
                    return;
                }
                text = File.ReadAllText(textArg).TrimEnd('\r', '\n');
            }
            else
            {
                text = textArg;
            }
        }

        if (trimNewline)
        {
            text = text.Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        if (command == "encrypt")
        {
            Console.WriteLine(BangbooEncrypto.Encrypt(text, password, seed));
        }
        else if (command == "decrypt")
        {
            Console.WriteLine(BangbooEncrypto.Decrypt(text, password));
        }
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Good day! Would you like to encrypt texts or decrypt gibberish? [encrypt/decrypt]");
        string command = Console.ReadLine() ?? string.Empty;
        
        while (command != "encrypt" && command != "decrypt")
        {
            Console.WriteLine("Error: Invalid command. Please enter \"encrypt\" or \"decrypt\"");
            command = Console.ReadLine() ?? string.Empty;
        }

        Console.WriteLine("Input source text (end input with a blank line):");
        string srcText = string.Empty;
        string lastInputLine = Console.ReadLine() ?? string.Empty;
        srcText += lastInputLine;
        
        while (srcText == string.Empty)
        {
            Console.WriteLine("Error: Input text cannot be blank");
            Console.WriteLine("Input source text (end input with a blank line):");
            lastInputLine = Console.ReadLine() ?? string.Empty;
            srcText += lastInputLine;
        }

        srcText += "\n";

        while (lastInputLine != string.Empty)
        {
            lastInputLine = Console.ReadLine() ?? string.Empty;
            if (lastInputLine != string.Empty)
            {
                srcText += lastInputLine + "\n";
            }
        }

        string text = srcText.TrimEnd('\r', '\n');

        Console.WriteLine($"Password {(command == "encrypt" ? "(optional)" : "(leave empty if unset)")}:");
        string passwordInput = Console.ReadLine() ?? string.Empty;
        string finalPassword = passwordInput != string.Empty ? passwordInput : string.Empty;

        if (command == "encrypt")
        {
            Console.WriteLine("Seed (optional):");
            string seedInput = Console.ReadLine() ?? string.Empty;
            int finalSeed;
            
            while (!int.TryParse(seedInput, out finalSeed) && seedInput != string.Empty)
            {
                Console.WriteLine("Error: Invalid seed. Must be a 32-bit integer.");
                seedInput = Console.ReadLine() ?? string.Empty;
            }

            Console.WriteLine(BangbooEncrypto.Encrypt(text, finalPassword, finalSeed));
        }
        else if (command == "decrypt")
        {
            Console.WriteLine(BangbooEncrypto.Decrypt(text, finalPassword));
        }
    }

    private static void DisplayHelp()
    {
        Console.WriteLine("Usage: BangbooEncryptor [options] command");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  encrypt [TEXT]             Use encryption mode. If [TEXT] is not specified, stdin will be used.");
        Console.WriteLine("  decrypt [TEXT]             Use decryption mode. If [TEXT] is not specified, stdin will be used.");
        Console.WriteLine("  interactive                Enter interactive mode.");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -h, --help                 Show this help");
        Console.WriteLine("  -p, --password             Specify password");
        Console.WriteLine("      --trim                 Remove newline in input file");
        Console.WriteLine("  -i, --file                 Specify path to a text to encrypt instead of capturing stdin");
        Console.WriteLine("  -s, --seed                 Specify seed");
        Console.WriteLine("  -V, --version              Show version information");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  BangbooEncryptor -V");
        Console.WriteLine("  BangbooEncryptor encrypt \"Hello\" -p \"secret\" -s 42");
        Console.WriteLine("  BangbooEncryptor decrypt \"嗯呐哒！哇哒，嗯呢嗯呢，哇哒嗯呐哒！哇哒！嗯呢，嗯呢嗯呢，嗯呐哒！嗯呐！\" -p \"secret\"");
    }
}
