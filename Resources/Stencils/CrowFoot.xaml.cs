using ArsVisual.Helpers;
using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArsVisual.Resources.Stencils
{
    public class CrowFoot : DesignerItem, INotifyPropertyChanged
    {
        private string tableName = "New Table";
        public string TableName
        {
            get => tableName;
            set
            {
                if (tableName != value)
                {
                    tableName = value;
                    OnPropertyChanged(nameof(TableName));
                }
            }
        }

        public ObservableCollection<TableField> Fields { get; } = new ObservableCollection<TableField>();

        public CrowFoot()
        {
            Fields.Add(new TableField());
        }

        private ICommand addFieldCommand;
        public ICommand AddFieldCommand => addFieldCommand ??= new RelayCommand(_ => Fields.Add(new TableField()));

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class TableField : INotifyPropertyChanged
    {
        private string name = "New Field";
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string type = "varchar";
        public string Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        private bool isPrimaryKey;
        public bool IsPrimaryKey
        {
            get => isPrimaryKey;
            set
            {
                if (isPrimaryKey != value)
                {
                    isPrimaryKey = value;
                    OnPropertyChanged(nameof(IsPrimaryKey));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
