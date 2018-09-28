using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StokEkstresi
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            populateMalKodu();
            //txtStartDate.DateTime = DateTime.Now;
            //txtEndDate.DateTime = DateTime.Now.AddMonths(1);
            getData("%", DateTime.MinValue, DateTime.MaxValue);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStartDate.DateTime.ToString()) || string.IsNullOrEmpty(txtEndDate.DateTime.ToString()))
            {
                MessageBox.Show("Lütfen tarih aralığını seçiniz.");
                return;

            }

            if (txtStartDate.DateTime > txtEndDate.DateTime)
            {
                MessageBox.Show("Başlangıç tarihi bitiş tarihinden sonra olamaz.");
                return;
            }

                string malkodu = luItem.Text.ToString();
                //Alttaki yazım şekli ile LookUpEdit'in kontrolünü "if" ile çözmeye çalıştığımızda kontrol edemiyor
                // string malkodu = luItem.Properties.GetKeyValueByDisplayText(luItem.Text).ToString();
                if (string.IsNullOrEmpty(malkodu))
                {
                    getData("%", txtStartDate.DateTime, txtEndDate.DateTime);
                }
                else
                {
                    getData(malkodu, txtStartDate.DateTime, txtEndDate.DateTime);
                }
            
            
        }

        void populateMalKodu()
        {
            SqlConnection cnn = new SqlConnection(@"server=JESUS\SQLEXPRESS;database=Test;uid=sa;pwd=123");
            SqlCommand cmd = new SqlCommand("SELECT MalKodu, MalAdi FROM Test.dbo.STK ", cnn);
            cnn.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd); /*DataAdapter ile aynı işlemi(okuma) yapıyor ikisini de kullanmak istedim*/
            DataTable dt = new DataTable();
            da.Fill(dt); /*"Fill" DataTable'a verileri doldurma işlemi yapıyor*/

            luItem.Properties.DataSource = dt;
            luItem.Properties.ValueMember = "MalKodu";
            luItem.Properties.DisplayMember = "MalAdi";
            luItem.Properties.ForceInitialize();
            luItem.Properties.PopulateColumns();

            da.Dispose();
            dt.Dispose();
            cnn.Close();
            cnn.Dispose();
        }

        void getData(string _malzemeKodu, DateTime _startDate, DateTime _endDate)
        {
            SqlConnection cnn = new SqlConnection(@"server=JESUS\SQLEXPRESS;database=Test;uid=sa;pwd=123");
            SqlCommand cmd = new SqlCommand("sp_StokEkstresi", cnn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter malzemeKodu = cmd.Parameters.Add("@MalKodu", SqlDbType.NVarChar, 30);
            malzemeKodu.Direction = ParameterDirection.Input;

            SqlParameter startDate = cmd.Parameters.Add("@StartDate", SqlDbType.Int);
            startDate.Direction = ParameterDirection.Input;

            SqlParameter endDate = cmd.Parameters.Add("@EndDate", SqlDbType.Int);
            endDate.Direction = ParameterDirection.Input;

            malzemeKodu.Value = _malzemeKodu;
            startDate.Value = Convert.ToInt32(_startDate.ToOADate());
            endDate.Value = Convert.ToInt32(_endDate.ToOADate());

            cnn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            List<StokEkstresi> StokEkstresiList = new List<StokEkstresi>();

            float stokMiktari = 0;
            while (dr.Read())
            {

                //float miktar = float.Parse(dr["Miktar"].ToString());
                float miktar;
                if (float.TryParse(dr["Miktar"].ToString(), out miktar))
                {
                    /*Parse işlemleri iki şekilde yapılabilir. TryParse ve Parse
                     * TryParse boolean tipinde değer döndürür*/

                    if (int.Parse(dr["IslemTurCode"].ToString()) == 0)
                    {
                        stokMiktari += miktar;
                    }
                    else
                    {
                        stokMiktari -= miktar;
                    }

                    StokEkstresiList.Add(new StokEkstresi
                    {
                        SiraNo = int.Parse(dr["SiraNo"].ToString()),
                        IslemTur = dr["IslemTur"].ToString(),
                        EvrakNo = dr["EvrakNo"].ToString(),
                        Tarih = dr["Tarih"].ToString(),
                        GirisMiktar = float.Parse(dr["GirisMiktar"].ToString()),
                        CikisMiktar = float.Parse(dr["CikisMiktar"].ToString()),
                        StokMiktar = stokMiktari
                    });
                }

               
            };

            dr.Close();
            cnn.Close();
            cnn.Dispose();

            gridStokEkstresi.DataSource = null;
            gridStokEkstresi.DataSource = StokEkstresiList;
        }
    }
}
