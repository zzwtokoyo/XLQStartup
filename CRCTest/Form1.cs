using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRCTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text.ToString();
            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }
            byte[] val = new byte[str.Length / 2];
            for (int i = 0; i < val.Length; i++)
            {
                string b = str.Substring(i * 2, 2);
                val[i] = Convert.ToByte(b, 16);
            }

            byte[] crcBytes = new byte[val.Length - 2];
            for (int j = 0; j < crcBytes.Length; j++)
            {
                crcBytes[j] = val[j + 2];
            }

            byte crc = crcCulture(crcBytes);

            textBox2.Text = crc.ToString("X2");
        }

        private byte crcCulture(byte[] data)
        {
            int crc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc = (crc >> 1 ^ 0x8c);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return (byte)crc;
        }
    }
}
