using Ex4.ViewModels;
using Storm.Mvvm.Forms;
using Xamarin.Forms.Xaml;

namespace Ex4.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddComment : BaseContentPage
	{
		public AddComment ()
		{
			InitializeComponent ();
            BindingContext = new AddCommentViewModel();
        }
    }
}