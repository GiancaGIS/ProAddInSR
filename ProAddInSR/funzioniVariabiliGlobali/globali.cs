using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;


namespace ProAddInSR.funzioniVariabiliGlobali
{

    static class VariabiliGlobali
    {
        public static ArcGIS.Desktop.Mapping.Map mappa = null;
        public static bool blnMappaAttivaCaricata = false;
    }
    
    class FunzioniGlobali
    {
        public static async Task<Map> FindOpenExistingMapAsync(string mapName)
        {
            return await QueuedTask.Run(async () =>
            {
                Map map = null;
                Project proj = Project.Current;

                //Finding the first project item with name matches with mapName
                MapProjectItem mpi =
                    proj.GetItems<MapProjectItem>()
                        .FirstOrDefault(m => m.Name.Equals(mapName, StringComparison.CurrentCultureIgnoreCase));
                if (mpi != null)
                {
                    map = mpi.GetMap();
                    //Opening the map in a mapview
                    await ProApp.Panes.CreateMapPaneAsync(map);
                }
                return map;
            });
        }

        /// <summary>
        /// Funzione che restituisce la Mappa attiva nel progetto corrente di ArcGIS Pro
        /// </summary>
        /// <returns></returns>
        public static Map RicavaMappaAttiva()
        {
            var mapView = MapView.Active;
            if (mapView == null)
            {
                return null;
            }
            else
            {
                return mapView.Map;
            }
        }

        public static async Task<SpatialReference> RicavaSRLayer(Layer layer)
        {
            return await QueuedTask.Run(() =>
            {
                SpatialReference spatialReference = layer.GetSpatialReference();
                return spatialReference;
            });
        }

        public static Task<CIMMap> RicavaInfoMappaCIMMapClass(Map mappa)
        {
            return QueuedTask.Run(() =>
            {
                return mappa.GetDefinition();
            });
        }

        public static async Task<IReadOnlyList<Map>> RicavaMapsFromMapPanesAsync()
        {
            return await QueuedTask.Run(() =>
            {
                //Gets the unique list of Maps from all the MapPanes.
                //Note: The list of maps retrieved from the MapPanes
                //maybe less than the total number of Maps in the project.
                //It depends on what maps the user has actually opened.
                var mapPanes = ProApp.Panes.OfType<IMapPane>()
                            .GroupBy((mp) => mp.MapView.Map.URI).Select(grp => grp.FirstOrDefault());

                List<Map> uniqueMaps = new List<Map>();

                foreach (var pane in mapPanes)
                    uniqueMaps.Add(pane.MapView.Map);

                return uniqueMaps;
            });
        }

        public static int NumeroMappeProgetto()
        {
            return RicavaMapsFromMapPanesAsync().Result.Count;
        }
    }
}