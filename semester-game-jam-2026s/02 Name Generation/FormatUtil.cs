using System;

namespace sgj.NameGeneration;

public class FormatUtil
{
    public static string Format(string name)
    {
        while (name.Contains('%'))
        {
            int start = name.IndexOf('%') + 1;
            int end = start;
            while (end < name.Length && '0' <= name[end] && name[end] <= '9') { end++; }
            if (end == start || end == name.Length)
            {
                throw new Exception("Unexpected EOS");
            }
            int rep_length = int.Parse(name[start..end]);
            string replacement_choice = "";
            switch (name[end])
            {
                case 'd':
                    replacement_choice = "0123456789";
                    end++;
                    break;
                case 's':
                    replacement_choice = "abcdefghijklmnopqrstuvwxyz";
                    end++;
                    break;
                case 'S':
                    replacement_choice = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    end++;
                    break;
                case '[':
                    end++;
                    char lastchar = (char)0;
                    while (end < name.Length && name[end] != ']')
                    {
                        if (name[end] == '-')
                        {
                            end++;
                            if (lastchar == 0 || end == name.Length || name[end] == ']')
                            {
                                throw new Exception("Invalid usage of '-'.");
                            }
                            lastchar++;
                            while (lastchar != name[end])
                            {
                                replacement_choice += lastchar;
                                lastchar++;
                            }
                        }
                        replacement_choice += name[end];
                        lastchar = name[end];
                        end++;
                    }
                    if (end == name.Length)
                    {
                        throw new Exception("Unexpected EOS");
                    }
                    end++;
                    break;
                default:
                    throw new Exception("Unknown replacement char");
            }
            string replacement_string = "";
            for (int i = 0; i < rep_length; i++)
            {
                replacement_string += replacement_choice.PickRandom();
            }
            name = name[..(start - 1)] + replacement_string + name[end..];
        }

        return name;
    }
}