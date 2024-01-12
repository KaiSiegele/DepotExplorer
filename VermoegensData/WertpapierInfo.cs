using Basics;
using System;
using System.Collections.Generic;

namespace VermoegensData
{
    /// <summary>
    /// Hilfsklasse, um Informationen über
    /// mehrere Wertpapierarten abzufragen
    /// </summary>
    public static class WertpapierInfo
    {
        /// <summary>
        /// Gibt das Wertpapiert mit dem übergebenen Namen zurück
        /// oder null falls kein Objekt gefunden wurde.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Gefundenes Wertpapier oder null</returns>
        public static Wertpapier GetWertpapierByName(string name)
        {
            return GetWertpapierByValue(wp => wp.Name == name);
        }

        /// <summary>
        /// Gibt das Wertpapiert mit der übergebenen Wkn zurück
        /// oder null falls kein Objekt gefunden wurde.
        /// </summary>
        /// <param name="wkn">WKN</param>
        /// <returns>Gefundenes Wertpapier oder null</returns>
        public static Wertpapier GetWertpapierByWKN(string wkn)
        {
            return GetWertpapierByValue(wp => wp.WKN == wkn);
        }

        /// <summary>
        /// Gibt das Wertpapiert mit der übergebenen ISIN zurück
        /// oder null falls kein Objekt gefunden wurde.
        /// </summary>
        /// <param name="isin">ISIN</param>
        /// <returns>Gefundenes Wertpapier oder null</returns>
        public static Wertpapier GetWertpapierByISIN(string isin)
        {
            return GetWertpapierByValue(wp => wp.ISIN == isin);
        }

        /// <summary>
        /// Gibt das Wertpapier mit der übergebenen Id zurück
        /// oder null falls kein Objekt gefunden wurde.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Wertpapier oder null</returns>
        public static Wertpapier GetWertpapierById(int id)
        {
            Wertpapier wp = CachedData.Fonds.GetObject<Wertpapier>(id);
            if (wp == null)
            {
                wp = CachedData.Aktien.GetObject<Wertpapier>(id);
            }
            return wp;
        }

        /// <summary>
        /// Gibt die Wertpapierart des Wertpapiers mit
        /// der uebergebenen Id zurück oder WertpapierArt.None
        /// falls kein Objekt gefunden wurde.
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Wertpapier-Art des Wertpapiers oder WertpapierArt.None</returns>
        public static WertpapierArt GetWertpapierArt(int id)
        {
            Wertpapier wp = GetWertpapierById(id);
            return wp != null ? wp.Art : WertpapierArt.None;
        }

        /// <summary>
        /// Gibt die Namen der Wertpapiere zu den übergebenen Ids zurück
        /// </summary>
        /// <param name="ids">Ids der Wertpapiere</param>
        /// <returns>Liste von Namen</returns>
        public static IEnumerable<string> GetNames(IEnumerable<int> ids)
        {
            foreach (int id in ids)
            {
                Wertpapier wertpapier = GetWertpapierById(id);
                if (wertpapier != null)
                {
                    yield return wertpapier.Name;
                }
            }
        }

        /// <summary>
        /// Gibt eine Liste mit allen Wertpapieren zurück
        /// </summary>
        /// <returns>Liste mit allen Wertpapieren</returns>
        public static BaseObjects GetWertpapiere()
        {
            BaseObjects bos = new BaseObjects();
            bos.InsertObjects(CachedData.Fonds);
            bos.InsertObjects(CachedData.Aktien);
            return bos;
        }

        private static Wertpapier GetWertpapierByValue(Func<Wertpapier, bool> praed)
        {
            Wertpapier wp = CachedData.Fonds.GetSingleObject<Wertpapier>(praed);
            if (wp == null)
            {
                wp = CachedData.Aktien.GetSingleObject<Wertpapier>(praed);
            }
            return wp;
        }
    }
}