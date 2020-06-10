using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using ArcGIS.Desktop.Mapping.Events;


namespace ProAddInSR
{
    /// <summary>
    /// Represents the ComboBox
    /// </summary>
    internal class ComboBoxFeatureLayer : ComboBox
    {
        Map mappa = null;
        public List<FeatureLayer> featureLayerList = null;

        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public ComboBoxFeatureLayer()
        {
            mappa = ProAddInSR.funzioniVariabiliGlobali.FunzioniGlobali.RicavaMappaAttiva();
            UpdateCombo();
            
            MapViewInitializedEvent.Subscribe(OnMapViewCaricata); // Occurs when the map view has initialized --> Cioè aperta nuova Mappa, e caricata da zero!!
            LayersAddedEvent.Subscribe(EventoLayerInTOC);
            LayersMovedEvent.Subscribe(EventoLayerInTOC);
            LayersRemovedEvent.Subscribe(EventoLayerInTOC);
            MapClosedEvent.Subscribe(AllaChiusuraMappa);
            MapPropertyChangedEvent.Subscribe(AllaVariazioneProprietaMappa); // Occurs when any property of a map is changed.
            MapMemberPropertiesChangedEvent.Subscribe(EventoLayerInTOC); // Occurs when any property of layer or standalone table changed.
        }

        /// <summary>
        /// Funzione da richiamare allo scattare dell'evento 'Chiusura Mappa'.
        /// </summary>
        /// <param name="args"></param>
        private void AllaChiusuraMappa(MapClosedEventArgs args)
        {
            this.Clear();
            this.UpdateCombo();
            LayersAddedEvent.Unsubscribe(EventoLayerInTOC);
            LayersMovedEvent.Unsubscribe(EventoLayerInTOC);
            LayersRemovedEvent.Unsubscribe(EventoLayerInTOC);
            //MapPropertyChangedEvent.Unsubscribe(AllaVariazioneProprietaMappa); // Occurs when any property of a map is changed.
            MapMemberPropertiesChangedEvent.Unsubscribe(EventoLayerInTOC);
        }

        private void AllaVariazioneProprietaMappa(MapPropertyChangedEventArgs args)
        {
            try
            {
                this.Clear();
                this.UpdateCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore");
            }
        }


        /// <summary>
        /// Updates the combo box with all the items.
        /// </summary>
        public void UpdateCombo()
        {
            try
            {
                Add(new ComboBoxItem("")); // Attenzione! Importante che il primo oggetto nel ComboBox sia stringa vuota!
                                           // In quanto se scatta l'evento OnSelectioChange, scatta sul primo oggetto nella lista! Lo metto nullo apposta!

                if (mappa is null)
                    return;

                featureLayerList = mappa.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();

                int numLayerInMappa = featureLayerList.Count;

                for (int i = 0; i < numLayerInMappa; i++)
                {
                    string name = featureLayerList[i].Name;
                    Add(new ComboBoxItem(name));
                }

                Enabled = true; //enables the ComboBox
                SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Funzione che richiama a sua volta l'aggiornatore Combobox
        /// Da richiamare allo scattare di un evento che riguarda i layer in TOC
        /// </summary>
        /// <param name="args"></param>
        private void EventoLayerInTOC(LayerEventsArgs args)
        {
            this.Clear();
            this.UpdateCombo();
        }

        private void EventoLayerInTOC(ActiveMapViewChangedEventArgs args)
        {
            this.Clear();
            this.UpdateCombo();
        }

        private void EventoLayerInTOC(MapMemberPropertiesChangedEventArgs args)
        {
            this.Clear();
            this.UpdateCombo();
        }

        private void OnMapViewCaricata(MapViewEventArgs args)
        {
            funzioniVariabiliGlobali.VariabiliGlobali.blnMappaAttivaCaricata = true;
            this.Clear();
            this.UpdateCombo();

            LayersAddedEvent.Subscribe(EventoLayerInTOC);
            LayersMovedEvent.Subscribe(EventoLayerInTOC);
            LayersRemovedEvent.Subscribe(EventoLayerInTOC);
            MapClosedEvent.Subscribe(AllaChiusuraMappa);
            MapPropertyChangedEvent.Subscribe(AllaVariazioneProprietaMappa); // Occurs when any property of a map is changed.
            MapMemberPropertiesChangedEvent.Subscribe(EventoLayerInTOC); // Occurs when any property of layer or standalone table changed.

        }

        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override void OnSelectionChange(ComboBoxItem item)
        {
            if (item is null)
                return;

            if (string.IsNullOrEmpty(item.Text))
                return;

            string NomefLayerSelez = item.Text;

            Layer layerSelez = null;

            for (int i = 0; i < featureLayerList.Count; i++)
            {
                if (featureLayerList[i].Name.ToUpper() == item.Text.ToUpper())
                {
                    layerSelez = featureLayerList[i];
                    break;
                }
            }
            string infoSRLayer = String.Empty;
            string sceltaUtente = String.Empty;

            SpatialReference spatialReference = null;

            //spatialReference = funzioniVariabiliGlobali.FunzioniGlobali.RicavaSRLayer(layerSelez);

            QueuedTask.Run(() =>
            {
                spatialReference = layerSelez.GetSpatialReference();

                string strTipologiaSR = String.Empty;

                if (spatialReference.IsProjected)
                    strTipologiaSR = "Proiettato";

                else if (spatialReference.IsGeographic)
                    strTipologiaSR = "Geografico";

                infoSRLayer =
                String.Format("Il Layer '{2}' selezionato ha un Sistema di Riferimento avente le seguenti caratteristiche:\n\nNome: {0}.\nTipologia: {1}.",
                spatialReference.Name, strTipologiaSR, item.Text);

                infoSRLayer += "\n\nPremere 'Yes' per ricercare il SR su 'www.epsg.io', altrimenti premere 'No'.";

                sceltaUtente = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(infoSRLayer, String.Format("Info Basiche sul SR del Feature Layer: {0}", item.Text), System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Information).ToString();

                if (sceltaUtente.ToString().ToUpper() == "YES")
                {
                    System.Diagnostics.Process.Start(String.Format("https://epsg.io/{0}", spatialReference.LatestWkid));
                }
            });
        }

        //protected override void OnUpdate()
        //{

        //}

    }
}
