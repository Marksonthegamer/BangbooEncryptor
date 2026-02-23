using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BangbooEncryptor;

public static class BangbooEncrypto
{
    private static byte[] Compress(byte[] data)
    {
        using var compressedStream = new MemoryStream();
        using var dfl = new DeflateStream(compressedStream, CompressionMode.Compress);
        dfl.Write(data, 0, data.Length);
        dfl.Close();
        return compressedStream.ToArray();
    }

    private static byte[] Decompress(byte[] data)
    {
        using var compressedStream = new MemoryStream(data);
        using var decompressedStream = new MemoryStream();
        using var dfl = new DeflateStream(compressedStream, CompressionMode.Decompress);
        dfl.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }

    public static string Encrypt(string plainText, string password, int? seed = null)
    {
        Random rnd = seed.HasValue ? new Random(seed.Value) : new Random();

        byte[] original = Encoding.UTF8.GetBytes(plainText);
        byte[] data = Compress(original);

        byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
        byte[] keyStream = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
            keyStream[i] = key[i % key.Length];

        byte[] encrypted = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
            encrypted[i] = (byte)(data[i] ^ keyStream[i]);

        StringBuilder binary = new StringBuilder();
        foreach (byte b in encrypted)
            binary.Append(Convert.ToString(b, 2).PadLeft(8, '0'));

        Dictionary<string, string> phraseMap = new Dictionary<string, string>
        {
            {"00", "嗯呢"},
            {"01", "嗯呐"},
            {"10", "哇哒"},
            {"11", "嗯呐哒"}
        };

        List<string> phrases = new List<string>();
        string binStr = binary.ToString();
        for (int i = 0; i < binStr.Length; i += 2)
        {
            string pair = binStr.Substring(i, Math.Min(2, binStr.Length - i)).PadRight(2, '0');
            phrases.Add(phraseMap[pair]);
        }

        List<string> bangboo = new List<string>();
        List<string> currentSentence = new List<string>();

        foreach (string phrase in phrases)
        {
            bool hasEn = currentSentence.Contains("嗯呢");
            bool hasNa = currentSentence.Contains("嗯呐");
            bool willViolate = (phrase == "嗯呢" && hasNa) || (phrase == "嗯呐" && hasEn);

            if (willViolate && currentSentence.Count > 0)
            {
                bangboo.Add(string.Join("", currentSentence));
                bangboo.Add(rnd.NextDouble() < 0.8 ? "，" : "！");
                currentSentence.Clear();
            }

            currentSentence.Add(phrase);

            if (phrase == "嗯呐哒")
            {
                bangboo.Add(string.Join("", currentSentence));
                bangboo.Add(rnd.NextDouble() < 0.9 ? "！" : "！！");
                currentSentence.Clear();
            }
            else if (currentSentence.Count >= 4 || rnd.NextDouble() < 0.35)
            {
                bangboo.Add(string.Join("", currentSentence));
                bangboo.Add(rnd.NextDouble() < 0.75 ? "，" : "！");
                currentSentence.Clear();
            }
        }

        if (currentSentence.Count > 0)
        {
            bangboo.Add(string.Join("", currentSentence));
            bangboo.Add(rnd.NextDouble() < 0.9 ? "！" : "！！");
        }

        return string.Join("", bangboo);
    }

    public static string Decrypt(string bangbooText, string password)
    {
        string clean = Regex.Replace(bangbooText, @"[，！]", "");

        Dictionary<string, string> reverseMap = new Dictionary<string, string>
        {
            {"嗯呢", "00"},
            {"嗯呐", "01"},
            {"哇哒", "10"},
            {"嗯呐哒", "11"}
        };

        List<string> phrases = new List<string>();
        int pos = 0;
        while (pos < clean.Length)
        {
            if (pos + 3 <= clean.Length && clean.Substring(pos, 3) == "嗯呐哒")
            {
                phrases.Add("嗯呐哒");
                pos += 3;
            }
            else if (pos + 2 <= clean.Length)
            {
                string two = clean.Substring(pos, 2);
                if (two == "嗯呢" || two == "嗯呐" || two == "哇哒")
                {
                    phrases.Add(two);
                    pos += 2;
                }
                else
                {
                    Console.WriteLine("Error: Invalid Bangboo language sequence");
                    return string.Empty;
                }
            }
            else
            {
                Console.WriteLine("Error: Insufficient characters remaining");
                return string.Empty;
            }
        }

        StringBuilder binary = new StringBuilder();
        foreach (string phrase in phrases)
            binary.Append(reverseMap[phrase]);

        List<byte> bytes = new List<byte>();
        string binStr = binary.ToString();
        for (int i = 0; i < binStr.Length - (binStr.Length % 8); i += 8)
        {
            bytes.Add(Convert.ToByte(binStr.Substring(i, 8), 2));
        }

        byte[] encrypted = bytes.ToArray();

        byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
        byte[] keyStream = new byte[encrypted.Length];
        for (int i = 0; i < encrypted.Length; i++)
            keyStream[i] = key[i % key.Length];

        byte[] decryptedCompressed = new byte[encrypted.Length];
        for (int i = 0; i < encrypted.Length; i++)
            decryptedCompressed[i] = (byte)(encrypted[i] ^ keyStream[i]);

        try
        {
            byte[] decompressed = Decompress(decryptedCompressed);
            return Encoding.UTF8.GetString(decompressed);
        }
        catch (InvalidDataException)
        {
            Console.WriteLine("Error: Wrong password or input text");
            return string.Empty;
        }
    }
}