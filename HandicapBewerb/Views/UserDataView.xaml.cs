using System;
using System.Windows.Controls;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;
using MahApps.Metro.Controls;

namespace TournamentManager.Views
{
    /// <summary>
    /// Interaction logic for UserDataView.xaml
    /// </summary>
    public partial class UserDataView : UserControl
    {
        public UserDataView()
        {
            InitializeComponent();
        }

        private void DataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                Mediator.NotifyColleagues(MediatorGlobal.PlayerEditCancel, null);
                return;
            }

            if (e.EditAction == DataGridEditAction.Commit)
            {
                double oldValue;
                double newValue;
                try
                {
                    oldValue = ((Round)((NumericUpDown)e.EditingElement).DataContext).Points;

                    var value = ((NumericUpDown) e.EditingElement).Value;
                    if (value != null)
                        newValue = (double) value;
                    else
                    {
                        newValue = Double.NaN;
                    }
                }
                catch (Exception)
                {
                    Mediator.NotifyColleagues(MediatorGlobal.PlayerEditCancel, null);
                    return;
                }

                if (oldValue.Equals(newValue))
                {
                    Mediator.NotifyColleagues(MediatorGlobal.PlayerEditCancel, null);
                    return;
                }

                Mediator.NotifyColleagues(MediatorGlobal.PlayerEditComit, null);
            }
        }

        private void DataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            Mediator.NotifyColleagues(MediatorGlobal.PlayerOnBegininningEdit, null);
        }

        private void DataGrid_UserOnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                return;
            }

            if (e.EditAction == DataGridEditAction.Commit)
            {
                string oldValue;
                string newValue;
                try
                {
                    oldValue = ((User)((TextBox)e.EditingElement).DataContext).Name;
                    newValue = ((TextBox)e.EditingElement).Text;

                    if (newValue == null || oldValue == null)
                    {
                        throw new Exception();
                    }

                }
                catch (Exception)
                {
                    return;
                }

                if (oldValue.Equals(newValue))
                {
                    return;
                }

                var userId = ((User)((TextBox)e.EditingElement).DataContext).UserId;
                Mediator.NotifyColleagues(MediatorGlobal.PlayerEditUserComit, userId);
            }
        }
    }
}
