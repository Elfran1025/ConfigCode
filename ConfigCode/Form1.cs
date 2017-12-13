using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ConfigCode.DataModel;

namespace ConfigCode
{

    public partial class Form1 : Form
    {
        private const String DEFAULT_TEXT = "公司名称";
        DataModel dm = new DataModel();
        string exportPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Code" + "\\"; //生成CSV路径

        public Form1()
        {
            InitializeComponent();
            SetDefaultText();
            //while (true) {
            //    checkListBox_Enable();
                

            //}
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataModel.dt.Clear();
            dm.voltageCount = (int)numericUpDown1.Value;
            dm.temperatureCount = (int)numericUpDown2.Value;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string selectItem = "";
                if (checkedListBox1.GetItemChecked(i))
                {
                    selectItem = checkedListBox1.GetItemText(checkedListBox1.Items[i]);
                }
                switch (selectItem)
                {
                    case "整车数据":
                        dm.VehicleData();
                        break;
                    case "驱动电机数据":
                        dm.DrivemotorData();
                        break;
                    case "燃料电池数据":
                        dm.FuelcellData();
                        break;
                    case "发动机数据":
                        dm.EngineData();
                        break;
                    case "极值数据":
                        dm.ExtremaData();
                        break;
                    case "报警数据":
                        dm.AlarmData();
                        break;
                    case "单体电压数据":
                        dm.VoltageData();
                        break;
                    case "单体温度数据":
                        dm.TemperatureData();
                        break;
                    default:
                        break;

                }

            }
            //DataTable dt = new DataTable();

            //dt = (dataGridView1.DataSource as DataTable);
            //DataColumn dc = new DataColumn();
            //dt.Columns.Add(dc);

            //DataRow dr = dt.NewRow();
            //dr[0] = "车辆状态";
            //dt.Rows.Add(dr);
            dataGridView1.DataSource = DataModel.dt.DefaultView;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.comboBox1.SelectedIndex = 0;
            // dc = new DataColumn("数据项目名称");
            ////if(dt.Columns.Contains(dc))
            //dt.Columns.Add(dc);
            //dc = new DataColumn("排列格式");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("起始字节");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("起始位");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("位长");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("CANID");
            //dt.Columns.Add(dc);
            //dc = new DataColumn("偏移量");
            //dt.Columns.Add(dc);
            //DataRow dr = dt.NewRow();
            //dr[0] = "车辆状态";
            //dt.Rows.Add(dr);
            this.dataGridView1.DataSource = DataModel.dt.DefaultView;
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            table_Resize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataModel.dt.Clear();
            DataModel.bitlength.Clear();
            DataModel.offset.Clear();
            DataModel.sourceDt_row_num.Clear();
            DataModel.precision.Clear();
            SetDefaultText();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
                //if (checkedListBox1.GetItemChecked(i))
                //{
                //    checkedListBox1.SetItemChecked(i, false);
                //}
                //else
                //{
                //    checkedListBox1.SetItemChecked(i, true);
                //}
            }
            checkListBox_Enable();

            //Form1_Load(sender,e);
        }




        private void button3_Click(object sender, EventArgs e)
        {
            dm.company = textBox2.Text;

            String code = dm.CreateCode();
            string NewFileName = "";


            dm.CreateFile(code, out NewFileName);
            textBox1.Text += "生成成功，文件：" + NewFileName + "\r\n";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(exportPath);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int count = this.dataGridView1.RowCount;
            for (int i = count-2; i >=0; i--) {
                string format = DataModel.dt.Rows[i]["排列格式"].ToString();
                    //Rows[i]["排列格式"].toString();
                //if (count >= 2)
                //{
                    this.dataGridView1.Rows.RemoveAt(i);

                //}
                if (format == "——") {
                    break;
                }
            }


        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == DEFAULT_TEXT)
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text))
                SetDefaultText();
        }

        private void SetDefaultText()
        {
            textBox2.Text = DEFAULT_TEXT;
            textBox2.ForeColor = Color.Gray;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataModel.sourceDt_row_num= new Dictionary<string, int>();
            AsposeTool ast = new AsposeTool();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XLS,XLSX|*.xls;*.xlsx;";
            //ofd.DefaultExt = ".xsl";
            ofd.ShowDialog();
            string path = ofd.FileName;
            String fileName = ofd.SafeFileName;
            DataTable dt = new DataTable();
            if (!path.Equals(null) && !path.Equals(""))
            {
                dt = ast.Excel2DataTable(path);
                //this.dataGridView1.DataSource = dt;
            }
            if (fileName != "" && fileName != null) {

                this.textBox2.Text = fileName;
            }
      
            if (dt.Rows.Count > 0)
            {
                dm.copy2DataTable(dt);
            }

        }
        private void checkListBox_Enable()
        {
            if (checkedListBox1.GetItemChecked(6))
            {
                numericUpDown1.Enabled = true;
                numericUpDown1.Visible = true;
                //return true;
            }
            else
            {
                numericUpDown1.Enabled = false;
                numericUpDown1.Visible = false;
                //return false;
            }
            if (checkedListBox1.GetItemChecked(7))
            {
                numericUpDown2.Enabled = true;
                numericUpDown2.Visible = true;
                //return true;
            }
            else
            {
                numericUpDown2.Enabled = false;
                numericUpDown2.Visible = false;
                //return false;
            }
            
        }

        //private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //    checkListBox_Enable();
        //}

        //private void checkedListBox1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    checkListBox_Enable();
        //}

        private void checkedListBox1_MouseUp(object sender, MouseEventArgs e)
        {
            checkListBox_Enable();
        }

        private void checkedListBox1_KeyUp(object sender, KeyEventArgs e)
        {
            checkListBox_Enable();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            table_Resize();
        }
        public void table_Resize() {

            this.dataGridView1.Columns[0].Width = (int)(this.dataGridView1.Width * 0.20);
            this.dataGridView1.Columns[1].Width = (int)(this.dataGridView1.Width * 0.15);
            this.dataGridView1.Columns[2].Width = (int)(this.dataGridView1.Width * 0.06);
            this.dataGridView1.Columns[3].Width = (int)(this.dataGridView1.Width * 0.06);
            this.dataGridView1.Columns[4].Width = (int)(this.dataGridView1.Width * 0.06);
            this.dataGridView1.Columns[5].Width = (int)(this.dataGridView1.Width * 0.17);
            this.dataGridView1.Columns[6].Width = (int)(this.dataGridView1.Width * 0.10);
            this.dataGridView1.Columns[7].Width = (int)(this.dataGridView1.Width * 0.10);
            this.dataGridView1.Columns[8].Width = (int)(this.dataGridView1.Width * 0.15);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != DEFAULT_TEXT)
            {
                //textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }
    }
}
