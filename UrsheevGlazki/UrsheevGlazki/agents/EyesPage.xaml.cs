using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrsheevGlazki
{
    /// <summary>
    /// Логика взаимодействия для EyesPage.xaml
    /// </summary>
    public partial class EyesPage : Page
    {
        private int CountRecords;
        private int CountPage;
        private int CurrentPage = 0;

        private List<Agent> CurrentPageList = new List<Agent>();
        private List<Agent> TableList;

        private Regex regex = new Regex(@"\D");

        public EyesPage()
        {

            InitializeComponent();
            Sorting.SelectedIndex = 0;
            Types.SelectedIndex = 0;
            var currentDBList = UrsheevGlazkiEntities.GetContext().Agent.ToList();
            EyesListView.ItemsSource = currentDBList;
            UpdateServices();

        }

        public void UpdateServices()
        {
            var currentDBList = UrsheevGlazkiEntities.GetContext().Agent.ToList();
            var AgentTypesDBList = UrsheevGlazkiEntities.GetContext().AgentType.ToList();


            if (Types.SelectedIndex > 0)
            {
                var CurrentType = (TextBlock)Types.SelectedValue;
                string CurrentTypeContent = CurrentType.Text;
                currentDBList = currentDBList.Where(p => AgentTypesDBList[p.AgentTypeID - 1].Title == CurrentTypeContent).ToList();
            }

            switch (Sorting.SelectedIndex)
            {
                case 1:
                    currentDBList = currentDBList = currentDBList.OrderBy(p => p.Title).ToList();
                    break;
                case 2:
                    currentDBList = currentDBList = currentDBList.OrderByDescending(p => p.Title).ToList();
                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:
                    currentDBList = currentDBList.OrderBy(p => p.Priority).ToList();
                    break;
                case 6:
                    currentDBList = currentDBList.OrderByDescending(p => p.Priority).ToList();
                    break;
            }
            currentDBList = currentDBList.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || regex.Replace(p.Phone, "").Contains(TBoxSearch.Text)).ToList();



            TableList = currentDBList;

            SelectedItemsUpdate();
            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage) //direction - 1: left, 2: right
        {
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            if (direction == 1 && CurrentPage - 1 < 0 || direction == 2 && CurrentPage + 1 >= CountPage)
                return;

            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        CurrentPage--;
                        min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                        for (int i = CurrentPage * 10; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                        break;

                    case 2:
                        CurrentPage++;
                        min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                        for (int i = CurrentPage * 10; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                        break;
                }
            }
            PageListBox.Items.Clear();

            for (int i = 1; i <= CountPage; i++)
            {
                PageListBox.Items.Add(i);
            }
            PageListBox.SelectedIndex = CurrentPage;

            EyesListView.ItemsSource = CurrentPageList;

            var productSale = Rafikov_eyesEntities.GetContext().ProductSale.ToList();

            foreach (Agent item in EyesListView.ItemsSource)
            {
                var currentProductSale = productSale.Where(p => p.AgentID == item.ID).ToList();
                if (currentProductSale.Count > 0)
                {
                    decimal TotalCost = 0;
                    foreach (ProductSale productSaleItem in currentProductSale)
                    {
                        TotalCost += productSaleItem.Product.MinCostForAgent * productSaleItem.ProductCount;
                    }

                    if (TotalCost >= 10_000 && TotalCost <= 50_000)
                        item.Discount = 5;

                    else if (TotalCost >= 50_000 && TotalCost <= 150_000)
                        item.Discount = 10;

                    else if (TotalCost >= 150_000 && TotalCost <= 500_000)
                        item.Discount = 20;

                    else if (TotalCost >= 500_000)
                        item.Discount = 25;

                }
            }

            EyesListView.Items.Refresh();

            TBCount.Text = (CurrentPage + 1).ToString();
            TBRecords.Text = CountPage.ToString();
        }


        private void SelectedItemsUpdate()
        {
            if (EyesListView.SelectedItems.Count > 0)
            {
                ChangePriorityButton.Visibility = Visibility.Visible;
            }
            else
            {
                ChangePriorityButton.Visibility = Visibility.Hidden;
            }
        }


        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
            
        }

        private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
            
        }

        private void Types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
           
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                UrsheevGlazkiEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                UpdateServices();
                UpdateServices();
            }
        }

        private void EyesListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedItemsUpdate();
        }

        private void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
        {
            IList selectedRows = (IList)EyesListView.SelectedItems;
            PriorityWindow priorityWindow = new PriorityWindow(selectedRows.Cast<Agent>().ToList(), this);

            priorityWindow.ShowDialog();
        }
    }
}   
