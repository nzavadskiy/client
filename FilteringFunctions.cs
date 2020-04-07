using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace client
{
    public partial class MainWindow : Window
    {
        private bool BSFilter(object item)
        {
            if (String.IsNullOrEmpty(tbSortFieldBS.Text))
                return true;
            else
                switch (cbFilterFieldsBS.SelectedIndex)
                {
                    case 0: //name
                        return ((item as BaseStation).name.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 1: //mcc
                        return ((item as BaseStation).mcc.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 2: //mnc
                        return ((item as BaseStation).mnc.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 3: //lac
                        return ((item as BaseStation).lac.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 4: //cell id
                        return ((item as BaseStation).cellid.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 5: //antenna
                        return ((item as BaseStation).antenna.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 6: //lat
                        return ((item as BaseStation).lat.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 7: //lon
                        return ((item as BaseStation).lon.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 8: //alt
                        return ((item as BaseStation).alt.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 9: //accuracy
                        return ((item as BaseStation).accuracy.IndexOf(tbSortFieldBS.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    default:
                        return false;
                }
        }
        private bool SubFilter(object item)
        {
            if (String.IsNullOrEmpty(tbSortFieldSub.Text))
                return true;
            else
                switch (cbFilterFieldsSub.SelectedIndex)
                {
                    case 0: //imsi
                        return ((item as Subscriber).imsi.IndexOf(tbSortFieldSub.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 1: //imeisv
                        return ((item as Subscriber).imeiSV.IndexOf(tbSortFieldSub.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 2: //sub name
                        return ((item as Subscriber).subName.IndexOf(tbSortFieldSub.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 3: //bs name
                        return ((item as Subscriber).bsName.IndexOf(tbSortFieldSub.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 4: //methods
                        return ((item as Subscriber).assistData.IndexOf(tbSortFieldSub.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    default:
                        return false;
                }
        }
        private bool GeoFilter(object item)
        {
            if (String.IsNullOrEmpty(tbSortFieldGeo.Text))
                return true;
            else
                switch (cbFilterFieldsGeo.SelectedIndex)
                {
                    case 0: //imsi
                        return ((item as Geolocation).imsi.IndexOf(tbSortFieldGeo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 1: //imeisv
                        return ((item as Geolocation).imeiSV.IndexOf(tbSortFieldGeo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 2: //lat
                        return ((item as Geolocation).latitude.ToString().IndexOf(tbSortFieldGeo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 3: //lon
                        return ((item as Geolocation).longtitude.ToString().IndexOf(tbSortFieldGeo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 4: //date
                        return ((item as Geolocation).date.IndexOf(tbSortFieldGeo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    default:
                        return false;
                }
        }
        private bool TAFilter(object item)
        {
            if (String.IsNullOrEmpty(tbSortFieldTA.Text))
                return true;
            else
                switch (cbFilterFieldsTA.SelectedIndex)
                {
                    case 0: //imsi
                        return ((item as TA).imsi.IndexOf(tbSortFieldTA.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 1: //imeisv
                        return ((item as TA).imeiSV.IndexOf(tbSortFieldTA.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 2: //lata
                        return ((item as TA).lat.IndexOf(tbSortFieldTA.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 3: //lon
                        return ((item as TA).lon.IndexOf(tbSortFieldTA.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 4: //date
                        return ((item as TA).date.IndexOf(tbSortFieldTA.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    default:
                        return false;
                }
        }
        private bool CellIDFilter(object item)
        {
            if (String.IsNullOrEmpty(tbSortFieldCellID.Text))
                return true;
            else
                switch (cbFilterFieldsCellID.SelectedIndex)
                {
                    case 0: //imsi
                        return ((item as CellID).imsi.IndexOf(tbSortFieldCellID.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 1: //imeisv
                        return ((item as CellID).imeiSV.IndexOf(tbSortFieldCellID.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 2: //bs name
                        return ((item as CellID).bsName.IndexOf(tbSortFieldCellID.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 3: //lat
                        return ((item as CellID).lat.IndexOf(tbSortFieldCellID.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 4: //lon
                        return ((item as CellID).lon.IndexOf(tbSortFieldCellID.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    case 5: //dist
                        return ((item as CellID).dist.IndexOf(tbSortFieldCellID.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                    default:
                        return false;
                }
        }
        private void tbSortFieldBS_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvBaseStations.ItemsSource).Refresh();
        }

        private void tbSortFieldSub_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvSubscribers.ItemsSource).Refresh();
        }

        private void tbSortFieldGeo_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvGeolocation.ItemsSource).Refresh();
        }

        private void tbSortFieldTA_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvTA.ItemsSource).Refresh();
        }

        private void tbSortFieldCellID_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvCellID.ItemsSource).Refresh();
        }
    }
}