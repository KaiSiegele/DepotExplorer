using Basics;
using Persistence;
using System;
using System.Data;
using System.Linq;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Report, der eine Übersicht über alle Depots in einem Jahr anzeigt
    /// </summary>
    public class JahresUebersichtReport : BaseReport
    {
        public JahresUebersichtReport()
        {
            JahrId = BaseObject.NotSpecified;
            
            AddColumn("Name", "System.String");
            AddColumn("Kaeufe", "System.Double");
            AddColumn("Verkaeufe", "System.Double");
            AddColumn("Dividende", "System.Double");
            AddColumn("Gesamtwert", "System.Double");
            AddColumn("Gewinn", "System.Double");
        }

        public int JahrId { set; get; }
        
        public override bool FillTable()
        {
            bool result = false;
            string title = Properties.Resources.LoadJahresUebersichtData;
            DataTable dtBestaende = new DataTable();
            string query = string.Format("select d.name as Name, b.Jahr as Jahr, sum(b.Anteile * coalesce(k.Wert, 0.0)) as Gesamtwert from depots d " +
                            "inner join bestaende b on b.depot = d.id " +
                            "left outer join kurse k on b.Wertpapier = k.Wertpapier and k.Jahr = {0} " +
                            "where b.jahr = {0} group by Name, Jahr", JahrId);
            if (LoadTable(title, query, dtBestaende))
            {
                DataTable dtKaufeVerkauefe = new DataTable();
                query = string.Format("select d.name as Name, sum(bs.Verkauf) as Verkaeufe, sum(bs.Kauf) as Kaeufe, sum(bs.Dividende) as Dividende from depots d " +
                        "inner join bestaende bs on bs.depot = d.id and bs.Jahr <= {0} " +
                        "group by Name", JahrId);
                if (LoadTable(title, query, dtKaufeVerkauefe))
                {
                    try
                    {
                        var dtResult = (from t1 in dtBestaende.AsEnumerable()
                                        join t2 in dtKaufeVerkauefe.AsEnumerable() on t1.Field<string>("Name") equals t2.Field<string>("Name")
                                        select new
                                        {
                                            Name = t1.Field<string>("Name"),
                                            Gesamtwert = t1.Field<double>("Gesamtwert"),
                                            Kaeufe = t2.Field<double>("Kaeufe"),
                                            Verkaeufe = t2.Field<double>("Verkaeufe"),
                                            Dividende = t2.Field<double>("Dividende")
                                        }).ToList();

                        foreach (var rowResult in dtResult)
                        {
                            DataRow newRow = _dataTable.NewRow();
                            newRow.SetStringValue("Name", rowResult.Name);
                            newRow.SetDoubleValue("Gesamtwert", rowResult.Gesamtwert);
                            newRow.SetDoubleValue("Kaeufe", rowResult.Kaeufe);
                            newRow.SetDoubleValue("Verkaeufe", rowResult.Verkaeufe);
                            newRow.SetDoubleValue("Dividende", rowResult.Dividende);
                            newRow.SetDoubleValue("Gewinn", rowResult.Gesamtwert - rowResult.Kaeufe + rowResult.Verkaeufe + rowResult.Dividende);
                            _dataTable.Rows.Add(newRow);
                        }

                        result = true;
                    }
                    catch(Exception ex)
                    {
                        Log.Write("JahresUebersichtReport", "FillTable", ex);
                    }
                }
            }
            return result;
        }
    }
}
