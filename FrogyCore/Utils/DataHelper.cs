using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FrogyCore.Utils
{
    public enum FileOperatingMethod
    {
        CommonMode = 0,
        SafeMode = 1
    }

    class DataHelper
    {
        public static void WriteFile(string location, string fileName, string content, FileOperatingMethod method = FileOperatingMethod.SafeMode)
        {
            string path = Path.Combine(location, fileName);

            Directory.CreateDirectory(location);

            if (method == FileOperatingMethod.SafeMode)
            {
                if (File.Exists(path))
                {
                    File.Move(path, path + ".bak");
                }
            }

            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            streamWriter.Write(content);
            streamWriter.Flush();

            streamWriter.Close();
            fileStream.Close();

            if(method == FileOperatingMethod.SafeMode)
            {
                if (File.Exists(path + ".bak"))
                {
                    File.Delete(path + ".bak");
                }
            }
        }

        public static string ReadFile(string path, FileOperatingMethod method = FileOperatingMethod.SafeMode)
        {
            if (method == FileOperatingMethod.SafeMode)
            {
                if (File.Exists(path + ".bak"))
                {
                    path += ".bak";
                }
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            char[] content = Encoding.UTF8.GetChars(bytes);
            fileStream.Close();

            return new string(content);
        }

        public static string ImgToBase64String(Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap Base64StringToImage(string strbase64)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        public static void TransferFolder(string srcFolder, string destFolder)
        {
            string fileName, destFile;
            if (Directory.Exists(srcFolder))
            {
                if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

                string[] files = Directory.GetFiles(srcFolder);

                foreach (string s in files)
                {
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(destFolder, fileName);
                    File.Copy(s, destFile, true);
                }
            }
            else
            {
                throw new InvalidOperationException("Source folder not found!");
            }
        }

        public static void RenameFile(string srcFile, string destFile)
        {
            FileInfo fi = new FileInfo(srcFile); //xx/xx/aa.rar
            if (File.Exists(destFile))
                File.Copy(destFile, destFile + ".bak");

            if (File.Exists(destFile)) File.Delete(destFile);
            fi.MoveTo(destFile); //xx/xx/xx.rar
            File.Delete(destFile + ".bak");
        }

        public static string MD5Crypto(string plaintext)
        {
            byte[] result = Encoding.Default.GetBytes(plaintext.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);

            return BitConverter.ToString(output).Replace("-", "");
        }
    }
}
