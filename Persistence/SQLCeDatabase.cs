using Basics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;

namespace Persistence
{
    /// <summary>
    /// Klasse, um eine SQLCe-Datenbank auf der 
    /// das Programm zugreift, zu verwalten
    /// </summary>
    internal class SQLCeDatabase : Database
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
                    DatabaseTools.HandleError(GetType().Name, "Open", ex, error);
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
                    DatabaseTools.HandleError(GetType().Name, "Close", ex, error);
                }
            }
            return (IsOpen == false);
        }

        public override bool LoadTable(string query, DataTable dt, Error error)
        {
            bool result = false;
            SqlCeDataAdapter adapter = null;

            adapter = new SqlCeDataAdapter(query, connection);
            try
            {
                adapter.Fill(dt);
                result = true;
            }
            catch (Exception ex)
            {
                DatabaseTools.HandleError(GetType().Name, "LoadTable", ex, error);
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
            using (SqlCeCommand cmd = new SqlCeCommand(query, connection))
            {
                result = LoadObject(cmd, bo, error);
            }

            return result;
        }

        public override bool LoadObject(string query, BaseObject bo, IEnumerable<string> fields, Error error)
        {
            bool result = false;
            using (SqlCeCommand cmd = new SqlCeCommand(query, connection))
            {
                if (UpdateParams(cmd, bo, fields))
                {
                    result = LoadObject(cmd, bo, error);
                }
                else
                {
                    DatabaseTools.HandleError(GetType().Name, "LoadObject", "Fehler beim Aktualieren der Parameter aus " + bo.ToString(), error);
                }
            }
            return result;
        }

        public override bool LoadObjects<T>(string query, BaseObjects bos, Error error)
        {
            bool result = false;
            SqlCeCommand cmd = null;
            SqlCeDataReader rdr = null;

            cmd = new SqlCeCommand(query, connection);
            try
            {
                rdr = cmd.ExecuteReader();
                result = DatabaseTools.LoadObjects<T>(rdr, query, bos, error);
            }
            catch (Exception ex)
            {
                DatabaseTools.HandleError(GetType().Name, "LoadObjects<T>", ex, error);
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
            SqlCeCommand cmd = new SqlCeCommand(query, connection);

            TryAddToTransaction(cmd);
            try
            {
                if (UpdateParams(cmd, bo, fields))
                {
                    int count = cmd.ExecuteNonQuery();
                    result = true;
                }
                else
                {
                    DatabaseTools.HandleError(GetType().Name, "Execute", "Fehler beim Aktualieren der Parameter aus " + bo.ToString(), error);
                }
            }
            catch (Exception ex)
            {
                DatabaseTools.HandleError(GetType().Name, "Execute", ex, error);
            }
            finally
            {
                if (!isInTransaction)
                {
                    cmd.Dispose();
                }
            }

            return result;
        }

        public override bool StartTransaction(Error error)
        {
            Debug.Assert(isInTransaction == false);
            actualTransaction = connection.BeginTransaction();
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
                if (successful)
                {
                    actualTransaction.Commit();
                }
                else
                {
                    actualTransaction.Rollback();
                }
                result = true;
            }
            catch (Exception ex)
            {
                DatabaseTools.HandleError(GetType().Name, "FinishTransaction", ex, error);
            }
            finally
            {
                actualTransaction.Dispose();
                actualTransaction = null;
                isInTransaction = false;
            }
            return result;
        }
        #endregion

        private bool LoadObject(SqlCeCommand cmd, BaseObject bo, Error error)
        {
            bool result = false;
            SqlCeDataReader rdr = null;

            try
            {
                rdr = cmd.ExecuteReader();
                result = DatabaseTools.LoadObject(rdr, bo, error);
            }
            catch (Exception ex)
            {
                DatabaseTools.HandleError(GetType().Name, "LoadObject", ex, error);
            }
            finally
            {
                if (rdr != null)
                    rdr.Dispose();
            }
            return result;
        }

        private static bool UpdateParams(SqlCeCommand command, BaseObject bo, IEnumerable<string> fields)
        {
            bool result = true;
            foreach (var field in fields)
            {
                string param = "@" + field.ToLower();
                object obj = bo.GetValue(field);
                if (obj != null)
                {
                    command.Parameters.AddWithValue(param, obj);
                }
                else
                {
                    command.Parameters.AddWithValue(param, DBNull.Value);
                }
            }
            return result;
        }

        private void TryAddToTransaction(SqlCeCommand cmd)
        {
            if (isInTransaction)
            {
                cmd.Transaction = actualTransaction;
            }
        }

        private readonly SqlCeConnection connection = new SqlCeConnection();
        private SqlCeTransaction actualTransaction = null;
        private bool isInTransaction = false;
    }
}
