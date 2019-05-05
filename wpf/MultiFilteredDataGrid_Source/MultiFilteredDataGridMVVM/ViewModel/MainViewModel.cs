using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiFilteredDataGridMVVM.Helpers;

namespace MultiFilteredDataGridMVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string selectedAuthor;
        private string selectedTitle;
        private Nullable<int> selectedYear;
        private ObservableCollection<int> year;
        ObservableCollection<Thing> things;
        private bool removeTitleFilter;
        private bool removeAuthorFilter;
        private bool removeYearFilter; 
     
        public MainViewModel(IDataService dataService)
        {
            InitializeCommands();
            DataService = dataService;
            LoadData();

            //--------------------------------------------------------------
            // This 'registers' the instance of this view model to recieve messages with this type of token.  This 
            // is used to recieve a reference from the view that the collectionViewSource has been instantiated
            // and to recieve a reference to the CollectionViewSource which will be used in the view model for 
            // filtering
            Messenger.Default.Register<ViewCollectionViewSourceMessageToken>(this, Handle_ViewCollectionViewSourceMessageToken);
        }

        public override void Cleanup()
        {
            Messenger.Default.Unregister<ViewCollectionViewSourceMessageToken> (this);
            base.Cleanup();
        }

        /// Gets or sets the IDownloadDataService member
        internal IDataService DataService { get; set; }
        
        /// Gets or sets the CollectionViewSource which is the proxy for the 
        /// collection of Things and the datagrid in which each thing is displayed.        
        private CollectionViewSource CVS { get; set; }

        /// Gets or sets the primary collection of Thing objects to be displayed
        public ObservableCollection<Thing> Things
        {
            get { return things; }
            set
            {
                if (things == value)
                    return;
                things = value;
                RaisePropertyChanged("Things");
            }
        }

        // Filter properties =============

        /// Gets or sets the selected author in the list authors to filter the collection
        public string SelectedAuthor
        {
            get { return selectedAuthor; }
            set
            {
                if (String.Equals(selectedTitle, value))
                    return;
                selectedAuthor = value;
                RaisePropertyChanged("SelectedAuthor");
                ApplyFilter(!string.IsNullOrEmpty(selectedAuthor) ? FilterField.Author : FilterField.None);
            }
        }
        
        /// Gets or sets the selected title in the list title to filter the collection
        public string SelectedTitle
        {
            get { return selectedTitle; }
            set
            {
                if (String.Equals(selectedTitle, value))
                    return;
                selectedTitle = value;
                RaisePropertyChanged("SelectedTitle");
                ApplyFilter(!string.IsNullOrEmpty(SelectedTitle) ? FilterField.Title : FilterField.None);
            }
        }
        
        /// Gets or sets the selected year in the list years to filter the collection
        public Nullable<int> SelectedYear
        {
            get { return selectedYear; }
            set
            {
                if (selectedYear == value)
                    return;
                selectedYear = value;
                RaisePropertyChanged("SelectedYear");
                ApplyFilter(selectedYear.HasValue ? FilterField.Year : FilterField.None);
            }
        }

        public ObservableCollection<int> Years
        {
            get { return year; }
            set
            {
                if (year == value)
                    return;
                year = value;
                RaisePropertyChanged("Years");
            }
        }

        /// Gets or sets a flag indicating if the Title filter, if applied, can be removed.
        public bool CanRemoveTitleFilter
        {
            get { return removeTitleFilter; }
            set
            {
                removeTitleFilter = value;
                RaisePropertyChanged("CanRemoveTitleFilter");
            }
        }
        
        /// Gets or sets a flag indicating if the Author filter, if applied, can be removed.
        public bool CanRemoveAuthorFilter
        {
            get { return removeAuthorFilter; }
            set
            {
                removeAuthorFilter = value;
                RaisePropertyChanged("CanRemoveAuthorFilter");
            }
        }
        
        /// Gets or sets a flag indicating if the Year filter, if applied, can be removed.
        public bool CanRemoveYearFilter
        {
            get { return removeYearFilter; }
            set
            {
                removeYearFilter = value;
                RaisePropertyChanged("CanRemoveYearFilter");
            }
        }

        public ICommand SetFiltersCommand
        {
            get;
            private set;
        }

        private void InitializesetCommands()
        {
            SetFiltersCommand = new RelayCommand(SetFilters, null);
        }

        public ICommand ResetFiltersCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            ResetFiltersCommand = new RelayCommand(ResetFilters, null);
        }

        private void LoadData()
        {
            var things = DataService.GetThings();

            var q1 = from t in things
                    select t.Year;
            Years = new ObservableCollection<int>(q1.Distinct());

            Things = new ObservableCollection<Thing>(things);
        }

        /// This method handles a message recieved from the View which enables a reference to the
        /// instantiated CollectionViewSource to be used in the ViewModel.
        private void Handle_ViewCollectionViewSourceMessageToken(ViewCollectionViewSourceMessageToken token)
        {
            CVS = token.CVS;
        }

        public void ResetFilters()
        {
            // clear filters 
            RemoveYearFilter();
            RemoveAuthorFilter();
            RemoveTitleFilter();
        }
        public void RemoveTitleFilter()
        {
            CVS.Filter -= new FilterEventHandler(FilterByTitle);
            SelectedTitle = null;
            CanRemoveTitleFilter = false;
        }
        public void RemoveAuthorFilter()
        {
            CVS.Filter -= new FilterEventHandler(FilterByAuthor);
            SelectedAuthor = null;
            CanRemoveAuthorFilter = false;
        }
        public void RemoveYearFilter()
        {
            CVS.Filter -= new FilterEventHandler(FilterByYear);
            SelectedYear = null;
            CanRemoveYearFilter = false;
        }

        public void SetFilters()
        {
            AddTitleFilter();
            AddAuthorFilter();
            AddYearFilter();
        }
        public void AddTitleFilter()
        {
            if (CanRemoveTitleFilter)
            {
                CVS.Filter -= new FilterEventHandler(FilterByTitle);
                CVS.Filter += new FilterEventHandler(FilterByTitle);  
            }
            else
            {
                CVS.Filter += new FilterEventHandler(FilterByTitle);
                CanRemoveTitleFilter = true;
            }
        }
        public void AddAuthorFilter()
        {
            if (CanRemoveAuthorFilter)
            {
                CVS.Filter -= new FilterEventHandler(FilterByAuthor);
                CVS.Filter += new FilterEventHandler(FilterByAuthor);                
            }
            else
            {
                CVS.Filter += new FilterEventHandler(FilterByAuthor);
                CanRemoveAuthorFilter = true;
            }
        }
        public void AddYearFilter()
        {
            if (CanRemoveYearFilter)
            {
                CVS.Filter -= new FilterEventHandler(FilterByYear);
                CVS.Filter += new FilterEventHandler(FilterByYear);
            }
            else
            {
                CVS.Filter += new FilterEventHandler(FilterByYear);
                CanRemoveYearFilter = true;
            }
        }

        /* Notes on Filter Methods:
         * When using multiple filters, do not explicitly set anything to true.  Rather,
         * only hide things which do not match the filter criteria
         * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
         * clears out any previous filters applied to it.  
         */

        private void FilterByTitle(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as Thing;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedTitle, src.Title) != 0)
                e.Accepted = false;
        }
        private void FilterByAuthor(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as Thing;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedAuthor, src.Author) != 0)
                e.Accepted = false;
        }
        private void FilterByYear(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as Thing;
            if (src == null)
                e.Accepted = false;
            else if (SelectedYear != src.Year)
                e.Accepted = false;
        }

        private enum FilterField
        {
            Author,
            Title,
            Year,
            None
        }
        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.Author:
                    AddAuthorFilter();
                    break;
                case FilterField.Title:
                    AddTitleFilter();
                    break;
                case FilterField.Year:
                    AddYearFilter();
                    break;
                default:
                    break;
            }
        }

    }
}
