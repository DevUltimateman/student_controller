using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OppilasSovellus
{
    class comboOppilaat
    {
        public string oppilasNimi { get; set; }

        public string oppilasSuku { get; set; }

        public string oppilasPosti { get; set; }

        public string oppilasOsoite { get; set; }

        public string oppilasPaikka { get; set; }

        public string oppilasSyntyma { get; set; }

        public int oppilasID { get; set; }

        
        public comboOppilaat( string OppilasNimi, string OppilasSuku, string OppilasPosti,
            string OppilasOsoite, string OppilasPaikka, string OppilasSyntyma , int ids)
        {
            oppilasNimi = OppilasNimi;
            oppilasSuku = OppilasSuku;
            oppilasPosti = OppilasPosti;
            oppilasOsoite = OppilasOsoite;
            oppilasPaikka = OppilasPaikka;
            oppilasSyntyma = OppilasSyntyma;
            oppilasID = ids;
        }
    }
}
