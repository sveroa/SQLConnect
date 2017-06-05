using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Globalization;

namespace SQLConnect
{
    class Program
    {
        private static string GetConnectAs(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("SELECT SUSER_SNAME()", conn);
            string user = (string)cmd.ExecuteScalar();
            return user;
        }

        private static string GetServerProperty(SqlConnection conn, string prop)
        {
            SqlCommand cmd = new SqlCommand("SELECT SERVERPROPERTY('" + prop + "')", conn);
            var val = cmd.ExecuteScalar();
            return val.ToString();
        }

        private static string GetServerCollation(SqlConnection conn)
        {
            return GetServerProperty(conn, "Collation");
        }

        private static string GetServerEdition(SqlConnection conn)
        {
            string tmp = GetServerProperty(conn, "Edition");
            string engVer = GetServerProperty(conn, "EngineEdition");
            int ver = Convert.ToInt16(engVer);
            switch (ver)
            {
                case 1: tmp += " (Personal)"; break;
                case 2: tmp += " (Standard)"; break;
                case 3: tmp += " (Enterprise)"; break;
                case 4: tmp += " (Express)"; break;
            }
            return tmp;
        }
        
        private static string SQLVersion(SqlConnection conn)
        {
            string tmp = conn.ServerVersion;
            string[] serverVersionDetails = tmp.Split(new string[] { "." }, StringSplitOptions.None); 
            int versionNumber = int.Parse(serverVersionDetails[0]); 
            switch (versionNumber) 
            { 
                case 8:tmp = "SQL Server 2000"; break; 
                case 9: tmp = "SQL Server 2005"; break; 
                case 10: tmp = "SQL Server 2008"; break;
                case 11: tmp = "SQL Server 2012"; break;
                case 12: tmp = "SQL Server 2014"; break;
                case 13: tmp = "SQL Server 2016"; break;
                default: tmp = string.Format("SQL Server {0}", versionNumber.ToString()); break; 
            }
            return tmp;
        }

        static void Main(string[] args)
        {
            string connStr = args[0];

            var cmd = new Arguments(args);

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    Console.WriteLine("Connecting using '" + connStr + "'");

                    conn.Open();

                    Console.WriteLine("Successfully connected as '" + GetConnectAs(conn) + "'");
                    Console.WriteLine("Client name: " + conn.WorkstationId);
                    Console.WriteLine("Server version: " + conn.ServerVersion + " ==> " + SQLVersion(conn));
                    Console.WriteLine("Server Edition: " + GetServerEdition(conn));
                    Console.WriteLine("Server collation: " + GetServerCollation(conn));

                    conn.Close();

                    Console.WriteLine("Successfully disconnected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

        }
    }
}
