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
        volatile private int lightZ = 50, tempZ = 28, wetZ = 30;
        volatile private int lightX = 50, tempX = 28, wetX = 30;
        volatile private int lightL = 50, tempL = 28, wetL = 30;

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            char[] tmp = new char[1000];
            int count = serialPort1.BytesToRead;
            serialPort1.Read(tmp, 0, count);
            for (int i = 0; i < count; i++)
                DataQueue.Enqueue(tmp[i]);

            while (DataQueue.Count >= 11)
            {
                switch (DataQueue.Peek())
                {
                    case 'z':
                    case 'l':
                    case 'x':
                        {
                            char[] tmp1 = new char[100];
                            for (int i = 0; i < 11; i++)
                                tmp1[i] = DataQueue.Dequeue();
                            try
                            {
                                int light = (tmp1[2] - 0x30) * 10 + (tmp1[3] - 0x30);
                                int temp = (tmp1[5] - 0x30) * 10 + (tmp1[6] - 0x30);
                                int wet = (tmp1[8] - 0x30) * 10 + (tmp1[9] - 0x30);
                                switch (tmp1[0])
                                {
                                    case 'z': lightZ = light; tempZ = temp; wetZ = wet; break;
                                    case 'x': lightX = light; tempX = temp; wetX = wet; break;
                                    case 'l': lightL = light; tempL = temp; wetL = wet; break;
                                }
                            }
                            catch (Exception) { }
                            break;
                        }
                    default:
                        DataQueue.Dequeue();
                        break;
                }
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
            timer1.Start();
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
                listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
            }
        }

        bool Timer1Flag = false;
        int timer = 0, TimeSet = 5;
        private void timer1_Tick(object sender, EventArgs e)
        {
            string BaseURL = "http://gcsj.hxlxz.com/";
            //string BaseURL = "http://localhost:50999/";
            timer++;
            if (Timer1Flag)
                return;
            string tmp = HTTPGet(BaseURL + "API.aspx?api=contralget1");
            try
            {
                TimeSet = int.Parse(tmp);
            }
            catch (Exception) { }
            if (TimeSet > 0 && timer >= TimeSet)
            {
                timer -= TimeSet;
                Timer1Flag = true;
                try
                {
                    string url = BaseURL + "API.aspx?api=upload&lightZ={LIGHTZ}&lightX={LIGHTX}&lightL={LIGHTL}&tempZ={TEMPZ}&tempX={TEMPX}&tempL={TEMPL}&wetZ={WETZ}&wetX={WETX}&wetL={WETL}".Replace("{LIGHTZ}", lightZ.ToString()).Replace("{LIGHTX}", lightX.ToString());
                    url = url.Replace("{LIGHTL}", lightL.ToString()).Replace("{TEMPZ}", tempZ.ToString()).Replace("{TEMPX}", tempX.ToString()).Replace("{TEMPL}", tempL.ToString()).Replace("{WETZ}", wetZ.ToString()).Replace("{WETX}", wetX.ToString()).Replace("{WETL}", wetL.ToString());

                    string message = HTTPGet(url);
                    if (!message.Equals("{\"statu\":\"ok\"}"))
                        listBoxLogAddMessage(DateTime.Now.ToString() + "  服务器返回异常：" + message);
                    else
                        listBoxLogAddMessage(DateTime.Now.ToString() + "  已上传至服务器。");
                }
                catch (Exception ext)
                {
                    listBoxLogAddMessage(DateTime.Now.ToString() + "  上传至服务器失败：" + ext.Message);
                }
                Timer1Flag = false;
            }
        }
    }
}
