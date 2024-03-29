/*
  *  This code is distribuited under GPL 3.0 or, at your opinion, any later version
 *  CBriscola 1.1.3
 *
 *  Created by Giulio Sorrentino (numerone) on 29/01/23.
 *  Copyright 2023 Some rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

namespace org.altervista.numerone.framework
{
  public  class Giocatore
	{
		private string nome;
		private Carta[] mano;
		private bool ordinaMano;
		private UInt16 numeroCarte, iCarta, iCartaGiocata, punteggio;
		private UInt16 dimensioneMano;
		private GiocatoreHelper helper;
		private List<UInt16> punteggi;
		public enum Carta_GIOCATA { NESSUNA_Carta_GIOCATA = UInt16.MaxValue };
		public Giocatore(GiocatoreHelper h, string n, UInt16 carte, bool ordina = true)
		{
			dimensioneMano = carte;
			ordinaMano = ordina;
			numeroCarte = dimensioneMano;
			iCartaGiocata = (UInt16)(Carta_GIOCATA.NESSUNA_Carta_GIOCATA);
			punteggio = 0;
			helper = h;
			nome = n;
			mano = new Carta[dimensioneMano];
			iCarta = 0;
			punteggi = new List<UInt16>();
		}
		public string GetNome() { return nome; }
		public void SetNome(string n) { nome = n; }
		public bool GetFlagOrdina() { return ordinaMano; }
		public void SetFlagOrdina(bool ordina) { ordinaMano = ordina; }
		public void AddCarta(Mazzo m)
		{
			UInt16 i = 0;
			Carta temp;
			if (iCarta == numeroCarte && iCartaGiocata == (UInt16)Carta_GIOCATA.NESSUNA_Carta_GIOCATA)
				throw new ArgumentException($"Chiamato Giocatore::setCarta con mano.size()==numeroCarte=={numeroCarte}");
			if (iCartaGiocata != (UInt16)Carta_GIOCATA.NESSUNA_Carta_GIOCATA)
			{
				for (i = iCartaGiocata; i < numeroCarte - 1; i++)
					mano[i] = mano[i + 1];
				mano[i] = null;
				iCartaGiocata = (UInt16)Carta_GIOCATA.NESSUNA_Carta_GIOCATA;
				mano[iCarta - 1] = SostituisciCartaGiocata(m);
				for (i = (UInt16)(iCarta - 2); i < UInt16.MaxValue && iCarta > 1 && mano[i].CompareTo(mano[i + 1]) < 0; i--)
				{
					temp = mano[i];
					mano[i] = mano[i+1];
					mano[i+1] = temp;
				}
				return;
			}
			Ordina(m);


		}

		private void Ordina(Mazzo m)
		{
			UInt16 i = 0;
			UInt16 j = 0;
			Carta c = SostituisciCartaGiocata(m);
			for (i = 0; i < iCarta && mano[i] != null && c.CompareTo(mano[i]) < 0; i++) ;
			for (j = (UInt16)(numeroCarte - 1); j > i; j--)
				mano[j] = mano[j - 1];
			mano[i] = c;
			iCarta++;
		}
		private Carta SostituisciCartaGiocata(Mazzo m)
		{
			Carta c;
			try
			{
				c = Carta.GetCarta(m.GetCarta());
			}
			catch (IndexOutOfRangeException e)
			{
				numeroCarte--;
				iCarta--;
				if (numeroCarte == 0)
					throw e;
				return mano[numeroCarte];
			}
			return c;
		}
		public Carta GetCartaGiocata()
		{
			return mano[iCartaGiocata];
		}
		public UInt16 GetPunteggio() { return punteggio; }
		public void Gioca(UInt16 i)
		{
			iCartaGiocata = helper.Gioca(i, mano, numeroCarte);
		}
		public void Gioca(UInt16 i, Giocatore g1)
		{
			iCartaGiocata = helper.Gioca(i, mano, numeroCarte, g1.GetCartaGiocata());
		}
		public void AggiornaPunteggio(Giocatore g)
		{
			helper.AggiornaPunteggio(ref punteggio, GetCartaGiocata(), g.GetCartaGiocata());
		}

        public BitmapImage GetImmagine(UInt16 quale)
        {
            return mano[quale].GetImmagine();
        }


        public UInt16 GetICartaGiocata()
		{
			return iCartaGiocata;
		}

		public UInt16 GetNumeroCarte()
		{
			return numeroCarte;
		}

		public void Resetta(GiocatoreHelper h=null, bool resettaPunteggi=true)
		{
			numeroCarte = dimensioneMano;
			if (h != null)
				helper = h;
			iCartaGiocata = (UInt16)(Carta_GIOCATA.NESSUNA_Carta_GIOCATA);
			iCarta = 0;
			for (UInt16 i = 0; i < dimensioneMano; i++)
				mano[i] = null;
			if (resettaPunteggi)
				punteggi.Clear();
			else if (GetPunteggio() > 0)
				punteggi.Add(GetPunteggio());
            punteggio = 0;
        }

        public UInt64 GetPunteggi()
		{
			UInt64 p=0;
			foreach (UInt16 i in punteggi)
				p += i;
			return p;
		}
	}

}