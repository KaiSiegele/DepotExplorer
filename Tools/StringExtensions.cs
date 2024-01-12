using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class StringExtensions
    {
        /// <summary>
        /// Stellt fest, ob eine Zeichenkette ein Name ist
        /// Ein Name beginnt mit einem Großbuchstaben, 
        /// danach folgen nur noch Kleinbuchstaben.
        /// </summary>
        /// <param name="s">Zeichenkette</param>
        /// <returns>true wenn es ein gültiger Name ist, false sonst</returns>
        public static bool IsName(this string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Count() > 1)
            {
                return char.IsUpper(s.First()) && s.Skip(1).All(c => char.IsLower(c));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stellt fest, ob eine Zeichenkette ein gülitger Ortsname ist
        /// Ein gültiger Ortsname ist entweder ein Name oder ein zusammen-
        /// gesetzter Name. Bei zusammengesetzten Namen ist der erste und
        /// der letzt Teil ein Name.
        /// </summar
        /// <param name="s">Zeichenkette</param>
        /// <returns>true, wenn gültiger Ortsname, false sonst</returns>
        public static bool IsLocationName(this string s)
        {
            Func<string[], bool, bool> checkNameParts = (nameParts, isName) =>
            {
                bool result = false;
                int numberOfParts = nameParts.Count();
                Debug.Assert(numberOfParts >= 2);
                if (nameParts[0].IsName() && nameParts[numberOfParts - 1].IsName())
                {
                    result = true;
                    for (int i = 1; i < (numberOfParts - 1) && result; i++)
                    {
                        result = isName ? nameParts[i].IsName() : nameParts[i].IsAddition();
                    }
                }
                return result;
            };

            if (s.IndexOf(' ') != -1)
            {
                string[] nameParts = s.Split(' ');
                return checkNameParts(nameParts, false);
            }
            else if (s.IndexOf('-') != -1)
            {
                string[] nameParts = s.Split('-');
                return checkNameParts(nameParts, true);
            }
            else
            {
                return s.IsName();
            }
        }

        /// <summary>
        /// Stellt fest, ob die uebergebene Zeichenkette eine
        /// Abkürzung ist die nur aus Großbuchstaben besteht
        /// </summary>
        /// <param name="s">Zeichenkette</param>
        /// <returns>true wenn gültige Abkürzung, false sonst</returns>
        public static bool IsAbbreviation(this string s)
        {
            return s.HasMinLenght(2) && s.All(c => char.IsUpper(c));
        }

        /// <summary>
        /// Stellt fest, ob die uebergebene Zeichenkette
        /// ein Identificationcode ist der nur aus Groß-
        /// buchstaben oder Nummern besteht
        /// </summary>
        /// <param name="s">Zeichenkette</param>
        /// <returns>true wenn gültiger Identificationscode, false sonst</returns>
        public static bool IsIdentificationCode(this string s)
        {
            return s.HasMinLenght(2) && s.All(c => char.IsUpper(c) || char.IsDigit(c));
        }

        /// <summary>
        /// Stellt fest, ob die Zeichenkette nur aus Nummern besteht
        /// z. B. 0 oder 1234
        /// </summary>
        /// <param name="s">Zeichenkette</param>
        /// <returns>true wenn die Zeichenkette nur aus Nummern besteht, false sonst</returns>
        public static bool AreAllDigits(this string s)
        {
            return s.HasMinLenght(1) && s.All(c => char.IsDigit(c));
        }

        private static bool IsAddition(this string s)
        {
            return s.HasMinLenght(1) && s.All(c => char.IsLower(c));
        }

        private static bool HasMinLenght(this string s, int minLength)
        {
            return !string.IsNullOrEmpty(s) && s.Count() >= minLength;
        }
    }
}
