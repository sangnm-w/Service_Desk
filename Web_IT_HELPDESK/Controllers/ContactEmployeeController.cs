using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;

namespace Web_IT_HELPDESK.Controllers
{
    public class ContactEmployeeController : Controller
    {
        //
        // GET: /ContactEmployee/

        private string connectionString = "Provider = OraOLEDB.Oracle; " +
           "Data Source = (DESCRIPTION = (CID = GTU_APP)(ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 52.213.3.78)(PORT = 1521)))(CONNECT_DATA = (SERVICE_NAME=CJFMS))); " +
           "User Id = CJFMS; Password = cjfms; ";


        public ActionResult ContactList()
        {
            var source = data_table_test();
            return View(source);
        }

        private DataTable data_table = new DataTable();
        private DataTable data_table_test()
        {
            string str_sql = "select count(1) from dual";
                  using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command1 = new OleDbCommand(str_sql);

                // Set the Connection to the new OleDbConnection.
                command1.Connection = connection;
                // Open the connection and execute the insert command. 
                try
                {
                    connection.Open();
                    OleDbDataAdapter sqldap1 = new OleDbDataAdapter(str_sql, connection);
                    DataSet ds = new DataSet();
                    ds.Clear();
                    sqldap1.Fill(ds, "Table_MOC");
                    data_table = ds.Tables["Table_MOC"];
                    connection.Close();
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return data_table;
        }

    }
}
