using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrSQl
{
    public partial class Identification : Form
    {
        private readonly string connect = "Server=127.0.0.1;User Id=postgres;Port=5432;Password=123;Database=postgres";
        private int _access;
        private readonly object db;
        public NpgsqlConnection Conn { get; private set; }
        
        public Identification()
        {
            InitializeComponent();
            
        }
        
        public string login
        {
            get
            {
                return textBoxlog.Text;
            }
        }
        public string pasw
        {
            get
            {
                return textBoxPas.Text;
            }
        }

        public new void Close()
        {
            //Identification.ActiveForm.Hide();
            this.Visible = false;
        }
        
        private void Button1_Click(object sender, EventArgs e)
        {
            OpenMainForm();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            textBoxlog.Text = "nero";
            textBoxPas.Text = "admin123";
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Button1_Click(this, new EventArgs());
            }
            
        }

        private void OpenMainForm()
        {
            if (login != "" && pasw != "")
            {
                SignIn();
            }
            else
            {
                MessageBox.Show(
                   "Введите логин и пароль!",
                   "Ошибка авторизации",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information,
                   MessageBoxDefaultButton.Button1,
                   MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public void SignIn()
        {
            try
            {
                Conn = new NpgsqlConnection();
                Conn.ConnectionString = connect;
                Conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT post,login,password,active FROM staff", Conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                //var q = from m in db.Users
                //        where m.Use_Name == txtUserName.Text && m.Use_Password == txtPassword.Text
                //        select m;
                if (!CheckSingIn(dr, login, pasw))
                {
                    MessageBox.Show(
                       "Введите заново логин и пароль.",
                       "Ошибка авторизации",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information,
                       MessageBoxDefaultButton.Button1,
                       MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    MainForm f1 = new MainForm(_access, Conn);
                    Close();
                    f1.Show();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Identification id = new Identification();
            }
            finally
            {
                Conn.Close();
            }
        }

        public bool CheckSingIn(NpgsqlDataReader dr, string login, string pasw)
        {
            while (dr.Read())
            {
                
                    if ((login == dr[1].ToString()) && (pasw == dr[2].ToString()))
                    {
                        if ((Boolean)dr[3])
                        {
                            if (dr[0].ToString() == "директор")
                            {
                                _access = 0;
                                return true;
                            }
                            else if (dr[0].ToString() == "учитель")
                            {
                                _access = 1;
                                return true;
                            }
                        }
                    }
                
            }
            return false;
        }

        private void textBoxlog_TextChanged(object sender, EventArgs e)
        {

        }

        private void Identification_Load(object sender, EventArgs e)
        {
           
        }

    }
}
