using Basics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Persistence
{
  /// <summary>
  /// Klasse, um eine SQLServer-Datenbank auf der 
  /// das Programm zugreift, zu verwalten
  /// </summary>
  internal class SQLServerDatabase : Database
  {
    #region Ueberschreibungen der Basisklasse
    public override bool Open(string connectionString, Error error)
    {
      if (!IsOpen)
      {
        try
        {
          connection.ConnectionString = connectionString;
          connection.Open();
          IsOpen = true;
        }
        catch (Exception ex)
        {
          DatabaseTools.HandleError(ownClassName, "Open", ex, error);
        }
      }
      return IsOpen;
    }

    public override bool Close(Error error)
    {
      if (IsOpen)
      {
        try
        {
          connection.Close();
          IsOpen = false;
        }
        catch (Exception ex)
        {
          DatabaseTools.HandleError(ownClassName, "Close", ex, error);
        }
      }
      return (IsOpen == false);
    }

    public override bool LoadTable(string query, DataTable dt, Error error)
    {
      bool result = false;
      SqlDataAdapter adapter = null;

      adapter = new SqlDataAdapter(query, connection);
      try
      {
        adapter.Fill(dt);
        result = true;
      }
      catch (Exception ex)
      {
        DatabaseTools.HandleError(ownClassName, "LoadTable", ex, error);
      }
      finally
      {
        adapter.Dispose();
      }
      return result;
    }

    public override bool LoadObject(string query, BaseObject bo, Error error)
    {
      bool result = false;
      using (SqlCommand cmd = new SqlCommand(query, connection))
      {
        result = LoadObject(cmd, bo, error);
      }
      return result;
    }

    public override bool LoadObject(string query, BaseObject bo, IEnumerable<string> fields, Error error)
    {
      bool result = false;
      using (SqlCommand cmd = new SqlCommand(query, connection))
      {
        if (UpdateParams(cmd, bo, fields))
        {
          result = LoadObject(cmd, bo, error);
        }
        else
        {
          DatabaseTools.HandleError(ownClassName, "LoadObject", "Fehler beim Aktualieren der Parameter aus " + bo.ToString(), error);
        }
      }
      return result;
    }

    public override bool LoadObjects<T>(string query, BaseObjects bos, Error error)
    {
      bool result = false;
      SqlCommand cmd = null;
      SqlDataReader rdr = null;

      cmd = new SqlCommand(query, connection);
      try
      {
        rdr = cmd.ExecuteReader();
        result = DatabaseTools.LoadObjects<T>(rdr, query, bos, error);
      }
      catch (Exception ex)
      {
        DatabaseTools.HandleError(ownClassName, "LoadObjects<T>", ex, error);
      }
      finally
      {
        cmd.Dispose();
        if (rdr != null)
          rdr.Dispose();
      }
      return result;
    }

    public override bool Execute(string query, BaseObject bo, IEnumerable<string> fields, Error error)
    {
      bool result = false;
      SqlCommand cmd = new SqlCommand(query, connection);

      try
      {
        if (UpdateParams(cmd, bo, fields))
        {
          int count = cmd.ExecuteNonQuery();
          result = true;
        }
        else
        {
          DatabaseTools.HandleError(ownClassName, "Execute", "Fehler beim Aktualieren der Parameter aus " + bo.ToString(), error);
        }
      }
      catch (Exception ex)
      {
        DatabaseTools.HandleError(ownClassName, "Execute", ex, error);
      }
      finally
      {
        cmd.Dispose();
      }

      return result;
    }

    public override bool StartTransaction(Error error)
    {
      Debug.Assert(isInTransaction == false);
      isInTransaction = true;

      return true;
    }

    public override bool ExecutesTransaction()
    {
      return isInTransaction;
    }

    public override bool FinishTransaction(bool successful, Error error)
    {
      Debug.Assert(isInTransaction == true);
      bool result = false;
      try
      {
        result = true;
      }
      catch (Exception ex)
      {
        DatabaseTools.HandleError(ownClassName, "FinishTransaction", ex, error);
      }
      finally
      {
        isInTransaction = false;
      }
      return result;
    }
    #endregion

    private bool LoadObject(SqlCommand cmd, BaseObject bo, Error error)
    {
      bool result = false;
      SqlDataReader rdr = null;

      try
      {
        rdr = cmd.ExecuteReader();
        result = DatabaseTools.LoadObject(rdr, bo, error);
      }
      catch (Exception ex)
      {
        DatabaseTools.HandleError(ownClassName, "LoadObject", ex, error);
      }
      finally
      {
        if (rdr != null)
          rdr.Dispose();
      }
      return result;
    }

    private bool UpdateParams(SqlCommand command, BaseObject bo, IEnumerable<string> fields)
    {
      bool result = true;
      foreach (var field in fields)
      {
        object obj = bo.GetValue(field);
        if (obj != null)
        {
          string param = "@" + field.ToLower();
          command.Parameters.AddWithValue(param, obj);
        }
        else
        {
          result = false;
        }
      }
      return result;
    }

    private static readonly string ownClassName = "SQLServerDatabase";

    private SqlConnection connection = new SqlConnection();
    private bool isInTransaction = false;
  }
}
