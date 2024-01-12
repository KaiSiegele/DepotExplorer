using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace UserInterface
{
    /// <summary>
    /// Hilfsklasse um den Programmablauf zu unterbrechen und 
    /// den Benutzer über wichtige Ereignsse zu informieren
    /// oder Informationen abzufragen
    /// </summary>
    public static class Prompts
    {
        /// <summary>
        ///  Öffent einen Dialog und stellt dem Benutzer eine 
        ///  Frage, die er mit Ja oder Nein beantworten kann
        /// </summary>
        /// <param name="caption">Überschrift des Dialogs</param>
        /// <param name="format">Frage</param>
        /// <param name="args">Zusätzliche Parameter</param>
        /// <returns>true wenn Benutzer Frage mit Ja beantwortet hat, false sonst</returns>
        public static bool AskUser(string caption, string format, params object[] args)
        {
            string question = CreateMessage(format, args);
            MessageBoxResult res = MessageBox.Show(question, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Öffnet einen Dialog und zeigt dem Benutzer eine Fehlermeldung an
        /// </summary>
        /// <param name="caption">>Überschrift des Dialogs</param>
        /// <param name="format">Fehlermeldung</param>
        /// <param name="args">Zusätzliche Parameter</param>
        public static void ShowError(string caption, string format, params object[] args)
        {
            Show(MessageBoxImage.Error, caption, format, args);
        }

        /// <summary>
        /// Öffnet einen Dialog und zeigt dem Benutzer eine Warnung an
        /// </summary>
        /// <param name="caption">>Überschrift des Dialogs</param>
        /// <param name="format">Fehlermeldung</param>
        /// <param name="args">Zusätzliche Parameter</param>
        public static void ShowWarning(string caption, string format, params object[] args)
        {
            Show(MessageBoxImage.Warning, caption, format, args);
        } 
        
        /// <summary>
        /// Öffnet einen Dialog und zeigt dem Benutzer eine Inforamtaion an
        /// </summary>
        /// <param name="caption">>Überschrift des Dialogs</param>
        /// <param name="format">Information</param>
        /// <param name="args">Zusätzliche Parameter</param>
        public static void ShowInfo(string caption, string format, params object[] args)
        {
            Show(MessageBoxImage.Information, caption, format, args);
        }

        private static void Show(MessageBoxImage image, string caption, string format, params object[] args)
        {
            string message = CreateMessage(format, args);
            MessageBox.Show(message, caption, MessageBoxButton.OK, image);
        }

        private static string CreateMessage(string format, params object[] args)
        {
            string message;
            try
            {
                message = string.Format(format, args);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                message = "???";
            }
            return message;
        }
    }
}
