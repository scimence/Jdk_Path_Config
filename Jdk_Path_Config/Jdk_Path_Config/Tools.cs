using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jdk_Path_Config
{
    class Tools
    {

        /// <summary>
        /// 保存数据data到文件处理过程，返回值为保存的文件名
        /// </summary>
        public static String SaveProcess(String data)
        {
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"Jdk环境变量备份\";    //设置当前目录
            if (!System.IO.Directory.Exists(CurDir)) System.IO.Directory.CreateDirectory(CurDir);   //该路径不存在时，在当前文件目录下创建文件夹"导出.."

           
            //不存在该文件时先创建
            string name = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            String filePath = CurDir + name + ".txt";
            System.IO.StreamWriter file1 = new System.IO.StreamWriter(filePath, false);     //文件已覆盖方式添加内容

            file1.Write(data);                                                              //保存数据到文件

            file1.Close();                                                                  //关闭文件
            file1.Dispose();                                                                //释放对象

            return filePath;
        }

        # region 注册表操作
        //设置软件开机启动项： RegistrySave(@"Microsoft\Windows\CurrentVersion\Run", "QQ", "C:\\windows\\system32\\QQ.exe");  
        static String Root = "Environment";
        // HKEY_CURRENT_USER\Environment

        /// <summary>
        /// 记录键值数据到注册表subkey = @"Scimence\Email\Set";
        /// </summary>
        public static void RegistrySave(string subkey, string name, object value)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keyCur = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root, true);
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root + @"\" + subkey, true);
            if (keySet == null) keySet = keyCur.CreateSubKey(subkey);   //键不存在时创建

            keySet.SetValue(name, value);   //保存键值数据
        }

        /// <summary>
        /// 获取注册表subkey下键name的字符串值
        /// <summary>
        public static string RegistryStrValue(string subkey, string name)
        {
            object value = RegistryValue(subkey, name);
            return value == null ? "" : value.ToString();
        }

        /// <summary>
        /// 获取注册表subkey下键name的值
        /// <summary>
        public static object RegistryValue(string subkey, string name)
        {
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root + @"\" + subkey, true);
            return (keySet == null ? null : keySet.GetValue(name, null));
        }

        /// <summary>
        /// 判断注册表是否含有子键subkey
        /// <summary>
        public static bool RegistryCotains(string subkey)
        {
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root + @"\" + subkey, true);
            return (keySet != null);
        }

        /// <summary>
        /// 判断注册表subkey下是否含有name键值信息
        /// <summary>
        public static bool RegistryCotains(string subkey, string name)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root + @"\" + subkey, true);

            if (keySet == null) return false;
            else return keySet.GetValueNames().Contains<string>(name);
        }

        /// <summary>
        /// 删除注册表subkey信息
        /// <summary>
        public static void RegistryRemove(string subkey)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keyCur = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root, true);
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root + @"\" + subkey, true);

            if (keySet != null) keyCur.DeleteSubKeyTree(subkey);      //删除注册表信息
        }

        /// <summary>
        /// 删除注册表subkey下的name键值信息
        /// <summary>
        public static void RegistryRemove(string subkey, string name)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Root + @"\" + subkey, true);
            if (keySet != null) keySet.DeleteValue(name, false);
        }

        # endregion


        # region 文件拖拽处理操作

        //=======================================================
        //其他相关功能
        //=======================================================
        /// <summary>
        /// 文件拖进事件处理：
        /// </summary>
        public static void dragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //判断拖来的是否是文件
                e.Effect = DragDropEffects.Link;                //是则将拖动源中的数据连接到控件
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 文件放下事件处理：
        /// 放下, 另外需设置对应控件的 AllowDrop = true; 
        /// 获取的文件名形如 "d:\tmp"
        /// </summary>
        public static string dragDropDir(DragEventArgs e)
        {
            StringBuilder filesName = new StringBuilder("");
            Array file = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            foreach (object I in file)
            {
                string str = I.ToString();

                System.IO.FileInfo info = new System.IO.FileInfo(str);
                //若为目录，则获取目录下所有子文件名
                if ((info.Attributes & System.IO.FileAttributes.Directory) != 0)
                {
                    return str;
                }
            }

            return "";
        }

        /// <summary>
        /// 文件放下事件处理：
        /// 放下, 另外需设置对应控件的 AllowDrop = true; 
        /// 获取的文件名形如 "d:\1.txt;d:\2.txt"
        /// </summary>
        public static string dragDrop(DragEventArgs e)
        {
            StringBuilder filesName = new StringBuilder("");
            Array file = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            foreach (object I in file)
            {
                string str = I.ToString();

                System.IO.FileInfo info = new System.IO.FileInfo(str);
                //若为目录，则获取目录下所有子文件名
                if ((info.Attributes & System.IO.FileAttributes.Directory) != 0)
                {
                    str = getAllFiles(str);
                    if (!str.Equals("")) filesName.Append((filesName.Length == 0 ? "" : ";") + str);
                }
                //若为文件，则获取文件名
                else if (System.IO.File.Exists(str))
                    filesName.Append((filesName.Length == 0 ? "" : ";") + str);
            }

            return filesName.ToString();
        }


        /// <summary>
        /// 判断path是否为目录
        /// </summary>
        public bool IsDirectory(String path)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            return (info.Attributes & System.IO.FileAttributes.Directory) != 0;
        }

        /// <summary>
        /// 获取目录path下所有子文件名
        /// </summary>
        public static string getAllFiles(String path)
        {
            StringBuilder str = new StringBuilder("");
            if (System.IO.Directory.Exists(path))
            {
                //所有子文件名
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (string file in files)
                    str.Append((str.Length == 0 ? "" : ";") + file);

                //所有子目录名
                string[] Dirs = System.IO.Directory.GetDirectories(path);
                foreach (string dir in Dirs)
                {
                    string tmp = getAllFiles(dir);  //子目录下所有子文件名
                    if (!tmp.Equals("")) str.Append((str.Length == 0 ? "" : ";") + tmp);
                }
            }
            return str.ToString();
        }

        ///// <summary>
        ///// 获取目录path下所有子文件名
        ///// </summary>
        //public string getAllFiles(String path)
        //{
        //    StringBuilder str = new StringBuilder("");
        //    if (System.IO.Directory.Exists(path))
        //    {
        //        //所有子文件名
        //        string[] files = System.IO.Directory.GetFiles(path);
        //        foreach (string file in files)
        //            str.Append((str.Length == 0 ? "" : ";") + file);

        //        //所有子目录名
        //        string[] Dirs = System.IO.Directory.GetDirectories(path);
        //        foreach (string dir in Dirs)
        //        {
        //            string tmp = getAllFiles(dir);  //子目录下所有子文件名
        //            if (!tmp.Equals("")) str.Append((str.Length == 0 ? "" : ";") + tmp);
        //        }
        //    }
        //    return str.ToString();
        //}


        # endregion

    }
}
