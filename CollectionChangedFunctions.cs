using System.Linq;
using System.Windows;
using System.Reactive.Linq;
using System.Collections.Specialized;

namespace client
{
    public partial class MainWindow : Window
    {
        private void BS_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    BaseStation newBS = e.NewItems[0] as BaseStation;
                    log.Add(new LogUnit("Добавлена", newBS));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    BaseStation oldBS = e.OldItems[0] as BaseStation;
                    log.Add(new LogUnit("Удалена", oldBS));
                    var subsForRemove = subs.Where(s => s.bsName == oldBS.name).ToList();
                    foreach (Subscriber sub in subsForRemove)
                    {
                        subs.Remove(sub);
                    }
                    break;
            }
        }
        private void Subs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Subscriber newSub = e.NewItems[0] as Subscriber;
                    Subscriber s = new Subscriber(newSub.imsi, newSub.imeiSV, newSub.assistData);
                    //RenewAssistDataForSub(s);
                    log.Add(new LogUnit("Добавлен", newSub));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Subscriber oldSub = e.OldItems[0] as Subscriber;
                    log.Add(new LogUnit("Удален", oldSub));
                    break;
            }
        }
        private void Geos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Geolocation newGeo = e.NewItems[0] as Geolocation;
                    log.Add(new LogUnit(newGeo));
                    break;
            }
        }
        private void TAs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TA newTA = e.NewItems[0] as TA;
                    log.Add(new LogUnit(newTA));
                    break;
            }
        }
        private void CellIDs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CellID newCellID = e.NewItems[0] as CellID;
                    log.Add(new LogUnit(newCellID));
                    break;
            }
        }
        private void Log_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    LogUnit newUser = e.NewItems[0] as LogUnit;
                    break;
            }
        }
    }
}