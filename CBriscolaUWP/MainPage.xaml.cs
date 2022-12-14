using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace CBriscolaUWP
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static giocatore g, cpu, primo, secondo, temp;
        private static mazzo m;
        private static carta c, c1, briscola;
        private static BitmapImage cartaCpu = new BitmapImage(new Uri("ms-appx:///Resources/retro carte pc.png"));
        private static Image i, i1;
        private static UInt16 secondi = 5;
        private static TimeSpan delay;
        private static bool avvisaTalloneFinito=true, briscolaDaPunti=false;
        elaboratoreCarteBriscola e;
        public MainPage()
        {
            this.InitializeComponent();
            delay = TimeSpan.FromSeconds(secondi);
            e = new elaboratoreCarteBriscola(briscolaDaPunti);
            m = new mazzo(e);
            carta.inizializza(40, cartaHelperBriscola.getIstanza(e));
            g = new giocatore(new giocatoreHelperUtente(), "Giulio", 3);
            cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), "Cpu", 3);
            primo = g;
            secondo = cpu;
            briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
            Image[] img = new Image[3];
            for (UInt16 i = 0; i < 3; i++)
            {
                g.addCarta(m);
                cpu.addCarta(m);

            }
            NomeUtente.Text = g.getNome();
            NomeCpu.Text = cpu.getNome();
            Utente0.Source = g.getImmagine(0);
            Utente1.Source = g.getImmagine(1);
            Utente2.Source = g.getImmagine(2);
            Cpu0.Source = cartaCpu;
            Cpu1.Source = cartaCpu;
            Cpu2.Source = cartaCpu;
            PuntiCpu.Text = $"{App.resourceMap.GetValue("PuntiDi", App.resourceContext).ValueAsString}  {cpu.getNome()}: {cpu.getPunteggio()}";
            PuntiUtente.Text = $"{App.resourceMap.GetValue("PuntiDi", App.resourceContext).ValueAsString}  {g.getNome()}: {g.getPunteggio()}";
            NelMazzoRimangono.Text = $"{App.resourceMap.GetValue("NelMazzoRimangono", App.resourceContext).ValueAsString} {m.getNumeroCarte()} {App.resourceMap.GetValue("carte", App.resourceContext).ValueAsString}";
            CartaBriscola.Text = $"{App.resourceMap.GetValue("SemeBriscola", App.resourceContext).ValueAsString}: {briscola.getSemeStr()}";
            cbCartaBriscola.Content = App.resourceMap.GetValue("CartaBriscolaDaPunti", App.resourceContext).ValueAsString;
            cbAvvisaTallone.Content = App.resourceMap.GetValue("AvvisaTallone", App.resourceContext).ValueAsString;
            opNomeUtente.Text = App.resourceMap.GetValue("NomeUtente", App.resourceContext).ValueAsString;
            opNomeCpu.Text = App.resourceMap.GetValue("NomeCpu", App.resourceContext).ValueAsString;
            Secondi.Text = App.resourceMap.GetValue("Secondi", App.resourceContext).ValueAsString;
            InfoApplicazione.Text = App.resourceMap.GetValue("Applicazione", App.resourceContext).ValueAsString;
            OpzioniApplicazione.Text = App.resourceMap.GetValue("Applicazione", App.resourceContext).ValueAsString;
            OpzioniInformazioni.Text = App.resourceMap.GetValue("Informazioni", App.resourceContext).ValueAsString;
            AppInformazioni.Text = App.resourceMap.GetValue("Informazioni", App.resourceContext).ValueAsString;
            AppOpzioni.Text = App.resourceMap.GetValue("Opzioni", App.resourceContext).ValueAsString;
            Briscola.Source = briscola.getImmagine();
        }
        private Image giocaUtente(Image img)
        {
            UInt16 quale = 0;
            Image img1 = Utente0;
            if (img == Utente1)
            {
                quale = 1;
                img1 = Utente1;
            }
            if (img == Utente2)
            {
                quale = 2;
                img1 = Utente2;
            }
            Giocata0.Visibility = Visibility.Visible;
            Giocata0.Source = img1.Source;
            img1.Visibility = Visibility.Collapsed;
            g.gioca(quale);
            return img1;
        }

        private void OnInfo_Click(object sender, TappedRoutedEventArgs e)
        {
            Applicazione.Visibility = Visibility.Collapsed;
            GOpzioni.Visibility = Visibility.Collapsed;
            Info.Visibility = Visibility.Visible;
        }

        private void OnApp_Click(object sender, TappedRoutedEventArgs e)
        {
            GOpzioni.Visibility = Visibility.Collapsed;
            Info.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Visible;
        }
        private void OnOpzioni_Click(object sender, TappedRoutedEventArgs e)
        {
            GOpzioni.Visibility = Visibility.Visible;
            Info.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Collapsed;
            txtNomeUtente.Text = g.getNome();
            txtCpu.Text = cpu.getNome();
            txtSecondi.Text = secondi.ToString();
            cbCartaBriscola.IsChecked = briscolaDaPunti;
            cbAvvisaTallone.IsChecked = avvisaTalloneFinito;
        }

        private void OnOkFp_Click(object sender, TappedRoutedEventArgs evt)
        {
            bool cartaBriscola = true;
            FinePartita.Visibility = Visibility.Collapsed;
            if (cbCartaBriscola.IsChecked == null || cbCartaBriscola.IsChecked == false)
                cartaBriscola = false;
            e = new elaboratoreCarteBriscola(cartaBriscola) ;
            m = new mazzo(e);
            briscola = carta.getCarta(elaboratoreCarteBriscola.getCartaBriscola());
            g = new giocatore(new giocatoreHelperUtente(), g.getNome(), 3);
            cpu = new giocatore(new giocatoreHelperCpu(elaboratoreCarteBriscola.getCartaBriscola()), cpu.getNome(), 3);
            for (UInt16 i = 0; i < 3; i++)
            {
                g.addCarta(m);
                cpu.addCarta(m);

            }
            Utente0.Source = g.getImmagine(0);
            Utente0.Visibility = Visibility.Visible;
            Utente1.Source = g.getImmagine(1);
            Utente1.Visibility = Visibility.Visible;
            Utente2.Source = g.getImmagine(2);
            Utente2.Visibility = Visibility.Visible;
            Cpu0.Source = cartaCpu;
            Cpu0.Visibility = Visibility.Visible;
            Cpu1.Source = cartaCpu;
            Cpu1.Visibility = Visibility.Visible;
            Cpu2.Source = cartaCpu;
            Cpu2.Visibility = Visibility.Visible;
            Giocata0.Visibility = Visibility.Collapsed;
            Giocata1.Visibility = Visibility.Collapsed;
            PuntiCpu.Text = $"{App.resourceMap.GetValue("PuntiDi", App.resourceContext).ValueAsString}  {cpu.getNome()}: {cpu.getPunteggio()}";
            PuntiUtente.Text = $"{App.resourceMap.GetValue("PuntiDi", App.resourceContext).ValueAsString}  {g.getNome()}: {g.getPunteggio()}";
            NelMazzoRimangono.Text = $"{App.resourceMap.GetValue("NelMazzoRimangono", App.resourceContext).ValueAsString} {m.getNumeroCarte()} {App.resourceMap.GetValue("carte", App.resourceContext).ValueAsString}";
            NelMazzoRimangono.Visibility = Visibility.Visible;
            CartaBriscola.Text = $"{App.resourceMap.GetValue("SemeBriscola", App.resourceContext).ValueAsString}: {briscola.getSemeStr()}";
            CartaBriscola.Visibility = Visibility.Visible;
            Briscola.Source = briscola.getImmagine();
            Briscola.Visibility = Visibility.Visible;
            primo = g;
            secondo = cpu;
            Briscola.Source = briscola.getImmagine();
            Applicazione.Visibility = Visibility.Visible;
        }
        private void OnCancelFp_Click(object sender, TappedRoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        private Image giocaCpu()
        {
            UInt16 quale = 0;
            Image img1 = Cpu0;
            if (primo == cpu)
                cpu.gioca(0);
            else
                cpu.gioca(0, g);
            quale = cpu.getICartaGiocata();
            if (quale == 1)
                img1 = Cpu1;
            if (quale == 2)
                img1 = Cpu2;
            Giocata1.Visibility = Visibility.Visible;
            Giocata1.Source = cpu.getCartaGiocata().getImmagine();
            img1.Visibility = Visibility.Collapsed;
            return img1;
        }
        private static bool aggiungiCarte()
        {
            try
            {
                primo.addCarta(m);
                secondo.addCarta(m);
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            return true;
        }

        private void Image_Tapped(object Sender, TappedRoutedEventArgs arg)
        {
            Image img = (Image)Sender;
            i = giocaUtente(img);
            if (secondo == cpu)
                i1 = giocaCpu();
            ThreadPoolTimer t = ThreadPoolTimer.CreateTimer((source) =>
            {

                IAsyncAction asyncAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {

                    c = primo.getCartaGiocata();
                    c1 = secondo.getCartaGiocata();
                    if ((c.CompareTo(c1) > 0 && c.stessoSeme(c1)) || (c1.stessoSeme(briscola) && !c.stessoSeme(briscola)))
                    {
                        temp = secondo;
                        secondo = primo;
                        primo = temp;
                    }

                    primo.aggiornaPunteggio(secondo);
                    PuntiCpu.Text = $"{App.resourceMap.GetValue("PuntiDi", App.resourceContext).ValueAsString}  {cpu.getNome()}: {cpu.getPunteggio()}";
                    PuntiUtente.Text = $"{App.resourceMap.GetValue("PuntiDi", App.resourceContext).ValueAsString}  {g.getNome()}: {g.getPunteggio()}";
                    if (aggiungiCarte())
                    {
                        NelMazzoRimangono.Text = $"{App.resourceMap.GetValue("NelMazzoRimangono", App.resourceContext).ValueAsString} {m.getNumeroCarte()} {App.resourceMap.GetValue("carte", App.resourceContext).ValueAsString}";
                        CartaBriscola.Text = $"{App.resourceMap.GetValue("SemeBriscola", App.resourceContext).ValueAsString}: {briscola.getSemeStr()}";
                        if (m.getNumeroCarte()==2 && avvisaTalloneFinito)
                            new ToastContentBuilder().AddArgument(App.resourceMap.GetValue("TalloneFinito", App.resourceContext).ValueAsString).AddText(App.resourceMap.GetValue("IlTalloneEFinito", App.resourceContext).ValueAsString).AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                        if (Briscola.Visibility == Visibility.Visible && m.getNumeroCarte() == 0)
                        {
                            NelMazzoRimangono.Visibility = Visibility.Collapsed;
                            Briscola.Visibility = Visibility.Collapsed;
                        }
                        Utente0.Source = g.getImmagine(0);
                        if (cpu.getNumeroCarte() > 1)
                            Utente1.Source = g.getImmagine(1);
                        if (cpu.getNumeroCarte() > 2)
                            Utente2.Source = g.getImmagine(2);
                        i.Visibility = Visibility.Visible;
                        i1.Visibility = Visibility.Visible;
                        Giocata0.Visibility = Visibility.Collapsed;
                        Giocata1.Visibility = Visibility.Collapsed;
                        if (cpu.getNumeroCarte() == 2)
                        {
                            Utente2.Visibility = Visibility.Collapsed;
                            Cpu2.Visibility = Visibility.Collapsed;
                        }
                        if (cpu.getNumeroCarte() == 1)
                        {
                            Utente1.Visibility = Visibility.Collapsed;
                            Cpu1.Visibility = Visibility.Collapsed;
                        }
                        if (primo == cpu)
                        {
                            i1 = giocaCpu();
                            if (cpu.getCartaGiocata().stessoSeme(briscola))
                                new ToastContentBuilder().AddArgument(App.resourceMap.GetValue("GiocataBriscola", App.resourceContext).ValueAsString).AddText($"{App.resourceMap.GetValue("LaCpuHaGiocatoIl", App.resourceContext).ValueAsString} {cpu.getCartaGiocata().getValore() + 1} {App.resourceMap.GetValue("diBriscola", App.resourceContext).ValueAsString}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                            else if (cpu.getCartaGiocata().getPunteggio() > 0)
                                new ToastContentBuilder().AddArgument(App.resourceMap.GetValue("GiocataCartaDiValore", App.resourceContext).ValueAsString).AddText($"{App.resourceMap.GetValue("LaCpuHaGiocatoIl", App.resourceContext).ValueAsString} {cpu.getCartaGiocata().getValore() + 1} {App.resourceMap.GetValue("di", App.resourceContext).ValueAsString} {cpu.getCartaGiocata().getSemeStr()}").AddAudio(new Uri("ms-winsoundevent:Notification.Reminder")).Show();
                        }

                    }
                    else
                    {
                        string s;
                        if (g.getPunteggio() == cpu.getPunteggio())
                            s = App.resourceMap.GetValue("PartitaPatta", App.resourceContext).ValueAsString;
                        else
                        {
                            if (g.getPunteggio() > cpu.getPunteggio())
                                s = App.resourceMap.GetValue("HaiVinto", App.resourceContext).ValueAsString;
                            else
                                s = App.resourceMap.GetValue("HaiPerso", App.resourceContext).ValueAsString;
                            s = $"{s} {App.resourceMap.GetValue("per", App.resourceContext).ValueAsString} {Math.Abs(g.getPunteggio() - cpu.getPunteggio())}  {App.resourceMap.GetValue("punti", App.resourceContext).ValueAsString}";
                        }
                        fpRisultrato.Text = $"{App.resourceMap.GetValue("PartitaFinita", App.resourceContext).ValueAsString}. {s} {App.resourceMap.GetValue("SecondaPartita", App.resourceContext).ValueAsString}";
                        Applicazione.Visibility = Visibility.Collapsed;
                        FinePartita.Visibility = Visibility.Visible;
                    }
                });
            }, delay);
        }
        public void OnOk_Click(Object source, RoutedEventArgs evt)
        {
            g.setNome(txtNomeUtente.Text);
            cpu.setNome(txtCpu.Text);
            if (cbCartaBriscola.IsChecked==null || cbCartaBriscola.IsChecked==false)
                briscolaDaPunti = false;
            else
                briscolaDaPunti = true;
            if (cbAvvisaTallone.IsChecked==null || cbAvvisaTallone.IsChecked==false)
                avvisaTalloneFinito = false;
            else
                avvisaTalloneFinito = true;
            try
            {
                secondi = UInt16.Parse(txtSecondi.Text);
            } catch (FormatException ex)
            {
                txtSecondi.Text = App.resourceMap.GetValue("ValoreNonValido", App.resourceContext).ValueAsString; ;
                return;
            }
            delay = TimeSpan.FromSeconds(secondi);
            NomeUtente.Text = g.getNome();
            NomeCpu.Text = cpu.getNome();
            GOpzioni.Visibility = Visibility.Collapsed;
            Applicazione.Visibility = Visibility.Visible;
        }

        private async void OnFPShare_Click(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"https://twitter.com/intent/tweet?text=Con%20la%20CBriscola%20la%20partita%20{g.getNome()}%20contro%20{cpu.getNome()}%20%C3%A8%20finita%20{g.getPunteggio()}%20a%20{cpu.getPunteggio()}&url=https%3A%2F%2Fgithub.com%2Fnumerunix%2Fcbriscolauwp"));
        }


        private async void OnSito_Click(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/numerunix/cbriscolauwp"));
        }
    }
}
