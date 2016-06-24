using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jdk_Path_Config
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Tools.RegistryStrValue("", label1.Text);
            textBox2.Text = Tools.RegistryStrValue("", label2.Text);
            textBox3.Text = Tools.RegistryStrValue("", label3.Text);

            string data = label1.Text + "@# " + textBox1.Text + "\r\n";
            data += label2.Text + "@# " + textBox2.Text + "\r\n";
            data += label3.Text + "@# " + textBox3.Text + "\r\n";

            Tools.SaveProcess(data);

            //JDK_Config(@"C:\Program Files\Java"); 
            comboBox1.SelectedIndex = 0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            JDK_Config(comboBox1.Text); 
        }

        private void JDK_Config(String path)
        {
            HOME = "";
            PATH = "";
            CLASSPATH = "";

            // 检测路径下的JDK环境变量
            CheckFiles(path);

            // 保存环境变量信息到注册表中
            Tools.RegistrySave("", label1.Text, textBox1.Text);
            Tools.RegistrySave("", label2.Text, textBox2.Text);
            Tools.RegistrySave("", label3.Text, textBox3.Text);

            if (!HOME.Equals("") && !PATH.Equals("") && !CLASSPATH.Equals(""))
                MessageBox.Show("恭喜，JDK环境变量，已配置完成！");
            else MessageBox.Show("环境变量配置失败！请选择JDK文件安装路径。\r\n或拖动JDK所在文件夹目录至此！");

        }

        /// <summary>
        /// 获取目录path下所有子文件名
        /// </summary>
        public void CheckFiles(String path)
        {
            try
            {
                StringBuilder str = new StringBuilder("");
                if (System.IO.Directory.Exists(path))
                {
                    //所有子文件名
                    string[] files = System.IO.Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        JDK_FileCheckProcess(file);
                    }

                    //所有子目录名
                    string[] Dirs = System.IO.Directory.GetDirectories(path);
                    foreach (string dir in Dirs)
                    {
                        CheckFiles(dir);  //子目录下所有子文件名
                    }
                }
            }
            catch (Exception ex) { }
        }

        string HOME = "", PATH = "", CLASSPATH = "";

        // JDK文件检测逻辑
        public void JDK_FileCheckProcess(string file)
        {
            String name = System.IO.Path.GetFileName(file);
            if (name.Equals("java.exe"))
            {
                if(file.EndsWith(@"\jre\bin\java.exe")) HOME = file.Replace(@"\jre\bin\java.exe", "") + ";";
                else if (file.EndsWith(@"\bin\java.exe")) HOME = file.Replace(@"\bin\java.exe", "") + ";";

                PATH = file.Replace(@"\java.exe", "") + ";";
                if (!textBox1.Text.Contains(HOME)) textBox1.Text = HOME + textBox1.Text;
                if (!textBox2.Text.Contains(PATH)) textBox2.Text = PATH + textBox2.Text;

                string Jar = HOME + @"\lib\tools.jar";
                if (System.IO.Directory.Exists(Jar))
                {
                    String lib = HOME + @"\lib" + ";";
                    lib += HOME + @"\lib\tools.jar";

                    CLASSPATH = lib + ";";
                    if (!textBox3.Text.Contains(CLASSPATH)) textBox3.Text = CLASSPATH + textBox3.Text;
                }
            }

            // 检测CLASSPATH路径
            if (name.Equals("tools.jar"))
            {
                CLASSPATH = file.Replace(@"\tools.jar", "") + ";" + file + ";";
                if (!textBox3.Text.Contains(CLASSPATH)) textBox3.Text = CLASSPATH + textBox3.Text;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            comboBox1.Text = Tools.dragDropDir(e);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            Tools.dragEnter(e);
        }


    }
}
