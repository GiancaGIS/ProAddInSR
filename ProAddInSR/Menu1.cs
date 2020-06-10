using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace ProAddInSR
{
    internal class Menu1_button1 : Button
    {
        protected override void OnClick()
        {
            ArcGIS.Desktop.Mapping.Map mappa = ProAddInSR.funzioniVariabiliGlobali.FunzioniGlobali.RicavaMappaAttiva();
            if (mappa is null)
                return;

            SpatialReference spatialReferenceMappaAttiva = mappa.SpatialReference;

            int EPSG = spatialReferenceMappaAttiva.LatestWkid;

            string strInfoBasiche = String.Empty;

            string strTipologiaSR = String.Empty;

            if (spatialReferenceMappaAttiva.IsProjected)
                strTipologiaSR = "Proiettato";

            else if (spatialReferenceMappaAttiva.IsGeographic)
                strTipologiaSR = "Geografico";

            strInfoBasiche = String.Format(
                "La mappa attiva ha un Sistema di Riferimento avente le seguenti caratteristiche:\n\nNome: '{0}'.\nTipologia: {1}.\nUnita' di misura: {2}.", spatialReferenceMappaAttiva.Name, strTipologiaSR, spatialReferenceMappaAttiva.Unit);
            //strInfoBasiche = strInfoBasiche +
            //    String.Format("\n\nTipologia di Proiezione Cartografica e relativo codice EPSG: '{0}', {1}", spatialReferenceMappaAttiva.  .projectionName, spatialReferenceMappaAttiva.LatestWkid) + "\n\nPremere 'Retry / Riprova' per ricercare il SR su 'www.epsg.io', altrimenti premere 'Cancel / Annulla'.";
            strInfoBasiche = strInfoBasiche + "\n\nPremere 'Yes' per ricercare il SR su 'www.epsg.io', altrimenti premere 'No'.";

            var obj = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(strInfoBasiche, "Info Basiche sul SR dell'active Map", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Information);

            if (obj.ToString().ToUpper() == "YES")
            {
                System.Diagnostics.Process.Start(String.Format("https://epsg.io/{0}", EPSG));
            }
        }
    }

    internal class Menu1_button2 : Button
    {
        protected override void OnClick()
        {
            ArcGIS.Desktop.Mapping.Map mappa = ProAddInSR.funzioniVariabiliGlobali.FunzioniGlobali.RicavaMappaAttiva();
            if (mappa is null)
                return;

            SpatialReference spatialReferenceMappaAttiva = mappa.SpatialReference;
            int EPSG = spatialReferenceMappaAttiva.LatestWkid;

            string strInfoAvanzate = String.Empty;

            string strTipologiaSR = String.Empty;

            if (spatialReferenceMappaAttiva.IsProjected)
                strTipologiaSR = "Proiettato";

            else if (spatialReferenceMappaAttiva.IsGeographic)
                strTipologiaSR = "Geografico";

            strInfoAvanzate = String.Format(
                "Il Dataframe attivo ha un Sistema di Riferimento avente le seguenti caratteristiche:\n\nNome: '{0}'.\nTipologia: {1}.\nCodice EPSG: {2}.\nUnita' lineare: {3}.\nNome e codice EPSG del DATUM: '{4}', {5}.\nNome e codice EPSG dell'ellissoide: '{6}', {7}.\nRisoluzione XY: {8}\nTolleranza XY: {9}\nRisoluzione Asse Z: {10}\nTolleranza Asse Z: {11}\nFattore di scala: {12}",
                spatialReferenceMappaAttiva.Name, strTipologiaSR, EPSG, spatialReferenceMappaAttiva.Unit,
                spatialReferenceMappaAttiva.Gcs.Datum.Name, spatialReferenceMappaAttiva.Gcs.Datum.SpheroidWkid,
                spatialReferenceMappaAttiva.Gcs.Datum.SpheroidName, spatialReferenceMappaAttiva.Gcs.Datum.SpheroidWkid,
                spatialReferenceMappaAttiva.XYResolution, spatialReferenceMappaAttiva.XYTolerance,
                spatialReferenceMappaAttiva.ZUnit, spatialReferenceMappaAttiva.ZTolerance, spatialReferenceMappaAttiva.XYScale);

            strInfoAvanzate = strInfoAvanzate + "\n\nPremere 'Yes' per ricercare il SR su 'www.epsg.io', altrimenti premere 'No'.";

            var obj = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(strInfoAvanzate, "Info Avanzate sul SR dell'active Map", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Information);

            if (obj.ToString().ToUpper() == "YES")
            {
                System.Diagnostics.Process.Start(String.Format("https://epsg.io/{0}", EPSG));
            }
        }
    }
}
