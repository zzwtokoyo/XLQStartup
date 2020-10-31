using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XQLHardwareManager.WriteFiles;

namespace XQLHardwareManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //从datagrid中生成加密文件
            if (dataGridView1.Rows.Count <= 1)
            {
                textBox3.Text = ("为添加相关门锁数据");
                return;
            }

            try
            {
                //
                string m_value1 = string.Empty;
                string m_value2 = string.Empty;
                string m_value3 = string.Empty;

                WriteEncrptyFile.GetInstance().DeletFile(WriteEncrptyFile.GetInstance().baseDir + "\\" + WriteEncrptyFile.GetInstance().getFileName());

                foreach (DataGridViewRow dgvr in dataGridView1.Rows)
                {
                    if (dgvr.Cells[0].Value != null && dgvr.Cells[1].Value != null && dgvr.Cells[2].Value != null)
                    {
                        m_value1 = dgvr.Cells[0].Value.ToString();
                        m_value2 = dgvr.Cells[1].Value.ToString();
                        m_value3 = dgvr.Cells[2].Value.ToString();
                        //Key
                        string m_inValue = m_value1 + "," + m_value2 + "," + m_value3;
                        string m_MasterKey = MasterKey.Text.ToString();
                        if (m_MasterKey.Length != 24)
                        {
                            textBox3.Text = ("密钥长度不正确，请输入24位主密钥");
                            return;
                        }
                        var wrIndata = dgvr.Cells[3].Value = Encrypt3Des(m_inValue, m_MasterKey, CipherMode.CBC, "12345678");
                        WriteEncrptyFile.GetInstance().WriteFile(wrIndata.ToString());
                    }
                }
                textBox3.Text = "加密数据完成";
            }
            catch(Exception ex)
            {
                textBox3.Text = "加密数据异常：" + ex.Message;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
            }
            dataGridView1.Refresh();
        }
      
        private void button6_Click(object sender, EventArgs e)
        {
            string file = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,//该值确定是否可以选择多个文件
                Title = "请选择文件夹",
                Filter = "授权文件(*.licd)|*.licd|所有文件(*.*)|*.*"
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
            }
            else
            {
                return;
            }
            List<string> vrList = new List<string>();
            vrList = WriteEncrptyFile.GetInstance().ReadEncrptyFile(file);

            foreach(var indata in vrList)
            {
                if (indata.ToString() != "")
                {
                    int irow = dataGridView1.Rows.Add();
                    dataGridView1.Rows[irow].Cells[3].Value = indata.ToString();
                }
            }
        }

        #region 3des解密
        /// <summary>
        /// des 解密
        /// </summary>
        /// <param name="aStrString">加密的字符串</param>
        /// <param name="aStrKey">密钥</param>
        /// <param name="iv">解密矢量：只有在CBC解密模式下才适用</param>
        /// <param name="mode">运算模式</param>
        /// <returns>解密的字符串</returns>
        public string Decrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB, string iv = "12345678")
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = Encoding.UTF8.GetBytes(aStrKey),
                    Mode = mode,
                    Padding = PaddingMode.PKCS7
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = Encoding.UTF8.GetBytes(iv);
                }
                var desDecrypt = des.CreateDecryptor();
                var result = string.Empty;
                byte[] buffer = Convert.FromBase64String(aStrString);
                result = Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                return result;
            }
            catch (Exception e)
            {
                textBox3.Text = (string.Format("数据解密异常：{0}", e.Message));
                return string.Empty;
            }
        }
        #endregion

        #region 3des加密
        /// <summary>
        /// 3des ecb模式加密
        /// </summary>
        /// <param name="aStrString">待加密的字符串</param>
        /// <param name="aStrKey">密钥</param>
        /// <param name="iv">加密矢量：只有在CBC解密模式下才适用</param>
        /// <param name="mode">运算模式</param>
        /// <returns>加密后的字符串</returns>
        public string Encrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB, string iv = "12345678")
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = Encoding.UTF8.GetBytes(aStrKey),
                    Mode = mode
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = Encoding.UTF8.GetBytes(iv);
                }
                var desEncrypt = des.CreateEncryptor();
                byte[] buffer = Encoding.UTF8.GetBytes(aStrString);
                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                textBox3.Text = (string.Format("数据加密异常：{0}", e.Message));
                return string.Empty;
            }
        }
        #endregion

        private void ClearDgv_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void DECrypt_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow readyData in dataGridView1.Rows)
                {
                    if (readyData.Cells[3].Value != null)
                    {
                        string strDecrpty = string.Empty;
                        string m_MasterKey = MasterKey.Text.ToString();
                        if (m_MasterKey.Length != 24)
                        {
                            textBox3.Text = "密钥长度不正确，请输入24位主密钥";
                            return;
                        }
                        strDecrpty = Decrypt3Des(readyData.Cells[3].Value.ToString(), m_MasterKey, CipherMode.CBC, "12345678");
                        readyData.Cells[0].Value = strDecrpty.Split(',')[0];
                        readyData.Cells[1].Value = strDecrpty.Split(',')[1];
                        readyData.Cells[2].Value = strDecrpty.Split(',')[2];
                    }
                }

                textBox3.Text = "解密完成";
            }
            catch(Exception ex)
            {
                textBox3.Text = "解密异常：" + ex.Message;
            }
        }
    }
}
