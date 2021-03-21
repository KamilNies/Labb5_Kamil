using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;

namespace ImageScraper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void extractButton_Click(object sender, EventArgs e)
        {
            //Checks to ensure a valid input
            if (textBox1.Text == string.Empty || textBox1.Text == " ")
            {
                MessageBox.Show("Textbox cannot be empty.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = string.Empty;
                return;
            }

            if (!Uri.IsWellFormedUriString(textBox1.Text, UriKind.Absolute))
            {
                MessageBox.Show("URL is not in correct format", "Warning",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Text = string.Empty;
                return;
            }

            listBox1.Items.Clear();
            CallingMethodsAsync callingHandler = new CallingMethodsAsync();
            List<string> imagePaths = await callingHandler.DownloadAsync(textBox1.Text);

            foreach (var imagePath in imagePaths)
            {
                listBox1.Items.Add(imagePath);
            }

            if (imagePaths.Count == 0)
            {
                MessageBox.Show("No images found", "Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            label1.Text = string.Format("Found {0} images.", imagePaths.Count);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                //When choosing your owns items
                folderBrowserDialog1.ShowDialog();
            }
            else
            {
                //save all images
            }

            folderBrowserDialog1.ShowDialog();
        }

        //Remove later maybe
        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                System.Text.StringBuilder copy_buffer = new System.Text.StringBuilder();
                foreach (object item in listBox1.SelectedItems)
                    copy_buffer.AppendLine(item.ToString());
                if (copy_buffer.Length > 0)
                    Clipboard.SetText(copy_buffer.ToString());
            }
        }


    }
}
