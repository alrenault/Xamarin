using Ex4.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ex4.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailPlace : TabbedPage
    {
		public DetailPlace ()
		{
			InitializeComponent ();
            BindingContext = new DetailPlaceViewModel();
		}
	}
}