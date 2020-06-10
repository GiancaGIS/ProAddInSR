using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
using Button = ArcGIS.Desktop.Framework.Contracts.Button;

namespace ProAddInSR
{
    internal class bottoneProiezioniAlVolo : Button
    {
        protected async override void OnClick()
        {

            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                ArcGIS.Desktop.Mapping.Map mappa = ProAddInSR.funzioniVariabiliGlobali.FunzioniGlobali.RicavaMappaAttiva();
                if (mappa is null)
                    return;

                CIMMap cIMMap = funzioniVariabiliGlobali.FunzioniGlobali.RicavaInfoMappaCIMMapClass(mappa).Result;

                CIMDatumTransform[] trasfDatum = cIMMap.DatumTransforms; // DatumTransform fornisce un array fornisce
                                                                         // una matrice unidimensionale di oggetti appartenenti alla classe CIMDatumTransform.

                string elencoProiezioniAlVolo = String.Empty;

                if (trasfDatum != null)
                {
                    for (int i = 0; i < trasfDatum.LongCount<CIMDatumTransform>(); i++)
                    {
                        CIMDatumTransform cIMDatumTransform = new CIMDatumTransform(); // Istanzio la CoClass
                        cIMDatumTransform = trasfDatum.ElementAt(i); // Ricavo quell'elemento alla tal posizione nella matrice
                                                                     // Ricavo l'oggetto GeographicTransformation, che dovrebbe essere un Abtract Class.
                        GeographicTransformation geographicTransformation = cIMDatumTransform.GeoTransformation as GeographicTransformation;
                        elencoProiezioniAlVolo = geographicTransformation.Name + Environment.NewLine;
                    }

                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(elencoProiezioniAlVolo, String.Format("Elenco proiezioni al volo della mappa: '{0}'", mappa.Name), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                else
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(String.Format("Non ci sono proiezioni al volo attive per la mappa: '{0}'", mappa.Name), "Attenzione", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }
            });
        }
    }
}
