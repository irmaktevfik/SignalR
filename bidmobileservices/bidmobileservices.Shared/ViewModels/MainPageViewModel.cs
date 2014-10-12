using bidmobileservices;
using bidmobileservicesModels;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using ZUMOAPPNAME.Common;
using ZUMOAPPNAME.Helpers;

namespace ZUMOAPPNAME.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        const string _mobileServiceUrl = "https://yoururl.azure-mobile.net/";
        private MobileServiceUser _user;
        private HubConnection _hubConnection;
        private IHubProxy _proxy;
        private CoreDispatcher _dispatcher;

        #region change handling
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            //Fire the PropertyChanged event in case somebody subscribed to it
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// Observable collection for the Page collection data
        /// </summary>
        private ObservableCollection<BidItems> _bidData;
        public ObservableCollection<BidItems> BidData
        {
            get
            {
                return this._bidData;
            }
        }

        /// <summary>
        /// will hold the selected item from listview
        /// </summary>
        private BidItems _selectedBidItem;
        public BidItems SelectedBidItem
        {
            get
            {
                return this._selectedBidItem;
            }
            set
            {
                this._selectedBidItem = value;
                this.OnPropertyChanged("SelectedBidItem");
            }

        }

        #region Commands
        private RelayCommand _processBid;
        public RelayCommand ProcessBid
        {
            get
            {
                if (_processBid == null)
                {
                    _processBid = new RelayCommand(() => this.InvokeBid(), () => this.CanBid());
                }
                return _processBid;
            }
            set
            {
                _processBid = value;
            }
        }

        private bool CanBid()
        {
            //ok, it is not a good logic to set it as true. Being lazy
            //best way is to have have status depending on the connection state. 
            //if connection is lost, user should not be able to process bid.
            return true;
        }

        private async void InvokeBid()
        {
            //invokes the PushBid method on our hub with the selected item Id
            if (SelectedBidItem != null)
            await _proxy.Invoke("PushBid", SelectedBidItem.Id.ToString());
        }
        #endregion


        #region ctor
        public MainPageViewModel(CoreDispatcher dispatcher)
        {
            _bidData = new ObservableCollection<BidItems>();
            _dispatcher = dispatcher;
            // will keep the connection live. 
            ConnectToHub().Forget();
        }

        #endregion

        private async Task ConnectToHub()
        {
            _hubConnection = new HubConnection(_mobileServiceUrl);

            if (_user != null)
            {
                _hubConnection.Headers["x-zumo-auth"] = _user.MobileServiceAuthenticationToken;
            }
            else
            {
                //either way, we need to add x-zumo-application header to authenticate on the service
                _hubConnection.Headers["x-zumo-application"] = App.MobileService.ApplicationKey;
            }

            //create Hub proxy as named and start connection
            _proxy = _hubConnection.CreateHubProxy("BidHub");
            await _hubConnection.Start();

            //will be triggered once " Clients.All.Message(jsonData);" has been triggered from Hub
            _proxy.On<string>("message", async msg =>
            {
                var fullBidItems = Helpers.JsonHelper.Deserialize<List<BidItems>>(msg);
                await this._dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    _bidData = fullBidItems.ToObservableCollection();
                    this.OnPropertyChanged("BidData");
                });
            });


            while (_hubConnection.State != ConnectionState.Connected)
            {
                //put a delay and retry
                await Task.Delay(3000);
                ConnectToHub();
            }

            // get items
            string result = await _proxy.Invoke<string>("GetBids","also we can give some parameters to methods");
            var fullData = Helpers.JsonHelper.Deserialize<List<BidItems>>(result);
            await this._dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                _bidData = fullData.ToObservableCollection();
                this.OnPropertyChanged("BidData");
            });

            //need to add these lines. I have included the source at the end of the article.
            //explained by a team member in Channel9
            while (true)
            {
                try
                {
                    // HTTP streaming transports don't work well on the Windows Phone stack so we force long polling.
                    // support for WebSockets to Windows Store/Phone 8.1 apps in SignalR 2.2.0
                    await _hubConnection.Start(new LongPollingTransport());
                    if (_hubConnection.State == ConnectionState.Connected)
                        break;
                }
                catch (Exception)
                {

                }
                await Task.Delay(3000);
            }

        }


    }
}
