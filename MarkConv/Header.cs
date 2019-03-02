﻿using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace MarkConv
{
    public class Header
    {
        private static readonly MarkdownType[] MarkdownTypes = (MarkdownType[]) Enum.GetValues(typeof(MarkdownType));
        private static readonly Dictionary<char, string> RussianTranslitMap = new Dictionary<char, string>
        {
            ['а'] = "a",
            ['б'] = "b",
            ['в'] = "v",
            ['г'] = "g",
            ['д'] = "d",
            ['е'] = "e",
            ['ё'] = "yo",
            ['ж'] = "zh",
            ['з'] = "z",
            ['и'] = "i",
            ['й'] = "y",
            ['к'] = "k",
            ['л'] = "l",
            ['м'] = "m",
            ['н'] = "n",
            ['о'] = "o",
            ['п'] = "p",
            ['р'] = "r",
            ['с'] = "s",
            ['т'] = "t",
            ['у'] = "u",
            ['ф'] = "f",
            ['х'] = "h",
            ['ц'] = "c",
            ['ч'] = "ch",
            ['ш'] = "sh",
            ['щ'] = "sch",
            ['ы'] = "y",
            ['э'] = "e",
            ['ю'] = "yu",
            ['я'] = "ya",
            ['-'] = "-",
            ['_'] = "_",
            [' '] = "-"
        };

        public string Title { get; set; } = "";

        public int Level { get; set; } = 1;

        public int SourceLineIndex { get; set; } = 0;

        public int DestLineIndex { get; set; } = 0;

        public readonly Dictionary<MarkdownType, HeaderLink> Links = new Dictionary<MarkdownType, HeaderLink>();

        public Header(string headerTitle, int level, List<Header> existingHeaders)
        {
            Title = headerTitle.ExtractLinkTitle();
            Level = level;

            foreach (MarkdownType markdownType in MarkdownTypes)
                Links[markdownType] = CalculateHeaderLink(existingHeaders, markdownType, headerTitle);
        }

        protected Header()
        {
        }

        private static HeaderLink CalculateHeaderLink(List<Header> headers, MarkdownType linkType, string headerTitle)
        {
            string headerLink = GenerateLink(linkType, headerTitle);

            var sameLinkHeaders = headers.Where(h => h.Links[linkType].Link == headerLink).ToList();
            int linkNumber = 0;
            if (sameLinkHeaders.Any())
            {
                linkNumber = sameLinkHeaders.Max(h => h.Links[linkType].LinkNumber) + 1;
            }
            else if (headers.Any(h => h.Links[linkType].FullLink == headerLink))
            {
                linkNumber = 1;
            }

            return new HeaderLink(headerLink, linkNumber);
        }

        public override string ToString()
        {
            return $"{new string('#', Level)} {Title}";
        }

        public static string GenerateLink(MarkdownType linkType, string headerTitle)
        {
            switch (linkType)
            {
                case MarkdownType.Habr:
                    return HeaderToTranslitLink(headerTitle);
                default:
                    return HeaderToLink(headerTitle.ExtractLinkTitle());
            }
        }

        public static string HeaderToLink(string header)
        {
            var link = new StringBuilder(header.Length);
            foreach (char c in header)
            {
                if (char.IsLetterOrDigit(c))
                {
                    link.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    if (c == ' ' || c == '-')
                        link.Append('-');
                    else if (c == '_')
                        link.Append('_');
                }
            }
            return link.ToString();
        }

        public static string HeaderToTranslitLink(string header)
        {
            string lower = header.ToLowerInvariant();
            var link = new StringBuilder(lower.Length);
            foreach (char c in lower)
            {
                if (c >= 'a' && c <= 'z' || c >= '0' && c <= '9')
                {
                    link.Append(c);
                }
                else if (RussianTranslitMap.TryGetValue(c, out string replacement))
                {
                    link.Append(replacement);
                }
            }
            return link.ToString();
        }
    }
}
