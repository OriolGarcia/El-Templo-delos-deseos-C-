using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSpiritualDimension
{
    class FilaNumeroSagrado
    {
        public string Numero { get; set; }
        public string beneficio { get; set; }
        public string benefactor { get; set; }
        public bool CSoGrabovoi { get; set; }
        public  FilaNumeroSagrado(string num, string beneficio, string benefactor, bool CSoGrabovoi) {
            this.Numero = num;
            this.beneficio = beneficio;
            this.benefactor = benefactor;
            this.CSoGrabovoi = CSoGrabovoi;

        }
    }
}
