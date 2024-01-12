using Basics;
using Persistence;
using System;
using System.Data;
using System.Linq;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Report, der die Entwicklung eines Depots über die Jahre ermittelt
    /// </summary>
    public class DepotEntwicklungReport : BaseReport
    {
        public DepotEntwicklungReport()
        {
            DepotId = BaseObject.NotSpecified;

            AddColumn("Jahr", "System.Int32");
            AddColumn("Kaeufe", "System.Double");
            AddColumn("Verkaeufe", "System.Double");
            AddColumn("Dividende", "System.Double");
            AddColumn("Gesamtwert", "System.Double");
            AddColumn("Gewinn", "System.Double");
        }

        public int DepotId { set; get; }

        public override bool FillTable()
        {
            string title = Properties.Resources.LoadDepotEntwicklungData;

            bool result = false;
            DataTable dtVerkaeufeKaeufe = new DataTable();
            string query = string.Format("select j.id as Jahr, sum(bs.kauf) as Kaeufe, sum(bs.verkauf) as Verkaeufe, sum(bs.dividende) as Dividende from jahre j " +
                                          "inner join bestaende bs on bs.jahr <= j.id where bs.depot = {0}  " +
                                          "group by j.id", DepotId);

            if (LoadTable(title, query, dtVerkaeufeKaeufe))
            {
                DataTable dtBestande = new DataTable();
                query = string.Format("select b.Depot as Depot, b.Jahr as Jahr, sum(b.Anteile * coalesce(k.Wert, 0.0)) as Gesamtwert from bestaende b " +
                        "left outer join kurse k on b.Wertpapier = k.Wertpapier and b.Jahr = k.Jahr where b.Depot = {0} group by Depot, Jahr", DepotId);

                if (LoadTable(title, query, dtBestande))
                {
                    try
                    {
                        var dtResult = (from t1 in dtVerkaeufeKaeufe.AsEnumerable()
                                        join t2 in dtBestande.AsEnumerable() on t1.Field<int>("Jahr") equals t2.Field<int>("Jahr")
                                        select new
                                        {
                                            Jahr = t1.Field<int>("Jahr"),
                                            Verkaeufe = t1.Field<double>("Verkaeufe"),
                                            Kaeufe = t1.Field<double>("Kaeufe"),
                                            Dividende = t1.Field<double>("Dividende"),
                                            Gesamtwert = t2.Field<double>("Gesamtwert")
                                        }).ToList();

                        foreach (var rowResult in dtResult)
                        {
                            DataRow newRow = _dataTable.NewRow();
                            newRow.SetIntValue("Jahr", rowResult.Jahr);
                            newRow.SetDoubleValue("Gesamtwert", rowResult.Gesamtwert);
                            newRow.SetDoubleValue("Kaeufe", rowResult.Kaeufe);
                            newRow.SetDoubleValue("Verkaeufe", rowResult.Verkaeufe);
                            newRow.SetDoubleValue("Dividende", rowResult.Dividende);
                            newRow.SetDoubleValue("Gewinn", rowResult.Gesamtwert - rowResult.Kaeufe + rowResult.Verkaeufe + rowResult.Dividende);
                            _dataTable.Rows.Add(newRow);
                        }

                        result = true;

                    }
                    catch (Exception ex)
                    {
                        Log.Write("DepotReports", "LoadEntwicklungReport", ex);

                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Report für den Jahresabschluss eines Depots. Die Werte am Ende
    /// des Jahres werden mit denen des Vorjahres verglichen.
    /// </summary>
    public class DepotJahresAbschlussgReport : BaseReport
    {
        public DepotJahresAbschlussgReport()
        {
            DepotId = BaseObject.NotSpecified;
            JahrId = BaseObject.NotSpecified;

            AddColumn("Wertpapier", "System.String");
            AddColumn("Vorjahr", "System.Double");
            AddColumn("Verkauf", "System.Double");
            AddColumn("Kauf", "System.Double");
            AddColumn("Dividende", "System.Double");
            AddColumn("Anteile", "System.Double");
            AddColumn("Kurswert", "System.Double");
            AddColumn("Gesamtwert", "System.Double");
            AddColumn("Gewinn", "System.Double");
        }

        public int DepotId { set; get; }
        public int JahrId { set; get; }

        public override bool FillTable()
        {
            bool result = false;
            string query =
              string.Format(
                "select Name, b.Kauf as Kauf, b.Verkauf as Verkauf, b.Dividende as Dividende, b.Verkauf + b.Dividende - b.Kauf as Saldo, b.Anteile as Anteile, coalesce(k.Wert, 0.0) as Kurswert, b.Anteile * coalesce(k.Wert, 0.0) as Gesamtwert, coalesce(bv.Anteile, 0.0) * coalesce(kv.Wert, 0.0) as Vorjahr " +
                "from Bestaende b inner join Wertpapiere w on w.Id = b.Wertpapier " +
                "left outer join Kurse k on k.Wertpapier = b.Wertpapier and k.Jahr = {1} " +
                "left outer join Bestaende bv on bv.Depot = b.Depot and bv.Wertpapier = b.Wertpapier and bv.Jahr = {2} " +
                "left outer join Kurse kv on kv.Wertpapier = b.Wertpapier and kv.Jahr = {2} " +
                "where b.Depot = {0} and b.Jahr = {1}",
                DepotId,
                JahrId,
                JahrId - 1);

            if (LoadTable(Properties.Resources.LoadDepotJahresAbschlussData, query, _dataTable))
            {
                Func<double, double, double, double> calcGewinn = (wert, vorjahreswert, saldo) => wert - vorjahreswert + saldo;
                _dataTable.UpdateColumn("Gewinn", "Gesamtwert", "Vorjahr", "Saldo", calcGewinn);
                result = true;
            }
            return result;
        }
    }

    /// <summary>
    /// Report, der die Entwicklung eines Wertpapiers in einem Depot über die Jahre ermittelt
    /// </summary>
    public class DepotWertpapierEntwicklungReport : BaseReport
    {
        public DepotWertpapierEntwicklungReport()
        {
            WertpapierId = BaseObject.NotSpecified;
            DepotId = BaseObject.NotSpecified;

            AddColumn("Jahr", "System.Int32");
            AddColumn("Kaeufe", "System.Double");
            AddColumn("Verkaeufe", "System.Double");
            AddColumn("Dividende", "System.Double");
            AddColumn("Anteile", "System.Double");
            AddColumn("Kurswert", "System.Double");
            AddColumn("Gesamtwert", "System.Double");
            AddColumn("Gewinn", "System.Double");
        }

        public int DepotId { set; get; }
        public int WertpapierId { set; get; }

        public override bool FillTable()
        {
            bool result = false;

            string query =
                  string.Format(
                    "select b.Jahr as Jahr, b.Anteile as Anteile, coalesce(k.Wert, 0.0) as Kurswert, b.Anteile * coalesce(k.Wert, 0.0) as Gesamtwert, sum(bs.Kauf) as Kaeufe, sum(bs.Verkauf) as Verkaeufe, sum(bs.Dividende) as Dividende " +
                    "from Bestaende b inner join Bestaende bs on bs.Depot = b.Depot and bs.Wertpapier = b.Wertpapier and bs.Jahr <= b.Jahr " +
                    "left outer join Kurse k on k.Jahr = b.Jahr and k.Wertpapier = b.Wertpapier " +
                    "where b.Depot = {0} and b.Wertpapier = {1} " +
                    "group by b.Jahr, b.Anteile, coalesce(k.Wert, 0.0), b.Anteile * coalesce(k.Wert, 0.0) " +
                    "order by b.Jahr",
                    DepotId,
                    WertpapierId);

            if (LoadTable(Properties.Resources.LoadDepotWertpapierEntwicklungData, query, _dataTable))
            {
                Func<double, double, double, double> calcGewinn = (wert, kauf, verkauf) => wert - kauf + verkauf;
                _dataTable.UpdateColumn("Gewinn", "Gesamtwert", "Kaeufe", "Verkaeufe", calcGewinn);
                result = true;
            }
            return result;
        }
    }
}
