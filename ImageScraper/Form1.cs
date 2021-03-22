using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;

namespace ImageScraper
{
    public partial class Form1 : Form
    {
        //Class Fields
        private string url = string.Empty;
        private List<string> imageList = new List<string>();
        private List<byte[]> byteList = new List<byte[]>();
        private string fileExtension;

        public Form1()
        {
            InitializeComponent();
        }

        private async void extractButton_Click(object sender, EventArgs e)
        {
            //Checks that ensures a valid input
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

            imageList.Clear();
            listBox1.Items.Clear();
            fileExtension = null;
            url = textBox1.Text;

            //Async calling methods
            CallingMethodsAsync caller = new CallingMethodsAsync();
            try
            {
                imageList = await caller.DownloadAsync(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            foreach (var imageLink in imageList)
            {
                listBox1.Items.Add(imageLink);
            }

            if (imageList.Count == 0)
            {
                MessageBox.Show("No images found", "Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            label1.Text = string.Format("Found {0} images.", imageList.Count);
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No images found. Cannot save images", "Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            byteList.Clear();
            fileExtension = null;

            if (listBox1.SelectedItems.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Save only selected items?", "Save",
                   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    folderBrowserDialog1.ShowDialog();
                    string path = folderBrowserDialog1.SelectedPath;
                    var selectedImages = lbMyListBox.Items.Cast<String>().ToList();

                    if (path == string.Empty)
                    {
                        return;
                    }

                    FormatListItems(imageList);
                }
                else if (dialogResult == DialogResult.No)
                {
                    try
                    {
                        await SaveAllImages(imageList);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Warning", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                try
                {
                    folderBrowserDialog1.ShowDialog();
                    string path = folderBrowserDialog1.SelectedPath;

                    if (path == string.Empty)
                    {
                        return;
                    }

                    await SaveAllImages(imageList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
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

        private async Task SaveAllImages(List<string> imageList)
        {
            //Have user set folderBrowserDialog1.SelectedPath property
            folderBrowserDialog1.ShowDialog();
            string path = folderBrowserDialog1.SelectedPath;

            if (path == string.Empty)
            {
                return;
            }

            FormatListItems(imageList);

            CallingMethodsAsync caller = new CallingMethodsAsync();
            ImageValidationMethods validator = new ImageValidationMethods();

            for (int i = 0; i < imageList.Count; i++)
            {
                try
                {
                    byteList.Add(await caller.DownloadImageAsync(imageList[i]));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                label1.Text = string.Format("Downloaded {0} of {1} images.", byteList.Count, imageList.Count);
            }

            int counter = 0;
            for (int i = 0; i < byteList.Count; i++)
            {
                if (byteList[i].Length > 1)
                {
                    if (validator.IsValidImageFormat(byteList[i]))
                    {
                        fileExtension = validator.ReturnImageExtension(byteList[i]);
                        try
                        {
                            await File.WriteAllBytesAsync(Path.Combine(path, "image" + i + fileExtension), byteList[i]);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        counter++;
                    }
                }
                else
                {
                    counter++;
                }
            }

            if (counter != 0)
            {
                MessageBox.Show(counter == 1 ? $"{counter} image could not be saved into folder" :
                    $"{counter} images could not be saved into folder",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FormatListItems(List<string> imageList)
        {
            for (int i = 0; i < imageList.Count; i++)
            {
                if (imageList[i].Substring(0, 7).Equals("http://") || imageList[i].Substring(0, 8).Equals("https://"))
                {
                    continue;
                }
                else if (imageList[i].Substring(0, 2).Equals("//"))
                {
                    continue;
                }
                else
                {
                    //Could possible be wrong for websites where the user is redirected such as wikipedia.
                    //Should work for gp.se though
                    string first = url.Last() == '/' ? url[0..(url.Length - 1)] : url;
                    string second = imageList[i].First() == '/' ? imageList[i][1..(imageList[i].Length)] : imageList[i];

                    imageList[i] = first + "/" + second;
                }
            }
        }

        
    }
}
