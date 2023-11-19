using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNSAuflösungHTMLTags
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts;
        private List<Task> tasks;

        public Form1()
        {
            InitializeComponent();
            cts = new CancellationTokenSource();
            tasks = new List<Task>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                string[] addresses = File.ReadAllLines(filePath);

                foreach (string address in addresses)
                {
                    tasks.Add(Task.Run(() => ProcessAddress(address, cts.Token)));
                    Application.DoEvents();
                }

                Task.WhenAll(tasks).ContinueWith(_ =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Cursor = Cursors.Default;
                    });
                });
            }
            else
            {
                textBox1.Text = "Keine Datei ausgewählt";
                this.Cursor = Cursors.Default;
            }
        }

        private void ProcessAddress(string address, CancellationToken t)
        {
            try
            {
                t.ThrowIfCancellationRequested();
                IPHostEntry host = Dns.GetHostEntry(address);

                string dnsAddress = address;
                UpdateTextBox1($"{dnsAddress}");

                string ipAddresses = string.Join(", ", host.AddressList.Select(ip => ip.ToString()));
                UpdateTextBox2($"{ipAddresses}");


                using (HttpClient client = new HttpClient())
                {
                    t.ThrowIfCancellationRequested();
                    string html = client.GetStringAsync($"http://{address}").Result;
                    string pattern = @"<(html|table|script)[^>]*>";
                    MatchCollection matches = Regex.Matches(html, pattern, RegexOptions.Singleline);
                    foreach (Match match in matches)
                    {
                        t.ThrowIfCancellationRequested();
                        UpdateTextBox3($"{match.Value}");
                    }

                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Der Vorgang wurde vom Benutzer abgebrochen.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
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
            cts.Cancel();
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
