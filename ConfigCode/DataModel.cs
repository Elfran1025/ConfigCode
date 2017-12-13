using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfigCode
{
    public class DataModel
    {
        public static DataTable dt = new DataTable();
        //public Dictionary<string, int> dt_row_num = new Dictionary<string, int>();
        public static Dictionary<string, int> sourceDt_row_num = new Dictionary<string, int>();
        public static Dictionary<string, int> offset = new Dictionary<string, int>();
        public static Dictionary<string, int> bitlength = new Dictionary<string, int>();
        public static Dictionary<string, double> precision = new Dictionary<string, double>();
        public int[] bitlengths;
        public double[] precisions;
        public string company = "";
        public int voltageCount = 0;
        public int temperatureCount = 0;

        #region 枚举


        public enum vehicle
        {
            整车数据,
            车辆状态,
            充电状态,
            运行模式,
            车速,
            累计里程,
            总电压,
            总电流,
            SOC,
            DCDC状态,
            档位信息,
            制动力状态,
            驱动力状态,
            绝缘电阻,
            加速踏板行程值,
            制动踏板状态

        }
        public enum engine
        {
            发动机数据,
            发动机状态,
            曲轴转速,
            发动机燃料消耗率

        }

        public enum drivemotor
        {
            驱动电机数据,
            驱动电机个数,
            驱动电机序号,
            驱动电机状态,
            驱动电机控制器温度,
            驱动电机转速,
            驱动电机转矩,
            驱动电机温度,
            电机控制器输入电压,
            电机控制器直流母线电流
        }

        public enum extrema
        {
            极值数据,
            最高电压电池子系统号,
            最高电压电池单体代号,
            电池单体电压最高值,
            最低电压电池子系统号,
            最低电压电池单体代号,
            电池单体电压最低值,
            最高温度子系统号,
            最高温度探针单体代号,
            最高温度值,
            最低温度子系统号,
            最低温度探针子系统代号,
            最低温度值
        }

        public enum fuelcell
        {
            燃料电池数据,
            燃料电池电压,
            燃料电池电流,
            燃料消耗率,
            燃料电池温度探针总数,
            探针温度值,
            氢系统中最高温度,
            氢系统中最高温度探针代号,
            氢气最高浓度,
            氢气最高浓度传感器代号,
            氢气最高压力,
            氢气最高压力传感器代号,
            高压DCDC状态

        }
        public enum alarm
        {
            报警数据,
            最高报警等级,
            温度差异报警,
            电池高温报警,
            车载储能装置类型过压报警,
            车载储能装置类型欠压报警,
            SOC低报警,
            单体电池过压报警,
            单体电池欠压报警,
            SOC过高报警,
            SOC跳变报警,
            可充电储能系统不匹配报警,
            电池单体一致性差报警,
            绝缘报警,
            DCDC温度报警,
            制动系统报警,
            DCDC状态报警,
            驱动电机控制器温度报警,
            高压互锁状态报警,
            驱动电机温度报警,
            车载储能装置类型过充



        }
        public enum voltage
        {
            单体电压数据,
            可充电储能装置电压,
            可充电储能装置电流,
            单体电池总数,
            单体电压


        }
        public enum temperature
        {
            单体温度数据,
            可充电储能温度探针个数,
            温度

        }
        public enum header
        {
            数据项目名称,
            排列格式,
            起始字节,
            起始位,
            位长度,
            CANID,
            精度,
            偏移量,
            备注

        }

        #endregion
        public DataModel()
        {
            DataColumn dc;
            foreach (int item in Enum.GetValues(typeof(header)))
            {

                String name = Enum.GetName(typeof(header), item);
                dc = new DataColumn(name);
                //if(dt.Columns.Contains(dc))
                dt.Columns.Add(dc);
            }
            
            offset.Add("总电流", -1000);
            offset.Add("驱动电机控制器温度", -40);
            offset.Add("驱动电机控制器转速", -20000);
            offset.Add("驱动电机控制器转矩", -20000);
            offset.Add("驱动电机温度", -40);
            offset.Add("电机控制器直流母线电流", -1000);
            offset.Add("最高温度值", -40);
            offset.Add("最低温度值", -40);
            offset.Add("探针温度值", -40);
            offset.Add("氢系统中最高温度", -40);
            offset.Add("可充电储能装置电流", -1000);
            //offset.Add("温度", -40);
        }
        #region 添加方法
        /// <summary>
        /// 整车数据
        /// </summary>
        public void VehicleData()
        {
            bitlengths =new int[] { 0,8,8,8,16,32,16,16,8,8,8,1,1,16,8,8};
            precisions = new double[] { 0, 1, 1, 1, 0.1, 0.1, 0.1, 0.1, 1, 1, 1, 1, 1, 1, 1, 1 };
            int i = 0;
            String name;
            DataRow dr;
            
            foreach (int item in Enum.GetValues(typeof(vehicle)))
            {
                
                name = Enum.GetName(typeof(vehicle), item);
             
                if (name.Equals("DCDC状态"))
                {

                    name = "DC-DC状态";
                }
                if (!bitlength.ContainsKey(name))
                {
                    bitlength.Add(name, bitlengths[i]);
                    precision.Add(name, precisions[i]);
                }
                i++;
                dr = dt.NewRow();
                dr["数据项目名称"] = name;

                if (name.Equals(Enum.GetName(typeof(vehicle), 0)))
                {
                    foreach (int hitem in Enum.GetValues(typeof(header)))
                    {

                        String hname = Enum.GetName(typeof(header), hitem);
                        if (hname.Equals("数据项目名称"))
                        {
                            dr[hname] = "——" + name + "——";

                        }
                        else
                        {
                            dr[hname] = "——";
                        }

                        //dc = new DataColumn(name);
                        ////if(dt.Columns.Contains(dc))
                        //dt.Columns.Add(dc);
                    }
                    //dr["数据项目名称"] = "——"+name+"——";
                    //dr["排列格式"] = "——";
                    //dr["起始字节"] = "——";
                    //dr["起始位"] = "——";
                    //dr["位长度"] = "——";
                    //dr["CANID"] = "——";
                    //dr["偏移量"] = "——";
                    //dr["备注"] = "——";

                }

                dt.Rows.Add(dr);
            }
        }
        /// <summary>
        /// 发动机数据
        /// </summary>
        public void EngineData()
        {
            bitlengths = new int[] { 0, 8, 16, 16 };
            precisions = new double[] { 0, 1, 1, 0.01 };
            int i = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(engine)))
            {
                name = Enum.GetName(typeof(engine), item);
                if (!bitlength.ContainsKey(name))
                {
                    bitlength.Add(name, bitlengths[i]);
                    precision.Add(name, precisions[i]);
                }
                i++;
                dr = dt.NewRow();
                dr["数据项目名称"] = name;
                if (name.Equals(Enum.GetName(typeof(engine), 0)))
                {
                    foreach (int hitem in Enum.GetValues(typeof(header)))
                    {

                        String hname = Enum.GetName(typeof(header), hitem);
                        if (hname.Equals("数据项目名称"))
                        {
                            dr[hname] = "——" + name + "——";

                        }
                        else
                        {
                            dr[hname] = "——";
                        }

                        //dc = new DataColumn(name);
                        ////if(dt.Columns.Contains(dc))
                        //dt.Columns.Add(dc);
                    }


                }

                dt.Rows.Add(dr);
            }
        }

        /// <summary>
        /// 极值数据
        /// </summary>
        public void ExtremaData()
        {
            bitlengths = new int[] { 0, 8, 8, 16,8,8,16,8,8,8,8,8,8 };
            precisions = new double[] { 0, 1, 1,0.001,1, 1, 0.001,1, 1, 1, 1, 1, 1 };
            int i = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(extrema)))
            {
                name = Enum.GetName(typeof(extrema), item);
                if (!bitlength.ContainsKey(name))
                {
                    bitlength.Add(name, bitlengths[i]);
                    precision.Add(name, precisions[i]);
                }
                i++;
                dr = dt.NewRow();
                dr["数据项目名称"] = name;
                if (name.Equals(Enum.GetName(typeof(extrema), 0)))
                {
                    foreach (int hitem in Enum.GetValues(typeof(header)))
                    {

                        String hname = Enum.GetName(typeof(header), hitem);
                        if (hname.Equals("数据项目名称"))
                        {
                            dr[hname] = "——" + name + "——";

                        }
                        else
                        {
                            dr[hname] = "——";
                        }

                        //dc = new DataColumn(name);
                        ////if(dt.Columns.Contains(dc))
                        //dt.Columns.Add(dc);
                    }


                }

                dt.Rows.Add(dr);
            }
        }
        /// <summary>
        /// 驱动电机数据
        /// </summary>
        public void DrivemotorData()
        {
            bitlengths = new int[] { 0, 8,8, 8, 8,16,16,8,16,16 };
            precisions = new double[] { 0, 1, 1, 1, 1, 1,1,1, 0.1, 0.1};
            int i = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(drivemotor)))
            {
                name = Enum.GetName(typeof(drivemotor), item);
                if (!bitlength.ContainsKey(name))
                {
                    bitlength.Add(name, bitlengths[i]);
                    precision.Add(name, precisions[i]);
                }
                i++;
                dr = dt.NewRow();
                dr["数据项目名称"] = name;
                if (name.Equals(Enum.GetName(typeof(drivemotor), 0)))
                {
                    foreach (int hitem in Enum.GetValues(typeof(header)))
                    {

                        String hname = Enum.GetName(typeof(header), hitem);
                        if (hname.Equals("数据项目名称"))
                        {
                            dr[hname] = "——" + name + "——";

                        }
                        else
                        {
                            dr[hname] = "——";
                        }

                        //dc = new DataColumn(name);
                        ////if(dt.Columns.Contains(dc))
                        //dt.Columns.Add(dc);
                    }

                }

                dt.Rows.Add(dr);
            }
        }
        /// <summary>
        /// 燃料电池数据
        /// </summary>
        public void FuelcellData()
        {
            bitlengths = new int[] { 0, 16, 16, 16, 16, 8, 16, 8, 16 ,8,16,8,8};
            precisions = new double[] { 0, 0.1,0.1, 0.01, 1, 1, 0.1, 1,1,1,0.1,1,1 };
            int i = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(fuelcell)))
            {
                name = Enum.GetName(typeof(fuelcell), item);
                if (name.Equals("高压DCDC状态"))
                {
                    name = "高压DC/DC状态";
                }
                if (!bitlength.ContainsKey(name))
                {
                    bitlength.Add(name, bitlengths[i]);
                    precision.Add(name, precisions[i]);
                }
                i++;
                dr = dt.NewRow();
                dr["数据项目名称"] = name;
                if (name.Equals(Enum.GetName(typeof(fuelcell), 0)))
                {
                    foreach (int hitem in Enum.GetValues(typeof(header)))
                    {

                        String hname = Enum.GetName(typeof(header), hitem);
                        if (hname.Equals("数据项目名称"))
                        {
                            dr[hname] = "——" + name + "——";

                        }
                        else
                        {
                            dr[hname] = "——";
                        }

                        //dc = new DataColumn(name);
                        ////if(dt.Columns.Contains(dc))
                        //dt.Columns.Add(dc);
                    }

                }

                dt.Rows.Add(dr);
            }


        }
        /// <summary>
        /// 报警数据
        /// </summary>
        public void AlarmData()
        {
            bitlengths = new int[] { 0, 8,1 };
            precisions = new double[] { 0, 1, 1 };
            int i = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(alarm)))
            {
                name = Enum.GetName(typeof(alarm), item);
                if (name.Equals("DCDC温度报警"))
                {
                    name = "DC-DC温度报警";
                }
                if (name.Equals("DCDC状态报警"))
                {
                    name = "DC-DC状态报警";
                }
                if (i >= bitlengths.Count())
                {
                    i = bitlengths.Count() - 1;
                }
                if (!bitlength.ContainsKey(name))
                {
                    bitlength.Add(name, bitlengths[i]);
                    precision.Add(name, precisions[i]);
                }
                i++;
     
                dr = dt.NewRow();
                dr["数据项目名称"] = name;
                if (name.Equals(Enum.GetName(typeof(alarm), 0)))
                {
                    foreach (int hitem in Enum.GetValues(typeof(header)))
                    {

                        String hname = Enum.GetName(typeof(header), hitem);
                        if (hname.Equals("数据项目名称"))
                        {
                            dr[hname] = "——" + name + "——";

                        }
                        else
                        {
                            dr[hname] = "——";
                        }

                        //dc = new DataColumn(name);
                        ////if(dt.Columns.Contains(dc))
                        //dt.Columns.Add(dc);
                    }

                }

                dt.Rows.Add(dr);
            }


        }

        public void VoltageData()
        {
            bitlengths = new int[] { 0, 16, 16,16,16 };
            precisions = new double[] { 0, 0.1, 0.1 ,1,0.001};
            int j = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(voltage)))
            {
                name = Enum.GetName(typeof(voltage), item);
                if (name.Equals("单体电压"))
                {
                   
                    for (int i = 0; i < voltageCount; i++)
                    {
                        string vname = name + (i + 1);
                        if (j >= bitlengths.Count())
                        {
                            j = bitlengths.Count() - 1;
                        }
                        if (!bitlength.ContainsKey(vname))
                        {
                            bitlength.Add(vname, bitlengths[j]);
                            precision.Add(name, precisions[i]);
                        }
                        j++;
                        dr = dt.NewRow();
                        dr["数据项目名称"] = vname;
                        dt.Rows.Add(dr);

                    }

                }
                else
                {
                    if (j >= bitlengths.Count())
                    {
                        j = bitlengths.Count() - 1;
                    }
                    if (!bitlength.ContainsKey(name))
                    {
                        bitlength.Add(name, bitlengths[j]);
                        precision.Add(name, precisions[j]);
                    }
                    j++;
                    dr = dt.NewRow();
                    dr["数据项目名称"] = name;

                    if (name.Equals(Enum.GetName(typeof(voltage), 0)))
                    {
                        foreach (int hitem in Enum.GetValues(typeof(header)))
                        {

                            String hname = Enum.GetName(typeof(header), hitem);
                            if (hname.Equals("数据项目名称"))
                            {
                                dr[hname] = "——" + name + "——";

                            }
                            else
                            {
                                dr[hname] = "——";
                            }

                            //dc = new DataColumn(name);
                            ////if(dt.Columns.Contains(dc))
                            //dt.Columns.Add(dc);
                        }


                    }

                    dt.Rows.Add(dr);



                }


            }


        }


        public void TemperatureData()
        {
            bitlengths = new int[] { 0, 16, 8 };
            precisions = new double[] { 0, 1,1 };
            int j = 0;
            String name;
            DataRow dr;
            foreach (int item in Enum.GetValues(typeof(temperature)))
            {
                name = Enum.GetName(typeof(temperature), item);
                if (name.Equals("温度"))
                {
                    for (int i = 0; i < temperatureCount; i++)
                    {
                        string tname = name + (i + 1);
                        if (!offset.ContainsKey(tname))
                        {
                            offset.Add(tname, -40);
                        }
                        if (j >= bitlengths.Count())
                        {
                            j = bitlengths.Count() - 1;
                        }
                        if (!bitlength.ContainsKey(tname))
                        {
                            bitlength.Add(tname, bitlengths[j]);
                            precision.Add(name, precisions[j]);
                        }
                        j++;
                        dr = dt.NewRow();
                        dr["数据项目名称"] = tname;
                        dt.Rows.Add(dr);

                    }

                }
                else
                {
                    if (j >= bitlengths.Count())
                    {
                        j = bitlengths.Count() - 1;
                    }
                    if (!bitlength.ContainsKey(name))
                    {
                        bitlength.Add(name, bitlengths[j]);
                        precision.Add(name, precisions[j]);
                    }
                    j++;
                    dr = dt.NewRow();
                    dr["数据项目名称"] = name;

                    if (name.Equals(Enum.GetName(typeof(temperature), 0)))
                    {
                        foreach (int hitem in Enum.GetValues(typeof(header)))
                        {

                            String hname = Enum.GetName(typeof(header), hitem);
                            if (hname.Equals("数据项目名称"))
                            {
                                dr[hname] = "——" + name + "——";

                            }
                            else
                            {
                                dr[hname] = "——";
                            }

                            //dc = new DataColumn(name);
                            ////if(dt.Columns.Contains(dc))
                            //dt.Columns.Add(dc);
                        }

                    }

                    dt.Rows.Add(dr);



                }


            }


        }


        #endregion




        public string CreateCodeHead(string description, int dataCount)
        {
            description = description.Replace("——", null);
            String codeHead = "";
            codeHead += "\r\n" + "////////////////////" + description + "CANID配置///////" + company;
            switch (description)
            {
                case "整车数据":
                    codeHead +=
                        "//////////////////vehicleDataConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig vehicleDataConfig[" + dataCount + "] =";

                    break;
                case "驱动电机数据":

                    codeHead += "//////////////////MOTODataConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig driveMotoDataConfig[" + dataCount + "]=";
                    break;
                case "燃料电池数据":
                    codeHead += "//////////////////FuelcellDataConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig FuelcellDataConfig[" + dataCount + "]=";
                    break;
                case "发动机数据":
                    codeHead += "//////////////////EngineConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig engineDataConfig[" + dataCount + "] =";
                    break;
                case "极值数据":

                    codeHead += "//////////////////ExtremaConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig extremaConfig[" + dataCount + "]=";
                    break;
                case "报警数据":
                    codeHead += "//////////////////AlarmConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig alarmConfig[" + dataCount + "]=";
                    break;

                case "单体电压数据":
                    codeHead += "//////////////////singleVoltageConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig singleVoltageConfig[" + dataCount + "]=";
                    break;
                case "单体温度数据":
                    codeHead += "//////////////////singleTempConfig///////////////////////////////////////////////////";
                    codeHead += "\r\n" + "GBConfig singleTempConfig[" + dataCount + "]=";
                    break;
                default:
                    break;

            }
            codeHead += "\r\n";
            codeHead += "{";
            codeHead += "\r\n";
            codeHead += "  /**排列格式*********|起始字节***|*起始位|**位长**|**CANID****|**偏移量***|*****数据项目名称*************偏移量=国标-私标***********************************/";
            codeHead += "\r\n";
            return codeHead;
        }


        public string CreateCode()
        {
            String code = "";
            string codeBody = "";
            int dataCount = 0;
            string CANID = "0";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String first = "";
                string head = dt.Rows[i]["排列格式"].ToString();
                if (head.Equals("——"))
                {
                    continue;
                }
                codeBody += "\t" + "{";
                dataCount++;
                foreach (int item in Enum.GetValues(typeof(header)))
                {
                    String name = Enum.GetName(typeof(header), item);
                    if (name.Equals("数据项目名称"))
                    {
                        //dataCount = 0;
                        continue;
                    }
                    if (name.Equals("精度")) {
                        continue;
                    }
                    string str = dt.Rows[i][name].ToString().Replace(" ",null);
                    if (name.Equals("偏移量"))
                    {
                        //int n_offset = 0;
                        int p_offset = 0;
                        try
                        {
                            p_offset = Convert.ToInt32(str);

                        }
                        catch
                        {
                            p_offset = 0;

                        }

                        //int r_offset = 0;
                        //if (offset.ContainsKey((string)dt.Rows[i]["数据项目名称"]))
                        //{
                        //    n_offset = offset[(string)dt.Rows[i]["数据项目名称"]];
                        //    r_offset = n_offset - p_offset;
                        //}
                        //else
                        //{
                        //    r_offset = 0 - p_offset;

                        //}
                        CANID = dt.Rows[i]["CANID"].ToString();
                        if (CANID == "0" || CANID == "" || CANID == null)
                        {
                            str = "0";

                        }
                        else {

                            str = Convert.ToString(p_offset);
                        }
                
                        str = "              " + str;
                    }
                    if (name.Equals("排列格式"))
                    {
                        str = str.ToUpper();
                        if (str == null || str == "") {
                            str = "INTEL_TYPE";

                        }
                        if (str.Contains("_TYPE"))
                        {

                        }
                        else
                        {
                            str = str + "_TYPE";

                        }
                    }
                    if (str.Equals("INTEL_TYPE"))
                    {

                        str += "          ";
                    }
                    if (name.Equals("位长度"))
                    {
                        if (str == null || str == "")
                        {
                            str = "0";

                        }

                        str = "   " + str;

                    }
                    if (name.Equals("CANID"))
                    {
                        if (str == null || str == "")
                        {
                            str = "0";

                        }
                        str = str.Replace(" ", null);
                        str = str.Replace("O","0");
                        str = str.Replace("o", "0");
                        str = "       " + str;


                    }
                    //if (str.Length < 5)
                    //{
                    //    for (int j = 0; j < 5 - str.Length; j++)
                    //        str += "\t";
                    //}
                    if (name.Equals("起始字节"))
                    {
                        if (str == null || str == "")
                        {
                            str = "0";

                        }

                        if (str.Contains("startByte"))
                        //if (str.Contains("STARTBYTE"))
                        {
                            //str.Remove(0, 9);
                        }
                        else
                        {
                            str = str.ToUpper();
                            if (str.Contains("BYTE"))
                            {
                                str = str.Remove(0, 4);
                            }
                            str = "startByte" + str;

                        }
                    }
                    if (name.Equals("起始位"))
                    {
                        if (str == null || str == "")
                        {
                            str = "0";

                        }
                        if (str.Contains("startbit"))
                        //if (str.Contains("STARTBYTE"))
                        {
                            //str.Remove(0, 9);
                        }
                        else
                        {
                            str = str.ToUpper();
                            if (str.Contains("BIT"))
                            {
                                str = str.Remove(0, 3);
                            }
                            str = "startbit" + str;

                        }

                    }
                    if (name.Equals("备注"))
                    {
                        continue;
                        //codeBody = codeBody.Substring(0, codeBody.Length - 1);
                    }

                    codeBody += str;
                    codeBody += ",";
                 
                }
                codeBody = codeBody.Substring(0, codeBody.Length - 1);
                codeBody += "},";
                codeBody += "//" + dt.Rows[i]["数据项目名称"].ToString();
                string remark= dt.Rows[i]["备注"].ToString();
                if (CANID == ""||CANID==null) {
                    remark = "未找到该项目";
                }

                if (remark != "" && remark != null) {

                    codeBody += "----------" + remark;
                }
                
                codeBody += "\r\n";
                if (i + 1 < dt.Rows.Count)
                {
                    first = dt.Rows[i + 1]["排列格式"].ToString();

                }
                if (first.Equals("——") || i == dt.Rows.Count - 1)
                {
                    //if (i != 0)
                    //{
                    //    code += "};";
                    //    code += "\r\n";
                    //}
                    //dataCount--;
                    string description = dt.Rows[i - dataCount]["数据项目名称"].ToString();
                    code += CreateCodeHead(description, dataCount);
                    code += codeBody;
                    code += "};";
                    code += "\r\n";
                    codeBody = "";
                    dataCount = 0;
                    continue;
                }
                //for (int j = 0; j < dt.Columns.Count; j++) {
                //}
            }

            //if (code != "")
            //{
            //    code += "};";
            //}

            return code;
        }


        public void CreateFile(string code, out string NewFileName)
        {
            string exportPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase; ; //生成CSV路径
            NewFileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".c";
            try
            {

                exportPath = exportPath + "\\" + "Code" + "\\";
                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }
                FileStream fs1 = new FileStream(exportPath + NewFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);
                //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"));
                sw.Write(code);
                sw.Close();
                fs1.Close();
            }
            catch { }


        }
        public void copy2DataTable(DataTable sourceDt)
        {

            for (int i = 0; i < sourceDt.Rows.Count; i++)
            {
                string description = sourceDt.Rows[i]["数据项名称"].ToString();
                if (sourceDt_row_num.ContainsKey(description))
                {
                    if (description.Equals("燃料消耗率"))
                    {
                        description = "发动机燃料消耗率";
                        sourceDt_row_num.Add(description, i);
                    }

                    //sourceDt_row_num[description] = i;
                    continue;
                }
                else
                {
                    sourceDt_row_num.Add(description, i);
                }



            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
                string description = dt.Rows[i]["数据项目名称"].ToString();
                //if (description.Equals("DCDC状态"))
                //{

                //    description = "DC-DC状态";
                //}
                int sdt_rowsNum = 0;
                if (dt.Rows[i]["排列格式"].ToString() == "——")
                {

                    continue;
                }
                else {
                    dt.Rows[i]["CANID"] = null;
                    dt.Rows[i]["排列格式"] = null;
                    dt.Rows[i]["起始字节"] = null;
                    dt.Rows[i]["起始位"] = null;
                    dt.Rows[i]["备注"] = null;
                    dt.Rows[i]["位长度"] = null;
                    dt.Rows[i]["精度"] = null;
                    dt.Rows[i]["偏移量"] = null;


                }


                if (sourceDt_row_num.ContainsKey(description))
                {
                    sdt_rowsNum = sourceDt_row_num[description];
                    string CANID = ((string)sourceDt.Rows[sdt_rowsNum]["CANID"]).Replace(" ", null);

                    //CANID = CANID.Replace(" ","");


                    CANID = CANID.Replace("O", "0");
                    CANID = CANID.Replace("o", "0");
                    if (CANID == "" || CANID == null)
                    {
                        CANID = "0";
                        dt.Rows[i]["备注"] += "未找到该项目";
                    }
                    dt.Rows[i]["CANID"] = CANID;
                    string format = sourceDt.Rows[sdt_rowsNum]["排列格式(MOTOROLA/INTEL)"].ToString().ToUpper().Replace(" ", null);
              
                    if (format == "" || format == null)
                    {
                        format = "INTEL_TYPE";
                    }
                    if (!format.Contains("_TYPE"))
                    {
                        format += "_TYPE";

                    }
                    dt.Rows[i]["排列格式"] = format;
                    string startByte = sourceDt.Rows[sdt_rowsNum]["起始字节(从0开始)"].ToString().Replace(" ", null);
                    startByte = startByte.ToUpper();
                    if (startByte == "" || startByte == null)
                    {
                        startByte = "0";
                    }
                    if (startByte.Contains("BYTE"))
                    {
                        startByte = startByte.Remove(0, 4);
                    }
                    dt.Rows[i]["起始字节"] = startByte;
                    string startBit = sourceDt.Rows[sdt_rowsNum]["起始位(从0开始)"].ToString().Replace(" ", null);
                    startBit = startBit.ToUpper();
                    if (startBit == "" || startBit == null)
                    {
                        startBit = "0";
                    }

                    if (startBit.Contains("BIT"))
                    {
                        startBit = startBit.Remove(0, 3);

                    }
                    dt.Rows[i]["起始位"] = startBit;
                    int blength = 0;
                    string sblength = sourceDt.Rows[sdt_rowsNum]["位长度"].ToString().Replace(" ", null);
                    if (IsInt(sblength))
                    {
                        blength = Convert.ToInt32(sblength);
                        dt.Rows[i]["位长度"] = blength;
                        if (bitlength[description] != blength)
                        {

                            dt.Rows[i]["备注"] += "位长度应为" + bitlength[description];
                        }
                    }
                    else
                    {
                        if (sblength == "" || sblength == null)
                        {
                            sblength = "0";
                        }
                        dt.Rows[i]["位长度"] = sblength;

                    }


                    string accuracy= sourceDt.Rows[sdt_rowsNum]["精度"].ToString().Replace(" ", null);
                    accuracy = GetNumber(accuracy);
                    if (accuracy == "" || accuracy == null) {
                        if (CANID =="0")
                        {
                            accuracy = "0";
                        }
                        else {
                            accuracy = "1";

                        }

                        
                    }
                    dt.Rows[i]["精度"] = accuracy;


                    string offset = sourceDt.Rows[sdt_rowsNum]["偏移量"].ToString().Replace(" ", null);
                    if (offset == "" || offset == null)
                    {
                        offset = "0";
                    }

                    //if (offset.Equals(""))
                    //{

                    //    offset = "0";
                    //}
                    dt.Rows[i]["偏移量"] = offset;
                }
                else if (description.Equals("制动力状态"))
                {
                    sdt_rowsNum = sourceDt_row_num["档位信息"];
                    dt.Rows[i]["CANID"] = dt.Rows[i - 1]["CANID"];
                    dt.Rows[i]["排列格式"] = dt.Rows[i - 1]["排列格式"];
                    //dt.Rows[i]["CANID"] = sourceDt.Rows[sdt_rowsNum]["CANID"].ToString().Replace(" ", null);
                    //dt.Rows[i]["排列格式"] = sourceDt.Rows[sdt_rowsNum]["排列格式(MOTOROLA/INTEL)"].ToString().ToUpper().Replace(" ", null);
                    string startByte = sourceDt.Rows[sdt_rowsNum]["起始字节(从0开始)"].ToString().Replace(" ", null);
                    startByte = startByte.ToUpper();
                    if (startByte.Contains("BYTE"))
                    {
                        startByte = startByte.Remove(0, 4);
                    }
                    dt.Rows[i]["起始字节"] = startByte;
                    dt.Rows[i]["起始位"] = 4;
                    dt.Rows[i]["位长度"] = 1;
                    dt.Rows[i]["精度"] = 1;
                    dt.Rows[i]["偏移量"] = 0;
                }
                else if (description.Equals("驱动力状态"))
                {
                    sdt_rowsNum = sourceDt_row_num["档位信息"];
                    dt.Rows[i]["CANID"] = dt.Rows[i - 2]["CANID"];
                    dt.Rows[i]["排列格式"] = dt.Rows[i - 2]["排列格式"];
                    //dt.Rows[i]["CANID"] = sourceDt.Rows[sdt_rowsNum]["CANID"].ToString().Replace(" ", null);
                    //dt.Rows[i]["排列格式"] = sourceDt.Rows[sdt_rowsNum]["排列格式(MOTOROLA/INTEL)"].ToString().ToUpper().Replace(" ", null);
                    string startByte = sourceDt.Rows[sdt_rowsNum]["起始字节(从0开始)"].ToString().Replace(" ", null);
                    startByte = startByte.ToUpper();
                    if (startByte.Contains("BYTE"))
                    {
                        startByte = startByte.Remove(0, 4);
                    }
                    dt.Rows[i]["起始字节"] = startByte;
                    dt.Rows[i]["起始位"] = 5;
                    dt.Rows[i]["位长度"] = 1;
                    dt.Rows[i]["精度"] = 1;
                    dt.Rows[i]["偏移量"] = 0;
                }
                else {
        
                    dt.Rows[i]["排列格式"] ="INTEL_TYPE";
                    dt.Rows[i]["起始字节"] = 0;
                    dt.Rows[i]["起始位"] = 0;
                    dt.Rows[i]["位长度"] = 0;
                    dt.Rows[i]["CANID"] = 0;
                    dt.Rows[i]["精度"] = 0;
                    dt.Rows[i]["偏移量"] = 0;
                    dt.Rows[i]["备注"] = "未找到该项目";



                }

                //dt_row_num.Add(description, i);


            }

        }

        public static bool IsInt(string value)
        {
            if (value == null || value == "")
            {
                return false;
            }
            else {
                return Regex.IsMatch(value, @"^[+-]?\d*$");
            }

        }
        public static string GetNumber(string source) {

            //string source= "47.64483, -122.141197";
            //Regex reg = new Regex(@"-?[\d]+.?[\d]+");
            Regex reg = new Regex(@"(\d+(\.\d+)?)");
            Match mm = reg.Match(source);
            MatchCollection mc = reg.Matches(source);
            //foreach (Match m in mc)
            //{
            //    MessageBox.Show(m.Value);
            //}
            string number = mm.Groups[0].Value;
            return number;
        }
        
        //public Boolean isNumeric(String str)
        //{
        //    Pattern pattern = Pattern.compile("[0-9]*");
        //    Matcher isNum = pattern.matcher(str);
        //    if (!isNum.matches())
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}
