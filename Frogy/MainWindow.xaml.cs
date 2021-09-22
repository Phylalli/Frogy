using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Pipes;
using System.IO;

using FrogyCore.Models;

namespace Frogy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string serviceFilePath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
            "FrogyCore.exe");

        NamedPipeClientStream pc;
        StreamString ss;

        public MainWindow()
        {
            InitializeComponent();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ss.WriteString("please");

            //Debug.WriteLine(ss.ReadString());

            text1.Text = ss.ReadString();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process core = new Process
            {
                StartInfo =
                {
                    FileName = serviceFilePath,
                    Arguments = "C:\\Users\\yaione\\AppData\\Roaming\\Frogy cdaed248e5e941ef91c7d95a67f982af 60"
                }
            };

            core.Start();


            pc = new NamedPipeClientStream("FrogyCorePipe");
            pc.Connect();
            ss = new StreamString(pc);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ss.WriteString("exit");
            pc.Close();
            
        }
    }

    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            var inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
