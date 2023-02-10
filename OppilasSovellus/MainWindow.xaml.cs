using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;

//for sql stuff
using System.Data.SqlClient;
using System.Data.Sql;
//for new sql con.open conn
using System.Configuration;
//for ado models






namespace OppilasSovellus
{
    //imutus nuget pack await syncille
    //Install-Package Microsoft.Bcl.Async -Version 1.0.168


    public partial class MainWindow : Window
    {
        //lue pöytä
        private readonly OppilaatEntities1 oppilaatEntities;
        //private readonly Oppilastiedot kaikkiOppilaat;

        OppilaatEntities1 db = new OppilaatEntities1();

        private static System.Timers.Timer ajastin;

        //comboboxia varten
        //vanha
        //public static SqlConnection con = new SqlConnection();

        public MainWindow()
        {
            InitializeComponent();
            oppilaatEntities = new OppilaatEntities1();
            funktioOppilaatCombo();
            //btnTallenaMuutos.IsEnabled = false;
            //TeeAjastin();

        }
        //bool apina = true;

        async Task waittimer()
        {
            await Task.Delay(100);
        }
        //2 alla olevaa voidia / .2 sekuntti viive for looppeihi venaa() methodilla
        private void tickvoidi(object sender, EventArgs e)
        {
            //txtboxEtu.Opacity = DateTime.Now.Millisecond;
            CommandManager.InvalidateRequerySuggested();
        }
        private void viive()
        {
            DispatcherTimer venaa = new DispatcherTimer();
            venaa.Tick += new EventHandler(tickvoidi);
            venaa.Interval = new TimeSpan(0, 0, 0, 100);
            venaa.Start();
        }
        /*
        private void txtboxEtu_TextChanged(object sender, TextChangedEventArgs e)
        {

            txtboxEtu.Opacity = 1;
            /*
            for (float i = 0; i < 5; i++)
            {
                txtboxEtu.Opacity -= 0.1;
            }
            for (float a = 0; a < 5; a++)
            {
                txtboxEtu.Opacity += 0.1;
            }
            */
        ///Task waiter = Task.Delay(100);
        /*
    }
    */

        private static void TeeAjastin()
        {
            ajastin = new System.Timers.Timer(100); // 1 kymmenesosa /sekuntti
            //ajastin.Elapsed += OnTimedEvent();
            ajastin.AutoReset = true;
            ajastin.Enabled = true;
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {

            //OppilaatEntities oppilaat = new OppilaatEntities();

            /*foreach (var item in oppilaat.Oppilastiedot )
            {
                MessageBox.Show("Etunimi: " + item.Etunimi + "\n" +"Sukunimi " +  item.Sukunimi + "\n" + "Paikkakunta: " + item.Paikkakunta);
            }
            */

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            var oppilaat = oppilaatEntities.Oppilastiedot.ToList();
            //cmbBoxOppilaat.ItemsSource = oppilaatEntities.Oppilastiedot;
            cmbBoxOppilaat.DisplayMemberPath = "Etunimi";
            cmbBoxOppilaat.SelectedValuePath = "Etunimi";
            */
        }

        private void funktioOppilaatCombo() //HAKEE OPPILLAAT DBSTÄ comboboxii
        {
            //comboOppilaslistalle luotu oma class määritelmä ( comboOppilaat.cs )
            List<comboOppilaat> combooppilaat = new List<comboOppilaat>();

            OppilaatEntities1 op = new OppilaatEntities1();

            var obiskelija = from ob in op.Oppilastiedot
                             orderby ob.Etunimi //järkkää listan etunimen mukaa

                             select ob;

            foreach (var obbilas in obiskelija)
            {
                combooppilaat.Add(new comboOppilaat(obbilas.Etunimi, obbilas.Sukunimi,
                    obbilas.Osoite, obbilas.Paikkakunta, obbilas.Postinumero, obbilas.Syntymapaiva, obbilas.id));
            }

            //cBoxOppilaat = comboboxin target name
            cBoxOppilaat.DisplayMemberPath = "oppilasNimi";
            cBoxOppilaat.SelectedValuePath = "oppilasSukunimi";
            cBoxOppilaat.ItemsSource = combooppilaat;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            lisääOppilas();
            tyhjennäKentät();
            funktioOppilaatCombo();
        }

        private void lisääOppilas()
        {
            //bool | var checkki hidasta käyttäjää varten
            var kunnossa = true;
            var virheViesti = "";

            var tallennaOppilas = new Oppilastiedot();
            //getInfo();
            //kerää täytetyt kentät ja luo niille nimi
            //käytä tboxEtua txtBoxEtu sijaan!!

            string nimi = tboxEtu.Text.ToString();
            string snimi = tboxSuku.Text.ToString();
            string osis = tboxOsoite.Text.ToString();
            string syntis = tboxSyntyma.Text.ToString();
            string paikkis = tboxPaikkakunta.Text.ToString();
            string postis = tboxPostiNumero.Text.ToString();


            //model fieldit
            tallennaOppilas.Etunimi = nimi;
            tallennaOppilas.Sukunimi = snimi;
            tallennaOppilas.Osoite = osis;
            tallennaOppilas.Syntymapaiva = syntis;
            tallennaOppilas.Paikkakunta = paikkis;
            tallennaOppilas.Postinumero = postis;

            //tsekkaa jos kentät tyhjänä
            if (string.IsNullOrWhiteSpace(nimi) || string.IsNullOrWhiteSpace(snimi)
                || string.IsNullOrWhiteSpace(osis) || string.IsNullOrWhiteSpace(syntis)
                || string.IsNullOrWhiteSpace(paikkis) || string.IsNullOrWhiteSpace(postis))

            {
                kunnossa = false;
                virheViesti += "Virhe: Puuttuvia oppilastietoja!";
            }
            if (kunnossa) // jos ei huuda virhettä tee nämä
            {
                //tallenna ja lisää db
                oppilaatEntities.Oppilastiedot.Add(tallennaOppilas);

                oppilaatEntities.SaveChanges();


                //käytettään tyhjennäkentät rutiinia sen sijaa
                tyhjennäKentät();
                //onnistunut teksti
                string lisäätxt = "Oppilaan lisääminen onnistui! \n \r";
                string tiedot = "\n" + "Etunimi:  " + nimi + "\nSukunimi:  " + snimi +
                    "\nSyntymäpäivä:  " + syntis + "\nOsoite:  " + osis + "\nPostinumero:  " + postis + "\nPaikkakunta:  "
                    + paikkis;
                //näytä onnistunut tallennus pop up
                MessageBox.Show(lisäätxt + tiedot);
            }
            else
            {
                //kussut tallennus pop up
                MessageBox.Show(virheViesti);
            }
        }
        private void btnTallenaMuutos_Click(object sender, RoutedEventArgs e)
        {
            if ( tboxEtu.Text == "" )
            {
                return;
            }
            TallennaMuutokset();
            funktioOppilaatCombo();
            tyhjennäKentät();
            //db.SaveChanges();
        }

        private void TallennaMuutokset()
        {

            var kunnossa = true;
            var virheViesti = "";

            var tallennaOppilas = new Oppilastiedot();

            //tarvitsee id:n jotta voi löytää mätsäävän
            Oppilastiedot guugaa = db.Oppilastiedot.Find(int.Parse(tboxID.Text));
            //getInfo();
            //kerää täytetyt kentät ja luo niille nimi
            //käytä tboxEtua txtBoxEtu sijaan!!
            //string id = tboxID.Text.ToString();
            string nimi = tboxEtu.Text;
            string snimi = tboxSuku.Text;
            string osis = tboxOsoite.Text;
            string syntis = tboxSyntyma.Text;
            string paikkis = tboxPaikkakunta.Text;
            string postis = tboxPostiNumero.Text;


            //model fieldit
            guugaa.Etunimi = nimi;
            guugaa.Sukunimi = snimi;
            guugaa.Osoite = osis;
            guugaa.Syntymapaiva = syntis;
            guugaa.Paikkakunta = paikkis;
            guugaa.Postinumero = postis;

            //tsekkaa jos kentät tyhjänä
            if (string.IsNullOrWhiteSpace(nimi) || string.IsNullOrWhiteSpace(snimi)
                || string.IsNullOrWhiteSpace(osis) || string.IsNullOrWhiteSpace(syntis)
                || string.IsNullOrWhiteSpace(paikkis) || string.IsNullOrWhiteSpace(postis))

            {
                kunnossa = false;
                virheViesti += "Virhe: Tallennus ei onnistunut!";
            }
            if (kunnossa) // jos ei huuda virhettä tee nämä
            {
                //tallenna ja lisää db

                db.SaveChanges();

                tyhjennäKentät();

                //onnistunut teksti
                string tallennustxt = "Oppilaan tietojen muokkaus & tallentuminen onnistui! \n \r";
                string tiedot = "\n" + "Etunimi:  " + nimi + "\nSukunimi:  " + snimi +
                    "\nSyntymäpäivä:  " + syntis + "\nOsoite:  " + osis + "\nPostinumero:  " + postis + "\nPaikkakunta:  "
                    + paikkis;
                //näytä onnistunut tallennus pop up
                MessageBox.Show(tallennustxt + tiedot);
            }
            else
            {
                //kussut tallennus pop up
                MessageBox.Show(virheViesti);
            }
            //tyhjennäKentät();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tyhjennäKentät();
        }

        

        private void tyhjennäKentät()
        {
            tboxEtu.Text = "";
            tboxSuku.Text = "";
            tboxOsoite.Text = "";
            tboxSyntyma.Text = "";
            tboxPaikkakunta.Text = "";
            tboxPostiNumero.Text = "";
        }

        private void CBoxOppilaat_DropDownClosed(object sender, EventArgs e)
        {
            comboOppilaat op = ( comboOppilaat )cBoxOppilaat.SelectedItem;
            //Oppilastiedot op = ( Oppilastiedot )cBoxOppilaat.SelectedItem;

            /* tsekkaa jos comboboxista on valittu oppilas,
                koska jos klikkaa box ja ei valitse mitään, heittää muuten errorin
                    jos yrittää täyttää kentät "tyhjästä oletuksesta"*/

            if (cBoxOppilaat.SelectedItem != null)
            {
                //oppilasNimi etc defined comboOppilaat luokan sisällä!
                int OppilasID = op.oppilasID;
                string OppilasEtu = op.oppilasNimi;
                string OppilasSuku = op.oppilasSuku;
                string OppilasOsoite = op.oppilasOsoite;
                string OppilasSyntyma = op.oppilasSyntyma;
                //tuskin tarpeellista convertaa tätä sovellusta varten postinumero stringistä inttii
                //int OppilasPosti = Convert.ToInt32(op.Postinumero);
                string OppilasPosti = op.oppilasPosti;
                string OppilasKunta = op.oppilasPaikka;

                // syötä textboxeihi variablet comboboxista
                tboxEtu.Text = OppilasEtu;
                tboxSuku.Text = OppilasSuku;
                tboxOsoite.Text = OppilasOsoite;
                tboxPaikkakunta.Text = OppilasKunta;
                tboxPostiNumero.Text = OppilasPosti;
                tboxSyntyma.Text = OppilasSyntyma;
                tboxID.Text = OppilasID.ToString();


                string tempname = OppilasPosti;

                //btnTallenaMuutos.IsEnabled = false;

            }
            //return;

        }
        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            btnTallenaMuutos.IsEnabled = true;
            if (btnTallenaMuutos.IsEnabled)
            {
                db.SaveChanges();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if ( tboxEtu.Text == "" )
            {
                return;
            }
            poistaOppilas();
            tyhjennäKentät();
            funktioOppilaatCombo();
        }

        private void poistaOppilas()
        {
            Oppilastiedot guugaa = db.Oppilastiedot.Find(int.Parse(tboxID.Text));
            
            //initate stringit jotta voi näyttää etu & suku messageboxissa oppilaan poiston yhteydessä
            string nimi = tboxEtu.Text;
            string snimi = tboxSuku.Text;

            //model fieldit
            guugaa.Etunimi = nimi;
            guugaa.Sukunimi = snimi;

            //poista
            string poisto = "Oppilas " + "[ " + nimi + " " + snimi + " ]" + " on poistettu tietokannasta";
            db.Oppilastiedot.Remove(guugaa);
            db.SaveChanges();
            
            MessageBox.Show(poisto );
        }

    }
}
