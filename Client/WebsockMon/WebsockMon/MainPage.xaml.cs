using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Websock.Model;

namespace WebsockMon
{
    public partial class MainPage : ContentPage
    {
        private MessageModelView _msgModelView = new MessageModelView();

        public MainPage()
        {
            InitializeComponent();

            LabelRefreshSessions.Tapped += LabelRefreshSessions_TappedAsync;

            ButtonSessionDeactivate.Clicked += ButtonSessionDeactivate_ClickedAsync;
            ButtonSessionDelete.Clicked += ButtonSessionDelete_ClickedAsync;
            ButtonSessionMessages.Clicked += ButtonSessionMessages_ClickedAsync;

            SessionList.ItemSelected += SessionList_ItemSelected;

            BindingContext = _msgModelView;
        }

        private void SessionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ButtonSessionMessages.IsEnabled = ButtonSessionDelete.IsEnabled =
                ButtonSessionDeactivate.IsEnabled = _msgModelView.EditSession != null;
        }

        async private void ButtonSessionDeactivate_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                _msgModelView.EditSession.Active = false;
                await _msgModelView.UpdateSessionAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("WebSock", ex.Message, "OK");
            }
        }

        async private void LabelRefreshSessions_TappedAsync(object sender, EventArgs e)
        {
            try
            {
                await _msgModelView.GetSessionsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("WebSock", ex.Message, "OK");
            }
        }

        async private void ButtonSessionDelete_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                await _msgModelView.DeleteSessionAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("WebSock", ex.Message, "OK");
            }
        }

        async private void ButtonSessionMessages_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                await _msgModelView.GetSessionMessagesAsync();
                await Navigation.PushModalAsync(new MessagesPage(_msgModelView));
            }
            catch (Exception ex)
            {
                await DisplayAlert("WebSock", ex.Message, "OK");
            }
        }
    }
}
