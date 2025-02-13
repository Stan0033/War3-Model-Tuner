using System;
using System.IO;
using System.Text;
using System.Windows;
public class FileCleaner
{
    public static void CleanFile(string filePath)
    {
        // Read the file content
        string content = File.ReadAllText(filePath);
        // Define the valid character set
        StringBuilder cleanedContent = new StringBuilder();
        foreach (char c in content)
        {
            if (IsValidCharacter(c))
            {
                cleanedContent.Append(c);
            }
        }
        // Delete the original file
        File.Delete(filePath);
        // Write the cleaned content to a new file
        File.WriteAllText(filePath, cleanedContent.ToString());
    }
    private static bool IsValidCharacter(char c)
    {
        // Check if the character is a valid English letter, symbol, digit, tab, or newline
        return char.IsLetterOrDigit(c) ||
               char.IsWhiteSpace(c) ||  // This includes tabs, spaces, and all other whitespaces
               c == '\n' || c == '\r' || // Newline and Carriage return
               IsValidSymbol(c);
    }
    private static bool IsValidSymbol(char c)
    {
        // Define the valid symbols (you can extend this list if needed)
        char[] validSymbols = new char[]
        {
            '!', '"', '#', '$', '%', '&', '(', ')', '*', '+', ',', '-', '.', '/',
            ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{',
            '|', '}', '~'
        };
        foreach (char symbol in validSymbols)
        {
            if (c == symbol)
            {
                return true;
            }
        }
        return false;
    }
}
