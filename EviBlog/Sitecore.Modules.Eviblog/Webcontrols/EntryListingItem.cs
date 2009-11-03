using System;
using System.Web.UI;

namespace Sitecore.Modules.Eviblog.Webcontrols
{
    public class EntryListingItem : Control, INamingContainer, IDataItemContainer
    {
        internal EntryListingItem(object dataItem, int itemIndex)
        {
            this._dataItem = dataItem;
            this._dataItemIndex = _dataItemIndex;
            this._displayIndex = _displayIndex;
        }

        #region DataItem

        private object _dataItem;

        public object DataItem
        {
            get
            {
                return this._dataItem;
            }
        }

        #endregion DataItem

        #region DataItemIndex

        private int _dataItemIndex;

        public int DataItemIndex
        {
            get
            {
                return this._dataItemIndex;
            }
        }

        #endregion DataItemIndex

        #region DisplayIndex

        private int _displayIndex;

        public int DisplayIndex
        {
            get
            {
                return this._displayIndex;
            }
        }

        #endregion DisplayIndex
    }
}