using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        //List<string> srcLinks = new List<string>();
        //BindingSource bs = new BindingSource(); //Not sure we need it

        public Form1()
        {
            InitializeComponent();
            //bs.DataSource = srcLinks;
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
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

            List<string> srcURLs = callingHandler.DownloadAsync(textBox1.Text).Result;

            if (srcURLs.Count == 0)
            {
                MessageBox.Show("No images found", "Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var srcURL in srcURLs)
            {
                listBox1.Items.Add(srcURL);
            }

            //srcLinks.Add(textBox1.Text);
            //listBox1.DataSource = bs;
            //bs.ResetBindings(false);

            label1.Text = string.Format("Found {0} images.", srcURLs.Count);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Save selected items?", "Save",
                   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    listBox1.Items.Add("Test Test.1"); //do something
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }

                return;
            }

            listBox1.Items.Add("Test Test.");
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
