using System;
using System.Diagnostics;
using System.IO;

namespace Tools
{
    /// <summary>
    /// Klasse, um Informationen ueber die Ausfuehrung des
    /// Programms in eine Ausgabedatei zu schreiben
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Initialisierung. 
        /// Setzt eine Ausgabedatei in der Log-Informationen geschrieben werden
        /// </summary>
        /// <param name="dir">Verzeichnis für die Ausgabedatei</param>
        /// <param name="filename">Ausgabedatei</param>
        /// <param name="program">Name des Programms</param>
        /// <param name="traceLevel">Level, um zu bestimmen, welche Informationen ausgegeben werden</param>
        /// <returns>Ausgabedatei erfolgreich gesetzt</returns>
        public static bool Init(string dir, string filename, string program, TraceLevel traceLevel, ref string message)
        {
            bool result = false;
            if (SetDirectory(dir, ref message))
            {
                result = true;
                string path = Path.Combine(Dir, filename);
                var tl = new System.Diagnostics.TextWriterTraceListener(path);
                Trace.Listeners.Add(tl);

                TraceSwitch traceSwitch = null;
                try
                {
                    traceSwitch = new TraceSwitch(program, "Level für Basisc");
                    traceSwitch.Level = traceLevel;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                Level = traceLevel;
                Write(TraceLevel.Verbose, "Log", "Init", "Programm startet um {0}, TraceLevel {1}", DateTime.Now, traceLevel);
            }
            return result;
        }

 
        /// <summary>
        /// Schreibt Informationen in die Ausgabedatei
        /// </summary>
        /// <param name="level">Level, der die Wichtigkeit anzeigt</param>
        /// <param name="className">Klassenname</param>
        /// <param name="method">Methode, die gerade ausgefuehrt wird</param>
        /// <param name="message">Nachricht</param>
        /// <param name="args">Zusaetzliche Argumente</param>
        public static void Write(TraceLevel level, string className, string method, string message, params object[] args)
        {
            if (Level >= level)
            {
                string output;
                try
                {
                    output = string.Format(message, args);
                }
                catch (Exception)
                {
                    Debug.Assert(false, "Cannot create output from " + message + ")");
                    output = message;
                }
                Write(LevelAsText(level) + " " + className + "::" + method + ": " + output);
            }
        }

        /// <summary>
        /// Schreibt Informationen ueber eine aufgetretene Exception in die Ausgabedatei
        /// </summary>
        /// <param name="className">Klassenname</param>
        /// <param name="method">Methode, die gerade ausgefuehrt wird</param>
        /// <param name="ex">Aufgetretene Exception</param>
        public static void Write(string className, string method, Exception ex)
        {
            Write(TraceLevel.Error, className, method, "Exception {0} ({1})", ex.Message, ex.StackTrace);
        }

        /// <summary>
        /// Bestimmt das Verzeichnis in dem die Log-Dateien geschrieben werden sollen
        /// </summary>
        /// <param name="dir">Verzeichnis für die Log-Dateien</param>
        /// <param name="message">Fehlermeldung falls Verzeichnis nicht gesetzt werden konnte</param>
        /// <returns>Verzeichnis erfolgreich gesetzt</returns>
        private static bool SetDirectory(string dir, ref string message)
        {
            bool result = false;
            if (Directory.Exists(dir))
            {
                result = true;
                Dir = dir;
            }
            else
            {
                message = Properties.Resource.LogDirectoryNotFound;
            }

            return result;
        }

        private static string LevelAsText(TraceLevel level)
        {
            string text = " ";
            switch (level)
            {
                case TraceLevel.Error:
                    text = "<X>";
                    break;


                case TraceLevel.Warning:
                    text = "<!>";
                    break;

                case TraceLevel.Info:
                    text = " I ";
                    break;

                case TraceLevel.Verbose:
                    text = " - ";
                    break;

                default:
                    break;
            }
            return text;
        }

        static Log()
        {
            Level = TraceLevel.Error;
            Dir = string.Empty;
        }

        private static void Write(string output)
        {
            Trace.WriteLine(output);
        }

  
        private static TraceLevel Level { get; set; }

        private static string Dir { get; set; }
    }
}
