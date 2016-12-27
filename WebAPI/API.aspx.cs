using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

public partial class API : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        switch (Request["API"])
        {
            case ("upload"): DataUpload(); break;
            case ("lookup"): DataLookup(); break;
            case ("delete"): DataDelete(); break;
            case ("login"): UserLogin(); break;
            default: ShowError(); break;
        }
    }

    private void UserLogin()
    {
        DataTable table = new DataTable();
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20162";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM user WHERE UserName = '{USERNAME}' AND PassWord = '{PASSWORD}'".Replace("{USERNAME}", Request["username"]).Replace("{PASSWORD}", Request["password"]);

            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);

            adap.Fill(table);
        }
        catch (Exception e)
        {
            Response.Write("{\"statu\":\"error\",\"reason\":\"{REASON}\"}".Replace("{REASON}", e.Message));
            return;
        }
        try
        {
            if ((table.Rows[0]["username"].ToString().ToLower().Equals(Request["username"].ToString().ToLower())) && (table.Rows[0]["password"].ToString().ToLower().Equals(Request["password"].ToString().ToLower())))
                Response.Write("{\"statu\":\"ok\",\"login\":\"true\"}");
            else
                Response.Write("{\"statu\":\"ok\",\"login\":\"false\"}");
        }
        catch (Exception)
        {
            Response.Write("{\"statu\":\"ok\",\"login\":\"false\"}");
        }
    }

    private void DataDelete()
    {
        int row = 0;
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20162";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "DELETE FROM data WHERE id = '{ID}'".Replace("{ID}", Request["id"]);
            row = cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Response.Write("{\"statu\":\"error\",\"reason\":\"{REASON}\"}".Replace("{REASON}", e.Message));
            return;
        }
        if (row != 0)
            Response.Write("{\"statu\":\"ok\"}");
        else
            Response.Write("{\"statu\":\"error\",\"reason\":\"no data.\"}");
    }

    private void DataLookup()
    {
        DataTable table = new DataTable();
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20162";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM data WHERE name = '{NAME}' AND timestamp >= '{STARTTIME}' AND timestamp <= '{ENDTIME}'".Replace("{NAME}", Request["name"]).Replace("{STARTTIME}", Request["starttime"]).Replace("{ENDTIME}", Request["endtime"]);

            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);

            adap.Fill(table);
        }
        catch (Exception e)
        {
            Response.Write("{\"statu\":\"error\",\"reason\":\"{REASON}\"}".Replace("{REASON}", e.Message));
            return;
        }

        try
        {

            string text = "{\"statu\":\"ok\",\"count\":\"{COUNT}\",\"data\":[".Replace("{COUNT}", table.Rows.Count.ToString());
            for (int i = 0; i < table.Rows.Count; i++)
            {
                text += "{\"time\":\"{TIME}\",\"name\":\"{NAME}\",\"value\":\"{VALUE}\",\"id\":\"{ID}\"},";
                text = text.Replace("{TIME}", table.Rows[i]["timestamp"].ToString()).Replace("{NAME}", table.Rows[i]["name"].ToString()).Replace("{VALUE}", table.Rows[i]["value"].ToString()).Replace("{ID}", table.Rows[i]["id"].ToString());
            }
            if (table.Rows.Count != 0)
                text = text.Remove(text.Length - 1);
            text += "]}";
            Response.Write(text);
        }

        catch (Exception e)
        {
            Response.Write("{\"statu\":\"error\",\"reason\":\"{REASON}\"}".Replace("{REASON}", e.Message));
            return;
        }
    }

    private void DataUpload()
    {
        int row = 0;
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20162";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "INSERT INTO data (name, value) VALUES ('{NAME}', '{VALUE}')".Replace("{NAME}", Request["name"]).Replace("{VALUE}", Request["value"]);
            row = cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Response.Write("{\"statu\":\"error\",\"reason\":\"{REASON}\"}".Replace("{REASON}", e.Message));
            return;
        }
        if (row != 0)
            Response.Write("{\"statu\":\"ok\"}");
        else
            Response.Write("{\"statu\":\"error\",\"reason\":\"failed.\"}");
    }

    private void ShowError()
    {
        Response.Write("{\"statu\":\"error\",\"reason\":\"API not support.\"}");
    }
}