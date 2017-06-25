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
            case ("contralset"): ContralSet(); break;
            case ("contralget"): ContralGet(); break;
            case ("contralget1"): ContralGet(true); break;
            default: ShowError(); break;
        }
    }

    private void ContralSet()
    {
        int row = 0;
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20171";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "UPDATE config SET time = {TIME}".Replace("{TIME}", Request["time"]);
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

    private void ContralGet(bool flag = false)
    {
        DataTable table = new DataTable();
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20171";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM config";

            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);

            adap.Fill(table);
            string text;
            if (!flag)
                text = "{\"statu\":\"ok\",\"time\":\"{TIME}\"}".Replace("{TIME}", table.Rows[0]["time"].ToString());
            else
                text = table.Rows[0]["time"].ToString();
            Response.Write(text);
        }
        catch (Exception e)
        {
            Response.Write("{\"statu\":\"error\",\"reason\":\"{REASON}\"}".Replace("{REASON}", e.Message));
            return;
        }
    }

    private void UserLogin()
    {
        DataTable table = new DataTable();
        try
        {
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20171";
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
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20171";
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
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20171";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM data WHERE timestamp >= '{STARTTIME}' AND timestamp <= '{ENDTIME}'".Replace("{NAME}", Request["name"]).Replace("{STARTTIME}", Request["starttime"]).Replace("{ENDTIME}", Request["endtime"]);

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
                text += "{\"time\":\"{TIME}\",\"lightz\":\"{LIGHTZ}\",\"lightx\":\"{LIGHTX}\",\"lightl\":\"{LIGHTL}\",\"tempz\":\"{TEMPZ}\",\"tempx\":\"{TEMPX}\",\"templ\":\"{TEMPL}\",\"wetz\":\"{WETZ}\",\"wetx\":\"{WETX}\",\"wetl\":\"{WETL}\",\"id\":\"{ID}\"},";
                text = text.Replace("{LIGHTZ}", table.Rows[i]["lightz"].ToString()).Replace("{LIGHTX}", table.Rows[i]["lightx"].ToString()).Replace("{LIGHTL}", table.Rows[i]["lightl"].ToString()).Replace("{TEMPZ}", table.Rows[i]["tempz"].ToString()).Replace("{TEMPX}", table.Rows[i]["tempx"].ToString());
                text = text.Replace("{TEMPL}", table.Rows[i]["templ"].ToString()).Replace("{WETZ}", table.Rows[i]["wetz"].ToString()).Replace("{WETX}", table.Rows[i]["wetx"].ToString()).Replace("{WETL}", table.Rows[i]["wetl"].ToString()).Replace("{TIME}", table.Rows[i]["timestamp"].ToString()).Replace("{ID}", table.Rows[i]["id"].ToString());
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
            string connectionString = "server=localhost;user id=user;Password=password;persistsecurityinfo=True;database=engineeringpractice_20171";
            MySqlConnection mysqlConn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            mysqlConn.Open();
            cmd = mysqlConn.CreateCommand();
            cmd.CommandText = "INSERT INTO data (lightz, lightx, lightl, tempz, tempx, templ, wetz, wetx, wetl) VALUES ('{LIGHTZ}', '{LIGHTX}', '{LIGHTL}', '{TEMPZ}', '{TEMPX}', '{TEMPL}', '{WETZ}', '{WETX}', '{WETL}')".Replace("{LIGHTZ}", Request["LIGHTZ"]).Replace("{LIGHTX}", Request["LIGHTX"]);
            cmd.CommandText = cmd.CommandText.Replace("{LIGHTL}", Request["LIGHTL"]).Replace("{TEMPZ}", Request["TEMPZ"]).Replace("{TEMPX}", Request["TEMPX"]).Replace("{TEMPL}", Request["TEMPL"]).Replace("{WETZ}", Request["WETZ"]).Replace("{WETX}", Request["WETX"]).Replace("{WETL}", Request["WETL"]);
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