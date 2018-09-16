using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StokEkstresi
{
    public class StokEkstresi
    {
        public int SiraNo { get; set; }
        public string IslemTur { get; set; }
        public string EvrakNo { get; set; }
        public string Tarih { get; set; }
        public float GirisMiktar { get; set; }
        public float CikisMiktar { get; set; }
        public float StokMiktar { get; set; }
    }
}
