using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace PrSQl
{

    public partial class MainForm : Form
    {
        public static MainForm Instance;
        private readonly DBWorker db;
        public string login, pasw;
        public int access;
        private CheckBox chkPasteToSelectedCells;
        private ContextMenuStrip contextMenuStrip1;
        static int i;

        public MainForm(int access, NpgsqlConnection Conn)
        {
            InitializeComponent();
            this.access = access;
            db = new DBWorker(access, Conn);
            Instance = this;

        }

        private readonly PrintDocument printDocument1 = new PrintDocument();

        private class DBWorker  //
        {
            private readonly NpgsqlConnection Conn;
            private readonly int access;
            private readonly string[] num_col = new string[15];

            public object JsonConvert { get; private set; }

            public DBWorker(int _access, NpgsqlConnection _Conn)
            {
                access = _access;
                Conn = _Conn;
            }

            public void list_tab(DataGridView dataGridView1)
            {
                DataGridViewColumn column1 = new DataGridViewColumn
                {
                    HeaderText = "Список таблиц",
                    ReadOnly = true,
                    Name = "list_tab",
                    CellTemplate = new DataGridViewTextBoxCell()
                };
                dataGridView1.Columns.Add(column1);
                dataGridView1.AllowUserToAddRows = false;
                if (access == 0)
                {
                    dataGridView1.Rows.Add("classroom");
                    dataGridView1.Rows.Add("contract");
                    dataGridView1.Rows.Add("curriculum");
                    dataGridView1.Rows.Add("equipement");
                    dataGridView1.Rows.Add("groups");
                    dataGridView1.Rows.Add("parent");
                    dataGridView1.Rows.Add("school");
                    dataGridView1.Rows.Add("staff");
                    dataGridView1.Rows.Add("student");
                    dataGridView1.Rows.Add("teacher");
                }
                else if (access == 1)
                {
                    dataGridView1.Rows.Add("classroom");
                    dataGridView1.Rows.Add("curriculum");
                    dataGridView1.Rows.Add("groups");
                    dataGridView1.Rows.Add("parent");
                    dataGridView1.Rows.Add("student");
                    dataGridView1.Rows.Add("teacher");

                }
                Conn.Close();
            }

            public void HistoryList(int choice)
            {
                Instance.comboBox2.Items.Clear();
                Instance.comboBox2.ResetText();
                string iq = "SELECT id,tstamp FROM t_history";
                NpgsqlCommand command = new NpgsqlCommand(iq, Conn);
                NpgsqlDataReader reader;
                command.Connection.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                i = 1;
                if (choice == 0)
                {
                    DateTime dateTime;
                    while (reader.Read())
                    {
                        dateTime = (DateTime)reader[1];
                        Instance.comboBox2.Items.Add(dateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff"));

                    }
                } else if (choice == 1)
                {
                    while (reader.Read())
                    {
                        Instance.comboBox2.Items.Add(i++);
                    }
                }

                command.Connection.Close();
            }

            public class MyDetail
            {
                [DataMember]
                public string FirstName { get; set; }
                [DataMember]
                public string LastName { get; set; }
            }

            public void undo()
            {
                string tab = MainForm.Instance.label1.Text;
                if (tab == "")
                {
                    MessageBox.Show("Выберите таблицу для изменения!");
                }
                else
                {
                    Conn.Open();
                    try
                    {
                        string n = "(";
                        for (int i = 0; i < Instance.dataGridView2.Columns.Count; i++)
                        {
                            n += Instance.dataGridView2.Columns[i].Name;
                            if (i != Instance.dataGridView2.Columns.Count - 1) n += ",";
                        }
                        n += ")";
                        NpgsqlCommand Command1 = new NpgsqlCommand();
                        Command1 = new NpgsqlCommand(
                           "select operation from t_history where id in (select max(id) " +
                                "from t_history where tabname = '" + tab + "')", Conn);
                        string operation = (string)Command1.ExecuteScalar();
                        if (operation == "UPDATE")
                        {
                            NpgsqlCommand Command = new NpgsqlCommand();
                            Command = new NpgsqlCommand(
                               "select old_val from t_history where id = (select max(id) " +
                                "from t_history where tabname = '" + tab + "') and tabname = '" + tab + "';", Conn);
                            string result = (string)Command.ExecuteScalar();
                            string[] pid = result.Split(',', '(', ')');
                            for (int i = 2; i < pid.Length - 1; i++)
                            {
                                pid[i] = "'" + pid[i] + "'";
                            }
                            result = "(";
                            for (int i = 1; i < pid.Length - 1; i++)
                            {
                                result += pid[i];
                                if (i != pid.Length - 2) result += ",";
                            }
                            result += ")";
                            NpgsqlCommand Cmd = new NpgsqlCommand();
                            Cmd = new NpgsqlCommand(
                                "update " + tab + " set " + n + " = " + result + " where " + Instance.dataGridView2.Columns[0].Name + " = " + pid[1] + ";", Conn
                                );
                            Cmd.ExecuteNonQuery();
                            NpgsqlCommand com = new NpgsqlCommand();
                            com = new NpgsqlCommand("DELETE FROM t_history WHERE id = (select max(id) " +
                                "from t_history where tabname = '" + tab + "') and tabname = '" + tab + "'; " +
                                 "DELETE FROM t_history WHERE tstamp in (select max(tstamp) from " +
                                "t_history where tabname = '" + tab + "');", Conn);
                            com.ExecuteNonQuery();
                            //HistoryList(0);
                        }
                        else if (operation == "DELETE")
                        {
                            NpgsqlCommand Command = new NpgsqlCommand();
                            Command = new NpgsqlCommand(
                               "select old_val from t_history where id = (select max(id) " +
                                "from t_history where tabname = '" + tab + "') and tabname = '" + tab + "';", Conn);
                            string result = (string)Command.ExecuteScalar();
                            string[] pid = result.Split(',', '(', ')');
                            for (int i = 2; i < pid.Length - 1; i++)
                            {
                                pid[i] = "'" + pid[i] + "'";
                            }
                            result = "(";
                            for (int i = 1; i < pid.Length - 1; i++)
                            {
                                result += pid[i];
                                if (i != pid.Length - 2) result += ",";
                            }
                            result += ")";
                            NpgsqlCommand Cmd = new NpgsqlCommand();
                            Cmd = new NpgsqlCommand(
                                "insert into " + tab + "  " + n + " values " + result + ";", Conn
                                );
                            Cmd.ExecuteNonQuery();
                            NpgsqlCommand com = new NpgsqlCommand();
                            com = new NpgsqlCommand("DELETE FROM t_history WHERE id = (select max(id) " +
                                "from t_history where tabname = '" + tab + "') and tabname = '" + tab + "'; " +
                                "DELETE FROM t_history WHERE tstamp in (select max(tstamp) " +
                                "from t_history where tabname = '" + tab + "');", Conn);
                            com.ExecuteNonQuery();
                            //HistoryList(0);

                        }
                        else if (operation == "INSERT")
                        {
                            NpgsqlCommand Command = new NpgsqlCommand();
                            Command = new NpgsqlCommand(
                               "select new_val from t_history where id = (select max(id) " +
                                "from t_history where tabname = '" + tab + "') and tabname = '" + tab + "';", Conn);
                            string result = (string)Command.ExecuteScalar();
                            string[] pid = result.Split(',', '(', ')');
                            for (int i = 2; i < pid.Length - 1; i++)
                            {
                                pid[i] = "'" + pid[i] + "'";
                            }
                            result = "(";
                            for (int i = 1; i < pid.Length - 1; i++)
                            {
                                result += pid[i];
                                if (i != pid.Length - 2) result += ",";
                            }
                            result += ")";
                            NpgsqlCommand Cmd = new NpgsqlCommand();
                            Cmd = new NpgsqlCommand(
                                "delete from " + tab + " where " + Instance.dataGridView2.Columns[0].Name + " = " + pid[1] + ";", Conn
                                );
                            Cmd.ExecuteNonQuery();
                            NpgsqlCommand com = new NpgsqlCommand();
                            com = new NpgsqlCommand("DELETE FROM t_history WHERE id = (select max(id) " +
                                "from t_history where tabname = '" + tab + "') and tabname = '" + tab + "'; " +
                                "DELETE FROM t_history WHERE tstamp in (select max(tstamp) " +
                                "from t_history where tabname = '" + tab + "'); ", Conn);
                            com.ExecuteNonQuery();
                            //HistoryList(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось внести изменения.");
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        Conn.Close();
                    }
                }


            }

            public void RestoreByDate()
            {
                string tab = MainForm.Instance.label1.Text;
                if (tab == "")
                {
                    MessageBox.Show("Выберите таблицу для изменения!");
                }
                else
                {
                    Conn.Open();
                    try
                    {
                        string n = "(";
                        for (int i = 0; i < Instance.dataGridView2.Columns.Count; i++)
                        {
                            n += Instance.dataGridView2.Columns[i].Name;
                            if (i != Instance.dataGridView2.Columns.Count - 1) n += ",";
                        }
                        n += ")";
                        NpgsqlCommand Command1 = new NpgsqlCommand();
                        Command1 = new NpgsqlCommand(
                           "select operation from t_history where tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "'", Conn);
                        string operation = (string)Command1.ExecuteScalar();
                        if (operation == "UPDATE")
                        {
                            NpgsqlCommand Command = new NpgsqlCommand();
                            Command = new NpgsqlCommand(
                               "select old_val from t_history where tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "';", Conn);
                            string result = (string)Command.ExecuteScalar();
                            string[] pid = result.Split(',', '(', ')');
                            for (int i = 2; i < pid.Length - 1; i++)
                            {
                                pid[i] = "'" + pid[i] + "'";
                            }
                            result = "(";
                            for (int i = 1; i < pid.Length - 1; i++)
                            {
                                result += pid[i];
                                if (i != pid.Length - 2) result += ",";
                            }
                            result += ")";
                            NpgsqlCommand Cmd = new NpgsqlCommand();
                            Cmd = new NpgsqlCommand(
                                "update " + tab + " set " + n + " = " + result + " where " + Instance.dataGridView2.Columns[0].Name + " = " + pid[1] + ";", Conn
                                );
                            Cmd.ExecuteNonQuery();
                            NpgsqlCommand com = new NpgsqlCommand();
                            com = new NpgsqlCommand("DELETE FROM t_history WHERE tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "'; " +
                                "DELETE FROM t_history WHERE tstamp in (select max(tstamp) from " +
                                "t_history where tabname = '" + tab + "');", Conn);
                            com.ExecuteNonQuery();
                            //HistoryList(0);
                        }
                        else if (operation == "DELETE")
                        {
                            NpgsqlCommand Command = new NpgsqlCommand();
                            Command = new NpgsqlCommand(
                               "select old_val from t_history where tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "';", Conn);
                            string result = (string)Command.ExecuteScalar();
                            string[] pid = result.Split(',', '(', ')');
                            for (int i = 2; i < pid.Length - 1; i++)
                            {
                                pid[i] = "'" + pid[i] + "'";
                            }
                            result = "(";
                            for (int i = 1; i < pid.Length - 1; i++)
                            {
                                result += pid[i];
                                if (i != pid.Length - 2) result += ",";
                            }
                            result += ")";
                            NpgsqlCommand Cmd = new NpgsqlCommand();
                            Cmd = new NpgsqlCommand(
                                "insert into " + tab + "  " + n + " values " + result + ";", Conn
                                );
                            Cmd.ExecuteNonQuery();
                            NpgsqlCommand com = new NpgsqlCommand();
                            com = new NpgsqlCommand("DELETE FROM t_history WHERE tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "'; " +
                                "DELETE FROM t_history WHERE tstamp in (select max(tstamp) " +
                                "from t_history where tabname = '" + tab + "');", Conn);
                            com.ExecuteNonQuery();
                            //HistoryList(0);

                        }
                        else if (operation == "INSERT")
                        {
                            NpgsqlCommand Command = new NpgsqlCommand();
                            Command = new NpgsqlCommand(
                               "select new_val from t_history where tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "';", Conn);
                            string result = (string)Command.ExecuteScalar();
                            string[] pid = result.Split(',', '(', ')');
                            for (int i = 2; i < pid.Length - 1; i++)
                            {
                                pid[i] = "'" + pid[i] + "'";
                            }
                            result = "(";
                            for (int i = 1; i < pid.Length - 1; i++)
                            {
                                result += pid[i];
                                if (i != pid.Length - 2) result += ",";
                            }
                            result += ")";
                            NpgsqlCommand Cmd = new NpgsqlCommand();
                            Cmd = new NpgsqlCommand(
                                "delete from " + tab + " where " + Instance.dataGridView2.Columns[0].Name + " = " + pid[1] + ";", Conn
                                );
                            Cmd.ExecuteNonQuery();
                            NpgsqlCommand com = new NpgsqlCommand();
                            com = new NpgsqlCommand("DELETE FROM t_history WHERE tstamp = '" + Instance.comboBox2.Text +
                           "' and tabname = '" + tab + "'; " +
                                "DELETE FROM t_history WHERE tstamp in (select max(tstamp) " +
                                "from t_history where tabname = '" + tab + "'); ", Conn);
                            com.ExecuteNonQuery();
                            //HistoryList(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось внести изменения.");
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        Conn.Close();
                    }
                }
            }

            public void RestoreByCount()
            {
                int x = Convert.ToInt32(Instance.comboBox2.Text);
                int i = Instance.comboBox2.Items.Count;
                while (i >= x)
                {
                    i--;
                    undo();
                }
            }

            public void select()
            {
                string t = MainForm.Instance.dataGridView1.CurrentCell.Value.ToString();
                MainForm.Instance.label1.Text = t;
                string iq = "SELECT * FROM " + t;
                DataSet dataset = new DataSet();
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter
                {
                    SelectCommand = new NpgsqlCommand(iq, Conn)
                };
                adapter.Fill(dataset, t);

                MainForm.Instance.dataGridView2.DataSource = dataset.Tables[t];
                Conn.Close();
                Instance.comboBox1.Items.Clear();
                for (int i = 0; i < Instance.dataGridView2.Columns.Count; i++)
                {
                    if (i == Instance.dataGridView2.Columns.Count - 1)
                    {
                        Instance.comboBox1.Items.Add(Instance.dataGridView2.Columns[i - 1].Name);
                        break;
                    }
                    num_col[i] = Instance.dataGridView2.Columns[i].Name;
                    Instance.comboBox1.Items.Add(Instance.dataGridView2.Columns[i].Name);
                }
                Instance.dataGridView2.Columns[0].ReadOnly = true;
            }

            public void add_str()
            {
                string tab = MainForm.Instance.label1.Text;
                if (tab == "")
                {
                    MessageBox.Show("Выберите таблицу для изменения!");
                }
                else
                {
                    Conn.Open();
                    try
                    {
                        int max = int.Parse(Instance.dataGridView2[0, 0].Value.ToString());
                        int k = max;
                        for (int i = 0; i < Instance.dataGridView2.RowCount - 2; i++)
                        {
                            if (int.Parse(Instance.dataGridView2[0, i].Value.ToString()) >= max)
                            {
                                max = int.Parse(Instance.dataGridView2[0, i].Value.ToString());
                                k = max;
                            }
                        }
                        k++;
                        string str = "";
                        str += k.ToString();
                        for (int i = 1; i < Instance.dataGridView2.ColumnCount; i++)
                        {
                            str += ",'" + Instance.dataGridView2.CurrentRow.Cells[i].Value.ToString() + "'";
                        }
                        NpgsqlCommand Command = new NpgsqlCommand();
                        Command = new NpgsqlCommand(
                            "INSERT INTO " + Instance.label1.Text + " VALUES (" + str + ")", Conn);
                        Command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось внести изменения.");
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        Conn.Close();
                    }
                }
            }

            public void delete_str()
            {
                string tab = MainForm.Instance.label1.Text;
                if (tab == "")
                {
                    MessageBox.Show("Выберите таблицу для изменения!");
                }
                else
                {
                    try
                    {
                        string cel = MainForm.Instance.dataGridView2.CurrentCell.Value.ToString();
                        int num = MainForm.Instance.dataGridView2.CurrentCell.ColumnIndex;
                        string name_r = MainForm.Instance.dataGridView2.Columns[num].HeaderCell.Value.ToString();

                        NpgsqlCommandBuilder com = new NpgsqlCommandBuilder();
                        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                        Conn.Open();
                        adapter.UpdateCommand = new NpgsqlCommand("DELETE FROM " + tab + " WHERE " + name_r + " = '" + cel + "';", Conn);
                        adapter.UpdateCommand.ExecuteNonQuery();

                        string iq = "SELECT * FROM " + tab;
                        DataSet dataset = new DataSet();
                        adapter.SelectCommand = new NpgsqlCommand(iq, Conn);
                        adapter.Fill(dataset, tab);
                        MainForm.Instance.dataGridView2.DataSource = dataset.Tables[tab];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    Conn.Close();
                }
            }

            public void change()
            {
                string tab = MainForm.Instance.label1.Text;
                if (tab == "")
                {
                    MessageBox.Show("Выберите таблицу для изменения!");
                }
                else
                {
                    Conn.Open();
                    try
                    {
                        int index = Instance.dataGridView2.CurrentCell.ColumnIndex;
                        string n = Instance.dataGridView2.Columns[index].Name;
                        string str;
                        if (Instance.dataGridView2.CurrentCell.Value == DBNull.Value)
                        {
                            str = "null";
                        }
                        //else if (num_col.Contains(n))
                        //    str = Instance.dataGridView2.CurrentCell.Value.ToString();
                        else
                        {
                            str = "'" + Instance.dataGridView2.CurrentCell.Value.ToString() + "'";
                        }
                        String quiery = "UPDATE " + Instance.dataGridView1.CurrentCell.Value.ToString() +
                            " SET " + n + " = " + str + " WHERE " + Instance.dataGridView2.Columns[0].Name + " = " + Instance.dataGridView2.CurrentRow.Cells[0].Value;
                        NpgsqlCommand Command = new NpgsqlCommand();
                        Command = new NpgsqlCommand(
                            quiery, Conn);
                        Command.ExecuteNonQuery();
                        /*for(int i =0;i<Instance.dataGridView2.Columns.Count; i++)
                        {
                            if (Instance.dataGridView2.CurrentRow.Cells[i] == Instance.dataGridView2.CurrentCell) ;
                              //data+= CheckBox()
                        }*/
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось внести изменения.");
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        Conn.Close();
                    }
                }
            }

            public void Table_logs(string login, string tab, string type_ch)
            {
                string date_ch = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string zap = "INSERT INTO tab_log VALUES ('" + login + "','" + tab + "','" + type_ch + "','" + date_ch + "');";
                NpgsqlCommandBuilder com = new NpgsqlCommandBuilder();
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                Conn.Open();
                adapter.UpdateCommand = new NpgsqlCommand(zap, Conn);
                //adapter.UpdateCommand.ExecuteNonQuery();
                Conn.Close();
            }

            internal AutoCompleteStringCollection AutoComplete()
            {
                AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                Conn.Open();
                //var arr = Instance.dataGridView2.Items.Cast<Object>().Select(item => item.ToString()).ToArray();
                string table = MainForm.Instance.dataGridView1.CurrentCell.Value.ToString();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM " + table, Conn);
                //Collection.Add(cmd.ExecuteScalar().ToString());
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    for (int i = 0; i < dr.VisibleFieldCount; i++)
                    {
                        Collection.Add(dr[i].ToString());
                    }
                }
                dr.Close();
                Conn.Close();
                return Collection;

            }
        }
        ////
        //// Form behaviuour
        ////
        //
        // when a table in DataGrid_1 is selected
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            db.select();
            if (access == 0)
            {
                MainForm.Instance.button1.Show();
                MainForm.Instance.insert_button.Show();
                MainForm.Instance.add_button.Show();
                MainForm.Instance.button6.Show();
                MainForm.Instance.search_button.Show();
                MainForm.Instance.update_button.Show();
            }
            else if (access == 1)
            {
                MainForm.Instance.button1.Show();
                MainForm.Instance.insert_button.Hide();
                MainForm.Instance.add_button.Hide();
                MainForm.Instance.button6.Hide();
                MainForm.Instance.search_button.Show();
                MainForm.Instance.update_button.Show();
            }
            //db.table_logs(login, tab, "чтение");
        }
        //
        // Ctrl + z etc. 
        //
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {

                MessageBox.Show("You pressed ctrl + c");
                Console.WriteLine("You pressed ctrl + c");
            }
            if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                db.undo();
                Console.WriteLine("You pressed ctrl + z");
            }
        }
        //
        // Copy, Paste context menu
        //
        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        private void PasteClipboardValue()
        {
            //Show Error if no cell is selected
            if (dataGridView2.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the starting Cell
            DataGridViewCell startCell = GetStartCell(dataGridView1);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue =
                    ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is within the limit
                    if (iColIndex <= dataGridView2.Columns.Count - 1
                    && iRowIndex <= dataGridView2.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridView2[iColIndex, iRowIndex];

                        //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                        if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                            (!chkPasteToSelectedCells.Checked))
                            cell.Value = cbValue[rowKey][cellKey];
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }
        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionary with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Copy to clipboard
            CopyToClipboard();

            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                dgvCell.Value = string.Empty;
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Perform paste Operation
            PasteClipboardValue();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.SelectedCells.Count > 0)
                dataGridView2.ContextMenuStrip = contextMenuStrip1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.AutoCompleteCustomSource = db.AutoComplete();
        }


        
        private void MainForm_Load(object sender, EventArgs e)
        {
            MainForm.Instance.dataGridView1.Show();
            MainForm.Instance.dataGridView2.Show();
            Instance.textBox1.Show();
            Instance.comboBox1.Show();
            MainForm.Instance.label1.Show();
            MainForm.Instance.label1.Show();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            db.list_tab(dataGridView1);
        }
        //
        // print page
        //
        public void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            //Bitmap bm = new Bitmap(this.dataGridView2.Width, this.dataGridView2.Height);
            //dataGridView2.DrawToBitmap(bm, new Rectangle(0, 0, this.dataGridView2.Width, this.dataGridView2.Height));
            //e.Graphics.DrawImage(bm, 0, 0);
            //Graphics g = e.Graphics;
            int x = 0;
            int y = 20;
            int cell_height = 0;

            int colCount = dataGridView2.ColumnCount;
            int rowCount = dataGridView2.RowCount - 1;
            //Graphics g = Graphics;
            Font font = new Font("Tahoma", 9, FontStyle.Bold, GraphicsUnit.Point);

            int[] widthC = new int[colCount];

            int current_col = 0;
            int current_row = 0;

            while (current_col < colCount)
            {
                if (e.Graphics.MeasureString(dataGridView2.Columns[current_col].HeaderText.ToString(), font).Width > widthC[current_col])
                {
                    widthC[current_col] = (int)e.Graphics.MeasureString(dataGridView2.Columns[current_col].HeaderText.ToString(), font).Width;
                }
                current_col++;
            }

            while (current_row < rowCount)
            {
                while (current_col < colCount)
                {
                    if (e.Graphics.MeasureString(dataGridView2[current_col, current_row].Value.ToString(), font).Width > widthC[current_col])
                    {
                        widthC[current_col] = (int)e.Graphics.MeasureString(dataGridView2[current_col, current_row].Value.ToString(), font).Width;
                    }
                    current_col++;
                }
                current_col = 0;
                current_row++;
            }

            current_col = 0;
            current_row = 0;

            string value = "";

            int width = widthC[current_col];
            int height = dataGridView2[current_col, current_row].Size.Height;

            Rectangle cell_border;
            SolidBrush brush = new SolidBrush(Color.Black);


            while (current_col < colCount)
            {
                width = widthC[current_col];
                cell_height = dataGridView2[current_col, current_row].Size.Height;
                cell_border = new Rectangle(x, y, width, height);
                value = dataGridView2.Columns[current_col].HeaderText.ToString();
                e.Graphics.DrawRectangle(new Pen(Color.Black), cell_border);
                e.Graphics.DrawString(value, font, brush, x, y);
                x += widthC[current_col];
                current_col++;
            }
            while (current_row < rowCount)
            {
                while (current_col < colCount)
                {
                    width = widthC[current_col];
                    cell_height = dataGridView2[current_col, current_row].Size.Height;
                    cell_border = new Rectangle(x, y, width, height);
                    value = dataGridView2[current_col, current_row].Value.ToString();
                    e.Graphics.DrawRectangle(new Pen(Color.Black), cell_border);
                    e.Graphics.DrawString(value, font, brush, x, y);
                    x += widthC[current_col];
                    current_col++;
                }
                current_col = 0;
                current_row++;
                x = 0;
                y += cell_height;
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void insert_button_Click(object sender, EventArgs e)
        {
            db.change();
            string tab = MainForm.Instance.label1.Text;
            db.Table_logs(login, tab, "изменение");
        }
        
        private void update_button_Click(object sender, EventArgs e)
        {
            db.select();
            if (access == 0)
            {
                MainForm.Instance.insert_button.Show();
                MainForm.Instance.add_button.Show();
                MainForm.Instance.button6.Show();
                MainForm.Instance.search_button.Show();
                MainForm.Instance.update_button.Show();
            }
            else
            {
                MainForm.Instance.search_button.Show();
                MainForm.Instance.update_button.Show();
            }
        }

        private void Remove_button_Click(object sender, EventArgs e)
        {
            db.delete_str();
            string tab = MainForm.Instance.label1.Text;
            db.Table_logs(login, tab, "удаление");
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            db.add_str();
            string tab = MainForm.Instance.label1.Text;
            db.Table_logs(login, tab, "добавление");
        }
        
        private void Search_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(Instance.textBox1.Text, out int userVal))
                {
                    (Instance.dataGridView2.DataSource as DataTable).DefaultView.RowFilter =
                                         string.Format("[{1}] = {0}", userVal, Instance.comboBox1.Text);
                }
                else
                {
                    (Instance.dataGridView2.DataSource as DataTable).DefaultView.RowFilter =
                    string.Format("[{1}] like '{0}%'", Instance.textBox1.Text, Instance.comboBox1.Text);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Не найдено");
                // MessageBox.Show(ex.ToString());
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument Document = new PrintDocument();
                Document.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                PrintPreviewDialog dlg = new PrintPreviewDialog
                {
                    Document = Document
                };
                dlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while printing", ex.ToString());
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Instance.radioButton1.Checked)
            {
                db.RestoreByDate();
                db.HistoryList(0);
            }
            if (Instance.radioButton2.Checked)
            {
                db.RestoreByCount();
                db.HistoryList(1);
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.radioButton1.Checked)
            {
                db.HistoryList(0);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance.radioButton2.Checked)
            {
                db.HistoryList(1);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


    }


}
