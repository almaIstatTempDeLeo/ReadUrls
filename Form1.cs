using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadUrls
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool stop = false;
        private void cmdStart_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            cmdStart.Enabled = false;
            cmdStop.Enabled = true;
            try
            { 
                stop = false;
                string lcUrl = txtUrl.Text;// @"http://sdmx.istat.it/SDMXWS/rest/data/IT1,41_993,1.0/A.../ALL/?detail=full&startPeriod=2006-01-01&endPeriod=2006-12-31&dimensionAtObservation=TIME_PERIOD";
                HttpWebRequest loHttp =  (HttpWebRequest)WebRequest.Create(lcUrl);
                loHttp.Timeout = 100000;     // 100 secs
                loHttp.UserAgent = "Sample Web Client";
                HttpWebResponse loWebResponse = (HttpWebResponse)loHttp.GetResponse();
                Encoding enc = Encoding.GetEncoding(1252);   
                StreamReader loResponseStream = new StreamReader(loWebResponse.GetResponseStream(), enc);
                char[] buffer = new char[1024 * 1024];
                int readed =0;
                fs = File.Create(txtFile.Text);
                //StringBuilder sb = new StringBuilder();
                int totWrited = 0;
                while ((readed=loResponseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    if (stop)
                        break;
                    //sb.Append(new string(buffer));
                    string toWrite = (new string(buffer)).Substring(0,readed);
                    fs.Write(Encoding.UTF8.GetBytes(toWrite),0, toWrite.Length);
                    fs.Flush();
                    totWrited += readed;
                    txtRep.Text = " Caratteri letti: " + totWrited.ToString();
                }
                txtRep.Text +=Environment.NewLine+ " Fine elaborazione.";
                fs.Close();
                loWebResponse.Close();
                loResponseStream.Close();
            }
            catch (Exception  ex)
            {
                if(fs!=null)
                    fs.Close();
                txtRep.Text = Environment.NewLine+ " Errore: " + ex.ToString();
                cmdStart.Enabled = true;
                cmdStop.Enabled = false;
            }
            cmdStart.Enabled = true;
            cmdStop.Enabled = false;
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            stop = true;
            cmdStart.Enabled = true;
            cmdStop.Enabled = false;

        }
    }
}
