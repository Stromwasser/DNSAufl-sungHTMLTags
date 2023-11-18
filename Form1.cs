using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNSAuflösungHTMLTags
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Logik für "Textdatei öffnen"
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //textBox1.Text = ofd.FileName;

                string filePath = ofd.FileName;
                string[] addresses = File.ReadAllLines(filePath);

                foreach (string address in addresses)
                {
                    // Запускаем задачу для каждого адреса
                    Task.Run(() => ProcessAddress(address)).Wait();
                }
            }
            else
            {
                textBox1.Text = "Keine Datei ausgewählt";
            }
        }

        private void ProcessAddress(string address)
        {
            try
            {
              
                IPHostEntry host = Dns.GetHostEntry(address);

               string dnsAddress = address;
                UpdateTextBox1($"{dnsAddress}");

               string ipAddresses = string.Join(", ", host.AddressList.Select(ip => ip.ToString()));
                UpdateTextBox2($"{ipAddresses}");


                using (HttpClient client = new HttpClient())
                {
                    string html = client.GetStringAsync($"http://{address}").Result;

                    UpdateTextBox3($"{html}");
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, например, выводим ее в консоль
                Console.WriteLine($"Error processing {address}: {ex.Message}");
            }
        }
        
        private void UpdateTextBox1(string text)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.BeginInvoke((MethodInvoker)delegate
                {
                    textBox1.AppendText(text + Environment.NewLine);
                });
            }
            else
            {
                textBox1.AppendText(text + Environment.NewLine);
            }
        }
        private void UpdateTextBox2(string text)
        {
            if (textBox2.InvokeRequired)
            {
                textBox2.BeginInvoke((MethodInvoker)delegate
                {
                    textBox2.AppendText(text + Environment.NewLine);
                });
            }
            else
            {
                textBox2.AppendText(text + Environment.NewLine);
            }
        }

        private void UpdateTextBox3(string text)
        {
            if (textBox3.InvokeRequired)
            {
                textBox3.BeginInvoke((MethodInvoker)delegate
                {
                    textBox3.AppendText(text + Environment.NewLine);
                });
            }
            else
            {
                textBox3.AppendText(text + Environment.NewLine);
            }
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            // Logik für "Abbrechen"
            // Code
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Logik für "Reinigen"
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
        }
    }
}
