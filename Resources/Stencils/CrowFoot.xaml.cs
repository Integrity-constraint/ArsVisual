using ArsVisual.Helpers;
using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArsVisual.Resources.Stencils
{
    public class CrowFoot : DesignerItem
    {
        public string TableName { get; set; } = "New Table"; // Добавьте это свойство
        public ObservableCollection<TableField> Fields { get; } = new ObservableCollection<TableField>();

        public CrowFoot()
        {
            // Инициализация таблицы с одним полем по умолчанию
            Fields.Add(new TableField());
        }

        public ICommand AddFieldCommand => new RelayCommand(_ => Fields.Add(new TableField()));

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TableField)))
            {
                Fields.Add((TableField)e.Data.GetData(typeof(TableField)));
            }
        }
    }

    public class TableField
    {
        public string Name { get; set; } = "New Field";
        public string Type { get; set; } = "varchar";
        public bool IsPrimaryKey { get; set; }
    }

}
