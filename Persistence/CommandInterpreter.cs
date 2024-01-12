using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Basics;
using Persistence.Properties;
using Tools;

namespace Persistence
{

    /// <summary>
    /// Parameter fuer eine Callback-Methode, um zu erfahren,
    /// dass ein Befehl ausgefuehrt wurde
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        public readonly bool Result;
        public readonly Error Error;

        public CommandEventArgs(bool result, Error error)
        {
            Result = result;
            Error = error;
        }
    }

    /// <summary>
    /// Signatur fuer eine Callback-Methode, um zu erfahren,
    /// dass ein Befehl ausgefuehrt wurde
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Zusaetzliche Parameter</param>
    public delegate void OnCommandExecuted(object sender, CommandEventArgs args);

    /// <summary>
    /// Klasse, um Befehle auszuefuehren und zusaetzlich
    /// Informationen ueber den ausgefuehrten Befehl in die
    /// Log-Datei zu schreiben und Beobachter ueber die
    /// Ausfuehrung zu benachrichtigen
    /// </summary>
    public static class CommandInterpreter
    {
        /// <summary>
        /// Fuehrt den uebergebenen Befehl als Load-Command aus
        /// </summary>
        /// <param name="cmd">Ausfuehrender Befehl</param>
        /// <returns>Befehl wurde erfolgreich ausgefuehrt</returns>
        public static bool ExecuteLoadCommand(BaseCommand cmd)
        {
            return ExecuteCommand(CommandTyp.Load, cmd);
        }

        /// <summary>
        /// Fuehrt den uebergebenen Befehl aus
        /// </summary>
        /// <param name="commandTyp">Befehlstyp</param>
        /// <param name="cmd">Ausfuehrender Befehl</param>
        /// <returns>Befehl wurde erfolgreich ausgefuehrt</returns>
        public static bool ExecuteCommand(CommandTyp commandTyp, BaseCommand cmd)
        {
            Debug.Assert(cmd != null, "Auszuführender Befehl darf nicht null sein !!!");
            DateTime dt = DateTime.Now;
            Log.Write(TraceLevel.Verbose, "CommandInterpreter", "ExecuteCommand", "Starte \"{0}\" um {1}", cmd.GetDescription(commandTyp), dt);
            Error error = new Error();
            bool result = false;
            try
            {
                result = cmd.Execute(commandTyp, error);
            }
            catch (Exception ex)
            {
                Log.Write("CommandInterpreter", "ExecuteCommand", ex);
            }
            CommandExecuted(cmd.GetDescription(commandTyp), result, error, DateTime.Now - dt);
            return result;
        }

        private static void CommandExecuted(string description, bool result, Error error, TimeSpan ts)
        {
            string message;
            TraceLevel level;
            if (result)
            {
                message = string.Format("\"{0}\" {1} ({2}).", description, Resource.Successful, ts.ToString());
                level = TraceLevel.Verbose;
            }
            else
            {
                message = string.Format("\"{0}\" {1}: {2}. ({3})", description, Resource.Failed, error.Description, ts.ToString());
                level = TraceLevel.Warning;
            }
            Log.Write(level, "CommandInterpreter", "ExecuteCommand", message);

            if (Executed != null)
            {
                Executed(null, new CommandEventArgs(result, error));
            }
        }

        public static event OnCommandExecuted Executed;
    }
}
