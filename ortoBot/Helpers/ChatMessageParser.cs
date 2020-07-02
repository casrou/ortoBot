using System;
using System.Collections.Generic;
using System.Text;

namespace ortoBot.Helpers
{
    public class ChatMessageParser
    {
        public static string GetKeywordValue(string message, string keyword)
        {
            string value = string.Empty;
            int pos = message.IndexOf(keyword);
            if (pos < 0) return null;

            string temp = message.Substring(pos + keyword.Length).Trim();
            string[] parts = temp.Split(' ');
            value = parts[0];

            return value.ToLower();
        }
    }
}
