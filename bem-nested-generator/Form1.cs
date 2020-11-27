using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace bem_nested_generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private ArrayList getAllClass(string html)
        {
            ArrayList allClass = new ArrayList();

            var matchesAllClasses = Regex.Matches(html, "class = \"([\\sA-Za-z_]{1,})\"");

            foreach (Match matchclass in matchesAllClasses)
            {
                allClass.Add(matchclass.Groups[1].Value);
            }

            return allClass;
        }

        private ArrayList getAllBlock(string pathToDir, ArrayList classes)
        {
            ArrayList allBlocks = new ArrayList();

            foreach (var cls in classes)
            {
                Match block = Regex.Match((string)cls, "[a-z]{1,}");

                if (block.Value != "" && !allBlocks.Contains(block.Value))
                {
                    allBlocks.Add(block.Value);

                    if (!Directory.Exists(pathToDir + "\\" + block.Value))
                    {
                        Directory.CreateDirectory(pathToDir + "\\" + block.Value);
                    }
                }

            }

            return allBlocks;
        }

        private ArrayList getElements(string pathToDir, ArrayList blocks, ArrayList classes)
        {
            ArrayList elements = new ArrayList();

            foreach (var cls in classes)
            {
                foreach (var block in blocks)
                {
                    Match element = Regex.Match((string)cls, block + "(__[a-z]{1,})");

                    if (element.Value != "")
                    {
                        Directory.CreateDirectory(pathToDir + "\\" + block + "\\" + element.Groups[1].Value);
                        File.WriteAllText(pathToDir + "\\" + block + "\\" + element.Groups[1].Value + "\\" + element + ".css", "." + element + " {\r\n" + "}");
                        elements.Add(element.Value);
                    }
                }


            }

            return elements;
        }

        private ArrayList getBlockModificator(string pathToDir, ArrayList blocks, ArrayList classes)
        {
            ArrayList blockModificators = new ArrayList();

            foreach (var cls in classes)
            {
                foreach (var block in blocks)
                {
                    var modificators = Regex.Matches((string)cls, block + "(_[a-z]{1,})_[a-z]{1,}");

                    foreach (Match modificator in modificators)
                    {
                        if (modificator.Value != "")
                        {
                            Directory.CreateDirectory(pathToDir + "\\" + block + "\\" + modificator.Groups[1].Value);
                            File.WriteAllText(pathToDir + "\\" + block + "\\" + modificator.Groups[1].Value + "\\" + modificator + ".css", "." + modificator + " {\r\n" + "}");
                            blockModificators.Add(modificator.Value);
                        }
                    }


                }


            }

            return blockModificators;
        }

        private ArrayList getElementModificator(string pathToDir, ArrayList blocks, ArrayList classes)
        {
            ArrayList elementModificators = new ArrayList();

            foreach (var cls in classes)
            {
                foreach (var block in blocks)
                {
                    var modificators = Regex.Matches((string)cls, block + "(__[a-z]{1,})(_[a-z]{1,})(_[a-z]{1,})");

                    foreach (Match modificator in modificators)
                    {
                        if (modificator.Value != "")
                        {
                            if (!Directory.Exists(pathToDir + "\\" + block + "\\" + modificator.Groups[1].Value + "\\" + modificator.Groups[2].Value))
                            {
                                Directory.CreateDirectory(pathToDir + "\\" + block + "\\" + modificator.Groups[1].Value + "\\" + modificator.Groups[2].Value);
                            }

                            File.WriteAllText(pathToDir + "\\" + block + "\\" + modificator.Groups[1].Value + "\\" + modificator.Groups[2].Value + "\\" + modificator + ".css", "." + modificator + " {\r\n" + "}");

                            elementModificators.Add(modificator.Value);
                        }
                    }

                }


            }

            return elementModificators;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string ALL_HTML = richTextBox1.Text;
                string PATH_TO_OUT_DIR = textBox1.Text;

                if (ALL_HTML == "")
                {
                    MessageBox.Show("Вставьте разметку");
                    return;
                }

                if (PATH_TO_OUT_DIR == "Выберите выходную директорию")
                {
                    MessageBox.Show("Выберите выходную директорию");
                    return;
                }

                if (!Directory.Exists(PATH_TO_OUT_DIR))
                {
                    MessageBox.Show("Выходная директория не найдена");
                    return;
                }

                ArrayList allClass = getAllClass(ALL_HTML);
                ArrayList allBlock = getAllBlock(PATH_TO_OUT_DIR, allClass);
                ArrayList allElements = getElements(PATH_TO_OUT_DIR, allBlock, allClass);
                ArrayList allBlockModificator = getBlockModificator(PATH_TO_OUT_DIR, allBlock, allClass);
                ArrayList allElementsModificator = getElementModificator(PATH_TO_OUT_DIR, allBlock, allClass);

                foreach (var block in allBlock)
                {
                    foreach (var element in allElements)
                    {


                        if (((string)element).IndexOf((string)block) == 0)
                        {
                            Match targetElement = Regex.Match((string)element, block + "(__[a-z]{1,})");

                            File.AppendAllText(PATH_TO_OUT_DIR + "\\" + (string)block + "\\" + (string)block + ".css", String.Format("@import url(./{0}/{1}.css);\r\n", targetElement.Groups[1].Value, (string)element));
                        }

                    }

                    foreach (var modificator in allElementsModificator)
                    {


                        if (((string)modificator).IndexOf((string)block) == 0)
                        {
                            Match targetModificator = Regex.Match((string)modificator, block + "(__[a-z]{1,})(_[a-z]{1,})(_[a-z]{1,})");

                            File.AppendAllText(PATH_TO_OUT_DIR + "\\" + (string)block + "\\" + (string)block + ".css", String.Format("@import url(./{0}/{1}/{2}.css);\r\n", targetModificator.Groups[1].Value, targetModificator.Groups[2].Value, (string)modificator));
                        }

                    }

                    File.AppendAllText(PATH_TO_OUT_DIR + "\\" + (string)block + "\\" + (string)block + ".css", "\r\n." + (string)block + " {\r\n}");
                }
            }

            catch (Exception error) 
            {
                MessageBox.Show(error.Message);
            }

            
        }
    }
}
