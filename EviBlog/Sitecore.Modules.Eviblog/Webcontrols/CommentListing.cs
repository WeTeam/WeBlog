using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Web.UI.WebControls;
using Sitecore.Collections;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Webcontrols;

namespace Sitecore.Modules.Eviblog.Webcontrols
{
    public class CommentListing : DataBoundControl, INamingContainer
	{
		#region Events

		public event EventHandler<CommentListingItemCreatedEventArgs> ItemCreated;

		#endregion Events

		#region Properties

		#region DataKeyNames

		private string[] _dataKeyNames;

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] DataKeyNames
		{
			get
			{
				if (this._dataKeyNames != null)
				{
					return (string[])this._dataKeyNames.Clone();
				}
				else
				{
					return new string[0];
				}
			}

			set
			{
				if (value != null)
				{
					this._dataKeyNames = (string[])value.Clone();
				}
				else
				{
					this._dataKeyNames = null;
				}

				this._dataKeysArrayList = null;
			}
		}

		#endregion DataKeyNames

		#region DataKeys

		private DataKeyArray _dataKeys;

		public DataKeyArray DataKeys
		{
			get
			{
				if (this._dataKeys == null)
				{
					this._dataKeys = new DataKeyArray(this.DataKeysArrayList);
				}

				return this._dataKeys;
			}
		}

		#endregion DataKeys

		#region DataKeysArrayList

		private ArrayList _dataKeysArrayList;

		private ArrayList DataKeysArrayList
		{
			get
			{
				if (this._dataKeysArrayList == null)
				{
					this._dataKeysArrayList = new ArrayList();
				}

				return this._dataKeysArrayList;
			}
		}

		#endregion DataKeysArrayList

		#region ItemContainerID

		private string _itemContainerID = "itemPlaceholder";

		public string ItemContainerID
		{
			get
			{
				return this._itemContainerID;
			}

			set
			{
				this._itemContainerID = value;
			}
		}

		#endregion ItemContainerID

		#region Items

		private IList<CommentListingItem> _items;

		public IList<CommentListingItem> Items
		{
			get
			{
				return this._items;
			}
		}

		#endregion Items

		#region ItemTemplate

		private ITemplate _itemTemplate;

		[PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(CommentListingItem), BindingDirection.TwoWay), Browsable(false)]
		public ITemplate ItemTemplate
		{
			get
			{
				return this._itemTemplate;
			}

			set
			{
				this._itemTemplate = value;
			}
		}

		#endregion ItemTemplate

		#region LayoutTemplate

		private ITemplate _layoutTemplate;

		[PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(CommentListing))]
		public ITemplate LayoutTemplate
		{
			get
			{
				return this._layoutTemplate;
			}

			set
			{
				this._layoutTemplate = value;
			}
		}

		#endregion LayoutTemplate

		#region SourceItemsCount

		private int SourceItemsCount
		{
			get
			{
				return (this.ViewState["_!ItemCount"] is int)
					? (int)this.ViewState["_!ItemCount"] : -1;
			}

			set
			{
				this.ViewState["_!ItemCount"] = value;
			}
		}

		#endregion SourceItemsCount

        public bool ShowRecentComments { get; set; }

		#endregion Properties

		#region Methods

		#region DataBind

		public override void DataBind()
		{
			base.DataBind();

			this.DataBind(true);
		}

		private new void DataBind(bool isReal)
		{
			this.Controls.Clear();

            // Check for the presence of an ItemTemplate
            if (this._itemTemplate != null)
            {
                this._layoutTemplate.InstantiateIn(this);

                Control container = this.FindControl(this._itemContainerID);

                List<CommentListingItem> items = new List<CommentListingItem>();

                List<Item> itemList = new List<Item>();

                foreach (object item in (IEnumerable)this.DataSource)
                {
                    CommentListingItem currentItem = new CommentListingItem(item, items.Count);
                    items.Add(currentItem);
                    itemList.Add((Item)item);

                    if (isReal && this.DataKeyNames.Length > 0)
                    {
                        OrderedDictionary keyTable = new OrderedDictionary(this.DataKeyNames.Length);

                        foreach (string dataKeyName in this.DataKeyNames)
                        {
                            object propertyValue = DataBinder.GetPropertyValue(item, dataKeyName);

                            keyTable.Add(dataKeyName, propertyValue);
                        }

                        this.DataKeysArrayList.Add(new DataKey(keyTable, this.DataKeyNames));
                    }
                }

                this._items = items.ToArray();

                if (isReal)
                {
                    // This will be stored in the ViewState so the control will
                    // rebuild itself on the next post back.
                    this.SourceItemsCount = this._items.Count;
                }

                foreach (CommentListingItem currentItem in this._items)
                {
                    this._itemTemplate.InstantiateIn(currentItem);

                    int ii = 0;

                    foreach (CommentListingItem lvItem in this.Items)
                    {
                        if (lvItem.Controls.Count > 0)
                        {
                            foreach (Control control in lvItem.Controls)
                            {
                                if (control is CommentTitle)
                                {
                                    CommentTitle ctrl = (CommentTitle)control;
                                    ctrl.Item = itemList[ii];
                                }
                                else if (control is CommentText)
                                {
                                    CommentText ctrl = (CommentText)control;
                                    ctrl.Item = itemList[ii];
                                }
                            }
                        }
                        ii++;
                    }

                    if (this.ItemCreated != null)
                    {
                        this.ItemCreated(this, new CommentListingItemCreatedEventArgs(currentItem));
                    }

                    container.Controls.Add(currentItem);

                    if (isReal)
                    {
                        currentItem.DataBind();
                    }
                }
            }
            else
            {
                CreateDefaultTemplate();
            }
		}

        private void CreateDefaultTemplate()
        {
            List<Item> commentsList = GetComments();

            if (ShowRecentComments == true)
            {
                this.Controls.Add(new LiteralControl("<ul>"));

                foreach (Item item in commentsList)
                {
                    string comment = item.Fields["Comment"].Value;

                    if (comment.Length > 25)
                        comment = comment.Substring(0, 25);

                    this.Controls.Add(new LiteralControl("<li>"));
                    
                    HyperLink link = new HyperLink();
                    link.NavigateUrl = LinkManager.GetItemUrl(item.Parent);
                    link.Text = string.Format("{0}...", comment);
                    this.Controls.Add(link);
                    
                    this.Controls.Add(new LiteralControl("</li>"));
                }

                this.Controls.Add(new LiteralControl("</ul>"));
            }
            else
            {
                foreach (Item item in commentsList)
                {
                    this.Controls.Add(new LiteralControl("<h3>"));
                    if (!string.IsNullOrEmpty(item.Fields["Website"].Value))
                    {
                        HyperLink link = new HyperLink();
                        link.Text = item.Fields["Name"].Value;
                        link.NavigateUrl = item.Fields["Website"].Value;
                        this.Controls.Add(link);
                    }
                    else
                    {
                        this.Controls.Add(new LiteralControl(item.Fields["Name"].Value));
                    }
                    this.Controls.Add(new LiteralControl("</h3>"));

                    this.Controls.Add(new LiteralControl("<span class=\"date\">"));
                    this.Controls.Add(new LiteralControl(item.Fields["Comment"].Value));
                    this.Controls.Add(new LiteralControl("</span>"));

                }
            }
        }
		#endregion DataBind

		#region LoadControlState

		protected override void LoadControlState(object savedState)
		{
			if (savedState is object[])
			{
				object[] savedStateItems = (object[])savedState;

				base.LoadControlState(savedStateItems[0]);

				this._dataKeyNames = (string[])savedStateItems[1];

				object[] serializedDataKeys = (object[])savedStateItems[2];

				for (int i = 0; i < serializedDataKeys.Length; i++)
				{
					this.DataKeysArrayList.Add(new DataKey(new OrderedDictionary(this.DataKeyNames.Length), this.DataKeyNames));

					((IStateManager)this.DataKeysArrayList[i]).LoadViewState(serializedDataKeys[i]);
				}
			}
			else
			{
				base.LoadControlState(savedState);
			}
		}

		#endregion LoadControlState

		#region LoadViewState

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			if (this.DataSource == null && string.IsNullOrEmpty(this.DataSourceID) && this.SourceItemsCount > -1)
			{
				this.DataSource = new object[this.SourceItemsCount];

				this.DataBind(false);
			}
		}

		#endregion LoadViewState

		#region OnInit

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.Page.RegisterRequiresControlState(this);
		}

		#endregion OnInit

		#region SaveControlState

		protected override object SaveControlState()
		{
			if (this._dataKeysArrayList != null && this._dataKeysArrayList.Count > 0)
			{
				object[] serializedDataKeys = new object[this._dataKeysArrayList.Count];

				for (int i = 0; i < this._dataKeysArrayList.Count; i++)
				{
					serializedDataKeys[i] = ((IStateManager)this.DataKeysArrayList[i]).SaveViewState();
				}

				return new object[]{
				   base.SaveControlState(),
				   this._dataKeyNames,
				   serializedDataKeys
			   };
			}
			else
			{
				return base.SaveControlState();
			}
		}

		#endregion SaveControlState

        protected override void CreateChildControls()
        {
            Controls.Clear();

            Items.Entry currentEntry = new Items.Entry(Sitecore.Context.Item);

            if (!currentEntry.DisableComments)
            {
                this.DataSource = GetComments();
                this.DataBind();

                base.CreateChildControls();
            }
        }

        private List<Item> GetComments()
        {
            Blog currentBlog = BlogManager.GetCurrentBlog();
            int TotalToShow = currentBlog.DisplayCommentSidebarCount == 0 ? 5 : currentBlog.DisplayCommentSidebarCount;
            List<Item> listComments = new List<Item>();

            listComments = null;

            if (ShowRecentComments == true)
            {
                listComments = CommentManager.GetCommentItemsByBlog(currentBlog.ID, 5);
            }
            else
            {
                listComments = CommentManager.GetEntryCommentsAsItems(Sitecore.Context.Item);
            }

            return listComments;
        }

		#endregion Methods
	}

	public class CommentListingItemCreatedEventArgs : EventArgs
	{
		public CommentListingItemCreatedEventArgs(CommentListingItem item)
		{
			this._item = item;
		}

		private CommentListingItem _item;

		public CommentListingItem Item
		{
			get
			{
				return this._item;
			}
		}
	}
}
