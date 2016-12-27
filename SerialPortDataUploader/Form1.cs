using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Net.Http;
using System.Collections;

namespace SerialPortDataUploader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBoxSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBoxSerialPort.Items[comboBoxSerialPort.SelectedIndex].ToString();
        }

        private void comboBoxSerialPort_MouseClick(object sender, MouseEventArgs e)
        {
            comboBoxSerialPort.Items.Clear();
            comboBoxSerialPort.Items.AddRange(SerialPort.GetPortNames());
        }

        private void listBoxLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLog.SelectedItem != null)
                textBoxLog.Text = listBoxLog.SelectedItem.ToString();
        }

        Queue<char> DataQueue = new Queue<char>();
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            char[] tmp = new char[1000];
            int count = serialPort1.BytesToRead;
            serialPort1.Read(tmp, 0, count);
            for (int i = 0; i < count; i++)
                DataQueue.Enqueue(tmp[i]);

            while (DataQueue.Count >= 32)
            {
                if (DataQueue.Peek() == 0x26)
                {
                    char[] tmp1 = new char[100];
                    for (int i = 0; i < 32; i++)
                        tmp1[i] = DataQueue.Dequeue();

                    int temp = (tmp1[15] - 0x30) * 10 + (tmp1[16] - 0x30);
                    int fire = (tmp1[18] - 0x30);
                    int gas = (tmp1[20] - 0x30) * 100 + (tmp1[21] - 0x30) * 10 + (tmp1[22] - 0x30);

                    try
                    {
                        HTTPGet("https://gcsj.hxlxz.com/API.aspx?api=upload&name=G&value=" + gas.ToString());
                        HTTPGet("https://gcsj.hxlxz.com/API.aspx?api=upload&name=F&value=" + fire.ToString());
                        string message = HTTPGet("https://gcsj.hxlxz.com/API.aspx?api=upload&name=T&value=" + temp.ToString());
                        if (!message.Equals("{\"statu\":\"ok\"}"))
                            listBoxLogAddMessage(DateTime.Now.ToString() + "  温度：" + temp.ToString() + "摄氏度，服务器返回异常：" + message);
                        else
                            listBoxLogAddMessage(DateTime.Now.ToString() + "  温度：" + temp.ToString() + "摄氏度，已上传至服务器。");
                    }
                    catch (Exception ext)
                    {
                        listBoxLogAddMessage(DateTime.Now.ToString() + "  温度：" + temp.ToString() + "摄氏度，上传至服务器失败：" + ext.Message);
                    }
                }
                else
                    DataQueue.Dequeue();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxSerialPort.Items.Clear();
            comboBoxSerialPort.Items.AddRange(SerialPort.GetPortNames());
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
            buttonConnect.Enabled = false;
            comboBoxSerialPort.Enabled = false;
            labelStatu.ForeColor = Color.Green;
        }
        private string HTTPGet(string url)
        {
            using (var client = new HttpClient())
            {
                var responseString = client.GetStringAsync(url);
                return responseString.Result;
            }
        }
        private async Task<string> HTTPPost(string url, Dictionary<string, string> values)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();
                return responseString.ToString();
            }
        }

        delegate void listBoxLogAddMessage_Callback(string mwssage);
        private void listBoxLogAddMessage(string message)
        {
            if (listBoxLog.InvokeRequired)
            {
                listBoxLogAddMessage_Callback d = new listBoxLogAddMessage_Callback(listBoxLogAddMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                listBoxLog.Items.Add(message);
            }
        }
    }
}
