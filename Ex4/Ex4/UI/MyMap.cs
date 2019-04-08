using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Ex4.UI
{
	public class MyMap : Map {

        public Position MapPosition{
            get => (Position)GetValue(MapPositionProperty);
            set => SetValue(MapPositionProperty, value);
        }

        public ObservableCollection<Pin> MapPins
        {
            get => (ObservableCollection<Pin>)GetValue(MapPinsProperty);
            set => SetValue(MapPinsProperty, value);
        }

        public static readonly BindableProperty MapPositionProperty = BindableProperty.Create(
                   nameof(MapPosition),
                   typeof(Position),
                   typeof(MyMap),
                   new Position(0, 0),
                   propertyChanged: (b, o, n) => {
                       ((MyMap)b).MoveToRegion(MapSpan.FromCenterAndRadius(
                           (Position)n,
                           Distance.FromMiles(1)));
                   });


        public static readonly BindableProperty MapPinsProperty = BindableProperty.Create(
                    nameof(MapPins),
                    typeof(ObservableCollection<Pin>),
                    typeof(MyMap),
                    new ObservableCollection<Pin>(),
                    propertyChanged: (b, o, n) =>{
                        var bindable = (MyMap)b;
                        bindable.Pins.Clear();

                        var collection = (ObservableCollection<Pin>)n;
                        if (collection == null) return;
                        foreach (var item in collection)
                            bindable.Pins.Add(item);
                        collection.CollectionChanged += (sender, e) =>{
                            Device.BeginInvokeOnMainThread(() =>{
                                switch (e.Action){

                                    case NotifyCollectionChangedAction.Add:

                                    case NotifyCollectionChangedAction.Replace:

                                    case NotifyCollectionChangedAction.Remove:
                                        if (e.OldItems != null)
                                            foreach (var item in e.OldItems)
                                                bindable.Pins.Remove((Pin)item);
                                        if (e.NewItems != null)
                                            foreach (var item in e.NewItems)
                                                bindable.Pins.Add((Pin)item);
                                        break;

                                    case NotifyCollectionChangedAction.Reset:
                                        bindable.Pins.Clear();
                                        break;
                                }
                            });
                        };
                    });
    }
}