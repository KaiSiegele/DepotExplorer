using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Resources;

namespace Tools
{
    public static class ResourceManagerExtensions
    {
        /// <summary>
        /// Hilfsmethode , um aus einem Resourcen-String
        /// mit Platzhaltern eine Ausgabe zu erzeugen
        /// </summary>
        /// <param name="rm">Resource-Manager</param>
        /// <param name="name">Name Resource</param>
        /// <param name="args">Argumente um die Platzhalter auszufüllen</param>
        /// <returns>Erzeugte Ausgabe</returns>
        public static String GetMessageFromResource(this ResourceManager rm, string name, params object[] args)
        {
            string value = rm.GetString(name);
            Check(name, value);
            string message;
            try
            {
                message = string.Format(value, args);
            }
            catch (Exception)
            {
                Debug.Assert(
                  false, "Cannot create message from " + value + " (name: " + name + ")");
                message = "???";
            }
            return message;
        }

        [Conditional("DEBUG")]
        private static void Check(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                Debug.Assert(false, "Cannot get value for name " + name + " from resource.");
        }
    }
}
