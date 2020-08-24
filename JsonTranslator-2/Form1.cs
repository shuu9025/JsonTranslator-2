
// わあ！dynamicがいっぱいだね！静的型付けの恩恵はどこ？？？？

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JsonTranslator_2
{
    public partial class Form1 : Form
    {

        JObject Data;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void AddDictionary(JObject dict, string parent)
        {
            foreach (KeyValuePair<string, dynamic> item in dict.ToObject<Dictionary<string, dynamic>>())
            {
                if (item.Value is JArray)
                {
                    AddArray(item.Value, parent + "/" + item.Key);
                }
                else if (item.Value is JObject)
                {
                    AddDictionary(item.Value, parent + "/" + item.Key);
                }
                else if (item.Value is string)
                {
                    listBox1.Items.Add(parent + "/" + item.Key);
                }
            }
        }
        private void AddArray(JArray array, string parent)
        {
            int i = 0;
            foreach (dynamic item in array)
            {
                if (item is JArray)
                {
                    AddArray(item, parent + $"[{i}]");
                }
                else if (item is JObject)
                {
                    AddDictionary(item, parent + $"[{i}]");
                }
                else if (item is string)
                {
                    listBox1.Items.Add(parent + $"[{i}]");
                }
                i++;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            richTextBox2.Text = "";
            richTextBox1.Text = "";
            textBox4.Text = "";
            int _;
            try
            {
                Data = JObject.Parse(textBox1.Text);
                textBox1.Text = JsonConvert.SerializeObject(Data, Formatting.Indented);
                foreach (KeyValuePair<dynamic, dynamic> kv in Data.ToObject<Dictionary<dynamic, dynamic>>())
                {
                    if (kv.Key is string &! int.TryParse(kv.Key, out _))
                    {
                        if (kv.Key.Contains("/"))
                        {
                            ShowError("contains \"/\". Please remove it.", kv.Key);
                            return;
                        }
                        if (kv.Key.Contains(".")) {
                            MessageBox.Show($"Key {kv.Key} contains \".\". In this case, there may be some unexpected problems.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        if (kv.Value is string)
                        {
                            listBox1.Items.Add($"{kv.Key}");
                        }
                        else if (kv.Value is JArray)
                        {
                            AddArray(kv.Value, kv.Key);
                        }
                        else if (kv.Value is JObject)
                        {
                            AddDictionary(kv.Value, kv.Key);
                        }
                        else
                        {
                            ShowError("is un-supported value. Please use String, Array (aka List) or Dictionary.", kv.Key);
                            return;
                        }
                    } else
                    {
                        ShowError("is un-supported key. Please use String. You can't use number-only keys.", kv.Key);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Json parse error: {ex.Message}");
            }

            splitContainer1.Enabled = true;
            toolStripStatusLabel1.Text = "Next, choose an item from \"Translations\" list that you want to translate.";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox4.Text = listBox1.SelectedItem.ToString();
            JToken value = Data.SelectToken(textBox4.Text.Replace("/", "."));
            richTextBox1.Text = (string) value;
            toolStripStatusLabel1.Text = "Finally, input translation of \"Original Text\" to \"Translated Text\" and click \"Apply\" (or Ctrl + Enter)";
        }

        private void ShowError(string error, string key)
        {
            MessageBox.Show($"Key \"{key}\" {error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            listBox1.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var value in Data.SelectTokens("$." + textBox4.Text.Replace("/", ".")).ToList())
            {
                if (value == Data)
                    Data = (JObject)JToken.FromObject(richTextBox2.Text);
                else
                    value.Replace(JToken.FromObject(richTextBox2.Text));
            }

            textBox1.Text = JsonConvert.SerializeObject(Data, Formatting.Indented);

            try
            {
                listBox1.SelectedIndex += 1;
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            richTextBox2.Focus();
            richTextBox2.SelectAll();
        }

        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.LineFeed) && (ModifierKeys == Keys.Control))
            {
                e.Handled = true;
                button2.PerformClick();
            } else if (e.KeyChar == '{' || e.KeyChar == '}')
            {
                MatchCollection mc = Regex.Matches(richTextBox2.Text, @"{.*?}");
                richTextBox1.BackColor = Color.White;
                foreach (Match m in mc)
                {
                    int startindex = 0;
                    while (startindex < richTextBox2.TextLength)
                    {
                        int wordstartIndex = richTextBox2.Find(m.Value, startindex, RichTextBoxFinds.None);
                        if (wordstartIndex != -1)
                        {
                            richTextBox2.SelectionStart = wordstartIndex;
                            richTextBox2.SelectionLength = m.Value.Length;
                            richTextBox2.SelectionBackColor = Color.FromArgb(100, 175, 190);
                        }
                        else
                            break;
                        startindex += wordstartIndex + m.Value.Length;
                    }
                }
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
