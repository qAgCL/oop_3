using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    using Classes;
    using Sire;
  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static int printField(Type type1, List<Control> controls, int x, int y)
        {
            FieldInfo[] fieldinfo1 = type1.GetFields();
            foreach (var field1 in fieldinfo1)
            {
                Type type2 = Type.GetType(field1.FieldType.ToString());
                if (type2.IsEnum)
                {
                    controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 10 + y * 28), Text = type1.Name + "   " + field1.Name });
                    y++;
                    ComboBox buf = new ComboBox() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Name = field1.Name, Location = new Point(0 + x, 10 + y * 28) };
                    FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (var field2 in fieldinfo2)
                    {
                        buf.Items.Add(field2.Name.ToString());
                    }
                    controls.Add(buf);
                    y++;
                }
                else if ((type2.IsClass) && (type2.Name != "String"))
                {
                    y = printField(field1.FieldType, controls, x, y);
                }
                else
                {
                    controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 10 + y * 28), Text = type1.Name + "   " + field1.Name });
                    y++;
                    controls.Add(new TextBox() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), Name = field1.Name, Location = new Point(0 + x, 10 + y * 28) });
                    y++;
                }
               
            }
            return y;
        }
        public static int printVal(Object obj,List<Control> controls, int x, int y)
        {
            if (obj != null)
            {
                Type type1 = obj.GetType();
                Console.WriteLine(type1.Name);
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if (type2.IsEnum)
                    {
                        controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 28 + y * 30), Text = type1.Name + "   " + field1.Name });
                        y++;
                        ComboBox buf = new ComboBox() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Name = field1.Name, Location = new Point(0 + x, 28 + y * 30) };
                        FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                        foreach (var field2 in fieldinfo2)
                        {
                            buf.Items.Add(field2.Name.ToString());
                        }
                        buf.Text = field1.GetValue(obj).ToString();
                        controls.Add(buf);
                        y++;
                    }
                    else if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        y = printVal(field1.GetValue(obj), controls, x, y);
                    }
                    else
                    {
                        controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 28 + y * 30), Text = type1.Name + "   " + field1.Name });
                        y++;
                        controls.Add(new TextBox() { Text = field1.GetValue(obj).ToString(), Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), Name = field1.Name, Location = new Point(0 + x, 28 + y * 30) });
                        y++;
                    }
                }
            }
            return y;
        }
        public static bool customDeSer(Object obj, List<String> nameFields, List<String> valueFields, List<Object> objects)
        {
            if (obj != null)
            {
                Type type1 = obj.GetType();
                objects.Add(obj);
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        customDeSer(field1.GetValue(obj), nameFields, valueFields, objects);
                    }
                    else if (type2.IsEnum)
                    {
                        FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                        bool flag = false;
                        int index = 0;
                        foreach (String nameField in nameFields)
                        {
                            if (nameField == field1.Name.ToString())
                            {
                                Object val = valueFields[index];
                                foreach (var field2 in fieldinfo2)
                                {
                                    if (val.ToString() == field2.Name)
                                    {
                                        field1.SetValue(obj, field2.GetValue(obj));
                                    }
                                }
                            }
                            index++;
                        }
                        if (flag)
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                    }
                    else
                    {
                        Object val = 0;
                        int index = 0;
                        foreach (String nameField in nameFields)
                        {
                            if (nameField == field1.Name.ToString())
                            {
                                val = valueFields[index];
                            }
                        }
                        try
                        {
                            val = Convert.ChangeType(val, field1.FieldType);
                        }
                        catch
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                        field1.SetValue(obj, val);
                    }
                }
            }
            return true;
        }
        public static bool setVal(Object obj,List<Control> controls, List<Object> objects)
        {
            if (obj != null)
            {
                Type type1 = obj.GetType();
                objects.Add(obj);
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        setVal(field1.GetValue(obj), controls, objects);
                    }
                    else if (type2.IsEnum)
                    {
                        FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                        bool flag = false;
                        foreach (Control control in controls)
                        {
                            if (control.Name.ToString() == field1.Name.ToString())
                            {
                                Object val = control.Text;
                                foreach (var field2 in fieldinfo2)
                                {
                                    if (val.ToString() == field2.Name)
                                    {
                                        field1.SetValue(obj, field2.GetValue(obj));
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                    }
                    else
                    {
                        Object val = 0;
                        foreach (Control control in controls)
                        {
                            if (control.Name.ToString() == field1.Name.ToString())
                            {
                                val = control.Text;
                            }
                        }
                        try
                        {
                            val = Convert.ChangeType(val, field1.FieldType);
                        }
                        catch
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                        field1.SetValue(obj, val);
                    }
                }
            }
            return true;
        }
        public static void customSer(Object obj,ref string test,bool agr)
        {
            if (obj != null)
            {
                if (agr)
                {
                    test += "<MainClass>\n";
                }
                else
                {
                    test += "<Class>\n";
                }
                Type type1 = obj.GetType();
                test += "   "+type1.Name + "\n";
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if (type2.IsEnum)
                    {
                        test += "<Field>\n";
                        test +="    |Name-"+field1.Name+"|";
                        test += "|Value-" + field1.GetValue(obj).ToString() + "|\n";
                        test += "</Field>\n";
                        
                    }
                    else if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        agr = false;
                        customSer(field1.GetValue(obj),ref test,agr);
                        agr = true;
                    }
                    else
                    {
                        test += "<Field>\n";
                        test += "   |Name-" + field1.Name + "|";
                        test += "|Value-" + field1.GetValue(obj).ToString() + "|\n";
                        test += "</Field>\n";
                    }
                    
                }
                if (agr)
                {
                    test += "</MainClass>\n";
                }
                else
                {
                    test += "</Class>\n";
                }
            }
        }
        public static Object create_obj(Type loc_type)
        {
            ConstructorInfo[] cons = loc_type.GetConstructors();
            ParameterInfo[] pars = cons[0].GetParameters();
            List<Object> test = new List<Object>();
            if (pars.Length == 0)
            {
                return Activator.CreateInstance(loc_type);
            }
            else
            {
                foreach (var para in pars)
                {
                    test.Add(create_obj(para.ParameterType));
                }
            }
            return Activator.CreateInstance(loc_type, test.ToArray());
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "Classes").ToArray();
            foreach (Type type in typelist)
            {
               
                if (type.IsClass)
                {
                    comboBox1.Items.Add(type.Name);
                }
            }
            comboBox1.SelectedIndex = 0;
            var types = GetType().Assembly.GetTypes().Where(t => typeof(ISerli).IsAssignableFrom(t) && !t.IsAbstract).ToList();
            foreach (var type in types)
            {
                comboBox2.Items.Add(type.Name);
            }
            comboBox2.SelectedIndex = 0;
        }
        List<Control> controls = new List<Control>();
        Object bufObject;
        List<Object> allObj = new List<Object>();

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control control in controls)
            {
                this.Controls.Remove(control);
            }
            controls.Clear();
            bufObject = create_obj(Type.GetType("Classes."+comboBox1.Text));
            printField(bufObject.GetType(), controls, 700, 0);
            foreach (Control control in controls)
            {
                this.Controls.Add(control);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Object> loc_arrObj = new List<Object>();
            if (setVal(bufObject, controls, loc_arrObj))
            {
                foreach (Control control in controls)
                {
                    this.Controls.Remove(control);
                }
                controls.Clear();
                int count = allObj.Count();
                foreach (Object obj in loc_arrObj)
                {
                    allObj.Add(obj);
                    listBox1.Items.Add(obj.ToString()+"_"+count.ToString());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            foreach (Control control in controls)
            {
                this.Controls.Remove(control);
            }
            controls.Clear();
            printVal(allObj[listBox1.SelectedIndex], controls, 700, 0);
            foreach (Control control in controls)
            {
                this.Controls.Add(control);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            List<Object> loc_arrObj = new List<Object>();
            if (setVal(allObj[listBox1.SelectedIndex], controls, loc_arrObj))
            {
                foreach (Control control in controls)
                {
                    this.Controls.Remove(control);
                }
                controls.Clear();
            }
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            allObj.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            foreach (Control control in controls)
            {
                this.Controls.Remove(control);
            }
            controls.Clear();
            ISerli test;
            test = (ISerli)Activator.CreateInstance(Type.GetType("Sire." + comboBox2.Text));
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            test.Serialize(allObj, filename);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            foreach (Control control in controls)
            {
                this.Controls.Remove(control);
            }
            controls.Clear();
            ISerli test;
            test = (ISerli)Activator.CreateInstance(Type.GetType("Sire."+ comboBox2.Text));
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            List<Object> loc_arrObj = new List<Object>();
            loc_arrObj= test.Deserialize(filename);
            int count = allObj.Count();
            foreach (Object obj in loc_arrObj)
            {
                allObj.Add(obj);
                listBox1.Items.Add(obj.ToString() + "_" + count.ToString());
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
namespace Sire
{
    using Classes;
    class CustomSir : ISerli
    {
        public static bool customDeSer(Object obj, List<String> nameFields, List<String> valueFields, List<Object> objects)
        {
            if (obj != null)
            {
                Type type1 = obj.GetType();
                objects.Add(obj);
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        customDeSer(field1.GetValue(obj), nameFields, valueFields, objects);
                    }
                    else if (type2.IsEnum)
                    {
                        FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                        bool flag = false;
                        int index = 0;
                        foreach (String nameField in nameFields)
                        {
                            if (nameField == field1.Name.ToString())
                            {
                                Object val = valueFields[index];
                                foreach (var field2 in fieldinfo2)
                                {
                                    if (val.ToString() == field2.Name)
                                    {
                                        field1.SetValue(obj, field2.GetValue(obj));
                                    }
                                }
                            }
                            index++;
                        }
                        if (flag)
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                    }
                    else
                    {
                        Object val = 0;
                        int index = 0;
                        foreach (String nameField in nameFields)
                        {
                            if (nameField == field1.Name.ToString())
                            {
                                val = valueFields[index];
                            }
                        }
                        try
                        {
                            val = Convert.ChangeType(val, field1.FieldType);
                        }
                        catch
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                        field1.SetValue(obj, val);
                    }
                }
            }
            return true;
        }
        public static Object create_obj(Type loc_type)
        {
            ConstructorInfo[] cons = loc_type.GetConstructors();
            ParameterInfo[] pars = cons[0].GetParameters();
            List<Object> test = new List<Object>();
            if (pars.Length == 0)
            {
                return Activator.CreateInstance(loc_type);
            }
            else
            {
                foreach (var para in pars)
                {
                    test.Add(create_obj(para.ParameterType));
                }
            }
            return Activator.CreateInstance(loc_type, test.ToArray());
        }
        public static void customSer(Object obj, ref string test, bool agr)
        {
            if (obj != null)
            {
                if (agr)
                {
                    test += "<MainClass>\n";
                }
                else
                {
                    test += "<Class>\n";
                }
                Type type1 = obj.GetType();
                test += "   " + type1.Name + "\n";
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {

                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if (type2.IsEnum)
                    {
                        test += "<Field>\n";
                        test += "    |Name-" + field1.Name + "|";
                        test += "|Value-" + field1.GetValue(obj).ToString() + "|\n";
                        test += "</Field>\n";

                    }
                    else if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        agr = false;
                        customSer(field1.GetValue(obj), ref test, agr);
                        agr = true;
                    }
                    else
                    {
                        test += "<Field>\n";
                        test += "   |Name-" + field1.Name + "|";
                        test += "|Value-" + field1.GetValue(obj).ToString() + "|\n";
                        test += "</Field>\n";
                    }

                }
                if (agr)
                {
                    test += "</MainClass>\n";
                }
                else
                {
                    test += "</Class>\n";
                }
            }
        }
        public void Serialize(List<Object> Object, string FileName)
        {
            string test = "";
            bool all = true;
            foreach (Object obj in Object)
            {
                customSer(obj, ref test, all);
                MessageBox.Show(obj.ToString());
                using (FileStream fstream = new FileStream(FileName, FileMode.OpenOrCreate))
                {

                    byte[] input = Encoding.Default.GetBytes(test);

                    fstream.Write(input, 0, input.Length);

                }
            }
        }
        public List<Object> Deserialize(string FileName)
        {
            
            string test;
            List<Object> loc_arrObj = new List<Object>();
            using (FileStream fs = File.OpenRead(FileName))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                test = System.Text.Encoding.Default.GetString(array);
            }
            try
            {
                string[] clssss = test.Split(new string[] { "</MainClass>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string hah in clssss)
                {
                    if (hah.Length > 1)
                    {
                        Regex regex = new Regex(@"<MainClass>[\s]+([\w]+)[\s]+<Field>");
                        string clas = "";
                        MatchCollection matches = regex.Matches(hah);
                        if (matches.Count > 0)
                        {
                            clas = matches[0].Groups[1].Value;
                        }
                        Regex regexF = new Regex(@"\|Name-([\w]+)\|\|Value-([\w]+)\|");
                        MatchCollection matchesF = regexF.Matches(hah);
                        List<string> fields = new List<string>();
                        List<string> values = new List<string>();
                        if (matchesF.Count > 0)
                        {
                            foreach (Match match in matchesF)
                            {
                                fields.Add(match.Groups[1].Value);
                                values.Add(match.Groups[2].Value);
                            }
                        }
                        Object bufObject = create_obj(Type.GetType("Classes." + clas));
                        customDeSer(bufObject, fields, values, loc_arrObj);
                       
                    }
                }
                return loc_arrObj;
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
            return loc_arrObj;
        }
    }
    class BinSir : ISerli
    {
  
        public void Serialize(List<Object> Object, string FileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Object);
            }
        }
        public List<Object> Deserialize(string FileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                try
                {
                    return (List<Object>)formatter.Deserialize(fs);
                }
                catch
                {
                    MessageBox.Show("Ошибка");
                }
            }
            List<Object> test = new List<Object>();
            return test;
        }
    };
    class XMLsir : ISerli
    {
   
        public void Serialize(List<Object> Object, string FileName)
        {
            Type[] extraTypes = new Type[8];
            extraTypes[0] = typeof(employee);
            extraTypes[1] = typeof(transport);
            extraTypes[2] = typeof(carpark);
            extraTypes[3] = typeof(engine);
            extraTypes[4] = typeof(airplane);
            extraTypes[5] = typeof(auto_transport);
            extraTypes[6] = typeof(ice_auto);
            extraTypes[7] = typeof(electro_auto);
            XmlSerializer formatter = new XmlSerializer(typeof(List<Object>), extraTypes);
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Object);
            }
        }
        public List<Object> Deserialize(string FileName)
        {
            Type[] extraTypes = new Type[8];
            extraTypes[0] = typeof(employee);
            extraTypes[1] = typeof(transport);
            extraTypes[2] = typeof(carpark);
            extraTypes[3] = typeof(engine);
            extraTypes[4] = typeof(airplane);
            extraTypes[5] = typeof(auto_transport);
            extraTypes[6] = typeof(ice_auto);
            extraTypes[7] = typeof(electro_auto);
            XmlSerializer formatter = new XmlSerializer(typeof(List<Object>), extraTypes);
            using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate))
            {

                try
                {
                    return (List<Object>)formatter.Deserialize(fs);
                }
                catch
                {
                    MessageBox.Show("Ошибка");
                }
            }
            List<Object> test = new List<Object>();
            return test;
        }
    }
    interface ISerli
    {
        void Serialize(List<Object> Object, string FileName);
        List<Object> Deserialize(string FileName);
    }
}
namespace Classes
{
    [Serializable]
    public enum yes_no
    {
        Yes,
        No,
    }
    [Serializable]
    public class airplane : transport
    {
       // public int flight_height;
        public int wingsspan;
        //  public int wing_area;
        public yes_no auto_pilot;
        public engine engine;
        public airplane(engine eng)
        {
            engine = eng;
        }
        public airplane() { }
    }
    [Serializable]
    public class auto_transport : transport
    {
        public enum drive
        {
            Front_wheel_drive,
            Real_wheel_drive,
            Four_wheel_drive,
        }
        // public int number_of_wheels;
        //public int tire_diameter;
        public int tire_width;
        //  public int tire_pressure;
        public drive drive_unit;
        public auto_transport() { }
    }
    [Serializable]
    public class carpark
    {
        public int area;
        //   public int car_capacity;
        //    public int number_of_cars;
        //    public int number_of_workers;
        public int profit;
        public yes_no car_wash;
        public yes_no gas_station;
        public yes_no service_station;
        public ice_auto auto;
        public employee employee;
        public carpark(employee emp, ice_auto aut)
        {
            auto = aut;
            employee = emp;
        }
        public carpark() { }
    }
    [Serializable]
    public class electro_auto : auto_transport
    {
        enum battery
        {

            Lithium_ion,
            Aluminum_ion,
            Lithium_sulfur,

        }
        //  public int battery_capacity;
        public int charging_time;
        //  public int power_reserve;
        public int battery_consumption;
        public yes_no quick_charge;
        public engine engine;
        public electro_auto(engine eng)
        {
            engine = eng;
        }
        public electro_auto() { }
    }
    [Serializable]
    public enum Gender
    {
        Female,
        Male,
        Agender,
        Bigender,
    }
    public class employee
    {
        public int height;
        //    public int weight;
        public int age;
        //     public string education;
        //     public string name;
        public string surname;
        public Gender gender;
        public int salary;
        public employee() { }
    }
    [Serializable]
    public class engine
    {
        public int weight;
        public int year_of_creation;
        public int service_life;
        public int power;
        public int torque;
        public engine() { }

    }
    [Serializable]
    public class transport
    {
        //   public int length;
        public int weight;
        public int height;
        // public int width;
        //    public int carrying;
        public string color;
        public transport() { }
    }
    [Serializable]
    public class ice_auto : auto_transport
    {
        public enum fuel
        {
            Gas,
            Petrol,
            Diesel,
        }
        // public int gas_tank_volume;
        // public int fuel_consumption;
        public int oil_consumption;
        // public int co2_emissions;
        public fuel fuel_grade;
        public engine engine;
        public ice_auto(engine eng)
        {
            engine = eng;
        }
        public ice_auto() { }
    }
}

